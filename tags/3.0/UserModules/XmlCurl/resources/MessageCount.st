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
                $endif$
              </params>
              <variables>
                <variable name="toll_allow" value="domestic,international,local"/>
                <variable name="accountcode" value="$ext.Number$"/>
                <variable name="user_context" value="$ext.Context$"/>
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
