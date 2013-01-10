FreeswitchConfig.Services.DialPlanTestService.GetAvailableExtensions(
    function(msg) {
        var opts = '';
        for (var y = 0; y < msg.length; y++) {
            opts += '<option value="' + msg[y] + '">' + msg[y] + '</option>';
        }
        var div = $('<div>Dialing Extension: <select name="dialingExt">' + opts + '</select><br/>' +
            ' Dialed Number: <input type="text" name="dialedNumber"/><br/>' +
            ' Pin: <input type="text" name="inputtedPin"/><br/>' +
            ' Date: <input type="text" name="inputtedDate"/><img class="Button calendar"/><br/>' +
            ' <center><img class="Button accept"/><img class="Button cancel"/></center><br/>' +
            ' <div name="results"></div></div>');
        container.append(div);
        var butTime = $(div.find('input[name="inputtedDate"]')[0]);
        FreeswitchConfig.Site.DateTimePicker.AttachToInput(butTime, { button: butTime.next() });
        var butCancel = $(div.find('img:last')[0]);
        butCancel.bind('click',
        { div: div },
        function(event) {
            $(event.data.div.find('input[name="dialedNumber"]')[0]).val('');
            $(event.data.div.find('input[name="inputtedPin"]')[0]).val('');
            $(event.data.div.find('input[name="inputtedDate"]')[0]).val('');
            var cont = $(event.data.div.find('div[name="results"]')[0]);
            cont.html('');
        });
        var butRunTest = $(butCancel.prev());
        butRunTest.bind('click',
        { div: div },
        function(event) {
            FreeswitchConfig.Site.Modals.ShowLoading();
            var cont = $(event.data.div.find('div[name="results"]')[0]);
            cont.html('');
           FreeswitchConfig.Services.DialPlanTestService.TestDialedNumber(
                $(event.data.div.find('select[name="dialingExt"]')[0]).val(),
                $(event.data.div.find('input[name="dialedNumber"]')[0]).val(),
                $(event.data.div.find('input[name="inputtedPin"]')[0]).val(),
                ($(event.data.div.find('input[name="inputtedDate"]')[0]).val() == '' ? null : FreeswitchConfig.Site.DateTimePicker.GetDateValueFromField(event.data.div.find('input[name="inputtedDate"]')[0])),
                function(msg) {
                    cont.html('<b>RESULTS:</b><br/>' + msg.replaceAll('\n', '<br/>'));
                    FreeswitchConfig.Site.Modals.HideLoading();
                },
                function(msg) {
                    FreeswitchConfig.Site.Modals.HideLoading();
                    alert('An error occured communicating with the Dial Plan Testing Service.');
                }
            );
        });
    },
    function(msg) {
        FreeswitchConfig.Site.Modals.HideLoading();
        alert('An error occured communicating with the Dial Plan Testing Service.');
    }, null,
    true
);