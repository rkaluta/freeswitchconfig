using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Stringtemplate.Interfaces;
using System.Text.RegularExpressions;
using Org.Reddragonit.Stringtemplate.Tokenizers;
using System.Reflection;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.Stringtemplate.Outputs;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Generators
{
    public class ConstructComponent : IComponent
    {
        private static readonly Regex regMatch = new Regex("^(c|C)(o|O)(n|N)(s|S)(t|T)(r|R)(u|U)(c|C)(t|T)\\(\\\"(.+)\\\",([^, ]+)(,((@.+)=(.+)))*\\)$", RegexOptions.Compiled | RegexOptions.ECMAScript);

        private string _type;
        private string _varName;
        private Dictionary<string, List<IComponent>> _pars;

        #region IComponent Members

        bool IComponent.CanLoad(Token token)
        {
            return regMatch.IsMatch(token.Content);
        }

        void IComponent.Append(ref Dictionary<string, object> variables,IOutputWriter writer)
        {
            object var = null;
            try
            {
                Type t = Type.GetType(_type, true);
                ConstructorInfo ci = null;
                foreach (ConstructorInfo c in t.GetConstructors())
                {
                    if ((_pars.Count == 0) && (c.GetParameters().Length == 0))
                    {
                        ci = c;
                        break;
                    }
                    else
                    {
                        bool isOkay = c.GetParameters().Length==_pars.Count;
                        foreach (ParameterInfo pi in c.GetParameters())
                        {
                            if (!_pars.ContainsKey("@" + pi.Name))
                            {
                                isOkay = false;
                                break;
                            }
                        }
                        if (isOkay)
                        {
                            ci = c;
                            break;
                        }
                    }
                }
                if (ci != null)
                {
                    object[] pars = new object[_pars.Count];
                    StringOutputWriter swo = new StringOutputWriter();
                    for (int x = 0; x < pars.Length; x++)
                    {
                        ParameterInfo pi = ci.GetParameters()[x];
                        string tmp = "";
                        foreach (IComponent ic in _pars["@" + pi.Name])
                        {
                            swo.Clear();
                            ic.Append(ref variables, swo);
                            tmp += swo.ToString();
                        }
                        pars[x] = Convert.ChangeType(tmp, pi.ParameterType);
                    }
                    var = ci.Invoke(pars);
                }
            }
            catch (Exception e)
            {
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                Log.Error(e);
            }
            if (variables.ContainsKey(_varName))
                variables.Remove(_varName);
            variables.Add(_varName, var);
        }

        bool IComponent.Load(Queue<Token> tokens, Type tokenizerType, Org.Reddragonit.Stringtemplate.TemplateGroup group)
        {
            try
            {
                Token t = tokens.Dequeue();
                Match m = regMatch.Match(t.Content);
                _type = m.Groups[10].Value.Trim();
                _varName = m.Groups[11].Value.Trim();
                _pars = new Dictionary<string, List<IComponent>>();
                for (int x = 14; x < m.Groups.Count; x += 4)
                {
                    Tokenizer tok = (Tokenizer)tokenizerType.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { m.Groups[x + 1].Value.Trim() });
                    _pars.Add(m.Groups[x].Value.Trim(), tok.TokenizeStream(group));
                }
                return true;
            }
            catch (Exception e)
            {
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                Log.Error(e);
                return false;
            }
        }

        IComponent IComponent.NewInstance()
        {
            return new ConstructComponent();
        }

        #endregion
    }
}
