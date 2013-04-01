CreateNameSpace('Org.Reddragonit.FreeSwitchConfig.Site.Password');

Org.Reddragonit.FreeSwitchConfig.Site.Password = $.extend(Org.Reddragonit.FreeSwitchConfig.Site.Password, {
    GeneratePage: function(container) {
        var frm = FreeswitchConfig.Site.Form.GenerateForm(
            null,
            [
                new FreeswitchConfig.Site.Form.FormInput('password', 'password', null, null, 'New Password:'),
                new FreeswitchConfig.Site.Form.FormInput('confirmPassword', 'password', null, null, 'Confirm New Password:')
            ]
        );
        FreeswitchConfig.Site.Modals.ShowFormPanel(
            'Change Password',
            frm,
            [
                CreateButton(
                    'accept',
                    'Okay',
                    function(button, pars) {
                        FreeswitchConfig.Site.Modals.ShowUpdating();
                        var pass = $(pars.frm.find('input[name="password"]')[0]).val();
                        var conPass = $(pars.frm.find('input[name="confirmPassword"]')[0]).val();
                        if (pass != conPass) {
                            FreeswitchConfig.Site.Modals.HideUpdating();
                            alert('The new password and confirm new password do not match.');
                        } else if (pass == '') {
                            FreeswitchConfig.Site.Modals.HideUpdating();
                            alert('You must specify a new password to set.');
                        } else {
                            Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Users.UserService.ChangePassword(
                                pass,
                                function(msg) {
                                    FreeswitchConfig.Site.Modals.HideUpdating();
                                    FreeswitchConfig.Site.Modals.HideFormPanel();
                                    alert('You password was successfully changed.');
                                },
                                function(msg) {
                                    FreeswitchConfig.Site.Modals.HideUpdating();
                                    alert('An error occured attempting to update your password, please try again.');
                                }
                            );
                        }
                    },
                    { frm: frm }
                ),
                CreateButton(
                    'cancel',
                    'Cancel',
                    function(button) {
                        FreeswitchConfig.Site.Modals.HideFormPanel();
                    }
                )
            ]
        );
    }
});