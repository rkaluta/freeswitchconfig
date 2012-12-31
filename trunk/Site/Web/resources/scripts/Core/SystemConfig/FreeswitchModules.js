CreateNameSpace('FreeswitchConfig.Core.FreeswitchModuleConfiguration');

FreeswitchConfig.Core.FreeswitchModuleConfiguration = $.extend(FreeswitchConfig.Core.FreeswitchModuleConfiguration,
{
    GeneratePage: function(container) {
        container = $(container);
        container.append('<strong>WARNING:  Only change these settings files if you know the freeswitch modules well.  Also DO NOT disable the event_socket module as the server requires it to operate properly.</strong><br/>');
        var opts = '<option></option>';
        var sels = FreeswitchConfig.Core.FreeswitchModuleConfiguration.SelectList();
        for (var x = 0; x < sels.length; x++) {
            opts += '<option value="' + sels[x].ID + '">' + sels[x].Text + '</option>';
        }
        container.append($('Configuraiton File: <select name="selModules">' + opts + '</select><div name="dvModSettings"></div>'));
        var sel = $(container.find('select[name="selModules"]')[0]);
        sel.bind('change', { sel: sel }, function(event) {
            var container = $($.find('div[name="dvModSettings"]')[0]);
            container.html('');
            if (event.data.sel.val() != '') {
                var file = new FreeswitchConfig.Core.FreeswitchModuleConfiguration.Model({ id: event.data.sel.val() });
                file.fetch();
                container.append($('<textarea name="configFile" style="min-width:800px;min-height:600px;"></textarea><br/>'));
                $(container.find('textarea')[0]).val(file.get('ConfigurationSection'));
                var butSave = CreateButton(
                        'disk',
                        'Save',
                        function(button, pars) {
                            FreeswitchConfig.Site.Modals.ShowUpdating();
                            var txt = $($.find('textarea[name="configFile"]')[0]);
                            pars.file.set({ ConfigurationSection: txt.val() });
                            if (pars.file.save()) {
                                alert('The configuration file was successfully updated.');
                            } else {
                                alert('An error occured updating the configuration file.');
                            }
                        }, { file: file }
                    );
                container.append(butSave);
            }
        });
        FreeswitchConfig.Site.Modals.HideLoading();
    }
});
