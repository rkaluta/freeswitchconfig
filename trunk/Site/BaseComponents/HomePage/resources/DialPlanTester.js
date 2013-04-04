FreeswitchConfig.Services.DialPlanTestService.GetAvailableExtensions(
    function(msg) {
        var opts = [];
        for (var y = 0; y < msg.length; y++) {
            opts.push(new FreeswitchConfig.Site.Form.SelectValue(msg[y]));
        }
        var frm = FreeswitchConfig.Site.Form.GenerateForm(null,
        [
            new FreeswitchConfig.Site.Form.FormInput('dialingExt', 'select', opts, null, 'Dialing Extension'),
            new FreeswitchConfig.Site.Form.FormInput('dialedNumber', 'text', null, null, 'Dialed Number'),
            new FreeswitchConfig.Site.Form.FormInput('inputtedPin', 'text', null, null, 'Pin'),
            new FreeswitchConfig.Site.Form.FormInput('inputtedDate', 'datetime', null, null, 'Date')
        ],
        [
            new FreeswitchConfig.Site.Form.FormButton('accept', 'Test', function(frm) {
                FreeswitchConfig.Site.Modals.ShowLoading();
                var cont = $($(frm.parent()).find('div[name="results"]')[0]);
                cont.html('');
                FreeswitchConfig.Services.DialPlanTestService.TestDialedNumber(
                $(frm.find('select[name="dialingExt"]')[0]).val(),
                $(frm.find('input[name="dialedNumber"]')[0]).val(),
                $(frm.find('input[name="inputtedPin"]')[0]).val(),
                ($(frm.find('input[name="inputtedDate"]')[0]).val() == '' ? null : new Date($(frm.find('input[name="inputtedDate"]')[0])).val()),
                function(msg) {
                    cont.html('<b>RESULTS:</b><br/>' + msg.replaceAll('\n', '<br/>'));
                    FreeswitchConfig.Site.Modals.HideLoading();
                },
                function(msg) {
                    FreeswitchConfig.Site.Modals.HideLoading();
                    alert('An error occured communicating with the Dial Plan Testing Service.');
                }
            );
            }),
            new FreeswitchConfig.Site.Form.FormButton('cancel', 'Cancel', function(frm) {
                $($(frm.parent()).find('div[name="results"]')[0]).html('');
                $(frm.find('input[name="dialedNumber"]')[0]).val('');
                $(frm.find('input[name="inputtedPin"]')[0]).val('');
                $(frm.find('input[name="inputtedDate"]')[0]).val('');
            })
        ]
        );
        container.append(frm);
        container.append('<div name="results"></div>');
    },
    function(msg) {
        FreeswitchConfig.Site.Modals.HideLoading();
        alert('An error occured communicating with the Dial Plan Testing Service.');
    }, null,
    true
);