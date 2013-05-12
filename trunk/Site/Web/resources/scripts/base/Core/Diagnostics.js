FreeswitchConfig.Services.DiagnosticsService = $.extend(FreeswitchConfig.Services.DiagnosticsService, {
    GeneratePage: function(container) {
        FreeswitchConfig.Services.DiagnosticsService.ProcessDiagnostics(
          function(msg) {
              var html = '';
              for (i in msg) {
                  html += '<h3>' + i + '</h3><ul>';
                  for (var x = 0; x < msg[i].length; x++) {
                      html += '<li>' + msg[i][x] + '</li>';
                  }
                  html += '</ul>'
              }
              FreeswitchConfig.Site.Modals.ShowFormPanel('Diagnostics Results', html, [CreateButton('accept', 'Okay', function(button) { FreeswitchConfig.Site.Modals.HideFormPanel(); })]);
          },
          null,
          null,
          true
        );
    }
});