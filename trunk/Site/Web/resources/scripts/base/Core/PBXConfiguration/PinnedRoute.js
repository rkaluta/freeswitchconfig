CreateNameSpace('FreeswitchConfig.Routes.PinnedRoute');

FreeswitchConfig.Routes.PinnedRoute = $.extend(FreeswitchConfig.Route, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: "tr",
        className: "FreeswitchConfig Routes PinnedRoute View",
        render: function() {
            this.$el.html('');
            this.$el.append('<td class="' + this.className + ' DestinationCondition" style="vertical-align:top">' + this.model.get('DestinationCondition').Value.replaceAll('\n', '<br/>') + '</td>');
            this.$el.append('<td class="' + this.className + ' PinFile" style="vertical-align:top">' + this.model.attributes['PinFile'].id.split('@')[0] + '</td>');
            this.$el.append('<td class="' + this.className + ' RouteContext" style="vertical-align:top">' + this.model.attributes['RouteContext'].id + '</td>');
            this.$el.append('<td class="' + this.className + ' Buttons" style="vertical-align:top;"><img class="button edit cog_edit"/><img class="button delete cog_delete"/></td>');
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
            FreeswitchConfig.Routes.PinnedRoute.GenerateForm(null, this.model);
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: "table",
        className: "FreeswitchConfig Routes PinnedRoute CollectionView",
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
            thead.append('<th className="' + this.className + ' DestinationCondition">Condition</th>');
            thead.append('<th className="' + this.className + ' PinFile">Pin File</th>');
            thead.append('<th className="' + this.className + ' RouteContext">Context</th>');
            thead.append('<th className="' + this.className + ' Buttons">Actions</th>');
            el.append('<tbody></tbody>');
            el = $(el.children()[0]);
            if (this.collection.length == 0) {
                this.trigger('render', this);
            } else {
                var alt = false;
                for (var x = 0; x < this.collection.length; x++) {
                    var vw = new FreeswitchConfig.Routes.PinnedRoute.View({ model: this.collection.at(x) });
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
        model = (model == undefined ? new FreeswitchConfig.Routes.PinnedRoute.Model() : model);
        var frm = FreeswitchConfig.Site.Form.GenerateForm(
            null,
            [
                new FreeswitchConfig.Site.Form.FormInput('Name', 'text', null, null, 'Name:', null, model.get('Name')),
                new FreeswitchConfig.Site.Form.FormInput('DestinationCondition', 'textarea', null, null, 'Condition:', { rows: '5', cols: '25' }, (isCreate ? null : model.get('DestinationCondition').Value)),
                new FreeswitchConfig.Site.Form.FormInput('PinFile', 'select', FreeswitchConfig.PBX.PinSet.SelectList(), null, 'Pin File:', null, (isCreate ? null : model.attributes['PinFile'].id))
            ]
        );
        AttachNPANXXHelpToInput(frm.find('textarea[name="DestinationCondition"]'));
        FreeswitchConfig.Site.Modals.ShowFormPanel(
            (isCreate ? 'Create New Pinned Route' : 'Edit Pinned Route'),
            frm,
            [
                CreateButton(
                    'accept',
                    (isCreate ? 'Create' : 'Update'),
                    function(button, pars) {
                        if (isCreate) {
                            FreeswitchConfig.Site.Modals.ShowSaving();
                        } else {
                            FreeswitchConfig.Site.Modals.ShowUpdating();
                        }
                        var name = $(pars.frm.find('input[name="Name"]')[0]);
                        var cond = $(pars.frm.find('textarea[name="DestinationCondition"]')[0]);
                        var canSubmit = FreeswitchConfig.Site.Validation.ValidateRequiredField(name)
                        && (FreeswitchConfig.Site.Validation.ValidateRequiredField(cond) ?
                            FreeswitchConfig.Site.Validation.ValidateNPANXXField(cond)
                         : false);
                        if (canSubmit) {
                            if(!pars.model.set({ Name: name.val(), DestinationCondition: cond.val(), RouteContext: FreeswitchConfig.CurrentContext, PinFile: new FreeswitchConfig.PBX.PinSet.Model({ id: $(pars.frm.find('select[name="PinFile"]')[0]).val() }) })) {
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
                                        alert('An error occured attempting to ' + (options.isCreate ? 'create' : 'update') + ' the pinned route.');
                                    },
                                    isCreate: pars.isCreate,
                                    collection: pars.collection
                                });
                            }
                        } else {
                            FreeswitchConfig.Site.Modals.HideUpdating();
                            alert('You must correct the indicated fields in order to ' + (isCreate ? 'create' : 'update') + ' the pinned route.');
                        }
                    },
                    { collection: collection, model: model, isCreate: isCreate, frm: frm }
                ),
                CreateButton(
                    'cancel',
                    'Cancel',
                    function() {
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
        var col = new FreeswitchConfig.Routes.PinnedRoute.Collection();
        var view = new FreeswitchConfig.Routes.PinnedRoute.CollectionView({ collection: col });
        FreeswitchConfig.Site.setChangedDomain({ collection: col }, function(event) { FreeswitchConfig.Site.Modals.ShowLoading(); event.data.collection.fetch(); });
        view.on('render', function() { FreeswitchConfig.Site.Modals.HideLoading(); });
        var butAdd = CreateButton(
                    'cog_add',
                    'Add New Pinned Route',
                    function(button, pars) {
                        FreeswitchConfig.Routes.PinnedRoute.GenerateForm(pars.collection);
                    },
                    { tbl: view.$el }
                );
        container.append(butAdd);
        container.append(view.$el);
        col.fetch();
    }
});