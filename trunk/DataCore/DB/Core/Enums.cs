using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core
{
    public enum ContextTypes
    {
        Internal,
        External
    }

    public enum SipProfileSettingTypes
    {
        debug,
        sip_trace,
        rfc2833_pt,
        sip_port,
        dialplan,
        context,
        dtmf_duration,
        inbound_codec_prefs,
        outbound_codec_prefs,
        hold_music,
        rtp_timer_name,
        enable_100rel,
        local_network_acl,
        manage_presence,
        dbname,
        presence_hosts,
        force_register_domain,
        force_register_db_domain,
        aggressive_nat_detection,
        inbound_codec_negotiation,
        nonce_ttl,
        auth_calls,
        rtp_ip,
        sip_ip,
        ext_rtp_ip,
        ext_sip_ip,
        rtp_timeout_sec,
        rtp_hold_timeout_sec,
        enable_3pcc,
        tls,
        tls_bind_params,
        tls_sip_port,
        tls_cert_dir,
        tls_version
    }

    public enum FreeswitchSettingTypes
    {
        default_password,
        domain,
        domain_name,
        hold_music,
        use_profile,
        zrtp_secure_media,
        global_codec_prefs,
        outbound_codec_prefs,
        xmpp_client_profile,
        xmpp_server_profile,
        external_rtp_ip,
        external_sip_ip,
        unroll_loops,
        outbound_caller_name,
        outbound_caller_id,
        call_debug,
        console_loglevel,
        default_areacode,
        default_country,
        default_provider,
        default_provider_username,
        default_provider_password,
        default_provider_from_domain,
        default_provider_register,
        default_provider_contact,
        sip_tls_version,
        internal_auth_calls,
        internal_sip_port,
        internal_tls_port,
        internal_ssl_enable,
        internal_ssl_dir,
        external_auth_calls,
        external_sip_port,
        external_tls_port,
        external_ssl_enable,
        external_ssl_dir
    }
}
