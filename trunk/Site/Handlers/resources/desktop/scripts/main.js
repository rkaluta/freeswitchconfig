var FreeswitchConfig = FreeswitchConfig || {};
FreeswitchConfig.Site = FreeswitchConfig.Site || {};

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

function SetOptionByText(option, text) {
    option = $(option);
    for (var y = 0; y < option.children().length; y++) {
        if ($(option.children()[y]).html()==text){
            option.val($(option.children()[y]).val());
            break;
        }
    }
}

function clone(obj) {
    if ((obj == null)||(typeof(obj)!='object'))
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

function ProduceErrorObject(content) {
    return $('<span class="error">' + content + '</span>');
}

function ShowSideMenu(){
    $('#MasterButtonsContainer').show();
    $('#MainContent').attr('style','left:210px;padding-left:10px;');
}

function HideSideMenu(){
    $('#MasterButtonsContainer').hide();
    $('#MainContent').attr('style','');
}

function CreateButton(icon,title,callback,callbackpars){
    var ret = $('<span class="NormalButton"></span>');
    ret.append('<span></span>');
    if (icon!=null){
        $(ret.children()[0]).append('<strong><img class="'+icon+'">'+title+'</strong>');
    }else{
        $(ret.children()[0]).append('<strong>'+title+'</strong>');
    }
    if (callback!=null){
        ret.bind('click',
        {callback:callback,pars:(callbackpars==undefined ? null : callbackpars),button:ret},
        function(event){
            if (event.data.pars!=null){
                event.data.callback(event.data.button,event.data.pars);    
            }else{
                event.data.callback(event.data.button);
            }
        });
    }
    return ret;
}

function AttachNPANXXHelpToInput(input) {
    var img = $('<img class="'+HELP_CLASS+'">');
    $(input).after(img);
    RegisterToolTip(img, null, NPANXX_HELP);
}

function ObjectToHidField(obj,hidField) {
    $(hidField).val(JSON.stringify(obj));
}

function HidFieldToObject(hidField) {
    var obj = new Object();
    if ($(hidField).val().length > 0) {
        obj =  JSON.parse($(hidField).val());
    }
    return obj;
}

function RegisterToolTip(element, delay, content) {
    var tmp = $('<div class="ToolTip" style="display:none;"></div>');
    if ($(element).attr('title')!=''){
        $(element).attr('bt-xTitle',$(element).attr('title'));
        $(element).attr('title','');
    }
    $(element).bt(content,
    {
        cssClass:'ToolTip',
        fill:tmp.css('background-color'),
        strokeStyle:tmp.css('border-color'),
        killTitle:false
    });
}

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

function loadjscssfile(filename, filetype) {
    if (filetype == "js") {
        if ($($.find('head')[0]).find('script[src="' + filename + '"]').length == 0) {
            var fileref = $('<script></script>');
            fileref.attr("type", "text/javascript");
            fileref.attr("src", filename);
        }
    }
    else if (filetype == "css") {
        if ($($.find('head')[0]).find('link[href="' + filename + '"]').length == 0) {
            var fileref = $('<link></link>');
            fileref.attr("rel", "stylesheet")
            fileref.attr("type", "text/css")
            fileref.attr("href", filename)
        }
    }
    if (typeof fileref != "undefined")
        $($.find('head')[0]).append(fileref)
}

function RegisterColorPicker(inputfield) {
    inputfield = $(inputfield);
    var val = inputfield.val();
    inputfield.hide();
    inputfield.attr('data-hex', 'true');
    inputfield.attr('data-text', 'hidden');
    inputfield.attr('style', 'height:20px;width:20px;');
    inputfield.mColorPicker();
    inputfield.attr('value',val);
}

function RegisterAjaxDownload(button, url, onStart, onFinish,pars) {
    $(button).bind('click',
    { start: onStart, url: url, fin: onFinish, pars: pars },
        function(event) {
            if ($('#fileDownloadDiv').length == 0) {
                var loadPanel = $('<div style="display:none;"></div>');
                loadPanel.attr('id', 'fileDownloadDiv');
                $(document.body).append(loadPanel);
            }
            if ((event.data.start != null) && (event.data.start != undefined)) {
                if ((event.data.pars != null) && (event.data.pars != undefined)) {
                    event.data.start(event.data.pars);
                } else {
                    event.data.start();
                }
            }
            $('#fileDownloadDiv').html('');
            var frm = $('<iframe></iframe>');
            if ((event.data.fin != null) && (event.data.fin != undefined)) {
                if ((event.data.pars != null) && (event.data.pars != undefined)) {
                    frm.bind('load',
                    { pars: event.data.pars, func: event.data.fin },
                    function(evnt) {
                        evnt.data.func(evnt.data.pars);
                    });
                } else {
                    frm.bind('load', event.data.fin);
                }
            }
            frm.attr('src', event.data.url);
            $('#fileDownloadDiv').append(frm);
        });
}

if ($.mobile != undefined){
    $.mobile.defaultDialogTransition = 'none';
    $.mobile.defaultPageTransition = 'none';
}

$.ajaxSetup({
    beforeSend: function(xhr) {
        xhr.setRequestHeader("If-Modified-Since", "0");
    }
});