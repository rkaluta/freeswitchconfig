using System;
using System.Data;
using System.Configuration;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Interfaces;

using ISite = Org.Reddragonit.EmbeddedWebServer.Interfaces.Site;
using System.Threading;
using Org.Reddragonit.FreeSwitchConfig.DataCore.API;
using System.IO;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.EmbeddedWebServer.Interfaces;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.EmbeddedWebServer.Components;
using Org.Reddragonit.EmbeddedWebServer.Components.Message;

/// <summary>
/// Summary description for FileDownloader
/// </summary>
namespace Org.Reddragonit.FreeSwitchConfig.Site.Handlers
{
    public class FileDownloader : IRequestHandler
    {

        static FileDownloader()
        {
            if (Settings.Current[Constants.BASE_PATH_NAME] == null)
                Settings.Current[Constants.BASE_PATH_NAME] = Constants.DEFAULT_BASE_BATH;
        }

        public FileDownloader()
        {
        }

        public const string VM_DB = "voicemail_default.db";
        private const string VM_SELECT_QUERY = "SELECT * FROM voicemail_msgs";

        private void ImportZipDirectory(string basePath, ZipFile zf, DirectoryInfo di)
        {
            foreach (FileInfo fi in di.GetFiles())
            {
                Org.Reddragonit.FreeSwitchConfig.DataCore.Log.Trace("Adding file " + basePath + "\\" + fi.Name + " into backup file for download.");
                zf.AddFile(fi, basePath);
            }
            foreach (DirectoryInfo d in di.GetDirectories())
            {
                Org.Reddragonit.FreeSwitchConfig.DataCore.Log.Trace("Adding directory " + basePath + "\\" + d.Name + " into backup file for download.");
                ImportZipDirectory(basePath, zf,d);
            }
        }

        #region IRequestHandler Members

        public bool CanProcessRequest(HttpRequest request, Org.Reddragonit.EmbeddedWebServer.Interfaces.Site site)
        {
            return request.URL.AbsolutePath == "/FileDownloader.ashx";
        }

        public void DeInit()
        {
        }

        public void Init()
        {
        }

        public bool IsReusable
        {
            get {return true; }
        }

        public void ProcessRequest(HttpRequest request, Org.Reddragonit.EmbeddedWebServer.Interfaces.Site site)
        {
            request.SetTimeout(20 * 60 * 1000);
            if (request.Parameters["FileType"] == "Backup")
            {
                string fileName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm") + "_";
                switch (request.Parameters["Level"])
                {
                    case "Database":
                        fileName += "database";
                        break;
                    case "Voicemail":
                        fileName += "voicemail";
                        break;
                    case "Recordings":
                        fileName += "recordings";
                        break;
                    case "Sounds":
                        fileName += "sounds";
                        break;
                    case "Script":
                        fileName += "scripts";
                        break;
                    default:
                        fileName += "complete";
                        break;

                }
                request.ResponseHeaders.ContentType = "application/octet-stream";
                request.ResponseHeaders["content-disposition"] = "attachment; filename=" + fileName + ".fscbak";
                ZipFile zf = new ZipFile(fileName);
                DirectoryInfo di;
                byte[] tmp;
                if ((request.Parameters["Level"] == "Database") || (request.Parameters["Level"] == "Complete"))
                {
                    Stream ms = new MemoryStream();
                    Org.Reddragonit.Dbpro.Backup.BackupManager.BackupDataToStream(Org.Reddragonit.Dbpro.Connections.ConnectionPoolManager.GetPool(typeof(Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones.Extension)), ref ms);
                    zf.AddFile("database.rdpbk", ((MemoryStream)ms).ToArray());
                }
                //backup voicemail
                if ((request.Parameters["Level"] == "Voicemail") || (request.Parameters["Level"] == "Complete"))
                {
                    di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_VOICEMAIL_PATH);
                    ImportZipDirectory("voicemail", zf, di);
                    Log.Trace("Issueing voicemail backup command...");
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("DELETE FROM voicemail_msgs;");
                    foreach (Dictionary<string, string> row in Utility.SelectFromFreeswitchDB(VM_DB, VM_SELECT_QUERY))
                    {
                        sb.Append("INSERT INTO voicemail_msgs VALUES(");
                        sb.Append("'" + row["created_epoch"] + "',");
                        sb.Append("'" + row["read_epoch"] + "',");
                        sb.Append("'" + row["username"] + "',");
                        sb.Append("'" + row["domain"] + "',");
                        sb.Append("'" + row["uuid"] + "',");
                        sb.Append("'" + row["cid_name"] + "',");
                        sb.Append("'" + row["cid_number"] + "',");
                        sb.Append("'" + row["in_folder"] + "',");
                        sb.Append("'" + row["file_path"] + "',");
                        sb.Append("'" + row["message_len"] + "',");
                        sb.Append("'" + row["flags"] + "',");
                        if (row.ContainsKey("read_flags") && row["read_flags"] != null)
                            sb.Append("'" + row["read_flags"] + "',");
                        else
                            sb.Append("null,");
                        if (row.ContainsKey("forwarded_by") && row["forwarded_by"] != null)
                            sb.Append("'" + row["forwarded_by"] + "'");
                        else
                            sb.Append("null");
                        sb.AppendLine(");");
                    }
                    zf.AddFile("voicemail_restore.sql", ASCIIEncoding.ASCII.GetBytes(sb.ToString()));
                }
                //backup scripts
                if ((request.Parameters["Level"] == "Script") || (request.Parameters["Level"] == "Complete"))
                {
                    di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_SCRIPTS_DIRECTORY);
                    ImportZipDirectory("scripts", zf, di);
                }
                //backup sounds
                if ((request.Parameters["Level"] == "Sounds") || (request.Parameters["Level"] == "Complete"))
                {
                    di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar + Constants.DEFAULT_SOUNDS_DIRECTORY);
                    ImportZipDirectory("sounds", zf, di);
                }
                //backup recordings
                if ((request.Parameters["Level"] == "Recordings") || (request.Parameters["Level"] == "Complete"))
                {
                    di = new DirectoryInfo(Settings.Current[Constants.BASE_PATH_NAME].ToString() + Path.DirectorySeparatorChar+ Constants.DEFAULT_RECORDINGS_DIRECTORY);
                    ImportZipDirectory("recordings", zf, di);
                }

                //flush and close the final zip stream
                request.UseResponseStream(zf.ToStream());
            }
            else if (request.Parameters["clazz"] != null)
            {
                Type t = Org.Reddragonit.FreeSwitchConfig.DataCore.Utility.LocateType(request.Parameters["clazz"]);
                if (t != null)
                {
                    System.Reflection.MethodInfo mi = null;
                    foreach (System.Reflection.MethodInfo m in t.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
                    {
                        if (m.Name == request.Parameters["MethodName"] && m.GetParameters().Length == 0)
                        {
                            mi = m;
                            break;
                        }
                    }
                    if (mi == null)
                    {
                        request.ResponseStatus = HttpStatusCodes.Not_Found;
                        request.ResponseWriter.Write("Unable to locate static public method " + request.Parameters["MethodName"] + " in class type " + request.Parameters["clazz"]);
                    }
                    else
                    {
                        mi.Invoke(null, new object[0]);
                    }
                }
                else
                {
                    request.ResponseStatus = HttpStatusCodes.Not_Found;
                    request.ResponseWriter.Write("Unable to locate clazz type " + request.Parameters["clazz"]);
                }
            }
            else
            {
                request.ResponseStatus = HttpStatusCodes.Not_Found;
            }
        }

        public bool RequiresSessionForRequest(HttpRequest request, ISite site)
        {
            return false;
        }

        #endregion
    }
}