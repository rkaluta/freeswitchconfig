using System;
using System.Collections.Generic;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using System.IO;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.FreeSwitchConfig.DataCore.API;
using Constants = Org.Reddragonit.FreeSwitchConfig.DataCore.Constants;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Files;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using Org.Reddragonit.EmbeddedWebServer.Attributes;
using Org.Reddragonit.FreeSwitchConfig.Site.Handlers;
using Org.Reddragonit.Dbpro.Backup;
using Org.Reddragonit.EmbeddedWebServer.Components;

/// <summary>
/// Summary description for BackupRestoreService
/// </summary>
namespace Org.Reddragonit.FreeSwitchConfig.Site.Services.SystemConfig
{
    [EmbeddedServiceNamespace("FreeswitchConfig.Services")]
    public class BackupRestoreService : EmbeddedService
    {
        public enum RestoreTypes{
            Database,
            Voicemail,
            Recordings,
            Sounds,
            Script,
            Complete
        }

        protected override bool IsValidAccess(string functionName)
        {
            return Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users.User.Current.HasRight(Constants.BACKUP_ACCESS_RIGHT);
        }

        public BackupRestoreService()
        {
        }

        [WebMethod(true)]
        public bool RestoreData(int cacheID, RestoreTypes type)
        {
            byte[] tmp = FileCache.GetFileFromCache(cacheID);
            if (tmp == null)
                return false;
            bool ret=true;
            ZipFile zf = new ZipFile(new MemoryStream(tmp), true);
            switch (type)
            {
                case RestoreTypes.Database:
                    foreach (ZipFile.sZippedFile zfi in zf.Files)
                    {
                        if (zfi.Name == "database.rdpbk")
                        {
                            Stream ms = new MemoryStream(zfi.Data);
                            ret = BackupManager.RestoreDataFromStream(ConnectionPoolManager.GetConnection(typeof(Extension)),
                                ref ms);
                        }
                    }
                    break;
                case RestoreTypes.Voicemail:
                    Log.Trace("Cleaning out voicemail directory...");
                    CleanDirectory(Settings.Current[Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_VOICEMAIL_PATH);
                    Log.Trace("Examining all content within uploaded zip file...");
                    foreach(sZippedFolder fold in zf.Folders)
                    {
                        if (fold.Name == "voicemail")
                        {
                            RestoreFiles(fold, Settings.Current[Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_VOICEMAIL_PATH);
                            break;
                        }
                    }
                    foreach (ZipFile.sZippedFile file in zf.Files)
                    {
                        if (file.Name == "voicemail_restore.sql")
                        {
                            Utility.ExecuteCommandToFreeswitchDB(FileDownloader.VM_DB, System.Text.ASCIIEncoding.ASCII.GetString(file.Data));
                            break;
                        }
                    }
                    break;
                case RestoreTypes.Recordings:
                    Log.Trace("Cleaning out recordings directory...");
                    CleanDirectory(Settings.Current[Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.DEFAULT_RECORDINGS_DIRECTORY);
                    Log.Trace("Examining all content within uploaded zip file...");
                    foreach (sZippedFolder fold in zf.Folders)
                    {
                        if (fold.Name == "recordings")
                        {
                            RestoreFiles(fold, Settings.Current[Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.DEFAULT_RECORDINGS_DIRECTORY);
                        }
                    }
                    break;
                case RestoreTypes.Script:
                    Log.Trace("Cleaning out scripts directory...");
                    CleanDirectory(Settings.Current[Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.DEFAULT_SCRIPTS_DIRECTORY);
                    Log.Trace("Examining all content within uploaded zip file...");
                    foreach (sZippedFolder fold in zf.Folders)
                    {
                        if (fold.Name == "scripts")
                        {
                            RestoreFiles(fold,Settings.Current[Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.DEFAULT_SCRIPTS_DIRECTORY);
                        }
                    }
                    break;
                case RestoreTypes.Sounds:
                    Log.Trace("Cleaning out sounds directory...");
                    CleanDirectory(Settings.Current[Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.DEFAULT_SOUNDS_DIRECTORY);
                    Log.Trace("Examining all content within uploaded zip file...");
                    foreach (sZippedFolder fold in zf.Folders)
                    {
                        if (fold.Name == "sounds")
                        {
                            RestoreFiles(fold, Settings.Current[Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.DEFAULT_SOUNDS_DIRECTORY);
                        }
                    }
                    break;
                case RestoreTypes.Complete:
                    Log.Trace("Cleaning out directories to perform complete restore...");
                    CleanDirectory(Settings.Current[Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_VOICEMAIL_PATH);
                    CleanDirectory(Settings.Current[Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.DEFAULT_RECORDINGS_DIRECTORY);
                    CleanDirectory(Settings.Current[Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.DEFAULT_SCRIPTS_DIRECTORY);
                    CleanDirectory(Settings.Current[Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.DEFAULT_SOUNDS_DIRECTORY);
                    Log.Trace("Restoring data from the zip file to the system...");
                    foreach (sZippedFolder fold in zf.Folders)
                    {
                        switch (fold.Name)
                        {
                            case "voicemail":
                                RestoreFiles(fold, Settings.Current[Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_VOICEMAIL_PATH);
                                break;
                            case "recordings":
                                RestoreFiles(fold, Settings.Current[Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.DEFAULT_RECORDINGS_DIRECTORY);
                                break;
                            case "scripts":
                                RestoreFiles(fold, Settings.Current[Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.DEFAULT_SCRIPTS_DIRECTORY);
                                break;
                            case "sounds":
                                RestoreFiles(fold, Settings.Current[Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Org.Reddragonit.FreeSwitchConfig.DataCore.Constants.DEFAULT_SOUNDS_DIRECTORY);
                                break;
                        }
                    }
                    foreach (ZipFile.sZippedFile zfi in zf.Files)
                    {
                        switch (zfi.Name)
                        {
                            case "database.rdpbk":
                                Stream ms = new MemoryStream(zfi.Data);
                                ret = BackupManager.RestoreDataFromStream(ConnectionPoolManager.GetConnection(typeof(Extension)),
                                    ref ms);
                                break;
                            case "voicemail_restore.sql":
                                Utility.ExecuteCommandToFreeswitchDB(FileDownloader.VM_DB, System.Text.ASCIIEncoding.ASCII.GetString(zfi.Data));
                                break;
                        }
                    }
                    break;
            }
            zf.Close();
            return ret;
        }

        private void RestoreFiles(sZippedFolder fold, string basePath)
        {
            foreach (sZippedFolder fld in fold.Folders)
            {
                DirectoryInfo di = new DirectoryInfo(basePath + Path.DirectorySeparatorChar + fld.Name);
                if (!di.Exists)
                    di.Create();
                RestoreFiles(fld, di.FullName);
            }
            foreach (ZipFile.sZippedFile file in fold.Files)
            {
                FileStream fs = new FileStream(basePath + Path.DirectorySeparatorChar + file.Name, FileMode.Create, FileAccess.Write, FileShare.None);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(file.Data);
                bw.Flush();
                bw.Close();
            }
        }

        private void CleanDirectory(string basePath)
        {
            DirectoryInfo di = new DirectoryInfo(basePath);
            foreach (DirectoryInfo d in di.GetDirectories())
            {
                d.Delete(true);
            }
            foreach (FileInfo fi in di.GetFiles())
            {
                fi.Delete();
            }
        }
    }
}