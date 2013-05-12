CreateNameSpace('FreeswitchConfig.Services.BackupRestoreService');

FreeswitchConfig.Services.BackupRestoreService = $.extend(FreeswitchConfig.Services.BackupRestoreService,{
    GeneratePage: function(container) {
        container = $(container);
        //Database Section
        container.append('<h3>Database</h3>');
        container.append('<span style="cursor:pointer;padding-right:5px;border-right:1px solid black;margin-right:5px;" onClick="window.open(\'/FileDownloader.ashx?FileType=Backup&Level=Database\',\'\',\'scrollbars=no,menubar=no,height=600,width=800,resizable=yes,toolbar=no,location=no,status=no\');">' +
        '<img class="database_save" title="Add New" style="padding-right:5px;"> Backup Configuration Database' +
        '</span>');
        container.append($('<span style="cursor:pointer;padding-right:5px;">' +
        '<img class="database_go" title="Add New" style="padding-right:5px;"> Restore Configuration Database' +
        '</span><br/>'));
        var spn = $(container.find('span:last')[0]);
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
        container.append('<h3>Audio</h3>');
        container.append('<span style="cursor:pointer;padding-right:5px;border-right:1px solid black;margin-right:5px;" onClick="window.open(\'/FileDownloader.ashx?FileType=Backup&Level=Voicemail\',\'\',\'scrollbars=no,menubar=no,height=600,width=800,resizable=yes,toolbar=no,location=no,status=no\');">' +
            '<img class="cd_burn" title="Add New" style="padding-right:5px;"> Backup Voicemail</span>');
        container.append($('<span style="cursor:pointer;padding-right:5px;"><img class="cd_go" title="Add New" style="padding-right:5px;"> Restore Voicemail</span><br/>'));
        spn = $(container.find('span:last')[0]);
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

        container.append('<span style="cursor:pointer;padding-right:5px;border-right:1px solid black;margin-right:5px;" onClick="window.open(\'/FileDownloader.ashx?FileType=Backup&Level=Recordings\',\'\',\'scrollbars=no,menubar=no,height=600,width=800,resizable=yes,toolbar=no,location=no,status=no\');">' +
            '<img class="cd_burn" title="Add New" style="padding-right:5px;"> Backup Recordings' +
            '</span>');
        container.append($('<span style="cursor:pointer;padding-right:5px;"><img class="cd_go" title="Add New" style="padding-right:5px;"> Restore Recordings</span><br/>'));
        spn = $(container.find('span:last')[0]);
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

        container.append('<span style="cursor:pointer;padding-right:5px;border-right:1px solid black;margin-right:5px;" onClick="window.open(\'/FileDownloader.ashx?FileType=Backup&Level=Sounds\',\'\',\'scrollbars=no,menubar=no,height=600,width=800,resizable=yes,toolbar=no,location=no,status=no\');">' +
            '<img class="cd_burn" title="Add New" style="padding-right:5px;"> Backup Sounds' +
            '</span>');
        container.append($('<span style="cursor:pointer;padding-right:5px;"><img class="cd_go" title="Add New" style="padding-right:5px;"> Restore Sounds</span><br/>'));
        spn = $(container.find('span:last')[0]);
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
        container.append('<h3>Scripts</h3>');
        container.append('<span style="cursor:pointer;padding-right:5px;border-right:1px solid black;margin-right:5px;" onClick="window.open(\'/FileDownloader.ashx?FileType=Backup&Level=Script\',\'\',\'scrollbars=no,menubar=no,height=600,width=800,resizable=yes,toolbar=no,location=no,status=no\');">' +
            '<img class="script_save" title="Add New" style="padding-right:5px;"> Backup Scripts' +
            '</span>');
        container.append($('<span style="cursor:pointer;padding-right:5px;"><img class="script_go" title="Add New" style="padding-right:5px;"> Restore Scripts</span><br/>'));
        spn = $(container.find('span:last')[0]);
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
        container.append('<h3>Complete</h3>');
        container.append('<span style="cursor:pointer;padding-right:5px;border-right:1px solid black;margin-right:5px;" onClick="window.open(\'/FileDownloader.ashx?FileType=Backup&Level=Complete\',\'\',\'scrollbars=no,menubar=no,height=600,width=800,resizable=yes,toolbar=no,location=no,status=no\');">' +
            '<img class="drive_disk" title="Add New" style="padding-right:5px;"> Create Complete Backup' +
            '</span>');
        container.append($('<span style="cursor:pointer;padding-right:5px;"><img class="drive_go" title="Add New" style="padding-right:5px;"> Restore from Complete Backup</span><br/>'));
        spn = $(container.find('span:last')[0]);
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