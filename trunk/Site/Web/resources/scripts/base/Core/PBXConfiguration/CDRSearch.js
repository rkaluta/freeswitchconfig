CreateNameSpace('FreeswitchConfig.PBX.CDR');

FreeswitchConfig.PBX.CDR = $.extend(FreeswitchConfig.PBX.CDR, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: "tr",
        className: "FreeswitchConfig PBX CDR View",
        render: function() {
            this.$el.append('<td class="' + this.className + ' CallerIDNumber">' + this.model.get('CallerIDNumber') + '</td>');
            this.$el.append('<td class="' + this.className + ' DestinationNumber">' + this.model.get('DestinationNumber') + '</td>');
            this.$el.append('<td class="' + this.className + ' FormattedBillableDuration">' + this.model.get('FormattedBillableDuration') + '</td>');
            this.$el.append('<td class="' + this.className + ' CallContext">' + (this.model.get('CallContext')==null ? '' : this.model.get('CallContext').get('Name')) + '</td>');
            this.$el.append('<td class="' + this.className + ' HangupCause">' + this.model.get('HangupCause') + '</td>');
            this.$el.append('<td class="' + this.className + ' InternalExtension">' + (this.model.get('InternalExtension') != null ? this.model.get('InternalExtension').get('Number') : 'N/A') + '</td>');
            this.$el.append('<td class="' + this.className + ' Pin">' + (this.model.get('Pin') == null ? 'N/A' : this.model.get('Pin')) + '</td>');
            this.trigger('render', this);
            return this;
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: "table",
        className: "FreeswitchConfig PBX CDR CollectionView",
        initialize: function() {
            this.collection.on('reset', this.render, this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        render: function() {
            var el = this.$el;
            if (el.find('thead').length == 0) {
                el.append($('<thead class="' + this.className + ' header"><tr></tr></thead>'));
                var tr = $(el.find('tr')[0]);
                tr.append('<th class="' + this.className + ' CallerIDNumber">Caller ID</th>');
                tr.append('<th class="' + this.className + ' DestinationNumber">Destination Number</th>');
                tr.append('<th class="' + this.className + ' FormattedBillableDuration">Billable Duration</th>');
                tr.append('<th class="' + this.className + ' CallContext">Call Context</th>');
                tr.append('<th class="' + this.className + ' HangupCause">Hangup Cause</th>');
                tr.append('<th class="' + this.className + ' InternalExtension">Internal Extension</th>');
                tr.append('<th class="' + this.className + ' Pin">Pin</th>');
                el.append($('<tbody class="' + this.className + ' body"></tbody>'));
            }
            var alt = false;
            el = $(el.find('tbody')[0]);
            el.html('');
            if (this.collection.length = 0) {
                el.append('<tr><td colspan="100%">No CDR results found.</td></tr>');
            } else {
                for (var x = 0; x < this.collection.length; x++) {
                    var vw = new FreeswitchConfig.PBX.CDR.View({ model: this.collection.at(x) });
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
            this.trigger('render', this);
        },
        changeCollection: function(collection) {
            this.collection = collection;
            this.collection.on('reset', this.render, this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
            collection.fetch();
        }
    }),
    GeneratePage: function(container) {
        container = $(container);
        FreeswitchConfig.Site.Modals.ShowLoading();
        var cdrView = new FreeswitchConfig.PBX.CDR.CollectionView(
            { collection: new FreeswitchConfig.PBX.CDR.Collection({ TotalPages: 0 }) }
        );

        var opts = FreeswitchConfig.Core.Extension.SelectList(); ;
        opts = Array.concat([new FreeswitchConfig.Site.Form.SelectValue('')], opts);

        var tblSearch = FreeswitchConfig.Site.Form.GenerateForm(
            null,
            [
                new FreeswitchConfig.Site.Form.FormInput('extension', 'select', opts, null, 'Extension:'),
                new FreeswitchConfig.Site.Form.FormInput('callerID', 'text', null, null, 'Caller ID:'),
                new FreeswitchConfig.Site.Form.FormInput('destination', 'text', null, null, 'Destination:'),
                new FreeswitchConfig.Site.Form.FormInput('callerName', 'text', null, null, 'Caller Name:'),
                new FreeswitchConfig.Site.Form.FormInput('startDate', 'datetime', null, null, 'Start Date:'),
                new FreeswitchConfig.Site.Form.FormInput('endDate', 'datetime', null, null, 'End Date:'),
                new FreeswitchConfig.Site.Form.FormInput('resPerPage', 'select', [
                    new FreeswitchConfig.Site.Form.SelectValue('10'),
                    new FreeswitchConfig.Site.Form.SelectValue('25'),
                    new FreeswitchConfig.Site.Form.SelectValue('50')
                ], null, 'Results Per Page:'),
                new FreeswitchConfig.Site.Form.FormInput('curPage', 'hidden'),
                new FreeswitchConfig.Site.Form.FormInput('maxPage', 'hidden'),
                new FreeswitchConfig.Site.Form.FormInput('curPars', 'hidden')
            ],
            [
                new FreeswitchConfig.Site.Form.FormButton(
                    'book_open',
                    'Search',
                    function(tbl, pars) {
                        FreeswitchConfig.Site.Modals.ShowLoading();
                        var searchPars = new Object();
                        searchPars.ext = $(tbl.find('input[name="extension"]')[0]).val();
                        searchPars.callID = $(tbl.find('input[name="callerID"]')[0]).val();
                        searchPars.destination = $(tbl.find('input[name="destination"]')[0]).val();
                        searchPars.callerName = $(tbl.find('input[name="callerName"]')[0]).val();
                        var dHolder = $(tbl.find('input[name="startDate"]')[0]);
                        searchPars.startDate = (dHolder.val() == '' ? null : FreeswitchConfig.Site.DateTimePicker.GetDateValueFromField(dHolder));
                        dHolder = $(tbl.find('input[name="endDate"]')[0]);
                        searchPars.endDate = (dHolder.val() == '' ? null : FreeswitchConfig.Site.DateTimePicker.GetDateValueFromField(dHolder));
                        searchPars.resPerPage = $(tbl.find('select[name="resPerPage"]')[0]).val();
                        pars.cdrView.changeCollection(
                            FreeswitchConfig.PBX.CDR.SearchCDRs(
                                (searchPars.ext == '' ? null : searchPars.ext),
                                (searchPars.callID == '' ? null : searchPars.callID),
                                (searchPars.destination == '' ? null : searchPars.destination),
                                (searchPars.callerName == '' ? null : searchPars.callerName),
                                searchPars.startDate,
                                searchPars.endDate,
                                0,
                                searchPars.resPerPage
                            )
                        );
                    },
                    { cdrView: cdrView }
                )
            ]
        );
        var navHolder = $('<div name="NavHolder" style="height:20px;"></div>');
        var resHolder = $('<div name="ResultsHolder"></div>');
        container.append(tblSearch);
        container.append(navHolder);
        navHolder.append('<center>' +
            '<img title="First" class="Button double_arrow_left" style="display:none;"/> ' +
            '<img  title="Previous" class="Button arrow_left" style="display:none;"/> ' +
            '<span name="CurPageDisplay" style="font-weight:bold;font-size:20;" style="display:none;"></span>' +
            '<img title="Next" class="Button arrow_right" style="display:none;"/> ' +
            '<img title="Last" class="Button double_arrow_right" style="display:none;"/> ' +
            '</center>');

        cdrView.on('render', { navbar: navHolder },
        function(view, pars) {
            $(pars.navbar.find('img.Button')).unbind('click');
            $(pars.navbar.find('span[name="CurPageDisplay"]')[0]).html('');
            $(pars.navbar.find('img.Button')).hide();
            if (view.collection.TotalPages >= 0) {
                var buttons = pars.navbar.find('img.Button');
                $(pars.navbar.find('span[name="CurPageDisplay"]')[0]).html(Math.floor(view.collection.currentIndex / view.collection.currentPageSize) + 1);
                if (view.collection.currentIndex > 0) {
                    $(buttons[0]).bind('click',
                    { view: view },
                    function(event) {
                        event.data.view.collection.MoveToPage(0);
                    });
                    $(buttons[1]).bind('click',
                    { view: view },
                    function(event) {
                        event.data.view.collection.MoveToPage(Math.floor(event.data.view.collection.currentIndex / event.data.view.collection.currentPageSize) - 1);
                    });
                }
                if (Math.floor(view.collection.currentIndex / view.collection.currentPageSize) < view.collection.TotalPages) {
                    $(buttons[2]).bind('click',
                    { view: view },
                    function(event) {
                        event.data.view.collection.MoveToPage(event.data.view.collection.TotalPages - 1);
                    });
                    $(buttons[3]).bind('click',
                    { view: view },
                    function(event) {
                        event.data.view.collection.MoveToPage(Math.floor(event.data.view.collection.currentIndex / event.data.view.collection.currentPageSize) + 1);
                    });
                }
            }
        });

        container.append(resHolder);
        resHolder.append(cdrView.$el);
        FreeswitchConfig.Site.Modals.HideLoading();
    }
});