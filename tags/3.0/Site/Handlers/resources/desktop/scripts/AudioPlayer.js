FreeswitchConfig.Site = $.extend(FreeswitchConfig.Site, { WavPlayer: {
    GetHiddenPlayer: function(container) {
        if (!!document.createElement('audio').canPlayType) {
            return container.find('audio')[0];
        } else {
            if ($.find('div[name="HiddenPlayer"]:first').length == 0) {
                $($.find('body')[0]).append($('<div name="HiddenPlayer" style="display:none;"></div>'));
            }
            return $($.find('div[name="HiddenPlayer"]:first'));
        }
    },
    CreateNewPlayer: function(file) {
        var ret = $('<span></span>');
        ret.append('<img title="Play File" class="Button control_play_blue"/>');
        if (!!document.createElement('audio').canPlayType) {
            ret.append('<img title="Continue" class="Button control_play_blue" style="display:none;"/>');
            ret.append('<img title="Pause Playback" class="Button control_pause_blue" style="display:none;"/>');
        }
        ret.append('<img title="Stop Playback" class="Button control_stop_blue" style="display:none;"/>');
        var butPlay = $(ret.find('img:first')[0]);
        if (!!document.createElement('audio').canPlayType) {
            var butContinue = $(butPlay.next());
            var butPause = $(butContinue.next());
            var butStop = $(butPause.next());
        } else {
            var butStop = $(butPlay.next());
        }

        butPlay.bind('click',
        { file: file, pause: butPause, stop: butStop, button: butPlay, butContinue: butContinue, container: ret },
        function(event) {
            FreeswitchConfig.Site.Modals.ShowLoading();
            if (!!document.createElement('audio').canPlayType) {
                var auds = $.find('img[title="Pause Playback"][isshowing="true"]');
                for (var y = 0; y < auds.length; y++) {
                    $(auds[y]).click();
                }
                event.data.container.append('<div style="display:none;"><audio src="' + event.data.file + '" onloadeddata="FreeswitchConfig.Site.Modals.HideLoading();this.play();"/></div>');
            } else {
                var player = Org.Reddragonit.FreeSwitchConfig.Site.WavPlayer.GetHiddenPlayer(event.data.container);
                if (player.html() != '') {
                    $($.find('img[title="Stop Playback"]')).hide();
                    player.html('');
                }
                player.append('<embed src="' + file + '" autostart="true" width="0" height="0"/>');
            }
            event.data.pause.attr('isshowing', 'true');
            event.data.pause.show();
            event.data.stop.show();
            event.data.button.hide();
            FreeswitchConfig.Site.Modals.HideLoading();
        });

        if (!!document.createElement('audio').canPlayType) {
            butContinue.bind('click',
            { file: file, pause: butPause, button: butContinue, container: ret },
            function(event) {
                var player = Org.Reddragonit.FreeSwitchConfig.Site.WavPlayer.GetHiddenPlayer(event.data.container);
                player.play();
                event.data.button.hide();
                event.data.pause.show();
            });

            butPause.bind('click',
            { file: file, pause: butPause, button: butContinue, container: ret },
            function(event) {
                var player = Org.Reddragonit.FreeSwitchConfig.Site.WavPlayer.GetHiddenPlayer(event.data.container);
                player.pause();
                event.data.button.show();
                event.data.pause.hide();
                event.data.pause.attr('isshowing', 'false');
            });
        }

        butStop.bind('click',
        { file: file, pause: butPause, stop: butStop, button: butPlay, butContinue: butContinue, container: ret },
        function(event) {
            var player = Org.Reddragonit.FreeSwitchConfig.Site.WavPlayer.GetHiddenPlayer(event.data.container);
            if (!!document.createElement('audio').canPlayType) {
                player.pause();
                player.currentTime = 0;
            } else {
                player.html('');
            }
            event.data.pause.hide();
            event.data.stop.hide();
            event.data.butContinue.hide();
            event.data.pause.attr('isshowing', 'false');
            event.data.button.show();
        });

        return ret;
    }
}
});