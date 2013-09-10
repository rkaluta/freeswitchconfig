CreateNameSpace('FreeswitchConfig.Core.Extension');

FreeswitchConfig.Core.Extension = $.extend(FreeswitchConfig.Core.Extension, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: FreeswitchConfig.Site.Skin.tr.Tag,
        className: "FreeswitchConfig Core Extension View",
        render: function() {
            this.$el.html('');
            this.$el.append([
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Number', Attributes: { style: 'vertical-align:top' }, Content: this.model.get('Number') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' InternalCallerIDName', Attributes: { style: 'vertical-align:top' }, Content: (this.model.get('InternalCallerIDName') == null ? '' : this.model.get('InternalCallerIDName')) }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Buttons', Attributes: { style: 'vertical-align:top' }, Content: [
                    FreeswitchConfig.Site.Skin.img.Create({ Class: 'button edit phone_edit', Attributes: { title: 'Edit Extension Settings'} }),
                    FreeswitchConfig.Site.Skin.img.Create({ Class: 'button delete cancel', Attributes: { title: 'Delete Extension'} }),
                    (this.model.get('HasVoicemail') ? [
                        FreeswitchConfig.Site.Skin.img.Create({ Class: 'email_edit button', Attributes: { title: 'Edit Voicemail Settings'} }),
                        FreeswitchConfig.Site.Skin.img.Create({ Class: 'email_delete button', Attributes: { title: 'Delete Voicemail'} })
                    ] : FreeswitchConfig.Site.Skin.img.Create({ Class: 'email_add button', Attributes: { title: 'Add Voicemail to Extension'} }))
                ]
                })
            ]);
            this.trigger('render', this);
            return this;
        },
        events: {
            'click .button.delete.phone_delete': 'deleteModel',
            'click .button.edit.phone_edit': 'editModel',
            'click .button.email_edit': 'editVoicemail',
            'click .button.email_delete': 'deleteVoicemail',
            'click .button.email_add': 'addVoicemail'
        },
        deleteModel: function() {
            FreeswitchConfig.Site.Modals.ShowUpdating();
            this.model.destroy({
                success: function() { FreeswitchConfig.Site.Modals.HideUpdating(); },
                error: function() { alert('An error occured attempting to delete the Intercom.'); FreeswitchConfig.Site.Modals.HideUpdating(); }
            });
        },
        editModel: function() {
            FreeswitchConfig.Core.Extension.GenerateForm(this.collection, this.model);
        },
        editVoicemail: function() {
            FreeswitchConfig.Site.Modals.ShowLoading();
            var vm = new FreeswitchConfig.Core.VoiceMail.Model({ id: this.model.id });
            vm.fetch({ async: false });
            FreeswitchConfig.Core.Extension.GenerateVoicemailForm(this.model, vm);
            FreeswitchConfig.Site.Modals.HideLoading();
        },
        deleteVoicemail: function() {
            FreeswitchConfig.Site.Modals.confirm('Click okay to delete the voicemail for ' + this.model.get('Number') + ' or cancel to abort',
            function(res) {
                if (res) {
                    FreeswitchConfig.Site.Modals.ShowUpdating();
                    this.model.get('Mail').destroy({ success: function() { this.render(); FreeswitchConfig.Site.Modals.HideUpdating(); },
                        failure: function() { FreeswitchConfig.Site.Modals.HideUpdating(); alert('An error occured removing the voicemail from the extension ' + this.model.get('Number')); }
                    });
                }
            });
        },
        addVoicemail: function() {
            FreeswitchConfig.Site.Modals.ShowLoading();
            FreeswitchConfig.Core.Extension.GenerateVoicemailForm(this.model);
            FreeswitchConfig.Site.Modals.HideLoading();
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: FreeswitchConfig.Site.Skin.table.Tag,
        className: "FreeswitchConfig Core Extension CollectionView " + FreeswitchConfig.Site.Skin.table.Class,
        initialize: function() {
            this.collection.on('reset', this.render, this); this.collection.on('sync',this.render,this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
            this.collection.on('sync', this.render, this);
        },
        attributes: { cellspacing: 0, cellpadding: 0 },
        render: function() {
            if (this.$el.find(FreeswitchConfig.Site.Skin.thead.Tag).length == 0) {
                this.$el.append([
                    FreeswitchConfig.Site.Skin.thead.Create({ Class: this.className + ' header', Content:
                        FreeswitchConfig.Site.Skin.tr.Create({ Content: [
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Number', Content: 'Extension #' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' CallerIDName', Content: 'Caller ID Name' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Buttons', Content: 'Actions' })
                        ]
                        })
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
                    var vw = new FreeswitchConfig.Core.Extension.View({ model: this.collection.at(x), collection: this });
                    if (alt) {
                        vw.$el.addClass(FreeswitchConfig.Site.Skin.tr.AltClass);
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
        model = (model == undefined ? new FreeswitchConfig.Core.Extension.Model() : model);
        var frm = FreeswitchConfig.Site.Form.GenerateForm(
            null,
            [
                (isCreate ? new FreeswitchConfig.Site.Form.FormInput('extensionNumber', 'text', null, null, 'Number:', null, model.get('Number'))
                : new FreeswitchConfig.Site.Form.FormStaticEntry('Number:', model.get('Number'))),
                new FreeswitchConfig.Site.Form.FormInput('password', 'text', null, null, 'Password:', null, model.get('Password')),
                new FreeswitchConfig.Site.Form.FormInput('internalCallerIDName', 'text', null, null, 'Internal Caller ID Name:', null, model.get('InternalCallerIDName')),
                new FreeswitchConfig.Site.Form.FormInput('internalCallerID', 'text', null, null, 'Internal Caller ID Number:', null, model.get('InternalCallerIDNumber')),
                new FreeswitchConfig.Site.Form.FormInput('externalCallerIDName', 'text', null, null, 'External Caller ID Name:', null, model.get('ExternalCallerIDName')),
                new FreeswitchConfig.Site.Form.FormInput('externalCallerID', 'text', null, null, 'External Caller ID Number:', null, model.get('ExternalCallerIDNumber'))
            ]);
        FreeswitchConfig.Site.Modals.ShowFormPanel(
            (isCreate ? 'Add New Extension' : 'Edit Extension'),
            frm,
            [
                CreateButton(
                    'accept',
                    (isCreate ? 'Create Extension' : 'Update Extension'),
                    function(button, pars) {
                        FreeswitchConfig.Site.Modals.ShowSaving();
                        var number = $(pars.frm.find('input[name="extensionNumber"]')[0]);
                        var password = $(pars.frm.find('input[name="password"]')[0]);
                        var callerName = $(pars.frm.find('input[name="internalCallerIDName"]')[0]);
                        var canSubmit = FreeswitchConfig.Site.Validation.ValidateRequiredField(number)
                        && FreeswitchConfig.Site.Validation.ValidateRequiredField(password)
                        && FreeswitchConfig.Site.Validation.ValidateRequiredField(callerName);
                        if (!canSubmit) {
                            FreeswitchConfig.Site.Modals.HideSaving();
                            alert('Pleae supply values for the fields marked with a star to create an extension.');
                        } else {
                            model.set({
                                'Number': number.val(),
                                'Password': password.val(),
                                'Domain': FreeswitchConfig.CurrentDomain,
                                'Context': FreeswitchConfig.CurrentContext,
                                'InternalCallerIDName': callerName.val(),
                                'InternalCallerIDNumber': $(pars.frm.find('select[name="internalCallerID"]')[0]).val(),
                                'ExternalCallerIDName': $(pars.frm.find('select[name="externalCallerIDName"]')[0]).val(),
                                'ExternalCallerIDNumber': $(pars.frm.find('select[name="externalCallerID"]')[0]).val()
                            });
                            if (isCreate) {
                                model.save({
                                    collection: pars.collection,
                                    success: function(model, response, options) {
                                        options.collection.add(model); FreeswitchConfig.Site.Modals.HideSaving();
                                    },
                                    error: function() {
                                        FreeswitchConfig.Site.Modals.HideSaving();
                                        alert('An error occured attempting to create the Extension.');
                                    }
                                });
                            } else {
                                model.save({ success: function() { FreeswitchConfig.Site.Modals.HideSaving(); },
                                    error: function() { FreeswitchConfig.Site.Modals.HideSaving(); alert('An error occured attempting to create the Extension.'); }
                                });
                            }
                        }
                    },
                    { collection: collection, isCreate: isCreate, model: model, frm: frm }
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
    },
    GenerateVoicemailForm: function(extension, model) {
        var isCreate = model == undefined;
        model = (model == undefined ? new FreeswitchConfig.Core.VoiceMail.Model() : model);
        var frm = FreeswitchConfig.Site.Form.GenerateForm(
                null,
                [
                    new FreeswitchConfig.Site.Form.FormInput('password', 'text', null, null, 'Password:', null, model.get('Password')),
                    new FreeswitchConfig.Site.Form.FormInput('email', 'text', null, null, 'Email:', null, model.get('Email')),
                    new FreeswitchConfig.Site.Form.FormInput('maxMessages', 'text', null, null, 'Maximum Number Of Messages:', null, model.get('MaxMessages')),
                    new FreeswitchConfig.Site.Form.FormInput('copyTo', 'select', [new FreeswitchConfig.Site.Form.SelectValue('')].concat(FreeswitchConfig.Core.Extension.SelectList()), null, 'Copy To:', null, (model.attributes['CopyTo'] == null ? null : model.attributes['CopyTo'].id)),
                    new FreeswitchConfig.Site.Form.FormInput('attachToEmail', 'checkbox', null, null, 'Attach to Email:', null, model.get('AttachToEmail'))
                ]
        );
        FreeswitchConfig.Site.Modals.ShowFormPanel(
            (isCreate ? 'Add Voicemail' : 'Edit Voicemail'),
            frm,
            [
                CreateButton(
                    'accept',
                    (isCreate ? 'Add Voicemail' : 'Update Voicemail'),
                    function(button, pars) {
                        if (pars.isCreate) {
                            FreeswitchConfig.Site.Modals.ShowSaving();
                        } else {
                            FreeswitchConfig.Site.Modals.ShowUpdating();
                        }
                        var password = $(pars.frm.find('input[name="password"]')[0]);
                        var email = $(pars.frm.find('input[name="email"]')[0]);
                        var maxMessages = $(pars.frm.find('input[name="maxMessages"]')[0]);
                        if (FreeswitchConfig.Site.Validation.ValidateRequiredField(password)
                            && FreeswitchConfig.Site.Validation.ValidateEmailField(email)
                            && FreeswitchConfig.Site.Validation.ValidatePositiveIntegerField(maxMessage)) {
                            if (pars.isCreate) {
                                FreeswitchConfig.Site.Modals.ShowSaving();
                            } else {
                                FreeswitchConfig.Site.Modals.ShowUpdating();
                            }
                            alert('Please correct the indicated fields');
                        } else {
                            pars.model.set({
                                'Password': password.val(),
                                'Email': (email.val() == '' ? null : email.val()),
                                'MaxMessages': (maxMessages.val() == '' ? null : maxMessages.val()),
                                'CopyTo': ($(pars.frm.find('select[name="copyTo"]')[0]).val() == '' ? null : new FreeswitchConfig.Core.Extension({ id: $(pars.frm.find('select[name="copyTo"]')[0]).val() })),
                                'AttachToEmail': pars.find('input[name="attachToEmail"]:checked').length > 0,
                                'Number': pars.extension.get('Number'),
                                'Context': pars.extension.attribute['Context']
                            });
                            pars.model.save(
                                {
                                    extension: extension,
                                    isCreate: isCreate,
                                    success: function(model, response, options) {
                                        if (options.isCreate) {
                                            FreeswitchConfig.Site.Modals.HideSaving();
                                            options.extension.attributes['HasVoicemail'] = true;
                                            options.extension.trigger('change', options.extension);
                                        } else {
                                            FreeswitchConfig.Site.Modals.HideUpdating();
                                        }
                                    },
                                    error: function() {
                                        if (options.isCreate) {
                                            FreeswitchConfig.Site.Modals.HideSaving();
                                            alert('An error occured attempting to create a voicemail box for the extension.');
                                        } else {
                                            FreeswitchConfig.Site.Modals.HideUpdating();
                                            alert('An error occured attempting to update the voicemail box settings.');
                                        }
                                    }
                                }
                            );
                        }
                    },
                    { extension: extension, isCreate: isCreate, model: model, frm: frm }
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
    },
    GeneratePage: function(container) {
        container = $(container);
        FreeswitchConfig.Site.Modals.ShowLoading();
        var col = new FreeswitchConfig.Core.Extension.Collection();
        var view = new FreeswitchConfig.Core.Extension.CollectionView({ collection: col });
        FreeswitchConfig.Site.setChangedDomain({ collection: col }, function(event) { FreeswitchConfig.Site.Modals.ShowLoading(); event.data.collection.fetch(); });
        view.on('render', function() { FreeswitchConfig.Site.Modals.HideLoading(); });
        container.append(
            CreateButton(
                null,
                'Add New Extension',
                function(button, pars) {
                    FreeswitchConfig.Site.Modals.ShowLoading();
                    FreeswitchConfig.Core.Extension.GenerateForm(pars.collection);
                    FreeswitchConfig.Site.Modals.HideLoading();
                },
                { collection: col }
            )
        );
        container.append(view.$el);
        col.fetch();
    }
});