using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Dbpro.Structure.Attributes;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core
{
    [Table()]
    public class FreeswitchSetting : Org.Reddragonit.Dbpro.Structure.Table
    {

        private FreeswitchSettingTypes _type;
        [PrimaryKeyField(false)]
        public FreeswitchSettingTypes Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private string _value;
        [Field(1024, false)]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }


        public static string GetDefaultValue(FreeswitchSettingTypes type)
        {
            string ret = null;
            switch (type)
            {
                case FreeswitchSettingTypes.call_debug:
                case FreeswitchSettingTypes.default_provider_register:
                case FreeswitchSettingTypes.external_auth_calls:
                case FreeswitchSettingTypes.external_ssl_enable:
                case FreeswitchSettingTypes.internal_ssl_enable:
                    ret = "false";
                    break;
                case FreeswitchSettingTypes.console_loglevel:
                    ret = "info";
                    break;
                case FreeswitchSettingTypes.default_areacode:
                    ret = "918";
                    break;
                case FreeswitchSettingTypes.default_country:
                    ret = "US";
                    break;
                case FreeswitchSettingTypes.default_password:
                case FreeswitchSettingTypes.default_provider_password:
                    ret = "1234";
                    break;
                case FreeswitchSettingTypes.default_provider:
                case FreeswitchSettingTypes.default_provider_from_domain:
                    ret = "example.com";
                    break;
                case FreeswitchSettingTypes.default_provider_contact:
                    ret = "5000";
                    break;
                case FreeswitchSettingTypes.default_provider_username:
                    ret = "joeuser";
                    break;
                case FreeswitchSettingTypes.domain:
                    ret = "$${local_ip_v4}";
                    break;
                case FreeswitchSettingTypes.domain_name:
                    ret = "$${domain}";
                    break;
                case FreeswitchSettingTypes.external_rtp_ip:
                case FreeswitchSettingTypes.external_sip_ip:
                    ret = "stun:stun.freeswitch.org";
                    break;
                case FreeswitchSettingTypes.external_sip_port:
                    ret = "5080";
                    break;
                case FreeswitchSettingTypes.external_ssl_dir:
                case FreeswitchSettingTypes.internal_ssl_dir:
                    ret = "$${base_dir}/conf/ssl";
                    break;
                case FreeswitchSettingTypes.external_tls_port:
                    ret = "5081";
                    break;
                case FreeswitchSettingTypes.global_codec_prefs:
                    ret = "G7221@32000h,G7221@16000h,G722,PCMU,PCMA,GSM";
                    break;
                case FreeswitchSettingTypes.hold_music:
                    ret = "local_stream://moh";
                    break;
                case FreeswitchSettingTypes.internal_auth_calls:
                case FreeswitchSettingTypes.unroll_loops:
                case FreeswitchSettingTypes.zrtp_secure_media:
                    ret = "true";
                    break;
                case FreeswitchSettingTypes.internal_sip_port:
                    ret = "5060";
                    break;
                case FreeswitchSettingTypes.internal_tls_port:
                    ret = "5061";
                    break;
                case FreeswitchSettingTypes.outbound_caller_id:
                    ret = "0000000000";
                    break;
                case FreeswitchSettingTypes.outbound_caller_name:
                    ret = "FreeSWITCH";
                    break;
                case FreeswitchSettingTypes.outbound_codec_prefs:
                    ret = "PCMU,PCMA,GSM";
                    break;
                case FreeswitchSettingTypes.sip_tls_version:
                    ret = "tlsv1";
                    break;
                case FreeswitchSettingTypes.use_profile:
                    ret = "internal";
                    break;
                case FreeswitchSettingTypes.xmpp_client_profile:
                    ret = "xmppc";
                    break;
                case FreeswitchSettingTypes.xmpp_server_profile:
                    ret = "xmpps";
                    break;
            }
            return ret;
        }
    }
}
