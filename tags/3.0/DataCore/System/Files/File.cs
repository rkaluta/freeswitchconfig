using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using System.Threading;
using System.IO;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes;
using System.Text.RegularExpressions;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.System.Files
{
    
    public class File : IConvertible,IComparable,IDiagnosable
    {
        private const string AUDIO_PATH = "sounds";
        private const string SCRIPT_PATH = "scripts";
        private const string VOICEMAIL_PATH = "storage";

        static File()
        {
            if (Settings.Current[Constants.FILE_ACCESS_LIST_SETTING_NAME] == null)
                Settings.Current[Constants.FILE_ACCESS_LIST_SETTING_NAME] = Constants.DEFAULT_FILE_ACCESS_LIST;
        }

        public static implicit operator string(File arg)
        {
            if (arg == null)
                return null;
            return arg.ToString();
        }

        public static implicit operator File(string arg)
        {
            if (arg == null)
                return null;
            return new File(arg);
        }

        [DiagnosticFunctionAttribute("File System")]
        public static List<string> RunDiagnostics()
        {
            List<string> ret = new List<string>();
            if (Settings.Current[Constants.BASE_PATH_NAME] == null)
                Settings.Current[Constants.BASE_PATH_NAME] = Constants.DEFAULT_BASE_BATH;
            DirectoryInfo di = new DirectoryInfo((string)Settings.Current[Constants.BASE_PATH_NAME]);
            if (!di.Exists)
                ret.Add("Unable to locate base freeswitch operating path at " + di.FullName + ".  Please corrected the system setting " + Constants.BASE_PATH_NAME);
            else
            {
                ret.Add("Located the base freeswitch operating path at " + di.FullName);
                if (di.GetDirectories(AUDIO_PATH).Length == 0)
                    ret.Add("Unable to locate the freeswitch audio files path at " + di.FullName + Path.DirectorySeparatorChar + AUDIO_PATH);
                else
                    ret.Add("Locate the freeswitch audio files path at " + di.FullName + Path.DirectorySeparatorChar + AUDIO_PATH);
                if (di.GetDirectories(SCRIPT_PATH).Length == 0)
                    ret.Add("Unable to locate the freeswitch script files path at " + di.FullName + Path.DirectorySeparatorChar + SCRIPT_PATH);
                else
                    ret.Add("Locate the freeswitch script files path at " + di.FullName + Path.DirectorySeparatorChar + SCRIPT_PATH);
                if (di.GetDirectories(VOICEMAIL_PATH).Length == 0)
                    ret.Add("Unable to locate the freeswitch voicemail files path at " + di.FullName + Path.DirectorySeparatorChar + VOICEMAIL_PATH);
                else
                    ret.Add("Locate the freeswitch voicemail files path at " + di.FullName + Path.DirectorySeparatorChar + VOICEMAIL_PATH);
            }
            return ret;
        }

        private string _relativePath;
        public string RelativePath
        {
            get { return _relativePath; }
        }

        public File()
        {
            _relativePath = "";
        }

        public File(string path)
        {
            _relativePath = (path==null ? "%BASEPATH%" : path);
        }

        public List<File> Children
        {
            get
            {
                List<File> ret = new List<File>();
                if (!IsFile)
                {
                    if (_relativePath == "%BASEPATH%")
                    {
                        ret.Add(new File("%AUDIOFILES%"));
                        ret.Add(new File("%SCRIPTS%"));
                        ret.Add(new File("%VOICEMAIL%"));
                    }
                    else
                    {
                        DirectoryInfo di = (DirectoryInfo)Convert.ChangeType(this, typeof(DirectoryInfo));
                        foreach (DirectoryInfo d in di.GetDirectories())
                            ret.Add(new File(_relativePath + Path.DirectorySeparatorChar + d.Name));
                        if (Settings.Current[Constants.FILE_ACCESS_LIST_SETTING_NAME] == null)
                            Settings.Current[Constants.FILE_ACCESS_LIST_SETTING_NAME] = Constants.DEFAULT_FILE_ACCESS_LIST;
                        Regex reg = new Regex("^.+\\.(" + ((string)Settings.Current[Constants.FILE_ACCESS_LIST_SETTING_NAME]).Replace(",", "|") + ")$", RegexOptions.Compiled | RegexOptions.ECMAScript);
                        foreach (FileInfo fi in di.GetFiles())
                        {
                            if (reg.IsMatch(fi.Name))
                                ret.Add(new File(_relativePath + Path.DirectorySeparatorChar + fi.Name));
                        }
                    }
                }
                return ret;
            }
        }

        public string FileName
        {
            get
            {
                if (_relativePath.Contains(Path.DirectorySeparatorChar.ToString()))
                    return _relativePath.Substring(_relativePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                return _relativePath;
            }
        }

        public bool IsFile
        {
            get
            {
                return FileName.Contains(".");
            }
        }

        public override string ToString()
        {
            return _relativePath;
        }

        public override int GetHashCode()
        {
            return _relativePath.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return _relativePath == ((File)obj)._relativePath;
        }

        string _actualPath = null;
        public string ActualPath
        {
            get
            {
                if (_actualPath == null)
                {
                    if (Settings.Current[Constants.BASE_PATH_NAME] == null)
                        Settings.Current[Constants.BASE_PATH_NAME] = Constants.DEFAULT_BASE_BATH;
                    _actualPath = _relativePath.Replace("%AUDIOFILES%", "%BASEPATH%" + Path.DirectorySeparatorChar + AUDIO_PATH).Replace("%SCRIPTS%", "%BASEPATH%" + Path.DirectorySeparatorChar + SCRIPT_PATH).Replace("%VOICEMAIL%", "%BASEPATH%" + Path.DirectorySeparatorChar + VOICEMAIL_PATH).Replace("%BASEPATH%", Settings.Current[Constants.BASE_PATH_NAME].ToString());
                }
                return _actualPath;
            }
        }

        #region Operators

        public static bool operator ==(File x, File y)
        {
            string sx = null;
            try
            {
                sx = x._relativePath;
            }
            catch (Exception e)
            {
                sx = null;
            }
            string sy = null;
            try
            {
                sy = y._relativePath;
            }
            catch (Exception e)
            {
                sy = null;
            }
            if ((sx == null) && (sy == null))
                return true;
            else if (sy == null)
                return false;
            else if (sx == null)
                return false;
            return sy == sx;
        }

        public static bool operator !=(File x, File y)
        {
            return !(x == y);
        }

        public static bool operator <(File x, File y)
        {
            return ((IComparable)x).CompareTo(y) < 0;
        }

        public static bool operator <=(File x, File y)
        {
            return ((IComparable)x).CompareTo(y) <= 0;
        }

        public static bool operator >(File x, File y)
        {
            return ((IComparable)x).CompareTo(y) > 0;
        }

        public static bool operator >=(File x, File y)
        {
            return ((IComparable)x).CompareTo(y) >= 0;
        }
        #endregion

        #region IConvertible Members

        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.String;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return this.ToString()[0];
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return this.ToString();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == typeof(FileInfo))
                return new FileInfo(this.ActualPath);
            else if (conversionType == typeof(File))
                return new File(_relativePath);
            else if (conversionType == typeof(DirectoryInfo))
                return new DirectoryInfo(this.ActualPath);
            else if (conversionType == typeof(string) || conversionType == typeof(String))
                return ToString();
            throw new InvalidCastException();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            return this.ToString().CompareTo(obj.ToString());
        }

        #endregion

        public bool Delete()
        {
            try{
                if (IsFile)
                    new FileInfo(ActualPath).Delete();
                else
                    new DirectoryInfo(ActualPath).Delete(true);
            }catch(Exception e){
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                Log.Error(e);
                return false;
            }
            return true;
        }

        public bool Update(byte[] tmp)
        {
            try
            {
                if (IsFile)
                {
                    BinaryWriter bw = new BinaryWriter(new FileStream(ActualPath, FileMode.Create, FileAccess.Write, FileShare.None));
                    bw.Write(tmp);
                    bw.Flush();
                    bw.Close();
                }
            }
            catch (Exception e)
            {
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                Log.Error(e);
                return false;
            }
            return IsFile;
        }

        public string ReadToEnd()
        {
            string ret = null;
            if (IsFile)
            {
                try
                {
                    StreamReader sr = new StreamReader(new FileStream(ActualPath, FileMode.Open, FileAccess.Read, FileShare.Read));
                    ret = sr.ReadToEnd();
                    sr.Close();
                }
                catch (Exception e)
                {
                    EventController.TriggerEvent(new ErrorOccuredEvent(e));
                    Log.Error(e);
                }
            }
            return ret;
        }
    }
}
