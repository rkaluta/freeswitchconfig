FreeswitchConfig.Site = $.extend(FreeswitchConfig.Site,
{
    InitPage: function() {
        $(document.body).html('<div class="header"></div><div class="header_shadow shadow"></div><div class="sidebar"></div><div class="sidebar_shadow shadow"></div><div id="MainContainer"></div><div id="PageTitleContainer"></div>');
    }
});