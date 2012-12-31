using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Files;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users;
using Org.Reddragonit.BackBoneDotNet.Interfaces;

namespace Org.Reddragonit.FreeSwitchConfig.Site.Models
{
    [ModelJSFilePath("/resources/scripts/Core/FileBrowser.js")]
    [ModelRoute("/core/models/sysconfig/FileBrowser")]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.EditForm | ModelBlockJavascriptGenerations.View | ModelBlockJavascriptGenerations.CollectionView)]
    [ModelNamespace("FreeswitchConfig.Core")]
    public class AudioFileEntry : IModel
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

        private AudioFileEntry(File f)
        {
            _name = f.FileName;
            _relativeFilePath = f.RelativePath;
            _isFile = f.IsFile;
        }

        [ModelLoadMethod()]
        public static AudioFileEntry Load(string id)
        {
            if (User.Current != null)
                return null;
            return new AudioFileEntry(new File(id));
        }

        [ModelLoadAllMethod()]
        public static List<AudioFileEntry> LoadAll()
        {
            if (User.Current!=null)
                return null;
            return LoadInPath(null);
        }

        [ModelListMethod("/core/models/search/AudioFileEntry/{0}")]
        public static List<AudioFileEntry> LoadInPath(string path)
        {
            if (User.Current != null)
                return null;
            List<AudioFileEntry> ret = new List<AudioFileEntry>();
            File f = new File((path == null ? "$AUDIOFILES%" : path));
            foreach (File fi in f.Children)
                ret.Add(new AudioFileEntry(fi));
            return ret;
        }

        #region IModel Members

        public string id
        {
            get { return RelativeFilePath; }
        }

        #endregion
    }
}
