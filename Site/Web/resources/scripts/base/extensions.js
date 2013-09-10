String.prototype.trim = function() {
    return this.replace(/^\s+|\s+$/g, "");
}
String.prototype.ltrim = function(chars) {
    if (chars == undefined) {
        return this.replace(/^\s+/, "");
    } else {
        return this.replace(new RegExp('^' + chars + '+'), "");
    }
}
String.prototype.rtrim = function() {
    return this.replace(/\s+$/, "");
}

String.prototype.endsWith = function(str) {
    return this.lastIndexOf(str) >= 0 && (this.lastIndexOf(str) + str.length == this.length);
}

String.prototype.replaceAll = function(find, replacewith) {
    if (this.indexOf(find) >= 0) {
        var index = 0;
        var ret = '';
        while (this.indexOf(find, index) >= 0) {
            var chunk = this.substring(index, this.indexOf(find, index));
            var index = this.indexOf(find, index) + find.length;
            if (chunk == '') {
                ret += replacewith;
            } else {
                ret += chunk + replacewith;
            }
        }
        return ''+(ret + this.substring(index))+'';
    } else {
        return this.toString();
    }
}

String.prototype.regexIndexOf = function(regex, startpos) {
    var indexOf = this.substring(startpos || 0).search(regex);
    return (indexOf >= 0) ? (indexOf + (startpos || 0)) : indexOf;
}

String.prototype.regexLastIndexOf = function(regex, startpos) {
    regex = (regex.global) ? regex : new RegExp(regex.source, "g" + (regex.ignoreCase ? "i" : "") + (regex.multiLine ? "m" : ""));
    if(typeof (startpos) == "undefined") {
        startpos = this.length;
    } else if(startpos < 0) {
        startpos = 0;
    }
    var stringToWorkWith = this.substring(0, startpos + 1);
    var lastIndexOf = -1;
    var nextStop = 0;
    while((result = regex.exec(stringToWorkWith)) != null) {
        lastIndexOf = result.index;
        regex.lastIndex = ++nextStop;
    }
    return lastIndexOf;
}

String.prototype.replaceAllRegexp = function(regex, replacewith) {
    if (this.regexIndexOf(regex) >= 0) {
        var index = 0;
        var ret = '';
        while (this.regexIndexOf(find, index) >= 0) {
            var chunk = this.substring(index, this.regexIndexOf(find, index));
            var index = this.indexOf(find, index) + find.length;
            if (chunk == '') {
                ret += replacewith;
            } else {
                ret += chunk + replacewith;
            }
        }
        return ''+(ret + this.substring(index)).toString()+'';
    } else {
        return this.toString();
    }
}

String.prototype.contains = function(str){
    return this.indexOf(str)>=0;
}


Number.prototype.toFixedStringSize = function(length) {
    var ret = this.toString();
    if (ret.length > length) {
        ret = ret.substring(ret.length - length);
    }
    while (ret.length < length) {
        ret = '0' + ret;
    }
    return ret;
}

Array.prototype.indexOf = function(val) {
    for (var y = 0; y < this.length; y++) {
        if (this[y] == val) {
            return y;
        }
    }
    return -1;
}