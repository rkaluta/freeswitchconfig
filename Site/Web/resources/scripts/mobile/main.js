FreeswitchConfig.Site = $.extend(FreeswitchConfig.Site,
{
    InitPage: function() {
        if (window.innerWidth > window.innerHeight) {
            $('html').addClass('landscape');
        } else {
            $('html').addClass('portrait');
        }
        $.bind('orientationchange', function() {
            if (window.innerWidth > window.innerHeight) {
                $('html').addClass('landscape');
            } else {
                $('html').addClass('portrait');
            }
        });
        $(document.body).html('');
        $(document.body).append([
            FreeswitchConfig.Site.Skin.div.Create({ Class: 'header' }),
            FreeswitchConfig.Site.Skin.div.Create({Class:'header_shadow shadow'}),
            FreeswitchConfig.Site.Skin.div.Create({Class:'sidebar'}),
            FreeswitchConfig.Site.Skin.div.Create({Class:'sidebar_shadow shadow'}),
            FreeswitchConfig.Site.Skin.div.Create({Attributes:{'id':'MainContainer'}}),
            FreeswitchConfig.Site.Skin.div.Create({Attributes:{'id':'PageTitleContainer'}})
        ]);
    }
});