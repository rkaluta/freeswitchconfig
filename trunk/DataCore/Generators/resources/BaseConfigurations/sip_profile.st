<profile name="$profile.Name$">
  <aliases>
  </aliases>
  <gateways>
    <X-PRE-PROCESS cmd="include" data="$profile.Name$/*.xml"/>
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
