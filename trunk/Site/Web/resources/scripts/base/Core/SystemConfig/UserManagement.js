CreateNameSpace('FreeswitchConfig.Services.UserManagement');

FreeswitchConfig.Services.UserManagement = $.extend(FreeswitchConfig.Services.UserManagement,{
    AvailableExtensions: function() {
        if (FreeswitchConfig.Services.UserManagement._availableExtensions == undefined) {
            FreeswitchConfig.Services.UserManagementService.GetAvailableExtensions(
                    function(msg) {
                        FreeswitchConfig.Services.UserManagement._availableExtensions = new Array();
                        FreeswitchConfig.Services.UserManagement._availableExtensions.push(new FreeswitchConfig.Site.Form.SelectValue(''));
                        for(var x=0;x<msg.length;x++){
                            FreeswitchConfig.Services.UserManagement._availableExtensions.push(new FreeswitchConfig.Site.Form.SelectValue(msg[x]));
                        }
                    },
                    null,
                    null,
                    true
                );
        }
        return FreeswitchConfig.Services.UserManagement._availableExtensions;
    },
    GenerateUserRow: function(data) {
        var buttons = new Array();
        if (data.Disabled){
            var butActive = $('<img title="Activate User" class="Button accept"/>');
            FreeswitchConfig.Services.UserManagement.BindActivateAccount(butActive, data.UserName);
            buttons.push(butActive);
        }else if (data.Locked){
            var butActive = $('<img title="Unlock User" class="Button lock_open"/>');
            butActive.bind('click',
                { button: butActive, username: data.UserName },
                function(event) {
                    FreeswitchConfig.Site.Modals.ShowUpdating();
                    FreeswitchConfig.Services.UserManagementService.UnlockAccount(
                        event.data.username,
                        function(msg) {
                            event.data.button.unbind('click');
                            event.data.button.attr('class', 'button cancel');
                            event.data.button.attr('title', 'Disable Account');
                            $(event.data.button.next()).html('Active');
                            FreeswitchConfig.Services.UserManagement.BindDisableAccount(event.data.button, event.data.username);
                            FreeswitchConfig.Site.Modals.HideUpdating();
                        },
                        function(msg) {
                            FreeswitchConfig.Site.Modals.HideUpdating();
                            alert('An error occured communicating with the user management service.');
                        }
                    );
                });
            buttons.push(butActive);
        }else{
            var butActive = $('<img title="Disable User" class="Button cancel">');
            FreeswitchConfig.Services.UserManagement.BindDisableAccount(butActive, data.UserName);
            buttons.push(butActive);
        }
        var butEdit = $('<img class="Button user_edit" title="Edit User">');
        var butChangePassword = $('<img class="Button key" title="Change Password">');
        var butEditUserRights = $('<img class="Button page_white_key" title="Change Rights">');
        buttons.push(butEdit);
        buttons.push(butChangePassword);
        buttons.push(butEditUserRights);
        FreeswitchConfig.Services.UserManagement.BindEditAccount(butEdit, data);
        FreeswitchConfig.Services.UserManagement.BindChangePassword(butChangePassword, data.UserName);
        FreeswitchConfig.Services.UserManagement.BindEditUserRights(butEditUserRights, data.UserName);
        return new TableRow(
            [
                new TableCell(data.UserName,{'style':'vertical-align:top;'}),
                new TableCell(data.FirstName,{'style':'vertical-align:top;'}),
                new TableCell(data.LastName,{'style':'vertical-align:top;'}),
                new TableCell((data.Email == undefined ? '' : data.Email),{'style':'vertical-align:top;'}),
                new TableCell((data.Extension == undefined ? '' : data.Extension),{'style':'vertical-align:top;'}),
                new TableCell((data.Disabled ? 'Disabled' : (data.Locked ? 'Locked' : 'Active')),{'style':'vertical-align:top;'}),
                new TableCell(buttons,{'style':'vertical-align:top;'})
            ]
        );
    },
    GeneratePage: function(container) {
        container = $(container);
        FreeswitchConfig.Services.UserManagementService.GetCurrentUsers(
            function(msg) {
                var thead = [
                    new TableHeaderCell('UserName'),
                    new TableHeaderCell('FirstName'),
                    new TableHeaderCell('LastName'),
                    new TableHeaderCell('Email'),
                    new TableHeaderCell('Extension'),
                    new TableHeaderCell('Status'),
                    new TableHeaderCell('Actions')
                ];
                var rows = new Array();
                for (var y = 0; y < msg.length; y++) {
                    rows.push(FreeswitchConfig.Services.UserManagement.GenerateUserRow(msg[y]));
                }
                tbl = RenderTable(rows,thead);
                
                var butAdd = CreateButton(
                    'user_add',
                    'Add New User',
                    function(button,pars){
                        FreeswitchConfig.Site.Modals.ShowLoading();
                        var frm = FreeswitchConfig.Site.Form.GenerateForm(
                            null,
                            [
                                new FreeswitchConfig.Site.Form.FormInput('username','text',null,null,'UserName:'),
                                new FreeswitchConfig.Site.Form.FormInput('firstname','text',null,null,'First Name:'),
                                new FreeswitchConfig.Site.Form.FormInput('lastname','text',null,null,'Last Name:'),
                                new FreeswitchConfig.Site.Form.FormInput('password','password',null,null,'Password:'),
                                new FreeswitchConfig.Site.Form.FormInput('confirmpassword','password',null,null,'Confirm Password:'),
                                new FreeswitchConfig.Site.Form.FormInput('email','text',null,null,'Email:'),
                                new FreeswitchConfig.Site.Form.FormInput('extension','select',FreeswitchConfig.Services.UserManagement.AvailableExtensions(),null,'Extension:')
                            ]
                        );
                        FreeswitchConfig.Site.Modals.ShowFormPanel(
                            'Add New User',
                            frm,
                            [
                                CreateButton(
                                    'accept',
                                    'Okay',
                                    function(button,pars){
                                        FreeswitchConfig.Site.Modals.ShowUpdating();
                                        var canSubmit = false;
                                        var uname = $(pars.frm.find('input[name="username"]')[0]);
                                        canSubmit = ValidateRequiredField(uname);
                                        var fname = $(pars.frm.find('input[name="firstname"]')[0]);
                                        canSubmit = canSubmit || ValidateRequiredField(fname);
                                        var lname = $(pars.frm.find('input[name="lastname"]')[0]);
                                        canSubmit = canSubmit || ValidateRequiredField(lname);
                                        var password = $(pars.frm.find('input[name="password"]')[0]);
                                        canSubmit = canSubmit || ValidateRequiredField(password);
                                        var conpassword = $(pars.frm.find('input[name="confirmpassword"]')[0]);
                                        canSubmit = canSubmit || ValidateRequiredField(conpassword);
                                        if (password.val() != conpassword.val()) {
                                            canSubmit = false;
                                        }
                                        var email = $(pars.frm.find('input[name="email"]')[0]);
                                        if (email.val() != '' && !IsValidEmail(email.val())) {
                                            if ($(email.next()).html() != '*') {
                                                email.after(FreeswitchConfig.Site.Validation.ProduceErrorObject('*'));
                                            }
                                            canSubmit = false;
                                        } else if ($(email.next()).html() == '*') {
                                            $(email.next()).remove();
                                        }
                                        var ext = $(pars.frm.find('select[name="extension"]')[0]);
                                        if (canSubmit) {
                                            var usr = new Object();
                                            usr.UserName = uname.val();
                                            usr.FirstName = fname.val();
                                            usr.LastName = lname.val();
                                            usr.Email = (email.val() == '' ? undefined : email.val());
                                            usr.Extension = (ext.val() == '' ? undefined : ext.val());
                                            FreeswitchConfig.Services.UserManagementService.CreateUser(
                                                usr.UserName,
                                                usr.FirstName,
                                                usr.LastName,
                                                password.val(),
                                                (usr.Email == undefined ? null : usr.Email),
                                                (usr.Extension == undefined ? null : usr.Extension),
                                                function(msg) {
                                                    if (msg != null) {
                                                        usr.ID = msg;
                                                        AppendToTable(pars.tbl,FreeswitchConfig.Services.UserManagement.GenerateUserRow(usr));
                                                        FreeswitchConfig.Site.Modals.HideUpdating();
                                                        FreeswitchConfig.Site.Modals.HideFormPanel();
                                                    } else {
                                                        FreeswitchConfig.Site.Modals.HideUpdating();
                                                        alert('An error occured trying to create the desired user, please review the information and try again.');
                                                    }
                                                },
                                                function(msg) {
                                                    FreeswitchConfig.Site.Modals.HideUpdating();
                                                    alert('An error occured trying to communicate with the User Management Service.');
                                                }
                                            );
                                        } else if (password.val() != conpassword.val()) {
                                            FreeswitchConfig.Site.Modals.HideUpdating();
                                            alert('The password and confirm password do not match.');
                                        } else {
                                            FreeswitchConfig.Site.Modals.HideUpdating();
                                            alert('Please correct the highlighted fields in order to create a user.');
                                        }  
                                    },
                                    {frm:frm,tbl:pars.tbl}
                                ),
                                CreateButton(
                                    'cancel',
                                    'Cancel',
                                    function(button){
                                        FreeswitchConfig.Site.Modals.HideFormPanel();
                                    }
                                )
                            ]
                        );
                        FreeswitchConfig.Site.Modals.HideLoading();
                    },
                    {tbl:tbl}
                );
                
                container.append(butAdd);
                container.append(tbl);
            },
            null,
            null,
            true
        );
    },
    BindActivateAccount: function(button, username) {
        $(button).bind('click',
        { button: $(button), username: username },
        function(event) {
            FreeswitchConfig.Site.Modals.ShowUpdating();
            FreeswitchConfig.Services.UserManagementService.EnableAccount(
                event.data.username,
                function(msg) {
                    event.data.button.unbind('click');
                    $(event.data.button.next()).html('Active');
                    event.data.button.attr('class', 'cancel button');
                    event.data.button.attr('title', 'Disable Account');
                    FreeswitchConfig.Services.UserManagement.BindDisableAccount(event.data.button, event.data.username);
                    FreeswitchConfig.Site.Modals.HideUpdating();
                },
                function(msg) {
                    FreeswitchConfig.Site.Modals.HideUpdating();
                    alert('An error occured communicating with the user management service.');
                }
            );
        });
    },
    BindDisableAccount: function(button, username) {
        $(button).bind('click',
        { button: $(button), username: username },
        function(event) {
            FreeswitchConfig.Site.Modals.ShowUpdating();
            FreeswitchConfig.Services.UserManagementService.DisableAccount(
                event.data.username,
                function(msg) {
                    event.data.button.unbind('click');
                    $(event.data.button.next()).html('Disabled');
                    event.data.button.attr('class', 'accept button');
                    event.data.button.attr('title', 'Activate Account');
                    FreeswitchConfig.Services.UserManagement.BindActivateAccount(event.data.button, event.data.username);
                    FreeswitchConfig.Site.Modals.HideUpdating();
                },
                function(msg) {
                    FreeswitchConfig.Site.Modals.HideUpdating();
                    alert('An error occured communicating with the user management service.');
                }
            );
        });
    },
    BindEditAccount: function(button, data) {
        $(button).bind('click',
            { button: $(button), usr: data },
            function(event) {
                FreeswitchConfig.Site.Modals.ShowLoading();
                var td = $(event.data.button.parent());
                $(td.find('img')).hide();
                var spn = $('<span></span>');
                td.append(spn);
                var exts = '<option></option>';
                for (var y = 0; y < FreeswitchConfig.Services.UserManagement.AvailableExtensions().length; y++) {
                    exts += '<option value="' + FreeswitchConfig.Services.UserManagement.AvailableExtensions()[y] + '" ' + (event.data.usr.Extension == FreeswitchConfig.Services.UserManagement.AvailableExtensions()[y] ? 'SELECTED' : '') + '>' + FreeswitchConfig.Services.UserManagement.AvailableExtensions()[y] + '</option>';
                }
                spn.append('UserName: <input type="text" name="username" value="' + event.data.usr.UserName + '"/><br/>');
                spn.append('First Name: <input type="text" name="firstname" value="' + event.data.usr.FirstName + '"/><br/>');
                spn.append('Last Name: <input type="text" name="lastname" value="' + event.data.usr.LastName + '"/><br/>');
                spn.append('Email: <input type="text" name="email" value="' + (event.data.usr.Email == undefined ? '' : event.data.usr.Email) + '"/><br/>');
                spn.append('Extension: <select name="extension">' + exts + '</select><br/>');
                spn.append('<center><img class="accept button" style="margin-right:10px;"/><img class="cancel button"/></center>');
                $(spn.find('img:last')[0]).bind('click',
                        { td: td, spn: spn },
                        function(evnt) {
                            $(td.find('img')).show();
                            spn.remove();
                        });
                $(spn.find('img:first')[0]).bind('click',
                        { td: td, spn: spn, usr: event.data.usr },
                        function(evnt) {
                            FreeswitchConfig.Site.Modals.ShowUpdating();
                            var canSubmit = false;
                            var uname = $(evnt.data.spn.find('input[name="username"]')[0]);
                            canSubmit = ValidateRequiredField(uname);
                            var fname = $(evnt.data.spn.find('input[name="firstname"]')[0]);
                            canSubmit = canSubmit || ValidateRequiredField(fname);
                            var lname = $(evnt.data.spn.find('input[name="lastname"]')[0]);
                            canSubmit = canSubmit || ValidateRequiredField(lname);
                            var email = $(evnt.data.spn.find('input[name="email"]')[0]);
                            if (email.val() != '' && !IsValidEmail(email.val())) {
                                if ($(email.next()).html() != '*') {
                                    email.after(FreeswitchConfig.Site.Validation.ProduceErrorObject('*'));
                                }
                                canSubmit = false;
                            } else if ($(email.next()).html() == '*') {
                                $(email.next()).remove();
                            }
                            var ext = $(evnt.data.spn.find('select[name="extension"]')[0]);
                            if (canSubmit) {
                                evnt.data.usr.UserName = uname.val();
                                evnt.data.usr.FirstName = fname.val();
                                evnt.data.usr.LastName = lname.val();
                                evnt.data.usr.Email = (email.val() == '' ? undefined : email.val());
                                evnt.data.usr.Extension = (ext.val() == '' ? undefined : ext.val());
                                FreeswitchConfig.Services.UserManagementService.UpdateUserInformation(
                                    evnt.data.usr.ID,
                                    evnt.data.usr.UserName,
                                    evnt.data.usr.FirstName,
                                    evnt.data.usr.LastName,
                                    (evnt.data.usr.Email == undefined ? null : evnt.data.usr.Email),
                                    (evnt.data.usr.Extension == undefined ? null : evnt.data.usr.Extension),
                                    function(msg) {
                                        if (msg) {
                                            var tr = $(td.parent());
                                            $(tr.children()[0]).html(evnt.data.usr.UserName);
                                            $(tr.children()[1]).html(evnt.data.usr.FirstName);
                                            $(tr.children()[2]).html(evnt.data.usr.LastName);
                                            $(tr.children()[3]).html((evnt.data.usr.Email == undefined ? '' : evnt.data.usr.Email));
                                            $(tr.children()[4]).html((evnt.data.usr.Extension == undefined ? '' : evnt.data.usr.Extension));
                                            FreeswitchConfig.Site.Modals.HideUpdating();
                                            $($(evnt.data.spn.find('img:last')[0]).prev()).unbind('click');
                                            FreeswitchConfig.Services.UserManagement.BindEditAccount(evnt.data.button, evnt.data.usr);
                                            $(evnt.data.spn.find('img:last')[0]).click();
                                        } else {
                                            FreeswitchConfig.Site.Modals.HideUpdating();
                                            alert('An error occured updating the selected user please review the information and try again.');
                                        }
                                    },
                                    function(msg) {
                                        FreeswitchConfig.Site.Modals.HideUpdating();
                                        alert('An error occured communicating witht the User Management Service.');
                                    }
                                );
                            } else {
                                FreeswitchConfig.Site.Modals.HideUpdating();
                                alert('Please correct the fields indicated and try again.');
                            }
                        });
                FreeswitchConfig.Site.Modals.HideLoading();
            });
    },
    BindChangePassword: function(button, username) {
        $(button).bind('click',
        { button: $(button), username: username },
        function(event) {
            FreeswitchConfig.Site.Modals.ShowLoading();
            var td = $(event.data.button.parent());
            $(td.find('img')).hide();
            var spn = $('<span></span>');
            td.append(spn);
            spn.append('New Password: <input type="password" name="newPassword"/><br/>');
            spn.append('Confirm Password: <input type="password" name="confirmPassword"/></br>');
            spn.append('<center><img class="accept" style="margin-right:10px;cursor:pointer;"/><img class="cancel" style="cursor:pointer;"/></center>');
            $(spn.find('img:last')[0]).bind('click',
                        { td: td, spn: spn },
                        function(evnt) {
                            $(td.find('img')).show();
                            spn.remove();
                        });
            $(spn.find('img:first')[0]).bind('click',
                        { td: td, spn: spn, username: event.data.username },
                        function(evnt) {
                            FreeswitchConfig.Site.Modals.ShowUpdating();
                            var newPass = $(spn.find('input[name="newPassword"]')[0]).val();
                            var confirmPass = $(spn.find('input[name="confirmPassword"]')[0]).val();
                            if (newPass != confirmPass) {
                                FreeswitchConfig.Site.Modals.HideUpdating();
                                alert('The New Password and Confirm New Password do not match.');
                            } else if (newPass == '') {
                                FreeswitchConfig.Site.Modals.HideUpdating();
                                alert('You must supply the new password.');
                            } else {
                                FreeswitchConfig.Services.UserManagementService.UpdateUserPassword(
                                    evnt.data.username,
                                    newPass,
                                    function(msg) {
                                        FreeswitchConfig.Site.Modals.HideUpdating();
                                        if (msg) {
                                            $(td.find('img')).show();
                                            spn.remove();
                                        } else {
                                            alert('An error occured attempting to update the selected user\'s password, please review and try again.');
                                        }
                                    },
                                    function(msg) {
                                        FreeswitchConfig.Site.Modals.HideUpdating();
                                        alert('An error occured communicating with the user management service.');
                                    }
                                );
                            }
                        });
            FreeswitchConfig.Site.Modals.HideLoading();
        });
    },
    BindEditUserRights: function(button, username) {
        $(button).bind('click',
        { button: $(button), username: username },
        function(event) {
            FreeswitchConfig.Site.Modals.ShowLoading();
            FreeswitchConfig.Services.UserManagementService.GetUserRights(
                event.data.username,
                function(msg) {
                    var td = $(event.data.button.parent());
                    $(td.find('img')).hide();
                    var spn = $('<span></span>');
                    td.append(spn);
                    for (var y = 0; y < msg.length; y++) {
                        spn.append(msg[y].Name + ': <input type="checkbox" name="' + msg[y].Name + '" ' + (msg[y].Value == 'true' ? 'CHECKED' : '') + '/><br/>');
                    }
                    spn.append('<center><img class="accept button" style="margin-right:10px;"/><img class="cancel button"/></center>');
                    $(spn.find('img:last')[0]).bind('click',
                        { td: td, spn: spn },
                        function(evnt) {
                            $(td.find('img')).show();
                            spn.remove();
                        });
                    $(spn.find('img:first')[0]).bind('click',
                        { td: td, spn: spn, username: event.data.username },
                        function(evnt) {
                            FreeswitchConfig.Site.Modals.ShowUpdating();
                            var rights = new Array();
                            var chks = evnt.data.spn.find('input[type="checkbox"]:checked');
                            for (var y = 0; y < chks.length; y++) {
                                rights.push($(chks[y]).attr('name'));
                            }
                            FreeswitchConfig.Services.UserManagementService.AssignRightsToUser(
                                evnt.data.username,
                                rights,
                                function(msg) {
                                    FreeswitchConfig.Site.Modals.HideUpdating();
                                    if (msg) {
                                        $(td.find('img')).show();
                                        spn.remove();
                                    } else {
                                        alert('An error occured attempting to update the selected user\'s rights, please review and try again.');
                                    }
                                },
                                function(msg) {
                                    FreeswitchConfig.Site.Modals.HideUpdating();
                                    alert('An error occured communicating with the user management service.');
                                }
                            );
                        });
                    FreeswitchConfig.Site.Modals.HideLoading();
                },
                function(msg) {
                    FreeswitchConfig.Site.Modals.HideLoading();
                    alert('An error occured communicating with the user management service.');
                }
            );
        });
    }
});