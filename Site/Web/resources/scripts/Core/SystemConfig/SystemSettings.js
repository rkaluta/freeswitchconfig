CreateNameSpace('FreeswitchConfig.Core.SystemSetting');
FreeswitchConfig.Core.SystemSetting = $.extend(FreeswitchConfig.Core.SystemSetting, {
    GeneratePage: function(container) {
        container = $(container);
        FreeswitchConfig.Site.Modals.ShowLoading();
        var col = new FreeswitchConfig.Core.SystemSetting.Collection();
        var colView = new FreeswitchConfig.Core.SystemSetting.CollectionView({ collection: col, attributes: { cellpadding: 0, cellspacing: 0} });
        colView.on('item_render', function(view) {
            var but = $(view.$el.find('span.edit')[0]);
            but.bind('click', { tr: view.$el, view: view, but: but }, function(event) {
                var inp;
                switch (event.data.view.model.get('ValueType')) {
                    case 'System.Boolean':
                        inp = '<input type="radio" name="Value" value="True" ' + (event.data.view.model.get('Value') ? 'checked' : '') + '/>True<input type="radio" name="Value" value="False" ' + (!event.data.model.view.get('Value') ? 'checked' : '') + '/>False';
                        break;
                    case 'System.String':
                        inp = $('<input type="Text" name="Value"/>');
                        inp.val(event.data.view.model.get('Value'));
                        break;
                }
                $(event.data.tr.children()[0]).html('');
                $(event.data.tr.children()[0]).append(inp);
                var td = $(event.data.tr.children()[1]);
                event.data.but.hide();
                var butAccept = $('<img class="button accept"/>');
                var butCancel = $('<img class="button cancel"/>');
                td.append(butAccept);
                butAccept.bind('click', { view: event.data.view, td: td }, function(evnt) {
                    FreeswitchConfig.Site.Modals.ShowUpdating();
                    var attrs;
                    switch (evnt.data.view.model.get('ValueType')) {
                        case 'System.Boolean':
                            attrs = { Value: evnt.data.td.find('input[value="True"]:checked').length > 0 };
                            break;
                        case 'System.String':
                            attrs = { Value: $(evnt.data.td.find('input')[0]).val() };
                            break;
                    }
                    evnt.data.view.on('render', function() {
                        FreeswitchConfig.Site.Modals.HideUpdating();
                    });
                    evnt.data.view.model.save(attrs, {
                        failure: function() {
                            FreeswitchConfig.Site.Modals.HideUpdating();
                        }
                    });
                });
                td.append(butCancel);
                butCancel.bind('click', { view: event.data.view,buttons:[butAccept,butCancel] }, function(evnt) {
                    $(evnt.data.view.$el.children()[0]).html(evnt.data.view.model.get('Value'));
                    $($(evnt.data.view.$el.children()[1]).find('span')).show();
                    evnt.data.buttons[0].remove();
                    evnt.data.buttons[1].remove();
                });
            });
        });
        colView.on('render', function() { FreeswitchConfig.Site.Modals.HideLoading(); });
        container.append(colView.$el);
        col.fetch();
    }
});