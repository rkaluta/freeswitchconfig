CreateNameSpace('FreeswitchConfig.Site.Form');

FreeswitchConfig.Site.Form = $.extend(FreeswitchConfig.Site.Form, {
    SelectValue: function(value, title) {
        this.ID = value;
        this.Text = (title == undefined ? value : title);
    },
    CheckboxListValue: function(value, title) {
        this.ID = value;
        this.Text = (title == undefined ? value : title);
    },
    FormButton: function(imgSrc, title, callback, callbackpars) {
        this.imgsrc = imgSrc;
        this.Title = title;
        this.callback = callback;
        this.pars = (callbackpars == undefined ? null : callbackpars);
    },
    FormArrayInput: function(title, values) {
        this.Title = title;
        this.Values = values;
        this.Type = 'array_inputs';
        this.render = function() {
            content = new Array();
            var spn = null;
            for (i in this.Values) {
                var spn = FreeswitchConfig.Site.Skin.span.Create({ Attributes: { 'style': 'display:block'} });
                spn.append((this.Values[i].Title == null ? this.Values[i].Name : this.Values[i].Title));
                var tmpCont = this.Values[i].render();
                if (tmpCont instanceof Array) {
                    for (var i in tmpCont) {
                        spn.append(tmpCont[i]);
                    }
                } else {
                    spn.append(tmpCont);
                }
                content.push(spn);
            }
            if (this.Attributes != null) {
                for (var i in this.Attributes) {
                    for (var x = 0; x < content.length; x++) {
                        content[x].attr(i, this.Attributes[i]);
                    }
                }
            }
            return content;
        };
    },
    FormStaticEntry: function(title, content) {
        this.Type = 'static';
        this.Content = content;
        this.Title = title;
        this.render = function() {
            return this.Content;
        }
    },
    FormInput: function(name, type, values, required, title, attributes, currentValue) {
        this.Name = name;
        this.Type = type;
        this.Values = (values == undefined ? null : values);
        this.Title = (title == undefined ? null : title);
        this.Attributes = (attributes == undefined ? null : attributes);
        this.CurrentValue = (currentValue == undefined ? null : currentValue);
        switch (type) {
            case 'text':
            case 'password':
            case 'hidden':
                this.render = function() {
                    var content = $('<input type="' + this.Type + '" name="' + this.Name + '"/>');
                    if (this.CurrentValue != null) {
                        content.val(this.CurrentValue);
                    }
                    if (this.Attributes != null) {
                        for (var i in this.Attributes) {
                            content.attr(i, this.Attributes[i]);
                        }
                    }
                    return content;
                };
                break;
            case 'radio':
                this.render = function() {
                    var content = new Array();
                    for (var x = 0; x < this.Values.length; x++) {
                        content.push($('<input type="radio" name="' + this.Name + '" value="' + this.Values[x].ID + '"/><label>' + this.Values[x].Text + '</label>'));
                    }
                    if (this.CurrentValue != null) {
                        for (var x = 0; x < content.length; x++) {
                            if (content[x].val() == this.CurrentValue) {
                                content[x].prop('checked', true);
                            }
                        }
                    }
                    if (this.Attributes != null) {
                        for (var i in this.Attributes) {
                            for (var x = 0; x < content.length; x++) {
                                content[x].attr(i, this.Attributes[i]);
                            }
                        }
                    }
                    return content;
                };
                break;
            case 'select':
                this.render = function() {
                    var content = $('<select name="' + this.Name + '"></select>');
                    for (var y = 0; y < this.Values.length; y++) {
                        content.append('<option value="' + this.Values[y].ID + '">' + this.Values[y].Text + '</option>');
                    }
                    if (this.CurrentValue != null) {
                        content.val(this.CurrentValue);
                    }
                    if (this.Attributes != null) {
                        for (var i in this.Attributes) {
                            content.attr(i, this.Attributes[i]);
                        }
                    }
                    return content;
                };
                break;
            case 'multiple_select':
                this.render = function() {
                    var content = $('<select name="' + this.Name + '" MULTIPLE rows="5"></select>');
                    for (var y = 0; y < this.Values.length; y++) {
                        content.append('<option value="' + this.Values[y].ID + '">' + this.Values[y].Text + '</option>');
                    }
                    if (this.CurrentValue != null) {
                        if (!(this.CurrentValue instanceof Array)) {
                            this.CurrentValue = [this.CurrentValue];
                        }
                        for (var x = 0; x < this.CurrentValue.length; x++) {
                            $(content.find('option[value="' + this.CurrentValue[x] + '"]')[0]).prop('selected', true);
                        }
                    }
                    if (this.Attributes != null) {
                        for (var i in this.Attributes) {
                            content.attr(i, this.Attributes[i]);
                        }
                    }
                    return content;
                };
                break;
            case 'checkbox':
                this.render = function() {
                    var content = $('<input type="' + this.Type + '" name="' + this.Name + '"/>');
                    if (this.CurrentValue != null) {
                        content.prop('checked', this.CurrentValue);
                    }
                    if (this.Attributes != null) {
                        for (var i in this.Attributes) {
                            content.attr(i, this.Attributes[i]);
                        }
                    }
                    return content;
                };
                break;
            case 'static':
                this.render = function() {
                    var content = this.Content;
                    if (this.Attributes != null) {
                        for (var i in this.Attributes) {
                            content.attr(i, this.Attributes[i]);
                        }
                    }
                    return content;
                };
                break;
            case 'datetime':
                this.render = function() {
                    var content = FreeswitchConfig.Site.DateTimePicker.Create(this.Name, this.CurrentValue);
                    if (this.Attributes != null) {
                        for (var i in this.Attributes) {
                            $(content.find('input,select')).attr(i, this.Attributes[i]);
                        }
                    }
                    return content;
                };
                break;
            case 'textarea':
                this.render = function() {
                    var content = $('<textarea name="' + this.Name + '"></textarea>');
                    if (this.CurrentValue != null) {
                        content.val(this.CurrentValue);
                    }
                    if (this.Attributes != null) {
                        for (var i in this.Attributes) {
                            content.attr(i, this.Attributes[i]);
                        }
                    }
                    return content;
                };
                break;
            case 'checkbox_list':
                this.render = function() {
                    var content = $('<div name="' + this.Name + '" class="multichecks"></div>');
                    if (this.Values != null) {
                        for (i in this.Values) {
                            content.append('<span><input type="checkbox" name="' + this.Values[i].ID + '"/>' + this.Values[i].Text + '</span>');
                        }
                    }
                    if (this.CurrentValue != null) {
                        for (var x = 0; x < this.CurrentValue.length; x++) {
                            $(content.find('input[name="' + this.CurrentValue[x] + '"]')[0]).prop('checked', true);
                        }
                    }
                    if (this.Attributes != null) {
                        for (var i in this.Attributes) {
                            content.attr(i, this.Attributes[i]);
                        }
                    }
                    return content;
                };
                break;
            case 'file_browser':
                this.render = function() {
                    var content = [
                        $('<input type="text" name="' + this.Name + '"/>'),
                        $('<img class="Button folder_explore"/>')
                    ]
                    content[1].bind('click',
                    { button: content[1], inp: content[0] },
                    function(event) {
                        event.data.button.unbind('click');
                        event.data.inp.unbind('click');
                        FreeswitchConfig.Site.Modals.RegisterAudioFileBrowser(event.data.inp, event.data.button);
                        event.data.button.click();
                    });
                    content[0].bind('click',
                    { button: content[1], inp: content[0] },
                    function(event) {
                        event.data.inp.unbind('click');
                        event.data.button.unbind('click');
                        FreeswitchConfig.Site.Modals.RegisterAudioFileBrowser(event.data.inp, event.data.button);
                        event.data.button.click();
                    });
                    if (this.CurrentValue != null) {
                        $(content[0]).val(this.CurrentValue);
                    }
                    if (this.Attributes != null) {
                        for (var i in this.Attributes) {
                            content[0].attr(i, this.Attributes[i]);
                        }
                    }
                    return content;
                };
                break;
        }
    },
    GenerateForm: function(title, inputs, buttons, tblattributes) {
        buttons = (buttons == undefined ? null : buttons);
        var alt = false;
        var ret = FreeswitchConfig.Site.Skin.table.Create({
            Content: (title == null ? null :
                FreeswitchConfig.Site.Skin.thead.Create({
                    Content: FreeswitchConfig.Site.Skin.tr.Create({
                        Content: FreeswitchConfig.Site.Skin.td.Create({
                            Attributes: { 'colspan': '2' },
                            Content: title
                        })
                    })
                })
            )
        });
        var tbody = FreeswitchConfig.Site.Skin.tbody.Create();
        ret.append(tbody);
        var hidFields = new Array();
        for (var x = 0; x < inputs.length; x++) {
            if (inputs[x] != null) {
                if (inputs[x].Type == 'hidden') {
                    hidFields.push(inputs[x]);
                } else {
                    if (alt){
                        tbody.append(FreeswitchConfig.Site.Skin.tr.CreateAlt({
                            Content: [
                                FreeswitchConfig.Site.Skin.td.Create({ Attributes: { 'style': 'text-align:right;vertical-align:top;' }, Content: (inputs[x].Title == null ? inputs[x].Name : inputs[x].Title) }),
                                FreeswitchConfig.Site.Skin.td.Create({ Attributes: { 'style': 'text-align:left;vertical-align:top;' }, Content: inputs[x].render() })
                            ]
                        }));
                    }else{
                        tbody.append(FreeswitchConfig.Site.Skin.tr.Create({
                            Content: [
                                FreeswitchConfig.Site.Skin.td.Create({ Attributes: { 'style': 'text-align:right;vertical-align:top;' }, Content: (inputs[x].Title == null ? inputs[x].Name : inputs[x].Title) }),
                                FreeswitchConfig.Site.Skin.td.Create({ Attributes: { 'style': 'text-align:left;vertical-align:top;' }, Content: inputs[x].render() })
                            ]
                        }));
                    }
                    alt=!alt;
                }
            }
        }
        var td = $(ret.find(FreeswitchConfig.Site.Skin.td.Tag + ':last')[0]);
        for (var x = 0; x < hidFields.length; x++) {
            td.append(hidFields[x].render());
        }
        if (buttons != null) {
            var td = FreeswitchConfig.Site.Skin.td.Create({ Attributes: { 'style': 'text-align:center', 'colspan': '2'} });
            for (var x = 0; x < buttons.length; x++) {
                td.append(
                    CreateButton(
                        buttons[x].imgsrc,
                        buttons[x].Title,
                        function(button, data) {
                            if (data.pars != null) {
                                data.callback(data.tbl, data.pars);
                            } else {
                                data.callback(data.tbl);
                            }
                        },
                        {
                            callback: buttons[x].callback,
                            pars: buttons[x].pars,
                            tbl: ret
                        })
                );
            }
            if (alt){
                tbody.append(FreeswitchConfig.Site.Skin.tr.CreateAlt({Content:td}));
            }else{
                tbody.append(FreeswitchConfig.Site.Skin.tr.Create({Content:td}));
            }
        }
        return ret;
    }
});
