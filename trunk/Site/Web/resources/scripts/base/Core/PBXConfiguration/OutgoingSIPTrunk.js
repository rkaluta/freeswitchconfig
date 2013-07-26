CreateNameSpace('FreeswitchConfig.Trunks.OutgoingSIPTrunk');

FreeswitchConfig.Trunks.OutgoingSIPTrunk = $.extend(FreeswitchConfig.Trunks.OutgoingSIPTrunk, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: FreeswitchConfig.Site.Skin.tr.Tag,
        className: "FreeswitchConfig Routing OutgoingSIPTrunk View",
        render: function() {
            this.$el.html('');
            this.$el.append([
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Name', Attributes: { 'style': 'vertical-align:top' }, Content: this.model.get('Name') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Authentication', Attributes: { 'style': 'vertical-align:top' }, Content: this.model.get('UserName') + '@' + this.model.get('Realm') + ':' + this.model.get('Password') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' RegistrationType', Attributes: { 'style': 'vertical-align:top' }, Content: this.model.get('RegistrationType') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Register', Attributes: { 'style': 'vertical-align:top' }, Content: (this.model.get('Register') ? FreeswitchConfig.Site.Skin.img.Create({ Class: 'tick' }) : '') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Profile', Attributes: { 'style': 'vertical-align:top' }, Content: this.model.attributes['Profile'].id }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' PingInterval', Attributes: { 'style': 'vertical-align:top' }, Content: this.model.get('PingInterval') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Buttons', Attributes: { 'style': 'vertical-align:top' }, Content: [
                    FreeswitchConfig.Site.Skin.img.Create({ Class: 'button edit pencil' }),
                    FreeswitchConfig.Site.Skin.img.Create({ Class: 'button delete cancel' })
                ]})
            ]);
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
        tagName: FreeswitchConfig.Site.Skin.table.Tag,
        className: "FreeswitchConfig Routing OutgoingSIPTrunk CollectionView " + FreeswitchConfig.Site.Skin.table.Class,
        initialize: function() {
            this.collection.on('reset', this.render, this); this.collection.on('sync',this.render,this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        attributes: { cellspacing: 0, cellpadding: 0 },
        render: function() {
            if (this.$el.find(FreeswitchConfig.Site.Skin.thead.Tag).length == 0) {
                this.$el.append([
                    FreeswitchConfig.Site.Skin.thead.Create({ Class: this.className + ' header', Content:
                        FreeswitchConfig.Site.Skin.tr.Create([
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Name', Content: 'Name' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Authentication', Content: 'Authentication' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' RegistrationType', Content: 'RegistrationType' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Register', Content: 'Register' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Profile', Content: 'Profile' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' PingInterval', Content: 'Ping Interval' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Buttons', Content: 'Actions' })
                        ])
                    }),
                    FreeswitchConfig.Site.Skin.tbody.Create()
                ]);
            }
            var el = $(this.$el.find(FreeswitchConfig.Site.Skin.tbody.Tag)[0]);
            if (this.collection.length == 0) {
                this.trigger('render', this);
            } else {
                var alt = false;
                for (var x = 0; x < this.collection.length; x++) {
                    var vw = new FreeswitchConfig.Trunks.OutgoingSIPTrunk.View({ model: this.collection.at(x) });
                    vw.$el.addClass((alt ? FreeswitchConfig.Site.Skin.tr.AltClass : FreeswitchConfig.Site.Skin.tr.Class));
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
                                var msg = 'Please correct the following error(s)/field(s)';
                                var lis = [];
                                for (var x = 0; x < pars.model.errors.length; x++) {
                                    lis.push(FreeswitchConfig.Site.Skin.li.Create((pars.model.errors[x].error == '' ? pars.model.errors[x].field : pars.model.errors[x].error)));
                                }
                                FreeswitchConfig.Site.Modals.HideUpdating();
                                alert([msg, FreeswitchConfig.Site.Skin.ul.Create(lis)]);
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