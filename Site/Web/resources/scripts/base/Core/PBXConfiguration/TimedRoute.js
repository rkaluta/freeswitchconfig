CreateNameSpace('FreeswitchConfig.Routes.TimedRoute');

FreeswitchConfig.Routes.TimedRoute = $.extend(FreeswitchConfig.Routes.TimedRoute, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: FreeswitchConfig.Site.Skin.tbody.Tag,
        className: "FreeswitchConfig Routing TimedRoute View",
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
            var start = this.model.get('Start');
            var end = this.model.get('End');
            if (end == null) {
                end = { Year: null, Month: null, WeekDay: null, DayOfMonth: null, DayOfYear: null, WeekOfYear: null, WeekOfMonth: null, Hour: null, Minute: null, MinuteOfDay: null };
            }
            this.$el.append([
                FreeswitchConfig.Site.Skin.tr.Create([
                    FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Name', Attributes: { 'rowspan': (this.model.get('End') == null ? '2' : '3'), 'style': 'vertical-align:top;' }, Content: this.model.get('Name') }),
                    FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' PerformOnFail', Attributes: { 'colspan': '2', 'style': 'vertical-align:top' }, Content: (this.model.get('PerformOnFail') ? FreeswitchConfig.Site.Skin.img.Create({Class:'tick'}) : '') }),
                    FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Context', Attributes: { 'style': 'vertical-align:top'},Content:this.model.get('RouteContext').get('Name') }),
                    FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' DestinationCondition', Attributes: { 'colspan': '4', 'style': 'vertical-align:top' }, Content: this.model.get('DestinationCondition').Value }),
                    FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Description', Attributes: { 'colspan': '4', 'style': 'vertical-align:top' }, Content: actionText }),
                    FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' buttons', Attributes: { 'style': 'vertical-align:top', 'rowspan': (this.model.get('End') == null ? '2' : '3') }, Content: [
                        FreeswitchConfig.Site.Skin.img.Create({ Class: 'button edit clock_edit' }),
                        FreeswitchConfig.Site.Skin.img.Create({Class:'button delete clock_delete'})
                    ]})
                ]),
                FreeswitchConfig.Site.Skin.tr.Create([
                    FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Start', Content: 'Start' }),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' Year',Content:start.Year}),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' Month',Content:start.Month}),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' Weekday',Content:start.WeekDay}),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' DayOfMonth',Content:(start.DayOfMonth != null ? start.DayOfMonth.toString().ltrim('0') : '')}),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' DayOfYear',Content:(start.DayOfYear!=null ? start.DayOfYear.toString().ltrim('0') : '')}),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' WeekOfYear',Content:(start.WeekOfYear!=null ? start.WeekOfYear.toString().ltrim('0') : '')}),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' WeekOfMonth',Content:(start.WeekOfMonth!=null ? start.WeekOfMonth.toString().ltrim('0') : '')}),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' Hour',Content:(start.Hour!=null ? start.Hour.toString().ltrim('0') : '')}),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' Minute',Content:(start.Minute!=null ? start.Minute.toString().ltrim('0') : '')}),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' MinuteOfDay',Content:(start.MinuteOfDay!=null ? start.MinuteOfDay.toString().ltrim('0') : '')})
                ]),
                FreeswitchConfig.Site.Skin.tr.Create([
                    FreeswitchConfig.Site.Skin.td.Create({ Class: this.className + ' Start', Content: 'End' }),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' Year',Content:end.Year}),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' Month',Content:end.Month}),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' Weekday',Content:end.WeekDay}),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' DayOfMonth',Content:(end.DayOfMonth != null ? end.DayOfMonth.toString().ltrim('0') : '')}),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' DayOfYear',Content:(end.DayOfYear!=null ? end.DayOfYear.toString().ltrim('0') : '')}),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' WeekOfYear',Content:(end.WeekOfYear!=null ? end.WeekOfYear.toString().ltrim('0') : '')}),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' WeekOfMonth',Content:(end.WeekOfMonth!=null ? end.WeekOfMonth.toString().ltrim('0') : '')}),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' Hour',Content:(end.Hour!=null ? end.Hour.toString().ltrim('0') : '')}),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' Minute',Content:(end.Minute!=null ? end.Minute.toString().ltrim('0') : '')}),
                    FreeswitchConfig.Site.Skin.td.Create({Class:this.className+' MinuteOfDay',Content:(end.MinuteOfDay!=null ? end.MinuteOfDay.toString().ltrim('0') : '')})    
                ])
            ]);
            this.$el.attr('name', this.model.id);
            this.trigger('render', this);
            return this;
        },
        events: {
            'click .button.delete.clock_delete': 'deleteModel',
            'click .button.edit.clock_edit': 'editModel'
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
            var frm = FreeswitchConfig.Routes.TimedRoute.GenerateForm(null, this.model);
            FreeswitchConfig.Site.Modals.HideLoading();
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: FreeswitchConfig.Site.Skin.table.Tag,
        className: "FreeswitchConfig Routing TimedRoute CollectionView "+FreeswitchConfig.Site.Skin.table.Class,
        initialize: function() {
            this.collection.on('reset', this.render, this); this.collection.on('sync',this.render,this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        attributes: { cellpadding: 0, cellspacing: 0 },
        render: function() {
            if (this.$el.find(FreeswitchConfig.Site.Skin.thead.Tag).length==0){
                this.$el.append(
                    FreeswitchConfig.Site.Skin.thead.Create([
                        FreeswitchConfig.Site.Skin.tr.Create([
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Name',Attributes:{'rowspan':'3','style':'vertical-align:top'},Content:'Name'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' InterTimeCondition',Attributes:{'colspan':'2'},Content:'Invert'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Context',Content:'Context'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Condition',Attributes:{'colspan':'4'},Content:'Condition'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Description',Attributes:{'colspan':'4'},Content:'Description'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Actions',Attributes:{'rowspan':'3'},Content:'Actions'})
                        ]),
                        FreeswitchConfig.Site.Skin.tr.Create([
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Start',Content:'Start'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Year',Content:'Year'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Month',Content:'Month'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Weekday',Content:'Weekday'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' DayOfMonth',Content:'Day Of Month'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' DayOfYear',Content:'Day Of Year'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' WeekOfYear',Content:'Week Of Year'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' WeekOfMonth',Content:'Week Of Month'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Hour',Content:'Hour'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Minute',Content:'Minute'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' MinuteOfDay',Content:'Minute Of Day'})
                        ]),
                        FreeswitchConfig.Site.Skin.tr.Create([
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' End',Content:'End'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Year',Content:'Year'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Month',Content:'Month'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Weekday',Content:'Weekday'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' DayOfMonth',Content:'Day Of Month'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' DayOfYear',Content:'Day Of Year'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' WeekOfYear',Content:'Week Of Year'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' WeekOfMonth',Content:'Week Of Month'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Hour',Content:'Hour'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' Minute',Content:'Minute'}),
                            FreeswitchConfig.Site.Skin.th.Create({Class:this.className+' MinuteOfDay',Content:'Minute Of Day'})
                        ])
                    ])
                );
            }
            $(this.$el.find(FreeswitchConfig.Site.Skin.tbody.Tag)).remove();
            if (this.collection.length == 0) {
                this.trigger('render', this);
            } else {
                var alt = false;
                for (var x = 0; x < this.collection.length; x++) {
                    var vw = new FreeswitchConfig.Routes.TimedRoute.View({ model: this.collection.at(x) });
                    $(vw.$el.find(FreeswitchConfig.Site.Skin.tr.Tag)).addClass((alt ? FreeswitchConfig.Site.Skin.tr.AltClass : FreeswitchConfig.Site.Skin.tr.Class));
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
    _months: ['', 'January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
    _days: ['', 'Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
    optMonths: function() {
        if (FreeswitchConfig.Routes.TimedRoute._optMonths == undefined) {
            FreeswitchConfig.Routes.TimedRoute._optMonths = new Array();
            for (var y = 0; y < FreeswitchConfig.Routes.TimedRoute._months.length; y++) {
                FreeswitchConfig.Routes.TimedRoute._optMonths.push(
                    new FreeswitchConfig.Site.Form.SelectValue((y == 0 ? '' : y.toFixedStringSize(2)), FreeswitchConfig.Routes.TimedRoute._months[y])
                );
            }
        }
        return FreeswitchConfig.Routes.TimedRoute._optMonths;
    },
    optWeekDays: function() {
        if (FreeswitchConfig.Routes.TimedRoute._optWeekDays == undefined) {
            FreeswitchConfig.Routes.TimedRoute._optWeekDays = new Array();
            for (var y = 0; y < FreeswitchConfig.Routes.TimedRoute._days.length; y++) {
                FreeswitchConfig.Routes.TimedRoute._optWeekDays.push(
                    new FreeswitchConfig.Site.Form.SelectValue((y == 0 ? '' : y.toFixedStringSize(1)), FreeswitchConfig.Routes.TimedRoute._days[y])
                );
            }
        }
        return FreeswitchConfig.Routes.TimedRoute._optWeekDays;
    },
    optDaysOfMonth: function() {
        if (FreeswitchConfig.Routes.TimedRoute._optDaysOfMonth == null) {
            FreeswitchConfig.Routes.TimedRoute._optDaysOfMonth = new Array();
            FreeswitchConfig.Routes.TimedRoute._optDaysOfMonth.push(new FreeswitchConfig.Site.Form.SelectValue(''));
            for (var y = 1; y < 32; y++) {
                FreeswitchConfig.Routes.TimedRoute._optDaysOfMonth.push(
                     new FreeswitchConfig.Site.Form.SelectValue(y.toFixedStringSize(2), y)
                );
            }
        }
        return FreeswitchConfig.Routes.TimedRoute._optDaysOfMonth;
    },
    optDaysOfYear: function() {
        if (FreeswitchConfig.Routes.TimedRoute._optDaysOfYear == null) {
            FreeswitchConfig.Routes.TimedRoute._optDaysOfYear = new Array();
            FreeswitchConfig.Routes.TimedRoute._optDaysOfYear.push(new FreeswitchConfig.Site.Form.SelectValue(''));
            for (var y = 1; y < 366; y++) {
                FreeswitchConfig.Routes.TimedRoute._optDaysOfYear.push(
                    new FreeswitchConfig.Site.Form.SelectValue(y.toFixedStringSize(3), y)
                );
            }
        }
        return FreeswitchConfig.Routes.TimedRoute._optDaysOfYear;
    },
    optWeeksOfYear: function() {
        if (FreeswitchConfig.Routes.TimedRoute._optWeeksOfYear == null) {
            FreeswitchConfig.Routes.TimedRoute._optWeeksOfYear = new Array();
            FreeswitchConfig.Routes.TimedRoute._optWeeksOfYear.push(new FreeswitchConfig.Site.Form.SelectValue(''));
            for (var y = 1; y < 53; y++) {
                FreeswitchConfig.Routes.TimedRoute._optWeeksOfYear.push(new FreeswitchConfig.Site.Form.SelectValue(y.toFixedStringSize(2), y));
            }
        }
        return FreeswitchConfig.Routes.TimedRoute._optWeeksOfYear;
    },
    optWeeksOfMonth: function() {
        if (FreeswitchConfig.Routes.TimedRoute._optWeeksOfMonth == null) {
            FreeswitchConfig.Routes.TimedRoute._optWeeksOfMonth = new Array();
            FreeswitchConfig.Routes.TimedRoute._optWeeksOfMonth.push(new FreeswitchConfig.Site.Form.SelectValue(''));
            for (var y = 1; y < 6; y++) {
                FreeswitchConfig.Routes.TimedRoute._optWeeksOfMonth.push(new FreeswitchConfig.Site.Form.SelectValue(y));
            }
        }
        return FreeswitchConfig.Routes.TimedRoute._optWeeksOfMonth;
    },
    optHours: function() {
        if (FreeswitchConfig.Routes.TimedRoute._optHours == null) {
            FreeswitchConfig.Routes.TimedRoute._optHours = new Array();
            FreeswitchConfig.Routes.TimedRoute._optHours.push(new FreeswitchConfig.Site.Form.SelectValue(''));
            for (var y = 0; y < 23; y++) {
                FreeswitchConfig.Routes.TimedRoute._optHours.push(new FreeswitchConfig.Site.Form.SelectValue(y.toFixedStringSize(2), y));
            }
        }
        return FreeswitchConfig.Routes.TimedRoute._optHours;
    },
    optMinutes: function() {
        if (FreeswitchConfig.Routes.TimedRoute._optHours == null) {
            FreeswitchConfig.Routes.TimedRoute._optHours = new Array();
            FreeswitchConfig.Routes.TimedRoute._optHours.push(new FreeswitchConfig.Site.Form.SelectValue(''));
            for (var y = 0; y < 59; y++) {
                FreeswitchConfig.Routes.TimedRoute._optHours.push(new FreeswitchConfig.Site.Form.SelectValue(y.toFixedStringSize(2), y));
            }
        }
        return FreeswitchConfig.Routes.TimedRoute._optHours;
    },
    GenerateForm: function(collection, model) {
        var isCreate = model == undefined;
        model = (model == undefined ? new FreeswitchConfig.Routes.TimedRoute.Model() : model);
        var frm = FreeswitchConfig.Site.Form.GenerateForm(
            null,
            [
                (isCreate ? new FreeswitchConfig.Site.Form.FormInput('Name', 'text', null, null, 'Name:', null) : new FreeswitchConfig.Site.Form.FormStaticEntry('Name:', model.get('Name'))),
                new FreeswitchConfig.Site.Form.FormStaticEntry('Context:', FreeswitchConfig.Site.CurrentContext.get('Name')),
                new FreeswitchConfig.Site.Form.FormInput('PerformOnFail', 'checkbox', null, null, 'Invert Time Condition:', null, model.get('PerformOnFail')),
                new FreeswitchConfig.Site.Form.FormInput('DestinationCondition', 'textarea', null, null, 'Condition:', { rows: 5, cols: 20 }, model.get('Condition')),
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
                ]),
                new FreeswitchConfig.Site.Form.FormArrayInput('Start', [
                    new FreeswitchConfig.Site.Form.FormInput('sYear', 'text', null, null, 'Year:', { size: '4' }, (isCreate ? null : model.get('Start').Year)),
                    new FreeswitchConfig.Site.Form.FormInput('sMonth', 'select', FreeswitchConfig.Routes.TimedRoute.optMonths(), null, 'Month:', null, (isCreate ? null : model.get('Start').Month)),
                    new FreeswitchConfig.Site.Form.FormInput('sWeekDay', 'select', FreeswitchConfig.Routes.TimedRoute.optWeekDays(), null, 'Week Day:', null, (isCreate ? null : model.get('Start').WeekDay)),
                    new FreeswitchConfig.Site.Form.FormInput('sMonthDay', 'select', FreeswitchConfig.Routes.TimedRoute.optDaysOfMonth(), null, 'Day Of Month:', null, (isCreate ? null : model.get('Start').MonthDay)),
                    new FreeswitchConfig.Site.Form.FormInput('sYearDay', 'select', FreeswitchConfig.Routes.TimedRoute.optDaysOfYear(), null, 'Day Of Year:', null, (isCreate ? null : model.get('Start').YearDay)),
                    new FreeswitchConfig.Site.Form.FormInput('sMonthWeek', 'select', FreeswitchConfig.Routes.TimedRoute.optWeeksOfMonth(), null, 'Week Of Month:', null, (isCreate ? null : model.get('Start').MonthWeek)),
                    new FreeswitchConfig.Site.Form.FormInput('sHour', 'select', FreeswitchConfig.Routes.TimedRoute.optHours(), null, 'Hour:', null, (isCreate ? null : model.get('Start').Hour)),
                    new FreeswitchConfig.Site.Form.FormInput('sMinute', 'select', FreeswitchConfig.Routes.TimedRoute.optMinutes(), null, 'Minute:', null, (isCreate ? null : model.get('Start').Minute)),
                    new FreeswitchConfig.Site.Form.FormInput('sMinuteOfDay', 'text', null, null, 'Minute Of Day:', { size: '4' }, (isCreate ? null : model.get('Start').MinuteOfDay))
                ]),
                new FreeswitchConfig.Site.Form.FormArrayInput('End', [
                    new FreeswitchConfig.Site.Form.FormInput('eYear', 'text', null, null, 'Year:', { size: '4' }, (model.get('End') == null ? null : model.get('End').Year)),
                    new FreeswitchConfig.Site.Form.FormInput('eMonth', 'select', FreeswitchConfig.Routes.TimedRoute.optMonths(), null, 'Month:', null, (model.get('End') == null ? null : model.get('End').Month)),
                    new FreeswitchConfig.Site.Form.FormInput('eWeekDay', 'select', FreeswitchConfig.Routes.TimedRoute.optWeekDays(), null, 'Week Day:', null, (model.get('End') == null ? null : model.get('End').WeekDay)),
                    new FreeswitchConfig.Site.Form.FormInput('eMonthDay', 'select', FreeswitchConfig.Routes.TimedRoute.optDaysOfMonth(), null, 'Day Of Month:', null, (model.get('End') == null ? null : model.get('End').MonthDay)),
                    new FreeswitchConfig.Site.Form.FormInput('eYearDay', 'select', FreeswitchConfig.Routes.TimedRoute.optDaysOfYear(), null, 'Day Of Year:', null, (model.get('End') == null ? null : model.get('End').YearDay)),
                    new FreeswitchConfig.Site.Form.FormInput('eMonthWeek', 'select', FreeswitchConfig.Routes.TimedRoute.optWeeksOfMonth(), null, 'Week Of Month:', null, (model.get('End') == null ? null : model.get('End').MonthWeek)),
                    new FreeswitchConfig.Site.Form.FormInput('eHour', 'select', FreeswitchConfig.Routes.TimedRoute.optHours(), null, 'Hour:', null, (model.get('End') == null ? null : model.get('End').Hour)),
                    new FreeswitchConfig.Site.Form.FormInput('eMinute', 'select', FreeswitchConfig.Routes.TimedRoute.optMinutes(), null, 'Minute:', null, (model.get('End') == null ? null : model.get('End').Minute)),
                    new FreeswitchConfig.Site.Form.FormInput('eMinuteOfDay', 'text', null, null, 'Minute Of Day:', { size: '4' }, (model.get('End') == null ? null : model.get('End').MinuteOfDay))
                ])
            ]
        );
        AttachNPANXXHelpToInput(frm.find('textarea[name="condition"]')[0]);
        var sel = $(frm.find('select[name="Type"]')[0]);
        sel.bind('change',
            { sel: sel },
            function(event) {
                var td = $($(event.data.sel.parent()).parent());
                $(td.find('span')).hide();
                $(td.find('span:first')[0]).show();
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
            (isCreate ? 'Create Timed Route' : 'Edit Timed Route'),
            frm,
            [
                CreateButton(
                    'accept',
                    (isCreate ? 'Create Route' : 'Update Route'),
                    function(button, pars) {
                        if (pars.isCreate) {
                            FreeswitchConfig.Site.Modals.ShowSaving();
                        } else {
                            FreeswitchConfig.Site.Modals.ShowUpdating();
                        }
                        var attrs = new Object();
                        var inps = pars.frm.find('select,input,textarea');
                        var canSubmit = true;
                        for (var x = 0; x < inps.length; x++) {
                            var inp = $(inps[x]);
                            switch (inp.attr('name')) {
                                case 'PerformOnFail':
                                    attrs[inp.attr('name')] = inp.prop('checked');
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
                                case 'sYear':
                                case 'sMonth':
                                case 'sWeekDay':
                                case 'sMonthDay':
                                case 'sYearDay':
                                case 'sMonthWeek':
                                case 'sHour':
                                case 'sMinute':
                                case 'sMinuteOfDay':
                                    if (attrs['Start'] == undefined) {
                                        attrs['Start'] = new Object();
                                    }
                                    attrs['Start'][inp.attr('name').substring(1)] = (inp.val() == '' ? null : inp.val() * 1);
                                    break;
                                case 'eYear':
                                case 'eMonth':
                                case 'eWeekDay':
                                case 'eMonthDay':
                                case 'eYearDay':
                                case 'eMonthWeek':
                                case 'eHour':
                                case 'eMinute':
                                case 'eMinuteOfDay':
                                    if (attrs['End'] == undefined) {
                                        attrs['End'] = new Object();
                                    }
                                    attrs['End'][inp.attr('name').substring(1)] = (inp.val() == '' ? null : inp.val() * 1);
                                    break;
                                default:
                                    if (inp.attr('name') == 'Name') {
                                        if (FreeswitchConfig.Site.Validation.ValidateRequiredField(inp)) {
                                            attrs[inp.attr('name')] = inp.val();
                                        } else {
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
                        if (canSubmit) {
                            attrs.RouteContext = FreeswitchConfig.Site.CurrentContext;
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
                                        alert('An error occured attempted to ' + (options.isCreate ? 'create' : 'update') + ' the timed route');
                                    },
                                    isCreate: pars.isCreate,
                                    collection: pars.collection
                                });
                            }
                        } else {
                            FreeswitchConfig.Site.Modals.HideUpdating();
                            alert('You must correct the highlighted fields in order to ' + (isCreate ? 'create a' : 'update the') + ' timed route');
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
            ]
        );
    },
    GeneratePage: function(container) {
        container = $(container);
        FreeswitchConfig.Site.Modals.ShowLoading();
        var col = new FreeswitchConfig.Routes.TimedRoute.Collection();
        var view = new FreeswitchConfig.Routes.TimedRoute.CollectionView({ collection: col });
        FreeswitchConfig.Site.setChangedDomain({ collection: col }, function(event) { FreeswitchConfig.Site.Modals.ShowLoading(); event.data.collection.fetch(); });
        view.on('render', function() { FreeswitchConfig.Site.Modals.HideLoading(); });
        var butAdd = CreateButton(
            'clock_add',
            'Add New Timed Route',
            function(button, pars) {
                FreeswitchConfig.Site.Modals.ShowLoading();
                FreeswitchConfig.Routes.TimedRoute.GenerateForm(pars.collection);
                FreeswitchConfig.Site.Modals.HideLoading();
            },
            { collection: col }
        );
        container.append(butAdd);
        container.append(view.$el);
        col.fetch();
    }
});