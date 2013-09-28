FreeswitchConfig.Site.MainMenuItem = $.extend(FreeswitchConfig.Site.MainMenuItem, {
View: FreeswitchConfig.Site.MainMenuItem.View.extend({
    subMenuClick: function(event) {
        var li = $(event.target);
        FreeswitchConfig.Site.Modals.ShowLoading();
        $($($.find('div.main_menu_container')[0]).find('ul.sub_menu')).hide();
        $($.find('div.main_menu_container')[0]).animate({ left:-300 });
        $('#MainContainer').html('');
        eval(this.model.get('SubMenus')[li.attr('index')].GenerateFunction + '($(\'#MainContainer\'))');
    },
        menuClick: function() {
            var ul = $(this.$el.parent());
            $(ul.find('ul.sub_menu')).hide();
            if (this.model.get('GenerateFunction') == null) {
                $(this.$el.find('ul.sub_menu')).show();
            } else {
                FreeswitchConfig.Site.Modals.ShowLoading();
                $(this.$el.find('ul.sub_menu')).hide();
                $($.find('div.main_menu_container')[0]).animate({ left: -300 });
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
    }),
    SetupMenu: function() {
        FreeswitchConfig.Site.Modals.ShowLoading();
        if ($.find('img.main_menu_button').length == 0) {
            var butMenu = $('<img src="/resources/images/menu_button.png" class="main_menu_button"/>');
            $(document.body).append(butMenu);
        } else {
            var butMenu = $($.find('img.main_menu_button')[0]);
        }
        if ($.find('div.main_menu_container').length == 0) {
            var dvMenu = $('<div class="main_menu_container"><div class="menu_border"></div></div>');
            $(document.body).append(dvMenu);
        } else {
            var dvMenu = $($.find('div.main_menu_container')[0]);
            dvMenu.html($('<div class="menu_border"></div>'));
        }
        butMenu.bind('click', { dvMenu: dvMenu },
        function(event) {
            event.data.dvMenu.animate({ left: 0 });
        });
        var vw = new FreeswitchConfig.Site.MainMenuItem.CollectionView({ collection: new FreeswitchConfig.Site.MainMenuItem.Collection() });
        vw.on('render', function() {
            FreeswitchConfig.Site.Modals.HideLoading();
        });
        $(dvMenu.find('div.menu_border')[0]).append(vw.$el);
        vw.collection.fetch();
    }
});
