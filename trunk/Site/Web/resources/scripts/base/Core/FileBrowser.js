CreateNameSpace("Org.Reddragonit.FreeSwitchConfig.Site.Models.AudioFileEntry");

Org.Reddragonit.FreeSwitchConfig.Site.Models.AudioFileEntry = $.extend(Org.Reddragonit.FreeSwitchConfig.Site.Models.AudioFileEntry,{
    View : Backbone.View.extend({
        tagName: "span",
        className: "Org Reddragonit FreeSwitchConfig Site Models AudioFileEntry View",
        initialize: function() {
        },
        render: function() {
            var el = this.$el;
            el.html('');
            if (model.get('IsFile')) {
                el.attr('class', el.attr('class') + ' File');
                if (this.input != undefined) {
                    el.bind('dblclick',
                    { input: this.input, button: el },
                    function(event) {
                        $(event.data.input).val($(event.data.button).attr('relpath'));
                        $('#FileBrowser').hide();
                        $('#FileBrowserOverlay').hide();
                    });
                }
            } else {
                el.attr('class', el.attr('class') + ' Folder');
                if (this.viewCollection != undefined) {
                    el.bind('click',
                    { button: el, viewCollection: this.viewCollection },
                    function(event) {
                        FreeswitchConfig.Site.Modals.ShowLoading();
                        var pth = $(event.data.button).attr('relpath');
                        var tmpEntry = new Org.Reddragonit.FreeSwitchConfig.Site.Models.AudioFileEntry.Model({
                            Name: '..',
                            RelativeFilePath: pth.substring(0, pth.lastIndexOf(DIRECTORY_SEPERATOR)),
                            IsFile: false
                        });
                        var col = Org.Reddragonit.FreeSwitchConfig.Site.Models.AudioFileEntry.LoadInPath(pth);
                        if (tmpEntry.get('RelativeFilePath') != '') {
                            col.add(tmpEntry);
                        }
                        event.data.viewCollection.changeCollection(col);
                        FreeswitchConfig.Site.Modals.HideLoading();
                    });
                }
            }
            el.append(model.get('Name'));
            el.attr('relpath', model.get('RelativeFilePath'));
            this.trigger('render', this);
        }
    }),
    CollectionView = Backbone.View.extend({
        tagName: "div",
        className: "Org Reddragonit FreeSwitchConfig Site Models AudioFileEntry CollectionView",
        initialize: function() {
        },
        changeCollection: function(collection) {
            this.collection = collection;
            this.render();
        },
        render: function() {
            var el = this.$el;
            el.html('');
            var alt = false;
            for (var x = 0; x < this.collection.length; x++) {
                var vw = new Org.Reddragonit.FreeSwitchConfig.Site.Models.AudioFileEntry.View({
                    model: this.collection.at(x),
                    input: this.input,
                    viewCollection: this
                });
                if (alt) {
                    vw.$el.attr('class', vw.$el.attr('class') + ' Alt');
                }
                alt = !alt;
                el.append(vw.$el);

                vw.render();
            }
            this.trigger('render', this);
        }
    })
});