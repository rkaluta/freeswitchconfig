CreateNameSpace('FreeswitchConfig.Site.Modals');

FreeswitchConfig.Site.Modals = $.extend(FreeswitchConfig.Site.Modals, {
    ShowOverlay: function() {
        if ($('#OverlayPanel').length == 0) {
            $(document.body).append($('<div id="OverlayPanel"></div>'));
        }
        $('#OverlayPanel').show();
    },
    HideOverlay: function() {
        $('#OverlayPanel').hide();
    },
    ShowLoading: function() {
        if ($('#AnimatedOverlayPanel').length == 0) {
            $(document.body).append($('<div id="AnimatedOverlayPanel"></div>'));
        }
        if ($('#AnimatedCirclePanel').length == 0) {
            $(document.body).append($('<div id="AnimatedCirclePanel"><span>LOADING...</span></div>'));
        }
        $('#AnimatedOverlayPanel').show();
        $('#AnimatedCirclePanel').show();
        $($('#AnimatedCirclePanel').find('span')[0]).html('LOADING...');
    },
    HideLoading: function() {
        $('#AnimatedCirclePanel').hide();
        $('#AnimatedOverlayPanel').hide();
    },
    ShowUpdating: function() {
        if ($('#AnimatedOverlayPanel').length == 0) {
            $(document.body).append($('<div id="AnimatedOverlayPanel"></div>'));
        }
        if ($('#AnimatedCirclePanel').length == 0) {
            $(document.body).append($('<div id="AnimatedCirclePanel"><span>LOADING...</span></div>'));
        }
        $('#AnimatedOverlayPanel').show();
        $('#AnimatedCirclePanel').show();
        $($('#AnimatedCirclePanel').find('span')[0]).html('UPDATING...');
    },
    HideUpdating: function() {
        $('#AnimatedCirclePanel').hide();
        $('#AnimatedOverlayPanel').hide();
    },
    _createPopupContainer: function() {
        if ($('#AnimatedOverlayPanel').length == 0) {
            $(document.body).append($('<div id="AnimatedOverlayPanel"></div>'));
        }
        if ($('#PopupPanel').length == 0) {
            $(document.body).append($('<div id="PopupPanel"><table class="container"><tr><td class="tdContainer"><div id="PopupPanelHeader"></div><div id="PopupPanelContent"></div><div id="PopupPanelButtons"></div></td></tr></table></div>'));
        }
    },
    alert: function(msg, callback) {
        FreeswitchConfig.Site.Modals._createPopupContainer();
        $('#AnimatedOverlayPanel').show();
        $('#PopupPanelContent').html('');
        $('#PopupPanelContent').append(msg);
        $('#PopupPanelButtons').html('');
        $('#PopupPanelButtons').append(
            FreeswitchConfig.Site.Modals.CreateButton(
                '/resources/icons/accept.png',
                'Okay',
                function(button, pars) {
                    $('#PopupPanel').hide();
                    $('#AnimatedOverlayPanel').hide();
                    if (pars.callback != undefined) {
                        if (pars.callback != null) {
                            pars.callback();
                        }
                    }
                },
                { callback: callback }
            )
        );
        $('#PopupPanelHeader').html('ALERT');
        $('#PopupPanel').show();
    },
    confirm: function(txt, callback, title) {
        FreeswitchConfig.Site.Modals._createPopupContainer();
        $('#AnimatedOverlayPanel').show();
        $('#PopupPanelContent').html(txt);
        $('#PopupPanelButtons').html('');
        $('#PopupPanelButtons').append(
            FreeswitchConfig.Site.Modals.CreateButton(
            '/resources/icons/accept.png',
            'Okay',
            function(button, pars) {
                $('#PopupPanel').hide();
                $('#AnimatedOverlayPanel').hide();
                if (pars.callback != undefined) {
                    if (pars.callback != null) {
                        pars.callback(true);
                    }
                }
            },
            { callback: callback })
        );
        $('#PopupPanelButtons').append(
            FreeswitchConfig.Site.Modals.CreateButton(
            '/resources/icons/cancel.png',
            'Cancel',
            function(button, pars) {
                $('#PopupPanel').hide();
                $('#AnimatedOverlayPanel').hide();
                if (pars.callback != undefined) {
                    if (pars.callback != null) {
                        pars.callback(false);
                    }
                }
            },
            { callback: callback })
        );
        $('#PopupPanelHeader').html(((title == undefined) ? 'Confirm' : title));
        $('#PopupPanel').show();
    },
    prompt: function(txt, callback, title, curvalue) {
        FreeswitchConfig.Site.Modals._createPopupContainer();
        $('#AnimatedOverlayPanel').show();
        $('#PopupPanelContent').html(txt);
        $('#PopupPanelContent').append('<br/><input type="text" name="confirmValue"/>');
        var inp = $($('#PopupPanelContent').find('input[name="confirmValue"]')[0]);
        $('#PopupPanelButtons').html('');
        var butOkay = FreeswitchConfig.Site.Modals.CreateButton(
            '/resources/icons/accept.png',
            'Okay',
            function(button, pars) {
                $('#PopupPanel').hide();
                $('#AnimatedOverlayPanel').hide();
                if (pars.callback != undefined) {
                    if (pars.callback != null) {
                        pars.callback(pars.input);
                    }
                }
            },
            { callback: callback, input: inp });
        inp.bind('keypress',
            { button: butOkay },
            function(e) {
                if (e.keyCode == 13) {
                    e.data.button.click();
                }
            });
        $('#PopupPanelButtons').append(butOkay);
        $('#PopupPanelButtons').append(
            FreeswitchConfig.Site.Modals.CreateButton(
            '/resources/icons/cancel.png',
            'Cancel',
            function(button, pars) {
                $('#PopupPanel').hide();
                $('#AnimatedOverlayPanel').hide();
                if (pars.callback != undefined) {
                    if (pars.callback != null) {
                        pars.callback(null);
                    }
                }
            },
            { callback: callback })
        );
        $('#PopupPanelHeader').html(((title == undefined) ? 'Prompt' : title));
        $('#PopupPanel').show();
    },
    ShowFormPanel: function(title, content, buttons) {
        FreeswitchConfig.Site.Modals.ShowOverlay();
        if ($('#FormPanel').length == 0) {
            $(document.body).append($('<div id="FormPanel"><table class="container"><tr><td class="tdContainer"><div id="FormPanelHeader"></div><div id="FormPanelContent"></div><div id="FormPanelButtons"></div></td></tr></table></div>'));
        }
        content = $(content);
        $('#FormPanelContent').html('');
        $('#FormPanelContent').append(content);
        $('#FormPanelButtons').html('');
        if (buttons != undefined) {
            if (buttons != null) {
                for (var x = 0; x < buttons.length; x++) {
                    $('#FormPanelButtons').append(buttons[x]);
                }
            }
        }
        $('#FormPanelHeader').html(title);
        $('#FormPanel').show();
    },
    HideFormPanel: function() {
        $('#FormPanel').hide();
        FreeswitchConfig.Site.Modals.HideOverlay();
    }
});

window.alert = function(msg){ FreeswitchConfig.Site.Modals.alert(msg); }

function alert(msg,callback){ FreeswitchConfig.Site.Modals.alert(msg, callback); }

function confirm(txt, callback, title) { FreeswitchConfig.Site.Modals.confirm(txt, callback, title); }

function prompt(txt, callback, title, curvalue) { FreeswitchConfig.Site.Modals.prompt(txt, callback, title, curvalue); }