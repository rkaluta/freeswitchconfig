using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl.Interfaces;
using System.Xml;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem.CallControl
{
   public static class Actions
    {
        public struct RingReady : ICallAction
        {
            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Ring_Ready; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get { return new NameValuePair[]{new NameValuePair("application","ring_ready")}; }
            }

            #endregion
        }

        public struct Answer : ICallAction
        {

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Answer; }
            }
            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get { return new NameValuePair[] { new NameValuePair("application", "answer") }; }
            }
            #endregion
        }

        public struct Sleep : ICallAction
        {
            private int _milliSeconds;
            public int MilliSeconds
            {
                get { return _milliSeconds; }
            }

            public Sleep(int milliSeconds)
            {
                _milliSeconds = milliSeconds;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Sleep; }
            }
            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get { return new NameValuePair[] { 
                    new NameValuePair("application", "sleep"),
                    new NameValuePair("data",_milliSeconds)}; }
            }
            #endregion
        }

        public struct Hangup : ICallAction
        {
            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Hangup; }
            }
            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get { return new NameValuePair[] { new NameValuePair("application", "hangup") }; }
            }
            #endregion
        }

        public struct AttendedTransfer : ICallAction
        {
            private sDomainExtensionPair _extension;
            public sDomainExtensionPair Extension
            {
                get { return _extension; }
            }

            public AttendedTransfer(sDomainExtensionPair extension, bool waitUntilDone)
            {
                _extension = extension;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Attended_Transfer; }
            }
            
            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get { return new NameValuePair[] { new NameValuePair("application", "att_xfer"),
                new NameValuePair("data","user/"+_extension.Extension+"@"+_extension.Domain)}; }
            }
            #endregion
        }

        public struct Break : ICallAction
        {
            private bool _all;
            public bool All
            {
                get { return _all; }
            }

            public Break(bool all)
            {
                _all = all;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Break; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get { return new NameValuePair[] { new NameValuePair("application", "break"),
                new NameValuePair("data",(_all ? All.ToString() : ""))}; }
            }
            #endregion
        }

        public struct Acl : ICallAction
        {
            private string _address;
            public string Address
            {
                get { return _address; }
            }

            private string _acl;
            public string ACL
            {
                get { return _acl; }
            }

            private string _variableName;
            public string VariableName
            {
                get { return _variableName; }
            }

            public Acl(string address, string acl, string variableName)
            {
                _address = address;
                _acl = acl;
                _variableName = variableName;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Acl; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "set"),
                new NameValuePair("data",_variableName+"=${acl("+Address+" "+ACL+")}")};
                }
            }
            #endregion
        }

        public struct CheckAcl : ICallAction
        {
            private string _address;
            public string Address
            {
                get { return _address; }
            }

            private string _acl;
            public string ACL
            {
                get { return _acl; }
            }

            private string _hangupCause;
            public string HangupCause
            {
                get { return _hangupCause; }
            }

            public CheckAcl(string address, string acl, string hangupCause)
            {
                _address = address;
                _acl = acl;
                _hangupCause = hangupCause;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.CheckAcl; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get { return new NameValuePair[] { new NameValuePair("application", "check_acl"),
                new NameValuePair("data",Address+" "+ACL+(_hangupCause==null ? "" : " "+_hangupCause))}; }
            }
            #endregion
        }

        public struct DB : ICallAction
        {
            private string _commandString;
            public string CommandString
            {
                get { return _commandString; }
            }

            private string _variableName;
            public string VariableName
            {
                get { return _variableName; }
            }

            public DB(string commandString, bool waitUntilDone)
                : this(commandString,null,waitUntilDone)
            {
            }

            public DB(string commandString,string variableName, bool waitUntilDone)
            {
                _variableName = variableName;
                _commandString = commandString;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.DB; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get { 
                    if (_variableName!=null)
                        return new NameValuePair[] { new NameValuePair("application", "set"),
                            new NameValuePair("data",_variableName+"=${db("+_commandString+")}")};
                    return new NameValuePair[] { new NameValuePair("application", "db"),
                new NameValuePair("data",_commandString)}; }
            }
            #endregion
        }

        public struct Deflect : ICallAction
        {
            private sDomainExtensionPair _extension;
            public sDomainExtensionPair Extension
            {
                get { return _extension; }
            }

            public Deflect(sDomainExtensionPair extension, bool waitUntilDone)
            {
                _extension = extension;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Deflect; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get { return new NameValuePair[] { new NameValuePair("application", "deflect"),
                new NameValuePair("data","sip:"+_extension.Extension+"@"+_extension.Domain)}; }
            }
            #endregion
        }

        public struct Echo : ICallAction
        {
            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Echo; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get { return new NameValuePair[] { new NameValuePair("application", "echo") }; }
            }
            #endregion
        }

        public struct Enum : ICallAction
        {
            private string _number;
            public string Number
            {
                get { return _number; }
            }

            private string _searchDomain;
            public string SearchDomain
            {
                get { return _searchDomain; }
            }

            private string _variableName;
            public string VariableName
            {
                get { return _variableName; }
            }

            public Enum(string number, string searchDomain)
                : this(number,searchDomain,null)
            {}

            public Enum(string number, string searchDomain, string variableName)
            {
                _number = number;
                _searchDomain = searchDomain;
                _variableName = variableName;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Enum; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    if (_variableName != null)
                        return new NameValuePair[] { new NameValuePair("application", "set"),
                            new NameValuePair("data",_variableName+"=${enum("+_number+" "+_searchDomain+")}")};
                    return new NameValuePair[] { new NameValuePair("application", "enum"),
                new NameValuePair("data",_number+" "+_searchDomain)};
                }
            }
            #endregion
        }

        public struct Eval : ICallAction
        {
            private string _commandString;
            public string CommandString
            {
                get { return _commandString; }
            }

            private string _variableName;
            public string VariableName
            {
                get { return _variableName; }
            }

            public Eval(string commandString, bool waitUntilDone)
                : this(commandString, null, waitUntilDone) { }

            public Eval(string commandString,string variableName, bool waitUntilDone)
            {
                _commandString = commandString;
                _variableName = variableName;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Eval; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get { 
                    if (_variableName!=null)
                        return new NameValuePair[] { new NameValuePair("application", "set"),
                                new NameValuePair("data",_variableName+"=${eval("+_commandString+")}")};     
                    return new NameValuePair[] { new NameValuePair("application", "eval"),
                        new NameValuePair("data",_commandString)}; 
                }
            }
            #endregion
        }

        public struct Event : ICallAction
        {
            private string _eventName;
            public string EventName
            {
                get { return _eventName; }
            }

            private string _eventSubClass;
            public string EventSubClass
            {
                get { return _eventSubClass; }
            }

            private Dictionary<string, string> _header;
            public Dictionary<string, string> Header
            {
                get { return _header; }
            }

            public Event(string eventName, string eventSubClass, Dictionary<string, string> header)
            {
                _eventName = eventName;
                _eventSubClass = eventSubClass;
                _header = header;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Event; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get {
                    string ihead = "";
                    if (_header != null)
                    {
                        foreach (string str in _header.Keys)
                            ihead += str + "=" + _header[str] + ",";
                        if (ihead.Length > 0)
                            ihead = ihead.Substring(0, ihead.Length - 1);
                    }
                    if (_eventSubClass != null)
                        ihead = "Event-Subclass=" + _eventSubClass + (ihead.Length == 0 ? "" : ",") + ihead;
                    if (_eventName != null)
                        ihead = _eventName + (ihead.Length == 0 ? "" : ",") + ihead;
                    return new NameValuePair[] { new NameValuePair("application", "event"),
                    new NameValuePair("data",ihead)}; 
                }
            }
            #endregion
        }

        public struct ExecuteExtension : ICallAction
        {
            private string _context;
            public string Context
            {
                get { return _context; }
            }

            private string _extension;
            public string Extension
            {
                get { return _extension; }
            }

            public ExecuteExtension(string context,string extension){
                _context = context;
                _extension = extension;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Execute_Extension; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "execute_entension"),
                new NameValuePair("data",(_extension == null ? "" : _extension)+" "+(_context == null ? "" : _context))};
                }
            }
            #endregion
        }

        public struct Export : ICallAction {
            private string _variableName;
            public string VariableName
            {
                get { return _variableName; }
            }

            private string _value;
            public string Value
            {
                get { return _value; }
            }

            public Export(string variableName, string value, bool waitUntilDone)
            {
                _variableName = variableName;
                _value = value;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Export; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "export"),
                new NameValuePair("data",_variableName+"="+_value)};
                }
            }
            #endregion
        }

        public struct ExportSettings : ICallAction
        {
            private string _variableName;
            public string VariableName
            {
                get { return _variableName; }
            }

            private string _value;
            public string Value
            {
                get { return _value; }
            }

            public ExportSettings(string variableName, string value)
            {
                _variableName = variableName;
                _value = value;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Export; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "export"),
                new NameValuePair("data",_variableName+"="+_value)};
                }
            }
            #endregion
        }

        public struct Info : ICallAction
        {
            private string _variableName;
            public string VariableName{
                get{return _variableName;}
            }

            public Info(string variableName){
                _variableName=variableName;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Info; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    if (_variableName!=null)
                        return new NameValuePair[] { new NameValuePair("application", "set"),
                            new NameValuePair("data",_variableName+"=${info()}")};
                    return new NameValuePair[] { new NameValuePair("application", "info") };
                }
            }
            #endregion
        }

        public struct MkDir : ICallAction
        {
            private string _path;
            public string Path
            {
                get { return _path; }
            }

            private string _variableName;
            public string VariableName{
                get{return _variableName;}
            }

            public MkDir(string path)
                :this(path,null){}

            public MkDir(string path,string variableName)
            {
                _path = path;
                _variableName=variableName;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.MkDir; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    if (_variableName!=null)
                        return new NameValuePair[] { new NameValuePair("application", "set"),
                            new NameValuePair("data",_variableName+"=${mkdir("+_path+")}")};
                    return new NameValuePair[] { new NameValuePair("application", "mkdir"),
                new NameValuePair("data",_path)};
                }
            }
            #endregion
        }

        public struct Presence : ICallAction
        {
            private bool _set;
            public bool Set
            {
                get { return _set; }
            }

            private sDomainExtensionPair _extension;
            public sDomainExtensionPair Extension
            {
                get { return _extension; }
            }

            private string _presenceName;
            public string PresenceName
            {
                get { return _presenceName; }
            }

            private string _message;
            public string Message
            {
                get { return _message; }
            }

            public Presence(bool set,sDomainExtensionPair extension,string presenceName,string message,bool waitUntilDone){
                _set = set;
                _extension = extension;
                _presenceName = presenceName;
                _message = message;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Presence; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "presence"),
                new NameValuePair("data",(_set ? "in" : "out" ) +" "+_extension.Extension+"@"+_extension.Domain+" "+(_presenceName==null ? "unkown" : _presenceName)+(_message==null ? "" : " "+_message))};
                }
            }
            #endregion
        }

        public struct Privacy : ICallAction
        {
            private CallPrivacyTypes _privacyType;
            public CallPrivacyTypes PrivacyType
            {
                get { return _privacyType; }
            }

            public Privacy(CallPrivacyTypes privacyType, bool waitUntilDone)
            {
                _privacyType = privacyType;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Privacy; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "privcay"),
                new NameValuePair("data",_privacyType.ToString())};
                }
            }
            #endregion
        }

        public struct ReadDigits : ICallAction
        {
            private int _min;
            public int Min
            {
                get { return _min; }
            }

            private int _max;
            public int Max
            {
                get { return _max; }
            }

            private string _soundFile;
            public string SoundFile
            {
                get { return _soundFile; }
            }

            private string _variableName;
            public string VariableName
            {
                get { return _variableName; }
            }

            private int _timeoutMS;
            public int TimeoutMS
            {
                get { return _timeoutMS; }
            }

            private string _terminators;
            public string Terminators
            {
                get { return _terminators; }
            }

            public ReadDigits(int min, int max, string soundFile, string variableName, int timeoutMS, string terminators)
            {
                _min = min;
                _max = max;
                _soundFile = soundFile;
                _variableName = variableName;
                _timeoutMS = timeoutMS;
                _terminators = terminators;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.ReadDigits; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "read"),
                new NameValuePair("data",_min.ToString()+" "+_max.ToString()+" "+_soundFile+" "+_soundFile+" "+_variableName+" "+_timeoutMS.ToString()+" "+_terminators)};
                }
            }
            #endregion
        }

        public struct Redirect : ICallAction
        {
            private sDomainExtensionPair[] _extension;
            public sDomainExtensionPair[] Extension
            {
                get { return _extension; }
            }

            public Redirect(sDomainExtensionPair[] extension, bool waitUntilDone)
            {
                _extension = extension;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Redirect; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    string spars = "";
                    foreach (sDomainExtensionPair ext in _extension)
                        spars += ",sip:" + ext.Extension + "@" + ext.Domain;
                    return new NameValuePair[] { new NameValuePair("application", "redirect"),
                new NameValuePair("data",spars.Substring(1))};
                }
            }
            #endregion
        }

        public struct Respond : ICallAction
        {
            private string _response;
            public string Response
            {
                get { return _response; }
            }

            public Respond(string response, bool waitUntilDone)
            {
                _response = response;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Respond; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "respond"),
                new NameValuePair("data",_response)};
                }
            }
            #endregion
        }

        public struct SendDisplay : ICallAction
        {
            private string _message;
            public string Message
            {
                get { return _message; }
            }

            public SendDisplay(string message, bool waitUntilDone)
            {
                _message = message;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.SendDisplay; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "send_display"),
                new NameValuePair("data",_message)};
                }
            }
            #endregion
        }

        public struct ToneDetect : ICallAction
        {
            private string _tone;
            public string Tone
            {
                get { return _tone; }
            }

            private int[] _frequencies;
            public int[] Frequencies
            {
                get { return _frequencies; }
            }

            private char[] _flags;
            public char[] Flags
            {
                get { return _flags; }
            }

            private int _relativeTimeout;
            public int RelativeTimeout
            {
                get { return _relativeTimeout; }
            }

            private string _app;
            public string App
            {
                get { return _app; }
            }

            private string _appData;
            public string AppData
            {
                get { return _appData; }
            }

            private int _hits;
            public int Hits
            {
                get { return _hits; }
            }

            public ToneDetect(string tone, int[] frequencies, char[] flags, int relativeTimeout, string app, string appData, int hits, bool waitUntilDone)
            {
                _tone = tone;
                _frequencies = frequencies;
                _flags = flags;
                _relativeTimeout = relativeTimeout;
                _app = app;
                _appData = appData;
                _hits = hits;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.ToneDetect; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    string sfreq = "";
                    foreach (int i in _frequencies)
                        sfreq += "," + i.ToString();
                    string sflags = "";
                    foreach (char c in _flags)
                        sflags += "," + c.ToString();
                    return new NameValuePair[] { new NameValuePair("application", "tone_detect"),
                new NameValuePair("data",_tone+" "+sfreq.Substring(1)+" "+sflags.Substring(1)+" +"+_relativeTimeout.ToString()+" "+(_app == null ? "" : _app)+" "+(_appData==null ? "" : _appData)+" "+_hits.ToString())};
                }
            }
            #endregion
        }

        public struct StopToneDetect : ICallAction
        {
            public StopToneDetect(bool waitUntilDone)
            {
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.StopToneDetect; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "stop_tone_detect")};
                }
            }
            #endregion
        }

        public struct CurrentTime : ICallAction
        {
            private string _format;
            public string Format
            {
                get { return _format; }
            }

            private string _variableName;
            public string VariableName
            {
                get { return _variableName; }
            }


            public CurrentTime(string format,string variableName)
            {
                _format = format;
                _variableName = variableName;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.CurrentTime; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "set"),
                    new NameValuePair("data",_variableName+"=${strftime( "+_format+")}")};
                }
            }
            #endregion
        }

        public struct System : ICallAction
        {
            private string _commandString;
            public string CommandString
            {
                get { return _commandString; }
            }

            private string _variableName;
            public string VariableName{
                get{return _variableName;}
            }

            public System(string commandString,bool waitUntilDone)
                :this(commandString,null,waitUntilDone){}

            public System(string commandString,string variableName,bool waitUntilDone)
            {
                _commandString = commandString;
                _waitUntilDone = waitUntilDone;
                _variableName=variableName;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.System; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    if (_variableName!=null)
                        return new NameValuePair[] { new NameValuePair("application", "set"),
                            new NameValuePair("data",_variableName+"=${system("+_commandString+")}")};
                    return new NameValuePair[] { new NameValuePair("application", "system"),
                        new NameValuePair("data",_commandString)};
                }
            }
            #endregion
        }

        public struct WaitForSilence : ICallAction
        {
            private int _silenceThreshold;
            public int SilenceThreshold
            {
                get { return _silenceThreshold; }
            }

            private int _silenceHits;
            public int SilenceHits
            {
                get { return _silenceHits; }
            }

            private int _listenHits;
            public int ListenHits
            {
                get { return _listenHits; }
            }

            private int _timeoutMS;
            public int TimeoutMS
            {
                get { return _timeoutMS; }
            }

            public WaitForSilence(int silenceThreshold, int silenceHits, int listenHits, int timeoutMS, bool waitUntilDone)
            {
                _silenceThreshold = silenceThreshold;
                _silenceHits = silenceHits;
                _listenHits = listenHits;
                _timeoutMS = timeoutMS;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.WaitForSilence; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "wait_for_silence"),
                new NameValuePair("data",_silenceThreshold.ToString()+" "+_silenceHits.ToString()+" "+_listenHits.ToString()+" "+_timeoutMS.ToString())};
                }
            }
            #endregion
        }

        public struct Chat : ICallAction
        {
            private string _proto;
            public string Proto
            {
                get { return _proto; }
            }

            private string _from;
            public string From
            {
                get { return _from; }
            }

            private string _to;
            public string To
            {
                get { return _to; }
            }

            private string _message;
            public string Message
            {
                get { return _message; }
            }

            public Chat(string proto, string from, string to, string message, bool waitUntilDone)
            {
                _proto = proto;
                _from = from;
                _to = to;
                _message = message;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Chat; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "chat"),
                new NameValuePair("data",_proto+"|"+_from+"|"+_to+"|"+_message)};
                }
            }
            #endregion
        }

        public struct Log : ICallAction
        {
            private LogLevels _level;
            public LogLevels Level
            {
                get { return _level; }
            }

            private string _message;
            public string Message
            {
                get { return _message; }
            }

            public Log(LogLevels level, string message)
            {
                _level = level;
                _message = message;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Log; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "log"),
                new NameValuePair("data",_level.ToString().ToUpper()+" "+_message)};
                }
            }
            #endregion
        }

        public struct SessionLogLevel : ICallAction
        {
            private LogLevels _level;
            public LogLevels Level
            {
                get { return _level; }
            }

            public SessionLogLevel(LogLevels level)
            {
                _level = level;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Session_LogLevel; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "session_loglevel"),
                new NameValuePair("data",_level.ToString().ToLower())};
                }
            }
            #endregion
        }

        public struct FIFOIn : ICallAction
        {
            private string _queue;
            public string Queue
            {
                get { return _queue; }
            }

            private string _exitSound;
            public string ExitSound
            {
                get { return _exitSound; }
            }

            private string _onHold;
            public string OnHold
            {
                get { return _onHold; }
            }

            public FIFOIn(string queue, string exitSound, string onHold, bool waitUntilDone)
            {
                _queue = queue;
                _exitSound = exitSound;
                _onHold = onHold;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.FIFO_In; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "fifo"),
                new NameValuePair("data",_queue+" in "+_exitSound+(_onHold==null ? "" : " "+_onHold))};
                }
            }
            #endregion
        }

        public struct FIFOOut : ICallAction
        {
            private string _queue;
            public string Queue
            {
                get { return _queue; }
            }

            private bool _wait;
            public bool Wait
            {
                get { return _wait; }
            }

            private string _foundSound;
            public string FoundSound
            {
                get { return _foundSound; }
            }

            private string _onHold;
            public string OnHold
            {
                get { return _onHold; }
            }

            public FIFOOut(string queue, bool wait, string foundSound, string onHold,bool waitUntilDone)
            {
                _queue = queue;
                _wait = wait;
                _foundSound = foundSound;
                _onHold = onHold;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.FIFO_Out; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "fifo"),
                new NameValuePair("data",_queue+" out "+(_wait ? "wait" : "nowait")+" "+_foundSound+(_onHold == null ? "" : " "+_onHold))};
                }
            }
            #endregion
        }

        public struct SetJitterBuffer : ICallAction
        {
            private int _milliSeconds;
            public int MilliSeconds
            {
                get { return _milliSeconds; }
            }

            private int? _maxMilliSeconds;
            public int? MaxMilliSeconds
            {
                get { return _maxMilliSeconds; }
            }

            private int? _maxDriftMilliSeconds;
            public int? MaxDriftMilliSeconds{
                get { return _maxDriftMilliSeconds; }
            }

            public SetJitterBuffer(int milliSeconds, int? maxMilliSeconds, int? maxDriftMilliSeconds, bool waitUntilDone)
            {
                _milliSeconds = milliSeconds;
                _maxMilliSeconds = maxMilliSeconds;
                _maxDriftMilliSeconds = maxDriftMilliSeconds;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.SetJitterBuffer; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "jitterbuffer"),
                new NameValuePair("data",_milliSeconds.ToString()+(_maxMilliSeconds.HasValue ? ":"+_maxMilliSeconds.Value.ToString()+(_maxDriftMilliSeconds.HasValue ? ":"+_maxDriftMilliSeconds.Value.ToString() : "")  : ""))};
                }
            }
            #endregion
        }

        public struct SetAudioLevel : ICallAction
        {
            private bool _incoming;
            public bool Incoming
            {
                get { return _incoming; }
            }

            private int _level;
            public int Level
            {
                get { return _level; }
            }

            public SetAudioLevel(bool incoming, int level)
            {
                if ((level >= -4) && (level <= 4))
                {
                    _incoming = incoming;
                    _level = level;
                }
                else
                    throw new Exception("Audio level must be between -4 and 4");
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Set_Audio_Level; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "set_audio_level"),
                new NameValuePair("data",(_incoming ? "read" : "write")+" "+_level.ToString())};
                }
            }
            #endregion
        }

        public struct SetGlobal : ICallAction
        {
            private string _variableName;
            public string VariableName
            {
                get { return _variableName; }
            }

            private string _value;
            public string Value
            {
                get { return _value; }
            }

            public SetGlobal(string variableName, string value, bool waitUntilDone)
            {
                _variableName = variableName;
                _value = value;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Set_Global; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "set_global"),
                new NameValuePair("data",_variableName+"="+_value)};
                }
            }
            #endregion
        }

        public struct SetChannelName : ICallAction
        {
            private string _name;
            public string Name
            {
                get { return _name; }
            }

            public SetChannelName(string name)
            {
                _name = name;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Set_Channel_Name; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "set_name"),
                new NameValuePair("data",_name)};
                }
            }
            #endregion
        }

        public struct SetUser : ICallAction
        {
            private sDomainExtensionPair _extension;
            public sDomainExtensionPair Extension
            {
                get { return _extension; }
            }

            private string _prefix;
            public string Prefix
            {
                get { return _prefix; }
            }

            public SetUser(sDomainExtensionPair extension, string prefix, bool waitUntilDone)
            {
                _extension = extension;
                _prefix = prefix;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Set_User; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "set_user"),
                new NameValuePair("data",_extension.Extension+"@"+_extension.Domain+(_prefix==null ? "" : " "+_prefix))};
                }
            }
            #endregion
        }

        public struct SetZombieExec : ICallAction
        {
            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Set_Zombie_Exec; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "set_zombie_exec")};
                }
            }
            #endregion
        }

        public struct SetVerboseEvents : ICallAction
        {
            private bool _yes;
            public bool Yes
            {
                get { return _yes; }
            }

            public SetVerboseEvents(bool yes){
                _yes = yes;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Set_Verbose_Events; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "verbose_events"),
                new NameValuePair("data",_yes)};
                }
            }
            #endregion
        }

        public struct BridgeToExtension : ICallAction
        {
            private sDomainExtensionPair _extension;
            public sDomainExtensionPair Extension
            {
                get { return _extension; }
            }

            public BridgeToExtension(sDomainExtensionPair extension, bool waitUntilDone)
            {
                _extension = extension;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.BridgeToExtension; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "bridge"),
                new NameValuePair("data","user/"+_extension.Extension+"@"+_extension.Domain)};
                }
            }
            #endregion
        }

        public struct BridgeToMultipleExtensions : ICallAction
        {
            private sDomainExtensionPair[] _extension;
            public sDomainExtensionPair[] Extension
            {
                get { return _extension; }
            }

            private bool _sequential;
            public bool Sequential
            {
                get { return _sequential; }
            }

            public BridgeToMultipleExtensions(sDomainExtensionPair[] extension,bool sequential, bool waitUntilDone)
            {
                _extension = extension;
                _sequential = sequential;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.BridgeToMultipleExtensions; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    string dstring = "";
                    foreach (sDomainExtensionPair sdep in _extension)
                    {
                        dstring += (_sequential ? "," : "|") + "user/" + sdep.Extension + "@" + sdep.Domain;
                    }
                    if (dstring.Length > 1)
                        dstring = dstring.Substring(1);
                    return new NameValuePair[] { new NameValuePair("application", "bridge"),
                new NameValuePair("data",dstring)};
                }
            }
            #endregion
        }

        public struct BridgeOutGateway : ICallAction
        {
            private string _gateway;
            public string Gateway
            {
                get { return _gateway; }
            }

            private string _number;
            public string Number
            {
                get { return _number; }
            }

            public BridgeOutGateway(string gateway, string number, bool waitUntilDone)
            {
                _gateway = gateway;
                _number = number;
                _waitUntilDone = waitUntilDone;
            }

            public BridgeOutGateway(sGatewayNumberPair gateway, bool waitUntilDone)
                : this(gateway.GatewayName, gateway.Number, waitUntilDone) { }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.BridgeOutGateway; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "bridge"),
                new NameValuePair("data","sofia/gateway/"+_gateway+"/"+_number)};
                }
            }
            #endregion
        }

        public struct Voicemail : ICallAction
        {
            private string _context;
            public string Context
            {
                get { return _context; }
            }

            private sDomainExtensionPair _extension;
            public sDomainExtensionPair Extension
            {
                get { return _extension; }
            }

            public Voicemail(string context, sDomainExtensionPair extension)
            {
                _context=context;
                _extension = extension;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Voicemail; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "voicemail"),
                new NameValuePair("data",_context+" "+_extension.Domain+" "+_extension.Extension)};
                }
            }
            #endregion
        }

        public struct BridgeExport : ICallAction
        {
            private string _variableName;
            public string VariableName
            {
                get { return _variableName; }
            }

            private string _value;
            public string Value
            {
                get { return _value; }
            }

            private bool _bLegOnly;
            public bool BLegOnly
            {
                get { return _bLegOnly; }
            }

            public BridgeExport(string variableName, string value, bool bLegOnly)
            {
                _variableName = variableName;
                _value = value;
                _bLegOnly = bLegOnly;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Bridge_Export; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "bridge_export"),
                new NameValuePair("data",(_bLegOnly ? "nolocal:" : "")+_variableName+"="+_value)};
                }
            }
            #endregion
        }

        public struct EavesDrop : ICallAction
        {
            private string _uuid;
            public string UUID
            {
                get { return _uuid; }
            }

            public EavesDrop(string uuid,bool waitUntilDone)
            {
                _uuid = uuid;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.EavesDrop; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "eavesdrop"),
                new NameValuePair("data",(_uuid == null ? "all" : _uuid))};
                }
            }
            #endregion
        }

        public struct Intercept : ICallAction
        {
            private string _uuid;
            public string UUID
            {
                get { return _uuid; }
            }

            private bool _bLeg;
            public bool BLeg
            {
                get { return _bLeg; }
            }

            public Intercept(string uuid, bool bleg, bool waitUntilDone)
            {
                _uuid = uuid;
                _bLeg = bleg;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Intercept; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "intercept"),
                new NameValuePair("data",(_bLeg?"-bleg ":"")+_uuid)};
                }
            }
            #endregion
        }

        public struct IVR : ICallAction
        {
            private string _name;
            public string Name
            {
                get { return _name; }
            }

            public IVR(string name, bool waitUntilDone)
            {
                _name = name;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.IVR; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "ivr"),
                new NameValuePair("data",_name)};
                }
            }
            #endregion
        }

        public struct SoftHold : ICallAction
        {
            private string _offHoldKey;
            public string OffHoldKey
            {
                get { return _offHoldKey; }
            }

            private string _mohA;
            public string MohA
            {
                get { return _mohA; }
            }

            private string _mohB;
            public string MohB
            {
                get { return _mohB; }
            }

            public SoftHold(string offHoldkey, string mohA, string mohB, bool waitUntilDone)
            {
                _offHoldKey = offHoldkey;
                _mohA = mohA;
                _mohB = mohB;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Soft_Hold; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "soft_hold"),
                new NameValuePair("data",_offHoldKey+" "+(_mohA == null ? "" : _mohA)+" "+(_mohB==null ? "" : _mohB))};
                }
            }
            #endregion
        }

        public struct ThreeWay : ICallAction
        {
            private string _uuid;
            public string UUID
            {
                get { return _uuid; }
            }

            public ThreeWay(string uuid, bool waitUntilDone)
            {
                _uuid = uuid;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Three_Way; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "three_way"),
                new NameValuePair("data",_uuid)};
                }
            }
            #endregion
        }

        public struct Transfer : ICallAction
        {
            private string _destinationNumber;
            public string DestinationNumber
            {
                get { return _destinationNumber; }
            }

            private string _dialPlan;
            public string DialPlan
            {
                get { return _dialPlan; }
            }

            private string _context;
            public string Context
            {
                get { return _context; }
            }

            public Transfer(string destinationNumber, string dialPlan, string context, bool waitUntilDone)
            {
                _destinationNumber = destinationNumber;
                _dialPlan = dialPlan;
                _context = context;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Transfer; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "transfer"),
                new NameValuePair("data",_destinationNumber+" "+(_dialPlan==null ? "" : _dialPlan)+" "+(_context==null ? "" : _context))};
                }
            }
            #endregion
        }

        public struct TransferToContext : ICallAction
        {
            private string _context;
            public string Context
            {
                get { return _context; }
            }

            public TransferToContext(string context)
            {
                _context = context;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Transfer_To_Context; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "transfer"),
                new NameValuePair("data","${destination_number} XML "+_context)};
                }
            }
            #endregion
        }

        public struct TransferToCallExtension : ICallAction
        {
            private sCallExtensionReference _destination;
            public sCallExtensionReference Destination
            {
                get { return _destination; }
            }

            public TransferToCallExtension(sCallExtensionReference destination)
            {
                _destination = destination;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Transfer_To_Call_Extension; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "transfer"),
                new NameValuePair("data",_destination.ToString())};
                }
            }
            #endregion
        }

        public struct Set : ICallAction
        {
            private string _variableName;
            public string VariableName
            {
                get { return _variableName; }
            }

            private string _value;
            public string Value
            {
                get { return _value; }
            }

            public Set(string variableName, string value)
            {
                _variableName = variableName;
                _value = value;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Set; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "set"),
                new NameValuePair("data",_variableName+"="+_value)};
                }
            }
            #endregion
        }

        public struct UnSet : ICallAction
        {
            private string _variableName;
            public string VariableName
            {
                get { return _variableName; }
            }

            public UnSet(string variableName)
            {
                _variableName = variableName;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.UnSet; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "unset"),
                new NameValuePair("data",_variableName)};
                }
            }
            #endregion
        }

        public struct ConferenceSetAutoOutCall : ICallAction
        {
            private sDomainExtensionPair _extension;
            public sDomainExtensionPair Extension
            {
                get { return _extension; }
            }

            public ConferenceSetAutoOutCall(sDomainExtensionPair extension)
            {
                _extension = extension;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Conference_Set_Auto_Outcall; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "conference_set_auto_outcall"),
                new NameValuePair("data","user/"+_extension.Extension+"@"+_extension.Domain)};
                }
            }
            #endregion
        }

        public struct JoinConference : ICallAction
        {
            private string _conferenceName;
            public string ConferenceName
            {
                get { return _conferenceName; }
            }

            public JoinConference(string conferenceName, bool waitUntilDone)
            {
                _conferenceName = conferenceName;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.JoinConference; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "conference"),
                new NameValuePair("data",_conferenceName)};
                }
            }
            #endregion
        }

        public struct KickFromConference : ICallAction
        {
            private string _conferenceName;
            public string ConferenceName
            {
                get { return _conferenceName; }
            }

            private string _extension;
            public string Extension
            {
                get { return _extension; }
            }

            public KickFromConference(string conferenceName, string extension, bool waitUntilDone)
            {
                _conferenceName = conferenceName;
                _extension = extension;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.KickFromConference; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone;}
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "conference"),
                new NameValuePair("data",_conferenceName+" kick "+_extension)};
                }
            }
            #endregion
        }

        public struct PlayAudioFile : ICallAction
        {
            private string _filePath;
            public string FilePath
            {
                get { return _filePath; }
            }

            private bool? _mux;
            public bool? Mux
            {
                get { return _mux; }
            }

            private int? _loop;
            public int? Loop
            {
                get { return _loop; }
            }

            private int? _timeLimit;
            public int? TimeLimit
            {
                get { return _timeLimit; }
            }

            public PlayAudioFile(string filePath, bool waitUntilDone)
            {
                _filePath = filePath;
                _mux = null;
                _loop = null;
                _timeLimit = null;
                _waitUntilDone = waitUntilDone;
            }

            public PlayAudioFile(string filePath, bool mux, int? loop, int? timeLimit, bool waitUntilDone)
            {
                _filePath = filePath;
                _mux = mux;
                _loop = loop;
                _timeLimit = timeLimit;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.PlayAudioFile; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    if (_mux.HasValue)
                        return new NameValuePair[] { new NameValuePair("application", "displace_session"),
                            new NameValuePair("data",_filePath+" "+(_loop.HasValue ? "loop="+_loop.Value.ToString() : "")+ (_mux.Value ? " mux" : "") + (_timeLimit.HasValue ? "+" + _timeLimit.Value.ToString() : ""))};
                    else
                        return new NameValuePair[] { new NameValuePair("application", "playback"),
                            new NameValuePair("data",_filePath)};
                }
            }
            #endregion
        }

        public struct PlayAudioFileEndlessly : ICallAction
        {
            private string _filePath;
            public string FilePath
            {
                get { return _filePath; }
            }

            public PlayAudioFileEndlessly(string filePath, bool waitUntilDone)
            {
                _filePath = filePath;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.PlayAudioFileEndlessly; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "endless_playback"),
                new NameValuePair("data",_filePath)};
                }
            }
            #endregion
        }

        public struct PlayAndGetDigits : ICallAction
        {
            private int _minDigits;
            public int MinDigits
            {
                get { return _minDigits; }
            }

            private int _maxDigits;
            public int MaxDigits
            {
                get { return _maxDigits; }
            }

            private int _tries;
            public int Tries
            {
                get { return _tries; }
            }

            private long _timeout;
            public long Timeout
            {
                get { return _timeout; }
            }

            private string _terminators;
            public string Terminators
            {
                get { return _terminators; }
            }

            private string _file;
            public string File
            {
                get { return _file; }
            }

            private string _invalidFile;
            public string InvalidFile
            {
                get { return _invalidFile; }
            }

            private string _var;
            public string Var
            {
                get { return _var; }
            }

            private string _regExp;
            public string RegExp
            {
                get { return _regExp; }
            }

            private int? _digitTimeout;
            public int? DigitTimeout
            {
                get { return _digitTimeout; }
            }

            public PlayAndGetDigits(int minDigits, int maxDigits, int tries, long timeout, string terminators, string file, string invalidFile,string var, string regexp, int? digitTimeout)
            {
                _minDigits = minDigits;
                _maxDigits = maxDigits;
                _tries = tries;
                _timeout = timeout;
                _terminators = terminators;
                _file = file;
                _invalidFile = invalidFile;
                _regExp = regexp;
                _digitTimeout = digitTimeout;
                _var = var;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.PlayAndGetDigits; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "play_and_get_digits"),
                new NameValuePair("data", _minDigits.ToString() + " " + _maxDigits.ToString() + " " + _tries.ToString() + " " + _timeout.ToString() + " " + _terminators + " " + _file + " " + (_invalidFile != null ? _invalidFile : "silence_stream://250") + " " + _var + " " + (_regExp == null ? "\\d+" : _regExp) + " " + (_digitTimeout.HasValue ? _digitTimeout.ToString() : ""))};
                }
            }
            #endregion
        }

        public struct GenTones : ICallAction
        {
            private string _toneString;
            public string ToneString
            {
                get { return _toneString; }
            }

            private int? _loops;
            public int? Loops
            {
                get { return _loops; }
            }

            private int _milliSeconds;
            public int MilliSeconds
            {
                get { return _milliSeconds; }
            }

            private int _hZ;
            public int HZ
            {
                get { return _hZ; }
            }

            public GenTones(string toneString,int? loops, bool waitUntilDone)
            {
                _toneString = toneString;
                _loops = loops;
                _waitUntilDone = waitUntilDone;
                _milliSeconds = -1;
                _hZ = -1;
            }

            public GenTones(int milliSeconds, int loops, int Hz, bool waitUntilDone)
            {
                _toneString = null;
                _milliSeconds = milliSeconds;
                _loops = loops;
                _hZ = Hz;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.GenTones; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    if (_toneString == null)
                        return new NameValuePair[] { new NameValuePair("application", "gentones"),
                            new NameValuePair("data",_toneString+(_loops.HasValue ? "|"+_loops.Value.ToString() : ""))};
                    else
                        return new NameValuePair[] { new NameValuePair("application", "gentones"),
                            new NameValuePair("data","%("+_milliSeconds.ToString()+","+_loops.Value.ToString()+","+_hZ.ToString()+")")};
                }
            }
            #endregion
        }

        public struct PlayAndDetectSpeech : ICallAction
        {
            private string _filePath;
            public string FilePath
            {
                get { return _filePath; }
            }

            private string _engine;
            public string Engine
            {
                get { return _engine; }
            }

            Dictionary<string, string> _parameters;
            public Dictionary<string, string> Parameters
            {
                get { return _parameters; }
            }

            private string _grammar;
            public string Grammar
            {
                get { return _grammar; }
            }

            public PlayAndDetectSpeech(string filePath, string engine, Dictionary<string, string> parameters, string grammer, bool waitUntilDone)
            {
                _filePath = filePath;
                _engine = engine;
                _parameters = parameters;
                _grammar = grammer;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.PlayAndDetectSpeech; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    string spars = "";
                    if (_parameters != null)
                    {
                        foreach (string str in _parameters.Keys)
                            spars += str + "=" + _parameters[str] + ",";
                    }
                    if (spars.Length > 0)
                        spars = " {" + spars.Substring(0, spars.Length - 1) + "}";
                    return new NameValuePair[] { new NameValuePair("application", "play_and_detect_speech"),
                new NameValuePair("data",_filePath + " detect:" + _engine + spars + (_grammar == null ? "" : " " + _grammar))};
                }
            }
            #endregion
        }

        public struct PlayFSV : ICallAction
        {
            private string _filePath;
            public string FilePath
            {
                get { return _filePath; }
            }

            public PlayFSV(string filePath, bool waitUntilDone)
            {
                _filePath = filePath;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Play_FSV; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "play_fsv"),
                new NameValuePair("data",_filePath)};
                }
            }
            #endregion
        }

        public struct Record : ICallAction
        {
            private string _path;
            public string Path
            {
                get { return _path; }
            }

            private int? _timeLimit;
            public int? TimeLimit
            {
                get { return _timeLimit; }
            }

            private int? _silenceThreshHold;
            public int? SilenceThreshHold
            {
                get { return _silenceThreshHold; }
            }

            private int? _silenceHits;
            public int? SilenceHits
            {
                get { return _silenceHits; }
            }

            public Record(string path, int? timeLimit, int? silenceThreshHold, int? silenceHits, bool waitUntilDone)
            {
                _path = path;
                _timeLimit = timeLimit;
                _silenceThreshHold = silenceThreshHold;
                _silenceHits = silenceHits;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Record; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "record"),
                new NameValuePair("data",_path + " " + (_timeLimit.HasValue ? _timeLimit.ToString() : "") + " " + (_silenceThreshHold.HasValue ? _silenceThreshHold.ToString() : "") + " " + (_silenceHits.HasValue ? _silenceHits.ToString() : ""))};
                }
            }
            #endregion
        }

        public struct RecordSession : ICallAction
        {
            private string _path;
            public string Path
            {
                get { return _path; }
            }

            public RecordSession(string path, bool waitUntilDone)
            {
                _path = path;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Record_Session; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "record_session"),
                new NameValuePair("data",_path)};
                }
            }
            #endregion
        }

        public struct StopRecordSession : ICallAction
        {
            private string _path;
            public string Path
            {
                get { return _path; }
            }

            public StopRecordSession(string path, bool waitUntilDone)
            {
                _path = path;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Stop_Record_Session; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "stop_record_session"),
                new NameValuePair("data",_path)};
                }
            }
            #endregion
        }

        public struct Say : ICallAction
        {
            private string _language;
            public string Language
            {
                get { return _language; }
            }

            private SayTypes _type;
            public SayTypes Type
            {
                get { return _type; }
            }

            private SayMethods _method;
            public SayMethods Method{
                get { return _method; }
            }

            private SayGenders _gender;
            public SayGenders Gender
            {
                get { return _gender; }
            }

            private string _text;
            public string Text
            {
                get { return _text; }
            }

            public Say(string language, SayTypes type, SayMethods method, SayGenders gender, string text, bool waitUntilDone)
            {
                _language = language;
                _type = type;
                _method = method;
                _gender = gender;
                _text = text;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Say; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "say"),
                new NameValuePair("data",_language+" "+_type.ToString()+" "+_method.ToString().Replace("_","/")+" "+_gender.ToString()+ " "+_text)};
                }
            }
            #endregion
        }

        public struct ScheduleBroadcast : ICallAction
        {
            private int _timeMS;
            public int TimeMS
            {
                get { return _timeMS; }
            }

            private string _path;
            public string Path
            {
                get { return _path; }
            }

            private BroadcastLegs _leg;
            public BroadcastLegs Leg
            {
                get { return _leg; }
            }

            public ScheduleBroadcast(int timeMS, string path, BroadcastLegs leg, bool waitUntilDone)
            {
                _timeMS = timeMS;
                _path = path;
                _leg = leg;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Schedule_Broadcast; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "sched_broadcast"),
                new NameValuePair("data","+" + _timeMS.ToString() + " " + _path + " " + _leg.ToString())};
                }
            }
            #endregion
        }

        public struct Speak : ICallAction
        {
            private string _engine;
            public string Engine
            {
                get { return _engine; }
            }

            private string _voice;
            public string Voice
            {
                get { return _voice; }
            }

            private string _text;
            public string Text
            {
                get { return _text; }
            }

            private string _timerName;
            public string TimerName
            {
                get { return _timerName; }
            }

            public Speak(string engine, string voice, string text, string timerName, bool waitUntilDone)
            {
                _engine = engine;
                _voice = voice;
                _text = text;
                _timerName = timerName;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Speak; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "speak"),
                new NameValuePair("data",_engine + "|" + _voice + "|" + _text + (_timerName != null ? "|" + _timerName : ""))};
                }
            }
            #endregion
        }

        public struct StopDisplace : ICallAction
        {
            private string _path;
            public string Path
            {
                get { return _path; }
            }

            public StopDisplace(string path, bool waitUntilDone)
            {
                _path = path;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Stop_Displace; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "stop_displace_session"),
                new NameValuePair("data",_path)};
                }
            }
            #endregion
        }

        public struct BindDigitAction : ICallAction{
            private string _realm;
            public string Realm
            {
                get { return _realm; }
            }

            private sDialableNumber? _number;
            public sDialableNumber? Number
            {
                get { return _number; }
            }

            private string _regex;
            public string Regex
            {
                get { return _regex; }
            }

            private string _commandString;
            public string CommandString
            {
                get { return _commandString; }
            }

            private string _arguements;
            public string Arguements
            {
                get { return _arguements; }
            }

            private BindDigitTargetLegs? _targetLeg;
            public BindDigitTargetLegs? TargetLeg
            {
                get { return _targetLeg; }
            }

            private BindDigitsEventLegs? _eventLeg;
            public BindDigitsEventLegs? EventLeg
            {
                get { return _eventLeg; }
            }

            public BindDigitAction(string realm, sDialableNumber number, string commandString, string arguements, BindDigitTargetLegs? targetLeg, BindDigitsEventLegs? eventLeg)
            {
                _realm = realm;
                _number = number;
                _commandString = commandString;
                _arguements = arguements;
                _targetLeg = targetLeg;
                _eventLeg = eventLeg;
                _regex = null;
            }

            public BindDigitAction(string realm, string regex, string commandString, string arguements, BindDigitTargetLegs? targetLeg, BindDigitsEventLegs? eventLeg)
            {
                _realm = realm;
                _number = null;
                _commandString = commandString;
                _arguements = arguements;
                _targetLeg = targetLeg;
                _eventLeg = eventLeg;
                _regex = regex;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Bind_Digit_Action; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    if (_number==null)
                        return new NameValuePair[] { new NameValuePair("application", "bind_digit_action"),
                        new NameValuePair("data","realm,~"+_regex+"," + _commandString + "," + (_arguements == null ? "" : _arguements) + "," + (_targetLeg.HasValue ? _targetLeg.Value.ToString() : "aleg") + "," + (_eventLeg.HasValue ? _eventLeg.Value.ToString() : "self"))};
                    else
                        return new NameValuePair[] { new NameValuePair("application", "bind_digit_action"),
                        new NameValuePair("data","realm," + _number.ToString() + "," + _commandString + "," + (_arguements == null ? "" : _arguements) + "," + (_targetLeg.HasValue ? _targetLeg.Value.ToString() : "aleg") + "," + (_eventLeg.HasValue ? _eventLeg.Value.ToString() : "self"))};
                }
            }
            #endregion
        }

        public struct ClearDigitAction : ICallAction
        {

            private string _realm;
            public string Realm
            {
                get { return _realm; }
            }

            public ClearDigitAction(string realm, bool waitUntilDone)
            {
                _realm = realm;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Clear_Digit_Aciton; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "clear_digit_action"),
                new NameValuePair("data",(_realm==null ? "all" : _realm))};
                }
            }
            #endregion
        }

        public struct DigitActionSetRealm : ICallAction
        {

            private string _realm;
            public string Realm
            {
                get { return _realm; }
            }

            public DigitActionSetRealm(string realm, bool waitUntilDone)
            {
                _realm = realm;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Digit_Action_Set_Realm; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "digit_action_set_realm"),
                new NameValuePair("data",(_realm==null ? "all" : _realm))};
                }
            }
            #endregion
        }

        public struct BindMetaApp : ICallAction
        {
            private sDialableNumber _digits;
            public sDialableNumber Digits
            {
                get { return _digits; }
            }

            private MetaAppLegTypes _leg;
            public MetaAppLegTypes Leg
            {
                get { return _leg; }
            }

            private MetaAppFlags[] _flags;
            public MetaAppFlags[] Flags
            {
                get { return _flags; }
            }

            private string _application;
            public string Application
            {
                get { return _application; }
            }

            private string _arguements;
            public string Arguements
            {
                get { return _arguements; }
            }

            public BindMetaApp(sDialableNumber digits, MetaAppLegTypes leg, MetaAppFlags[] flags,
                string application, string arguements)
            {
                _digits = digits;
                _leg = leg;
                _flags = flags;
                _application = application;
                _arguements = arguements;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Bind_Meta_App; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    string sflags = "";
                    if (_flags != null)
                    {
                        foreach (MetaAppFlags flg in _flags)
                            sflags += (char)flg;
                    }
                    return new NameValuePair[] { new NameValuePair("application", "bind_meta_app"),
                new NameValuePair("data", _digits.ToString() + " " + _leg.ToString() + " " + sflags + " " + _application + (_arguements == null ? "" : "::" + _arguements))};
                }
            }
            #endregion
        }

        public struct UnBindMetaApp : ICallAction
        {
            private sDialableNumber _digits;
            public sDialableNumber Digits
            {
                get { return _digits; }
            }

            public UnBindMetaApp(sDialableNumber digits, bool waitUntilDone)
            {
                _digits = digits;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.UnBind_Meta_App; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "unbind_meta_app"),
                new NameValuePair("data",_digits.ToString())};
                }
            }
            #endregion
        }

        public struct DetectSpeech : ICallAction
        {
            private string _modName;
            public string ModName
            {
                get { return _modName; }
            }

            private string _grammarName;
            public string GrammarName
            {
                get { return _grammarName; }
            }

            private string _grammarPath;
            public string GrammarPath {
                get { return _grammarPath; }
            }

            private string _address;
            public string Address
            {
                get { return _address; }
            }

            public DetectSpeech(string modName, string grammarName, string grammarPath, string address, bool waitUntilDone)
            {
                _modName = modName;
                _grammarName = grammarName;
                _grammarPath = grammarPath;
                _address = address;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Detect_Speech; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "detect_speech"),
                new NameValuePair("data",_modName+" "+_grammarName+" "+_grammarPath+(_address==null ? "" : " "+_address))};
                }
            }
            #endregion
        }

        public struct DetectSpeechGrammar : ICallAction
        {
            private string _grammarName;
            public string GrammarName
            {
                get { return _grammarName; }
            }

            private string _grammarPath;
            public string GrammarPath
            {
                get { return _grammarPath; }
            }

            public DetectSpeechGrammar(string grammarName, string grammarPath, bool waitUntilDone)
            {
                _grammarName = grammarName;
                _grammarPath = grammarPath;
                _waitUntilDone=waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Detect_Speech_Grammar; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "detect_speech"),
                new NameValuePair("data","grammar "+_grammarName+(_grammarPath==null ? "" : " "+_grammarPath))};
                }
            }
            #endregion
        }

        public struct DetectSpeechGrammarOn : ICallAction
        {
            private string _grammarName;
            public string GrammarName
            {
                get { return _grammarName; }
            }

            public DetectSpeechGrammarOn(string grammarName, bool waitUntilDone)
            {
                _grammarName = grammarName;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Detect_Speech_Grammar_On; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "detect_speech"),
                new NameValuePair("data","grammaron "+_grammarName)};
                }
            }
            #endregion
        }

        public struct DetectSpeechGrammarOff : ICallAction
        {
            private string _grammarName;
            public string GrammarName
            {
                get { return _grammarName; }
            }

            public DetectSpeechGrammarOff(string grammarName, bool waitUntilDone)
            {
                _grammarName = grammarName;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Detect_Speech_Grammar_Off; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "detect_speech"),
                new NameValuePair("data","grammaroff "+_grammarName)};
                }
            }
            #endregion
        }

        public struct DetectSpeechGrammarOffAll : ICallAction
        {

            public DetectSpeechGrammarOffAll(bool waitUntilDone)
            {
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Detect_Speech_Grammar_OffAll; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "detect_speech"),
                new NameValuePair("data","grammaralloff")};
                }
            }
            #endregion
        }

        public struct DetectSpeechNoGrammar : ICallAction
        {
            private string _grammarName;
            public string GrammarName
            {
                get { return _grammarName; }
            }

            public DetectSpeechNoGrammar(string grammarName, bool waitUntilDone)
            {
                _grammarName = grammarName;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Detect_Speech_No_Grammar; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "detect_speech"),
                new NameValuePair("data","nogrammar "+_grammarName)};
                }
            }
            #endregion
        }

        public struct DetectSpeechParam : ICallAction
        {
            private string _paramName;
            public string ParamName
            {
                get { return _paramName; }
            }

            private string _paramValue;
            public string ParamValue
            {
                get { return _paramValue; }
            }

            public DetectSpeechParam(string paramName, string paramValue, bool waitUntilDone)
            {
                _paramName = paramName;
                _paramValue = paramValue;
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Detect_Speech_Param; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "detect_speech"),
                new NameValuePair("data","param "+_paramName+" "+_paramValue)};
                }
            }
            #endregion
        }

        public struct DetectSpeechPause : ICallAction
        {
            public DetectSpeechPause(bool waitUntilDone)
            {
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Detect_Speech_Pause; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "detect_speech"),
                new NameValuePair("data","pause")};
                }
            }
            #endregion
        }

        public struct DetectSpeechResume : ICallAction
        {
            public DetectSpeechResume(bool waitUntilDone)
            {
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Detect_Speech_Resume; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "detect_speech"),
                new NameValuePair("data","resume")};
                }
            }
            #endregion
        }

        public struct DetectSpeechStartInputTimers : ICallAction
        {
            public DetectSpeechStartInputTimers(bool waitUntilDone)
            {
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Detect_Speech_Start_Input_Timers; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "detect_speech"),
                new NameValuePair("data","start_input_timers")};
                }
            }
            #endregion
        }

        public struct DetectSpeechStop : ICallAction
        {
            public DetectSpeechStop(bool waitUntilDone)
            {
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Detect_Speech_Stop; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "detect_speech"),
                new NameValuePair("data","stop")};
                }
            }
            #endregion
        }

        public struct FlushDTMF : ICallAction
        {
            public FlushDTMF(bool waitUntilDone)
            {
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Flush_DTMF; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "flush_dtmf")};
                }
            }
            #endregion
        }

        public struct QueueDTMF : ICallAction
        {
            private DTMFQueueDelays _delay;
            public DTMFQueueDelays Delay
            {
                get { return _delay; }
            }

            private string _dtmfString;
            public string DTMFString
            {
                get { return _dtmfString; }
            }

            private int? _durationMS;
            public int? DurationMS {
                get { return _durationMS; }
            }

            public QueueDTMF(DTMFQueueDelays delay, string dtmfString, int? durationMS)
            {
                _delay = delay;
                _dtmfString = dtmfString;
                _durationMS = durationMS;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Queue_DTMF; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "queue_dtmf"),
                new NameValuePair("data",(_delay == DTMFQueueDelays.None ? "" : ((char)_delay).ToString()) + _dtmfString + (_durationMS.HasValue ? "@" + _durationMS.ToString() : ""))};
                }
            }
            #endregion
        }

        public struct SendDTMF : ICallAction
        {
            private DTMFQueueDelays _delay;
            public DTMFQueueDelays Delay
            {
                get { return _delay; }
            }

            private string _dtmfString;
            public string DTMFString
            {
                get { return _dtmfString; }
            }

            private int? _durationMS;
            public int? DurationMS
            {
                get { return _durationMS; }
            }

            public SendDTMF(DTMFQueueDelays delay, string dtmfString, int? durationMS)
            {
                _delay = delay;
                _dtmfString = dtmfString;
                _durationMS = durationMS;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Send_DTMF; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "send_dtmf"),
                new NameValuePair("data",(_delay == DTMFQueueDelays.None ? "" : ((char)_delay).ToString()) + _dtmfString + (_durationMS.HasValue ? "@" + _durationMS.ToString() : ""))};
                }
            }
            #endregion
        }

        public struct StartDTMF : ICallAction
        {
            public StartDTMF(bool waitUntilDone)
            {
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Start_DTMF; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "start_dtmf")};
                }
            }
            #endregion
        }

        public struct StopDTMF : ICallAction
        {
            public StopDTMF(bool waitUntilDone)
            {
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Stop_DTMF; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "stop_dtmf")};
                }
            }
            #endregion
        }

        public struct StartDTMFGenerate : ICallAction
        {
            public StartDTMFGenerate(bool waitUntilDone)
            {
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Start_DTMF_Generate; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "start_dtmf_generate")};
                }
            }
            #endregion
        }

        public struct StopDTMFGenerate : ICallAction
        {
            public StopDTMFGenerate(bool waitUntilDone)
            {
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Stop_DTMF_Generate; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "stop_dtmf_generate")};
                }
            }
            #endregion
        }

        public struct Javascript : ICallAction
        {
            private string _scriptPath;
            public string ScriptPath
            {
                get { return _scriptPath; }
            }

            private string[] _args;
            public string[] Args
            {
                get { return _args; }
            }

            private string _variableName;
            public string VariableName{
                get{return _variableName;}
            }

            public Javascript(string scriptPath, string[] args, bool waitUntilDone)
                : this(scriptPath,args,null,waitUntilDone){}

            public Javascript(string scriptPath, string[] args,string variableName, bool waitUntilDone)
            {
                _scriptPath = scriptPath;
                _args = args;
                _waitUntilDone = waitUntilDone;
                _variableName=variableName;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Javascript; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    string sargs = "";
                    if (_args != null)
                    {
                        foreach (string str in _args)
                            sargs += str + " ";
                    }
                    if (_variableName!=null)
                        return new NameValuePair[] { new NameValuePair("application", "set"),
                            new NameValuePair("data",_variableName+"=${javascript("+_scriptPath+" "+sargs+")}")};
                    return new NameValuePair[] { new NameValuePair("application", "javascript"),
                    new NameValuePair("data",_scriptPath+" "+sargs)};
                }
            }
            #endregion
        }

        public struct Lua : ICallAction
        {
            private string _scriptPath;
            public string ScriptPath
            {
                get { return _scriptPath; }
            }

            private string[] _args;
            public string[] Args
            {
                get { return _args; }
            }

            private string _variableName;
            public string VariableName{
                get{return _variableName;}
            }

            public Lua(string scriptPath, string[] args, bool waitUntilDone)
                : this(scriptPath,args,null,waitUntilDone){}

            public Lua(string scriptPath, string[] args,string variableName, bool waitUntilDone)
            {
                _scriptPath = scriptPath;
                _args = args;
                _waitUntilDone = waitUntilDone;
                _variableName=variableName;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Lua; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    string sargs = "";
                    if (_args != null)
                    {
                        foreach (string str in _args)
                            sargs += str + " ";
                    }
                    if (_variableName!=null)
                        return new NameValuePair[] { new NameValuePair("application", "set"),
                            new NameValuePair("data",_variableName+"=${lua("+_scriptPath+" "+sargs+")}")};
                    return new NameValuePair[] { new NameValuePair("application", "lua"),
                    new NameValuePair("data",_scriptPath+" "+sargs)};
                }
            }
            #endregion
        }

        public struct Park : ICallAction
        {
            public Park(bool waitUntilDone)
            {
                _waitUntilDone = waitUntilDone;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Park; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { throw new NotImplementedException(); }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "park")};
                }
            }
            #endregion
        }

        public enum ValetParkDirections
        {
            get,
            put,
            ask
        }

        public struct ValetPark : ICallAction
        {
            private ValetParkDirections _direction;
            public ValetParkDirections Direction
            {
                get { return _direction; }
            }

            private string _parkingLot;
            public string ParkingLot
            {
                get { return _parkingLot; }
            }

            private int _timeout;
            public int Timeout
            {
                get { return _timeout; }
            }

            private string _prompt;
            public string Prompt
            {
                get { return _prompt; }
            }

            private int _stallNumber;
            public int StallNumber
            {
                get { return _stallNumber; }
            }

            private int _minStall;
            public int MinStall
            {
                get { return _minStall; }
            }

            private int _maxStall;
            public int MaxStall
            {
                get { return _maxStall; }
            }

            private ValetPark(ValetParkDirections direction, string parkingLot, int timeout, string prompt,
                int stallNumber, int minStall, int maxStall,bool waitUntilDone)
            {
                _direction = direction;
                _parkingLot = parkingLot;
                _timeout = timeout;
                _stallNumber = stallNumber;
                _prompt = prompt;
                _minStall = minStall;
                _maxStall = maxStall;
                _waitUntilDone = waitUntilDone;
            }

            public static ValetPark Ask(string parkingLot, int minStall, int maxStall, int timeout, string prompt, bool waitUntilDone)
            {
                return new ValetPark(ValetParkDirections.ask, parkingLot, timeout, prompt, -1,minStall,maxStall, waitUntilDone);
            }

            public static ValetPark Get(string parkingLot, int stallNumber, bool waitUntilDone)
            {
                return new ValetPark(ValetParkDirections.get, parkingLot, -1, null, stallNumber, -1, -1, waitUntilDone);
            }

            public static ValetPark Put(string parkingLot, int stallNumber, bool waitUntilDone)
            {
                return new ValetPark(ValetParkDirections.put, parkingLot, -1, null, stallNumber, -1, -1, waitUntilDone);
            }

            public static ValetPark Put(string parkingLot, int minStall,int maxStall, bool waitUntilDone)
            {
                return new ValetPark(ValetParkDirections.put, parkingLot, -1, null, -1, minStall, maxStall, waitUntilDone);
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Valet_Park; }
            }

            private bool _waitUntilDone;
            public bool WaitUntilDone
            {
                get { return _waitUntilDone; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    NameValuePair nvp=null;
                    switch (_direction)
                    {
                        case ValetParkDirections.put:
                            if (StallNumber >= 0)
                                nvp = new NameValuePair("data", _parkingLot + " " + _stallNumber.ToString());
                            else
                                nvp = new NameValuePair("data", _parkingLot + " auto in " + _minStall.ToString() + " " + _maxStall.ToString());
                            break;
                        case ValetParkDirections.get:
                            nvp = new NameValuePair("data", _parkingLot + " " + _stallNumber.ToString());
                            break;
                        case ValetParkDirections.ask:
                            nvp = new NameValuePair("data", _parkingLot + " ask " + _minStall.ToString() + " " + _maxStall.ToString()+" "+_timeout.ToString()+" "+_prompt);
                            break;
                    }
                    return new NameValuePair[] { new NameValuePair("application", "valet_park"),
                    nvp};
                }
            }
            #endregion
        }

        public struct ScheduleHangup : ICallAction
        {
            private int _seconds;
            public int Seconds
            {
                get { return _seconds; }
            }

            private string _reason;
            public string Reason
            {
                get { return _reason; }
            }

            public ScheduleHangup(int seconds, string reason)
            {
                _seconds = seconds;
                _reason = reason;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Schedule_Hangup; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "sched_hangup"),
                    new NameValuePair("data","+"+_seconds.ToString()+" "+_reason)};
                }
            }
            #endregion
        }

        public struct ScheduleTransfer : ICallAction
        {
            private int _seconds;
            public int Seconds
            {
                get { return _seconds; }
            }

            private sDomainExtensionPair _extension;
            public sDomainExtensionPair Extension
            {
                get { return _extension; }
            }

            private string _dialPlan;
            public string DialPlan
            {
                get { return _dialPlan; }
            }

            private string _context;
            public string Context
            {
                get { return _context; }
            }

            public ScheduleTransfer(int seconds, sDomainExtensionPair extension, string dialPlan, string context)
            {
                _seconds = seconds;
                _extension = extension;
                _dialPlan = dialPlan;
                _context=context;
            }

            #region ICallAction Members

            public CallActionTypes Command
            {
                get { return CallActionTypes.Schedule_Transfer; }
            }

            public bool WaitUntilDone
            {
                get { return false; }
            }

            public NameValuePair[] ActionXMLAttributes
            {
                get
                {
                    return new NameValuePair[] { new NameValuePair("application", "sched_transfer"),
                    new NameValuePair("data","+" + _seconds.ToString() + " " + _extension.Extension + "@" + _extension.Domain + (_dialPlan != null ? " " + _dialPlan + " " + _context : ""))};
                }
            }
            #endregion
        }

    }
}
