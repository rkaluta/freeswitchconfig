<profiles>
  $profiles:{profile |
  <profile name="$profile.Name$">
    <aliases>
    </aliases>
    <gateways>
      $profile.Trunks:{trunk |
      <gateway name="$trunk.Name$">
        <param name="username" value="$trunk.UserName$"/>
        <param name="password" value="$trunk.Password$"/>
        <param name="register" value="$trunk.Register$"/>
        <param name="register-transport" value="$trunk.RegistrationType$"/>
        <param name="retry-seconds" value="$trunk.RetrySeconds$"/>
        <param name="context" value="$trunk.Context$"/>
        <param name="caller-id-in-from" value="false"/>
        <param name="contact-params" value="tport=tcp"/>
        <param name="ping" value="$trunk.PingInterval$"/>
        <param name="realm" value="$trunk.Realm$"/>
        $if(trunk.FromUser)$
        <param name="from-user" value="$trunk.FromUser$"/>
        $endif$
        $if(trunk.FromDomain)$
        <param name="from-domain" value="$trunk.FromDomain$"/>
        $endif$
        $if(trunk.Extension)$
        <param name="extension" value="$trunk.Extension$"/>
        $endif$
        $if(trunk.Proxy)$
        <param name="proxy" value="$trunk.Proxy$"/>
        $endif$
        $if(trunk.RegisterProxy)$
        <param name="register-proxy" value="$trunk.RegisterProxy$"/>
        $endif$
        $if(trunk.ExpireSeconds)$
        <param name="expire-seconds" value="$trunk.ExpireSeconds$"/>
        $endif$
      </gateway>
      }$
    </gateways>
    <domains>
      $profile.Domains:{dom |
      <domain name="$dom.Name$" parse="false"/>
      }$
    </domains>
    <settings>
      $profile.Settings:{setting |
      <param name="$replace($setting.Name$,'=','-')$" value="$setting.Value$"/>
      }$
    </settings>
  </profile>
  }$
</profiles>