FreeswitchConfig.Site.MainMenuItem = {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: "li",
        className: "ui-btn ui-btn-icon-right ui-li-has-arrow ui-li ui-li-has-thumb ui-btn-up-c",
        attributes: {
            'data-corners': false,
            'data-shadow': false,
            'data-iconshadow': true,
            'data-wrapperels': 'div',
            'data-icon': 'arrow-r',
            'data-iconpos': 'right',
            'data-theme': 'c'
        },
        render: function() {
            this.$el.html('<div class="ui-btn-inner ui-li"><div class="ui-btn-text"><a name="' + this.model.get('Name') + '" class="ui-link-inherit"><img class="ui-li-thumb '+this.model.get('Name')+'"/><h3 class="ui-li-heading">' + this.model.get('Title') + '</h3></a></div><span class="ui-icon ui-icon-arrow-r ui-icon-shadow">&nbsp;</span></div>');
            this.$el.unbind('click');
            if (this.model.get('Name') != 'ReloadMenus') {
                this.$el.bind('click',
                { model: this.model },
                function(event) {
                    FreeswitchConfig.Site.Modals.ShowLoading();
                    if (event.data.model.get('ClearMainWindow')) {
                        FreeswitchConfig.Site.clearChangedDomain();
                        $('#pgMainContent').html('');
                    }
                    if (event.data.model.get('CssURLs') != null) {
                        for (var x = 0; x < event.data.model.get('CssURLs').length; x++) {
                            loadjscssfile(event.data.model.get('CssURLs')[x], 'css');
                        }
                    }
                    if (event.data.model.get('JavascriptURLs') != null) {
                        for (var x = 0; x < event.data.model.get('JavascriptURLs').length; x++) {
                            loadjscssfile(event.data.model.get('JavascriptURLs')[x], 'js');
                        }
                    }
                    if (event.data.model.get('GenerateFunction') != null) {
                        FreeswitchConfig.Site.Modals.previousPages.push($('#pgMainContainer'));
                        $('#MainContent').html('');
                        ShowBack($('#pgMainMenu'));
                        eval(event.data.model.get('GenerateFunction') + '($(\'#MainContent\'))');
                    } else {
                    FreeswitchConfig.Site.Modals.previousPages.push($('#pgSubMenu'));
                        ShowBack($('#pgSubMenu'));
                        var butCont = $('#liSubMenu');
                        butCont.html('');
                        for (var x = 0; x < event.data.model.get('SubMenus').length; x++) {
                            butCont.append('<li class="ui-btn ui-btn-icon-right ui-li-has-arrow ui-li ui-btn-up-c" data-corners="false" data-shadow="false" data-iconshadow="true" data-wrapperels="div" data-icon="arrow-r" data-iconpos="right" data-theme="c"><div class="ui-btn-inner ui-li"><div class="ui-btn-text"><a class="ui-link-inherit"></a></div><span class="ui-icon ui-icon-arrow-r ui-icon-shadow">&nbsp;</span></div></li>');
                            var but = $(butCont.find('a:last')[0]);
                            but.attr('name', event.data.model.get('SubMenus')[x].Name);
                            but.append(event.data.model.get('SubMenus')[x].Name.replace('<br/>', ''));
                            but.bind('click',
                            { 
                                menuItem: event.data.model.get('SubMenus')[x] },
                                function(evnt) {
                                    FreeswitchConfig.Site.Modals.ShowLoading();
                                    $('#MainContent').html('');
                                    if (evnt.data.menuItem.CssURLs != null) {
                                        for (var x = 0; x < evnt.data.menuItem.CssURLs.length; x++) {
                                            loadjscssfile(evnt.data.menuItem.CssURLs[x], 'css');
                                        }
                                    }
                                    if (evnt.data.menuItem.JavascriptURLs != null) {
                                        for (var x = 0; x < evnt.data.menuItem.JavascriptURLs.length; x++) {
                                            loadjscssfile(evnt.data.menuItem.JavascriptURLs[x], 'js');
                                        }
                                    }
                                    FreeswitchConfig.Site.Modals.previousPages.push($('#pgMainContainer'));
                                    eval(evnt.data.menuItem.GenerateFunction + '($(\'#MainContent\'))');
                                    FreeswitchConfig.Site.Modals.HideLoading();
                                }
                            );
                    }
                    ShowSideMenu();
                }
                FreeswitchConfig.Site.Modals.HideLoading();
                });
            }
            return this;
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: "ul",
        className: "ui-listview",
        attributes: {
            id: 'liMainMenu',
            'data-role': 'listview'
        },
        initialize: function() {
            this.collection.on('reset', this.render, this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        render: function() {
            var el = this.$el;
            el.html('');
            if (this.collection.length == 0) {
                this.trigger('render', this);
            } else {
                for (var x = 0; x < this.collection.length; x++) {
                    var vw = new FreeswitchConfig.Site.MainMenuItem.View({ model: this.collection.at(x) });
                    if (x + 1 == this.collection.length) {
                        vw.on('render', function() { this.col.trigger('item_render', this.view); this.col.trigger('render', this.col); }, { col: this, view: vw });
                    } else {
                        vw.on('render', function() { this.col.trigger('item_render', this.view); }, { col: this, view: vw });
                    }
                    el.append(vw.$el);
                    vw.render();
                    if (this.collection.at(x).get('Name') == 'ReloadMenus') {
                        vw.$el.bind('click',
                        { collection: this.collection, view: this },
                        function(event) {
                            FreeswitchConfig.Site.Modals.ShowLoading();
                            event.data.collection.fetch(
                                {
                                    success: function() { FreeswitchConfig.Site.Modals.HideLoading(); },
                                    error: function() { FreeswitchConfig.Site.Modals.HideLoading(); }
                                }
                            );
                        });
                    }
                }
            }
        }
    })
};
