<?xml version="1.0"?>
<document type="freeswitch/xml">
  <section name="$section$">
    <domain name="$domain.Name$">
      <params>
        $domain.Params:{param | <param name="$param.Name$" value="$param.Value$"/>
        }$
      </params>
      <variables>
        $domain.Variables:{variable | <variable name="$variable.Name$" value="$variable.Value$"/>
        }$
      </variables>
      <groups>
        <group name="default">
          <users>
            <user id="$ext.Number$">
              <params>
                <param name="a1-hash" value="$ext.A1Hash$"/>
                $if(ext.vm)$
                  <param name="vm-a1-hash" value="$ext.vm.A1Hash$"/>
                  $if(ext.vm.MaxMessage)$
                    <param name="vm-disk-quota" value="$ext.vm.MaxMessage$"/>
                  $endif$
                  $if(ext.vm.CopyTo)$
                    <variable name="vm_cc" value="$ext.vm.CopyTo$" />
                  $endif$
                  $if(ext.vm.Email)$
                    <param name="vm-mailto" value="$ext.vm.Email$"/>
                    <param name="email-addr" value="$ext.vm.Email$"/>
                    $if(ext.vm.AttachToEmail)$
                      <param name="vm-delete-file" value="true"/>
                      <param name="vm-attach-file" value="true"/>
                    $endif$
                  $endif$
                $endif$
              </params>
              <variables>
                <variable name="toll_allow" value="domestic,international,local"/>
                <variable name="accountcode" value="$ext.Number$"/>
                <variable name="user_context" value="$ext.Context$"/>
                <variable name="process_cdr" value="false"/>
                <variable name="numbering_plan" value="\$\${default_country}"/>
                <variable name="default_areacode" value="\$\${default_areacode}"/>
                <variable name="default_gateway" value="\$\${default_provider}"/>
                $if(ext.InternalCallerIDName)$
                <variable name="effective_caller_id_name" value="$ext.InternalCallerIDName$"/>
                $else$
                <variable name="effective_caller_id_name" value="$ext.Number$"/>
                $endif$
                $if(ext.InternalCallerID)$
                <variable name="effective_caller_id_number" value="$ext.InternalCallerID$"/>
                $else$
                <variable name="effective_caller_id_number" value="$ext.Number$"/>
                $endif$
                $if(ext.ExternalCallerIDName)$
                <variable name="outbound_caller_id_name" value="$ext.ExternalCallerIDName$"/>
                $else$
                <variable name="outbound_caller_id_name" value="\$\${outbound_caller_name}"/>
                $endif$
                $if(ext.ExternalCallerID)$
                <variable name="outbound_caller_id_number" value="$ext.ExternalCallerID$"/>
                $else$
                <variable name="outbound_caller_id_number" value="\$\${outbound_caller_id}"/>
                $endif$
                $if(ext.vm)$
                <variable name="vm_message_ext" value="wav"/>
                $endif$
              </variables>
            </user>
          </users>
        </group>
      </groups>
    </domain>
  </section>
</document>
