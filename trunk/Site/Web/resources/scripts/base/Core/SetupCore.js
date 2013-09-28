CreateNameSpace('FreeswitchConfig.Core.Context');
CreateNameSpace('FreeswitchConfig.Core.Domain');
CreateNameSpace('FreeswitchConfig.Core.SipProfile');

FreeswitchConfig.Core.Context = $.extend(
FreeswitchConfig.Core.Context,
{
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: FreeswitchConfig.Site.Skin.tr.Tag,
        render: function() {
            this.$el.empty();
            this.$el.append([
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Name', Content: this.model.get('Name') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Description', Content: this.model.get('Description') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Type', Content: this.model.get('Type') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' buttons', Content: [
                    FreeswitchConfig.Site.Skin.span.Create({ Class: this.className + ' button pencil' }),
                    FreeswitchConfig.Site.Skin.span.Create({ Class: this.className + ' button cancel' })
                ]
                })
            ]);
            $(this.el).attr('name', this.model.id);
            this.trigger('render', this);
            return this;
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: FreeswitchConfig.Site.Skin.table.Tag,
        className: "FreeswitchConfig Core Context CollectionView " + FreeswitchConfig.Site.Skin.table.Class,
        initialize: function() {
            this.collection.on('reset', this.render, this); this.collection.on('sync',this.render,this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        attributes: { cellspacing: '0', cellpadding: '0' },
        render: function() {
            this.$el.empty();
            this.$el.append(FreeswitchConfig.Site.Skin.tr.Create({ Content: [
                FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Name', Content: 'Name' }),
                FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Description', Content: 'Description' }),
                FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Type', Content: 'Type' }),
                FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Buttons', Content: 'Actions' })
            ]}));
            el = FreeswitchConfig.Site.Skin.tbody.Create();
            this.$el.append(el);
            if (this.collection.length == 0) {
                this.trigger('render', this);
            } else {
                var alt = false;
                for (var x = 0; x < this.collection.length; x++) {
                    var vw = new FreeswitchConfig.Core.Context.View({ model: this.collection.at(x),
                        className: "FreeSwitchConfig Core Context View " + (alt ? FreeswitchConfig.Site.Skin.tr.AltClass : FreeswitchConfig.Site.Skin.tr.Class)
                    });
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
    })
});

FreeswitchConfig.Core.SipProfile = $.extend(FreeswitchConfig.Core.SipProfile, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: FreeswitchConfig.Site.Skin.tr.Tag,
        className: "FreeSwitchConfig Core SipProfile View ",
        render: function() {
            this.$el.empty();
            this.$el.append([
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Name', Content: this.model.get('Name') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Context', Content: FreeswitchConfig.Site.Skin.span.Create({ Class: 'Org Reddragonit FreeSwitchConfig DataCore DB Core Context Name', Content: this.model.get('Context').get('Name') }) }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' SIPPort', Content: this.model.get('SIPPort') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' SIPInterface', Content: FreeswitchConfig.Site.Skin.span.Create({ Class: 'FreeSwitchConfig System sNetworkCard Name', Content: this.model.get('SIPInterface').get('Name') }) }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' RTPInterface', Content: FreeswitchConfig.Site.Skin.span.Create({ Class: 'FreeSwitchConfig System sNetworkCard Name', Content: this.model.get('RTPInterface').get('Name') }) }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' buttons', Content: [
                    FreeswitchConfig.Site.Skin.span.Create({ Class: this.className + ' button pencil' }),
                    FreeswitchConfig.Site.Skin.span.Create({ Class: this.className + ' button cancel' })
                ]
                })
            ]);
            this.trigger('render', this);
            return this;
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: FreeswitchConfig.Site.Skin.table.Tag,
        className: "FreeswitchConfig Core SipProfile CollectionView " + FreeswitchConfig.Site.Skin.table.Class,
        initialize: function() {
            this.collection.on('reset', this.render, this); this.collection.on('sync', this.render, this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        attributes: { cellspacing: '0', cellpadding: '0' },
        render: function() {
            this.$el.empty();
            this.$el.append(FreeswitchConfig.Site.Skin.thead.Create({ Class: this.className + ' header', Content: [
                FreeswitchConfig.Site.Skin.tr.Create({ Content: [
                    FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Name', Content: 'Name', Attributes: { rowspan: 2} }),
                    FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Context', Content: 'Context', Attributes: { rowspan: 2} }),
                    FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' SIP', Content: 'SIP', Attributes: { colspan: 2} }),
                    FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' RTP', Content: 'RTP' }),
                    FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Buttons', Content: 'Actions', Attributes: { rowspan: 2} })
                ]
                }),
                FreeswitchConfig.Site.Skin.tr.Create({ Content: [
                    FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' SIPPort', Content: 'Port' }),
                    FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' SIPInterface', Content: 'Iface' }),
                    FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' RTPInterface', Content: 'Iface' })
                ]
                })
            ]
            }));
            var el = FreeswitchConfig.Site.Skin.tbody.Create();
            this.$el.append(el);
            if (this.collection.length == 0) {
                this.trigger('render', this);
            } else {
                var alt = false;
                for (var x = 0; x < this.collection.length; x++) {
                    var vw = new FreeswitchConfig.Core.SipProfile.View({ model: this.collection.at(x) });
                    if (alt) {
                        vw.$el.attr('class', vw.$el.attr('class') + ' Alt');
                    }
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
    })
});

FreeswitchConfig.Core.Domain = $.extend(FreeswitchConfig.Core.Domain, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: FreeswitchConfig.Site.Skin.tr.Tag,
        className: "FreeSwitchConfig Core Domain View " + FreeswitchConfig.Site.Skin.tr.Class,
        render: function() {
            this.$el.empty();
            this.$el.append([
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Name', Content: this.model.get('Name') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' VoicemailTimeout', Content: this.model.get('VoicemailTimeout') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' InternalProfile', Content: FreeswitchConfig.Site.Skin.span.Create({ Class: 'FreeSwitchConfig Core SipProfile Name', Content: this.model.get('InternalProfile').get('Name') }) }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' ExternalProfile', Content: FreeswitchConfig.Site.Skin.span.Create({ Class: 'FreeSwitchConfig Core SipProfile Name', Content: this.model.get('ExternalProfile').get('Name') }) }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' buttons', Content: [
                    FreeswitchConfig.Site.Skin.span.Create({ Class: this.className + ' button pencil' }),
                    FreeswitchConfig.Site.Skin.span.Create({ Class: this.className + ' button cancel' })
                ]
                })
            ]);
            this.trigger('render', this);
            return this;
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: FreeswitchConfig.Site.Skin.table.Tag,
        className: "FreeswitchConfig Core Domain CollectionView " + FreeswitchConfig.Site.Skin.table.Class,
        initialize: function() {
            this.collection.on('reset', this.render, this); this.collection.on('sync', this.render, this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        attributes: { cellspacing: '0', cellpadding: '0' },
        render: function() {
            this.$el.empty();
            this.$el.append(FreeswitchConfig.Site.Skin.thead.Create({ Class: this.className + ' header', Content: [
                FreeswitchConfig.Site.Skin.tr.Create({ Content: [
                    FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Name', Content: 'Name', Attributes: { rowspan: 2} }),
                    FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' VoicemailTimeout', Content: 'Voicemail Timeout', Attributes: { rowspan: 2} }),
                    FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Profile', Content: 'Profile', Attributes: { colspan: 2} }),
                    FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Buttons', Content: 'Actions', Attributes: { rowspan: 2} })
                ]
                }),
                FreeswitchConfig.Site.Skin.tr.Create({ Content: [
                    FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' InternalProfile', Content: 'Internal' }),
                    FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' ExternalProfile', Content: 'External' })
                ]
                })
            ]
            }));
            var el = FreeswitchConfig.Site.Skin.tbody.Create();
            this.$el.append(el);
            if (this.collection.length == 0) {
                this.trigger('render', this);
            } else {
                var alt = false;
                for (var x = 0; x < this.collection.length; x++) {
                    var vw = new FreeswitchConfig.Core.Domain.View({ model: this.collection.at(x) });
                    if (alt) {
                        vw.$el.attr('class', vw.$el.attr('class') + ' Alt');
                    }
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
    })
})