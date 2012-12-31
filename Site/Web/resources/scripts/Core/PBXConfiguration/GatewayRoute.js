CreateNameSpace('FreeswitchConfig.Routes.GatewayRoute');

FreeswitchConfig.Routes.GatewayRoute = $.extend(FreeswitchConfig.Routes.GatewayRoute, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: "tr",
        className: "FreeswitchConfig Routing GatewayRoute View",
        render: function() {
            this.$el.html('');
            this.$el.append('<td className="' + this.className + ' Order" style="vertical-align:top;">' + this.model.get('Index') + '</td>');
            this.$el.append('<td className="' + this.className + ' Gateway" style="vertical-align:top;">' + this.model.get('OutGateway').get('Name') + '</td>');
            this.$el.append('<td className="' + this.className + ' Condition" style="vertical-align:top;">' + this.model.get('DestinationCondition').Value.replaceAll('\n','<br/>') + '</td>');
            this.$el.append('<td className="' + this.className + ' Buttons" style="vertical-align:top;"><img class="button edit cog_edit"/><img class="button delete cog_delete"/><img class="arrow_up_down" style="margin-right:5px;cursor:move;"/></td>');
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
        tagName: "table",
        className: "FreeswitchConfig Routing GatewayRoute CollectionView",
        initialize: function() {
            this.collection.on('reset', this.render, this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        attributes: { cellspacing: 0, cellpadding: 0 },
        render: function() {
            var el = this.$el;
            el.html('');
            var thead = $('<thead class="' + this.className + ' header"></thead>');
            el.append(thead);
            thead.append('<tr></tr>');
            thead = $(thead.children()[0]);
            thead.append('<th className="' + this.className + ' Order">Order</th>');
            thead.append('<th className="' + this.className + ' Gateway">Gateway</th>');
            thead.append('<th className="' + this.className + ' Condition">Condition</th>');
            thead.append('<th className="' + this.className + ' Buttons">Actions</th>');
            el.append('<tbody></tbody>');
            el = $(el.children()[0]);
            if (this.collection.length == 0) {
                this.trigger('render', this);
            } else {
                var alt = false;
                for (var x = 0; x < this.collection.length; x++) {
                    var vw = new FreeswitchConfig.Routes.GatewayRoute.View({ model: this.collection.at(x) });
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
                                    attrs[inp.attr('name')]=inp.val();
                                    break;
                                case 'OutGateway':
                                    canSubmit = canSubmit && FreeswitchConfig.Site.Validation.ValidateRequiredField(inp);
                                    attrs[inp.attr('name')]=new FreeswitchConfig.Trunks.Gateway.Model({id:inp.val()});
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
            $(view.$el.find('tbody')[0]).sortable({
                containment: $(view.$el.find('tbody')[0]),
                items: '> tr',
                axis: 'y',
                handle: 'img:last',
                placeholder: 'ui-state-highlight',
                tolerance: 'pointer',
                change: function(event, ui) {
                    ui.placeholder.html('<td colspan="5" style="height: 1.5em;line-height: 1.2em;">&nbsp;</td>');
                }
            });
            $(view.$el.find('tbody')[0]).bind("sortupdate",
            { view: view },
            function(event, ui) {
                FreeswitchConfig.Site.Modals.ShowUpdating();
                var tbl = $(event.data.view.$el.find('tbody')[0]);
                var col = event.data.view.collection;
                var trs = tbl.find('tr');
                var alt = false;
                for (var x = 0; x < trs.length; x++) {
                    var obj = col.get($(trs[x]).attr('name'));
                    obj.set({ Index: x });
                    obj.syncSave();
                    $(trs[x]).attr('class', FreeswitchConfig.Routes.GatewayRoute.className + (alt ? ' Alt' : ''));
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