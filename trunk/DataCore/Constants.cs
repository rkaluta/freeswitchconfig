/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 08/11/2009
 * Time: 3:07 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore
{
	/// <summary>
	/// Description of Constants.
	/// </summary>
	public class Constants
	{
        public const string BASE_CSS_PATH = "/resources/styles";
        public const string BASE_JS_PATH = "/resources/scripts";
        public const string BASE_IMAGES_PATH = "/resources/images";
        public const string SERVER_NAME_SETTING_NAME = "ServerName";
        public const string FILE_ACCESS_LIST_SETTING_NAME = "AllowedFilesAccess";
        public const string RUNNING_USERNAME_SETTING_NAME = "Running User";
        public const string SERVER_PORT_SETTING_NAME = "Web Server Running Port";
        public const int DEFAULT_SERVER_PORT_NUMBER = 8080;
        public const string SERVER_IP_SETTING_NAME = "Web Server IP";
        internal const string DEFAULT_RUNNING_USERNAME = "apache";
        internal const string FREESWITCH_DBS_PATH_NAME = "Freeswitch_db_path";
        internal const string DEFAULT_SERVER_NAME = "localhost";
        public const string HTTP_AUTH_REALM = "FreeSwitchConfig";

        //Module Page constants
		internal const string EXTENSION_REGEX = "^\\d{1,6}$";
        public const string DIAL_PLAN_CONFIG_RIGHT = "Dial Plan Configurations";
        public const string EXTERNAL_GATEWAY_PATH_BASE = "sofia/gateway/";
		
		//Default settings
		public const string DEFAULT_BASE_BATH = "/opt/freeswitch";
		public const string DEFAULT_CONF_DIR="conf";
        public const string DEFAULT_MODULES_DIR = "autoload_configs";
		internal const string DEFAULT_MODULES_CONF_FILE = "modules.conf.xml";
		public const string DEFAULT_AUTOLOAD_CONF_DIR = "autoload_configs";
        internal const string DEFAULT_FILE_ACCESS_LIST = "wav,mp3,lua,txt";
        public const string DEFAULT_DIALPLAN_DIR = "/dialplan";
        public const string DEFAULT_SCRIPTS_DIRECTORY = "scripts";
        public const string DEFAULT_SOUNDS_DIRECTORY = "sounds";
        public const string DEFAULT_RECORDINGS_DIRECTORY = "recordings";
        public const string DEFAULT_EXTENSIONS_DIRECTORY = "directory";
        public const string DEFAULT_EXTERNAL_CONTEXT = "public";
        public static readonly string DEFAULT_VOICEMAIL_PATH = "storage"+Path.DirectorySeparatorChar+"voicemail";
        public const string DEFAULT_DBS_PATH = "db";
        public const string DEFAULT_SIP_PROFILES_PATH = "sip_profiles";
		
		//System setting codes
		public const string BASE_PATH_NAME = "BasePath";

        //Regular Expressions for built in objects
        public const string NPANXX_REGEX_STRING = "^((\\d|N|Z|X|(\\[\\d+-\\d+\\])|(\\[(\\d+,)+\\d+\\])|\\.)+\\|?(\\d|N|Z|X|(\\[\\d+-\\d+\\])|(\\[(\\d+,)+\\d+\\])|\\.)+)$";
        public const string URL_REGEX_STRING = "^([a-zA-Z0-9]([a-zA-Z0-9\\-]{0,61}[a-zA-Z0-9])?\\.)*[a-zA-Z0-9]([a-zA-Z0-9\\-]{0,61}[a-zA-Z0-9])?$";
        public const string IPADDRESS_REGEX_STRING = "(((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?))";
        public const string PORT_RANGE_REGEX = "^((102[5-9])|(10[3-9]\\d)|(1[1-9]\\d{2})|([2-9]\\d{3})|([1-5]\\d{4})|(6[0-4]\\d{3})|(65[0-4]\\d{2})|(655[0-2]\\d)|(6553[0-5]))$";

        //Basic User Rights
        public const string EXTENSION_CONFIG_RIGHT = "ExtensionControl";
        public const string SYSTEM_CONTROL_RIGHT = "SystemControl";
        public const string CDR_RIGHT = "CDRsAccess";
        public const string FILE_ACCESS_RIGHT = "FileAccess";
        public const string HOLD_MUSIC_ACCESS_RIGHT = "HoldMusicAccess";
        public const string PIN_SECURITY_ACESS_RIGHT = "PinSetsAccess";
        public const string TRUNK_SETTINGS_ACCESS_RIGHT = "TrunksAccess";
        public const string DIAL_PLAN_ACCESS_RIGHT = "DialPlanAccess";
        public const string BACKUP_ACCESS_RIGHT = "BackupRestore";
        public const string RELOAD_CONFIGURATIONS_RIGHT = "ReloadConfigurations";
        public const string CHANGE_FREESWITCH_MODULE_SETTINGS_RIGHT = "AlterFreeswitchModules";
        public const string DOMAIN_PROFILE_SETUP_RIGHT= "DomainProfileSetup";

        //byte units
        public readonly static long KB_BYTE_COUNT = 1024;
        public readonly static long MB_BYTE_COUNT = KB_BYTE_COUNT * 1024;
        public readonly static long GB_BYTE_COUNT = MB_BYTE_COUNT * 1024;
        public readonly static long TB_BYTE_COUNT = GB_BYTE_COUNT * 1024;
	}
}
