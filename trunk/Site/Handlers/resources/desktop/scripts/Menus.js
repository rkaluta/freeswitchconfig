FreeswitchConfig.Site.MainMenuItem = {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: "span",
        render: function() {
            this.$el.attr('class', this.model.get('Name'));
            this.$el.html(this.model.get('Title'));
            this.$el.unbind('click');
            if (this.model.get('Name') != 'ReloadMenus') {
                this.$el.bind('click',
                { model: this.model },
                function(event) {
                    FreeswitchConfig.Site.Modals.ShowLoading();
                    if (event.data.model.get('ClearMainWindow')) {
                        FreeswitchConfig.Site.clearChangedDomain();
                        HideSideMenu();
                        $('#MainContent').html('');
                        $('#MasterButtonsContainer').html('');
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
                        eval(event.data.model.get('GenerateFunction') + '($(\'#MainContent\'))');
                    } else {
                        var trows = new Array();
                        for (var x = 0; x < event.data.model.get('SubMenus').length; x++) {
                            trows.push(
                                    new FreeswitchConfig.Site.Tables.Row([
                                        new FreeswitchConfig.Site.Tables.Cell(
                                            CreateButton(
                                                null,
                                                event.data.model.get('SubMenus')[x].Name,
                                                function(button, evnt) {
                                                    FreeswitchConfig.Site.Modals.ShowLoading();
                                                    $('#MainContent').html('');
                                                    if (evnt.menuItem.CssURLs != null) {
                                                        for (var x = 0; x < evnt.menuItem.CssURLs.length; x++) {
                                                            loadjscssfile(evnt.menuItem.CssURLs[x], 'css');
                                                        }
                                                    }
                                                    if (evnt.menuItem.JavascriptURLs != null) {
                                                        for (var x = 0; x < evnt.menuItem.JavascriptURLs.length; x++) {
                                                            loadjscssfile(evnt.menuItem.JavascriptURLs[x], 'js');
                                                        }
                                                    }
                                                    eval(evnt.menuItem.GenerateFunction + '($(\'#MainContent\'))');
                                                },
                                                { menuItem: event.data.model.get('SubMenus')[x] }
                                            )
                                        )
                                    ]));
                        }
                        $('#MasterButtonsContainer').append(
                                FreeswitchConfig.Site.Tables.Render(trows)
                            );
                        ShowSideMenu();
                    }
                    FreeswitchConfig.Site.Modals.HideLoading();
                });
            }
            return this;
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: "div",
        className: "ButtonContainer",
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
