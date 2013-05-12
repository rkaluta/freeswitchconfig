CreateNameSpace('FreeswitchConfig.Core.SiteModule');
FreeswitchConfig.Core.SiteModule = $.extend(FreeswitchConfig.Core.SiteModule, { CollectionView: Backbone.View.extend({
    tagName: "table",
    className: "FreeswitchConfig Core SiteModule View Rowed",
    initialize: function() {
        this.collection.on('reset', this.render, this);
        this.collection.on('add', this.render, this);
        this.collection.on('remove', this.render, this);
    },
    render: function() {
        var el = this.$el;
        el.html('');
        el.attr('cellpadding', '0');
        el.attr('cellspacing', '0');
        el.append('<thead><tr><th>Module Name</th><th>Description</th><th style="width:110px;">Actions</th></tr></thead>');
        el.append($('<tbody></tbody>'));
        el = $(el.children()[1]);
        var alt = false;
        for (var x = 0; x < this.collection.length; x++) {
            var model = this.collection.at(x);
            el.append($('<tr class="' + this.className + (alt ? ' Alt' : '') + '"><td class="' + this.className + ' ModuleName">' + model.get('ModuleName') + '</td>' +
                    '<td class="' + this.className + ' Description">' + model.get('Description') + '</td>' +
                    '<td class="' + this.className + ' buttons" style="valign:top;"></td></tr>'));
            var td = $(el.find('td:last')[0]);
            td.append(CreateButton(
                (model.get('Enabled') ? 'cancel' : 'accept'),
                (model.get('Enabled') ? 'Disable' : 'Enable'),
                function(button, pars) {
                    FreeswitchConfig.Site.Modals.ShowUpdating();
                    pars.model.save({ Enabled: !pars.model.get('Enabled') }, {
                        success: function(model, response, options) {
                            FreeswitchConfig.Site.Modals.HideUpdating();
                        },
                        failure: function() {
                            FreeswitchConfig.Site.Modals.HideUpdating();
                            alert('An error occured attempting to enable the module');
                        }
                    });
                },
                { model: model }
            ));
            model.on('change', function() {
                if (this.model.get('Enabled')) {
                    $(this.td.find('strong')[0]).html('<img class="cancel"/>Disable');
                } else {
                    $(this.td.find('strong')[0]).html('<img class="accept"/>Enable');
                }
            }, { model: model, td: td });
            alt = !alt;
        }
        this.trigger('render', this);
    }
}),
    GeneratePage: function(container) {
        FreeswitchConfig.Site.Modals.ShowLoading();
        var div = $(container);
        var col = new FreeswitchConfig.Core.SiteModule.Collection();
        var view = new FreeswitchConfig.Core.SiteModule.CollectionView({ collection: col });
        div.append(view.$el);
        col.fetch();
        FreeswitchConfig.Site.Modals.HideLoading();
    }
});