$.mobile.defaultDialogTransition = 'none';
$.mobile.defaultPageTransition = 'none';

$(window).bind('orientationchange', function(e) {
    if ($.event.special.orientationchange.orientation() == "portrait") {
        $($.find('html')[0]).removeClass('landscape');
        $($.find('html')[0]).addClass('portrait');
    } else {
        $($.find('html')[0]).addClass('landscape');
        $($.find('html')[0]).removeClass('portrait');
    }
});

function CreateButton(icon, title, callback, callbackpars) {
    var ret = $('<a class="ui-btn-left ui-mini ui-btn ui-shadow ui-btn-corner-all ui-btn-icon-left ui-btn-up-b" href="#"><span class="ui-btn-inner ui-btn-corner-all"><span class="ui-btn-text">' + title + '</span><span class="ui-icon ui-icon-shadow"> </span></span></a>');
    if (icon != null) {
        if (ret.find('span.ui-icon').length == 0) {
            ret.append('<span class="ui-icon ' + icon + ' ui-icon-shadow"> </span>');
        } else {
            $(ret.find('span.ui-icon')[0]).addClass(icon);
        }
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
            '/resources/images/icons.png',
            '/resources/images/thbackground.png'
        ];
    },
    InitPage: function() {
        $(document.body).html($('<div data-role="page" id="MainPage"><div data-role="header"><img src="/resources/images/mobile/logo.png"/><h3 id="PageTitleContainer"/></div><div data-role="content" id="MainContainer"></div><div data-role="footer"></div></div>'));
        $('#MainPage').page();
        $(document.body).append($('<div id="alertPanel" data-role="popup" id="popupDialog" data-overlay-theme="a" data-theme="b" data-dismissible="false" style="max-width:400px;" class="ui-corner-all"><div data-role="header" data-theme="b" class="ui-corner-top"><h1>ALERT</h1></div><div data-role="content" data-theme="d" class="ui-corner-bottom ui-content"><p></p><a href="#" data-role="button" data-inline="true" data-rel="back" data-theme="b" name="Okay">Okay</a></div></div>'));
        $('#alertPanel').page();
        $(document.body).append($('<div id="LoadingPanel" data-role="popup" id="popupDialog" data-overlay-theme="a" data-theme="b" data-dismissible="false" style="max-width:400px;" class="ui-corner-all"><div data-role="content" data-theme="d" class="ui-corner-bottom ui-content"><span class="ui-icon ui-icon-loading"></span><h1>LOADING...</h1></div></div>'));
        $('#LoadingPanel').page();
        if (window.innerHeight > window.innerWidth) {
            $($.find('html')[0]).removeClass('landscape');
            $($.find('html')[0]).addClass('portrait');
        } else {
            $($.find('html')[0]).addClass('landscape');
            $($.find('html')[0]).removeClass('portrait');
        }
    },
    MainContainer: function() {
        return $('#MainContainer');
    },
    TitleContainer: function() {
        return $('#PageTitleContainer');
    }
});