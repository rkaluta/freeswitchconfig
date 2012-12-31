<?xml version="1.0"?>
<document type="freeswitch/xml">
  <section name="$section$">
    <domain name="$extension.DomainName$">
		<user id="$extension.Number$">
			<param name="reverse-auth-user" value="$extension.Number$"/>
			<param name="reverse-auth-a1-hash" value="$extension.A1Hash$"/>
		</user>
    </domain>
  </section>
</document>