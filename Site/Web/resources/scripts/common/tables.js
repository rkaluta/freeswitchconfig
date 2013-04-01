FreeswitchConfig.Site = $.extend(FreeswitchConfig.Site, { Tables: {
    HeaderCell: function(content, attributes) {
        this.Content =  (content == undefined ? null : content);
        this.Attributes =  (attributes == undefined ? null : attributes);
    },
    Row:function(tableCells, attributes) {
        this.Cells = tableCells;
        this.Attributes = (attributes == undefined ? new Array() : (attributes == null ? new Array() : attributes));
    },
    Cell:function(content, attributes) {
        this.Content =  (content == undefined ? null : content);
        this.Attributes =  (attributes == undefined ? null : attributes);
    },
    Body:function(rows, attributes) {
        rows = (rows == undefined ? null : rows);
        if (rows != null) {
            if (rows instanceof FreeswitchConfig.Site.Tables.Body) {
                return rows;
            }
            if (!(rows instanceof Array)){
                rows = [rows];
            }
            if (rows.length>0){
                isBodys = true;
                var curRows = new Array();
                var x = 0;
                var y = 0;
                if (!(rows[y] instanceof FreeswitchConfig.Site.Tables.Body)) {
                    curRows.push(new FreeswitchConfig.Site.Tables.Body(new Array(), attributes));
                } else {
                    while (rows[y] instanceof FreeswitchConfig.Site.Tables.Body) {
                        curRows.push(rows[y]);
                        y++;
                        if (y >= rows.length) {
                            break;
                        }
                    }
                    if (y < rows.length) {
                        x = curRows.length;
                        curRows.push(new FreeswitchConfig.Site.Tables.Body(new Array(), attributes));
                    }
                }
                while (y < rows.length) {
                    if (rows[y] instanceof FreeswitchConfig.Site.Tables.Row) {
                        curRows[x].Rows.push(rows[y]);
                    } else {
                        curRows.push(rows[y]);
                        x = curRows.length;
                        curRows.push(new FreeswitchConfig.Site.Tables.Body(new Array(), attributes));
                    }
                    y++;
                }
                for (var x = 0; x < curRows.length; x++) {
                    if (curRows[x].Rows.length == 0) {
                        curRows.splice(x, 1);
                        x--;
                    }
                }
                rows = curRows;
            }else{
                rows = null;
            }
        }
        if (rows==null) {
            this.Rows= new Array();
            this.Attributes= (attributes == undefined ? null : attributes);
        } else {
            if (rows instanceof Array){
                if (rows.length==1){
                    if (rows[0] instanceof FreeswitchConfig.Site.Tables.Body){
                        return rows[0];
                    }
                }
            }
            return rows;
        }
    },
    Render: function(rows, headers, attributes, tableClass, trClass, altTrClass) {
        attributes = (attributes == undefined ? null : attributes);
        headers = (headers == undefined ? null : headers);
        tableClass = (tableClass == undefined ? 'Rowed' : (tableClass == null ? 'Rowed' : tableClass));
        trClass = (trClass == undefined ? '' : (trClass == null ? '' : trClass));
        altTrClass = (altTrClass == undefined ? 'Alt' : (altTrClass == null ? 'Alt' : altTrClass));
        var ret = '<table cellspacing="0" class="' + tableClass + '" ';
        if (attributes != null) {
            for (var i in attributes) {
                ret += i + '="' + attributes[i] + '" ';
            }
        }
        ret += '>';
        ret = $(ret + '</table>');
        var tr;
        var cont;
        var td;
        if (headers != null) {
            var th = $('<thead></thead>');
            ret.append(th);
            if (headers instanceof TableRow){
                headers = [headers];
            }else if (headers instanceof Array){
                if (headers[0] instanceof FreeswitchConfig.Site.Tables.HeaderCell){
                    headers = new TableRow(headers);
                }   
            }
            if (!(headers instanceof Array)){
                headers = [headers];
            }
            for (var x = 0; x < headers.length; x++) {
                tr = $('<tr></tr>');
                th.append(tr);
                if (headers[x].Attributes != null) {
                    for (var i in headers[x].Attributes) {
                        tr.attr(i, headers[x].Attributes[i]);
                    }
                }
                for (var y = 0; y < headers[x].Cells.length; y++) {
                    td = $('<th></th>');
                    tr.append(td);
                    if (headers[x].Cells[y].Attributes != null) {
                        for (var i in headers[x].Cells[y].Attributes) {
                            td.attr(i, headers[x].Cells[y].Attributes[i]);
                        }
                    }
                    td.append((headers[x].Cells[y].Content == null ? '' : headers[x].Cells[y].Content));
                }
            }
        }
        if (rows != null) {
            rows = new FreeswitchConfig.Site.Tables.Body(rows);
            if (!(rows instanceof Array)){
                rows = [rows];
            }
            var curClass = trClass;
            for (var x = 0; x < rows.length; x++) {
                var tb = $('<tbody></tbody>');
                ret.append(tb);
                for (var y = 0; y < rows[x].Rows.length; y++) {
                    if (rows[x].Rows[y].Attributes['class'] == null) {
                        rows[x].Rows[y].Attributes['class'] = curClass;
                    } else {
                        if ((rows[x].Rows[y].Attributes['class'].indexOf(trClass) < 0) && (rows[x].Rows[y].Attributes['class'].indexOf(altTrClass) < 0)) {
                            rows[x].Rows[y].Attributes['class'] += ' ' + curClass;
                        }
                    }
                    tr = $('<tr></tr>');
                    tb.append(tr);
                    for (var i in rows[x].Rows[y].Attributes) {
                        tr.attr(i, rows[x].Rows[y].Attributes[i]);
                    }
                    for (var z = 0; z < rows[x].Rows[y].Cells.length; z++) {
                        td = $('<td></td>');
                        tr.append(td);
                        if (rows[x].Rows[y].Cells[z].Attributes != null) {
                            for (var i in rows[x].Rows[y].Cells[z].Attributes) {
                                td.attr(i, rows[x].Rows[y].Cells[z].Attributes[i]);
                            }
                        }
                        if (rows[x].Rows[y].Cells[z].Content != null) {
                            if (rows[x].Rows[y].Cells[z].Content instanceof Array) {
                                for (var a = 0; a < rows[x].Rows[y].Cells[z].Content.length; a++) {
                                    td.append(rows[x].Rows[y].Cells[z].Content[a]);
                                }
                            } else {
                                td.append(rows[x].Rows[y].Cells[z].Content);
                            }
                        }
                    }
                    if (curClass == trClass) {
                        curClass = altTrClass;
                    } else {
                        curClass = trClass;
                    }
                }
            }
        }
        return ret;
    },
    Append:function(table,row,trClass,altTrClass){
        trClass = (trClass == undefined ? '' : (trClass == null ? '' : trClass));
        altTrClass = (altTrClass == undefined ? 'Alt' : (altTrClass == null ? 'Alt' : altTrClass));
        table = $(table);
        var curClass = altTrClass;
        var tb = null;
        if (table.is('table')) {
            if (row.Rows != undefined){
                tb = $('<tbody></tbody>');
                if (row.Attributes!=null){
                    for(var i in row.Attributes){
                        tb.attr(i,row.Attributes[i]);
                    }
                }
                table.append(tb);
                if (table.find('tbody:last>tr:last').length>0){
                    curClass = $(table.find('tr:last')[0]).attr('class');
                }
            }else if (table.find('tbody').length == 0) {
                tb = $('<tbody></tbody>');
                table.append(tb);
            } else {
                tb = $(table.find('tbody')[0]);
                curClass = $(tb.find('tr:last')[0]).attr('class');
            }
        }else if(table.is('tbody')){
            tb = table;
            curClass = $(tb.find('tr:last')[0]).attr('class');
        }
        if (trClass == curClass){
            curClass= altTrClass;
        }else{
            curClass=trClass;
        }
        var tbodys = new FreeswitchConfig.Site.Tables.Body(row);
        if (!(tbodys instanceof Array)){
            tbodys = [tbodys];
        }
        var tbfirst = tb;
        for(var y=0;y<tbodys.length;y++){
            var tbody = tbodys[y];
            for(var x=0;x<tbody.Rows.length;x++){
                row = tbody.Rows[x];
                if (row.Attributes['class'] == null){
                    row.Attributes['class'] = curClass;
                }else{
                    if ((row.Attributes['class'].indexOf(trClass)<0) && (row.Attributes['class'].indexOf(altTrClass)<0)){
                        row.Attributes['class']+=' '+curClass;
                    }
                }
                tr=$('<tr></tr>');
                for (var i in row.Attributes){
                    tr.attr(i,row.Attributes[i]);
                }
                for(var y=0;y<row.Cells.length;y++){
                    var td=$('<td></td>');
                    tr.append(td);
                    if (row.Cells[y].Attributes!=null){
                        for(var i in row.Cells[y].Attributes){
                            td.attr(i,row.Cells[y].Attributes[i]);
                        }
                    }
                    if (row.Cells[y].Content!=null){
                        if (row.Cells[y].Content instanceof Array){
                            for(var z=0;z<row.Cells[y].Content.length;z++){
                                td.append(row.Cells[y].Content[z]);
                            }
                        }else{
                            td.append(row.Cells[y].Content);
                        }
                    }
                }
                tb.append(tr);
                if (trClass == curClass){
                    curClass= altTrClass;
                }else{
                    curClass=trClass;
                }
            }
            if ((tbodys.length>1)&&(y+1<tbodys.length)){
                if (tb.next().length==0){
                    tb.after($('<tbody></tbody>'));
                    if (tbodys[y+1].Attributes!=null){
                        var tmpTb = $(tb.next());
                        tbody = tbodys[y+1];
                        for (var i in tbody.Attributes){
                            tmpTb.attr(i,tbody.Attributes[i]);
                        }
                    }
                }
                tb = $(tb.next());
            }
        }
        tb=tbfirst;
        while(tb.next().length>0){
            tb = $(tb.next());
            var child = tb.children();
            for(var x=0;x<child.length;x++){
                if ((' '+$(child[x]).attr('class')+' ').indexOf(' '+trClass+' ')>0){
                    $(child[x]).attr('class',$(child[x]).attr('class').replace(trClass,curClass));
                }else{
                    $(child[x]).attr('class',$(child[x]).attr('class').replace(altTrClass,curClass));
                }
                if (trClass == curClass){
                    curClass= altTrClass;
                }else{
                    curClass=trClass;
                }
            }
        }
    },
    RemoveFromTable:function(table,row,trClass,altTrClass){
        trClass = (trClass == undefined ? '' : (trClass == null ? '' : trClass));
        altTrClass = (altTrClass == undefined ? 'Alt' : (altTrClass == null ? 'Alt' : altTrClass));
        table = $(table);
        row = $(row);
        var curClass = altTrClass;
        var tb = null;
        if (table.is('tbody')){
            table = $(table.parent());
        }
        row.remove();
        if (table.find('tbody').length>0){
            tb = $(table.find('tbody:first')[0]);
        }
        if (tb!=null){
            var child = tb.children();
            for(var x=0;x<child.length;x++){
                if ((' '+$(child[x]).attr('class')+' ').indexOf(' '+trClass+' ')>0){
                    $(child[x]).attr('class',$(child[x]).attr('class').replace(trClass,curClass));
                }else{
                    $(child[x]).attr('class',$(child[x]).attr('class').replace(altTrClass,curClass));
                }
                if (trClass == curClass){
                    curClass= altTrClass;
                }else{
                    curClass=trClass;
                }
            }
            while(tb.next().length>0){
                tb = $(tb.next());
                child = tb.children();
                for(var x=0;x<child.length;x++){
                    if ((' '+$(child[x]).attr('class')+' ').indexOf(' '+trClass+' ')>0){
                        $(child[x]).attr('class',$(child[x]).attr('class').replace(trClass,curClass));
                    }else{
                        $(child[x]).attr('class',$(child[x]).attr('class').replace(altTrClass,curClass));
                    }
                    if (trClass == curClass){
                        curClass= altTrClass;
                    }else{
                        curClass=trClass;
                    }
                }
            }
        }
    }
}
});