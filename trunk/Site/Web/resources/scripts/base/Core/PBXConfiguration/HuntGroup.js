CreateNameSpace('FreeswitchConfig.Routes.HuntGroup');

FreeswitchConfig.Routes.HuntGroup = $.extend(FreeswitchConfig.Routes.HuntGroup, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: FreeswitchConfig.Site.Skin.tr.Tag,
        className: "FreeswitchConfig Routes HuntGroup View",
        render: function() {
            this.$el.html('');
            var content = [];
            for (var x = 0; x < this.model.attributes['Extensions'].length; x++) {
                content.push(FreeswitchConfig.Site.Skin.li.Create(this.model.attributes['Extensions'].at(x).id));
            }
            this.$el.append([
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Number', Content: this.model.get('Number') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' RingSequential', Content: (this.model.get('RingSequential') ? FreeswitchConfig.Site.Skin.img.Create({ Class: 'tick' }) : '') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Extensions', Content: FreeswitchConfig.Site.Skin.ul.Create({ Content: content }) }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Actions', Content: [
                    FreeswitchConfig.Site.Skin.img.Create({ Class: 'button edit pencil' }),
                    FreeswitchConfig.Site.Skin.img.Create({ Class: 'button cancel delete' })
                ]
                })
            ]);
            return this;
        },
        events: {
            'click button.edit.pencil': 'editModel',
            'click button.delete.cancel': 'deleteModel'
        },
        deleteModel: function() {
            FreeswitchConfig.Site.Modals.ShowUpdating();
            this.model.destroy({
                success: function() { FreeswitchConfig.Site.Modals.HideUpdating(); },
                error: function() { alert('An error occured attempting to delete the Direct Line.'); FreeswitchConfig.Site.Modals.HideUpdating(); }
            });
        },
        editModel: function() {
            FreeswitchConfig.Site.Modals.ShowLoading();
            FreeswitchConfig.Routes.HuntGroup.GenerateForm(null, this.model);
            FreeswitchConfig.Site.Modals.HideLoading();
        }
    }),
    CollectionView: Backbone.View.extend({
        initialize: function() {
            this.collection.on('reset', this.render, this); this.collection.on('sync',this.render,this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        tagName: FreeswitchConfig.Site.Skin.table.Tag,
        className: "FreeswitchConfig Routes HuntGroup CollectionView " + FreeswitchConfig.Site.Skin.table.Class,
        attributes: FreeswitchConfig.Site.Skin.table.Attributes,
        render: function() {
            if (this.$el.find(FreeswitchConfig.Site.Skin.thead.Tag).length == 0) {
                this.$el.append([
                    FreeswitchConfig.Site.Skin.thead.Create({ Class: this.className + ' header', Content:
                        FreeswitchConfig.Site.Skin.tr.Create({ Content: [
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Number', Content: 'Number' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' RingSequential', Content: 'Ring Sequential' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Extensions', Content: 'Extensions' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Actions', Content: 'Actions' })
                        ]
                        })
                    }),
                    FreeswitchConfig.Site.Skin.tbody.Create()
                ]);
            }
            var el = $(this.$el.find(FreeswitchConfig.Site.Skin.tbody.Tag)[0]);
            el.html('');
            var alt = false;
            for (var x = 0; x < this.collection.length; x++) {
                var vw = new FreeswitchConfig.Routes.HuntGroup.View({ model: this.collection.at(x) });
                if (alt) {
                    vw.$el.attr('class', vw.$el.attr('class') + ' Alt');
                }
                alt = !alt;
                el.append(vw.$el);
                vw.render();
            }
            this.trigger('render', this);
        }
    }),
    GenerateForm: function(collection, model) {
        var isCreate = model == undefined;
        model = (model == undefined ? new FreeswitchConfig.Routes.HuntGroup.Model() : model);
        var inp = $('<select></select>');
        var ul = FreeswitchConfig.Site.Skin.ol.Create({ Attributes: { 'name': 'extensions'} });
        var butAdd = FreeswitchConfig.Site.Skin.img.Create({ Class: 'button add' });
        var frm = FreeswitchConfig.Site.Form.GenerateForm(
            null,
            [
                new FreeswitchConfig.Site.Form.FormInput('number', 'text', null, true, 'Extension Number', null, null),
                new FreeswitchConfig.Site.Form.FormInput('sequential', 'radio', [new FreeswitchConfig.Site.Form.SelectValue('true', 'Yes'), new FreeswitchConfig.Site.Form.SelectValue('false', 'No')], true, 'Ring Sequential', null, null),
                new FreeswitchConfig.Site.Form.FormStaticEntry('Extensions', [inp, butAdd, ul])
            ]
        );
        butAdd.bind('click',
        { inp: inp, ul: ul },
        function(event) {
            var opt = $(event.data.inp.find('option:selected')[0]);
            event.data.ul.append(FreeswitchConfig.Site.Skin.li.Create({ Attributes: { 'name': opt.val() }, Content: [
                opt.text(),
                FreeswitchConfig.Site.Skin.img.Create({ Class: 'button cancel' }),
                FreeswitchConfig.Site.Skin.img.Create({ Class: 'arrow_up_down', Attributes: { 'style': 'cursor:move;'} })
                ]
            }));
            var imgDelete = $(spn.find(FreeswitchConfig.Site.Skin.img.Tag + '.button')[0]);
            imgDelete.bind('click',
            { button: imgDelete, select: event.data.inp },
            function(evnt) {
                var li = $(evnt.data.button.parent());
                evnt.data.select.append('<option value="' + li.attr('name') + '">' + li.text() + '</option>');
                $(li.parent()).sortable({
                    axis: 'y',
                    handle: FreeswitchConfig.Site.Skin.img.Tag + ':last',
                    placeholder: 'ui-state-highlight',
                    tolerance: 'pointer'
                });
                li.remove();
            });
            opt.remove();
        });
        var sels = FreeswitchConfig.Core.Extension.SelectList();
        for (var x = 0; x < sels.length; x++) {
            inp.append('<option value="' + sels[x].ID + '">' + sels[x].Text + '</option>');
        }
        if (model.attributes['Extensions'] != null) {
            for (var x = 0; x < model.attributes['Extensions'].length; x++) {
                $(inp.find('option[value="' + model.attributes['Extensions'][x].id + '"]')).remove();
                ul.append(FreeswitchConfig.Site.Skin.li.Create({ Attributes: { 'name': model.attributes['Extensions'][x].id }, Content: [
                    model.attibutes['Extensions'][x].id,
                    FreeswitchConfig.Site.Skin.img.Create({ Class: 'button cancel' }),
                    FreeswitchConfig.Site.Skin.img.Create({Class:'arrow_up_down',Attributes:{'style':'cursor:move;'}})
                ]
                }));
                var imgDelete = $(spn.find(FreeswitchConfig.Site.Skin.img.Tag+'.button')[0]);
                imgDelete.bind('click',
                { button: imgDelete, select: inp },
                function(evnt) {
                    var li = $(evnt.data.button.parent());
                    evnt.data.select.append('<option value="' + li.attr('name') + '">' + li.text() + '</option>');
                    $(li.parent()).sortable({
                        axis: 'y',
                        handle: FreeswitchConfig.Site.Skin.img.Tag+':last',
                        placeholder: 'ui-state-highlight',
                        tolerance: 'pointer'
                    });
                    li.remove();
                });
            }
        }
        if (!isCreate) {
            ul.sortable({
                axis: 'y',
                handle: 'img:last',
                placeholder: 'ui-state-highlight',
                tolerance: 'pointer'
            });
        }
        FreeswitchConfig.Site.Modals.ShowFormPanel(
            (isCreate ? 'Add Hunt Group' : 'Edit Hunt Group'),
            frm,
            [
                CreateButton(
                    'accept',
                    'Okay',
                    function(button, pars) {
                        FreeswitchConfig.Site.Modals.ShowUpdating();
                        var number = $(pars.table.find('input[name="number"]')[0]);
                        var RingSequential = pars.table.find('input[name="sequential"]:checked').length > 0;
                        var Extensions = new Array();
                        var ul = $(pars.table.find('ol[name="extensions"]')[0]);
                        for (var y = 0; y < ul.children().length; y++) {
                            Extensions.push(new FreeswitchConfig.Core.Extension.Model({ id: $(ul.children()[y]).attr('name') }));
                        }
                        var canSubmit = FreeswitchConfig.Site.Validation.ValidateRequiredField(number)
                        && FreeswitchConfig.Site.Validation.ValidatePositiveIntegerField(number)
                        && Extensions.length >= 2;
                        if (canSubmit) {
                            model.set({
                                'Number': number.val(),
                                'Context': FreeswitchConfig.CurrentContext,
                                'RingSequential': RingSequential,
                                'Extensions': Extensions
                            });
                            if (model.syncSave()) {
                                if (pars.isCreate) {
                                    pars.collection.add(pars.model);
                                }
                                FreeswitchConfig.Site.Modals.HideUpdating();
                            } else {
                                FreeswitchConfig.Site.Modals.HideUpdating();
                                alert('An error has occured attempting to ' + (pars.isCreate ? 'create' : 'edit') + ' the Hunt Group');
                            }
                        } else {
                            FreeswitchConfig.Site.Modals.HideUpdating();
                            if (Extensions.length < 2) {
                                alert('Please correct all requried fields and ensure that there is at least 2 extensions in the hunt group');
                            } else {
                                alert('Please correct all fields indicated');
                            }
                        }
                    },
                    { table: frm, isCreate: isCreate, collection: collection, model: model }
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
        var col = new FreeswitchConfig.Routes.HuntGroup.Collection();
        var view = new FreeswitchConfig.Routes.HuntGroup.CollectionView({ collection: col, attributes: { cellpadding: 0, cellspacing: 0} });

        var butAdd = CreateButton(
            'zoom_in',
            'Add New Hunt Group',
            function(button, pars) {
                FreeswitchConfig.Site.Modals.ShowLoading();
                FreeswitchConfig.Routes.HuntGroup.GenerateForm(pars.collection);
                FreeswitchConfig.Site.Modals.HideLoading();
            },
            { collection: col });
        container.append(butAdd);

        view.on('render', function() { FreeswitchConfig.Site.Modals.HideLoading(); });
        container.append(view.$el);
        col.fetch();
    }
});