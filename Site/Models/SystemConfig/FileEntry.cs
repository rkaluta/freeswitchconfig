using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Files;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;
using Org.Reddragonit.FreeSwitchConfig.DataCore;

namespace Org.Reddragonit.FreeSwitchConfig.Site.Models.SystemConfig
{
    [ModelJSFilePath("/resources/scripts/Core/SystemConfig/FileAccess.js")]
    [ModelRoute("/core/models/sysconfig/FileEntry")]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.CollectionView | ModelBlockJavascriptGenerations.EditForm | ModelBlockJavascriptGenerations.View)]
    [ModelNamespace("FreeswitchConfig.Core")]
    public class FileEntry : IModel
    {
        private string _name;
        [ReadOnlyModelProperty()]
        public string Name
        {
            get { return _name; }
        }

        private string _relativeFilePath;
        [ReadOnlyModelProperty()]
        public string RelativeFilePath
        {
            get { return _relativeFilePath; }
        }

        private bool _isFile;
        [ReadOnlyModelProperty()]
        public bool IsFile
        {
            get { return _isFile; }
        }

        private string _content;
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        private string _uploadedFileID;
        public string UploadedFileID
        {
            get { return _uploadedFileID; }
            set { _uploadedFileID = value; }
        }

        private FileEntry(File f)
        {
            _name = f.FileName;
            _relativeFilePath = f.RelativePath;
            _content = null;
            _isFile = f.IsFile;
        }

        private FileEntry(string path)
        {
            File f = new File(path);
            _name = f.FileName;
            _relativeFilePath = f.RelativePath;
            if (f.FileName.EndsWith(".txt")||f.FileName.EndsWith(".lua"))
                _content = f.ReadToEnd();
            _isFile = f.IsFile;
        }

        [ModelLoadMethod()]
        public static FileEntry Load(string path)
        {
            if (!User.Current.HasRight(Constants.FILE_ACCESS_RIGHT))
                return null;
            return new FileEntry(new File(path));
        }

        [ModelLoadAllMethod()]
        public static List<FileEntry> LoadAll()
        {
            if (!User.Current.HasRight(Constants.FILE_ACCESS_RIGHT))
                return null;
            return LoadInPath(null);
        }

        [ModelListMethod("/core/models/search/sysconfig/FileEntry/{0}")]
        public static List<FileEntry> LoadInPath(string path)
        {
            if (!User.Current.HasRight(Constants.FILE_ACCESS_RIGHT))
                return null;
            List<FileEntry> ret = new List<FileEntry>();
            File f = new File(path);
            foreach (File fi in f.Children)
                ret.Add(new FileEntry(fi));
            return ret;
        }

        [ModelDeleteMethod()]
        public bool Delete()
        {
            if (!User.Current.HasRight(Constants.FILE_ACCESS_RIGHT))
                throw new UnauthorizedAccessException();
            return new File(id).Delete();
        }

        [ModelUpdateMethod()]
        public bool Update()
        {
            if (!User.Current.HasRight(Constants.FILE_ACCESS_RIGHT))
                throw new UnauthorizedAccessException();
            byte[] tmp;
            if (_uploadedFileID == null)
            {
                tmp = System.Text.ASCIIEncoding.ASCII.GetBytes(_content);
                return new File(id).Update(tmp);
            }
            else
            {
                tmp = FileCache.GetFileFromCache(int.Parse(_uploadedFileID));
                if (tmp == null)
                    return false;
                return new File(id).Update(tmp);
            }
        }

        #region IModel Members

        public string id
        {
            get { return RelativeFilePath; }
        }

        #endregion
    }
}
