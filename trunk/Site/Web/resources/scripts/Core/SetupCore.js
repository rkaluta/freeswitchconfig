CreateNameSpace('FreeswitchConfig.Core.Context');
CreateNameSpace('FreeswitchConfig.Core.Domain');
CreateNameSpace('FreeswitchConfig.Core.SipProfile');

FreeswitchConfig.Core.Context = $.extend(
FreeswitchConfig.Core.Context,
{
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: "tr",
        className: "FreeSwitchConfig Core Context  View",
        render: function() {
            $(this.el).html('<td class="' + this.className + ' Name">' + this.model.get('Name') + '</td>' +
            '<td class="' + this.className + ' Description">' + (this.model.get('Description')==null ? '' : this.model.get('Description')) + '</td>' +
            '<td class="' + this.className + ' Type">' + this.model.get('Type') + '</td>' +
            '<td class="' + this.className + ' SocketIP">' + this.model.get('SocketIP') + '</td>' +
            '<td class="' + this.className + ' SocketPort">' + this.model.get('SocketPort') + '</td>' +
            '<td class="' + this.className + ' buttons">' +
            '<span class="' + this.className + ' button pencil"></span>' +
            '<span class="' + this.className + ' button cancel"></span>' +
            '</td>');
            $(this.el).attr('name', this.model.id);
            this.trigger('render', this);
            return this;
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: "table",
        className: "FreeswitchConfig Core Context  CollectionView",
        initialize: function() {
            this.collection.on('reset', this.render, this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        attributes: { cellspacing: '0', cellpadding: '0' },
        render: function() {
            var el = this.$el;
            el.html('');
            var thead = $('<thead class="' + this.className + ' header"></thead>');
            el.append(thead);
            thead.append('<tr></tr>');
            thead = $(thead.children()[0]);
            thead.append('<th className="' + this.className + ' Name">Name</th>');
            thead.append('<th className="' + this.className + ' Description">Description</th>');
            thead.append('<th className="' + this.className + ' Type">Type</th>');
            thead.append('<th className="' + this.className + ' SocketIP">SocketIP</th>');
            thead.append('<th className="' + this.className + ' SocketPort">SocketPort</th>');
            thead.append('<th className="' + this.className + ' Buttons">Actions</th>');
            el.append('<tbody></tbody>');
            el = $(el.children()[0]);
            if (this.collection.length == 0) {
                this.trigger('render', this);
            } else {
                var alt = false;
                for (var x = 0; x < this.collection.length; x++) {
                    var vw = new FreeswitchConfig.Core.Context.View({ model: this.collection.at(x) });
                    if (alt) {
                        vw.$el.attr('class', vw.$el.attr('class') + ' Alt');
                    }
                    alt = !alt;
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
    })
});

FreeswitchConfig.Core.SipProfile = _.extend(FreeswitchConfig.Core.SipProfile, { 
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: "tr",
        className: "FreeSwitchConfig Core SipProfile  View",
        render: function() {
        $(this.el).html('<td class="' + this.className + ' Name">' + this.model.get('Name') + '</td>' +
            '<td class="' + this.className + ' Context">' + '<span class="Org Reddragonit FreeSwitchConfig DataCore DB Core Context Name">' + this.model.get('Context').get('Name') + '</span>' + '</td>' +
            '<td class="' + this.className + ' SIPPort">' + this.model.get('SIPPort') + '</td>' +
            '<td class="' + this.className + ' SIPInterface">' + '<span class="FreeSwitchConfig System sNetworkCard Name">' + this.model.get('SIPInterface').get('Name') + '</span></td>' +
            '<td class="' + this.className + ' RTPInterface">' + '<span class="FreeSwitchConfig System sNetworkCard Name">' + this.model.get('RTPInterface').get('Name') + '</span></td>' + 
            '<td class="' + this.className + ' buttons">' + '<span class="' + this.className + ' button pencil"></span>' + '<span class="' + this.className + ' button cancel"></span>' + '</td>');
            this.trigger('render', this);
            return this;
        }
    }),
    CollectionView: Backbone.View.extend({
        tagName: "table",
        className: "FreeswitchConfig Core SipProfile  CollectionView",
        initialize: function() {
            this.collection.on('reset', this.render, this);
            this.collection.on('add', this.render, this);
            this.collection.on('remove', this.render, this);
        },
        attributes:{cellspacing:'0',cellpadding:'0'},
        render: function() {
            var el = this.$el;
            el.html('');
            var thead = $('<thead class="' + this.className + ' header"></thead>');
            el.append(thead);
            thead.append('<tr></tr>');
            thead = $(thead.children()[0]);
            thead.append('<th className="' + this.className + ' Name">Name</th>');
            thead.append('<th className="' + this.className + ' Context">Context</th>');
            thead.append('<th className="' + this.className + ' SIPPort">SIPPort</th>');
            thead.append('<th className="' + this.className + ' SIPInterface">SIPInterface</th>');
            thead.append('<th className="' + this.className + ' RTPInterface">RTPInterface</th>');
            thead.append('<th className="' + this.className + ' Buttons">Actions</th>');
            el.append('<tbody></tbody>');
            el = $(el.children()[0]);
            if (this.collection.length == 0) {
                this.trigger('render', this);
            } else {
                var alt = false;
                for (var x = 0; x < this.collection.length; x++) {
                    var vw = new FreeswitchConfig.Core.SipProfile.View({ model: this.collection.at(x) });
                    if (alt) {
                        vw.$el.attr('class', vw.$el.attr('class') + ' Alt');
                    }
                    alt = !alt;
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
    })
});

FreeswitchConfig.Core.Domain = _.extend(FreeswitchConfig.Core.Domain, { 
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: "tr",
        className: "FreeSwitchConfig Core Domain  View",
        render: function() {
            $(this.el).html('<td class="' + this.className + ' Name">' + this.model.get('Name') + '</td>' +
            '<td class="' + this.className + ' VoicemailTimeout">' + this.model.get('VoicemailTimeout') + '</td>' +
            '<td class="' + this.className + ' InternalProfile">' + '<span class="FreeSwitchConfig Core SipProfile Name">' + this.model.get('InternalProfile').get('Name') + '</span>' + '</td>' +
            '<td class="' + this.className + ' ExternalProfile">' + '<span class="FreeSwitchConfig Core SipProfile Name">' + this.model.get('ExternalProfile').get('Name') + '</span>' + '</td>' +
            '<td class="' + this.className + ' buttons">' + '<span class="' + this.className + ' button pencil"></span>' + 
            '<span class="' + this.className + ' button cancel"></span>' + '</td>');
            this.trigger('render', this);
            return this;
        }
    }),
    CollectionView : Backbone.View.extend({
	    tagName : "table",
	    className : "FreeswitchConfig Core Domain  CollectionView",
	    initialize : function(){
		    this.collection.on('reset',this.render,this);
		    this.collection.on('add',this.render,this);
		    this.collection.on('remove',this.render,this);
	    },
	    attributes:{cellspacing:'0',cellpadding:'0'},
	    render : function(){
		    var el = this.$el;
		    el.html('');
		    var thead = $('<thead class="'+this.className+' header"></thead>');
		    el.append(thead);
		    thead.append('<tr></tr>');
		    thead = $(thead.children()[0]);
		    thead.append('<th className="'+this.className+' Name">Name</th>');
		    thead.append('<th className="'+this.className+' VoicemailTimeout">VoicemailTimeout</th>');
		    thead.append('<th className="'+this.className+' InternalProfile">InternalProfile</th>');
		    thead.append('<th className="'+this.className+' ExternalProfile">ExternalProfile</th>');
		    thead.append('<th className="' + this.className + ' Buttons">Actions</th>');
		    el.append('<tbody></tbody>');
		    el = $(el.children()[0]);
		    if(this.collection.length==0){
			    this.trigger('render',this);
		    }else{
			    var alt=false;
			    for(var x=0;x<this.collection.length;x++){
				    var vw = new FreeswitchConfig.Core.Domain.View({model:this.collection.at(x)});
				    if (alt){
					    vw.$el.attr('class',vw.$el.attr('class')+' Alt');
				    }
				    alt=!alt;
				    if(x+1==this.collection.length){
					    vw.on('render',function(){this.col.trigger('item_render',this.view);this.col.trigger('render',this.col);},{col:this,view:vw});
				    }else{
					    vw.on('render',function(){this.col.trigger('item_render',this.view);},{col:this,view:vw});
				    }
				    el.append(vw.$el);
				    vw.render();
			    }
		    }
	    }
    })
});