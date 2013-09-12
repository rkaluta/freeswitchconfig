CreateNameSpace('FreeswitchConfig.Site.Validation');

FreeswitchConfig.Site.Validation = $.extend(FreeswitchConfig.Site.Validation, {
    ProduceErrorObject: function(value) {
        return FreeswitchConfig.Site.Skin.span.Create({ Content: value, Class: 'ErrorText' });
    },
    IsValidNPANXXValue: function(value) {
        var reg = new RegExp('^((\\d|N|Z|X|(\\[\\d+-\\d+\\])|(\\[(\\d+,)+\\d+\\])|\\.)+\\|?(\\d|N|Z|X|(\\[\\d+-\\d+\\])|(\\[(\\d+,)+\\d+\\])|\\.)+)$');
        if (value.indexOf('\n') >= 0) {
            var tmp = value.split('\n');
            for (var x = 0; x < tmp.length; x++) {
                if (!reg.test(tmp[x])) {
                    return false;
                }
            }
        } else {
            return reg.test(value);
        }
        return true;
    },
    IsValidIPAddress: function(value) {
        var reg = new RegExp('^(((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?))$');
        return reg.test(value);
    },
    IsValidDialableNumber: function(value) {
        var reg = new RegExp('^(\\d{7,25})$');
        return reg.test(value);
    },
    IsValidDomainName: function(value) {
        var reg = new RegExp('^([a-zA-Z0-9]([a-zA-Z0-9\\-]{0,61}[a-zA-Z0-9])?\\.)*[a-zA-Z0-9]([a-zA-Z0-9\\-]{0,61}[a-zA-Z0-9])?$');
        return reg.test(value);
    },
    IsValidPositiveInteger: function(value) {
        var reg = new RegExp('^(\\d+)$');
        return reg.test(value);
    },
    IsValidInteger: function(value) {
        var reg = new RegExp('^(-)?(\\d+)$');
        return reg.test(value);
    },
    IsValidMACAddress: function(value) {
        var reg = new RegExp('^([0-9A-F]{2}(:|-)?){5}[0-9A-F]{2}$');
        return reg.test(value);
    },
    IsValidEmail: function(email) {
        var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
        return reg.test(email);
    },
    IsValidVersionNumber: function(value) {
        var reg = new RegExp('^(\\d+\\.)*\\d+$');
        return reg.test(value);
    },
    ValidateEmailField: function(field) {
        field = $(field);
        if (field.val() != '' && !this.IsValidEmail(field.val())) {
            if (field.next().length == 0 || $(field.next()).html() != '*') {
                field.after(FreeswitchConfig.Site.Validation.ProduceErrorObject('*'));
            }
            return false;
        } else if (field.next().length > 0 && $(field.next()).html() == '*') {
            $(field.next()).remove();
        }
        return true;
    },
    ValidateRequiredField: function(field) {
        field = $(field);
        if (field.val() == '') {
            if (field.next().length == 0 || $(field.next()).html() != '*') {
                field.after(FreeswitchConfig.Site.Validation.ProduceErrorObject('*'));
            }
            return false;
        } else if (field.next().length > 0 && $(field.next()).html() == '*') {
            $(field.next()).remove();
        }
        return true;
    },
    ValidateFieldLength: function(field, length) {
        field = $(field);
        if (field.val().length > length) {
            if (field.next().length == 0 || $(field.next()).html() != '*') {
                field.after(FreeswitchConfig.Site.Validation.ProduceErrorObject('*'));
            }
            return false;
        } else if (field.next().length > 0 && $(field.next()).html() == '*') {
            $(field.next()).remove();
        }
        return true;
    },
    ValidateVersionNumberField: function(field) {
        field = $(field);
        if (field.val() != '' && !this.IsValidVersionNumber(field.val())) {
            if (field.next().length == 0 || $(field.next()).html() != '*') {
                field.after(FreeswitchConfig.Site.Validation.ProduceErrorObject('*'));
            }
            return false;
        } else if (field.next().length > 0 && $(field.next()).html() == '*') {
            $(field.next()).remove();
        }
        return true;
    },
    ValidatePositiveIntegerField: function(field) {
        field = $(field);
        if (field.val() != '' && !this.IsValidPositiveInteger(field.val())) {
            if (field.next().length == 0 || $(field.next()).html() != '*') {
                field.after(FreeswitchConfig.Site.Validation.ProduceErrorObject('*'));
            }
            return false;
        } else if (field.next().length > 0 && $(field.next()).html() == '*') {
            $(field.next()).remove();
        }
        return true;
    },
    ValidateDialableNumberField: function(field) {
        field = $(field);
        if (field.val() != '' && !this.IsValidDialableNumber(field.val())) {
            if (field.next().length == 0 || $(field.next()).html() != '*') {
                field.after(FreeswitchConfig.Site.Validation.ProduceErrorObject('*'));
            }
            return false;
        } else if (field.next().length > 0 && $(field.next()).html() == '*') {
            $(field.next()).remove();
        }
        return true;
    },
    ValidateIntegerInRange: function(field, min, max) {
        field = $(field);
        if (field.val() != '' && !this.IsValidInteger(field.val())) {
            if (field.next().length == 0 || $(field.next()).html() != '*') {
                field.after(FreeswitchConfig.Site.Validation.ProduceErrorObject('*'));
            }
            return false;
        } else if (field.val() != '' && this.IsValidInteger(field.val())) {
            var val = 1 * field.val();
            min = (min == null ? val : min);
            max = (max == undefined ? val : (max == null ? val : max));
            if (val < min || val > max) {
                if (field.next().length == 0 || $(field.next()).html() != '*') {
                    field.after(FreeswitchConfig.Site.Validation.ProduceErrorObject('*'));
                }
                return false;
            } else if (field.next().length > 0 && $(field.next()).html() == '*') {
                $(field.next()).remove();
            }
        } else if (field.next().length > 0 && $(field.next()).html() == '*') {
            $(field.next()).remove();
        }
        return true;
    },
    ValidateNPANXXField: function(field) {
        field = $(field);
        if (field.val() != '' && !this.IsValidNPANXXValue(field.val())) {
            if (field.next().length == 0 || $(field.next()).html() != '*') {
                field.after(FreeswitchConfig.Site.Validation.ProduceErrorObject('*'));
            }
            return false;
        } else if (field.next().length > 0 && $(field.next()).html() == '*') {
            $(field.next()).remove();
        }
        return true;
    },
    ValidateIPAddressField: function(field) {
        field = $(field);
        if (field.val() != '' && !this.IsValidIPAddress(field.val())) {
            if (field.next().length == 0 || $(field.next()).html() != '*') {
                field.after(FreeswitchConfig.Site.Validation.ProduceErrorObject('*'));
            }
            return false;
        } else if (field.next().length > 0 && $(field.next()).html() == '*') {
            $(field.next()).remove();
        }
        return true;
    },
    ValidateMACAddressField: function(field) {
        field = $(field);
        if (field.val() != '' && !this.IsValidMACAddress(field.val())) {
            if (field.next().length == 0 || $(field.next()).html() != '*') {
                field.after(FreeswitchConfig.Site.Validation.ProduceErrorObject('*'));
            }
            return false;
        } else if (field.next().length > 0 && $(field.next()).html() == '*') {
            $(field.next()).remove();
        }
        return true;
    },
    ValidateDomainNameField: function(field) {
        field = $(field);
        if (field.val() != '' && !this.IsValidDomainName(field.val())) {
            if (field.next().length == 0 || $(field.next()).html() != '*') {
                field.after(FreeswitchConfig.Site.Validation.ProduceErrorObject('*'));
            }
            return false;
        } else if (field.next().length > 0 && $(field.next()).html() == '*') {
            $(field.next()).remove();
        }
        return true;
    }
});