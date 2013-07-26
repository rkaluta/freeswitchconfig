CreateNameSpace('FreeswitchConfig.Routes.VacationRoute');

FreeswitchConfig.Routes.VacationRoute = $.extend(FreeswitchConfig.Routes.VacationRoute, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: FreeswitchConfig.Site.Skin.tr.Tag,
        className: "FreeswitchConfig Routes VacationRoute VacationRoute View",
        render: function() {
            this.$el.html('');
            var actionText = '';
            switch (this.model.get('Type')) {
                case 'TransferToExtension':
                    actionText = 'Bridge to Extension: ' + this.model.attributes['BridgeExtension'].id;
                    break;
                case 'PhoneExtension':
                    actionText = 'Transfer to Dial Plan Extension: ' + this.model.get('ExtensionReference').Extension + ' in ' + this.model.get('ExtensionReference').Context;
                    break;
                case 'OutGateway':
                    actionText = 'Transfer Call to ' + this.model.get('GatewayNumber') + ' using the gateway ' + this.model.attributes['OutGateway'].id;
                    break;
                case 'PlayFile':
                    actionText = 'Pick up the phone and play the file ' + this.attributes['AudioFile'].id;
                    break;
            }
            if (this.model.get('EndWithVoicemail')) {
                actionText += '<br/>After ' + this.model.get('Timeout') + ' seconds, transfer the call to voicemail.';
            }
            this.$el.append([
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Name', Content: this.model.get('Name') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Number', Content: this.model.get('Number') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' StartDate EndDate', Content: 'FROM ' + FreeswitchConfig.Site.DateTimePicker.format(this.model.get('StartDate'), FreeswitchConfig.Routes.VacationRoute.dateFormat) + '<br/>TO ' + FreeswitchConfig.Site.DateTimePicker.format(this.model.get('EndDate'), FreeswitchConfig.Routes.VacationRoute.dateFormat) }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Type EndWithVoicemail', Content: actionText }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Buttons', Content: [
                    FreeswitchConfig.Site.Skin.img.Create({ Class: 'button edit cog_edit' }),
                    FreeswitchConfig.Site.Skin.img.Create({ Class: 'button delete cog_delete' })
                ]
                })
            ]);
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
                error: function() { alert('An error occured attempting to delete the Vacation Route.'); FreeswitchConfig.Site.Modals.HideUpdating(); }
            });
        },
        editModel: function() {
            FreeswitchConfig.Site.Modals.ShowLoading();
            var frm = FreeswitchConfig.Routes.VacationRoute.GenerateForm(null, this.model);
            FreeswitchConfig.Site.Modals.HideLoading();
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: FreeswitchConfig.Site.Skin.table.Tag,
        className: "FreeswitchConfig Routes VacationRoute CollectionView " + FreeswitchConfig.Site.Skin.table.Class,
        initialize: function() {
            this.collection.on('reset', this.render, this); this.collection.on('sync',this.render,this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        attributes: { cellpadding: 0, cellspacing: 0 },
        render: function() {
            if (this.$el.find(FreeswitchConfig.Site.Skin.thead.Tag).length == 0) {
                this.$el.append([
                    FreeswitchConfig.Site.Skin.thead.Create({
                        Class: this.className + ' header',
                        Content: FreeswitchConfig.Site.Skin.tr.Create([
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Name', Content: 'Name' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Number', Content: 'Number' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' StartDate EndDate', Content: 'Date' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Type EndWithVoicemail', Content: 'Description' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Buttons', Content: 'Actions' })
                        ])
                    }),
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
                    var vw = new FreeswitchConfig.Routes.VacationRoute.View({ model: this.collection.at(x) });
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
    dateFormat: '%d-%m-%Y %H:%i:%s',
    GenerateForm: function(collection, model) {
        isCreate = model == undefined;
        model = (model == undefined ? new FreeswitchConfig.Routes.VacationRoute.Model() : model);
        var frm = FreeswitchConfig.Site.Form.GenerateForm(
                null,
                [
                    (isCreate ?
                        new FreeswitchConfig.Site.Form.FormInput('Name', 'text', null, null, 'Name:') :
                        new FreeswitchConfig.Site.Form.FormStaticEntry('Name:', model.get('Name'))
                    ),
                    (isCreate ?
                        new FreeswitchConfig.Site.Form.FormInput('Extension', 'select', FreeswitchConfig.Core.ExtensionNumber.SelectList(), null, 'Extension:') :
                        new FreeswitchConfig.Site.Form.FormStaticEntry('Extension:', model.get('Number'))
                    ),
                    new FreeswitchConfig.Site.Form.FormInput('StartDate', 'datetime', null, null, 'Start:', null, model.get('StartDate')),
                    new FreeswitchConfig.Site.Form.FormInput('EndDate', 'datetime', null, null, 'End:', null, model.get('EndDate')),
                    new FreeswitchConfig.Site.Form.FormArrayInput(
                        'Voicemail:',
                        [
                            new FreeswitchConfig.Site.Form.FormInput('EndWithVoicemail', 'checkbox', null, null, 'End With Voicemail:', null, model.get('EndWithVoicemail')),
                            new FreeswitchConfig.Site.Form.FormInput('Timeout', 'text', null, null, 'Seconds before going to voicemail:', null, model.get('Timeout'))
                        ]),
                    new FreeswitchConfig.Site.Form.FormArrayInput('Actions', [
                        new FreeswitchConfig.Site.Form.FormInput('Type', 'select', [
                            new FreeswitchConfig.Site.Form.SelectValue('TransferToExtension', 'Transfer To Extension'),
                            new FreeswitchConfig.Site.Form.SelectValue('OutGateway', 'Transfer To External Number'),
                            new FreeswitchConfig.Site.Form.SelectValue('PlayFile', 'Play Audio File'),
                            new FreeswitchConfig.Site.Form.SelectValue('PhoneExtension', 'Transfer To Phone Control Extension')
                        ], null, 'Action:', null, model.get('Type')),
                        new FreeswitchConfig.Site.Form.FormInput('BridgeExtension', 'select', FreeswitchConfig.Core.Extension.SelectList(), null, 'Transfer To Extension:', null, (model.attributes['BridgeExtension'] == null ? null : model.attributes['BridgeExtension'].id)),
                        new FreeswitchConfig.Site.Form.FormInput('OutGateway', 'select', FreeswitchConfig.Trunks.Gateway.SelectList(), null, 'Outgoing Gateway:', null, (model.attributes['OutGateway'] == null ? null : model.attributes['OutGateway'].id)),
                        new FreeswitchConfig.Site.Form.FormInput('GatewayNumber', 'text', null, null, 'Outgoing Number:', null, model.get('GatewayNumber')),
                        new FreeswitchConfig.Site.Form.FormInput('AudioFile', 'file_browser', null, null, 'File To Play:', null, (model.get('AudioFile') == null ? null : model.attributes['AudioFile'].id)),
                        new FreeswitchConfig.Site.Form.FormInput('ExtensionReference', 'select', FreeswitchConfig.Routes.sCallExtensionReference.SelectList(), null, 'Phone Control Extension:', null, (model.attributes['ExtensionReference'] == null ? null : model.attributes['ExtensionReference'].id))
                    ])
                ]
            );
        var chkVoicemail = $(frm.find('input[name="EndWithVoicemail"]')[0]);
        var spn = $($(chkVoicemail.parent()).next());
        spn.hide();
        chkVoicemail.bind('click',
            { chk: chkVoicemail, spn: spn },
            function(event) {
                if (event.data.chk.is(':checked')) {
                    event.data.spn.show();
                } else {
                    event.data.spn.hide();
                }
            });
        var sel = $(frm.find('select[name="Type"]')[0]);
        sel.bind('change',
            { sel: sel },
            function(event) {
                var td = $($(event.data.sel.parent()).parent());
                $(td.find(FreeswitchConfig.Site.Skin.span.Tag)).hide();
                $(td.find(FreeswitchConfig.Site.Skin.span.Tag + ':first')[0]).show();
                switch (sel.val()) {
                    case 'TransferToExtension':
                        $(td.children()[1]).show();
                        break;
                    case 'OutGateway':
                        $(td.children()[2]).show();
                        $(td.children()[3]).show();
                        break;
                    case 'PlayFile':
                        $(td.children()[4]).show();
                        break;
                    case 'PhoneExtension':
                        $(td.children()[5]).show();
                        break;
                }
            });
        sel.trigger('change');
        FreeswitchConfig.Site.Modals.ShowFormPanel(
            (isCreate ? 'Create Vacation Route' : 'Edit Vacation Route'),
            frm,
            [
                CreateButton(
                    (isCreate ? 'cog_add' : 'cog_go'),
                    (isCreate ? 'Add New Vacation' : 'Update Vacation'),
                    function(button, pars) {
                        if (isCreate) {
                            FreeswitchConfig.Site.Modals.ShowSaving();
                        } else {
                            FreeswitchConfig.Site.Modals.ShowUpdating();
                        }
                        var inps = pars.frm.find('select,input');
                        var canSubmit = true;
                        var attrs = new Object();
                        var errors = [];
                        for (var x = 0; x < inps.length; x++) {
                            var inp = $(inps[x]);
                            attrs[inp.attr('name')] = null;
                            switch (inp.attr('name')) {
                                case 'StartDate':
                                case 'EndDate':
                                    if (!FreeswitchConfig.Site.Validation.ValidateRequiredField(inp)) {
                                        canSubmit = false;
                                    } else {
                                        attrs[inp.attr('name')] = FreeswitchConfig.Site.DateTimePicker.GetDateValueFromField(inp);
                                    }
                                    break;
                                case 'EndWithVoicemail':
                                    attrs.EndWithVoicemail = $(inp).prop('checked');
                                    break;
                                case 'BridgeExtension':
                                    if (inp.find('option:selected').length > 0) {
                                        attrs.BridgeExtension = new FreeswitchConfig.Core.Extension.Model({ id: $(inp.find('option:selected')[0]).val() });
                                    } else if (attrs.Type == 'TransferToExtension') {
                                        canSubmit = false;
                                    }
                                    break;
                                case 'ExtensionReference':
                                    if (inp.find('option:selected').length > 0) {
                                        attrs[inp.attr('name')] = new FreeswitchConfig.Routes.sCallExtensionReference.Model({ id: $(inp.find('option:selected')[0]).val() });
                                    } else if (attrs.Type == 'PhoneExtension') {
                                        canSubmit = false;
                                    }
                                    break;
                                case 'OutGateway':
                                    if (inp.find('option:selected').length > 0) {
                                        attrs[inp.attr('name')] = new FreeswitchConfig.Trunks.Gateway.Model({ id: $(inp.find('option:selected')[0]).val() });
                                    } else if (attrs.Type == 'OutGateway') {
                                        canSubmit = false;
                                    }
                                    break;
                                default:
                                    if (inp.attr('name') == 'Name' || inp.attr('name') == 'Extension') {
                                        if (FreeswitchConfig.Site.Validation.ValidateRequiredField(inp)) {
                                            attrs[inp.attr('name')] = inp.val();
                                        } else {
                                            canSubmit = false;
                                        }
                                    } else if (inp.attr('name') == 'VoicemailTimeout') {
                                        if (FreeswitchConfig.Site.Validation.ValidateRequiredField(inp)
                                            && FreeswitchConfig.Site.Validation.ValidatePositiveIntegerField(inp)) {
                                            attrs[inp.attr('name')] = inp.val();
                                        } else {
                                            errors.push(FreeswitchConfig.Site.Skin.li.Create('You must enter a positive whole number as the timeout to lead to voicemail.'));
                                            canSubmit = false;
                                        }
                                    } else if ((inp.attr('name') == 'GatewayNumber' && attrs.Type == 'OutGateway')
                                    || (inp.attr('name') == 'AudioFile' && attrs.Type == 'PlayFile')) {
                                        if (!FreeswitchConfig.Site.Validation.ValidateRequiredField(inp)) {
                                            attrs[inp.attrs('name')] = inp.val();
                                        } else {
                                            canSubmit = false;
                                        }
                                    } else {
                                        attrs[inp.attr('name')] = (inp.val() == '' ? null : inp.val());
                                    }
                                    break;
                            }
                        }
                        if (attrs.StartDate == attrs.EndDate) {
                            canSubmit = false;
                            errors.push(FreeswitchConfig.Site.Skin.li.Create('You cannot have a start and end date the same for a vacation entry.'));
                        } else if (attrs.StartDate > attrs.EndDate) {
                            canSubmit = false;
                            errors.push(FreeswitchConfig.Site.Skin.li.Create('You cannot have a start after the  end date for a vacation entry.'));
                        }
                        if (canSubmit) {
                            if (pars.isCreate) {
                                attrs.Number = attrs.Extension.split('@')[0];
                                attrs.Context = new FreeswitchConfig.Core.Context.Model({ id: attrs.Extension.split('@')[1] });
                                delete attrs.Extension;
                            }
                            if (!pars.model.set(attrs)) {
                                for (var x = 0; x < pars.model.errors.length; x++) {
                                    errors.push(FreeswitchConfig.Site.Skin.li.Create((pars.model.errors[x].error == '' ? pars.model.errors[x].field : pars.model.errors[x].error)));
                                }
                                FreeswitchConfig.Site.Modals.HideUpdating();
                                alert('Please correct the following error(s)/field(s):', FreeswitchConfig.Site.Skin.ul.Create(errors));
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
                                        alert('An error occured attempting to ' + (options.isCreate ? 'create' : 'update') + ' the Vacation Route');
                                    },
                                    isCreate: pars.isCreate,
                                    collection: pars.collection
                                });
                            }
                        } else if (errors != '') {
                            FreeswitchConfig.Site.Modals.HideUpdating();
                            alert('Please correct the errors below to continue:', FreeswitchConfig.Site.Skin.ul.Create(errors));
                        } else {
                            FreeswitchConfig.Site.Modals.HideUpdating();
                            alert('You must specify values for the required fields indicated.');
                        }
                    },
                    { collection: collection, model: model, isCreate: isCreate, frm: frm }
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
        var col = new FreeswitchConfig.Routes.VacationRoute.Collection();
        var view = new FreeswitchConfig.Routes.VacationRoute.CollectionView({ collection: col });
        FreeswitchConfig.Site.setChangedDomain({ collection: col }, function(event) { FreeswitchConfig.Site.Modals.ShowLoading(); event.data.collection.fetch(); });
        view.on('render', function() { FreeswitchConfig.Site.Modals.HideLoading(); });
        var butAddNew = CreateButton(
            'cog_add',
            'Add New Vacation',
            function(button, pars) {
                FreeswitchConfig.Site.Modals.ShowLoading();
                FreeswitchConfig.Routes.VacationRoute.GenerateForm(pars.collection);
                FreeswitchConfig.Site.Modals.HideLoading();
            },
            { collection: col }
        );
        container.append(butAddNew);
        container.append(view.$el);
        col.fetch();
    }
});
