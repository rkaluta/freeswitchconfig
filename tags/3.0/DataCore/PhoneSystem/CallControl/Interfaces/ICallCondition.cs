using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl.Interfaces
{
    public struct sPreActionsElement
    {
        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private NameValuePair[] _elementAttributes;
        public NameValuePair[] ElementAttributes
        {
            get { return _elementAttributes; }
        }

        public sPreActionsElement(string name, NameValuePair[] attributes)
        {
            _name = name;
            _elementAttributes = attributes;
        }
    }

    public interface ICallCondition
    {
        NameValuePair[] ConditionTagAttributes { get; }
        sPreActionsElement[] PreActionElements { get; }
        ICallAction[] Actions { get; }
        ICallAction[] AntiActions { get; }
        CallConditionBreakTypes? Break { get; }
    }
}
