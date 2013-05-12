CreateNameSpace('FreeswitchConfig.System.sNetworkCard');
FreeswitchConfig.System.sNetworkCard = $.extend(FreeswitchConfig.System.sNetworkCard, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: "tr",
        className: "Org Reddragonit FreeSwitchConfig DataCore System sNetworkCard View",
        render: function() {
            $(this.el).html('<td ' + (this.model.get('IsBondSlave') ? '>&nbsp;</td><td ' : '') + (this.model.get('IsBondMaster') ? 'colspan="2"' : '') + ' class="' + this.className + ' Name">' + (this.model.get('Name') == null ? '' : this.model.get('Name')) + '</td>'
            + '<td class="' + this.className + ' IsDHCP">' + (this.model.get('IsDHCP') ? '<img class="tick"/>' : '') + '</td>'
            + '<td class="' + this.className + ' Speed">' + (this.model.get('Speed') == null ? '' : this.model.get('Speed')) + '</td>'
            + '<td class="' + this.className + ' IPAddress NetworkMask">' + (this.model.get('IPAddress') == null ? '' : this.model.get('IPAddress')) + '/' + (this.model.get('NetworkMask') == null ? '' : this.model.get('NetworkMask')) + '</td>'
            + '<td class="' + this.className + ' Gateway">' + (this.model.get('Gateway') == null ? '' : this.model.get('Gateway')) + '</td>'
            + '<td class="' + this.className + ' MAC">' + (this.model.get('MAC') == null ? '' : this.model.get('MAC')) + '</td>'
            + '<td class="' + this.className + ' Live">' + (this.model.get('Live') == null ? '' : (this.model.get('Live') ? '<img class="tick"/>' : '')) + '</td>');
            $(this.el).attr('name', this.model.id);
            this.trigger('render', this);
            return this;
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: "table",
        className: "Org Reddragonit FreeSwitchConfig DataCore System sNetworkCard CollectionView Rowed",
        attributes: {cellpadding:'0',cellspacing:'0'},
        initialize: function() {
            this.collection.on('reset', this.render, this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        render: function() {
            var el = this.$el;
            el.html('');
            var thead = $('<thead class="' + this.className + 'header"></thead>');
            el.append(thead);
            thead.append('<tr></tr>');
            thead = $(thead.children()[0]);
            var hasBonding = false;
            for (var x = 0; x < this.collection.length; x++) {
                if (this.collection.at(x).get('IsBondMaster')) {
                    hasBonding = true;
                    x = this.collection.length;
                }
            }
            thead.append('<th className="' + this.className + ' Name">Name</th>');
            thead.append('<th className="' + this.className + ' IsDHCP">IsDHCP</th>');
            thead.append('<th className="' + this.className + ' Speed">Speed</th>');
            thead.append('<th className="' + this.className + ' IPAddress NetworkMask">IP Address/Network Mask</th>');
            thead.append('<th className="' + this.className + ' Gateway">Gateway</th>');
            thead.append('<th className="' + this.className + ' MAC">MAC</th>');
            thead.append('<th className="' + this.className + ' Live">Live</th>');
            el.append('<tbody></tbody>');
            el = $(el.children()[0]);
            if (this.collection.length == 0) {
                this.trigger('render', this);
            } else {
                var alt = false;
                for (var x = 0; x < this.collection.length; x++) {
                    if (!this.collection.at(x).get('IsBondSlave')) {
                        var vw = new FreeswitchConfig.System.sNetworkCard.View({ model: this.collection.at(x) });
                        if (alt) {
                            vw.$el.attr('class', vw.$el.attr('class') + ' Alt');
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