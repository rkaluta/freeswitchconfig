CreateNameSpace('FreeswitchConfig.PBX.CDR');

FreeswitchConfig.PBX.CDR = $.extend(FreeswitchConfig.PBX.CDR, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: FreeswitchConfig.Site.Skin.tr.Tag,
        className: "FreeswitchConfig PBX CDR View",
        render: function() {
            this.$el.html('');
            this.$el.append([
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' CallerIDNumber', Content: this.model.get('CallerIDNumber') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' DestinationNumber', Content: this.model.get('DestinationNumber') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' FormattedBillableDuration', Content: this.model.get('FormattedBillableDuration') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' CallContext', Content: (this.model.get('ClassContext') == null ? '' : this.model.get('CallContext').get('Name')) }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' HangupCause', Content: this.model.get('HangupCause') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' InternalExtension', Content: (this.model.get('InternalExtension') != null ? this.model.get('InternalExtension').get('Number') : 'N/A') }),
                FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Pin', Content: (this.model.get('Pin') == null ? 'N/A' : this.model.get('Pin')) })
            ]);
            this.trigger('render', this);
            return this;
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: FreeswitchConfig.Site.Skin.table.Tag,
        className: "FreeswitchConfig PBX CDR CollectionView " + FreeswitchConfig.Site.Skin.table.Class,
        initialize: function() {
            this.collection.on('reset', this.render, this); this.collection.on('sync',this.render,this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        render: function() {
            if (this.$el.find(FreeswitchConfig.Site.Skin.thead.Tag).length == 0) {
                this.$el.append(
                    FreeswitchConfig.Site.Skin.thead.Create({ Class: this.className + ' header', Content:
                        FreeswitchConfig.Site.Skin.tr.Create({ Content: [
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' CallerIDNumber', Content: 'Caller ID' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' DestinationNumber', Content: 'Destination Number' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' FormattedBillableDuration', Content: 'Billable Duration' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' CallContext', Content: 'Call Context' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' HangupCause', Content: 'Hangup Cause' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' InternalExtension', Content: 'Internal Extension' }),
                            FreeswitchConfig.Site.Skin.th.Create({ Class: this.className + ' Pin', Content: 'Pin' })
                        ]
                        })
                    })
                );
                this.$el.append(FreeswitchConfig.Site.Skin.tbody.Create({ Class: this.className + ' body' }));
            }
            var alt = false;
            el = $(el.find(FreeswitchConfig.Site.Skin.tbody.Tag)[0]);
            el.html('');
            if (this.collection.length = 0) {
                el.append(FreeswitchConfig.Site.Skin.tr.Create({Content:FreeswitchConfig.Site.Skin.td.Create({Attributes:{colspan:'100%'},Content:'No CDR results found.'})}));
            } else {
                for (var x = 0; x < this.collection.length; x++) {
                    var vw = new FreeswitchConfig.PBX.CDR.View({ model: this.collection.at(x) });
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
            this.trigger('render', this);
        },
        changeCollection: function(collection) {
            this.collection = collection;
            this.collection.on('reset', this.render, this); this.collection.on('sync',this.render,this);
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
        var navHolder = FreeswitchConfig.Site.Skin.div.Create({Class:'NavHolder',Attributes:{style:'height:20px'}});
        var resHolder = FreeswitchConfig.Site.Skin.div.Create({Class:'ResultsHolder'});
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