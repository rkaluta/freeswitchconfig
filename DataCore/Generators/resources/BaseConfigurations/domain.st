<include>
  <domain name="$domain.Name$">
    <params>
		$domain.Params:{par | <param name="$par.Name$" value="$par.Value$"/>
		}$
    </params>
    <variables>
		$domain.Variables:{var |
			<variable name="$var.Name$" value="$var.Value$"/>
		}$
    </variables>
    <users>
		<X-PRE-PROCESS cmd="include" data="$domain.Name$/*.xml"/>
    </users>
    <groups>
		$domain.Groups:{group |
		<group name="$group.Name$">
			<users>
			$group.Extensions:{number |
				<user id="$number$" type="pointer"/>
			}$
			</users>
		</group>
      }$
    </groups>
  </domain>
</include>
