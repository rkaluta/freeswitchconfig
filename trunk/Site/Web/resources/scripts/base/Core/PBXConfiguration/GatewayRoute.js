CreateNameSpace('FreeswitchConfig.Routes.GatewayRoute');

FreeswitchConfig.Routes.GatewayRoute = $.extend(FreeswitchConfig.Routes.GatewayRoute, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: FreeswitchConfig.Site.Skin.tr.Tag,
        attributes: FreeswitchConfig.Site.Skin.tr.Attributes,
        className: "FreeswitchConfig Routing GatewayRoute View ",
        render: function() {
            this.$el.html('');
            this.$el.append([
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Order', Attributes: { style: 'vertical-align:top;' }, Content: this.model.get('Index') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Gateway', Attributes: { style: 'vertical-align:top;' }, Content: this.model.get('OutGateway').get('Nane') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Condition', Attributes: { style: 'vertical-align:top;' }, Content: this.model.get('DestinationCondition').Value.replaceAll('\n', '<br/>') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Buttons', Attributes: { style: 'vertical-align:top;' }, Content: [
                    FreeswitchConfig.Site.Skin.img.Create({ Class: 'button edit cog_edit' }),
                    FreeswitchConfig.Site.Skin.img.Create({ Class: 'button delete cog_delete' }),
                    FreeswitchConfig.Site.Skin.img.Create({ Class: 'arrow_up_down', Attributes: { style: 'margin-right:5px;cursor:move;'} })
                ]
                })
            ]);
            $(this.el).attr('name', this.model.id);
            this.trigger('render', this);
            return this;
        },
        events: {
            'click .button.delete.cog_delete': 'deleteModel',
            'click .button.edit.cog_edit': 'editModel'
        },
        deleteModel: function() {
            FreeswitchConfig.Site.Modals.ShowUpdating();
            this.model.destroy({
                success: function() { FreeswitchConfig.Site.Modals.HideUpdating(); },
                error: function() { alert('An error occured attempting to delete the Intercom.'); FreeswitchConfig.Site.Modals.HideUpdating(); }
            });
        },
        editModel: function() {
            FreeswitchConfig.Routes.GatewayRoute.GenerateForm(null, this.model);
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: FreeswitchConfig.Site.Skin.table.Tag,
        className: "FreeswitchConfig Routing GatewayRoute CollectionView " + FreeswitchConfig.Site.Skin.table.Class,
        initialize: function() {
            this.collection.on('reset', this.render, this); this.collection.on('sync',this.render,this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        attributes: $.extend({}, FreeswitchConfig.Site.Skin.table.Attributes, { cellspacing: 0, cellpadding: 0 }),
        render: function() {
            if (this.$el.find(FreeswitchConfig.Site.Skin.thead.Tag).length == 0) {
                this.$el.append([
                    FreeswitchConfig.Site.Skin.thead.Create({Class:this.className+' header',Content:[
                        FreeswitchConfig.Site.Skin.tr.Create({ Content: [
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Order', Content: 'Order' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Gateway', Content: 'Gateway' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Condition', Content: 'Condition' }),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Buttons',Content:'Actions'})
                        ]})
                    ]}),
                    FreeswitchConfig.Site.Skin.tbody.Create()
                ]);    
            }
            var el = $(this.$el.find(FreeswitchConfig.Site.Skin.tbody.Tag)[0]);
            el.html('');
            if (this.collection.length == 0) {
                this.trigger('render', this);
            } else {
                var alt = false;
                for (var x = 0; x < this.collection.length; x++) {
                    var vw = new FreeswitchConfig.Routes.GatewayRoute.View({ model: this.collection.at(x) });
                    if (alt) {
                        vw.$el.addClas(FreeswitchConfig.Site.Skin.tr.AltClass);
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
    }),
    GenerateForm: function(collection, model) {
        FreeswitchConfig.Site.Modals.ShowLoading();
        var isCreate = model == undefined;
        model = (model == undefined ? new FreeswitchConfig.Routes.GatewayRoute.Model() : model);
        var frm = FreeswitchConfig.Site.Form.GenerateForm(
            null,
            [
               new FreeswitchConfig.Site.Form.FormInput('OutGateway', 'select', FreeswitchConfig.Trunks.Gateway.SelectList(), null, 'Gateway:', null, (model.attributes['OutGateway'] == null ? null : model.attributes['OutGateway'].id)),
               new FreeswitchConfig.Site.Form.FormInput('DestinationCondition', 'textarea', null, null, 'Condition:', { rows: '5', cols: '20' }, (model.get('DestinationCondition') == null ? null : model.get('DestinationCondition').Value))
            ]
         );
        AttachNPANXXHelpToInput(frm.find('textarea[name="DestinationCondition"]')[0]);
        FreeswitchConfig.Site.Modals.ShowFormPanel(
            (isCreate ? 'Create New Gateway Route' : 'Edit Gateway Route'),
            frm,
            [
                CreateButton(
                    'add',
                    'Okay',
                    function(button, pars) {
                        FreeswitchConfig.Site.Modals.ShowUpdating();
                        var attrs = { RouteContext: FreeswitchConfig.CurrentContext, Index: (pars.collection == null ? pars.model.get('Index') : pars.collection.length + 1) };
                        var inps = pars.frm.find('textarea,select');
                        var canSubmit = true;
                        for (var x = 0; x < inps.length; x++) {
                            var inp = $(inps[x]);
                            switch (inp.attr('name')) {
                                case 'DestinationCondition':
                                    canSubmit = canSubmit && (FreeswitchConfig.Site.Validation.ValidateRequiredField(inp) ? FreeswitchConfig.Site.Validation.ValidateNPANXXField(inp) : false);
                                    attrs[inp.attr('name')] = inp.val();
                                    break;
                                case 'OutGateway':
                                    canSubmit = canSubmit && FreeswitchConfig.Site.Validation.ValidateRequiredField(inp);
                                    attrs[inp.attr('name')] = new FreeswitchConfig.Trunks.Gateway.Model({ id: inp.val() });
                                    break;
                            }
                        }
                        if (canSubmit) {
                            if (!pars.model.set(attrs)) {
                                var msg = 'Please correct the following error(s)/field(s):<ul>';
                                for (var x = 0; x < pars.model.errors.length; x++) {
                                    msg += '<li>' + (pars.model.errors[x].error == '' ? pars.model.errors[x].field : pars.model.errors[x].error) + '</li>';
                                }
                                FreeswitchConfig.Site.Modals.HideUpdating();
                                alert(msg + '</ul>');
                            } else {
                                pars.model.save(pars.model.attributes, {
                                    success: function(model, response, options) {
                                        if (options.isCreate) {
                                            options.collection.push(pars.model);
                                        }
                                        FreeswitchConfig.Site.Modals.HideFormPanel();
                                        FreeswitchConfig.Site.Modals.HideUpdating();
                                    },
                                    error: function(model, response, options) {
                                        FreeswitchConfig.Site.Modals.HideUpdating();
                                        alert('An error occured attempting to ' + (options.isCreate ? 'create' : 'update') + ' the gateway route');
                                    },
                                    isCreate: pars.isCreate,
                                    collection: pars.collection
                                });
                            }
                        } else {
                            FreeswitchConfig.Site.Modals.HideUpdating();
                            alert('Please correct the highlighted areas in order to create a Gateway Route.');
                        }
                    },
                    { frm: frm, model: model, isCreate: isCreate, collection: collection }
                ),
                CreateButton(
                    'cancel',
                    'Cancel',
                    function(button) {
                        FreeswitchConfig.Site.Modals.HideFormPanel();
                    }
                )
            ]
         );
        FreeswitchConfig.Site.Modals.HideLoading();
    },
    GeneratePage: function(container) {
        container = $(container);
        FreeswitchConfig.Site.Modals.ShowLoading();
        var col = new FreeswitchConfig.Routes.GatewayRoute.Collection();
        var view = new FreeswitchConfig.Routes.GatewayRoute.CollectionView({ collection: col });
        view.on('render', function(view) {
            $(view.$el.find(FreeswitchConfig.Site.Skin.tbody.Tag)[0]).sortable({
                containment: $(view.$el.find(FreeswitchConfig.Site.Skin.tbody.Tag)[0]),
                items: '> '+FreeswitchConfig.Site.Skin.tr.Tag,
                axis: 'y',
                handle: 'img:last',
                placeholder: 'ui-state-highlight',
                tolerance: 'pointer',
                change: function(event, ui) {
                    ui.placeholder.html(FreeswitchConfig.Site.Skin.td.Create({Attributes:{colspan:'5',style:'height: 1.5em;line-height: 1.2em;'}}));
                }
            });
            $(view.$el.find(FreeswitchConfig.Site.Skin.tbody.Tag)[0]).bind("sortupdate",
            { view: view },
            function(event, ui) {
                FreeswitchConfig.Site.Modals.ShowUpdating();
                var tbl = $(event.data.view.$el.find(FreeswitchConfig.Site.Skin.tbody.Tag)[0]);
                var col = event.data.view.collection;
                var trs = tbl.find(FreeswitchConfig.Site.Skin.tr.Tag);
                var alt = false;
                for (var x = 0; x < trs.length; x++) {
                    var obj = col.get($(trs[x]).attr('name'));
                    obj.set({ Index: x });
                    obj.syncSave();
                    $(trs[x]).attr('class', FreeswitchConfig.Routes.GatewayRoute.className + (alt ? ' '+FreeswitchConfig.Site.Skin.tr.AltClass : ''));
                    alt = !alt;
                }
                FreeswitchConfig.Site.Modals.HideUpdating();
            });
            FreeswitchConfig.Site.Modals.HideLoading();
        });
        var butAdd = CreateButton('cog_add',
            'Add New Gateway Route',
            function(button, pars) {
                FreeswitchConfig.Routes.GatewayRoute.GenerateForm(pars.collection);
            },
            { collection: col }
        );
        container.append(butAdd);
        container.append(view.$el);
        col.fetch();
    }
});