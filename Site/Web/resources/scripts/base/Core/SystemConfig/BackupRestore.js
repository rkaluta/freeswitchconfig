CreateNameSpace('FreeswitchConfig.Services.BackupRestoreService');

FreeswitchConfig.Services.BackupRestoreService = $.extend(FreeswitchConfig.Services.BackupRestoreService, {
    GeneratePage: function(container) {
        container = $(container);
        //Database Section
        container.append([
            FreeswitchConfig.Site.Skin.h3.Create('Database'),
            FreeswitchConfig.Site.Skin.span.Create({ Attributes: { 'style': 'cursor:pointer;padding-right:5px;border-right:1px solid black;margin-right:5px;', 'onClick': 'window.open(\'/FileDownloader.ashx?FileType=Backup&Level=Database\',\'\',\'scrollbars=no,menubar=no,height=600,width=800,resizable=yes,toolbar=no,location=no,status=no\');' }, Content: [
                FreeswitchConfig.Site.Skin.img.Create({ Class: 'database_save', Attributes: { 'title': 'Add New', 'style': 'padding-right:5px;'} }),
                'Backup Configuration Database'
            ]
            }),
            FreeswitchConfig.Site.Skin.span.Create({ Attributes: { 'style': 'cursor:pointer;padding-right:5px;' }, Content: [
                FreeswitchConfig.Site.Skin.img.Create({ Class: 'database_go', Attributes: { 'style': 'padding-right:5px;'} }),
                'Restore Configuration Database'
            ]
            }),
            '<br/>'
        ]);
        var spn = $(container.find(FreeswitchConfig.Site.Skin.span.Tag + ':last')[0]);
        spn.AjaxFileUpload({
            action: '/FileUpload.ashx',
            allowedExtensions: ['.fscbak]'],
            onError: function(errorType) {
                switch (errorType) {
                    case 'incorrect_file_type':
                        alert('The file type you specified to upload for restoring the database is invalid.');
                        break;
                }
            },
            onSubmit: function(button, fileName) {
                FreeswitchConfig.Site.Modals.ShowUploading();
            },
            onComplete: function(button, fileName, response) {
                FreeswitchConfig.Site.Modals.HideUploading();
                FreeswitchConfig.Site.Modals.ShowLoading();
                FreeswitchConfig.Services.BackupRestoreService.RestoreData(
                    response,
                    'Database',
                    function(msg) {
                        FreeswitchConfig.Site.Modals.HideLoading();
                        if (msg) {
                            alert('Restoring the database was completed successfully');
                        } else {
                            alert('An error occured restoring the database.');
                        }
                    });
            }
        });

        //Audio files Section
        container.append([
            FreeswitchConfig.Site.Skin.h3.Create('Audio'),
            FreeswitchConfig.Site.Skin.span.Create({ Attributes: { 'style': 'cursor:pointer;padding-right:5px;border-right:1px solid black;margin-right:5px;', 'onClick': 'window.open(\'/FileDownloader.ashx?FileType=Backup&Level=Voicemail\',\'\',\'scrollbars=no,menubar=no,height=600,width=800,resizable=yes,toolbar=no,location=no,status=no\');' }, Content: [
                FreeswitchConfig.Site.Skin.img.Create({ Class: 'cd_burn', Attributes: { 'title': 'Add New', 'style': 'padding-right:5px;'} }),
                'Backup Voicemail'
            ]
            }),
            FreeswitchConfig.Site.Skin.span.Create({ Attributes: { 'style': 'cursor:pointer;padding-right:5px;' }, Content: [
                FreeswitchConfig.Site.Skin.img.Create({ Class: 'cd_go', Attributes: { 'title': 'Add New', 'style': 'padding-right:5px;'} }),
                'Restore Voicemail'
            ]
            }),
            '<br/>'
        ]);
        spn = $(container.find(FreeswitchConfig.Site.Skin.span.Tag + ':last')[0]);
        spn.AjaxFileUpload({
            action: '/FileUpload.ashx',
            allowedExtensions: ['.fscbak]'],
            onError: function(errorType) {
                switch (errorType) {
                    case 'incorrect_file_type':
                        alert('The file type you specified to upload for restoring the voicemails is invalid.');
                        break;
                }
            },
            onSubmit: function(button, fileName) {
                FreeswitchConfig.Site.Modals.ShowUploading();
            },
            onComplete: function(button, fileName, response) {
                FreeswitchConfig.Site.Modals.HideUploading();
                FreeswitchConfig.Site.Modals.ShowLoading();
                FreeswitchConfig.Services.BackupRestoreService.RestoreData(
                    response,
                    'Voicemail',
                    function(msg) {
                        FreeswitchConfig.Site.Modals.HideLoading();
                        if (msg) {
                            alert('Restoring the voicemails was completed successfully');
                        } else {
                            alert('An error occured restoring the voicemails.');
                        }
                    });
            }
        });

        container.append([
            FreeswitchConfig.Site.Skin.span.Create({ Attributes: { 'style': 'cursor:pointer;padding-right:5px;border-right:1px solid black;margin-right:5px;', 'onClick': 'window.open(\'/FileDownloader.ashx?FileType=Backup&Level=Recordings\',\'\',\'scrollbars=no,menubar=no,height=600,width=800,resizable=yes,toolbar=no,location=no,status=no\');' }, Content: [
                FreeswitchConfig.Site.Skin.img.Create({ Class: 'cd_burn', Attributes: { 'style': 'padding-right:5px;', 'title': 'Add New'} }),
                'Backup Recordings'
            ]
            }),
            FreeswitchConfig.Site.Skin.span.Create({ Attributes: { 'style': 'cursor:pointer;padding-right:5px;' }, Content: [
                FreeswitchConfig.Site.Skin.img.Create({ Class: 'cd_go', Attributes: { 'title': 'Add New', 'style': 'padding-right:5px;'} }),
                'Restore Recordings'
            ]
            }),
            '<br/>'
        ]);
        spn = $(container.find(FreeswitchConfig.Site.Skin.span.Tag + ':last')[0]);
        spn.AjaxFileUpload({
            action: '/FileUpload.ashx',
            allowedExtensions: ['.fscbak]'],
            onError: function(errorType) {
                switch (errorType) {
                    case 'incorrect_file_type':
                        alert('The file type you specified to upload for restoring the recordings is invalid.');
                        break;
                }
            },
            onSubmit: function(button, fileName) {
                FreeswitchConfig.Site.Modals.ShowUploading();
            },
            onComplete: function(button, fileName, response) {
                FreeswitchConfig.Site.Modals.HideUploading();
                FreeswitchConfig.Site.Modals.ShowLoading();
                FreeswitchConfig.Services.BackupRestoreService.RestoreData(
                    response,
                    'Recordings',
                    function(msg) {
                        FreeswitchConfig.Site.Modals.HideLoading();
                        if (msg) {
                            alert('Restoring the recordings was completed successfully');
                        } else {
                            alert('An error occured restoring the recordings.');
                        }
                    }
                 );
            }
        });

        container.append([
            FreeswitchConfig.Site.Skin.span.Create({ Attributes: { 'style': 'cursor:pointer;padding-right:5px;border-right:1px solid black;margin-right:5px;', 'onClick': 'window.open(\'/FileDownloader.ashx?FileType=Backup&Level=Sounds\',\'\',\'scrollbars=no,menubar=no,height=600,width=800,resizable=yes,toolbar=no,location=no,status=no\');' }, Content: [
                FreeswitchConfig.Site.Skin.img.Create({ Class: 'cd_burn', Attributes: { 'title': 'Add New', 'style': 'padding-right:5px;'} }),
                'Backup Sounds'
            ]
            }),
            FreeswitchConfig.Site.Skin.span.Create({ Attributes: { 'style': 'cursor:pointer;padding-right:5px;' }, Content: [
                FreeswitchConfig.Site.Skin.img.Create({ Class: 'cd_go', Attributes: { 'title': 'Add New', 'style': 'padding-right:5px;'} }),
                'Restore Sounds'
            ]
            }),
            '<br/>'
        ]);
        spn = $(container.find(FreeswitchConfig.Site.Skin.span.Tag + ':last')[0]);
        spn.AjaxFileUpload({
            action: '/FileUpload.ashx',
            allowedExtensions: ['.fscbak]'],
            onError: function(errorType) {
                switch (errorType) {
                    case 'incorrect_file_type':
                        alert('The file type you specified to upload for restoring the sdounds is invalid.');
                        break;
                }
            },
            onSubmit: function(button, fileName) {
                FreeswitchConfig.Site.Modals.ShowUploading();
            },
            onComplete: function(button, fileName, response) {
                FreeswitchConfig.Site.Modals.HideUploading();
                FreeswitchConfig.Site.Modals.ShowLoading();
                FreeswitchConfig.Services.BackupRestoreService.RestoreData(
                    response,
                    'Sounds',
                    function(msg) {
                        FreeswitchConfig.Site.Modals.HideLoading();
                        if (msg) {
                            alert('Restoring the sounds was completed successfully');
                        } else {
                            alert('An error occured restoring the sounds.');
                        }
                    }
                 );
            }
        });


        //scripts section
        container.append([
            FreeswitchConfig.Site.Skin.h3.Create('Scripts'),
            FreeswitchConfig.Site.Skin.span.Create({ Attributes: { 'style': 'cursor:pointer;padding-right:5px;border-right:1px solid black;margin-right:5px;', 'onClick': 'window.open(\'/FileDownloader.ashx?FileType=Backup&Level=Script\',\'\',\'scrollbars=no,menubar=no,height=600,width=800,resizable=yes,toolbar=no,location=no,status=no\');' }, Content: [
                FreeswitchConfig.Site.Skin.img.Create({ Class: 'script_save', Attributes: { 'title': 'Add New', 'style': 'padding-right:5px;'} }),
                'Backup Scripts'
            ]
            }),
            FreeswitchConfig.Site.Skin.span.Create({ Attributes: { 'style': 'cursor:pointer;padding-right:5px;' }, Content: [
                FreeswitchConfig.Site.Skin.img.Create({ Class: 'script_go', Attributes: { 'title': 'Add New', 'style': 'padding-right:5px;'} }),
                'Restore Scripts'
            ]
            }),
            '<br/>'
        ]);
        spn = $(container.find(FreeswitchConfig.Site.Skin.span.Tag + ':last')[0]);
        spn.AjaxFileUpload({
            action: '/FileUpload.ashx',
            allowedExtensions: ['.fscbak]'],
            onError: function(errorType) {
                switch (errorType) {
                    case 'incorrect_file_type':
                        alert('The file type you specified to upload for restoring the scripts is invalid.');
                        break;
                }
            },
            onSubmit: function(button, fileName) {
                FreeswitchConfig.Site.Modals.ShowUploading();
            },
            onComplete: function(button, fileName, response) {
                FreeswitchConfig.Site.Modals.HideUploading();
                FreeswitchConfig.Site.Modals.ShowLoading();
                FreeswitchConfig.Services.BackupRestoreService.RestoreData(
                    response,
                    'Scripts',
                    function(msg) {
                        FreeswitchConfig.Site.Modals.HideLoading();
                        if (msg) {
                            alert('Restoring the scripts was completed successfully');
                        } else {
                            alert('An error occured restoring the scripts.');
                        }
                    }
                 );
            }
        });

        //Complete Section
        container.append([
            FreeswitchConfig.Site.Skin.h3.Create('Complete'),
            FreeswitchConfig.Site.Skin.span.Create({ Attributes: { 'style': 'cursor:pointer;padding-right:5px;border-right:1px solid black;margin-right:5px;', 'onClick': 'window.open(\'/FileDownloader.ashx?FileType=Backup&Level=Complete\',\'\',\'scrollbars=no,menubar=no,height=600,width=800,resizable=yes,toolbar=no,location=no,status=no\');' }, Content: [
                FreeswitchConfig.Site.Skin.img.Create({ Class: 'drive_disk', Attributes: { 'title': 'Add New', 'style': 'padding-right:5px;'} }),
                'Create Complete Backup'
            ]
            }),
            FreeswitchConfig.Site.Skin.span.Create({ Attributes: { 'style': 'cursor:pointer;padding-right:5px;' }, Content: [
                FreeswitchConfig.Site.Skin.img.Create({ Class: 'drive_go', Attributes: { 'title': 'Add New', 'style': 'padding-right:5px;'} }),
                'Restore from Complete Backup'
            ]
            }),
            '<br/>'
        ]);
        spn = $(container.find(FreeswitchConfig.Site.Skin.span.Tag + ':last')[0]);
        spn.AjaxFileUpload({
            action: '/FileUpload.ashx',
            allowedExtensions: ['.fscbak]'],
            onError: function(errorType) {
                switch (errorType) {
                    case 'incorrect_file_type':
                        alert('The file type you specified to upload for restoring the system is invalid.');
                        break;
                }
            },
            onSubmit: function(button, fileName) {
                FreeswitchConfig.Site.Modals.ShowUploading();
            },
            onComplete: function(button, fileName, response) {
                FreeswitchConfig.Site.Modals.HideUploading();
                FreeswitchConfig.Site.Modals.ShowLoading();
                FreeswitchConfig.Services.BackupRestoreService.RestoreData(
                    response,
                    'Complete',
                    function(msg) {
                        FreeswitchConfig.Site.Modals.HideLoading();
                        if (msg) {
                            alert('Restoring the scripts was completed successfully');
                        } else {
                            alert('An error occured restoring the scripts.');
                        }
                    }
                 );
            }
        });
        FreeswitchConfig.Site.Modals.HideLoading();
    }
});