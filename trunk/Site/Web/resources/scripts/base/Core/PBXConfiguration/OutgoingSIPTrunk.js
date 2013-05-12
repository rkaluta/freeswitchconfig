CreateNameSpace('FreeswitchConfig.Trunks.OutgoingSIPTrunk');

FreeswitchConfig.Trunks.OutgoingSIPTrunk = $.extend(FreeswitchConfig.Trunks.OutgoingSIPTrunk, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: "tr",
        className: "FreeswitchConfig Routing OutgoingSIPTrunk View",
        render: function() {
            this.$el.html('');
            this.$el.append('<td class="' + this.className + ' Name" style="vertical-align:top">' + this.model.get('Name') + '</td>');
            this.$el.append('<td class="' + this.className + ' Authentication" style="vertical-align:top">' + this.model.get('UserName') + '@' + this.model.get('Realm') + ':' + this.model.get('Password') + '</td>');
            this.$el.append('<td class="' + this.className + ' RegistrationType" style="vertical-align:top">' + this.model.get('RegistrationType') + '</td>');
            this.$el.append('<td class="' + this.className + ' Register" style="vertical-align:top">' + (this.model.get('Register') ? '<img class="tick"/>' : '') + '</td>');
            this.$el.append('<td class="' + this.className + ' Profile" style="vertical-align:top">' + this.model.attributes['Profile'].id + '</td>');
            this.$el.append('<td class="' + this.className + ' PingInterval" style="vertical-align:top">' + this.model.get('PingInterval') + ' seconds</td>');
            this.$el.append('<td class="' + this.className + ' Buttons" style="vertical-align:top;"><img class="button edit pencil"/><img class="button delete cancel"/></td>');
            $(this.el).attr('name', this.model.id);
            this.trigger('render', this);
            return this;
        },
        events: {
            'click .button.delete.cancel': 'deleteModel',
            'click .button.edit.pencil': 'editModel'
        },
        deleteModel: function() {
            FreeswitchConfig.Site.Modals.ShowUpdating();
            this.model.destroy({
                success: function() { FreeswitchConfig.Site.Modals.HideUpdating(); },
                error: function() { alert('An error occured attempting to delete the Intercom.'); FreeswitchConfig.Site.Modals.HideUpdating(); }
            });
        },
        editModel: function() {
            FreeswitchConfig.Trunks.OutgoingSIPTrunk.GenerateForm(null, this.model);
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: "table",
        className: "FreeswitchConfig Routing OutgoingSIPTrunk CollectionView",
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
            thead.append('<th className="' + this.className + ' Name">Name</th>');
            thead.append('<th className="' + this.className + ' Authentication">Authentication</th>');
            thead.append('<th className="' + this.className + ' RegistrationType">Protocol</th>');
            thead.append('<th className="' + this.className + ' Register">Register</th>');
            thead.append('<th className="' + this.className + ' Profile">Profile</th>');
            thead.append('<th className="' + this.className + ' PingInterval">Ping Interval</th>');
            thead.append('<th className="' + this.className + ' Buttons">Actions</th>');
            el.append('<tbody></tbody>');
            el = $(el.children()[0]);
            if (this.collection.length == 0) {
                this.trigger('render', this);
            } else {
                var alt = false;
                for (var x = 0; x < this.collection.length; x++) {
                    var vw = new FreeswitchConfig.Trunks.OutgoingSIPTrunk.View({ model: this.collection.at(x) });
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
        var isCreate = model == undefined;
        model = (model == undefined ? new FreeswitchConfig.Trunks.OutgoingSIPTrunk.Model() : model);
        var frm = FreeswitchConfig.Site.Form.GenerateForm(
            null,
            [
                new FreeswitchConfig.Site.Form.FormInput('Name', 'text', null, null, 'Name:', null, model.get('Name')),
                new FreeswitchConfig.Site.Form.FormInput('UserName', 'text', null, null, 'User Name:', null, model.get('UserName')),
                new FreeswitchConfig.Site.Form.FormInput('Realm', 'text', null, null, 'Domain Name:', null, model.get('Realm')),
                new FreeswitchConfig.Site.Form.FormInput('Password', 'text', null, null, 'Password:', null, model.get('Password')),
                new FreeswitchConfig.Site.Form.FormInput('RegistrationType', 'select', [
                    new FreeswitchConfig.Site.Form.SelectValue('tcp'),
                    new FreeswitchConfig.Site.Form.SelectValue('udp')
                ], null, 'Registration Protocol:', null, model.get('RegistrationType')),
                new FreeswitchConfig.Site.Form.FormInput('Register', 'checkbox', null, null, 'Register:', null, model.get('Register')),
                new FreeswitchConfig.Site.Form.FormInput('Profile', 'select', FreeswitchConfig.Core.SipProfile.SelectList(), null, 'SIP Profile:', null, (model.attributes['Profile'] != null ? model.attributes['Profile'].id : null)),
                new FreeswitchConfig.Site.Form.FormInput('PingInterval', 'text', null, null, 'Ping Interval (in seconds):', null, model.get('PingInterval')),
                new FreeswitchConfig.Site.Form.FormInput('RetrySeconds', 'text', null, null, 'Retry Interval (in seconds):', null, model.get('RetrySeconds')),
                new FreeswitchConfig.Site.Form.FormInput('FromUser', 'text', null, null, 'From User:', null, model.get('FromUser')),
                new FreeswitchConfig.Site.Form.FormInput('FromDomain', 'text', null, null, 'From Domain:', null, model.get('FromDomain')),
                new FreeswitchConfig.Site.Form.FormInput('Extension', 'text', null, null, 'Extension:', null, model.get('Extension')),
                new FreeswitchConfig.Site.Form.FormInput('Proxy', 'text', null, null, 'Proxy:', null, model.get('Proxy')),
                new FreeswitchConfig.Site.Form.FormInput('RegisterProxy', 'text', null, null, 'Register Proxy:', null, model.get('RegisterProxy')),
                new FreeswitchConfig.Site.Form.FormInput('ExpireSeconds', 'text', null, null, 'Expire time (in seconds):', null, model.get('ExpireSeconds'))
            ]
        );
        FreeswitchConfig.Site.Modals.ShowFormPanel(
            (isCreate ? 'Add New Outgoing SIP Trunk' : 'Edit Outgoing SIP Trunk'),
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
                        var attrs = new Object();
                        var canSubmit = true;
                        var inps = pars.frm.find('input,select');
                        for (var x = 0; x < inps.length; x++) {
                            var inp = $(inps[x]);
                            switch (inp.attr('name')) {
                                case 'Name':
                                case 'UserName':
                                case 'Realm':
                                case 'Password':
                                    canSubmit = canSubmit && FreeswitchConfig.Site.Validation.ValidateRequiredField(inp);
                                    attrs[inp.attr('name')] = inp.val();
                                    break;
                                case 'PingInterval':
                                case 'RetrySeconds':
                                    if (FreeswitchConfig.Site.Validation.ValidateRequiredField(inp)) {
                                        canSubmit = canSubmit && FreeswitchConfig.Site.Validation.ValidateIntegerInRange(inp, 10, 3600);
                                    } else {
                                        canSubmit = false;
                                    }
                                    attrs[inp.attr('name')] = inp.val() * 1;
                                    break;
                                case 'ExpireSeconds':
                                    if (inp.val() != '') {
                                        if (!FreeswitchConfig.Site.Validation.ValidateIntegerInRange(inp, 10, 3600)) {
                                            canSubmit = false;
                                        }
                                    }
                                    attrs[inp.attr('name')] = (inp.val() == '' ? null : inp.val());
                                    break;
                                case 'Profile':
                                    attrs[inp.attr('name')] = new FreeswitchConfig.Core.SipProfile.Model({ id: inp.val() });
                                    break;
                                case 'Register':
                                    attrs[inp.attr('name')] = inp.prop('checked');
                                    break;
                                default:
                                    attrs[inp.attr('name')] = (inp.val() == '' ? null : inp.val());
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
                                        alert('An error occured attempting to ' + (options.isCreate ? 'create' : 'update') + ' the ougoing SIP trunk');
                                    },
                                    isCreate: pars.isCreate,
                                    collection: pars.collection
                                });
                            }
                        } else {
                            FreeswitchConfig.Site.Modals.HideUpdating();
                            alert('You must corrected the required fields as well as ensure that the Ping Interval is a positive integer to create a new Outgoing SIP Trunk');
                        }
                    },
                    { frm: frm, collection: collection, model: model, isCreate: isCreate }
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
    },
    GeneratePage: function(container) {
        container = $(container);
        FreeswitchConfig.Site.Modals.ShowLoading();
        var col = new FreeswitchConfig.Trunks.OutgoingSIPTrunk.Collection();
        var view = new FreeswitchConfig.Trunks.OutgoingSIPTrunk.CollectionView({ collection: col });
        view.on('render', function() { FreeswitchConfig.Site.Modals.HideLoading(); });
        var addButton = CreateButton(
            'connect',
            'Create New Trunk',
            function(button, pars) {
                FreeswitchConfig.Trunks.OutgoingSIPTrunk.GenerateForm(pars.collection);
            },
            { collection: col }
        );

        container.append(addButton);
        container.append(view.$el);
        col.fetch();
    }
});