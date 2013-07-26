CreateNameSpace('FreeswitchConfig.Web.Setup');

FreeswitchConfig.Web.Setup = $.extend(FreeswitchConfig.Web.Setup, {
    Domains: null,
    Contexts: null,
    SipProfiles: null,
    Settings: null,
    Interfaces: null,
    ContextForm: function(context) {
        return FreeswitchConfig.Site.Form.GenerateForm(
            null,
            [
                new FreeswitchConfig.Site.Form.FormInput('name', 'text', null, null, 'Name:', null, (context == undefined ? null : context.get('Name'))),
                new FreeswitchConfig.Site.Form.FormInput('type', 'select', [
                    new FreeswitchConfig.Site.Form.SelectValue('Internal'),
                    new FreeswitchConfig.Site.Form.SelectValue('External')
                ], null, 'Type:', null, (context == undefined ? null : context.get('Type'))),
                new FreeswitchConfig.Site.Form.FormInput('controlIP', 'text', null, null, 'Controller IP:', null, (context == undefined ? null : context.get('SocketIP'))),
                new FreeswitchConfig.Site.Form.FormInput('controlPort', 'text', null, null, 'Controller Port:', null, (context == undefined ? null : context.get('SocketPort'))),
                new FreeswitchConfig.Site.Form.FormInput('description', 'textarea', null, null, 'Description:', null, (context == undefined ? null : context.get('Description')))
            ]
        );
    },
    SipProfileForm: function(profile) {
        var sels = new Array();
        for (var x = 0; x < FreeswitchConfig.Web.Setup.Contexts.length; x++) {
            sels.push(new FreeswitchConfig.Site.Form.SelectValue(FreeswitchConfig.Web.Setup.Contexts.at(x).cid, FreeswitchConfig.Web.Setup.Contexts.at(x).get('Name')));
        }
        return FreeswitchConfig.Site.Form.GenerateForm(
            null,
            [
                new FreeswitchConfig.Site.Form.FormInput('name', 'text', null, null, 'Name:', null, (profile == undefined ? null : profile.get('Name'))),
                new FreeswitchConfig.Site.Form.FormInput('context', 'select', sels, null, 'Context:', null, (profile == undefined ? null : profile.get('Context').cid)),
                new FreeswitchConfig.Site.Form.FormInput('port', 'text', null, null, 'Port:', null, (profile == undefined ? null : profile.get('Port'))),
                new FreeswitchConfig.Site.Form.FormInput('sipInterface', 'select', FreeswitchConfig.Web.Setup.AvailableInterfaces(), null, 'SIP Interface:', null, (profile == undefined ? null : profile.get('SIPInterface').cid)),
                new FreeswitchConfig.Site.Form.FormInput('rtpInterface', 'select', FreeswitchConfig.Web.Setup.AvailableInterfaces(), null, 'RTP Interface:', null, (profile == undefined ? null : profile.get('RTPInterface').cid))
            ]
        );
    },
    DomainForm: function(domain) {
        var selIntProfiles = new Array();
        var selExtProfiles = new Array();
        for (var x = 0; x < FreeswitchConfig.Web.Setup.SipProfiles.length; x++) {
            for (var y = 0; y < FreeswitchConfig.Web.Setup.Contexts.length; y++) {
                if (FreeswitchConfig.Web.Setup.Contexts.at(y).get('Name') == FreeswitchConfig.Web.Setup.SipProfiles.at(x).get('Context').get('Name')) {
                    if (FreeswitchConfig.Web.Setup.Contexts.at(y).get('Type') == 'Internal') {
                        selIntProfiles.push(new FreeswitchConfig.Site.Form.SelectValue(FreeswitchConfig.Web.Setup.SipProfiles.at(x).cid, FreeswitchConfig.Web.Setup.SipProfiles.at(x).get('Name')));
                    } else {
                        selExtProfiles.push(new FreeswitchConfig.Site.Form.SelectValue(FreeswitchConfig.Web.Setup.SipProfiles.at(x).cid, FreeswitchConfig.Web.Setup.SipProfiles.at(x).get('Name')));
                    }
                }
            }
        }
        return FreeswitchConfig.Site.Form.GenerateForm(
            null,
            [
                new FreeswitchConfig.Site.Form.FormInput('name', 'text', null, null, 'Name:', null, (domain == undefined ? null : domain.get('Name'))),
                new FreeswitchConfig.Site.Form.FormInput('internalProfile', 'select', selIntProfiles, null, 'Internal Profile:', null, (domain == undefined ? null : domain.get('InternalProfile').cid)),
                new FreeswitchConfig.Site.Form.FormInput('externalProfile', 'select', selExtProfiles, null, 'External Profile:', null, (domain == undefined ? null : domain.get('ExternalProfile').cid))
            ]
        );
    },
    AvailableInterfaces: function() {
        var ret = new Array();
        for (var x = 0; x < FreeswitchConfig.Web.Setup.Interfaces.length; x++) {
            var iface = FreeswitchConfig.Web.Setup.Interfaces.at(x);
            ret.push(new FreeswitchConfig.Site.Form.SelectValue(iface.cid, iface.get('Name')));
        }
        return ret;
    },
    GeneratePage: function(container) {
        if (!IS_SETUP) {
            var dv = FreeswitchConfig.Site.Skin.div.Create({ Class: 'SetupComponentContainer', Content:
                FreeswitchConfig.Site.Skin.div.Create({ Class: 'shadow', Content: [
                    FreeswitchConfig.Site.Skin.div.Create({ Class: 'HeaderBar', Content: 'Welcome' }),
                    FreeswitchConfig.Site.Skin.div.Create({ Class: 'Content', Content: [
                        FreeswitchConfig.Site.Skin.p.Create({ Content: 'Welcome to the FreeSWITCH Configuration Site.  Since this is the first time accessing the site, the system is going to take you through a few steps required to get the site setup and ready to be used.  Click Being to proceed.' }),
                        CreateButton(
                            'arrow_right',
                            'Next',
                            function(button, pars) {
                                $($($(button.parent()).parent()).parent()).remove();
                                FreeswitchConfig.Web.Setup.HideWelcome(pars.container);
                            },
                            { container: container })
                    ]
                    })
                ]
                })
            });
            container.append(dv);
            FreeswitchConfig.Site.Modals.HideLoading();
        } else {
            FreeswitchConfig.Web.Setup.HideWelcome(container);
        }
    },
    HideWelcome: function(container) {
        FreeswitchConfig.Site.Modals.ShowLoading();
        FreeswitchConfig.Web.Setup.Domains = new FreeswitchConfig.Core.Domain.Collection();
        FreeswitchConfig.Web.Setup.Contexts = new FreeswitchConfig.Core.Context.Collection();
        FreeswitchConfig.Web.Setup.SipProfiles = new FreeswitchConfig.Core.SipProfile.Collection();
        FreeswitchConfig.Web.Setup.Interfaces = new FreeswitchConfig.System.sNetworkCard.Collection();
        FreeswitchConfig.Web.Setup.Interfaces.fetch();

        var cvDomains = new FreeswitchConfig.Core.Domain.CollectionView({ collection: FreeswitchConfig.Web.Setup.Domains });
        cvDomains.on('render', function(view) {
            if (FreeswitchConfig.Web.Setup.Contexts.length == 0) {
                FreeswitchConfig.Web.Setup.Contexts.fetch();
            }
        });
        cvDomains.on('item_render', function(view) {
            var spn = $(view.$el.find(FreeswitchConfig.Site.Skin.span.Tag + '.delete')[0]);
            spn.bind('click',
                { model: view.model },
                function(event) {
                    FreeswitchConfig.Site.Modals.ShowLoading();
                    event.data.model.destroy();
                    FreeswitchConfig.Site.Modals.HideLoading();
                });
            spn = $(view.$el.find(FreeswitchConfig.Site.Skin.span.Tag + '.edit')[0]);
            spn.bind('click',
            { model: view.model },
            function(event) {
                FreeswitchConfig.Site.Modals.ShowLoading();
                var frm = FreeswitchConfig.Web.Setup.DomainForm();
                FreeswitchConfig.Site.Modals.ShowFormPanel(
                    'Edit Domain',
                    frm,
                    [
                        CreateButton(
                            'accept',
                            'Okay',
                            function(button, pars) {
                                FreeswitchConfig.Site.Modals.ShowUpdating();
                                if (FreeswitchConfig.Site.Validation.ValidateRequiredField(pars.frm.find('input[name="name"]')[0])) {
                                    pars.model.set({
                                        Name: $(pars.frm.find('input[name="name"]')[0]).val(),
                                        InternalProfile: FreeswitchConfig.Web.Setup.SipProfiles.get($(pars.frm.find('select[name="internalProfile"]')[0]).val()),
                                        ExternalProfile: FreeswitchConfig.Web.Setup.SipProfiles.get($(pars.frm.find('select[name="externalProfile"]')[0]).val())
                                    });
                                    FreeswitchConfig.Site.Modals.HideUpdating();
                                    FreeswitchConfig.Site.Modals.HideFormPanel();
                                } else {
                                    FreeswitchConfig.Site.Modals.HideUpdating();
                                    alert('You must specify a domain name.');
                                }
                            },
                            { frm: frm, model: event.data.model }
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
                FreeswitchConfig.Site.Modals.HideLoading();
            });
        });
        var cvContexts = new FreeswitchConfig.Core.Context.CollectionView({ collection: FreeswitchConfig.Web.Setup.Contexts });
        cvContexts.on('render', function(view) {
            if (FreeswitchConfig.Web.Setup.SipProfiles.length == 0) {
                FreeswitchConfig.Web.Setup.SipProfiles.fetch();
            }
        });
        cvContexts.on('item_render', function(view) {
            var spn = $(view.$el.find(FreeswitchConfig.Site.Skin.span.Tag + '.delete')[0]);
            spn.bind('click',
            { model: view.model },
            function(event) {
                FreeswitchConfig.Site.Modals.ShowLoading();
                event.data.model.destroy();
                FreeswitchConfig.Site.Modals.HideLoading();
            });
            spn = $(view.$el.find(FreeswitchConfig.Site.Skin.span.Tag + '.edit')[0]);
            spn.bind('click',
            { model: view.model },
            function(event) {
                FreeswitchConfig.Site.Modals.ShowLoading();
                var frm = FreeswitchConfig.Web.Setup.ContextForm(event.data.model);
                FreeswitchConfig.Site.Modals.ShowFormPanel(
                    'Edit Context',
                    frm,
                    [
                        CreateButton(
                            'accept',
                            'Okay',
                            function(button, pars) {
                                FreeswitchConfig.Site.Modals.ShowUpdating();
                                var con = FreeswitchConfig.Web.Setup.ValidateContextForm(pars);
                                if (con != null) {
                                    pars.model.set(con.attributes);
                                    FreeswitchConfig.Site.Modals.HideUpdating();
                                    FreeswitchConfig.Site.Modals.HideFormPanel();
                                } else {
                                    FreeswitchConfig.Site.Modals.HideUpdating();
                                    alert('Please correct the fields in error in order to update the selected Context<br/>*All controller ports must be between 1025 and 65535');
                                }
                            },
                            { frm: frm, model: event.data.model }
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
                FreeswitchConfig.Site.Modals.HideLoading();
            });
        });
        var cvSipProfiles = new FreeswitchConfig.Core.SipProfile.CollectionView({ collection: FreeswitchConfig.Web.Setup.SipProfiles });
        cvSipProfiles.on('render', function(view) {
            if (FreeswitchConfig.Web.Setup.Settings === null) {
                FreeswitchConfig.Site.Modals.HideLoading();
            } else if (FreeswitchConfig.Web.Setup.Settings.length == 0) {
                FreeswitchConfig.Web.Setup.Settings.fetch();
            }
        });
        cvSipProfiles.on('item_render', function(view) {
            var spn = $(view.$el.find(FreeswitchConfig.Site.Skin.span.Tag + '.delete')[0]);
            spn.bind('click',
            { model: view.model },
            function(event) {
                FreeswitchConfig.Site.Modals.ShowLoading();
                event.data.model.destroy();
                FreeswitchConfig.Site.Modals.HideLoading();
            });
            spn = $(view.$el.find(FreeswitchConfig.Site.Skin.span.Tag + '.edit')[0]);
            spn.bind('click',
            { model: view.model },
            function(event) {
                FreeswitchConfig.Site.Modals.ShowLoading();
                var frm = FreeswitchConfig.Web.Setup.SipProfileForm(event.data.model);
                FreeswitchConfig.Site.Modals.ShowFormPanel(
                    'Edit Sip Profile',
                    frm,
                    [
                        CreateButton(
                            'accept',
                            'Okay',
                            function(button, pars) {
                                FreeswitchConfig.Site.Modals.ShowUpdating();
                                var con = FreeswitchConfig.Web.Setup.ValidateProfileForm(pars);
                                if (con != null) {
                                    pars.model.set(con.attributes);
                                    FreeswitchConfig.Site.Modals.HideUpdating();
                                    FreeswitchConfig.Site.Modals.HideFormPanel();
                                } else {
                                    FreeswitchConfig.Site.Modals.HideUpdating();
                                    alert('Please correct the fields in error in order to update the selected Context<br/>*All controller ports must be between 1025 and 65535');
                                }
                            },
                            { frm: frm, model: event.data.model }
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
                FreeswitchConfig.Site.Modals.HideLoading();
            });
        });
        var frmUser = null;
        if (!IS_SETUP) {
            frmUser = FreeswitchConfig.Site.Form.GenerateForm(
                null,
                [
                    new FreeswitchConfig.Site.Form.FormInput('username', 'text', null, null, 'User Name:'),
                    new FreeswitchConfig.Site.Form.FormInput('firstName', 'text', null, null, 'First Name:'),
                    new FreeswitchConfig.Site.Form.FormInput('lastName', 'text', null, null, 'Last Name:'),
                    new FreeswitchConfig.Site.Form.FormInput('password', 'password', null, null, 'Password:'),
                    new FreeswitchConfig.Site.Form.FormInput('confirmPassword', 'password', null, null, 'Confirm Password:')
                ]
            );
            FreeswitchConfig.Web.Setup.Settings = new FreeswitchConfig.Core.SystemSetting.Collection();
            var cvSettings = new FreeswitchConfig.Core.SystemSetting.CollectionView({ collection: FreeswitchConfig.Web.Setup.Settings });
            cvSettings.on('render', function(view) {
                var col = view.collection;
                var trs = view.$el.find('tbody>tr');
                for (var x = 0; x < trs.length; x++) {
                    var but = $($(trs[x]).find('span.edit')[0]);
                    but.bind('click', { tr: $(trs[x]), model: col.get(x) }, function(event) {
                        if (event.model.get('ValueType') == 'System.Boolean') {
                            var inp = $(event.data.tr.find('input[name="Value"]')[0]);
                            inp.after($('<input class="' + inp.attr('class') + ' radTrue" type="radio" name="' + inp.attr('name') + '" proptype="System.Boolean" value="true"/><label class="' + inp.attr('class') + ' lblTrue">True</label><input class="' + inp.attr('class') + ' radTrue" type="radio" name="' + inp.attr('name') + '" proptype="System.Boolean" value="false"/><label class="' + inp.attr('class') + ' lblTrue">False</label>'));
                            $(event.data.tr.find('input[name="' + inp.attr('name') + '"][value="' + inp.val() + '"]')[0]).attr('checked', true);
                            inp.remove();
                        }
                    });
                }
                FreeswitchConfig.Site.Modals.HideLoading();
            });
        }

        var butSaveSetup = CreateButton(
            'disk',
            'Save Setup',
            function(button, pars) {
                FreeswitchConfig.Site.Modals.ShowUpdating();
                var errors = [];
                if (!IS_SETUP) {
                    if (!FreeswitchConfig.Site.Validation.ValidateRequiredField(pars.frmUser.find('input[name="username"]')[0])) {
                        errors.push(FreeswitchConfig.Site.li.Create('You must specify a user name'));
                    }
                    if (!FreeswitchConfig.Site.Validation.ValidateRequiredField(pars.frmUser.find('input[name="firstName"]')[0])) {
                        errors.push(FreeswitchConfig.Site.li.Create('You must specify a first name'));
                    }
                    if (!FreeswitchConfig.Site.Validation.ValidateRequiredField(pars.frmUser.find('input[name="lastName"]')[0])) {
                        errors.push(FreeswitchConfig.Site.li.Create('You must specify a last name'));
                    }
                    if (!FreeswitchConfig.Site.Validation.ValidateRequiredField(pars.frmUser.find('input[name="password"]')[0])) {
                        errors.push(FreeswitchConfig.Site.li.Create('You must specify a password'));
                    }
                    if ($(pars.frmUser.find('input[name="confirmPassword"]')[0]).val() !=
                    $(pars.frmUser.find('input[name="password"]')[0]).val()) {
                        errors.push(FreeswitchConfig.Site.li.Create('Password and Confirm Password do not match'));
                    }
                }
                if (errors.length == 0) {
                    for (var x = 0; x < FreeswitchConfig.Web.Setup.Domains.length; x++) {
                        var save = false;
                        var dom = FreeswitchConfig.Web.Setup.Domains.at(x);
                        var iprof = dom.get('InternalProfile');
                        var icont = iprof.get('Context');
                        var eprof = dom.get('ExternalProfile');
                        var econt = eprof.get('Context');
                        if (icont.isNew()) {
                            icont.syncSave();
                            iprof.syncSave();
                            save = true;
                        } else {
                            if (icont.hasChanged() != false) {
                                icont.syncSave();
                                save = true;
                            }
                            if (iprof.isNew() || iprof.hasChanged() != false) {
                                iprof.syncSave();
                                save = true;
                            }
                        }
                        if (econt.isNew()) {
                            econt.syncSave();
                            eprof.syncSave();
                            save = true;
                        } else {
                            if (econt.hasChanged() != false) {
                                econt.syncSave();
                                save = true;
                            }
                            if (eprof.isNew() || eprof.hasChanged() != false) {
                                eprof.syncSave();
                                save = true;
                            }
                        }
                        if (save || dom.isNew() || dom.hasChanged() != false) {
                            dom.syncSave();
                        }
                    }
                    var Domains = new FreeswitchConfig.Core.Domain.Collection();
                    for (var x = 0; x < Domains.length; x++) {
                        var found = false;
                        for (var y = 0; y < FreeswitchConfig.Web.Setup.Domains.length; y++) {
                            if (FreeswitchConfig.Web.Setup.Domains.at(y).id == Domains.at(x).id) {
                                found = true;
                                y = FreeswitchConfig.Web.Setup.Domains.length;
                            }
                        }
                        if (!found) {
                            Domains.at(x).destroy({ wait: true });
                        }
                    }
                    var Contexts = new FreeswitchConfig.Core.Context.Collection();
                    for (var x = 0; x < Contexts.length; x++) {
                        var found = false;
                        for (var y = 0; y < FreeswitchConfig.Web.Setup.Contexts.length; y++) {
                            if (FreeswitchConfig.Web.Setup.Contexts.at(y).id == Contexts.at(x).id) {
                                found = true;
                                y = FreeswitchConfig.Web.Setup.Contexts.length;
                            }
                        }
                        if (!found) {
                            Contexts.at(x).destroy({ wait: true });
                        }
                    }
                    var SipProfiles = new FreeswitchConfig.Core.SipProfile.Collection();
                    for (var x = 0; x < SipProfiles.length; x++) {
                        var found = false;
                        for (var y = 0; y < FreeswitchConfig.Web.Setup.SipProfiles.length; y++) {
                            if (FreeswitchConfig.Web.Setup.SipProfiles.at(y).id == SipProfiles.at(x).id) {
                                found = true;
                                y = FreeswitchConfig.Web.Setup.SipProfiles.length;
                            }
                        }
                        if (!found) {
                            SipProfiles.at(x).destroy({ wait: true });
                        }
                    }
                    if (!IS_SETUP) {
                        var url = '/core/SetupComplete?UserName=' + $(pars.frmUser.find('input[name="username"]')[0]).val() +
                        '&FirstName=' + $(pars.frmUser.find('input[name="firstName"]')[0]).val() +
                        '&LastName=' + $(pars.frmUser.find('input[name="lastName"]')[0]).val() +
                        '&Password=' + $(pars.frmUser.find('input[name="password"]')[0]).val();
                        $.ajax(url, {
                            success: function(msg) {
                                FreeswitchConfig.Site.Modals.HideUpdating();
                                alert('Initial setup complete, reloading site...', function() {
                                    location.href = '/';
                                });
                            },
                            error: function(msg) {
                                FreeswitchConfig.Site.Modals.HideUpdating();
                                alert('An error occured completing the initial setup: ' + msg);
                            }
                        })
                    } else {
                        FreeswitchConfig.Site.Modals.HideUpdating();
                    }
                } else {
                    alert('You must correct the folloring errors: ' + FreeswitchConfig.Site.Skin.ul.Create({ Content: errors }).prop('outerHTML'));
                    FreeswitchConfig.Site.Modals.HideUpdating();
                }
            },
            { frmUser: frmUser }
        );

        dv = FreeswitchConfig.Site.Skin.div.Create({ Attributes: { style: 'width:100%;float:left;'} });
        dv.append(butSaveSetup);
        container.append(dv);
        //create contexts section
        dv = FreeswitchConfig.Site.Skin.div.Create({ Class: 'SetupComponentContainer', Content:
            FreeswitchConfig.Site.Skin.div.Create({ Class: 'shadow', Content: [
                FreeswitchConfig.Site.Skin.div.Create({ Class: 'HeaderBar', Content: 'Contexts' }),
                FreeswitchConfig.Site.Skin.div.Create({ Class: 'Content', Content: [
                    FreeswitchConfig.Site.Skin.p.Create('This section is where you setup the initial contexts of the system.  A context is essentially a grouping of the dial plan rules, SIP profiles are bound to these contexts to know what to do with an incoming call.  The minimum required is 2, the defaults are public (for all incoming calls) and private (for all calls originating internally).'),
                    CreateButton(
                        'add',
                        'Add New Context',
                        function(button) {
                            FreeswitchConfig.Site.Modals.ShowLoading();
                            var frm = FreeswitchConfig.Web.Setup.ContextForm();
                            FreeswitchConfig.Site.Modals.ShowFormPanel(
                                        'Add Context',
                                        frm,
                                        [
                                            CreateButton(
                                                'accept',
                                                'Okay',
                                                function(button, pars) {
                                                    FreeswitchConfig.Site.Modals.ShowUpdating();
                                                    var con = FreeswitchConfig.Web.Setup.ValidateContextForm(pars);
                                                    if (con != null) {
                                                        FreeswitchConfig.Web.Setup.Contexts.add(con);
                                                        FreeswitchConfig.Site.Modals.HideUpdating();
                                                        FreeswitchConfig.Site.Modals.HideFormPanel();
                                                    } else {
                                                        FreeswitchConfig.Site.Modals.HideUpdating();
                                                        alert('Please correct the fields in error in order to update the selected Context<br/>*All controller ports must be between 1025 and 65535');
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
                            FreeswitchConfig.Site.Modals.HideLoading();
                        }
                    ),
                    cvContexts.$el
                ]
                })
            ]
            })
        });
        container.append(dv);

        //sip profiles section
        dv = FreeswitchConfig.Site.Skin.div.Create({Class:'SetupComponentContainer',Attributes:{style:'max-width:500px;'},Content:
            FreeswitchConfig.Site.Skin.div.Create({ Class: 'shadow', Content: [
                FreeswitchConfig.Site.Skin.div.Create({ Class: 'HeaderBar', Content: 'SIP Profiles' }),
                FreeswitchConfig.Site.Skin.div.Create({ Class: 'Content', Content: [
                    FreeswitchConfig.Site.Skin.p.Create('This section is where you specify which IP addresses/ports that freeswitch will bind to.  Each profile 	                    is setup to bind to 1 interface and use specific ports for the required task of being a SIP server.  The 	                    minimum required is 2, the defaults are public (for all external SIP clients, uses an External Context) and	                    private (for all internally networked clients, uses an Internal Context).'),
                    CreateButton(
                        'add',
                        'Add New Profile',
                        function(button) {
                            FreeswitchConfig.Site.Modals.ShowLoading();
                            var frm = FreeswitchConfig.Web.Setup.SipProfileForm();
                            FreeswitchConfig.Site.Modals.ShowFormPanel(
                                'Add Profile',
                                frm,
                                [
                                    CreateButton(
                                        'accept',
                                        'Okay',
                                        function(button, pars) {
                                            FreeswitchConfig.Site.Modals.ShowUpdating();
                                            var profile = FreeswitchConfig.Web.Setup.ValidateProfileForm(pars);
                                            if (profile != null) {
                                                FreeswitchConfig.Web.Setup.SipProfiles.add(profile);
                                                FreeswitchConfig.Site.Modals.HideUpdating();
                                                FreeswitchConfig.Site.Modals.HideFormPanel();
                                            } else {
                                                FreeswitchConfig.Site.Modals.HideUpdating();
                                                alert('Please correct the fields in error in order to update the selected Profile<br/>*All profile ports must be between 1025 and 65535');
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
                            FreeswitchConfig.Site.Modals.HideLoading();
                        }
                    ),
                    cvSipProfiles.$el
                ]})
            ]})
        });
        container.append(dv);

        //domains section
        dv = FreeswitchConfig.Site.Skin.div.Create({Class:'SetupComponentContainer',Attributes:{style:'max-width:500px;'},Content:
            FreeswitchConfig.Site.Skin.div.Create({ Class: 'shadow', Content: [
                FreeswitchConfig.Site.Skin.div.Create({ Class: 'HeaderBar', Content: 'Domains' }),
                FreeswitchConfig.Site.Skin.div.Create({ Class: 'Content', Content: [
                    FreeswitchConfig.Site.Skin.p.Create('This section is where you specify the domains that this server is responsible for.  Each domain is a collection 	                    of extensions and requires an external and an internal SIP Profile that it uses for handling authentications as 	                    well as call routing.  Default will be the IP of the first SIP internal profile.'),
                    CreateButton(
                        'add',
                        'Add Domain',
                        function(button) {
                            FreeswitchConfig.Site.Modals.ShowLoading();
                            var frm = FreeswitchConfig.Web.Setup.DomainForm();
                            FreeswitchConfig.Site.Modals.ShowFormPanel(
                                'Add Domain',
                                frm,
                                [
                                    CreateButton(
                                        'accept',
                                        'Okay',
                                        function(button, pars) {
                                            FreeswitchConfig.Site.Modals.ShowUpdating();
                                            if (FreeswitchConfig.Site.Validation.ValidateRequiredField(pars.frm.find('input[name="name"]')[0])) {
                                                var domain = new FreeswitchConfig.Core.Domain.Model({
                                                    Name: $(pars.frm.find('input[name="name"]')[0]).val(),
                                                    InternalProfile: FreeswitchConfig.Web.Setup.SipProfiles.get($(pars.frm.find('select[name="internalProfile"]')[0]).val()),
                                                    ExternalProfile: FreeswitchConfig.Web.Setup.SipProfiles.get($(pars.frm.find('select[name="externalProfile"]')[0]).val())
                                                });
                                                FreeswitchConfig.Web.Setup.Domains.add(domain);
                                                FreeswitchConfig.Site.Modals.HideUpdating();
                                                FreeswitchConfig.Site.Modals.HideFormPanel();
                                            } else {
                                                FreeswitchConfig.Site.Modals.HideUpdating();
                                                alert('You must specify a domain name.');
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
                            FreeswitchConfig.Site.Modals.HideLoading();
                        }
                    ),
                    cvDomains.$el
                ]})   
            ]})
        });
        container.append(dv);

        //system settings
        if (!IS_SETUP) {
            //initial user
            dv = $('<div class="SetupComponentContainer" style="max-width:500px;"></div>');
            dv.append('<div class="shadow"><div class="HeaderBar">Initial User</div><div class="Content"><p>This section is where you specify the initial user for the system.</p></div></div>');
            d = $(dv.find('div.Content')[0]);
            d.append(frmUser);
            container.append(dv);

            dv = $('<div class="SetupComponentContainer" style="max-width:500px;"></div>');
            dv.append('<div class="shadow"><div class="HeaderBar">System Settings</div><div class="Content"><p>This section is where you specify the core system settings that the system will use.</p></div></div>');
            d = $(dv.find('div.Content')[0]);
            d.append(cvSettings.$el);
            container.append(dv);
        }

        FreeswitchConfig.Web.Setup.Domains.fetch();
    },
    ValidateContextForm: function(pars) {
        var inpName = $(pars.frm.find('input[name="name"]')[0]);
        var selType = $(pars.frm.find('select[name="type"]')[0]);
        var inpIP = $(pars.frm.find('input[name="controlIP"]')[0]);
        var inpPort = $(pars.frm.find('input[name="controlPort"]')[0]);
        var inpDescription = $(pars.frm.find('textarea[name="description"]')[0]);
        var canSubmit = FreeswitchConfig.Site.Validation.ValidateRequiredField(inpName);
        if (FreeswitchConfig.Site.Validation.ValidateRequiredField(inpIP)) {
            canSubmit |= FreeswitchConfig.Site.Validation.ValidateIPAddressField(inpIP);
        } else {
            canSubmit = false;
        }
        if (FreeswitchConfig.Site.Validation.ValidateRequiredField(inpPort)) {
            canSubmit |= FreeswitchConfig.Site.Validation.ValidateIntegerInRange(inpPort, 1025, 65535);
        } else {
            canSubmit = false;
        }
        if (canSubmit) {
            var con = new FreeswitchConfig.Core.Context.Model({
                Name: inpName.val(),
                Type: selType.val(),
                SocketIP: inpIP.val(),
                SocketPort: inpPort.val(),
                Description: (inpDescription.val() == '' ? null : inpDescription.val())
            });
            return con;
        } else {
            return null;
        }
    },
    ValidateProfileForm: function(pars) {
        var inpName = $(pars.frm.find('input[name="name"]')[0]);
        var selContext = $(pars.frm.find('select[name="context"]>option:selected')[0]);
        var inpPort = $(pars.frm.find('input[name="port"]')[0]);
        var selSIP = $(pars.frm.find('select[name="sipInterface"]>option:selected')[0]);
        var selRTP = $(pars.frm.find('select[name="rtpInterface"]>option:selected')[0]);
        var canSubmit = FreeswitchConfig.Site.Validation.ValidateRequiredField(inpName);
        if (FreeswitchConfig.Site.Validation.ValidateRequiredField(inpPort)) {
            canSubmit |= FreeswitchConfig.Site.Validation.ValidateIntegerInRange(inpPort, 1025, 65535);
        } else {
            canSubmit = false;
        }
        if (canSubmit) {
            var profile = new FreeswitchConfig.Core.SipProfile.Model({
                Name: inpName.val(),
                SIPPort: inpPort.val(),
                Context: FreeswitchConfig.Web.Setup.Contexts.get(selContext.val()),
                SIPInterface: FreeswitchConfig.Web.Setup.Interfaces.get(selSIP.val()),
                RTPInterface: FreeswitchConfig.Web.Setup.Interfaces.get(selRTP.val())
            });
            return profile;
        } else {
            return null;
        }
    }
});
