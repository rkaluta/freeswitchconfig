CreateNameSpace('FreeswitchConfig.System.mFirewallRule');
FreeswitchConfig.System.mFirewallRule = $.extend(FreeswitchConfig.System.mFirewallRule, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: FreeswitchConfig.Site.Skin.tr.Tag,
        className: "FreeswitchConfig System mFirewallRule View " + FreeswitchConfig.Site.Skin.tr.Class,
        render: function() {
            this.$el.html('');
            if (this.model.get('TextDescription') != null) {
                this.$el.attr('class', this.$el.attr('class') + ' ' + this.model.get('TextDescription').replaceAll(' ', '_'));
            }
            if (this.model.get('ModuleName') != null) {
                this.$el.append(FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' ModuleName', Content: this.model.get('ModuleName') }));
            }
            var cstates = '';
            if (this.model.get('ConnectionStates') != null) {
                for (var x = 0; x < this.model.get('ConnectionStates').length; x++) {
                    cstates += this.model.get('ConnectionStates')[x] + ',';
                }
            }
            if (cstates.length > 0) {
                cstates = cstates.substring(0, cstates.length - 1);
            }
            this.$el.append([
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Interface', Content: (this.model.get('Interface') == null ? '&nbsp;' : this.model.get('Interface')) }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Protocol', Content: this.model.get('Protocol') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Source', Content: (this.model.get('SourceIP') == null ? '&nbsp;' : this.model.get('SourceIP')) }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' SourcePort', Content: (this.model.get('SourcePort') == null ? '&nbsp;' : this.model.get('SourcePort')) }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Destination', Content: (this.model.get('DestinationIP') == null ? '&nbsp;' : this.model.get('DestinationIP')) }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' DestinationPort', Content: (this.model.get('DestinationPort') == null ? '&nbsp;' : this.model.get('DestinationPort')) }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' ConnectionStates', Content: cstates }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' AdditionalInfo', Content: (this.model.get('ICMPType') == null ? '' : 'ICMPType: ' + this.model.get('ICMPType') + '<br/>')
                    + (this.model.get('AdditionalInfo') == null ? '' : this.model.get('AdditionalInfo').replaceAll('\n', '<br/>'))
                })
            ]);
            $(this.el).attr('name', this.model.id);
            this.trigger('render', this);
            return this;
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: FreeswitchConfig.Site.Skin.table.Tag,
        className: "FreeswitchConfig System mFirewallRule CollectionView " + FreeswitchConfig.Site.Skin.table.Class,
        initialize: function() {
            this.collection.on('reset', this.render, this); this.collection.on('sync',this.render,this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        attributes: $.extend({}, FreeswitchConfig.Site.Skin.table.Attributes, { cellspacing: 0, cellpadding: 0 }),
        render: function() {
            var el = this.$el;
            el.html('');
            var alt = false;
            if (this.collection.length == 0) {
                this.trigger('render', this);
            } else {
                var useModules = this.collection.at(0).get('ModuleName') != null;
                var thead = FreeswitchConfig.Site.Skin.thead.Create({ Class: this.className + ' head', Content: [
                    FreeswitchConfig.Site.Skin.tr.Create({ Content: [
                        FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Interface', Attributes: { rowspan: 2 }, Content: 'Interface' }),
                        FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Protocol', Attributes: { rowspan: 2 }, Content: 'Protocol' }),
                        FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Source', Attributes: { colspan: 2 }, Content: 'Source' }),
                        FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Destination', Attributes: { colspan: 2 }, Content: 'Destination' }),
                        FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' State', Attributes: { rowspan: 2 }, Content: 'State' }),
                        FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' AdditionalInformation', Attributes: { rowspan: 2 }, Content: 'Additional Information' })
                    ]
                    }),
                    FreeswitchConfig.Site.Skin.tr.Create({ Content: [
                        FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' IP', Content: 'IP' }),
                        FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Port', Content: 'Port' }),
                        FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' IP', Content: 'IP' }),
                        FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Port', Content: 'Port' })
                    ]
                    })
                ]
                });
                if (useModules) {
                    $(this.$el.find(FreeswitchConfig.Site.Skin.th.Tag + ':first')[0]).before(FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' ModuleName', Attributes: { rowspan: 2 }, Content: 'Module Name' }));
                }
                var el = FreeswitchConfig.Site.Skin.tbody.Create({ Class: this.className + ' body' });
                this.$el.append(el);
                var curModule = FreeswitchConfig.Site.Skin.td.Create();
                for (var x = 0; x < this.collection.length; x++) {
                    var vw = new FreeswitchConfig.System.mFirewallRule.View({ model: this.collection.at(x) });
                    if (x + 1 == this.collection.length) {
                        vw.on('render', function() { this.col.trigger('item_render', this.view); this.col.trigger('render', this.col); }, { col: this, view: vw });
                    } else {
                        vw.on('render', function() { this.col.trigger('item_render', this.view); }, { col: this, view: vw });
                    }
                    el.append(vw.$el);
                    vw.render();
                    if (useModules) {
                        if (curModule.text() != this.collection.at(x).get('ModuleName')) {
                            curModule = $(vw.$el.children()[0]);
                        } else {
                            curModule.attr('rowspan', curModule.attr('rowspan') + 1);
                            $(vw.$el.children()[0]).remove();
                        }
                    }
                }
            }
        }
    }),
    Chains: ['Input', 'Output', 'Forward'],
    GeneratePage: function(container) {
        container = $(container);
        var sel = $('<select style="margin-left:5px;"></select>');
        container.append(sel);
        for (var x = 0; x < this.Chains.length; x++) {
            sel.append('<option value="' + this.Chains[x] + '">' + this.Chains[x] + '</option>');
        }
        sel.append('<option value="FullScript">Show Full Script</option>');
        container.append('<div class="FirewallSettingsContainer"></div>');

        sel.bind('change',
        { sel: sel },
        function(event) {
            FreeswitchConfig.Site.Modals.ShowLoading();
            var cont = $(event.data.sel.next());
            cont.html('');
            if (event.data.sel.val() == 'FullScript') {
                var col = new FreeswitchConfig.System.mFirewallRule.Collection();
                col.on('reset', function() {
                    this.cont.append('iptables -F<br/>' +
                    'iptables -X<br/>' +
                    'iptables -t nat -F<br/>' +
                    'iptables -t nat -X<br/>' +
                    'iptables -t mangle -F<br/>' +
                    'iptables -t mangle -X<br/>' +
                    'iptables -P INPUT ACCEPT<br/>' +
                    'iptables -P FORWARD ACCEPT<br/>' +
                    'iptables -P OUTPUT ACCEPT<br/>');
                    for (var x = 0; x < this.col.length; x++) {
                        this.cont.append('iptables ' + this.col.at(x).get('AddRuleCommand') + '<br/>');
                    }
                    FreeswitchConfig.Site.Modals.HideLoading();
                }, { col: col, cont: cont });
                col.fetch();
            } else {
                var vwBase = new FreeswitchConfig.System.mFirewallRule.CollectionView(
                    { collection: FreeswitchConfig.System.mFirewallRule.LoadAllForChain(event.data.sel.val(), false) }
                );
                cont.append(vwBase.$el);
                vwBase.on('render', function(cont) {
                    var vwMod = new FreeswitchConfig.System.mFirewallRule.CollectionView(
                        { collection: FreeswitchConfig.System.mFirewallRule.LoadAllForChain(event.data.sel.val(), true) }
                    );
                    this.append(vwMod.$el);
                    vwMod.on('render', function() {
                        FreeswitchConfig.Site.Modals.HideLoading();
                    });
                    vwMod.collection.fetch();
                }, cont);
                vwBase.collection.fetch();
            }
        });
        sel.trigger('change');
    }
});