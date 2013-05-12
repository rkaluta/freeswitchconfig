FreeswitchConfig.Site = $.extend(FreeswitchConfig.Site, { Modals: {
    _pageHistory: [],
    ShowLoading: function() {
        if ($.mobile.activePage != 'LoadingPanel' && $.mobile.activePage != undefined) {
            FreeswitchConfig.Site.Modals._pageHistory.push($.mobile.activePage);
            $($.mobile.activePage).removeClass('ui-page-active');
        }
        $($('#LoadingPanel').find('h1')[0]).html('LOADING...');
        $('#LoadingPanel').addClass('ui-page-active');
    },
    HideLoading: function() {
        $('#LoadingPanel').removeClass('ui-page-active');
        if (FreeswitchConfig.Site.Modals._pageHistory.length == 0) {
            $('#MainPage').addClass('ui-page-active');
        } else {
            $(FreeswitchConfig.Site.Modals._pageHistory.pop()).addClass('ui-page-active');
        }
    },
    ShowUpdating: function() {
        if ($.model.activePage != 'LoadingPanel' && $.mobile.activePage != undefined) {
            FreeswitchConfig.Site.Modals._pageHistory.push($.mobile.activePage);
            $($.mobile.activePage).removeClass('ui-page-active');
        }
        $($('#LoadingPanel').find('h1')[0]).html('UPDATING...');
        $('#LoadingPanel').addClass('ui-page-active');
    },
    HideUpdating: function() {
        $('#LoadingPanel').removeClass('ui-page-active');
        if (FreeswitchConfig.Site.Modals._pageHistory.length == 0) {
            $('#MainPage').addClass('ui-page-active');
        } else {
            $(FreeswitchConfig.Site.Modals._pageHistory.pop()).addClass('ui-page-active');
        }
    },
    alert: function(msg, callback) {
        FreeswitchConfig.Site.Modals._pageHistory.push($.mobile.activePage);
        var dvPanel = $('#alertDialog');
        dvPanel.focus();
        $(dvPanel.find('p')[0]).html(msg);
        var butOkay = $(dvPanel.find('a[name="Okay"]:last')[0]);
        butOkay.unbind('click');
        butOkay.bind('click', { callback: callback },
        function(event) {
            $.mobile.changePage(FreeswitchConfig.Site.Modals._pageHistory[FreeswitchConfig.Site.Modals._pageHistory.length - 1]);
            FreeswitchConfig.Site.Modals._pageHistory.splice(FreeswitchConfig.Site.Modals._pageHistory.length - 1, 1);
            if (event.data.callback != undefined) {
                if (event.data.callback != null) {
                    event.data.callback();
                }
            }
        });
        $.mobile.changePage(dvPanel);
    },
    confirm: function(txt, callback, title) {
        if ($('#ConfirmDialog').length == 0) {
            $(document.body).append($('<div id="ConfirmDialog" data-role="dialog"><div data-role="header" data-theme="a"><h1></h1></div><div data-role="content" data=theme="b" id="ConfirmDialogContent"></div><div data-role="footer" data-theme="a"><a data-role="button" data-theme="b" data-icon="accept">Okay</a><a data-role="button" data-theme="b" data-icon="delete">Cancel</a></div></div>'));
            $('#ConfirmDialog').page();
            $($('#ConfirmDialog').find('a:first')[0]).remove();
            FreeswitchConfig.Site.Modals._preConfirmPage = $.find('div.ui-page-active');
        } else if ($('#ConfirmDialog').attr('class').indexOf('ui-page-active') < 0) {
            FreeswitchConfig.Site.Modals._preConfirmPage = $.find('div.ui-page-active');
        }
        if (FreeswitchConfig.Site.Modals._preConfirmPage.length > 0) {
            $(FreeswitchConfig.Site.Modals._preConfirmPage).removeClass('ui-page-active');
        }
        title = (title == undefined ? 'Confirm' : (title == null ? 'Confirm' : title));
        $($('#ConfirmDialog').find('h1')[0]).html(title);
        $('#ConfirmDialogContent').html('');
        var buttons = $('#ConfirmDialog').find('a');
        $(buttons).unbind('click');
        $(buttons[1]).bind('click',
            { callback: callback },
            function(event) {
                $('#ConfirmDialog').removeClass('ui-page-active');
                if (FreeswitchConfig.Site.Modals._preConfirmPage.length > 0) {
                    $(FreeswitchConfig.Site.Modals._preConfirmPage).addClass('ui-page-active');
                }
                FreeswitchConfig.Site.Modals._preConfirmPage = [];
                if (event.data.callback != undefined) {
                    if (event.data.callback != null) {
                        event.data.callback(false);
                    }
                }
            }
        );
        $(buttons[0]).bind('click',
            { callback: callback },
            function(event) {
                $('#ConfirmDialog').removeClass('ui-page-active');
                if (FreeswitchConfig.Site.Modals._preConfirmPage.length > 0) {
                    $(FreeswitchConfig.Site.Modals._preConfirmPage).addClass('ui-page-active');
                }
                FreeswitchConfig.Site.Modals._preConfirmPage = [];
                if (event.data.callback != undefined) {
                    if (event.data.callback != null) {
                        event.data.callback(true);
                    }
                }
            }
        );
        $('#ConfirmDialogContent').html(txt);
        $('#ConfirmDialog').addClass('ui-page-active');
    },
    prompt: function(txt, callback, title, curvalue) {
        if ($('#PromptDialog').length == 0) {
            $(document.body).append($('<div id="PromptDialog" data-role="dialog"><div data-role="header" data-theme="a"><h1></h1></div><div data-role="content" data=theme="b" id="PromptDialogContent"><p></p><input type="text" name="promptAnswer"/></div><div data-role="footer" data-theme="a"><a data-role="button" data-theme="b" data-icon="accept">Okay</a><a data-role="button" data-theme="b" data-icon="delete">Cancel</a></div></div>'));
            $('#PromptDialog').page();
            $($('#PromptDialog').find('a:first')[0]).remove();
            FreeswitchConfig.Site.Modals._prePromptPage = $.find('div.ui-page-active');
        } else if ($('#PromptDialog').attr('class').indexOf('ui-page-active') < 0) {
            FreeswitchConfig.Site.Modals._prePromptPage = $.find('div.ui-page-active');
        }
        if (FreeswitchConfig.Site.Modals._prePromptPage.length > 0) {
            $(FreeswitchConfig.Site.Modals._prePromptPage).removeClass('ui-page-active');
        }
        title = (title == undefined ? 'Prompt' : (title == null ? 'Prompt' : title));
        $($('#PromptDialog').find('h1')[0]).html(title);
        $($('#PromptDialogContent').find('p')[0]).html('');
        var buttons = $('#PromptDialog').find('a');
        $(buttons).unbind('click');
        $(buttons[1]).bind('click',
                { callback: callback },
                function(event) {
                    $('#PromptDialog').removeClass('ui-page-active');
                    if (FreeswitchConfig.Site.Modals._prePromptPage.length > 0) {
                        $(FreeswitchConfig.Site.Modals._prePromptPage).addClass('ui-page-active');
                    }
                    FreeswitchConfig.Site.Modals._prePromptPage = [];
                    if (event.data.callback != undefined) {
                        if (event.data.callback != null) {
                            event.data.callback(false);
                        }
                    }
                }
            );
        $(buttons[0]).bind('click',
                { callback: callback },
                function(event) {
                    $('#PromptDialog').removeClass('ui-page-active');
                    if (FreeswitchConfig.Site.Modals._prePromptPage.length > 0) {
                        $(FreeswitchConfig.Site.Modals._prePromptPage).addClass('ui-page-active');
                    }
                    FreeswitchConfig.Site.Modals._prePromptPage = [];
                    if (event.data.callback != undefined) {
                        if (event.data.callback != null) {
                            event.data.callback($($('PrompDialogContent').find('input:last')[0]).val());
                        }
                    }
                }
            );
        $($('#PromptDialogContent').find('p')[0]).html(txt);
        $('#PromptDialog').addClass('ui-page-active');
    },
    ShowFormPanel: function(title, content, buttons) {
        if ($('#FormDialog').length == 0) {
            $(document.body).append($('<div id="FormDialog" data-role="dialog"><div data-role="header" data-theme="a"><h1></h1></div><div data-role="content" data=theme="b" id="FormDialogContent"></div><div data-role="footer" data-theme="a"></div></div>'));
            $('#FormDialog').page();
            $($('#FormDialog').find('a:first')[0]).remove();
            FreeswitchConfig.Site.Modals._preFormPage = $.find('div.ui-page-active');
        } else if ($('#FormDialog').attr('class').indexOf('ui-page-active') < 0) {
            FreeswitchConfig.Site.Modals._preFormPage = $.find('div.ui-page-active');
        }
        if (FreeswitchConfig.Site.Modals._preFormPage.length > 0) {
            if ($(FreeswitchConfig.Site.Modals._preFormPage[0]).attr('id') == 'LoadingPage') {
                FreeswitchConfig.Site.Modals._preFormPage = FreeswitchConfig.Site.Modals._preLoadingPage;
                FreeswitchConfig.Site.Modals._preLoadingPage = null;
            }
            $(FreeswitchConfig.Site.Modals._preFormPage).removeClass('ui-page-active');
        }
        title = (title == undefined ? '' : (title == null ? '' : title));
        $($('#FormDialog').find('h1')[0]).html(title);
        var footer = $($('#FormDialog').find('div:last')[0]);
        footer.html('');
        $('#FormDialogContent').html(content);
        if (buttons != null) {
            for (var x = 0; x < buttons.length; x++) {
                footer.append(buttons[x]);
            }
        }
        $('#FormDialog').trigger('create');
        $('#FormDialog').addClass('ui-page-active');
    },
    HideFormPanel: function() {
        $('#FormDialog').removeClass('ui-page-active');
        $(FreeswitchConfig.Site.Modals._preFormPage).addClass('ui-page-active');
    }
}
});

window.alert = function(msg){ FreeswitchConfig.Site.Modals.alert(msg); }

function alert(msg,callback){ FreeswitchConfig.Site.Modals.alert(msg, callback); }

function confirm(txt, callback, title) { FreeswitchConfig.Site.Modals.confirm(txt, callback, title); }

function prompt(txt, callback, title, curvalue) { FreeswitchConfig.Site.Modals.prompt(txt, callback, title, curvalue); }