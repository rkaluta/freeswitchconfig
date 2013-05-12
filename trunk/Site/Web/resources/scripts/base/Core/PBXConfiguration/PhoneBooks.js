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
            tagName: "tr",
            className: "FreeswitchConfig PBX PhoneBookEntry View",
            render: function() {
                this.$el.html('');
                this.$el.append('<td class="' + this.className + ' LastName" style="vertical-align:top">' + this.model.get('LastName') + '</td>');
                this.$el.append('<td class="' + this.className + ' FirstName" style="vertical-align:top">' + this.model.get('FirstName') + '</td>');
                this.$el.append('<td class="' + this.className + ' Number" style="vertical-align:top">' + FreeswitchConfig.PBX.PhoneBookEntry.FormatPhoneNumber(this.model.get('Number')) + '</td>');
                this.$el.append('<td class="' + this.className + ' Organization" style="vertical-align:top;">' + (this.model.get('Organization') == null ? '' : this.model.get('Organization')) + '</td>');
                this.$el.append('<td class="' + this.className + ' Type" style="vertical-align:top;">' + this.model.get('Type') + '</td>');
                this.$el.append('<td class="' + this.className + ' Buttons" style="vertical-align:top;"><img class="button edit phone_edit"/><img class="button delete phone_delete"/></td>');
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
            tagName: "table",
            className: "FreeswitchConfig PBX PhoneBookEntry CollectionView",
            initialize: function() {
                this.collection.on('reset', this.render, this);
                this.collection.on('add', this.render, this);
                this.collection.on('remove', this.render, this);
            },
            attributes: { cellspacing: 0, cellpadding: 0 },
            render: function() {
                var el = this.$el;
                if (el.find('thead').length == 0) {
                    var thead = $('<thead class="' + this.className + ' header"></thead>');
                    thead.append('<tr><th>Last Name</th><th>First Name</th><th>Number</th><th>Organization</th><th>Type</th><th>Actions</th></tr>');
                    el.append(thead);
                    el.append('<tbody></tbody>');
                }
                el = $(el.find('tbody')[0]);
                el.html('');
                if (this.collection.length == 0) {
                    this.trigger('render', this);
                } else {
                    var alt = false;
                    for (var x = 0; x < this.collection.length; x++) {
                        var vw = new FreeswitchConfig.PBX.PhoneBookEntry.View({ model: this.collection.at(x) });
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
                if (this.$el.find('thead>tr[name="searchFields"]').length == 0) {
                    var thead = $(this.$el.find('thead')[0]);
                    thead.append('<tr name="searchFields"><td><input type="text" name="pbeLastName"/></td><td><input type="text" name="pbeFirstName"/></td><td><input type="text" name="pbeNumber" max-length="25"/></td><td><input type="text" name="pbeOrganization"/></td><td><select name="pbeType"></select></td><td>&nbsp;</td></tr>');
                    thead.append('<tr><td colspan="4" style="text-align:center;"><img class="Button control_start_blue" name="pbeStart" title="First Page"/><img class="Button control_rewind_blue" title="Previous Page"/>' +
                                '<span style="margin-left:10px;margin-right:10px;">PAGE:<select name="pbePageNum" style="margin-right:10px;"></select><img class="Button arrow_refresh" Title="Filter"/><img class="Button cancel" title="Clear Filter"/></span>' +
                                '<img class="Button control_fastforward_blue"/><img class="Button control_end_blue"/></td><td colspan="2" style="text-align:right">RESULTS PER PAGE: <select name="pbeNumResults"><option value="10">10</option><option value="20">20</option><option value="30">30</option></select></td></tr>');

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

            var subContent = $('<div style="position:fixed;bottom:20px;right:20px;left:230px;top:' + (butAdd.offset().top + butAdd.height() + 10) + 'px;overflow-x:auto;"></div>');
            container.append(subContent);
            subContent.append(view.$el);
        }
    }),
    PhoneBook: $.extend(FreeswitchConfig.PBX.PhoneBook, {
        PhoneBookSortTypes: ['FirstName', 'LastName', 'EntryType'],
        View: Backbone.View.extend({
            initialize: function() {
                this.model.on('change', this.render, this);
            },
            tagName: "tr",
            className: "FreeswitchConfig PBX PhoneBook View",
            render: function() {
                this.$el.html('');
                this.$el.append('<td class="' + this.className + ' Name">' + this.model.get('Name') + '</td>');
                this.$el.append('<td class="' + this.className + ' OwningUser">' + (this.model.attributes['OwningUser'] != null ? this.model.attributes['OwningUser'].id : '') + '</td>');
                this.$el.append('<td class="' + this.className + ' NumberOfEntries">' + (this.model.attributes['Entries'] == null ? 0 : this.model.attributes['Entries'].length).toString() + '</td>');
                this.$el.append('<td class="' + this.className + ' NumberOfUsers">' + (this.model.attributes['AttachedToUsers'] == null ? 0 : this.model.attributes['AttachedToUsers'].length).toString() + '</td>');
                this.$el.append('<td class="' + this.className + ' Buttons" style="vertical-align:top;"><img class="button edit sound_edit"/><img class="button delete sound_delete"/></td>');
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
            tagName: "table",
            className: "FreeswitchConfig PBX PhoneBook CollectionView",
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
                thead.append('<tr><th class="' + this.className + ' Name">Name</th>' +
                '<th class="' + this.className + ' OwningUser">Owner</th>' +
                '<th class="' + this.className + ' NumberOfEntries"># of Entries</th>' +
                '<th class="' + this.className + ' NumberOfUsers"># of Attached Users</th>' +
                '<th class="' + this.className + ' Buttons">Actions</th></tr>');
                el.append('<tbody></tbody>');
                el = $(el.children()[0]);
                if (this.collection.length == 0) {
                    this.trigger('render', this);
                } else {
                    var alt = false;
                    for (var x = 0; x < this.collection.length; x++) {
                        var vw = new FreeswitchConfig.PBX.PhoneBook.View({ model: this.collection.at(x) });
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
            container.append('<h3>SECTION: <select><option value="pb">Phone Books</option><option value="pn">Phone #\'s</option></select></h3>');
            var subCont = $('<div></div>');
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
                    var subContent = $('<div style="position:fixed;bottom:20px;right:20px;left:230px;top:' + (butAdd.offset().top + butAdd.height() + 10) + 'px;overflow-x:auto;"></div>');
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