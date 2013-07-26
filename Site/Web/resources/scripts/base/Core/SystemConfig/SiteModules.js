CreateNameSpace('FreeswitchConfig.Core.SiteModule');
FreeswitchConfig.Core.SiteModule = $.extend(FreeswitchConfig.Core.SiteModule, { CollectionView: Backbone.View.extend({
    tagName: FreeswitchConfig.Site.Skin.table.Tag,
    className: "FreeswitchConfig Core SiteModule View " + FreeswitchConfig.Site.Skin.table.Class,
    attributes: $.extend({}, FreeswitchConfig.Site.Skin.table.Attributes, { cellspacing: 0, cellpadding: 0 }),
    initialize: function() {
        this.collection.on('reset', this.render, this); this.collection.on('sync',this.render,this);
        this.collection.on('add', this.render, this);
        this.collection.on('remove', this.render, this);
    },
    render: function() {
        this.$el;
        this.$el.html('');
        this.$el.append(
            FreeswitchConfig.Site.Skin.thead.Create({ Content: [
                FreeswitchConfig.Site.Skin.tr.Create({ Content: [
                    FreeswitchConfig.Site.Skin.th.Create('Module Name'),
                    FreeswitchConfig.Site.Skin.th.Create('Description'),
                    FreeswitchConfig.Site.Skin.th.Create({ Content: 'Actions', Attributes: { style: 'width:110px'} })
                ]
                })
            ]
            })
        );
        var el = FreeswitchConfig.Site.Skin.tbody.Create();
        this.$el.append(el);
        var alt = false;
        for (var x = 0; x < this.collection.length; x++) {
            var model = this.collection.at(x);
            var tr = (alt ? FreeswitchConfig.Site.Skin.tr.CreateAlt({ Class: this.className }) : FreeswitchConfig.Site.Skin.tr.Create({ Class: this.className }));
            el.append(tr);
            var td = FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' buttons', Attributes: { style: 'valign:top' }, Content: CreateButton(
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
            )
            });
            tr.append([
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' ModuleName', Content: model.get('ModuleName') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Description', Content: model.get('Description') }),
                td
            ]);
            model.on('change', function() {
                if (this.model.get('Enabled')) {
                    this.td.html(td.html().replaceAll('cancel', 'accept').replaceAll('Disable', 'Enable'));
                } else {
                    this.td.html(td.html().replaceAll('accept', 'cancel').replaceAll('Enable','Disable'));
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