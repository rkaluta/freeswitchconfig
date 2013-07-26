CreateNameSpace('FreeswitchConfig.Core.DeploymentMethod');

FreeswitchConfig.Core.DeploymentMethod = $.extend(
FreeswitchConfig.Core.DeploymentMethod,
{
    CollectionView: Backbone.View.extend({
        tagName: FreeswitchConfig.Site.Skin.table.Tag,
        className: "FreeswitchConfig Core DeploymentMethod View " + FreeswitchConfig.Site.Skin.table.Class,
        attributes: $.extend({},FreeswitchConfig.Site.Skin.table.Attributes,{ cellspacing: 0, cellpadding: 0 }),
        initialize: function() {
            this.collection.on('reset', this.render, this); this.collection.on('sync',this.render,this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        render: function() {
            this.$el.html('');
            this.$el.append(FreeswitchConfig.Site.Skin.thead.Create({ Content:
                FreeswitchConfig.Site.Skin.tr.Create({ Content: [
                    FreeswitchConfig.Site.Skin.th.Create('&nbsp;'),
                    FreeswitchConfig.Site.Skin.th.Create('Name'),
                    FreeswitchConfig.Site.Skin.th.Create('Description')
                ]
                })
            }));
            var el = FreeswitchConfig.Site.Skin.tbody.Create();
            this.$el.append(el);
            var alt = false;
            for (var x = 0; x < this.collection.length; x++) {
                var model = this.collection.at(x);
                var cont = [
                    FreeswitchConfig.Site.Skin.td.Create({ Content: '<input type="radio" value="' + x + '" ' + (model.get('IsCurrent') ? 'checked' : '') + '/>' }),
                    FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Name', Content: model.get('Name') }),
                    FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Description', Content: model.get('Description') })
                ];
                el.append((alt ? FreeswitchConfig.Site.Skin.tr.CreateAlt({ Class: this.className, Content: cont }) :
                FreeswitchConfig.Site.Skin.tr.Create({ Class: this.className, Content: cont })));
                alt = !alt;
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
        vw.on('render', function() { FreeswitchConfig.Site.Modals.HideLoading(); FreeswitchConfig.Site.Modals.HideUpdating(); });
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