/*
ERROR TYPES: incorrect_file_type,no_file_selected
*/

$.fn.AjaxFileUpload = function(o) {
    if ((o == undefined) || (o == null)) {
        o = new Object();
    }
    this.options = {
        action: (o.action == undefined ? document.location.href : o.action),
        allowedExtensions: o.allowedExtensions,
        alertOnError: (o.alertOnError == undefined ? true : o.alertOnError),
        onSubmit: (o.onSubmit == undefined ? function(button, fileName) { return true; } : o.onSubmit),
        onComplete: (o.onComplete == undefined ? function(button, fileName, response) { alert(response); } : o.onComplete),
        onError: (o.onError == undefined ? function(errorType) {
            if (this.alertOnError) {
                switch (errorType) {
                    case 'incorrect_file_type':
                        alert('The file type you specified to upload is invalid');
                        break;
                    case 'no_file_selected':
                        alert('You did not specify a file to upload');
                        break;
                }
            }
        } : o.onError)
    };
    var n = 'f' + Math.floor(Math.random() * 99999);
    $(this).attr('frmID', n);
    $(document.body).append(this.uploadContainer);
    var frm = $('<div id="' + n.replace('f', 'frm') + '" style="filter:alpha(opacity=0);opacity: 0;-moz-opacity:0;width:' + $(this).width() + 'px;height:' + $(this).height() + 'px;top:' + $(this).offset().top + 'px;left:' + $(this).offset().left + 'px;z-index:10;position:absolute;overflow:hidden;">' +
        '<iframe id="' + n + '" name="' + n + '" style="width:100%;height:100%;margin:0;padding:0;"></iframe></div>');
    $(document.body).append(frm);
    var doc = frm.children()[0].contentWindow.document;
    doc.open();
    doc.write('<body style="margin:0;padding:0;overflow:hidden;width:' + $(this).width() + 'px;height:' + $(this).height() + 'px;"><form id=' + n + ' name="' + n + '" action="' + this.options.action + '" method="POST" enctype="multipart/form-data">' +
        '<input name="myfile" type="file" style="font-size:' + $(this).width() + 'px;margin:0;padding:0;margin-left:' + ($(this).width() * -11) + 'px;' + ($(this).css('cursor') != '' ? 'cursor:' + $(this).css('cursor') + ';' : '') + '"/>' +
        '</form></body>');
    doc.close();
    this.uploadContainer = $($(doc).find('body')[0]);
    var inp = $($(frm.children()[0].contentWindow.document.body).find('input')[0]);
    inp.bind('change',
        { options: this.options, button: $(this), container: this.uploadContainer },
        function(event) {
            $('#' + event.data.button.attr('frmid')).unbind('load');
            var filename = $(this).val();
            if (filename == '') {
                event.data.options.onError('no_file_selected');
            } else {
                var submit = false;
                if ((event.data.options.allowedExtensions != null) && (event.data.options.allowedExtensions != undefined)) {
                    for (var y = 0; y < event.data.options.allowedExtensions.length; y++) {
                        if (filename.toUpperCase().endsWith(event.data.options.allowedExtensions[y].toUpperCase())) {
                            submit = true;
                            break;
                        }
                    }
                    if (!submit) {
                        event.data.options.onError('incorrect_file_type');
                    }
                } else {
                    submit = true;
                }
                if (submit) {
                    submit = event.data.options.onSubmit(event.data.button, filename);
                    if (submit == null || submit) {
                        $('#'+event.data.button.attr('frmid')).bind('load',
                            { options: event.data.options, fileName: filename, button: event.data.button},
                            function(event) {
                                event.data.options.onComplete(
                                    event.data.button,
                                    event.data.filename,
                                    $(document.getElementById(event.data.button.attr('frmID')).contentWindow.document.body).html()
                                );
                            }
                        );
                        $(event.data.container.find('form')[0]).submit();
                    }
                }
            }
        }
    );
}