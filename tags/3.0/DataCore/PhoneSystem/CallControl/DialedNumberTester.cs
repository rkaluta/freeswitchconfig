using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl
{
    public class DialedNumberTester
    {
        private StringBuilder _results;
        private Extension _origin;
        public Extension Origin
        {
            get { return _origin; }
        }
        private string _dialedNumber;
        public string DialedNumber
        {
            get { return _dialedNumber; }
        }
        private string _pin;
        public string Pin
        {
            get { return _pin; }
        }
        private bool _hungUp;
        public bool HungUp
        {
            get { return _hungUp; }
        }
        private DateTime _currentTime;
        public DateTime CurrentTime
        {
            get { return _currentTime; }
        }
        private string _domain;
        public string Domain {
            get { return _domain; }
        }
        private string _context;
        public string Context
        {
            get { return _context; }
        }

        internal DialedNumberTester(Extension origin,string dialedNumber,string pin,DateTime? currentTime)
        {
            _origin = origin;
            _context = origin.Context.Name;
            _domain = origin.Domain.Name;
            _dialedNumber = dialedNumber;
            _pin = pin;
            _results = new StringBuilder();
            _hungUp = false;
            if (currentTime == null)
                _currentTime = DateTime.Now;
            else
                _currentTime = currentTime.Value;
        }

        public override string ToString()
        {
            return _results.ToString();
        }

        public void TransferNumber(string newNumber)
        {
            _dialedNumber = newNumber;
            _results.AppendLine("Transfered to " + newNumber);
        }

        public void ChangeDestinationNumber(string newNumber)
        {
            _dialedNumber = newNumber;
            _results.AppendLine("Changed destination number to " + newNumber);
        }

        public void ChangePin(string newPin)
        {
            _pin = newPin;
            _results.AppendLine("Changed pin to " + newPin);
        }

        public void Hangup(string note)
        {
            if (note != null)
                _results.AppendLine(note);
            _hungUp = true;
            _results.AppendLine("Hung up Call");
        }

        public void Hangup()
        {
            Hangup(null);
        }

        public void PlayFile(string fileName)
        {
            _results.AppendLine("Play audio file " + fileName);
        }

        public void Continue()
        {
            Continue(null);
        }

        public void Continue(string additional)
        {
            if (additional != null)
                _results.AppendLine(additional);
        }

        public void ChangeOrigin(Extension newOrigin)
        {
            _origin = newOrigin;
            _results.AppendLine("Switch origin number to " + newOrigin.Number);
        }

        public void ChangeDomain(string domain)
        {
            _domain = domain;
            _results.AppendLine("Switch domain to " + domain);
        }

        public void ChangeContext(string context)
        {
            _context = context;
            _results.AppendLine("Switch context to " + context);
        }
    }
}
