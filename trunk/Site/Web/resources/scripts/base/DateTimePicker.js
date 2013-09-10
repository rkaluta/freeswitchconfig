CreateNameSpace('FreeswitchConfig.Site.DateTimePicker');

FreeswitchConfig.Site.DateTimePicker = $.extend(FreeswitchConfig.Site.DateTimePicker, {
        _monthSels: '<option value="0">Jan</option><option value="1">Feb</option><option value="2">Mar</option><option value="3">Apr</option><option value="4">May</option><option value="5">Jun</option><option value="6">Jul</option><option value="7">Aug</option><option value="8">Sep</option><option value="9">Oct</option><option value="10">Nov</option><option value="11">Dec</option>',
        Create: function(name, val) {
            val = (val == undefined ? new Date() : (val == null ? new Date() : val));
            var ret = $('<span class="datetimepicker"></span>');
            ret.append($('<input type="hidden" name="' + name + '"/>'));
            ret.append($('<select>' + FreeswitchConfig.Site.DateTimePicker._monthSels + '</select>'));
            var sel = $('<select></select>');
            for (var x = 1; x < 32; x++) {
                sel.append('<option value="' + x.toString() + '">' + x.toString() + '</option>');
            }
            ret.append('/');
            ret.append(sel);
            ret.append('/');
            ret.append('<input type="text" length="4" size="4"/>&nbsp;&nbsp;');
            sel = $('<select></select>');
            for (var x = 0; x < 24; x++) {
                sel.append('<option value="' + x.toString() + '">' + x.toString() + '</option>');
            }
            ret.append(sel);
            ret.append(':');
            sel = $('<select></select>');
            for (var x = 0; x < 60; x++) {
                sel.append('<option value="' + x.toString() + '">' + x.toString() + '</option>');
            }
            ret.append(sel);
            $(ret.children()[0]).val(val.getMonth());
            $(ret.children()[1]).val(val.getDate());
            $(ret.children()[2]).val(val.getFullYear());
            $(ret.children()[3]).val(val.getHours());
            $(ret.children()[4]).val(val.getMinutes());
            $(ret.find('input[type="text"],select')).bind('change', { prnt: ret }, function(event) {
                var dt = new Date(
                    $(event.data.prnt.children()[3]).val(),
                    $(event.data.prnt.children()[1]).val(),
                    $(event.data.prnt.children()[2]).val(),
                    $(event.data.prnt.children()[4]).val(),
                    $(event.data.prnt.children()[5]).val(),
                    0);
                $(event.data.prnt.children()[0]).val(dt.toString());
            });
            return ret;
        }
    });