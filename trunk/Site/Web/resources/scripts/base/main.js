function CreateNameSpace(name) {
    var splitted = name.split('.');
    var tmp = splitted[0];
    if (window[tmp] == undefined) {
        window[tmp] = new Object();
    }
    var curObj = window[tmp];
    for (var x = 1; x < splitted.length; x++) {
        if (curObj[splitted[x]] == undefined) {
            curObj[splitted[x]] = {};
        }
        tmp += '.' + splitted[x];
        curObj = curObj[splitted[x]];
    }
}

function CreateButton(icon, title, callback, callbackpars) {
    var ret = $('<span class="NormalButton"></span>');
    ret.append('<span class="corner"></span>');
    if (icon != null) {
        $(ret.children()[0]).append('<strong><span class="icon ' + icon + '"></span>' + title + '</strong>');
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


function clone(obj) {
    if ((obj == null) || (typeof (obj) != 'object'))
        return obj;

    var temp = new Object(); // changed

    for (var key in obj) {
        if (typeof (obj[key]) == 'object') {
            temp[key] = clone(obj[key]);
        } else {
            temp[key] = obj[key];
        }
    }
    return temp;
}

CreateNameSpace('FreeswitchConfig.Site');

FreeswitchConfig.Site = $.extend(FreeswitchConfig.Site,
{
    clearChangedDomain: function() {
        $(window).unbind('domainChanged');
    },
    setChangedDomain: function(data, method) {
        $(window).bind('domainChanged', data, method);
    },
    triggerDomainChange: function() {
        $(window).trigger('domainChanged');
    },
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
    MainContainer: function() {
        return $('#MainContainer');
    },
    TitleContainer: function() {
        return $('#PageTitleContainer');
    }
});

$.ajaxSetup({
    beforeSend: function(xhr) {
        xhr.setRequestHeader("If-Modified-Since", "0");
    }
});