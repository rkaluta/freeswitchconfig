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
    }
});

$.ajaxSetup({
    beforeSend: function(xhr) {
        xhr.setRequestHeader("If-Modified-Since", "0");
    }
});