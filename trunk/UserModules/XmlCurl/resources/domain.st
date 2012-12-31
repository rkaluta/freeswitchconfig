<?xml version="1.0"?>
<document type="freeswitch/xml">
	<section name="$section$">
		$domains:{domain |
			<domain name="$domain.Name$">
				<params>
					$domain.Params:{param | <param name="$param.Name$" value="$param.Value$"/>
					}$
				</params>
				<variables>
					$domain.Variables:{variable | <variable name="$variable.Name$" value="$variable.Value$"/>
					}$
				</variables>
				<user id="default"/>
				<groups>
          <group name="default">
            $domain.Extensions:{ext | <user id="$ext$" type="pointer"/> }$
          </group>
					$domain.Groups:{group | <group name="$group.Name$">
						$group.Extensions:{ext | <user id="$ext$" type="pointer"/>
						}$
					</group> }$
				</groups>
			</domain>
		}$
	</section>
</document>