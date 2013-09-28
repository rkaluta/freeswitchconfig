FreeswitchConfig.Site.MainMenuItem = $.extend(FreeswitchConfig.Site.MainMenuItem, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: "li",
        render: function() {
            this.$el.attr('class', this.model.get('Name'));
            this.$el.html('<span>' + this.model.get('Title') + '</span>');
            if (this.model.get('GenerateFunction') == null) {
                var ul = $('<ul class="sub_menu"></ul>');
                for (var x = 0; x < this.model.get('SubMenus').length; x++) {
                    var li = $('<li>' + this.model.get('SubMenus')[x].Name + '</li>');
                    li.bind('click', { method: this.model.get('SubMenus')[x].GenerateFunction }, function(event) {
                        FreeswitchConfig.Site.Modals.ShowLoading();
                        $($($.find('div.main_menu_container')[0]).find('ul.sub_menu')).hide();
                        $($.find('div.main_menu_container')[0]).animate({ left: -300 });
                        $('#MainContainer').html('');
                        eval(event.data.method + '($(\'#MainContainer\'))');
                    });
                    ul.append(li);
                }
                this.$el.append(ul);
            }
            this.trigger('render');
        },
        events: { 'click span': 'menuClick' }
    })
});
