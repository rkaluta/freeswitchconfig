CreateNameSpace('FreeswitchConfig.PBX.PhoneBook');
CreateNameSpace('FreeswitchConfig.PBX.PhoneBookEntry');

FreeswitchConfig.PBX = $.extend(FreeswitchConfig.PBX, {
    PhoneBookEntry: $.extend(FreeswitchConfig.PBX.PhoneBookEntry, {
        PhoneBookEntryTypes: ['GENERAL', 'FRIEND', 'COLLEAGUE', 'FAMILY', 'PERSONAL', 'VIP', 'BLACKLIST', 'OTHER'],
        FormatPhoneNumber: function(number) {
            number = number.replaceAllRegexp(new RegExp('[^0-9]'), '');
            var ret = '';
            if (number.length > 10) {
                ret = number.substring(0, number.length - 10);
                number = number.substring(number.length - 10, 10);
            }
            if (number.length > 7) {
                ret += '(' + number.substring(0, 3) + ') ';
                number = number.substring(3);
            }
            ret += number.substring(0, 3) + '-' + number.substring(3);
            return ret;
        },
        View: Backbone.View.extend({
            initialize: function() {
                this.model.on('change', this.render, this);
            },
            tagName: FreeswitchConfig.Site.Skin.tr.Tag,
            className: "FreeswitchConfig PBX PhoneBookEntry View",
            render: function() {
                this.$el.html('');
                this.$el.append([
                    FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' LastName', Attributes: { 'style': 'vertical-align:top' }, Content: this.model.get('LastName') }),
                    FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' FirstName', Attributes: { 'style': 'vertical-align:top' }, Content: this.model.get('FirstName') }),
                    FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Number', Attributes: { 'style': 'vertical-align:top' }, Content: FreeswitchConfig.PBX.PhoneBookEntry.FormatPhoneNumber(this.model.get('Number')) }),
                    FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Organization', Attributes: { 'style': 'vertical-align:top' }, Content: this.model.get('Organization') }),
                    FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Type', Attributes: { 'style': 'vertical-align:top' }, Content: this.model.get('Type') }),
                    FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + 'Buttons', Attributes: { 'style': 'vertical-align:top' }, Content: [
                        FreeswitchConfig.Site.Skin.img.Create({ Class: 'button edit phone_edit' }),
                        FreeswitchConfig.Site.Skin.img.Create({ Class: 'button delete phone_delete' })
                    ]})
                ]);
                this.trigger('render', this);
                return this;
            },
            events: {
                'click .button.delete.phone_delete': 'deleteModel',
                'click .button.edit.phone_edit': 'editModel'
            },
            deleteModel: function() {
                FreeswitchConfig.Site.Modals.ShowUpdating();
                this.model.destroy({
                    success: function() { FreeswitchConfig.Site.Modals.HideUpdating(); },
                    error: function() { alert('An error occured attempting to delete the Phone Number.'); FreeswitchConfig.Site.Modals.HideUpdating(); }
                });
            },
            editModel: function() {
                FreeswitchConfig.PBX.PhoneBookEntry.GenerateForm(null, this.model);
            }
        }),
        CollectionView: Backbone.View.extend({
            tagName: FreeswitchConfig.Site.Skin.table.Tag,
            className: "FreeswitchConfig PBX PhoneBookEntry CollectionView " + FreeswitchConfig.Site.Skin.table.Class,
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
                            FreeswitchConfig.Site.Skin.th.Create('Last Name'),
                            FreeswitchConfig.Site.Skin.th.Create('First Name'),
                            FreeswitchConfig.Site.Skin.th.Create('Number'),
                            FreeswitchConfig.Site.Skin.th.Create('Organization'),
                            FreeswitchConfig.Site.Skin.th.Create('Type'),
                            FreeswitchConfig.Site.Skin.th.Create('Actions')
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
                        var vw = new FreeswitchConfig.PBX.PhoneBookEntry.View({ model: this.collection.at(x) });
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
        EntryTypes: [new FreeswitchConfig.Site.Form.SelectValue('GENERAL'), new FreeswitchConfig.Site.Form.SelectValue('FRIEND'), new FreeswitchConfig.Site.Form.SelectValue('COLLEAGUE'), new FreeswitchConfig.Site.Form.SelectValue('FAMILY'), new FreeswitchConfig.Site.Form.SelectValue('PERSONAL'), new FreeswitchConfig.Site.Form.SelectValue('VIP'), new FreeswitchConfig.Site.Form.SelectValue('BLACKLIST'), new FreeswitchConfig.Site.Form.SelectValue('OTHER')],
        GenerateForm: function(collection, model) {
            FreeswitchConfig.Site.Modals.ShowLoading();
            var isCreate = model == undefined;
            model = (isCreate ? new FreeswitchConfig.PBX.PhoneBookEntry.Model() : model);
            var frm = FreeswitchConfig.Site.Form.GenerateForm(
                null,
                [
                    new FreeswitchConfig.Site.Form.FormInput('FirstName', 'text', null, null, 'First Name:', null, model.get('FirstName')),
                    new FreeswitchConfig.Site.Form.FormInput('LastName', 'text', null, null, 'Last Name:', null, model.get('LastName')),
                    new FreeswitchConfig.Site.Form.FormInput('Number', 'text', null, null, 'Number:', { 'max-length': '25' }, model.get('Number')),
                    new FreeswitchConfig.Site.Form.FormInput('Type', 'select', FreeswitchConfig.PBX.PhoneBookEntry.EntryTypes, null, 'Type:', null, model.get('Type')),
                    new FreeswitchConfig.Site.Form.FormInput('Organization', 'text', null, null, 'Organization:', null, model.get('Organization')),
                    new FreeswitchConfig.Site.Form.FormInput('Title', 'text', null, null, 'Title:', null, model.get('Title')),
                    new FreeswitchConfig.Site.Form.FormInput('Email', 'text', null, null, 'Email:', null, model.get('Email')),
                    new FreeswitchConfig.Site.Form.FormInput('BirthDay', 'datetime', null, null, 'BirthDay:', null, model.get('BirthDay')),
                    new FreeswitchConfig.Site.Form.FormInput('Note', 'text', null, null, 'Notes:', null, model.get('Note')),
                    new FreeswitchConfig.Site.Form.FormInput('EditableByUser', 'checkbox', null, null, 'Editable By User:', null, model.get('EditableByUser'))
                ]
            );
            FreeswitchConfig.Site.Modals.ShowFormPanel(
                (isCreate ? 'Add New Phone Entry' : 'Edit Phone Entry'),
                frm,
                [
                    CreateButton(
                        'accept',
                        'Okay',
                        function(button, pars) {
                            FreeswitchConfig.Site.Modals.ShowUpdating();
                            var canSubmit = FreeswitchConfig.Site.Validation.ValidateRequiredField(pars.frm.find('input[name="FirstName"]')[0])
                            && FreeswitchConfig.Site.Validation.ValidateRequiredField(pars.frm.find('input[name="LastName"]')[0])
                            && FreeswitchConfig.Site.Validation.ValidateRequiredField(pars.frm.find('input[name="Number"]')[0]);
                            if (canSubmit) {
                                var attrs = new Object();
                                var inps = pars.frm.find('input,select');
                                for (var x = 0; x < inps.length; x++) {
                                    attrs[$(inps[x]).attr('name')] = ($(inps[x]).val() == '' ? null : $(inps[x]).val());
                                }
                                attrs.EditableByUser = pars.frm.find('input[name="EditableByUser"]:checked').length > 0;
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
                                            alert('An error occured attempted to ' + (options.isCreate ? 'create' : 'update') + ' the Phone Book Entry');
                                        },
                                        isCreate: pars.isCreate,
                                        collection: pars.collection
                                    });
                                }
                            } else {
                                FreeswitchConfig.Site.Modals.HideUpdating();
                                alert('Please fill in the required fields in order to ' + (pars.isCreate ? 'create a new' : 'update the') + ' phone book entry.');
                            }
                        },
                        { frm: frm, isCreate: isCreate, model: model, collection: collection }
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
            var view = new FreeswitchConfig.PBX.PhoneBookEntry.CollectionView({ collection: FreeswitchConfig.PBX.PhoneBookEntry.GetPagedList(null, null, null, null, null, 0, 10) });
            view.on('render', function() {
                if (this.$el.find(FreeswitchConfig.Site.Skin.thead.Tag + '>' + FreeswitchConfig.Site.Skin.tr.Tag + '[name="searchFields"]').length == 0) {
                    var thead = $(this.$el.find(FreeswitchConfig.Site.Skin.thead.Tag)[0]);
                    thead.append([
                        FreeswitchConfig.Site.Skin.tr.Create({Attributes:{'name':'searchFields'},Content:[
                            FreeswitchConfig.Site.Skin.td.Create('<input type="text" name="pbeLastName"/>'),
                            FreeswitchConfig.Site.Skin.td.Create('<input type="text" name="pbeLastName"/>'),
                            FreeswitchConfig.Site.Skin.td.Create('<input type="text" name="pbeNumber" max-length="25"/>'),
                            FreeswitchConfig.Site.Skin.td.Create('<input type="text" name="pbeOrganization"/>'),
                            FreeswitchConfig.Site.Skin.td.Create('<select name="pbeType"/>')
                        ]}),
                        FreeswitchConfig.Site.Skin.tr.Create([
                            FreeswitchConfig.Site.Skin.td.Create({Attributes:{'colspan':'4','style':'text-align:center'},Content:[
                                FreeswitchConfig.Site.Skin.img.Create({Class:'button control_start_blue',Attributes:{'name':'pbeStart','title':'First Page'}}),
                                FreeswitchConfig.Site.Skin.img.Create({Class:'button control_rewind_blue',Attributes:{'title':'Previous Page'}}),
                                FreeswitchConfig.Site.Skin.span.Create({Attributes:{'style':'margin-left:10px;margin-right:10px;'},Content:[
                                    'PAGE:<select name="pbePageNum" style="margin-right:10px"/>',
                                    FreeswitchConfig.Site.Skin.img.Create({Class:'button arrow_refresh',Attributes:{'title':'Filter'}}),
                                    FreeswitchConfig.Site.Skin.img.Create({Class:'button cancel',Attributes:{'title':'Clear Filter'}})
                                ]}),
                                FreeswitchConfig.Site.Skin.img.Create({Class:'button control_fastforward_blue'}),
                                FreeswitchConfig.Site.Skin.img.Create({Class:'button control_end_blue'})
                            ]}),
                            FreeswitchConfig.Site.Skin.td.Crreat({Attributes:{'colspan':'2','style':'text-align:right'},Content:'RESULTS PER PAGE: <select name="pbeNumResults"><option value="10">10</option><option value="20">20</option><option value="30">30</option></select>'})
                        ])
                    ]);

                    //bind move to end button
                    var but = $(thead.find('img:last')[0]);
                    but.bind('click',
                    { view: this },
                    function(evnt) {
                        FreeswitchConfig.Site.Modals.ShowLoading();
                        evnt.data.view.collection.MoveToPage(evnt.data.view.collection.TotalPages);
                    });
                    //bind move to next page button
                    but = $(but.prev());
                    but.bind('click',
                    { view: this },
                    function(evnt) {
                        FreeswitchConfig.Site.Modals.ShowLoading();
                        evnt.data.view.collection.MoveToNextPage();
                    });
                    //bind move to first page button
                    but = $(thead.find('img:first')[0]);
                    but.bind('click',
                    { view: this },
                    function(evnt) {
                        FreeswitchConfig.Site.Modals.ShowLoading();
                        evnt.data.view.collection.MoveToPage(0);
                    });
                    //bind move to previous page button
                    but = $(but.next());
                    but.bind('click',
                    { view: this },
                    function(evnt) {
                        FreeswitchConfig.Site.Modals.ShowLoading();
                        evnt.data.view.collection.MoveToPreviousPage();
                    });
                    //bind clear filter button
                    but = $(thead.find('span:last > img:last')[0]);
                    but.bind('click',
                    { view: this },
                    function(evnt) {
                        FreeswitchConfig.Site.Modals.ShowLoading();
                        var tr = $(evnt.data.view.$el.find('thead>tr[name="searchFields"]')[0]);
                        $(tr.find('input[name="pbeFirstName"]')[0]).val('');
                        $(tr.find('input[name="pbeLastName"]')[0]).val('');
                        $(tr.find('input[name="pbeNumber"]')[0]).val('');
                        $(tr.find('input[name="pbeOrganization"]')[0]).val('');
                        $(tr.find('select[name="pbeType"]')[0]).val('');
                        $($(tr.next()).find('select[name="pbeNumResults"]')[0]).val('10');
                        evnt.data.view.collection = FreeswitchConfig.PBX.PhoneBookEntry.GetPagedList(null, null, null, null, null, 0, 10);
                        evnt.data.view.initialize();
                    });
                    //bind filder button
                    but = $(but.prev());
                    but.bind('click',
                    { view: this },
                    function(evnt) {
                        FreeswitchConfig.Site.Modals.ShowLoading();
                        var tr = $(evnt.data.view.$el.find('thead>tr[name="searchFields"]')[0]);
                        var searchPars = new Object();
                        searchPars.fname = $(tr.find('input[name="pbeFirstName"]')[0]).val();
                        searchPars.lname = $(tr.find('input[name="pbeLastName"]')[0]).val();
                        searchPars.number = $(tr.find('input[name="pbeNumber"]')[0]).val();
                        searchPars.org = $(tr.find('input[name="pbeOrganization"]')[0]).val();
                        searchPars.type = $(tr.find('select[name="pbeType"]')[0]).val();
                        searchPars.resPerPage = $($(tr.next()).find('select[name="pbeNumResults"]')[0]).val();
                        evnt.data.view.collection = FreeswitchConfig.PBX.PhoneBookEntry.GetPagedList(
                            (searchPars.fname == '' ? null : searchPars.fname),
                            (searchPars.lname == '' ? null : searchPars.lname),
                            (searchPars.number == '' ? null : searchPars.number),
                            (searchPars.org == '' ? null : searchPars.org),
                            (searchPars.type == '' ? null : searchPars.type),
                            0,
                            searchPars.resPerPage);
                        evnt.data.view.initialize();
                    });
                }
                FreeswitchConfig.Site.Modals.HideLoading();
            });
            var butAdd = CreateButton(
                'phone_add',
                'Add Phone Number',
                function(button, pars) {
                    FreeswitchConfig.PBX.PhoneBookEntry.GenerateForm(pars.collection);
                },
                { collection: view.collection }
            );

            container.append(butAdd);

            container.append(FreeswitchConfig.Site.Skin.div.Create({Attributes:{'style':'style="position:fixed;bottom:20px;right:20px;left:230px;top:' + (butAdd.offset().top + butAdd.height() + 10) + 'px;overflow-x:auto;'},Content:view.$el}));
        }
    }),
    PhoneBook: $.extend(FreeswitchConfig.PBX.PhoneBook, {
        PhoneBookSortTypes: ['FirstName', 'LastName', 'EntryType'],
        View: Backbone.View.extend({
            initialize: function() {
                this.model.on('change', this.render, this);
            },
            tagName: FreeswitchConfig.Site.Skin.tr.Tag,
            className: "FreeswitchConfig PBX PhoneBook View",
            render: function() {
                this.$el.html('');
                this.$el.append([
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' Name',Content:this.model.get('Name')}),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' OwningUser',Content:(this.model.attributes['OwningUser']!=null ? this.model.attributes['OwningUser'].id : '')}),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' NumberOfEntries',Content:(this.model.attributes['Entries']==null ? 0 : this.model.attributes['Entries'].length)}),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' NumberOfUsers',Content:(this.model.attributes['AttechedToUsers']==null ? 0 : this.model.attributes['AttachedToUsers'].length)}),
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
                FreeswitchConfig.PBX.PhoneBook.GenerateForm(null, this.model);
            }
        }),
        CollectionView: Backbone.View.extend({
            tagName: FreeswitchConfig.Site.Skin.table.Tag,
            className: "FreeswitchConfig PBX PhoneBook CollectionView "+FreeswitchConfig.Site.Skin.table.Class,
            initialize: function() {
                this.collection.on('reset', this.render, this); this.collection.on('sync',this.render,this);
                this.collection.on('add', this.render, this);
                this.collection.on('remove', this.render, this);
            },
            attributes: { cellspacing: 0, cellpadding: 0 },
            render: function() {
                if (this.$el.find(FreeswitchConfig.Site.Skin.thead.Tag).length==0){
                    this.$el.append([
                        FreeswitchConfig.Site.Skin.thead.Create({Class:this.className+' header',Content:FreeswitchConfig.Site.Skin.tr.Create([
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Name',Content:'Name'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' OwningUser',Content:'Owner'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' NumberOfEntries',Content:'# of Entries'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' NumberOfUsers',Content:'# of Attached Users'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Buttons',Content:'Actions'})
                        ])}),
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
                        var vw = new FreeswitchConfig.PBX.PhoneBook.View({ model: this.collection.at(x) });
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
        SortTypes: [new FreeswitchConfig.Site.Form.SelectValue('FirstName'), new FreeswitchConfig.Site.Form.SelectValue('LastName'), new FreeswitchConfig.Site.Form.SelectValue('EntryType')],
        GenerateForm: function(collection, model) {
            FreeswitchConfig.Site.Modals.ShowLoading();
            var isCreate = model == undefined;
            model = (model == undefined ? new FreeswitchConfig.PBX.PhoneBook.Model() : model);
            var atUsers = new Array();
            if (model.attributes['AttachedToUsers'] != null) {
                for (var x = 0; x < model.attributes['AttachedToUsers'].length; x++) {
                    atUsers.push(model.attributes['AttachedToUsers'][x].id);
                }
            }
            var entries = new Array();
            if (model.attributes['Entries'] != null) {
                for (var x = 0; x < model.attributes['Entries'].length; x++) {
                    entries.push(model.attributes['Entries'][x].id);
                }
            }
            var usrs = FreeswitchConfig.Core.User.SelectList();
            var frm = FreeswitchConfig.Site.Form.GenerateForm(
                null,
                [
                    new FreeswitchConfig.Site.Form.FormInput('Name', 'text', null, null, 'Name:', null, model.get('Name')),
                    new FreeswitchConfig.Site.Form.FormInput('Description', 'textarea', null, null, 'Description:', { rows: '5', cols: '20' }, model.get('Description')),
                    new FreeswitchConfig.Site.Form.FormInput('OwningUser', 'select', [new FreeswitchConfig.Site.Form.SelectValue('')].concat(usrs), null, 'Owner:', null, (model.attributes['OwningUser'] != null ? null : model.attributes['OwningUser'].id)),
                    new FreeswitchConfig.Site.Form.FormInput('AttachedToUsers', 'select', usrs, null, 'Attached To Users:', { multiple: '', rows: '5' }, (isCreate ? null : atUsers)),
                    new FreeswitchConfig.Site.Form.FormInput('NumberSort', 'select', FreeswitchConfig.PBX.PhoneBook.SortTypes, null, 'Sort Attached Numbers:', null, model.get('Order')),
                    new FreeswitchConfig.Site.Form.FormInput('Entries', 'checkbox_list', FreeswitchConfig.PBX.PhoneBookEntry.SelectList(), null, 'Entries:', null, (isCreate ? null : entries))
                ]
            );
            FreeswitchConfig.Site.Modals.ShowFormPanel(
                (isCreate ? 'Add New Phone Book' : 'Edit Phone Book'),
                frm,
                [
                    CreateButton(
                        'accept',
                        'Okay',
                        function(button, pars) {
                            if (!pars.isCreate) {
                                FreeswitchConfig.Site.Modals.ShowUpdating();
                            } else {
                                FreeswitchConfig.Site.Modals.ShowSaving();
                            }
                            var inpName = $(pars.frm.find('input[name="Name"]')[0]);
                            if (!FreeswitchConfig.Site.Validation.ValidateRequiredField(inpName)) {
                                FreeswitchConfig.Site.Modals.HideUpdating();
                                alert('You must supply a Phonebook name.');
                            } else {
                                var attrs = new Object();
                                attrs.Name = inpName.val();
                                if ($(pars.frm.find('textarea[name="Description"]')[0]).val() != '') {
                                    attrs.Description = $(pars.frm.find('textarea[name="Description"]')[0]).val();
                                } else {
                                    attrs.Description = null;
                                }
                                if ($(pars.frm.find('select[name="OwningUser"]')[0]).val() != '') {
                                    attrs.OwningUser = new FreeswitchConfig.Core.User.Model({ id: $(pars.frm.find('select[name="OwningUser"]')[0]).val() });
                                } else {
                                    attrs.OwningUser = null;
                                }
                                attrs.AttachedToUsers = new Array();
                                var sels = $(pars.frm.find('select[name="pbAttachedUsers"] > option:selected'));
                                for (var y = 0; y < sels.length; y++) {
                                    attrs.AttachedToUsers.push(new FreeswitchConfig.Core.User.Model({ id: $(sels[y]).val() }));
                                }
                                attrs.AttachedToUsers = (attrs.AttachedToUsers.length == 0 ? null : attrs.AttachedToUsers);
                                attrs.Entries = new Array();
                                var div = $(pars.frm.find('div[name="Entries"]')[0]);
                                sels = div.find('input:checked');
                                for (var y = 0; y < sels.length; y++) {
                                    attrs.Entries.push(new FreeswitchConfig.PBX.PhoneBookEntry.Model({ id: $(sels[y]).attr('name') }));
                                }
                                attrs.Entries = (attrs.Entries.length == 0 ? null : attrs.Entries);
                                pars.model.set(attrs);
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
                                        alert('An error occured attempted to ' + (options.isCreate ? 'create' : 'update') + ' the Phone Book');
                                    },
                                    isCreate: pars.isCreate,
                                    collection: pars.collection
                                });
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
            container.append(FreeswitchConfig.Site.Skin.h3.Create('SECTION: <select><option value="pb">Phone Books</option><option value="pn">Phone #\'s</option></select>'));
            var subCont = FreeswitchConfig.Site.Skin.div.Create();
            container.append(subCont);
            var sel = $(container.find('select:last')[0]);
            sel.bind('change',
            { sel: sel, cont: subCont },
            function(event) {
                event.data.cont.html('');
                FreeswitchConfig.Site.Modals.ShowLoading();
                if (event.data.sel.val() == 'pb') {
                    var col = new FreeswitchConfig.PBX.PhoneBook.Collection();
                    var view = new FreeswitchConfig.PBX.PhoneBook.CollectionView({ collection: col });
                    view.on('render', function() { FreeswitchConfig.Site.Modals.HideLoading(); });
                    var butAdd = CreateButton(
                        'book_add',
                        'Add Phone Book',
                        function(button, pars) {
                            FreeswitchConfig.PBX.PhoneBook.GenerateForm(pars.collection);
                        },
                        { collection: col }
                    );
                    event.data.cont.append(butAdd);
                    var subContent = FreeswitchConfig.Site.Skin.div.Create({Attributes:{'style':'position:fixed;bottom:20px;right:20px;left:230px;top:' + (butAdd.offset().top + butAdd.height() + 10) + 'px;overflow-x:auto;'}});
                    subContent.append(view.$el);
                    event.data.cont.append(subContent);
                    col.fetch();
                } else {
                    FreeswitchConfig.PBX.PhoneBookEntry.GeneratePage(event.data.cont);
                }
            });
            sel.trigger('change');
        }
    })
});