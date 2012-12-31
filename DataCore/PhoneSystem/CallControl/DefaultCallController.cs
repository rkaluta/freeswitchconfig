using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl
{
    public class DefaultCallController : ICallController
    {
        private const string _UNLOOP_EXTENSION = @"<extension name=""unloop"">
			<condition field=""${unroll_loops}"" expression=""^true\$"" />
			<condition field=""${sip_looped_call}"" expression=""^true\$"">
				<action application=""deflect"" data=""${destination_number}"" />
			</condition>
		</extension>";

        private Dictionary<string, List<XmlContextFile>> _files;
        private List<string> _plans;

        #region ICallController Members

        public void Start(ADialPlan[] plans)
        {
            _files = new Dictionary<string, List<XmlContextFile>>();
            _plans = new List<string>();
            foreach (ADialPlan plan in plans)
            {
                _plans.Add(plan.Name);
                foreach (sCallContext cont in plan.CallContexts)
                {
                    if (!_files.ContainsKey(cont.Name))
                        _files.Add(cont.Name, new List<XmlContextFile>());
                    List<XmlContextFile> tfiles = _files[cont.Name];
                    XmlContextFile file = new XmlContextFile(plan.Name);
                    tfiles.Add(file);
                    _files.Remove(cont.Name);
                    _files.Add(cont.Name, tfiles);
                    foreach (sCallExtension ext in cont.Extensions)
                        WriteCallExtension(ext, file);
                }
            }
        }

        private void WriteCallExtension(sCallExtension ext, XmlContextFile file)
        {
            file.WriteStartElement("extension");
            file.WriteStartAttribute("name");
            file.WriteValue(ext.Name);
            file.WriteEndAttribute();
            if (ext.Continue)
            {
                file.WriteStartAttribute("continue");
                file.WriteValue("true");
                file.WriteEndAttribute();
            }
            if (ext.Inline)
            {
                file.WriteStartAttribute("inline");
                file.WriteValue("true");
                file.WriteEndAttribute();
            }
            foreach (ICallCondition cond in ext.Conditions)
            {
                file.WriteStartElement("condition");
                _AppendElementAttributes(file, cond.ConditionTagAttributes);
                if (cond.Break.HasValue)
                {
                    file.WriteStartAttribute("break");
                    switch (cond.Break.Value)
                    {
                        case CallConditionBreakTypes.Never:
                            file.WriteValue("never");
                            break;
                        case CallConditionBreakTypes.OnFalse:
                            file.WriteValue("on-false");
                            break;
                        case CallConditionBreakTypes.OnTrue:
                            file.WriteValue("on-true");
                            break;
                    }
                    file.WriteEndAttribute();
                }
                if (cond.PreActionElements!=null)
                {
                    foreach (sPreActionsElement pae in cond.PreActionElements)
                    {
                        file.WriteStartElement(pae.Name);
                        _AppendElementAttributes(file, pae.ElementAttributes);
                        file.WriteEndElement();
                    }
                }
                if (cond.Actions != null)
                {
                    foreach (ICallAction act in cond.Actions)
                    {
                        file.WriteStartElement("action");
                        _AppendElementAttributes(file, act.ActionXMLAttributes);
                        file.WriteEndElement();
                    }
                }
                if (cond.AntiActions != null)
                {
                    foreach (ICallAction act in cond.AntiActions)
                    {
                        file.WriteStartElement("anti-action");
                        _AppendElementAttributes(file, act.ActionXMLAttributes);
                        file.WriteEndElement();
                    }
                }
                file.WriteEndElement();
            }
            file.WriteEndElement();
        }

        private void _AppendElementAttributes(XmlContextFile file, NameValuePair[] attributes)
        {
            if (attributes != null)
            {
                foreach (NameValuePair nvp in attributes)
                {
                    file.WriteStartAttribute(nvp.Name);
                    file.WriteValue(nvp.Value);
                    file.WriteEndAttribute();
                }
            }
        }

        public void Stop()
        {
            _files.Clear();
        }

        public void DialplanUpdated(ADialPlan plan)
        {
            lock (_files)
            {
                string[] keys = new string[_files.Count];
                _files.Keys.CopyTo(keys, 0);
                foreach (string key in keys)
                {
                    List<XmlContextFile> files = _files[key];
                    for (int x = 0; x < files.Count; x++)
                    {
                        if (files[x].FileName == plan.Name)
                        {
                            files.RemoveAt(x);
                            break;
                        }
                    }
                    _files.Remove(key);
                    _files.Add(key, files);
                }
                foreach (sCallContext cont in plan.CallContexts)
                {
                    if (!_files.ContainsKey(cont.Name))
                        _files.Add(cont.Name, new List<XmlContextFile>());
                    List<XmlContextFile> tfiles = _files[cont.Name];
                    XmlContextFile file = new XmlContextFile(plan.Name);
                    for (int x = 0; x < tfiles.Count; x++)
                    {
                        if (_plans.IndexOf(tfiles[x].FileName) > _plans.IndexOf(plan.Name))
                        {
                            tfiles.Insert(x, file);
                            break;
                        }
                    }
                    _files.Remove(cont.Name);
                    _files.Add(cont.Name, tfiles);
                    foreach (sCallExtension ext in cont.Extensions)
                        WriteCallExtension(ext, file);
                }
            }
        }

        public void DialplanDeployed(ADialPlan plan, int index)
        {
            lock (_files)
            {
                _plans.Insert(index, plan.Name);
                foreach (sCallContext cont in plan.CallContexts)
                {
                    if (!_files.ContainsKey(cont.Name))
                        _files.Add(cont.Name, new List<XmlContextFile>());
                    List<XmlContextFile> tfiles = _files[cont.Name];
                    XmlContextFile file = new XmlContextFile(plan.Name);
                    for (int x = 0; x < tfiles.Count; x++)
                    {
                        if (_plans.IndexOf(tfiles[x].FileName) > _plans.IndexOf(plan.Name))
                        {
                            tfiles.Insert(x, file);
                            break;
                        }
                    }
                    _files.Remove(cont.Name);
                    _files.Add(cont.Name, tfiles);
                    foreach (sCallExtension ext in cont.Extensions)
                        WriteCallExtension(ext, file);
                }
            }
        }

        public void DialplanUnDeployed(ADialPlan plan)
        {
            lock (_files)
            {
                string[] keys = new string[_files.Count];
                _files.Keys.CopyTo(keys, 0);
                foreach (string key in keys)
                {
                    List<XmlContextFile> files = _files[key];
                    for (int x = 0; x < files.Count; x++)
                    {
                        if (files[x].FileName == plan.Name)
                        {
                            files.RemoveAt(x);
                            break;
                        }
                    }
                    _files.Remove(key);
                    _files.Add(key, files);
                }
            }
        }

        public string Name
        {
            get { return "Default Configuration File Based Call Control"; }
        }

        public string Description
        {
            get { return "This is the default call control method, it writes out the configuration files in the standard freeswitch paths."; }
        }

        public XmlContextFile GenerateContextFile(sDeployedContext context)
        {
            XmlContextFile ret = new XmlContextFile(context.Name);
            ret.WriteStartElement("context");
            ret.WriteStartAttribute("name");
            ret.WriteValue(context.Name);
            ret.WriteEndAttribute();
            ret.WriteRaw(_UNLOOP_EXTENSION);
            lock (_files)
            {
                if (_files.ContainsKey(context.Name)){
                    foreach (XmlContextFile xcf in _files[context.Name])
                        ret.AddInclude(xcf);
                }
            }
            ret.WriteEndElement();
            return ret;
        }

        #endregion
    }
}
