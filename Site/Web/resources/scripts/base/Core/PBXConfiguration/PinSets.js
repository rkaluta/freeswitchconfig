CreateNameSpace('FreeswitchConfig.PBX.PinSet');

FreeswitchConfig.PBX.PinSet = $.extend(FreeswitchConfig.PBX.PinSet, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: FreeswitchConfig.Site.Skin.tr.Tag,
        className: "FreeswitchConfig PBX PinSet View",
        render: function() {
            this.$el.html('');
            this.$el.append([
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Name', Attributes: { 'style': 'vertical-align:top' }, Content: this.model.get('Name') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Advanced', Attributes: { 'style': 'vertical-align:top' }, Content: (this.model.get('Advanced') ? FreeswitchConfig.Site.Skin.img.Create({ Class: 'tick' }) : '') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Description', Attributes: { 'style': 'vertical-align:top' }, Content: this.model.get('Description') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Buttons', Attributes: { 'style': 'vertical-align:top' }, Content: [
                    FreeswitchConfig.Site.Skin.img.Create({ Class: 'button edit book_edit' }),
                    FreeswitchConfig.Site.Skin.img.Create({ Class: 'button delete book_delete' })
                ]
                })
            ]);
            $(this.el).attr('name', this.model.id);
            this.trigger('render', this);
            return this;
        },
        events: {
            'click .button.delete.book_delete': 'deleteModel',
            'click .button.edit.book_edit': 'editModel'
        },
        deleteModel: function() {
            FreeswitchConfig.Site.Modals.ShowUpdating();
            this.model.destroy({
                success: function() { FreeswitchConfig.Site.Modals.HideUpdating(); },
                error: function() { alert('An error occured attempting to delete the Pinset.'); FreeswitchConfig.Site.Modals.HideUpdating(); }
            });
        },
        editModel: function() {
            FreeswitchConfig.PBX.PinSet.GenerateForm(null, this.model);
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: FreeswitchConfig.Site.Skin.table.Tag,
        className: "FreeswitchConfig PBX PinSet CollectionView " + FreeswitchConfig.Site.Skin.table.Class,
        initialize: function() {
            this.collection.on('reset', this.render, this); this.collection.on('sync',this.render,this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        attributes: { cellspacing: 0, cellpadding: 0 },
        render: function() {
            if (this.$el.find(FreeswitchConfig.Site.Skin.thead.Tag).length == 0) {
                this.$el.append([
                    FreeswitchConfig.Site.Skin.thead.Create({ Class: this.className + ' header', Content: FreeswitchConfig.Site.Skin.tr.Create([
                        FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Name', Content: 'Name' }),
                        FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Advanced', Content: 'Advanced' }),
                        FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Description', Attributes: { 'colspan': '2', 'style': 'text-align:left' }, Content: 'Description' })
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
                    var vw = new FreeswitchConfig.PBX.PinSet.View({ model: this.collection.at(x) });
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
        model = (model == undefined ? new FreeswitchConfig.PBX.PinSet.Model() : model);
        var adExts = FreeswitchConfig.Site.Skin.span.Create({ Attributes: { 'name': 'advancedPins'} });
        var exts = FreeswitchConfig.Core.Extension.SelectList();
        for (var y = 0; y < exts.length; y++) {
            adExts.append($('<label for="pin_' + exts[y].ID + '">' + exts[y].Text + ' : </label>' + '<input type="text" id="pin_' + exts[y].ID + '" name="' + exts[y].ID + '" maxlength="10"/><br/>'));
        }
        var pins = null;
        if (!isCreate) {
            if (model.get('Advanced')) {
                for (var x = 0; x < model.get('Pins').length; x++) {
                    $(adExts.find('input[name="' + model.get('Pins')[x].Name + '"]')[0]).val(model.get('Pins')[x].Value);
                }
            } else {
                pins = '';
                for (var x = 0; x < model.get('Pins').length; x++) {
                    pins += model.get('Pins')[x].Value + '\n';
                }
            }
        }
        var frm = FreeswitchConfig.Site.Form.GenerateForm(
            null,
            [
                new FreeswitchConfig.Site.Form.FormInput('Name', 'text', null, null, 'Name:', { 'max-length': '15' }, model.get('Name')),
                new FreeswitchConfig.Site.Form.FormInput('Description', 'textarea', null, null, 'Description:', { 'max-length': '250', rows: 5, cols: 20 }, model.get('Description')),
                new FreeswitchConfig.Site.Form.FormInput('Advanced', 'checkbox', null, null, 'Advanced:', null, model.get('Advanced')),
                new FreeswitchConfig.Site.Form.FormStaticEntry('Pins:', adExts),
                new FreeswitchConfig.Site.Form.FormInput('Pins', 'textarea', null, null, 'Pins:', { rows: 5, cols: 20 }, pins)
            ]
        );
        var trAdv = $($($(frm.find(FreeswitchConfig.Site.Skin.span.Tag + '[name="advancedPins"]')[0]).parent()).parent());
        var trBasic = $(trAdv.next());
        trAdv.hide();
        if (!isCreate) {
            if (model.get('Advanced')) {
                trAdv.show();
                trBasic.hide();
            }
        }
        trBasic.attr('class', trAdv.attr('class'));
        var chkAdv = $(frm.find('input[name="Advanced"]')[0]);
        chkAdv.bind('click',
        { chkAdv: chkAdv, trAdv: trAdv, trBasic: trBasic },
        function(event) {
            if (event.data.chkAdv.is(':checked')) {
                event.data.trAdv.show();
                event.data.trBasic.hide();
            } else {
                event.data.trAdv.hide();
                event.data.trBasic.show();
            }
        });
        FreeswitchConfig.Site.Modals.ShowFormPanel(
            (isCreate ? 'Create Pinset' : 'Edit Pinset'),
            frm,
            [
                CreateButton(
                    'accept',
                    'Okay',
                    function(button, pars) {
                        FreeswitchConfig.Site.Modals.ShowUpdating();
                        var attrs = new Object();
                        var name = $(pars.frm.find('input[name="Name"]')[0]);
                        var desc = $(pars.frm.find('textarea[name="Description"]')[0]);
                        var canSubmit = (FreeswitchConfig.Site.Validation.ValidateRequiredField(name) ? FreeswitchConfig.Site.Validation.ValidateFieldLength(name, 15) : false)
                        && (FreeswitchConfig.Site.Validation.ValidateRequiredField(desc) ? FreeswitchConfig.Site.Validation.ValidateFieldLength(desc, 250) : false);
                        var attrs = { Context: FreeswitchConfig.CurrentContext, Name: name.val(), Description: desc.val(), Advanced: pars.frm.find('input[name="Advanced"]:checked').length > 0, Pins: new Array() };
                        if (attrs.Advanced) {
                            var ins = pars.frm.find(FreeswitchConfig.Site.Skin.span.Tag + '[name="advancedPins"]>input');
                            for (var x = 0; x < ins.length; x++) {
                                if ($(ins[x]).val() != '') {
                                    attrs.Pins.push({ Name: $(ins[x]).attr('name'), Value: $(ins[x]).val() });
                                }
                            }
                        } else {
                            var pins = $(pars.frm.find('textarea[name="Pins"]')[0]).val().split('\n');
                            var x = 0;
                            for (var y = 0; y < pins.length; y++) {
                                if (pins[y] != '') {
                                    attrs.Pins.push({ Name: x, Value: pins[y] });
                                    x++;
                                }
                            }
                        }
                        canSubmit = canSubmit && attrs.Pins.length > 0;
                        if (canSubmit) {
                            if (!pars.model.set(attrs)) {
                                var lis = [];
                                for (var x = 0; x < pars.model.errors.length; x++) {
                                    lis.push(FreeswitchConfig.Site.Skin.li.Create((pars.model.errors[x].error == '' ? pars.model.errors[x].field : pars.model.errors[x].error)));
                                }
                                FreeswitchConfig.Site.Modals.HideUpdating();
                                alert(['Please correct the following error(s)/field(s):', FreeswitchConfig.Site.Skin.ul.Create(lis)]);
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
                                        alert('An error occured attempted to ' + (options.isCreate ? 'create' : 'update') + ' the pinset');
                                    },
                                    isCreate: pars.isCreate,
                                    collection: pars.collection
                                });
                            }
                        } else {
                            FreeswitchConfig.Site.Modals.HideUpdating();
                            alert('You must specify the required fields as well as at least 1 pin before creating the pinset.');
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
            ]);
        FreeswitchConfig.Site.Modals.HideLoading();
    },
    GeneratePage: function(container) {
        FreeswitchConfig.Site.Modals.ShowLoading();
        var col = new FreeswitchConfig.PBX.PinSet.Collection();
        FreeswitchConfig.Site.setChangedDomain({ collection: col }, function(event) { FreeswitchConfig.Site.Modals.ShowLoading(); event.data.collection.fetch(); });
        var view = new FreeswitchConfig.PBX.PinSet.CollectionView({ collection: col });
        view.on('render', function() { FreeswitchConfig.Site.Modals.HideLoading(); });
        var butAdd = CreateButton(
            'book_add',
            'Add New Pinset',
            function(button, pars) {
                FreeswitchConfig.PBX.PinSet.GenerateForm(pars.collection);
            },
            { collection: col }
        );
        container.append(butAdd);
        container.append(view.$el);
        col.fetch();
    }
});