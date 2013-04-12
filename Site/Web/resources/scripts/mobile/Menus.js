FreeswitchConfig.Site.MainMenuItem = $.extend(FreeswitchConfig.Site.MainMenuItem, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: "li",
        attributes: {
            'data-corner': 'false',
            'data-shadow': 'false',
            'data-iconshadow': 'true',
            'data-icon': 'arrow-r',
            'data-iconpos': 'right',
            'data-theme': 'c'
        },
        render: function() {
            this.$el.attr('class', this.model.get('Name'));
            this.$el.html('<a href="#">' + this.model.get('Title') + '</a>');
            if (this.model.get('GenerateFunction') == null) {
                var dvMenu = $('<div data-role="panel" data-position="left" data-display="push"></div>');
                $($('#main_menu_container').parent()).prepend(dvMenu);
                dvMenu.panel();
                this.options.dvMenu = dvMenu;
                var ul = $('<ul class="sub_menu" data-role="listview"></ul>');
                dvMenu.append(ul);
                var li = $('<li data-corner="false" data-shadow="false" data-iconshadow="false" data-icon="arrow-l" data-iconpos="left" data-theme="c"><a href="#">Back</a></li>');
                li.bind('click', { dvMenu: dvMenu }, function(event) {
                    event.data.dvMenu.panel('close');
                    $('#main_menu_container').panel('open');
                });
                ul.append(li);
                for (var x = 0; x < this.model.get('SubMenus').length; x++) {
                    li = $('<li data-corner="false" data-shadow="false" data-iconshadow="false" data-icon="arrow-r" data-iconpos="right" data-theme="c"><a href="#">' + this.model.get('SubMenus')[x].Name + '</a></li>');
                    li.bind('click', { method: this.model.get('SubMenus')[x].GenerateFunction, dvMenu: dvMenu }, function(event) {
                        FreeswitchConfig.Site.Modals.ShowLoading();
                        event.data.dvMenu.panel('close');
                        $('#MainContainer').html('');
                        eval(event.data.method + '($(\'#MainContainer\'))');
                    });
                    ul.append(li);
                }
                ul.listview();
            }
            this.trigger('render');
        },
        events: { 'click a': 'menuClick' },
        menuClick: function() {
            if (this.model.get('GenerateFunction') == null) {
                this.options.dvMenu.panel('open');
            } else {
                FreeswitchConfig.Site.Modals.ShowLoading();
                $('#main_menu_container').panel('close');
                $('#MainContainer').html('');
                eval(this.model.get('GenerateFunction') + '($(\'#MainContainer\'))');
            }
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: "ul",
        className: "menu",
        attributes: { 'data-role': 'listview' },
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
                        vw.on('render', function() { this.col.trigger('item_render', this.view); this.col.$el.listview(); this.col.trigger('render', this.col); }, { col: this, view: vw });
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
    }),
    SetupMenu: function() {
        FreeswitchConfig.Site.Modals.ShowLoading();
        var butMenu = $('<a class="ui-btn-left ui-btn ui-shadow ui-btn-corner-all ui-btn-icon-left ui-btn-up-b" data-icon="gear" href="#" data-corners="true" data-shadow="true" data-iconshadow="true" data-wrapperels="span" data-theme="b"></a>');
        butMenu.append('<span class="ui-btn-inner ui-btn-corner-all"><span class="ui-btn-text">Menu</span><span class="ui-icon ui-icon-gear ui-icon-shadow"> </span></span>');
        $($.find('div[data-role="header"]')[0]).prepend(butMenu);
        var dvMenu = $('<div id="main_menu_container" data-role="panel" data-position="left" data-display="push"></div>');
        $($.find('div[data-role="page"]:first')[0]).prepend(dvMenu);
        dvMenu.panel();
        butMenu.bind('click', { dvMenu: dvMenu },
        function(event) {
            dvMenu.panel("open", { display: "push" });
        });
        var vw = new FreeswitchConfig.Site.MainMenuItem.CollectionView({ collection: new FreeswitchConfig.Site.MainMenuItem.Collection() });
        vw.on('render', function() {
            FreeswitchConfig.Site.Modals.HideLoading();
        });
        dvMenu.append(vw.$el);
        vw.collection.fetch();
    }
});
