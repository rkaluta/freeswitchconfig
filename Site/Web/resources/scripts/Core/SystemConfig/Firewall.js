CreateNameSpace('FreeswitchConfig.System.mFirewallRule');
FreeswitchConfig.System.mFirewallRule = _.extend(FreeswitchConfig.System.mFirewallRule, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: "tr",
        className: "FreeswitchConfig System mFirewallRule View",
        render: function() {
            if (this.model.get('TextDescription') != null) {
                this.$el.attr('class', this.$el.attr('class') + ' ' + this.model.get('TextDescription').replaceAll(' ', '_'));
            }
            if (this.model.get('ModuleName') != null) {
                this.$el.append('<td class="' + this.className + ' ModuleName">' + this.model.get('ModuleName') + '</td>');
            }
            this.$el.append('<td class="' + this.className + ' Interface">' + (this.model.get('Interface') == null ? '&nbsp;' : this.model.get('Interface')) + '</td>');
            this.$el.append('<td class="' + this.className + ' Protocol">' + this.model.get('Protocol') + '</td>');
            this.$el.append('<td class="' + this.className + ' Source">' + (this.model.get('SourceIP') == null ? '&nbsp;' : this.model.get('SourceIP')) + '</td>');
            this.$el.append('<td class="' + this.className + ' SourcePort">' + (this.model.get('SourcePort') == null ? '&nbsp;' : this.model.get('SourcePort')) + '</td>');
            this.$el.append('<td class="' + this.className + ' Destination">' + (this.model.get('DestinationIP') == null ? '&nbsp;' : this.model.get('DestinationIP')) + '</td>');
            this.$el.append('<td class="' + this.className + ' DestinationPort">' + (this.model.get('DestinationPort') == null ? '&nbsp;' : this.model.get('SourcePort')) + '</td>');
            var cstates = '';
            if (this.model.get('ConnectionStates') != null) {
                for (var x = 0; x < this.model.get('ConnectionStates').length; x++) {
                    cstates += this.model.get('ConnectionStates')[x] + ',';
                }
            }
            if (cstates.length > 0) {
                cstates = cstates.substring(0, cstates.length - 1);
            }
            this.$el.append('<td class="' + this.className + ' ConnectionStates">' + cstates + '</td>');
            this.$el.append('<td class="' + this.className + ' ICMPType AdditionalInfo">'
            + (this.model.get('ICMPType') == null ? '' : 'ICMPType: ' + this.model.get('ICMPType') + '<br/>')
            + (this.model.get('AdditionalInfo') == null ? '' : this.model.get('AdditionalInfo').replaceAll('\n', '<br/>'))
            + '</td>');
            $(this.el).attr('name', this.model.id);
            this.trigger('render', this);
            return this;
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: "table",
        className: "FreeswitchConfig System mFirewallRule CollectionView",
        initialize: function() {
            this.collection.on('reset', this.render, this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        attributes: {
            cellspacing: 0,
            cellpadding: 0
        },
        render: function() {
            var el = this.$el;
            el.html('');
            var alt = false;
            if (this.collection.length == 0) {
                this.trigger('render', this);
            } else {
                var useModules = this.collection.at(0).get('ModuleName') != null;
                var thead = $('<thead class="' + this.className + ' head"></thead>');
                var tr = $('<tr></tr>');
                thead.append(tr);
                el.append(thead);
                if (useModules) {
                    tr.append('<th class="' + this.className + ' ModuleName" rowspan="2">Module Name</th>');
                }
                tr.append('<th class="' + this.className + ' Interface" rowspan="2">Interface</th>' +
		    '<th class="' + this.className + ' Protocol" rowspan="2">Protocol</th>' +
		    '<th class="' + this.className + ' Source" colspan="2">Source</th>' +
		    '<th class="' + this.className + ' Destination" colspan="2">Destination</th>' +
		    '<th class="' + this.className + ' State" rowspan="2">State</th>' +
		    '<th class="' + this.className + ' AdditionalInformation" rowspan="2">Additional Information</th>');
                thead.append('<tr>'
		    + '<th class="' + this.className + ' IP">IP</th>'
		    + '<th class="' + this.className + ' Port">Port</th>'
		    + '<th class="' + this.className + ' IP">IP</th>'
		    + '<th class="' + this.className + ' Port">Port</th>'
		    + '</tr>');
                el = $('<tbody class="' + this.className + ' body"></tbody>');
                this.$el.append(el);
                var curModule = $('<td></td>');
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
                        this.cont.append('iptables '+this.col.at(x).get('AddRuleCommand') + '<br/>');
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