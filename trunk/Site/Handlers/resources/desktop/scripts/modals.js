FreeswitchConfig.Site = $.extend(FreeswitchConfig.Site, { Modals: {
    previousPages: new Array(),
    ShowOverlay: function() {
        $('#OverlayPanel').show();    
    },
    HideOverlay: function(){
        $('#OverlayPanel').hide();
    },
    ShowLoading:function(){
        if (IS_MOBILE){
            var curPage = $($.find('.ui-page-active')[0]);
            FreeswitchConfig.Site.Modals.previousPages.push(curPage);
            $($('#pgLoading > p')[0]).html('LOADING...');
            $.mobile.changePage($('#pgLoading'),{reverse: false,changeHash: false});
        }else{
            $('#AnimatedOverlayPanel').show();
            $('#AnimatedCirclePanel').show();
            $($('#AnimatedCirclePanel').find('span')[0]).html('LOADING...');
        }
    },
    HideLoading:function(){
        if (IS_MOBILE){
            if ($('#pgLoading').attr('class').indexOf('ui-page-active')>=0){
                $.mobile.changePage($(FreeswitchConfig.Site.Modals.previousPages.pop()),{reverse: false,changeHash: false});
            }
        }else{
            $('#AnimatedCirclePanel').hide();
            $('#AnimatedOverlayPanel').hide();
        }
    },
    ShowSaving:function(){
        if (IS_MOBILE){
            var curPage = $($.find('.ui-page-active')[0]);
            FreeswitchConfig.Site.Modals.previousPages.push(curPage);
            $($('#pgLoading > p')[0]).html('SAVING...');
            $.mobile.changePage($('#pgLoading'),{reverse: false,changeHash: false});
        }else{
            $('#AnimatedOverlayPanel').show();
            $('#AnimatedCirclePanel').show();
            $($('#AnimatedCirclePanel').find('span')[0]).html('SAVING...');
        }
    },
    HideSaving:function(){
        if (IS_MOBILE){
            if ($('#pgLoading').attr('class').indexOf('ui-page-active')>=0){
                $.mobile.changePage($(FreeswitchConfig.Site.Modals.previousPages.pop()),{reverse: false,changeHash: false});
            }
        }else{
            $('#AnimatedCirclePanel').hide();
            $('#AnimatedOverlayPanel').hide();
        }
    },
    ShowUpdating:function(){
        if (IS_MOBILE){
            var curPage = $($.find('.ui-page-active')[0]);
            FreeswitchConfig.Site.Modals.previousPages.push(curPage);
            $($('#pgLoading > p')[0]).html('UPDATING...');
            $.mobile.changePage($('#pgLoading'),{reverse: false,changeHash: false});
        }else{
            $('#AnimatedOverlayPanel').show();
            $('#AnimatedCirclePanel').show();
            $($('#AnimatedCirclePanel').find('span')[0]).html('UPDATING...');
        }
    },
    HideUpdating:function(){
        if (IS_MOBILE){
            if ($('#pgLoading').attr('class').indexOf('ui-page-active')>=0){
                $.mobile.changePage($(FreeswitchConfig.Site.Modals.previousPages.pop()),{reverse: false,changeHash: false});
            }
        }else{
            $('#AnimatedCirclePanel').hide();
            $('#AnimatedOverlayPanel').hide();
        }
    },
    ShowUploading:function(){
        if (IS_MOBILE){
            var curPage = $($.find('.ui-page-active')[0]);
            FreeswitchConfig.Site.Modals.previousPages.push(curPage);
            $($('#pgLoading > p')[0]).html('UPLOADING...');
            $.mobile.changePage($('#pgLoading'),{reverse: false,changeHash: false});
        }else{
            $('#AnimatedOverlayPanel').show();
            $('#AnimatedCirclePanel').show();
            $($('#AnimatedCirclePanel').find('span')[0]).html('UPLOADING...');
        }
    },
    HideUploading:function(){
        if (IS_MOBILE){
            if ($('#pgLoading').attr('class').indexOf('ui-page-active')>=0){
                $.mobile.changePage($(FreeswitchConfig.Site.Modals.previousPages.pop()),{reverse: false,changeHash: false});
            }
        }else{
            $('#AnimatedCirclePanel').hide();
            $('#AnimatedOverlayPanel').hide();
        }
    },
    ShowDeploying:function(){
        if (IS_MOBILE){
            var curPage = $($.find('.ui-page-active')[0]);
            FreeswitchConfig.Site.Modals.previousPages.push(curPage);
            $($('#pgLoading > p')[0]).html('DEPLOYING...');
            $.mobile.changePage($('#pgLoading'),{reverse: false,changeHash: false});
        }else{
            $('#AnimatedOverlayPanel').show();
            $('#AnimatedCirclePanel').show();
            $($('#AnimatedCirclePanel').find('span')[0]).html('DEPLOYING...');
        }
    },
    HideDeploying:function(){
        if (IS_MOBILE){
            if ($('#pgLoading').attr('class').indexOf('ui-page-active')>=0){
                $.mobile.changePage($(FreeswitchConfig.Site.Modals.previousPages.pop()),{reverse: false,changeHash: false});
            }
        }else{
            $('#AnimatedCirclePanel').hide();
            $('#AnimatedOverlayPanel').hide();
        }
    },
    customAlert:function(msg,callback){
        if (IS_MOBILE){
            var pgAlert = $('#pgAlert');
            if ($($.find('.ui-page-active')[0]).attr('id')=='pgMainContainer'){
                FreeswitchConfig.Site.Modals.previousPages.push($($.find('.ui-page-active')[0]));
            }else{
                FreeswitchConfig.Site.Modals.previousPages.push(pgAlert);
            }
            $(pgAlert.find('div.content')[0]).html(msg);
            $(pgAlert.find('footer')[0]).html('');
            $(pgAlert.find('footer')[0]).append(
                CreateButton(
                'accept',
                'Okay',
                function(button,pars){
                    var pg = previousPages.pop();
                    if (pg.attr('id')=='pgAlert'){
                        pg = FreeswitchConfig.Site.Modals.previousPages.pop();
                    }
                    $.mobile.changePage(pg,{reverse: false,changeHash: false});
                    if (pars.callback!=undefined){
                        if (pars.callback!=null){
                            pars.callback();
                        }
                    }
                },
                {callback:callback})
            );
            $.mobile.changePage(pgAlert);
        }else{
            $('#PopupPanelOverlayPanel').show();
            $('#PopupPanelContent').scrollTop(0);
            $('#PopupPanelContent').html(msg);
            $('#PopupPanelButtons').html('');
            $('#PopupPanelButtons').append(
                CreateButton(
                'accept',
                'Okay',
                function(button,pars){
                    $('#PopupPanel').hide();    
                    $('#PopupPanelOverlayPanel').hide();
                    if (pars.callback!=undefined){
                        if (pars.callback!=null){
                            pars.callback();
                        }
                    }
                },
                {callback:callback})
            );
            $('#PopupPanelHeader').html('ALERT');
            $('#PopupPanel').show();
        }
    },
    confirm:function(txt,callback,title) {
        if (IS_MOBILE){
            var pgConfirm = $('#pgConfirm');
            if ($($.find('.ui-page-active')[0]).attr('id')=='pgMainContainer'){
                FreeswitchConfig.Site.Modals.previousPages.push($($.find('.ui-page-active')[0]));
            }else{
                FreeswitchConfig.Site.Modals.previousPages.push(pgConfirm);
            }
            $(pgConfirm.find('header')[0]).html((title == undefined ? 'Confirm' : title));
            $(pgConfirm.find('div.content')[0]).html('');
            $(pgConfirm.find('div.content')[0]).html(txt);
            $(pgConfirm.find('footer')[0]).html('');
            $(pgConfirm.find('footer')[0]).append(
                CreateButton(
                'accept',
                'Okay',
                function(button,pars){
                    var pg = previousPages.pop();
                    if (pg.attr('id')=='pgConfirm'){
                        pg = FreeswitchConfig.Site.Modals.previousPages.pop();
                    }
                    $.mobile.changePage(pg,{reverse: false,changeHash: false});
                    if (pars.callback!=undefined){
                        if (pars.callback!=null){
                            pars.callback(true);
                        }
                    }
                },
                {callback:callback})
            );
            $(pgConfirm.find('footer')[0]).append(
                CreateButton(
                'cancel',
                'Cancel',
                function(button,pars){
                    var pg = FreeswitchConfig.Site.Modals.previousPages.pop();
                    if (pg.attr('id')=='pgConfirm'){
                        pg = FreeswitchConfig.Site.Modals.previousPages.pop();
                    }
                    $.mobile.changePage(pg,{reverse: false,changeHash: false});
                    if (pars.callback!=undefined){
                        if (pars.callback!=null){
                            pars.callback(false);
                        }
                    }
                },
                {callback:callback})
            );
            $.mobile.changePage(pgConfirm);
        }else{
            $('#PopupPanelOverlayPanel').show();
            $('#PopupPanelContent').scrollTop(0);
            $('#PopupPanelContent').html(txt);
            $('#PopupPanelButtons').html('');
            $('#PopupPanelButtons').append(
                CreateButton(
                'accept',
                'Okay',
                function(button,pars){
                    $('#PopupPanel').hide();    
                    $('#PopupPanelOverlayPanel').hide();
                    if (pars.callback!=undefined){
                        if (pars.callback!=null){
                            pars.callback(true);
                        }
                    }
                },
                {callback:callback})
            );
            $('#PopupPanelButtons').append(
                CreateButton(
                'cancel',
                'Cancel',
                function(button,pars){
                    $('#PopupPanel').hide();    
                    $('#PopupPanelOverlayPanel').hide();
                    if (pars.callback!=undefined){
                        if (pars.callback!=null){
                            pars.callback(false);
                        }
                    }
                },
                {callback:callback})
            );
            $('#PopupPanelHeader').html(((title == undefined) ? 'Confirm' : title));
            $('#PopupPanel').show();
        }
    },
    prompt:function(txt, callback, title, curvalue) {
        if (IS_MOBILE){
            var pgPrompt = $('#pgPrompt');
            if ($($.find('.ui-page-active')[0]).attr('id')=='pgMainContainer'){
                FreeswitchConfig.Site.Modals.previousPages.push($($.find('.ui-page-active')[0]));
            }else{
                FreeswitchConfig.Site.Modals.previousPages.push(pgPrompt);
            }
            $(pgPrompt.find('header')[0]).html((title==undefined ? 'Prompt' : title));
            $(pgPrompt.find('div.content')[0]).html(txt);
            $(pgPrompt.find('div.content')[0]).append('<br/><input type="text" name="confirmValue"/>');
            var inp = $(pgPrompt.find('input[name="confirmValue"]')[0]);
            $(pgPrompt.find('footer')[0]).html('');
            var butOkay = CreateButton(
                'accept',
                'Okay',
                function(button, pars) {
                    var pg = FreeswitchConfig.Site.Modals.previousPages.pop();
                    if (pg.attr('id')=='pgPrompt'){
                        pg = FreeswitchConfig.Site.Modals.previousPages.pop();
                    }
                    $.mobile.changePage(pg,{reverse: false,changeHash: false});
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
            $(pgPrompt.find('footer')[0]).append(butOkay);
            $(pgPrompt.find('footer')[0]).append(
                CreateButton(
                'cancel',
                'Cancel',
                function(button, pars) {
                    var pg = FreeswitchConfig.Site.Modals.previousPages.pop();
                    if (pg.attr('id')=='pgPrompt'){
                        pg = FreeswitchConfig.Site.Modals.previousPages.pop();
                    }
                    $.mobile.changePage(pg,{reverse: false,changeHash: false});
                    if (pars.callback != undefined) {
                        if (pars.callback != null) {
                            pars.callback(null);
                        }
                    }
                },
                { callback: callback })
            );
            $.mobile.changePage(pgPrompt);
        }else{
            $('#PopupPanelOverlayPanel').show();
            $('#PopupPanelContent').scrollTop(0);
            $('#PopupPanelContent').html(txt);
            $('#PopupPanelContent').append('<br/><input type="text" name="confirmValue"/>');
            var inp = $($('#PopupPanelContent').find('input[name="confirmValue"]')[0]);
            $('#PopupPanelButtons').html('');
            var butOkay = CreateButton(
                'accept',
                'Okay',
                function(button, pars) {
                    $('#PopupPanel').hide();
                    $('#PopupPanelOverlayPanel').hide();
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
                CreateButton(
                'cancel',
                'Cancel',
                function(button, pars) {
                    $('#PopupPanel').hide();
                    $('#PopupPanelOverlayPanel').hide();
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
        }
    },
    promptForm:function(parts,callback,title,pars){
        if (IS_MOBILE){
            var pgForm = $('#pgForm');
            if ($($.find('.ui-page-active')[0]).attr('id')=='pgMainContainer'){
                FreeswitchConfig.Site.Modals.previousPages.push($($.find('.ui-page-active')[0]));
            }else{
                FreeswitchConfig.Site.Modals.previousPages.push(pgForm);
            }
            $(pgForm.find('header')[0]).html((title==undefined ? 'Prompt' : title));
            callback=(callback==undefined ? null : callback);
            pars = (pars==undefined ? null : pars);
            var frm = FreeswitchConfig.Site.Form.GenerateForm(
                null,
                parts
            );
            $(pgForm.find('div.content')[0]).html('');
            $(pgForm.find('div.content')[0]).append(frm);
            $(pgForm.find('footer')[0]).html('');
            $(pgForm.find('footer')[0]).append(
                CreateButton(
                'accept',
                'Okay',
                function(button, pars) {
                    var pg = FreeswitchConfig.Site.Modals.previousPages.pop();
                    if (pg.attr('id')=='pgForm'){
                        pg = FreeswitchConfig.Site.Modals.previousPages.pop();
                    }
                    $.mobile.changePage(pg,{reverse: false,changeHash: false});
                    if (pars.callback != undefined) {
                        if (pars.callback != null) {
                            pars.callback(pars.input);
                        }
                    }
                },
                { callback: callback, input: inp })
            );
            $(pgForm.find('footer')[0]).append(
                CreateButton(
                'cancel',
                'Cancel',
                function(button, pars) {
                    var pg = FreeswitchConfig.Site.Modals.previousPages.pop();
                    if (pg.attr('id')=='pgForm'){
                        pg = FreeswitchConfig.Site.Modals.previousPages.pop();
                    }
                    $.mobile.changePage(pg,{reverse: false,changeHash: false});
                    if (pars.callback != undefined) {
                        if (pars.callback != null) {
                            pars.callback(null);
                        }
                    }
                },
                { callback: callback })
            );
            $.mobile.changePage(pgForm);
        }else{
            $('#PopupPanelOverlayPanel').show();
            $('#PopupPanelContent').html('');
            $('#PopupPanelContent').scrollTop(0);
            callback=(callback==undefined ? null : callback);
            pars = (pars==undefined ? null : pars);
            var frm = FreeswitchConfig.Site.Form.GenerateForm(
                null,
                parts
            );
            $('#PopupPanelContent').append(frm);
            $('#PopupPanelButtons').html('');
            var butOkay = CreateButton(
                'accept',
                'Okay',
                function(button, pars) {
                    $('#PopupPanel').hide();
                    $('#PopupPanelOverlayPanel').hide();
                    if (pars.callback != undefined) {
                        if (pars.callback != null) {
                            if (pars.additional!=null){
                                pars.callback(pars.frm,pars.additional);
                            }else{
                                pars.callback(pars.frm);
                            }
                        }
                    }
                },
                { callback: callback, frm:frm,additional:pars });
            $('#PopupPanelButtons').append(butOkay);
            $('#PopupPanelButtons').append(
                CreateButton(
                'cancel',
                'Cancel',
                function(button, pars) {
                    $('#PopupPanel').hide();
                    $('#PopupPanelOverlayPanel').hide();
                    if (pars.callback != undefined) {
                        if (pars.callback != null) {
                            if (pars.additional!=null){
                                pars.callback(null,pars.additional);
                            }else{
                                pars.callback(null);
                            }
                        }
                    }
                },
                { callback: callback,additional:pars })
            );
            $('#PopupPanelHeader').html(((title == undefined) ? 'Prompt' : title));
            $('#PopupPanel').show();
        }
    },
    ShowFormPanel:function(title,content,buttons){
        if (IS_MOBILE){
            var pgForm = $('#pgForm');
            if ($($.find('.ui-page-active')[0]).attr('id')=='pgMainContainer'){
                FreeswitchConfig.Site.Modals.previousPages.push($($.find('.ui-page-active')[0]));
            }else{
                FreeswitchConfig.Site.Modals.previousPages.push(pgForm);
            }
            $(pgForm.find('header')[0]).html(title);
            if (!(content instanceof Array)){
                content = $(content);
                content = [content];
            }
            $(pgForm.find('div.content')[0]).html('');
            for(var x=0;x<content.length;x++){
                $(pgForm.find('div.content')[0]).append(content[x]);
            }
            $(pgForm.find('footer')[0]).html('');
            if (buttons!=undefined){
                if (buttons!=null){
                    for(var x=0;x<buttons.length;x++){
                        $(pgForm.find('footer')[0]).append(buttons[x]);
                    }
                }
            }
            $.mobile.changePage(pgForm,{reverse: false,changeHash: false});
        }else{
            FreeswitchConfig.Site.Modals.ShowOverlay();
            if (!(content instanceof Array)){
                content = $(content);
                content = [content];
            }
            $('#FormPanelContent').scrollTop(0);
            $('#FormPanelContent').html('');
            for(var x=0;x<content.length;x++){
                $('#FormPanelContent').append(content[x]);
            }
            $('#FormPanelButtons').html('');
            if (buttons!=undefined){
                if (buttons!=null){
                    for(var x=0;x<buttons.length;x++){
                        $('#FormPanelButtons').append(buttons[x]);
                    }
                }
            }
            $('#FormPanelHeader').html(title);
            $('#FormPanel').show();
        }
    },
    HideFormPanel:function(){
        if (IS_MOBILE){
            var pg = FreeswitchConfig.Site.Modals.previousPages.pop();
            if (pg.attr('id')=='pgForm'){
                pg = FreeswitchConfig.Site.Modals.previousPages.pop();
            }
            $.mobile.changePage(pg,{reverse: false,changeHash: false});
        }else{
            $('#FormPanel').hide();
            FreeswitchConfig.Site.Modals.HideOverlay();
        }
    },
    RegisterAudioFileBrowser:function(input,button){
        if ((button==undefined)||(button==null))
        {
            button=input;
        }
        $(button).bind('click',
        { input: $(input) },
        function(event) {
            ShowLoading();
            $('#FileBrowserOverlay').show();
            var cont = $('#FileBrowserContent');
            cont.html('');
            $('#FileBrowserButtons').html('');
            $('#FileBorderButtons').append(
                CreateButton(
                    'cancel',
                    'Cancel',
                    function(button) {
                        $('#FileBrowser').hide();
                        $('#FileBrowserOverlay').hide();
                    }
                )
            );
            var col;
            if (event.data.input.val() == '') {
                col = new FreeswitchConfig.Core.AudioFileEntry.Collection();
                col.fetch();
            } else {
                var pth = event.data.input.val();
                col = FreeswitchConfig.Core.AudioFileEntry.LoadInPath(pth);
                var tmpEntry = new FreeswitchConfig.Core.AudioFileEntry.Model({
                    Name: '..',
                    RelativeFilePath: pth.substring(0, pth.lastIndexOf(DIRECTORY_SEPERATOR)),
                    IsFile: false
                });
                col.add(tmpEntry);
            }
            var vw = new FreeswitchConfig.Core.AudioFileEntry.CollectionView({
                collection: col,
                input: event.data.input
            });
            $('#FileBrowserContent').append(vw.$el);
            vw.render();
            $('#FileBrowser').show();
            HideLoading();
        });
    }
}
});

window.alert = function(msg){
    FreeswitchConfig.Site.Modals.customAlert(msg);
}

function alert(msg,callback){
    FreeswitchConfig.Site.Modals.customAlert(msg, callback);
}
