FreeswitchConfig.Site.MainMenuItem = $.extend(FreeswitchConfig.Site.MainMenuItem, {
    View : FreeswitchConfig.Site.MainMenuItem.View.extend({
        menuClick: function() {
            var ul = $(this.$el.parent());
            $(ul.find('ul.sub_menu')).hide();
            if (this.model.get('GenerateFunction') == null) {
                $(this.$el.find('ul.sub_menu')).show();
            } else {
                FreeswitchConfig.Site.Modals.ShowLoading();
                $(this.$el.find('ul.sub_menu')).hide();
                $($.find('div.main_menu_container')[0]).animate({ width:0 });
                $('#MainContainer').html('');
                eval(this.model.get('GenerateFunction') + '($(\'#MainContainer\'))');
            }
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: "ul",
        className: "menu",
        initialize: function() {
            this.collection.on('sync', this.render, this);
        },
        render: function() {
            var el = this.$el;
            el.html('');
            if (this.collection.length == 0) {
                this.trigger('render', this);
            } else {
                for (var x = 0; x < this.collection.length; x++) {
                    if (this.collection.at(x).get('Name') == 'Logout') {
                        var but = CreateButton(
                            'door_out',
                            'Exit',
                            function(button, pars) {
                                eval(pars.func + '($(\'#MainContainer\'))');
                            },
                            { func: this.collection.at(x).get('GenerateFunction') });
                        but.attr('id', 'logout-button');
                        $(document.body).append(but);
                        if (x + 1 == this.collection.length) {
                            this.trigger('render', this);
                        }
                    }
                    else {
                        var vw = new FreeswitchConfig.Site.MainMenuItem.View({ model: this.collection.at(x) });
                        if (x + 1 == this.collection.length) {
                            vw.on('render', function() { this.col.trigger('item_render', this.view); this.col.trigger('render', this.col); }, { col: this, view: vw });
                        } else {
                            vw.on('render', function() { this.col.trigger('item_render', this.view); }, { col: this, view: vw });
                        }
                        el.append(vw.$el);
                        vw.render();
                    }
                }
            }
        }
    }),
    SetupMenu: function() {
        FreeswitchConfig.Site.Modals.ShowLoading();
        if ($('#menu-button').length == 0) {
            var butMenu = CreateButton(
                'application_cascade',
                'Menu',
                function() {
                    $($.find('div.main_menu_container')[0]).animate({ width:'100%' });
                }
            );
            butMenu.attr('id', 'menu-button');
            $(document.body).append(butMenu);
        }
        if ($.find('div.main_menu_container').length == 0) {
            var dvMenu = $('<div class="main_menu_container"><div class="menu_border"></div></div>');
            $(document.body).append(dvMenu);
        } else {
            var dvMenu = $($.find('div.main_menu_container')[0]);
            dvMenu.html($('<div class="menu_border"></div>'));
        }
        var vw = new FreeswitchConfig.Site.MainMenuItem.CollectionView({ collection: new FreeswitchConfig.Site.MainMenuItem.Collection() });
        vw.on('render', function() {
            FreeswitchConfig.Site.Modals.HideLoading();
        });
        $(dvMenu.find('div.menu_border')[0]).append(vw.$el);
        vw.collection.fetch();
    }
});
