CreateNameSpace('FreeswitchConfig.Core.DeploymentMethod');

FreeswitchConfig.Core.DeploymentMethod = $.extend(
FreeswitchConfig.Core.DeploymentMethod,
{
    CollectionView: Backbone.View.extend({
        tagName: "table",
        className: "FreeswitchConfig Core DeploymentMethod View Rowed",
        attributes : { cellspacing:0,cellpadding:0},
        initialize: function() {
            this.collection.on('reset', this.render, this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        render: function() {
            var el = this.$el;
            el.html('');
            el.append('<thead><tr><th></th><th>Name</th><th>Description</th></tr></thead>');
            el.append($('<tbody></tbody>'));
            el = $(el.children()[1]);
            var alt = false;
            for (var x = 0; x < this.collection.length; x++) {
                var model = this.collection.at(x);
                el.append('<tr class="' + this.className + (alt ? ' Alt' : '') + '"><td><input type="radio" value="' + x + '" ' + (model.get('IsCurrent') ? 'checked="checked"' : '') + '/></td>' +
                '<td class="' + this.className + ' Name">' + model.get('Name') + '</td>' +
                '<td class="' + this.className + ' Description">' + model.get('Description') + '</td></tr>');
            }
            this.trigger('render', this);
            return this;
        }
    }),
    GeneratePage: function(container) {
        container = $(container);
        FreeswitchConfig.Site.Modals.ShowLoading();
        var col = new FreeswitchConfig.Core.DeploymentMethod.Collection();
        var vw = new FreeswitchConfig.Core.DeploymentMethod.CollectionView({ collection: col });
        vw.on('render', function() { FreeswitchConfig.Site.Modals.HideLoading(); FreeswitchConfig.Site.Modals.HideSaving(); });
        container.append(vw.$el);
        col.fetch();
        var butSave = CreateButton(
                'accept',
                'Change Method',
                function(button, pars) {
                    FreeswitchConfig.Site.Modals.ShowUpdating();
                    var model = pars.col.get($(pars.vw.$el.find('input:checked')[0]).val());
                    for (var x = 0; x < pars.col.length; x++) {
                        pars.col.get(x).set({ IsCurrent: false });
                    }
                    model.set({ IsCurrent: true });
                    model.save();
                    pars.vw.render();
                },
                { vw: vw, col: col }
            );
        var butCancel = CreateButton(
                'cancel',
                'Cancel',
                function(button, pars) {
                    FreeswitchConfig.Site.Modals.ShowLoading();
                    pars.vw.render();
                },
                { vw: vw }
            );
        container.append(butSave);
        container.append(butCancel);
    }
});