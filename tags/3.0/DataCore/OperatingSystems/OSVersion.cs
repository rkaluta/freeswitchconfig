using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.OperatingSystems
{
    public class OSVersion : IComparable
    {
        private List<int> _components;


        public int Major
        {
            get { return _components[0]; }
        }

        public int Minor
        {
            get { 
                if (_components.Count>1)
                    return _components[1];
                return 0;
            }
        }

        public int Build
        {
            get
            {
                if (_components.Count > 2)
                    return _components[2];
                return 0;
            }
        }

        public int Revision
        {
            get
            {
                if (_components.Count > 3)
                    return _components[3];
                return 0;
            }
        }

        public OSVersion(string version)
        {
            _components = new List<int>();
            foreach (string str in version.Split('.'))
                _components.Add(int.Parse(str));
        }

        public static bool operator == (OSVersion x, OSVersion y)
        {
            
            return (((object)x==null && (object)y==null) ? true : (((object)x!=null && (object)y!=null) ? x.CompareTo(y) == 0 : false));
        }

        public static bool operator !=(OSVersion x, OSVersion y)
        {
            return !(x == y);
        }

        public static bool operator <(OSVersion x, OSVersion y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator <=(OSVersion x, OSVersion y)
        {
            return x.CompareTo(y) <= 0;
        }

        public static bool operator >(OSVersion x, OSVersion y)
        {
            return x.CompareTo(y) > 0;
        }

        public static bool operator >=(OSVersion x, OSVersion y)
        {
            return x.CompareTo(y) >= 0;
        }

        public override int GetHashCode()
        {
            return _components.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.CompareTo(obj) == 0;
        }

        public override string ToString()
        {
            return Major.ToString() + "." + Minor.ToString() + "." + Build.ToString()+"."+Revision.ToString();
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            OSVersion ver = (OSVersion)obj;
            if (ver.Major != this.Major)
                return this.Major.CompareTo(ver.Major);
            else if (ver.Minor != this.Minor)
                return this.Minor.CompareTo(ver.Minor);
            else if (ver.Build != this.Build)
                return this.Build.CompareTo(ver.Build);
            else
                return this.Revision.CompareTo(ver.Revision);
        }

        #endregion
    }
}
