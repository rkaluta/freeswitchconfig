function CreateButton(icon, title, callback, callbackpars) {
    var ret = $('<span class="NormalButton"></span>');
    ret.append('<span class="corner"></span>');
    if (icon != null) {
        $(ret.children()[0]).append('<strong><span class="icon '+icon+'"></span>' + title + '</strong>');
    } else {
        $(ret.children()[0]).append('<strong>' + title + '</strong>');
    }
    if (callback != null) {
        ret.bind('click',
        { callback: callback, pars: (callbackpars == undefined ? null : callbackpars), button: ret },
        function(event) {
            if (event.data.pars != null) {
                event.data.callback(event.data.button, event.data.pars);
            } else {
                event.data.callback(event.data.button);
            }
        });
    }
    return ret;
}

FreeswitchConfig.Site = $.extend(FreeswitchConfig.Site, {
    AttachNPANXXHelpToInput: function(input) {
        var img = $('<img class="' + HELP_CLASS + '">');
        $(input).after(img);
        RegisterToolTip(img, null, NPANXX_HELP);
    },
    RegisterToolTip: function(element, delay, content) {
        var tmp = $('<div class="ToolTip" style="display:none;"></div>');
        if ($(element).attr('title') != '') {
            $(element).attr('bt-xTitle', $(element).attr('title'));
            $(element).attr('title', '');
        }
        $(element).bt(content,
        {
            cssClass: 'ToolTip',
            fill: tmp.css('background-color'),
            strokeStyle: tmp.css('border-color'),
            killTitle: false
        });
    },
    PreloadImages: function() {
        return [
            '/resources/icons.png',
            '/resources/images/buttonBack.png',
            '/resources/images/buttonBackAlt.png',
            '/resources/images/buttonCorners.png',
            '/resources/images/buttonCornersAlt.png',
            '/resources/images/thbackground.png'
        ];
    },
    InitPage: function() {
        $(document.body).html('<div class="header"></div><div class="header_shadow shadow"></div><div class="sidebar"></div><div class="sidebar_shadow shadow"></div><div id="MainContainer"></div><div id="PageTitleContainer"></div>');
    },
    MainContainer: function() {
        return $('#MainContainer');
    },
    TitleContainer: function() {
        return $('#PageTitleContainer');
    }
});