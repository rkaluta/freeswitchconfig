<include>
  <user id="$trunk.Number$">
    <params>
      <param name="password" value="$trunk.Password$"/>
    </params>
    <variables>
      <variable name="toll_allow" value="local"/>
      <variable name="accountcode" value="$trunk.Number$"/>
      <variable name="user_context" value="$trunk.UserContext.Name$"/>
      <variable name="process_cdr" value="false"/>
      <variable name="numbering_plan" value="\$\${default_country}"/>
      <variable name="default_areacode" value="\$\${default_areacode}"/>
      <variable name="default_gateway" value="\$\${default_provider}"/>
      $if(trunk.InternalCallerIDName)$
		    <variable name="effective_caller_id_name" value="$trunk.InternalCallerIDName$"/>
      $endif$
      $if(trunk.InternalCallerID)$
		    <variable name="effective_caller_id_number" value="$trunk.InternalCallerID$"/>
      $endif$
      $if(trunk.ExternalCallerIDName)$
		    <variable name="outbound_caller_id_name" value="$trunk.ExternalCallerIDName$"/>
      $endif$
      $if(trunk.ExternalCallerID)$
		    <variable name="outbound_caller_id_number" value="$trunk.ExternalCallerID$"/>
	    $endif$
	  <variable name="max_calls" value="$trunk.MaxLines$"/>
    </variables>
  </user>
</include>