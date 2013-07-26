CreateNameSpace('Org.Reddragonit.FreeSwitchConfig.Site.Models.SystemConfig.FileEntry');
CreateNameSpace('Org.Reddragonit.FreeSwitchConfig.Site.Core.SystemConfig.FileAccess');

Org.Reddragonit.FreeSwitchConfig.Site.Models.SystemConfig.FileEntry = $.extend(Org.Reddragonit.FreeSwitchConfig.Site.Models.SystemConfig.FileEntry,{
    CollectionView : Backbone.View.extend({
        className: 'Org Reddragonit FreeSwitchConfig Site Models SystemConfig FileEntry CollectionView FileBrowser '+FreeswitchConfig.Site.Skin.div.Class,
        tagName: FreeswitchConfig.Site.Skin.div.Tag,
        initialize: function() {
            this.collection.on('reset', this.render, this); this.collection.on('sync',this.render,this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        attributes:FreeswitchConfig.Site.Skin.div.Attributes,
        render: function() {
            var el = this.$el;
            el.html('');
            for (var x = 0; x < this.collection.length; x++) {
                var model = this.collection.get(x);
                var spn = FreeswitchConfig.Site.Skin.span.Create({Class:(model.get('IsFile') ? 'File' : 'Folder'),Attributes:{relPath:model.get('RelativePath')}});
                el.append(spn);
                if (model.get('IsFile')) {
                    if (this.IsPlayableFile(model)) {
                        var player = Org.Reddragonit.FreeSwitchConfig.Site.WavPlayer.CreateNewPlayer('/custom_handlers/RelativeFiles/' + model.get('RelativePath').replaceAll('\\', '/').replaceAll('%', '%25'));
                        spn.append(player);
                        spn.contextMenu({
                            menu: 'fileMenuAudio'
                        },
                    function(action, el, pos) {
                        switch (action) {
                            case 'download':
                                window.open('/custom_handlers/RelativeFiles/' + escape($(el).attr('relpath')));
                                break;
                            case 'delete':
                                FreeswitchConfig.Site.Modals.confirm('You have selected to delete the file ' + $(el).text() + '.  Click Okay to Continue or Cancel to abort.',
                                function(res) {
                                    if (res) {
                                        var file = Org.Reddragonit.FreeSwitchConfig.Site.Models.SystemConfig.FileEntry.Model({id:$(el).attr('relpath')});
                                        if (file.delete()){
                                            $(el).remove();
                                        }else{
                                            alert('An error occured deleting the selected file.');
                                        }
                                    }
                                });
                            case 'update':
                                var spn = $('<span></span>');
                                $(document.body).append(spn);
                                spn.AjaxFileUpload({
                                    action: '/FileUpload.ashx',
                                    allowedExtensions: ['.wav', '.mp3'],
                                    onError: function(errorType) {
                                        switch (errorType) {
                                            case 'incorrect_file_type':
                                                alert('The file type you specified to upload for replacing ' + $(el).text() + ' is incorrect.');
                                                break;
                                        }
                                    },
                                    onSubmit: function(button, fileName) {
                                        FreeswitchConfig.Site.Modals.ShowUploading();
                                    },
                                    onComplete: function(button, fileName, response) {
                                        var file = Org.Reddragonit.FreeSwitchConfig.Site.Models.SystemConfig.FileEntry.Model({id:$(el).attr('relpath')});
                                        file.set({RelativeFilePath:response});
                                        if (file.update()){
                                            FreeswitchConfig.Site.Modals.HideUploading();
                                        }else{
                                            FreeswitchConfig.Site.Modals.HideUploading();
                                            alert('An error occured updating the selected file.');
                                        }
                                        spn.remove();
                                    }
                                });
                                spn.click();
                                break;
                        }
                    });
                    } else {
                        spn.contextMenu({
                            menu: 'fileMenuText'
                        },
                    function(action, el, pos) {
                        switch (action) {
                            case 'download':
                                window.open('/custom_handlers/RelativeFiles/' + escape($(el).attr('relpath')));
                                break;
                            case 'delete':
                                FreeswitchConfig.Site.Modals.confirm('You have selected to delete the file ' + $(el).text() + '.  Click Okay to Continue or Cancel to abort.',
                                function(res) {
                                    if (res) {
                                        var file = Org.Reddragonit.FreeSwitchConfig.Site.Models.SystemConfig.FileEntry.Model({id:$(el).attr('relpath')});
                                        if (file.delete()){
                                            $(el).remove();
                                        }else{
                                            alert('An error occured deleting the selected file.');
                                        }
                                    }
                                });
                                break;
                            case 'update':
                                var spn = $('<span></span>');
                                $(document.body).append(spn);
                                spn.AjaxFileUpload({
                                    action: '/FileUpload.ashx',
                                    allowedExtensions: ['.txt', '.lua'],
                                    onError: function(errorType) {
                                        switch (errorType) {
                                            case 'incorrect_file_type':
                                                alert('The file type you specified to upload for replacing ' + $(el).text() + ' is incorrect.');
                                                break;
                                        }
                                    },
                                    onSubmit: function(button, fileName) {
                                        FreeswitchConfig.Site.Modals.ShowUploading();
                                    },
                                    onComplete: function(button, fileName, response) {
                                        var file = Org.Reddragonit.FreeSwitchConfig.Site.Models.SystemConfig.FileEntry.Model({id:$(el).attr('relpath')});
                                        file.set({RelativeFilePath:response});
                                        if (file.update()){
                                            FreeswitchConfig.Site.Modals.HideUploading();
                                        }else{
                                            FreeswitchConfig.Site.Modals.HideUploading();
                                            alert('An error occured updating the selected file.');
                                        }
                                        spn.remove();
                                    }
                                });
                                spn.click();
                                break;
                            case 'edit':
                                FreeswitchConfig.Site.Modals.ShowLoading();
                                var file = Org.Reddragonit.FreeSwitchConfig.Site.Models.SystemConfig.FileEntry.Model({id:$(el).attr('relpath')});
                                var pnlEditor = $('#pnlFileEdit');
                                $(pnlEditor.find('span:first')[0]).html($(el).text());
                                $(pnlEditor.find('input[name="hidFilePath"]')[0]).val($(el).attr('relpath'));
                                $(pnlEditor.find('textarea[name="fileContent"]')[0]).val(file.Content);
                                FreeswitchConfig.Site.Modals.HideLoading();
                                FreeswitchConfig.Site.Modals.ShowOverlay();
                                pnlEditor.show();
                                Org.Reddragonit.FreeSwitchConfig.Site.Services.SystemConfig.FileAccessService.GetTextFileContent(
                                    $(el).attr('relpath'),
                                    function(msg) {
                                        if (msg != null) {
                                            var pnlEditor = $('#pnlFileEdit');
                                            $(pnlEditor.find('span:first')[0]).html($(el).text());
                                            $(pnlEditor.find('input[name="hidFilePath"]')[0]).val($(el).attr('relpath'));
                                            $(pnlEditor.find('textarea[name="fileContent"]')[0]).val(msg);
                                            FreeswitchConfig.Site.Modals.HideLoading();
                                            FreeswitchConfig.Site.Modals.ShowOverlay();
                                            pnlEditor.show();
                                        } else {
                                            FreeswitchConfig.Site.Modals.HideLoading();
                                            alert('An error occured attempting to load the file ' + $(el).text() + ' for editing.');
                                        }
                                    },
                                    function(msg) {
                                        FreeswitchConfig.Site.Modals.HideLoading();
                                        alert('An error occured communicating with the file access service.');
                                    });
                                break;
                        }
                    });
                    }
                } else {
                    if (!(entry.Name.endsWith('%') && entry.Name[0] == '%')) {
                        spn.contextMenu({
                            menu: 'folderClickMenu'
                        },
                    function(action, el, pos) {
                        switch (action) {
                            case 'exploreFolder':
                                $(el).click();
                                break;
                            case 'deleteFolder':
                                FreeswitchConfig.Site.Modals.confirm('You have selected to delete the folder ' + $(el).text() + '.  Click Okay to Continue or Cancel to abort.',
                                function(res) {
                                    if (res) {
                                        var file = Org.Reddragonit.FreeSwitchConfig.Site.Models.SystemConfig.FileEntry.Model({id:$(el).attr('relpath')});
                                        if (file.delete()){
                                            $(el).remove();
                                        }else{
                                            alert('An error occured deleting the selected folder.');
                                        }
                                    }
                                });
                                break;
                        }
                    });
                    }
                    spn.bind('click',
                    { relPath: entry.RelativePath, name: entry.Name,view:this },
                    function(event) {
                        FreeswitchConfig.Site.Modals.ShowLoading();
                        var dvTrail = $($.find('div[name="Trail"]')[0]);
                        var spn = FreeswitchConfig.Site.Skin.span.Create({Attributes:{pth:event.data.relPath},Content:event.data.name});
                        dvTrail.append(spn);
                        spn.bind('click',
                        { spn: spn,view:event.data.view },
                        function(event) {
                            FreeswitchConfig.Site.Modals.ShowLoading();
                            event.data.view.changePath(event.data.spn.attr('pth'));
                            FreeswitchConfig.Site.Modals.HideLoading();
                        });
                        event.data.view.changePath(event.data.relPath);
                        FreeswitchConfig.Site.Modals.HideLoading();
                    });
                }
            }
        },
        changePath: function(path) {
            if (path == null) {
                this.collection = new Org.Reddragonit.FreeSwitchConfig.Site.Models.SystemConfig.FileEntry.Collection();
            } else {
                this.collection = Org.Reddragonit.FreeSwitchConfig.Site.Models.SystemConfig.FileEntry.LoadInPath(path);
            }
            this.initialize();
            this.render();
        }, IsPlayableFile: function(model) {
            return new RegExp('^.+\\.(wav|mp3)$').test(model.Name);
        }
    })
});

Org.Reddragonit.FreeSwitchConfig.Site.Core.SystemConfig.FileAccess = $.extend(Org.Reddragonit.FreeSwitchConfig.Site.Core.SystemConfig.FileAccess,{
    GeneratePage: function(container) {
        container = $(container);
        FreeswitchConfig.Site.Modals.ShowLoading();
        container.append('<div style="display:block;width:100%;border-top:none;border-left:none;border-right:none;" class="Bordered FileTrail" name="Trail"><span>/</span></div>');
        //append physical folder context menu
        container.append('<ul class="contextMenu" style="display:none;" id="folderClickMenu">' +
        '<li class="ExploreFolder"><a href="#exploreFolder">Open</a></li>' +
        '<li class="DeleteFolder"><a href="#deleteFolder">Delete</a></li>' +
        '</ul>');
        //append text file menu
        container.append('<ul class="contextMenu" style="display:none;" id="fileMenuText">' +
        '<li class="UpdateFile"><a href="#update">Update</a></li>' +
        '<li class="DeleteFile"><a href="#delete">Delete</a></li>' +
        '<li class="EditFile"><a href="#edit">Edit</a></li>' + //only available for lua and txt
        '<li class="DownloadFile"><a href="#download">Download</a></li>' +
        '</ul>');
        //append audio file menu 
        container.append('<ul class="contextMenu" style="display:none;" id="fileMenuAudio">' +
        '<li class="UpdateFile"><a href="#update">Update</a></li>' +
        '<li class="DeleteFile"><a href="#delete">Delete</a></li>' +
        '<li class="DownloadFile"><a href="#download">Download</a></li>' +
        '</ul>');
        var pnlEditor = $('#pnlFileEdit');
        if (pnlEditor.length == 0) {
            pnlEditor = $('<div class="FileEditorContainer PopupPanel" id="pnlFileEdit"></div>');
            $(document.body).append(pnlEditor);
            pnlEditor.append('<div class="RoundBox"><div class="tl" ><div class="tr"><div class="FileEditorContentContainer"></div></div></div><div class="bl"><div class="br"></div></div></div>');
            pnlEditor = $(pnlEditor.find('div.FileEditorContentContainer')[0]);
            pnlEditor.append('<span style="height:25px;margin:0;"></span>' +
                '<div>' +
                '<textarea name="fileContent"></textarea>' +
                '</div>' +
                '<input type="hidden" name="hidFilePath"/>' +
                '<center><img class="button accept" title="Update File"/>' +
            '<img class="button cancel" title="Cancel"/></center>');

            var butUpdateFile = $(pnlEditor.find('img:first')[0]);
            butUpdateFile.bind('click',
            { button: butUpdateFile },
            function(evnt) {
                var pnlEditor = $('#pnlFileEdit');
                pnlEditor.hide();
                FreeswitchConfig.Site.Modals.ShowUpdating();
                var file = new Org.Reddragonit.FreeSwitchConfig.Site.Models.SystemConfig.FileEntry.Model({id:$(pnlEditor.find('input[name="hidFilePath"]')[0]).val()});
                file.set({Content:$(pnlEditor.find('textarea[name="fileContent"]')[0]).val()});
                if (file.save()){
                    FreeswitchConfig.Site.Modals.HideUpdating();
                    alert('File successfully updated.', function() { $(evnt.data.button.next()).click(); });
                }else{
                    FreeswitchConfig.Site.Modals.HideUpdating();
                    alert('An error occured updated the selected file.', function() { FreeswitchConfig.Site.Modals.ShowOverlay(); pnlEditor.show(); });
                }
            });
            var butCancelUpdateFile = $(butUpdateFile.next());
            butCancelUpdateFile.bind('click',
            function(evnt) {
                var pnlEditor = $('#pnlFileEdit');
                $(pnlEditor.find('input[name="hidFilePath"]')[0]).val('');
                pnlEditor.hide();
                FreeswitchConfig.Site.Modals.HideOverlay();
            });
        }

        var spn = $(container.find('span:last')[0]);
        var brw = $('<div class="FileBrowser"></div>');
        container.append(brw);
        spn.bind('click',
        { spn: spn },
            function(event) {
                FreeswitchConfig.Site.Modals.ShowLoading();
                Org.Reddragonit.FreeSwitchConfig.Site.Services.SystemConfig.FileAccessService.GetFolderContent(null,
                function(msg) {
                    if (msg.length > 0) {
                        var brw = $($.find('div.FileBrowser')[0]);
                        brw.html('');
                        brw.disableContextMenu();
                        for (var y = 0; y < msg.length; y++) {
                            Org.Reddragonit.FreeSwitchConfig.Site.Core.SystemConfig.FileAccess.RenderEntry(brw, msg[y]);
                        }
                        while (event.data.spn.next().length > 0) {
                            $(event.data.spn.next()).remove();
                        }
                    } else {
                        container.append('<h3>Sorry No files or Folders located.</h3>');
                    }
                    FreeswitchConfig.Site.Modals.HideLoading();
                },
                function(msg) {
                    FreeswitchConfig.Site.Modals.HideLoading();
                    alert('An error occured communicating with the file management service.');
                });
            });
        spn.click();
    } 
});