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
        $(document.body).html('<div class="header"></div><div class="header_shadow shadow"></div><div class="sidebar"></div><div class="sidebar_shadow shadow"></div><div id="MainContainer"></div><div id="PageTitleContainer"></div>');
    }
});