FreeswitchConfig.Services.NetworkTestService.AvailableInterfaces(
    function(msg) {
        var opts = '';
        for (var y = 0; y < msg.length; y++) {
            opts += '<option value="' + msg[y] + '">' + msg[y] + '</option>';
        }
        var div = $('<div>' +
                    ' Interface To Use: <select name="interfaces">' + opts + '</select><br/>' +
                    ' IP To PING: <input type="text" name="pingIP"/><br/>' +
                    ' Count: <input type="text" name="pingCount"/><br/>' +
                    ' <center><img class="Button accept"/><img class="Button cancel"/></center><br/>' +
                    ' <div name="results"></div>' +
                    ' </div>');
        container.append(div);
        var butCancel = $(div.find('img:last')[0]);
        butCancel.bind('click',
        { div: div },
        function(event) {
            $(event.data.div.find('input[name="pingIP"]')[0]).val('');
            $(event.data.div.find('input[name="pingCount"]')[0]).val('');
            var cont = $(event.data.div.find('div[name="results"]')[0]);
            cont.html('');
            cont.attr('style', '');
        });
        var butRunTest = $(butCancel.prev());
        butRunTest.bind('click',
        { div: div },
        function(event) {
            FreeswitchConfig.Site.Modals.ShowLoading();
            var cont = $(event.data.div.find('div[name="results"]')[0]);
            cont.html('');
            var canSubmit = IsValidIPAddress(event.data.div.find('input[name="pingIP"]')[0]);
            canSubmit = canSubmit || IsValidPositiveInteger(event.data.div.find('input[name="pingCount"]')[0]);
            canSubmit = canSubmit || $(event.data.div.find('input[name="pingIP"]')[0]).val()!='';
            canSubmit = canSubmit || $(event.data.div.find('input[name="pingCount"]')[0]).val()!='';
            if (!canSubmit){
                FreeswitchConfig.Site.Modals.HideLoading();
                alert('You must specify an IP Address to ping to and a valid count (number of times to try)');
            }else{
               FreeswitchConfig.Services.NetworkTestService.GetPacketLoss(
                    $(event.data.div.find('select[name="interfaces"]')[0]).val(),
                    $(event.data.div.find('input[name="pingIP"]')[0]).val(),
                    $(event.data.div.find('input[name="pingCount"]')[0]).val(),
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
        });
    },
    function(msg) {
        FreeswitchConfig.Site.Modals.HideLoading();
        alert('An error occured communicating with the Network Testing Service.');  
    },
    null,
    true
);