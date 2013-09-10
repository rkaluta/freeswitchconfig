CreateNameSpace('FreeswitchConfig.Routes.Intercom');

FreeswitchConfig.Routes.Intercom = $.extend(FreeswitchConfig.Routes.Intercom, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: FreeswitchConfig.Site.Skin.tr.Tag,
        className: "FreeswitchConfig Routing Intercom View",
        render: function() {
            this.$el.html('');
            var exts = '';
            for (var x = 0; x < this.model.attributes['Extensions'].length; x++) {
                exts += this.model.attributes['Extensions'][x].id + ', ';
            }
            if (exts.length > 0) {
                exts = exts.substring(0, exts.length - 2);
            }
            this.$el.append([
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Number', Attributes: { 'style': 'vertical-align:top' }, Content: this.model.get('Number') }),
                FreeswitchConfig.Site.Skin.td.Create({Class: this.className+' OneWay',Attributes:{'style':'vertical-align:top'},Content:(this.model.get('OneWay') ? FreeswitchConfig.Site.Skin.img.Create({Class:'tick'}) : '')}),
                FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' Description',Attributes:{'style':'vertical-align:top;'},Content:this.model.get('Description')}),
                FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' Extensions',Attributes:{'style':'vertical-align:top'},Content:exts}),
                FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' Buttons',Attributes:{'style':'vertical-align:top'},Content:[
                    FreeswitchConfig.Site.Skin.img.Create({Class:'button edit sound_edit'}),
                    FreeswitchConfig.Site.Skin.img.Create({Class:'button delete sound_delete'})
                ]})
            ]);
            this.trigger('render', this);
            return this;
        },
        events: {
            'click .button.delete.sound_delete': 'deleteModel',
            'click .button.edit.sound_edit': 'editModel'
        },
        deleteModel: function() {
            FreeswitchConfig.Site.Modals.ShowUpdating();
            this.model.destroy({
                success: function() { FreeswitchConfig.Site.Modals.HideUpdating(); },
                error: function() { alert('An error occured attempting to delete the Intercom.'); FreeswitchConfig.Site.Modals.HideUpdating(); }
            });
        },
        editModel: function() {
            FreeswitchConfig.Routes.Intercom.GenerateForm(null, this.model);
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: FreeswitchConfig.Site.Skin.table.Tag,
        className: "FreeswitchConfig Routing Intercom CollectionView "+FreeswitchConfig.Site.Skin.table.Class,
        initialize: function() {
            this.collection.on('reset', this.render, this); this.collection.on('sync',this.render,this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        attributes: { cellspacing: 0, cellpadding: 0 },
        render: function() {
            if (this.$el.find(FreeswitchConfig.Site.Skin.thead.Tag).length==0){
                this.$el.append([
                    FreeswitchConfig.Site.Skin.thead.Create({Class:this.className+' header',
                        Content:FreeswitchConfig.Site.Skin.tr.Create([
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Extension',Content:'Extension'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' OneWay',Content:'One Way'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Description',Content:'Description'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Extensions',Content:'Extensions'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Buttons',Content:'Actions'})
                      ])
                    }),
                    FreeswitchConfig.Site.Skin.tbody.Create()
                ]);
            }
            el = $(this.$el.find(FreeswitchConfig.Site.Skin.tbody.Tag)[0]);
            el.html('');
            if (this.collection.length == 0) {
                this.trigger('render', this);
            } else {
                var alt = false;
                for (var x = 0; x < this.collection.length; x++) {
                    var vw = new FreeswitchConfig.Routes.Intercom.View({ model: this.collection.at(x) });
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
        FreeswitchConfig.Site.Modals.ShowLoading();
        var isCreate = model == undefined;
        model = (model == undefined ? new FreeswitchConfig.Routes.Intercom.Model() : model);
        var exts = new Array();
        if (model.attributes['Extensions'] != null) {
            for (var x = 0; x < model.attributes['Extensions'].length; x++) {
                exts.push(model.attributes['Extensions'][x].id);
            }
        }
        var frm = FreeswitchConfig.Site.Form.GenerateForm(
            null,
            [
                new FreeswitchConfig.Site.Form.FormInput('Number', 'text', null, null, 'Extension Number:', null, model.get('Number')),
                new FreeswitchConfig.Site.Form.FormInput('OneWay', 'checkbox', null, null, 'One Way:', null, model.get('OneWay')),
                new FreeswitchConfig.Site.Form.FormInput('Description', 'textarea', null, null, 'Description:', { rows: '5', cols: '20' }, model.get('Description')),
                new FreeswitchConfig.Site.Form.FormInput('Extensions', 'checkbox_list', FreeswitchConfig.Core.Extension.SelectList(), null, 'Extension Numbers:', null, exts)
            ]
        );
        FreeswitchConfig.Site.Modals.ShowFormPanel(
            (isCreate ? 'Create New Intercom' : 'Edit Intercom'),
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
                        var number = $(pars.frm.find('input[name="Number"]')[0]);
                        var exts = new Array();
                        var inps = pars.frm.find('div[name="Extensions"] > span > input:checked');
                        for (var x = 0; x < inps.length; x++) {
                            exts.push(new FreeswitchConfig.Core.Extension.Model({ id: $(inps[x]).attr('name') }));
                        }
                        var canSubmit = (FreeswitchConfig.Site.Validation.ValidateRequiredField(number) ?
                        FreeswitchConfig.Site.Validation.ValidatePositiveIntegerField(number) :
                        false) && exts.length > 0;
                        if (canSubmit) {
                            if (!pars.model.set({ Number: number.val(), OneWay: pars.frm.find('input[name="OneWay"]:checked').legnth > 0, Description: ($(pars.frm.find('textarea')[0]).val() == '' ? null : $(pars.find('textarea')[0]).val()), Extensions: exts, Context: FreeswitchConfig.CurrentContext })) {
                                var msg = 'Please correct the following error(s)/field(s):';
                                var lis = [];
                                for (var x = 0; x < pars.model.errors.length; x++) {
                                    lis.push(FreeswitchConfig.Site.Skin.li.Create((pars.model.errors[x].error == '' ? pars.model.errors[x].field : pars.model.errors[x].error)));
                                }
                                FreeswitchConfig.Site.Modals.HideUpdating();
                                alert([msg,FreeswitchConfig.Site.Skin.ul.Create(lis)]);
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
                                        alert('An error occured attempting to ' + (options.isCreate ? 'create' : 'update') + ' the Intercom');
                                    },
                                    isCreate: pars.isCreate,
                                    collection: pars.collection
                                });
                            }
                        } else {
                            FreeswitchConfig.Site.Modals.HideUpdating();
                            alert('Please corrected the highlighted fields with the appropriate information and ensure you have selected at least 1 extension to intercom.');
                        }
                    },
                    { frm: frm, model: model, collection: collection, isCreate: isCreate }
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
        var col = new FreeswitchConfig.Routes.Intercom.Collection();
        var view = new FreeswitchConfig.Routes.Intercom.CollectionView({ collection: col });
        FreeswitchConfig.Site.setChangedDomain({ collection: col }, function(event) { FreeswitchConfig.Site.Modals.ShowLoading(); event.data.collection.fetch(); });
        view.on('render', function() { FreeswitchConfig.Site.Modals.HideLoading(); });
        var butAddNew = CreateButton(
            'sound_add',
            'Add New Intercom',
            function(button, pars) {
                FreeswitchConfig.Routes.Intercom.GenerateForm(pars.collection);
            },
            { collection: col }
        );
        container.append(butAddNew);
        container.append(view.$el);
        col.fetch();
    }
});