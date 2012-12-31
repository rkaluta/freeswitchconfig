FreeswitchConfig.Site = $.extend(FreeswitchConfig.Site,{DateTimePicker : {
    __dAbbr: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
    __dNames: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
    __mAbbr: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
    __mNames: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
    __baseHTML: '<div aria-live="off" id="DateCal" style="width: 424px; height: 281px; display: none; position: absolute;" class="AnyTime-win AnyTime-pkr ui-widget ui-widget-content ui-corner-all">' +
	'<h5 class="AnyTime-hdr ui-widget-header ui-corner-top"><div class="AnyTime-x-btn ui-state-default" name="closeButton">X</div>Select a Date and Time</h5>' +
	'<div style="width: 408px; height: 241px;" class="AnyTime-body"><div style="width: 188px; height: 238px;" class="AnyTime-date"><h6 class="AnyTime-lbl AnyTime-lbl-yr">Year</h6>' +
	'<input type="hidden" name="hidDateValue"/>' +
	'<ul class="AnyTime-yrs ui-helper-reset" name="YearList">' +
	'<li class="AnyTime-btn AnyTime-yrs-past-btn ui-state-default">&lt;</li>' +
	'<li class="AnyTime-btn AnyTime-yr-prior-btn ui-state-default"></li>' +
	'<li class="AnyTime-btn AnyTime-yr-cur-btn AnyTime-cur-btn AnyTime-focus-btn ui-state-default ui-state-highlight ui-state-focus"></li>' +
	'<li class="AnyTime-btn AnyTime-yr-next-btn ui-state-default"></li>' +
	'<li class="AnyTime-btn AnyTime-yrs-ahead-btn ui-state-default">&gt;</li></ul>' +
	'<h6 class="AnyTime-lbl AnyTime-lbl-month" style="">Month</h6><ul class="AnyTime-mons" name="months"></ul>' +
	'<h6 class="AnyTime-lbl AnyTime-lbl-dom" style="">Day of Month</h6>' +
	'<table cellspacing="0" cellpadding="0" border="0" class="AnyTime-dom-table"><thead class="AnyTime-dom-head"><tr class="AnyTime-dow"><th class="AnyTime-dow AnyTime-dow1">Sun</th><th class="AnyTime-dow AnyTime-dow2">Mon</th><th class="AnyTime-dow AnyTime-dow3">Tue</th><th class="AnyTime-dow AnyTime-dow4">Wed</th><th class="AnyTime-dow AnyTime-dow5">Thu</th><th class="AnyTime-dow AnyTime-dow6">Fri</th><th class="AnyTime-dow AnyTime-dow7">Sat</th></tr></thead>' +
	'<tbody class="AnyTime-dom-body" name="dayOfWeek"></tbody></table></div>' +
	'<div style="width: 199px; height: 241px;" class="AnyTime-time"><div class="AnyTime-hrs"><h6 class="AnyTime-lbl AnyTime-lbl-hr">Hour</h6>' +
	'<ul class="AnyTime-hrs-am" name="amHours"></ul>' +
	'<ul class="AnyTime-hrs-pm" name="pmHours"></ul>' +
	'</div><div class="AnyTime-mins"><h6 class="AnyTime-lbl AnyTime-lbl-min">Minute</h6>' +
	'<ul class="AnyTime-mins-tens" name="minTens"></ul>' +
	'<ul class="AnyTime-mins-ones" name="minNines"></ul>' +
	'</div><div class="AnyTime-secs"><h6 class="AnyTime-lbl AnyTime-lbl-sec">Second</h6>' +
	'<ul class="AnyTime-secs-tens" name="secTens"></ul>' +
	'<ul class="AnyTime-secs-ones" name="secNines"></ul></div></div></div>' +
	'<div style="position: absolute; top: 0px; left: 0px; height: 287px; width: 428px; display: none;" class="AnyTime-cloak"></div><div style="position: absolute; width: 171px; height: 232px; top: 30.4px; left: 18.5px; display: none;" class="AnyTime-win AnyTime-yr-selector ui-widget ui-widget-content ui-corner-all"><h5 class="AnyTime-hdr AnyTime-hdr-yr-selector ui-widget-header ui-corner-top"><div class="AnyTime-x-btn ui-state-default">X</div>Year</h5><div class="AnyTime-body AnyTime-body-yr-selector" style="width: 151px; height: 210px;"><ul class="AnyTime-yr-mil"><li class="AnyTime-btn AnyTime-mil-btn AnyTime-mil0-btn ui-state-default">0</li><li class="AnyTime-btn AnyTime-mil-btn AnyTime-mil1-btn ui-state-default">1</li><li class="AnyTime-btn AnyTime-mil-btn AnyTime-mil2-btn AnyTime-cur-btn ui-state-default ui-state-highlight">2</li><li class="AnyTime-btn AnyTime-mil-btn AnyTime-mil3-btn ui-state-default">3</li><li class="AnyTime-btn AnyTime-mil-btn AnyTime-mil4-btn ui-state-default">4</li><li class="AnyTime-btn AnyTime-mil-btn AnyTime-mil5-btn ui-state-default">5</li><li class="AnyTime-btn AnyTime-mil-btn AnyTime-mil6-btn ui-state-default">6</li><li class="AnyTime-btn AnyTime-mil-btn AnyTime-mil7-btn ui-state-default">7</li><li class="AnyTime-btn AnyTime-mil-btn AnyTime-mil8-btn ui-state-default">8</li><li class="AnyTime-btn AnyTime-mil-btn AnyTime-mil9-btn ui-state-default">9</li></ul><ul class="AnyTime-yr-cent"><li class="AnyTime-btn AnyTime-cent-btn AnyTime-cent0-btn AnyTime-cur-btn ui-state-default ui-state-highlight">0</li><li class="AnyTime-btn AnyTime-cent-btn AnyTime-cent1-btn ui-state-default">1</li><li class="AnyTime-btn AnyTime-cent-btn AnyTime-cent2-btn ui-state-default">2</li><li class="AnyTime-btn AnyTime-cent-btn AnyTime-cent3-btn ui-state-default">3</li><li class="AnyTime-btn AnyTime-cent-btn AnyTime-cent4-btn ui-state-default">4</li><li class="AnyTime-btn AnyTime-cent-btn AnyTime-cent5-btn ui-state-default">5</li><li class="AnyTime-btn AnyTime-cent-btn AnyTime-cent6-btn ui-state-default">6</li><li class="AnyTime-btn AnyTime-cent-btn AnyTime-cent7-btn ui-state-default">7</li><li class="AnyTime-btn AnyTime-cent-btn AnyTime-cent8-btn ui-state-default">8</li><li class="AnyTime-btn AnyTime-cent-btn AnyTime-cent9-btn ui-state-default">9</li></ul><ul class="AnyTime-yr-dec"><li class="AnyTime-btn AnyTime-dec-btn AnyTime-dec0-btn ui-state-default">0</li><li class="AnyTime-btn AnyTime-dec-btn AnyTime-dec1-btn AnyTime-cur-btn ui-state-default ui-state-highlight">1</li><li class="AnyTime-btn AnyTime-dec-btn AnyTime-dec2-btn ui-state-default">2</li><li class="AnyTime-btn AnyTime-dec-btn AnyTime-dec3-btn ui-state-default">3</li><li class="AnyTime-btn AnyTime-dec-btn AnyTime-dec4-btn ui-state-default">4</li><li class="AnyTime-btn AnyTime-dec-btn AnyTime-dec5-btn ui-state-default">5</li><li class="AnyTime-btn AnyTime-dec-btn AnyTime-dec6-btn ui-state-default">6</li><li class="AnyTime-btn AnyTime-dec-btn AnyTime-dec7-btn ui-state-default">7</li><li class="AnyTime-btn AnyTime-dec-btn AnyTime-dec8-btn ui-state-default">8</li><li class="AnyTime-btn AnyTime-dec-btn AnyTime-dec9-btn ui-state-default">9</li></ul><ul class="AnyTime-yr-yr"><li class="AnyTime-btn AnyTime-yr-btn AnyTime-yr0-btn ui-state-default">0</li><li class="AnyTime-btn AnyTime-yr-btn AnyTime-yr1-btn ui-state-default">1</li><li class="AnyTime-btn AnyTime-yr-btn AnyTime-yr2-btn ui-state-default">2</li><li class="AnyTime-btn AnyTime-yr-btn AnyTime-yr3-btn ui-state-default">3</li><li class="AnyTime-btn AnyTime-yr-btn AnyTime-yr4-btn ui-state-default">4</li><li class="AnyTime-btn AnyTime-yr-btn AnyTime-yr5-btn ui-state-default">5</li><li class="AnyTime-btn AnyTime-yr-btn AnyTime-yr6-btn ui-state-default">6</li><li class="AnyTime-btn AnyTime-yr-btn AnyTime-yr7-btn ui-state-default">7</li><li class="AnyTime-btn AnyTime-yr-btn AnyTime-yr8-btn AnyTime-cur-btn ui-state-default ui-state-highlight">8</li><li class="AnyTime-btn AnyTime-yr-btn AnyTime-yr9-btn ui-state-default">9</li></ul><ul class="AnyTime-yr-era"><li class="AnyTime-btn AnyTime-era-btn AnyTime-bce-btn ui-state-default">BCE</li><li class="AnyTime-btn AnyTime-era-btn AnyTime-ce-btn AnyTime-cur-btn ui-state-default ui-state-highlight">CE</li></ul></div></div></div>',
    __baseOptions: {
        button: null,
        format: '%d-%m-%Y %H:%i:%s'
    },
    _curInput: null,
    _curOptions: null,
    AttachToInput: function(input, options) {
        var tmp = clone(this.__baseOptions);
        if (options == null) {
            options = tmp;
        } else {
            options = $.extend(tmp, options);
        }
        if ((options.initialValue == undefined) || (options.initialValue == null)) {
            options.initialValue = new Date();
        } else {
            this.SetDateValueOnField(input, options.initialValue, options);
        }
        if (options.button == null) {
            options.button = $(input);
        }
        $(input).attr('actualdate', options.initialValue.toString());
        $(options.button).bind('click',
	    { button: $(input), options: options },
	    function(event) {
	        $('#DateCal').hide();
	        var but = $(event.data.button);
	        if ((event.data.options != null) && (event.data.options != undefined)) {
	            if ((event.data.options.button != undefined) && (event.data.options.button != null)) {
	                but = $(event.data.options.button);
	            }
	        }
	        FreeswitchConfig.Site.DateTimePicker._curInput = event.data.button;
	        FreeswitchConfig.Site.DateTimePicker._curOptions = event.data.options;
	        FreeswitchConfig.Site.DateTimePicker.SetCurrentDateTime(FreeswitchConfig.Site.DateTimePicker.GetDateValueFromField(event.data.button));
	        var top = but.offset().top + (but.height() / 2);
	        if (top + 281 > $(document.body).height()) {
	            top = top - (5 + ((top + 281) - $(document.body).height()));
	        }
	        $('#DateCal').css('top', top + 'px');
	        $('#DateCal').css('left', (but.offset().left + (but.width() / 2)) + 'px');
	        $('#DateCal').show();
	    });
    },
    GetDateValueFromField: function(input) {
        return new Date($(input).attr('actualdate'));
    },
    SetDateValueOnField: function(input, date, options) {
        if (date == undefined) {
            date = FreeswitchConfig.Site.DateTimePicker.CurrentDateValue();
        }
        if (options == undefined) {
            options = this._curOptions;
        }
        if (options == undefined || options == null) {
            options = this.__baseOptions;
        }
        $(input).attr('actualdate', date.toString());
        $(input).val(this.format(date, options.format));
    },
    CurrentDateValue: function() {
        return new Date($($('#DateCal').find('input[name="hidDateValue"]')[0]).val());
    },
    CloseCalendar: function() {
        if (FreeswitchConfig.Site.DateTimePicker._curInput != null) {
            FreeswitchConfig.Site.DateTimePicker.SetDateValueOnField(FreeswitchConfig.Site.DateTimePicker._curInput);
            FreeswitchConfig.Site.DateTimePicker._curInput.val(FreeswitchConfig.Site.DateTimePicker.format(FreeswitchConfig.Site.DateTimePicker.CurrentDateValue(), FreeswitchConfig.Site.DateTimePicker._curOptions.format));
            $('#DateCal').hide();
        }
    },
    formatNumWithZero: function(number) {
        return (number <= 9 ? '0' + number.toString() : number.toString());
    },
    format: function(datetime, curform) {
        if ((curform == null) || (curform == undefined)) {
            curform = this.__baseOptions.format;
        }
        var ret = '';
        for (var y = 0; y < curform.length; y++) {
            if (curform[y] == '\\' && y + 1 < curform.length && curform[y + 1] == '%') {
                ret += '%';
            } else if (curform[y] == '%') {
                y += 1;
                switch (curform[y]) {
                    case 'a':
                        ret += this.__dAbbr[datetime.getDay()];
                        break;
                    case 'b':
                        ret += this.__mAbbr[datetime.getMonth()];
                        break;
                    case 'c':
                        ret += (datetime.getMonth() + 1).toString();
                        break;
                    case 'd':
                        ret += this.formatNumWithZero(datetime.getDate());
                        break;
                    case 'H':
                        ret += this.formatNumWithZero(datetime.getHours());
                        break;
                    case 'h':
                        ret += this.formatNumWithZero((datetime.getHours() > 12 ? datetime.getHours() - 12 : datetime.getHours()));
                        break;
                    case 'i':
                        ret += this.formatNumWithZero(datetime.getMinutes());
                        break;
                    case 'M':
                        ret += this.__mNames[datetime.getMonth()];
                        break;
                    case 'm':
                        ret += this.formatNumWithZero(datetime.getMonth()+1);
                        break;
                    case 'p':
                        ret += (datetime.getHours() > 12 ? 'PM' : 'AM');
                        break;
                    case 'r':
                        ret += this.formatNumWithZero((datetime.getHours() > 12 ? datetime.getHours() - 12 : datetime.getHours())) + ':' + this.formatNumWithZero(datetime.getMinutes()) + ':' + this.formatNumWithZero(datetime.getSeconds()) + ' ' + (datetime.getHours() > 12 ? 'PM' : 'AM');
                        break;
                    case 'W':
                        ret += this.__dNames[datetime.getDay()];
                        break;
                    case 'w':
                        ret += datetime.getDay().toString();
                        break;
                    case 'Y':
                        ret += datetime.getFullYear().toString();
                        break;
                    case 'y':
                        ret += datetime.getFullYear().toString().substring(2);
                        break;
                    case 's':
                        ret += this.formatNumWithZero(datetime.getSeconds());
                        break;
                }
            } else {
                ret += curform[y];
            }
        }
        return ret;
    },
    SetCurrentDateTime: function(datetime) {
        var dateCal = $('#DateCal');
        if (dateCal.length == 0) {
            dateCal = $(this.__baseHTML);
            $(document.body).append(dateCal);
            $(dateCal.find('div[name="closeButton"]')[0]).bind('click', FreeswitchConfig.Site.DateTimePicker.CloseCalendar);
            dateCal.bind('mouseenter',
            function() {
                dateCal.bind('mouseleave',
                function() {
                    dateCal.unbind('mouseleave');
                    FreeswitchConfig.Site.DateTimePicker.CloseCalendar();
                });
            });
            var ul = $(dateCal.find('ul[name="months"]')[0]);
            for (var y = 0; y < 12; y++) {
                ul.append($('<li>' + this.__mAbbr[y] + '</li>'));
                $(ul.find('li:last')[0]).bind('click',
                { monthNum: y },
                function(event) {
                    var curDate = FreeswitchConfig.Site.DateTimePicker.CurrentDateValue();
                    curDate.setMonth(event.data.monthNum);
                    FreeswitchConfig.Site.DateTimePicker.SetCurrentDateTime(curDate);
                });
                $(ul.find('li:last')[0]).bind('dblclick', function() { FreeswitchConfig.Site.DateTimePicker.CloseCalendar(); });
            }
            ul = $(dateCal.find('ul[name="YearList"]')[0]);
            for (var y = 0; y < 5; y++) {
                $(ul.children()[y]).bind('click',
                    { button: $(ul.children()[y]) },
                    function(event) {
                        switch (event.data.button.html()) {
                            case '&lt;':
                                break;
                            case '&gt;':
                                break;
                            default:
                                var curDate = FreeswitchConfig.Site.DateTimePicker.CurrentDateValue();
                                curDate.setFullYear(event.data.button.html());
                                FreeswitchConfig.Site.DateTimePicker.SetCurrentDateTime(curDate);
                                break;
                        }
                    }
                );
                $(ul.children()[y]).bind('dblclick', function() { FreeswitchConfig.Site.DateTimePicker.CloseCalendar(); });
            }
            var tbody = $(dateCal.find('tbody[name="dayOfWeek"]')[0]);
            for (var y = 0; y < 6; y++) {
                var tr = $('<tr class="AnyTime-wk"></tr>');
                tbody.append(tr);
                for (var z = 0; z < 7; z++) {
                    var td = $('<td></td>');
                    tr.append(td);
                    td.bind('click',
                    { button: td },
                    function(event) {
                        if (event.data.button.html() != '') {
                            var curDate = FreeswitchConfig.Site.DateTimePicker.CurrentDateValue();
                            curDate.setDate(event.data.button.html());
                            FreeswitchConfig.Site.DateTimePicker.SetCurrentDateTime(curDate);
                        }
                    });
                    $(td).bind('dblclick', { button: td }, function(event) { if (event.data.button.html() != '') { FreeswitchConfig.Site.DateTimePicker.CloseCalendar(); } });
                }
            }
            ul = $(dateCal.find('ul[name="amHours"]')[0]);
            for (var y = 0; y < 12; y++) {
                ul.append($('<li>' + (y < 10 ? '0' + y : y) + '</li>'));
                $(ul.find('li:last')[0]).bind('click',
                    { button: $(ul.find('li:last')[0]) },
                    function(event) {
                        var curDate = FreeswitchConfig.Site.DateTimePicker.CurrentDateValue();
                        curDate.setHours(event.data.button.html());
                        FreeswitchConfig.Site.DateTimePicker.SetCurrentDateTime(curDate);
                    });
                $(ul.find('li:last')[0]).bind('dblclick',
                    { button: $(ul.find('li:last')[0]) },
                    function(event) { if (event.data.button.html() != '') { FreeswitchConfig.Site.DateTimePicker.CloseCalendar(); } });
            }
            ul = $(dateCal.find('ul[name="pmHours"]')[0]);
            for (var y = 12; y < 24; y++) {
                ul.append($('<li>' + y + '</li>'));
                $(ul.find('li:last')[0]).bind('click',
                    { button: $(ul.find('li:last')[0]) },
                    function(event) {
                        var curDate = FreeswitchConfig.Site.DateTimePicker.CurrentDateValue();
                        curDate.setHours(event.data.button.html());
                        FreeswitchConfig.Site.DateTimePicker.SetCurrentDateTime(curDate);
                    });
                $(ul.find('li:last')[0]).bind('dblclick',
                    { button: $(ul.find('li:last')[0]) },
                        function(event) { if (event.data.button.html() != '') { FreeswitchConfig.Site.DateTimePicker.CloseCalendar(); } });
            }
            ul = $(dateCal.find('ul[name="minTens"]')[0]);
            for (var y = 0; y < 12; y++) {
                ul.append($('<li>' + (y < 6 ? y : '') + '</li>'));
                if (y >= 6) {
                    $(ul.find('li:last')[0]).attr('class', 'AnyTime-btn AnyTime-min-ten-btn ui-state-default AnyTime-min-ten-btn-empty ui-state-disabled');
                } else {
                    $(ul.find('li:last')[0]).bind('click',
                    { button: $(ul.find('li:last')[0]) },
                    function(event) {
                        var curDate = FreeswitchConfig.Site.DateTimePicker.CurrentDateValue();
                        var min = curDate.getMinutes();
                        if (min >= 10) {
                            min = min.toString().substring(1);
                        }
                        curDate.setMinutes(event.data.button.html() + min.toString());
                        FreeswitchConfig.Site.DateTimePicker.SetCurrentDateTime(curDate);
                    });
                    $(ul.find('li:last')[0]).bind('dblclick',
                    { button: $(ul.find('li:last')[0]) },
                        function(event) { if (event.data.button.html() != '') { FreeswitchConfig.Site.DateTimePicker.CloseCalendar(); } });
                }
            }
            ul = $(dateCal.find('ul[name="minNines"]')[0]);
            for (var y = 0; y < 12; y++) {
                ul.append($('<li>' + (y < 10 ? y : '') + '</li>'));
                if (y >= 10) {
                    $(ul.find('li:last')[0]).attr('class', 'AnyTime-btn AnyTime-min-ten-btn ui-state-default AnyTime-min-ten-btn-empty ui-state-disabled');
                } else {
                    $(ul.find('li:last')[0]).bind('click',
                    { button: $(ul.find('li:last')[0]) },
                    function(event) {
                        var curDate = FreeswitchConfig.Site.DateTimePicker.CurrentDateValue();
                        var min = curDate.getMinutes();
                        if (min < 10) {
                            min = '0';
                        } else {
                            min = min.toString().substring(0, 1);
                        }
                        curDate.setMinutes(min.toString() + event.data.button.html());
                        FreeswitchConfig.Site.DateTimePicker.SetCurrentDateTime(curDate);
                    });
                    $(ul.find('li:last')[0]).bind('dblclick',
                    { button: $(ul.find('li:last')[0]) },
                        function(event) { if (event.data.button.html() != '') { FreeswitchConfig.Site.DateTimePicker.CloseCalendar(); } });
                }
            }
            ul = $(dateCal.find('ul[name="secTens"]')[0]);
            for (var y = 0; y < 12; y++) {
                var li = $('<li>' + (y < 6 ? y : '') + '</li>');
                ul.append(li);
                if (y >= 6) {
                    li.attr('class', 'AnyTime-btn AnyTime-min-ten-btn ui-state-default AnyTime-min-ten-btn-empty ui-state-disabled');
                } else {
                    li.bind('click',
                    { button: li },
                    function(event) {
                        var curDate = FreeswitchConfig.Site.DateTimePicker.CurrentDateValue();
                        var min = curDate.getSeconds();
                        if (min >= 10) {
                            min = min.toString().substring(1);
                        }
                        curDate.setSeconds(event.data.button.html() + min.toString());
                        FreeswitchConfig.Site.DateTimePicker.SetCurrentDateTime(curDate);
                    });
                    li.bind('dblclick',
                    { button: li },
                        function(event) { if (event.data.button.html() != '') { FreeswitchConfig.Site.DateTimePicker.CloseCalendar(); } });
                }
            }
            ul = $(dateCal.find('ul[name="secNines"]')[0]);
            for (var y = 0; y < 12; y++) {
                var li = $('<li>' + (y < 10 ? y : '') + '</li>');
                ul.append(li);
                if (y >= 10) {
                    li.attr('class', 'AnyTime-btn AnyTime-min-ten-btn ui-state-default AnyTime-min-ten-btn-empty ui-state-disabled');
                } else {
                    li.bind('click',
                    { button: li },
                    function(event) {
                        var curDate = FreeswitchConfig.Site.DateTimePicker.CurrentDateValue();
                        var min = curDate.getSeconds();
                        if (min < 10) {
                            min = '0';
                        } else {
                            min = min.toString().substring(0, 1);
                        }
                        curDate.setSeconds(min.toString() + event.data.button.html());
                        FreeswitchConfig.Site.DateTimePicker.SetCurrentDateTime(curDate);
                    });
                    li.bind('dblclick',
                    { button: li },
                        function(event) { if (event.data.button.html() != '') { FreeswitchConfig.Site.DateTimePicker.CloseCalendar(); } });
                }
            }
        }
        $(dateCal.find('input[name="hidDateValue"]')[0]).val(datetime.toString());
        this._curInput.attr('actualDate', datetime.toString());
        this._curInput.val(this.format(datetime, this._curOptions.format));
        this.ReloadCalendar();
    },
    ReloadCalendar: function() {
        var dateCal = $('#DateCal');
        var datetime = new Date($(dateCal.find('input[name="hidDateValue"]')[0]).val());
        var ul = $(dateCal.find('ul[name="YearList"]')[0]);
        for (var y = -1; y < 2; y++) {
            $(ul.children()[2 + y]).html(datetime.getFullYear() + y);
        }
        ul = $(dateCal.find('ul[name="months"]')[0]);
        for (var y = 0; y < 12; y++) {
            $(ul.children()[y]).attr('class', 'AnyTime-btn AnyTime-mon-btn ui-state-default');
            if (y == datetime.getMonth()) {
                $(ul.children()[y]).attr('class', 'AnyTime-btn AnyTime-cur-btn AnyTime-mon-btn ui-state-default ui-state-highlight');
            }
        }
        tbody = $(dateCal.find('tbody[name="dayOfWeek"]')[0]);
        var tmp = new Date(datetime.toString());
        tmp.setDate(1);
        $(tbody.find('td')).attr('class', 'AnyTime-btn AnyTime-dom-btn AnyTime-dom-btn-empty AnyTime-dom-btn-empty-above-filled ui-state-disabled ui-state-default');
        $(tbody.find('td')).html('');
        var cur = $($(tbody.children()[0]).children()[tmp.getDay()]);
        var curRow = $(tbody.children()[0]);
        var curMon = tmp.getMonth();
        while (tmp.getMonth() == curMon) {
            cur.html(tmp.getDate());
            cur.attr('class', 'AnyTime-btn AnyTime-dom-btn AnyTime-dom-btn-filled ui-state-default');
            if (tmp.getDate() == datetime.getDate()) {
                cur.attr('class', 'AnyTime-btn AnyTime-dom-btn AnyTime-dom-btn-filled AnyTime-cur-btn ui-state-default ui-state-highlight');
            }
            if (tmp.getDay() == 6) {
                curRow = $(curRow.next());
                cur = $(curRow.children()[0]);
            } else {
                cur = $(cur.next());
            }
            tmp.setDate(tmp.getDate() + 1);
        }
        ul = $(dateCal.find('ul[name="amHours"]')[0]);
        var curhour = datetime.getHours();
        if (curhour < 10) {
            curhour = '0' + curhour;
        }
        for (var y = 0; y < ul.children().length; y++) {
            $(ul.children()[y]).attr('class', 'AnyTime-btn AnyTime-hr-btn ui-state-default');
            if ($(ul.children()[y]).html() == curhour) {
                $(ul.children()[y]).attr('class', 'AnyTime-btn AnyTime-hr-btn AnyTime-cur-btn ui-state-default ui-state-highlight');
            }
        }
        ul = $(dateCal.find('ul[name="pmHours"]')[0]);
        for (var y = 0; y < ul.children().length; y++) {
            $(ul.children()[y]).attr('class', 'AnyTime-btn AnyTime-hr-btn ui-state-default');
            if ($(ul.children()[y]).html() == curhour) {
                $(ul.children()[y]).attr('class', 'AnyTime-btn AnyTime-hr-btn AnyTime-cur-btn ui-state-default ui-state-highlight');
            }
        }
        ul = $(dateCal.find('ul[name="minTens"]')[0]);
        var min = datetime.getMinutes();
        if (min < 10) {
            min = '0';
        } else {
            min = min.toString().substring(0, 1);
        }
        for (var y = 0; y <= 5; y++) {
            $(ul.children()[y]).attr('class', 'AnyTime-btn AnyTime-min-ten-btn ui-state-default');
            if ($(ul.children()[y]).html() == min) {
                $(ul.children()[y]).attr('class', 'AnyTime-btn AnyTime-min-ten-btn AnyTime-cur-btn ui-state-default ui-state-highlight');
            }
        }
        ul = $(dateCal.find('ul[name="minNines"]')[0]);
        min = datetime.getMinutes();
        if (min >= 10) {
            min = min.toString().substring(1);
        }
        for (var y = 0; y <= 9; y++) {
            $(ul.children()[y]).attr('class', 'AnyTime-btn AnyTime-min-one-btn ui-state-default');
            if ($(ul.children()[y]).html() == min) {
                $(ul.children()[y]).attr('class', 'AnyTime-btn AnyTime-min-one-btn ui-state-default ui-state-highlight');
            }
        }
        ul = $(dateCal.find('ul[name="secTens"]')[0]);
        min = datetime.getSeconds();
        if (min < 10) {
            min = '0';
        } else {
            min = min.toString().substring(0, 1);
        }
        for (var y = 0; y <= 5; y++) {
            $(ul.children()[y]).attr('class', 'AnyTime-btn AnyTime-sec-ten-btn AnyTime-cur-btn ui-state-default');
            if ($(ul.children()[y]).html() == min) {
                $(ul.children()[y]).attr('class', 'AnyTime-btn AnyTime-sec-ten-btn AnyTime-cur-btn ui-state-default ui-state-highlight');
            }
        }
        ul = $(dateCal.find('ul[name="secNines"]')[0]);
        min = datetime.getSeconds();
        if (min >= 10) {
            min = min.toString().substring(1);
        }
        for (var y = 0; y <= 9; y++) {
            $(ul.children()[y]).attr('class', 'AnyTime-btn AnyTime-sec-ten-btn AnyTime-cur-btn ui-state-default');
            if ($(ul.children()[y]).html() == min) {
                $(ul.children()[y]).attr('class', 'AnyTime-btn AnyTime-sec-ten-btn AnyTime-cur-btn ui-state-default ui-state-highlight');
            }
        }
    }
}});