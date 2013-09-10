FreeswitchConfig.Site = $.extend(FreeswitchConfig.Site,
{
    InitPage: function() {
        $(document.body).html('');
        $(document.body).append([
            FreeswitchConfig.Site.Skin.div.Create({ Class: 'header' }),
            FreeswitchConfig.Site.Skin.div.Create({ Class: 'header_shadow shadow' }),
            FreeswitchConfig.Site.Skin.div.Create({ Class: 'sidebar' }),
            FreeswitchConfig.Site.Skin.div.Create({ Class: 'sidebar_shadow shadow' }),
            FreeswitchConfig.Site.Skin.div.Create({ Attributes: { 'id': 'MainContainer'} }),
            FreeswitchConfig.Site.Skin.div.Create({ Attributes: { 'id': 'PageTitleContainer'} })
        ]);
    }
});