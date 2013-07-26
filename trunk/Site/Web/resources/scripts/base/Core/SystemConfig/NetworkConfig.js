CreateNameSpace('FreeswitchConfig.System.sNetworkCard');

FreeswitchConfig.System.sNetworkCard = $.extend(FreeswitchConfig.System.sNetworkCard, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: FreeswitchConfig.Site.Skin.tr.Tag,
        className: "Org Reddragonit FreeSwitchConfig DataCore System sNetworkCard View",
        render: function() {
            this.$el.html('');
            this.$el.append([
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' IsDHCP', Attributes: { colspan: (this.model.get('IsBondMaster') ? 2 : 1) }, Content: (this.model.get('Name') != null ? this.mdeol.get('Name') : '') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' IsDHCP', Content: (this.model.get('IsDHCP') ? '<img class="tick"/>' : '') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Speed', Content: (this.model.get('Speed') != null ? this.model.get('Speed') : '') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' IPAddress NetworkMask', Content: (this.model.get('IPAddress') != null ? this.model.get('IPAddress') + '/' + (this.model.get('NetworkMask') != null ? this.model.get('NetworkMask') : '') : '') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Gateway', Content: (this.model.get('Gateway') != null ? this.model.get('Gateway') : '') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' MAC', Content: (this.model.get('MAC') != null ? this.model.get('MAC') : '') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Live', Content: (this.model.get('Live') ? '<img class="tick"/>' : '') })
            ]);
            if (this.model.get('IsBondSlave')) {
                this.$el.prepend(FreeswitchConfig.Site.Skin.td.Create('&nbsp;'));
            }
            $(this.el).attr('name', this.model.id);
            this.trigger('render', this);
            return this;
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: FreeswitchConfig.Site.Skin.table.Tag,
        className: "Org Reddragonit FreeSwitchConfig DataCore System sNetworkCard CollectionView " + FreeswitchConfig.Site.Skin.table.Class,
        attributes: $.extend({}, FreeswitchConfig.Site.Skin.table.Attributes, { cellpadding: '0', cellspacing: '0' }),
        initialize: function() {
            this.collection.on('reset', this.render, this); this.collection.on('sync',this.render,this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        render: function() {
            this.$el.html('');
            var hasBonding = false;
            for (var x = 0; x < this.collection.length; x++) {
                if (this.collection.at(x).get('IsBondMaster')) {
                    hasBonding = true;
                    x = this.collection.length;
                }
            }
            this.$el.append(FreeswitchConfig.Site.Skin.thead.Create({ Class: this.className + ' header', Content:
                FreeswitchConfig.Site.Skin.tr.Create({ Content: [
                    FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Name', Content: 'Name' }),
                    FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' IsDHCP', Content: 'IsDHCP' }),
                    FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Speed', Content: 'Speed' }),
                    FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' IPAddress NetworkMask', Content: 'IP Address/Network Mask' }),
                    FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Gateway', Content: 'Gateway' }),
                    FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' MAC', Content: 'MAC' }),
                    FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Live', Content: 'Live' })
                ]
                })
            }));
            var el = FreeswitchConfig.Site.Skin.tbody.Create();
            this.$el.append(el);
            if (this.collection.length == 0) {
                this.trigger('render', this);
            } else {
                var alt = false;
                for (var x = 0; x < this.collection.length; x++) {
                    if (!this.collection.at(x).get('IsBondSlave')) {
                        var vw = new FreeswitchConfig.System.sNetworkCard.View({ model: this.collection.at(x) });
                        if (alt) {
                            vw.$el.addClass(FreeswitchConfig.Site.Skin.tr.AltClass);
                        }
                        if (this.collection.at(x).get('IsBondMaster')) {
                            el.append(vw.$el);
                            var isLast = x + 1 == this.collection.length;
                            if (!isLast) {
                                isLast = true;
                                for (var y = x + 1; y < this.collection.length; y++) {
                                    if (!this.collection(y).get('IsBondSlave')) {
                                        isLast = false;
                                        y = this.collection.length;
                                    }
                                }
                            }
                            for (var y = 0; y < this.collection.length; y++) {
                                if (this.collection.at(y).get('IsBondSlave')) {
                                    if (this.collection.at(x).get('Name') == this.collection.at(y).get('BondParent')) {
                                        var vew = new FreeswitchConfig.System.sNetworkCard.View({ model: this.collection.at(y) });
                                        vew.on('render', function() { this.col.trigger('item_render', this.view); }, { col: this, view: vew });
                                        el.append(vew.$el);
                                        vew.render();
                                    }
                                }
                            }
                            if (isLast) {
                                vw.on('render', function() { this.col.trigger('item_render', this.view); this.col.trigger('render', this.col); }, { col: this, view: vw });
                            } else {
                                vw.on('render', function() { this.col.trigger('item_render', this.view); }, { col: this, view: vw });
                            }
                            alt = !alt;
                            vw.render();
                        } else {
                            alt = !alt;
                            if (x + 1 == this.collection.length) {
                                vw.on('render', function() { this.col.trigger('item_render', this.view); this.col.trigger('render', this.col); }, { col: this, view: vw });
                            } else {
                                vw.on('render', function() { this.col.trigger('item_render', this.view); }, { col: this, view: vw });
                            }
                            el.append(vw.$el);
                            vw.render();
                        }
                    }
                }
            }
        }
    }),
    GeneratePage: function(container) {
        container = $(container);
        var col = new FreeswitchConfig.System.sNetworkCard.Collection();
        var colView = new FreeswitchConfig.System.sNetworkCard.CollectionView({ collection: col });
        colView.on('render', function(view) { FreeswitchConfig.Site.Modals.HideLoading(); }, colView);
        container.append(colView.$el);
        col.fetch();
    }
});