using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Stringtemplate.Interfaces;
using System.Text.RegularExpressions;
using Org.Reddragonit.Stringtemplate.Tokenizers;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using System.Data;
using System.Collections;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.Stringtemplate.Outputs;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.Generators
{
    public class ClassQueryComponent : IComponent
    {

        private static readonly Regex regMatch = new Regex("^((C|c)(L|l)(a|A)(s|S){2}(q|Q)(u|U)(e|E)(r|R)(y|Y))\\(\\\"(.+)\\\",\\\"(.+)\\\",([^, ]+)(,((@.+)=(.+)))*\\)$",
            RegexOptions.Compiled | RegexOptions.ECMAScript);

        private string _namespace;
        private string _query;
        private string _variableName;
        private Dictionary<string, List<IComponent>> _parameters;

        #region IComponent Members

        bool IComponent.CanLoad(Org.Reddragonit.Stringtemplate.Tokenizers.Token token)
        {
            return regMatch.IsMatch(token.Content);
        }

        void IComponent.Append(ref Dictionary<string, object> variables, IOutputWriter writer)
        {
            ClassQuery cq = new ClassQuery(_namespace, _query);
            List<IDbDataParameter> pars = new List<IDbDataParameter>();
            StringOutputWriter swo = new StringOutputWriter();
            foreach (string str in _parameters.Keys)
            {
                string par = "";
                foreach (IComponent ic in _parameters[str])
                {
                    swo.Clear();
                    ic.Append(ref variables, swo);
                    par += swo.ToString();
                }
                pars.Add(cq.CreateParameter(str, par));
            }
            cq.Execute(pars.ToArray());
            object var = new ArrayList();
            while (cq.Read())
            {
                Hashtable ht = new Hashtable();
                for (int x = 0; x < cq.FieldCount; x++)
                {
                    if (!cq.IsDBNull(x))
                    {
                        ht.Add(cq.GetName(x), cq[x]);
                    }
                }
                ((ArrayList)var).Add(ht);
            }
            cq.Close();
            if (((ArrayList)var).Count == 1 && ((Hashtable)((ArrayList)var)[0]).Count == 1)
            {
                Hashtable tmp = (Hashtable)((ArrayList)var)[0];
                var = tmp;
                object[] objAr = new object[1];
                ((Hashtable)var).Values.CopyTo(objAr, 0);
                var = objAr[0];
            }
            if (variables.ContainsKey(_variableName))
                variables.Remove(_variableName);
            variables.Add(_variableName, var);
        }

        bool IComponent.Load(Queue<Org.Reddragonit.Stringtemplate.Tokenizers.Token> tokens, Type tokenizerType, Org.Reddragonit.Stringtemplate.TemplateGroup group)
        {
            try
            {
                Token t = tokens.Dequeue();
                Match m = regMatch.Match(t.Content);
                _namespace = m.Groups[11].Value.Trim();
                _query = m.Groups[12].Value.Trim();
                _variableName = m.Groups[13].Value.Trim();
                _parameters = new Dictionary<string, List<IComponent>>();
                for (int y = 16; y < m.Groups.Count; y += 4)
                {
                    Tokenizer tok = (Tokenizer)tokenizerType.GetConstructor(new Type[]{typeof(string)}).Invoke(new object[] { m.Groups[y + 1].Value.Trim() });
                    _parameters.Add(m.Groups[y].Value.Trim(), tok.TokenizeStream(group));
                }
            }
            catch (Exception e)
            {
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                Log.Error(e);
                return false;
            }
            return true;
        }

        IComponent IComponent.NewInstance()
        {
            return new ClassQueryComponent();
        }

        #endregion
    }
}
