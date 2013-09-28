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
                    var li = $('<li index="'+x.toString()+'">' + this.model.get('SubMenus')[x].Name + '</li>');
                    ul.append(li);
                }
                this.$el.append(ul);
            }
            this.trigger('render');
        },
        events: { 'click span': 'menuClick',
        'click li':'subMenuClick' }
    })
});
