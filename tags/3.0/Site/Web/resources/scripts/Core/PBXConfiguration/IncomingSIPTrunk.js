CreateNameSpace('FreeswitchConfig.Trunks.IncomingSIPTrunk');

FreeswitchConfig.Trunks.IncomingSIPTrunk = $.extend(FreeswitchConfig.Trunks.IncomingSIPTrunk, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: "tr",
        className: "FreeswitchConfig Trunks IncomingSIPTrunk  View",
        render: function() {
            this.$el.html('');
            this.$el.attr('name', this.model.id);
            this.$el.append('<td className="' + this.className + ' Name">' + this.model.get('Name') + '</td>');
            this.$el.append('<td className="' + this.className + ' Password">' + this.model.get('Password') + '</td>');
            this.$el.append('<td className="' + this.className + ' InternalCallerID InternalCallerIDName">' + (this.model.get('InternalCallerIDName') == null ? '' : this.model.get('InternalCallerIDName'))
            + (this.model.get('InternalCallerID') == null ? '' : '(' + this.model.get('InternalCallerID') + ')') + '</td>');
            this.$el.append('<td className="' + this.className + ' ExternalCallerID ExternalCallerIDName">' + (this.model.get('ExternalCallerIDName') == null ? '' : this.model.get('ExternalCallerIDName'))
            + (this.model.get('ExternalCallerID') == null ? '' : '(' + this.model.get('ExternalCallerID') + ')') + '</td>');
            this.$el.append('<td className="' + this.className + ' MaxLines">' + this.model.get('MaxLines') + '</td>');
            this.$el.append('<td className="' + this.className + ' Number">' + this.model.get('Number') + '</td>');
            this.$el.append('<td className="' + this.className + ' Buttons"><img class="button edit phone_edit"/><img class="button delete phone_delete"/></td>');
            this.trigger('render', this);
            return this;
        },
        events: {
            'click .button.edit.phone_edit': 'editModel',
            'click .button.delete.phone_delete': 'deleteModel'
        },
        editModel: function() {
            FreeswitchConfig.Trunks.IncomingSIPTrunk.GenerateForm(null, this.model);
        },
        deleteModel: function() {
            this.model.destroy();
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: "table",
        className: "FreeswitchConfig Trunks IncomingSIPTrunk  CollectionView",
        initialize: function() {
            this.collection.on('reset', this.render, this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        render: function() {
            var el = this.$el;
            el.html('');
            var thead = $('<thead class="' + this.className + ' header"></thead>');
            el.append(thead);
            thead.append('<tr></tr>');
            thead = $(thead.children()[0]);
            thead.append('<th className="' + this.className + ' Name">Name</th>');
            thead.append('<th className="' + this.className + ' Password">Password</th>');
            thead.append('<th className="' + this.className + ' InternalCallerID InternalCallerIDName">InternalCallerID</th>');
            thead.append('<th className="' + this.className + ' ExternalCallerID ExternalCallerIDName">ExternalCallerID</th>');
            thead.append('<th className="' + this.className + ' MaxLines">MaxLines</th>');
            thead.append('<th className="' + this.className + ' Number">Number</th>');
            thead.append('<th className="' + this.className + ' Buttons">Actions</th>');
            el.append('<tbody></tbody>');
            el = $(el.children()[0]);
            if (this.collection.length == 0) {
                this.trigger('render', this);
            } else {
                var alt = false;
                for (var x = 0; x < this.collection.length; x++) {
                    var vw = new FreeswitchConfig.Trunks.IncomingSIPTrunk.View({ model: this.collection.at(x) });
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
        model = (model == undefined ? new FreeswitchConfig.Trunks.IncomingSIPTrunk.Model() : model);
        var frm = FreeswitchConfig.Site.Form.GenerateForm(
            null,
            [
                new FreeswitchConfig.Site.Form.FormInput('Name', 'text', null, null, 'Name:', null, model.get('Name')),
                new FreeswitchConfig.Site.Form.FormInput('Number', 'text', null, null, 'Number:', null, model.get('Number')),
                new FreeswitchConfig.Site.Form.FormInput('Password', 'text', null, null, 'Password:', null, model.get('Password')),
                new FreeswitchConfig.Site.Form.FormInput('MaxLines', 'text', null, null, 'Maximum Concurrent Phone Calls:', null, model.get('MaxLines')),
                new FreeswitchConfig.Site.Form.FormInput('InternalCallerIDName', 'text', null, null, 'Internal Caller ID Name:', null, model.get('InternalCallerIDName')),
                new FreeswitchConfig.Site.Form.FormInput('InternalCallerIDNumber', 'text', null, null, 'Internal Caller ID Number:', null, model.get('InternalCallerID')),
                new FreeswitchConfig.Site.Form.FormInput('ExternalCallerIDName', 'text', null, null, 'External Caller ID Name:', null, model.get('ExternalCallerIDName')),
                new FreeswitchConfig.Site.Form.FormInput('ExternalCallerIDNumber', 'text', null, null, 'External Caller ID Number:', null, model.get('ExternalCallerID'))
            ]
        );
        FreeswitchConfig.Site.Modals.ShowFormPanel(
            'Create New Incoming SIP Trunk',
            frm,
            [
                CreateButton(
                    'accept',
                    'Okay',
                    function(button, pars) {
                        if (pars.isCreate) {
                            FreeswitchConfig.Site.Modals.ShowSaving();
                        } else {
                            FreeswitchConfig.Site.Modals.ShowUpdating();
                        }
                        var canSubmit = FreeswitchConfig.Site.Validation.ValidateRequiredField(frm.find('input[name="Number"]')[0])
                        && FreeswitchConfig.Site.Validation.ValidateRequiredField(frm.find('input[name="Password"]')[0])
                        && FreeswitchConfig.Site.Validation.ValidateRequiredField(frm.find('input[name="Name"]')[0])
                        && (FreeswitchConfig.Site.Validation.ValidateRequiredField(frm.find('input[name="MaxLines"]')[0]) ? FreeswitchConfig.Site.Validation.ValidatePositiveIntegerField(frm.find('input[name="MaxLines"]')[0]) : false)
                        && ($(frm.find('input[name="InternalCallerIDNumber"]')[0]).val() != '' ? FreeswitchConfig.Site.Validation.ValidatePositiveIntegerField(frm.find('input[name="InternalCallerIDNumber"]')[0]) : true)
                        && ($(frm.find('input[name="ExternalCallerIDNumber"]')[0]).val() != '' ? FreeswitchConfig.Site.Validation.ValidatePositiveIntegerField(frm.find('input[name="ExternalCallerIDNumber"]')[0]) : true)
                        if (canSubmit) {
                            if (!pars.model.set({
                                Name: $(pars.frm.find('input[name="Name"]')[0]).val(),
                                Number: $(pars.frm.find('input[name="Number"]')[0]).val(),
                                Password: $(pars.frm.find('input[name="Password"]')[0]).val(),
                                Context: FreeswitchConfig.CurrentContext,
                                Domain: FreeswitchConfig.CurrentDomain,
                                MaxLines: $(pars.frm.find('input[name="MaxLines"]')[0]).val(),
                                InternalCallerIDName: ($(pars.frm.find('input[name="InternalCallerIDName"]')[0]).val() == '' ? null : $(pars.frm.find('input[name="InternalCallerIDName"]')[0]).val()),
                                InternalCallerID: ($(pars.frm.find('input[name="InternalCallerIDNumber"]')[0]).val() == '' ? null : $(pars.frm.find('input[name="InternalCallerIDNumber"]')[0]).val()),
                                ExternalCallerIDName: ($(pars.frm.find('input[name="ExternalCallerIDName"]')[0]).val() == '' ? null : $(pars.frm.find('input[name="ExternalCallerIDName"]')[0]).val()),
                                ExternalCallerID: ($(pars.frm.find('input[name="ExternalCallerIDNumber"]')[0]).val() == '' ? null : $(pars.frm.find('input[name="ExternalCallerIDNumber"]')[0]).val())
                            })) {
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
                                        alert('An error occured attempted to ' + (options.isCreate ? 'create' : 'update') + ' the incoming SIP trunk');
                                    },
                                    isCreate: pars.isCreate,
                                    collection: pars.collection
                                });
                            }
                        } else {
                            FreeswitchConfig.Site.Modals.HideSaving();
                            alert('Correct the highlighted fields and please try again.');
                        }
                    },
                    { collection: collection, frm: frm, isCreate: isCreate, model: model }
                ),
                CreateButton(
                    'cancel',
                    'Cancel',
                    function(tbl) {
                        FreeswitchConfig.Site.Modals.HideFormPanel();
                    }
                )
            ]
        );
        FreeswitchConfig.Site.Modals.HideLoading();
    },
    GeneratePage: function(container) {
        FreeswitchConfig.Site.Modals.ShowLoading();
        container = $(container);
        var col = FreeswitchConfig.Trunks.IncomingSIPTrunk.LoadAllAvailable();
        var vw = new FreeswitchConfig.Trunks.IncomingSIPTrunk.CollectionView({ collection: col, attributes: { cellspacing: 0, cellpadding: 0} });
        FreeswitchConfig.Site.setChangedDomain({ collection: col }, function(event) { FreeswitchConfig.Site.Modals.ShowLoading(); event.data.collection.fetch(); });
        vw.on('render', function() { FreeswitchConfig.Site.Modals.HideLoading(); });
        var butAdd = CreateButton(
            'connect',
            'Create New Incoming SIP Trunk',
            function(button, pars) {
                FreeswitchConfig.Trunks.IncomingSIPTrunk.GenerateForm(pars.collection);
            },
            { collection: col }
        );
        container.append(butAdd);
        container.append(vw.$el);
        col.fetch();
    }
});