FreeswitchConfig.Services.NetworkTestService.AvailableInterfaces(
    function(msg) {
        var opts = [];
        for (var y = 0; y < msg.length; y++) {
            opts.push(new FreeswitchConfig.Site.Form.SelectValue(msg[y]));
        }
        var frm = FreeswitchConfig.Site.Form.GenerateForm(null,
        [
            new FreeswitchConfig.Site.Form.FormInput('interfaces', 'select', opts, null, 'Interface To Use'),
            new FreeswitchConfig.Site.Form.FormInput('pingIP', 'text', null, null, 'IP To PING'),
            new FreeswitchConfig.Site.Form.FormInput('pingCount', 'text', null, null, 'Count')
        ],
        [
            new FreeswitchConfig.Site.Form.FormButton('accept', 'Test', function(frm) {
                FreeswitchConfig.Site.Modals.ShowLoading();
                var cont = $($(frm.parent()).find('div[name="results"]')[0]);
                cont.html('');
                var canSubmit = FreeswitchConfig.Site.Validation.IsValidIPAddress(frm.find('input[name="pingIP"]')[0]);
                canSubmit = canSubmit || FreeswitchConfig.Site.Validation.IsValidPositiveInteger(frm.find('input[name="pingCount"]')[0]);
                canSubmit = canSubmit || $(frm.find('input[name="pingIP"]')[0]).val() != '';
                canSubmit = canSubmit || $(frm.find('input[name="pingCount"]')[0]).val() != '';
                if (!canSubmit) {
                    FreeswitchConfig.Site.Modals.HideLoading();
                    alert('You must specify an IP Address to ping to and a valid count (number of times to try)');
                } else {
                    FreeswitchConfig.Services.NetworkTestService.GetPacketLoss(
                            $(frm.find('select[name="interfaces"]')[0]).val(),
                            $(frm.find('input[name="pingIP"]')[0]).val(),
                            $(frm.find('input[name="pingCount"]')[0]).val(),
                            function(msg) {
                                cont.html('<b>RESULTS:</b><br/>' + msg);
                                FreeswitchConfig.Site.Modals.HideLoading();
                            },
                            function(msg) {
                                FreeswitchConfig.Site.Modals.HideLoading();
                                alert('An error occured communicating with the Network Testing Service.');
                            }
                        );
                }
            }),
            new FreeswitchConfig.Site.Form.FormButton('cancel', 'Cancel', function(frm) {
                $($(frm.parent()).find('div[name="results"]')[0]).html('');
                $(frm.find('input[name="pingIP"]')[0]).val('');
                $(frm.find('input[name="pingCount"]')[0]).val('');
            })
        ]);
        container.append(frm);
        container.append('<div name="results"></div>');
    },
    function(msg) {
        FreeswitchConfig.Site.Modals.HideLoading();
        alert('An error occured communicating with the Network Testing Service.');
    },
    null,
    true
);