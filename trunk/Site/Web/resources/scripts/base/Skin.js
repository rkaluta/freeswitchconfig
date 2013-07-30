CreateNameSpace('FreeswitchConfig.Site.Skin');

FreeswitchConfig.Site.Skin.baseObject = {
    Tag : '',
    Class : '',
    AltClass: '',
    Attributes:{},
    Create : function(options){
      options = (options == undefined ? {} : (options == null ? {} : (typeof (options) == 'string' ? { Content: options} : options)));
      options = $.extend(true,$.extend(true,{},{ Class: '', Attributes: this.Attributes, Content: [] }), options);
      var ret = $('<' + this.Tag + ' class="' + options.Class + ' ' + this.Class + '"></' + this.Tag + '>');
      for (var x in options.Attributes) {
        ret.attr(x, options.Attributes[x]);
      }
      ret.append(options.Content);
      return ret;
    },
    CreateAlt : function(options) {
        options = (options == undefined ? {} : (options == null ? {} : (typeof (options) == 'string' ? { Content: options} : options)));
        options = $.extend({ Class: '', Attributes: {}, Content: [] }, options);
        var ret = $('<' + this.Tag + ' cellspacing="0" class="' + options.Class + ' ' + this.AltClass + '"></' + this.Tag + '>');
        for (var x in options.Attributes) {
            ret.attr(x, options.Attributes[x]);
        }
        ret.append(options.Content);
        return ret;
    }
};

FreeswitchConfig.Site = $.extend(FreeswitchConfig.Site, { Skin: {
    table: $.extend({}, FreeswitchConfig.Site.Skin.baseObject, {
        Tag: 'table',
        Class: 'Rowed',
        Attributes: { cellspacing: 0, cellpadding: 0 }
    }),
    thead: $.extend({}, FreeswitchConfig.Site.Skin.baseObject, {
        Tag: 'thead'
    }),
    tbody: $.extend({}, FreeswitchConfig.Site.Skin.baseObject, {
        Tag: 'tbody'
    }),
    tr: $.extend({}, FreeswitchConfig.Site.Skin.baseObject, {
        Tag: 'tr',
        AltClass: 'Alt'
    }),
    td: $.extend({}, FreeswitchConfig.Site.Skin.baseObject, {
        Tag: 'td'
    }),
    th: $.extend({}, FreeswitchConfig.Site.Skin.baseObject, {
        Tag: 'th'
    }),
    span: $.extend({}, FreeswitchConfig.Site.Skin.baseObject, {
        Tag: 'span'
    }),
    div: $.extend({}, FreeswitchConfig.Site.Skin.baseObject, {
        Tag: 'div'
    }),
    p: $.extend({}, FreeswitchConfig.Site.Skin.baseObject, {
        Tag: 'p'
    }),
    ul: $.extend({}, FreeswitchConfig.Site.Skin.baseObject, {
        Tag: 'ul'
    }),
    li: $.extend({}, FreeswitchConfig.Site.Skin.baseObject, {
        Tag: 'li'
    }),
    ol: $.extend({}, FreeswitchConfig.Site.Skin.baseObject, {
        Tag: 'ol'
    }),
    img: $.extend({}, FreeswitchConfig.Site.Skin.baseObject, {
        Tag:'img'
    }),
    h3: $.extend({}, FreeswitchConfig.Site.Skin.baseObject, {
        Tag:'h3'
    })
}
});