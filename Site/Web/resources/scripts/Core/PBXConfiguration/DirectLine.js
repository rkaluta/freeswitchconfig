﻿CreateNameSpace('FreeswitchConfig.PBX.DirectLine');

FreeswitchConfig.PBX.DirectLine = $.extend(FreeswitchConfig.PBX.DirectLine, {
    View: Backbone.View.extend({
        initialize: function() {
            this.model.on('change', this.render, this);
        },
        tagName: "tr",
        className: "FreeswitchConfig PBX DirectLine View",
        render: function() {
            this.$el.html('');
            this.$el.append('<td class="' + this.className + ' DialedNumber">' + this.model.get('DialedNumber') + '</td>');
            this.$el.append('<td class="' + this.className + ' DialedContext">' + this.model.attributes['DialedContext'].id + '</td>');
            this.$el.append('<td class="' + this.className + ' TransferTo">' + this.model.get('TransferTo').get('Number') + '@' + this.model.get('TransferTo').get('Context').get('Name') + '</td>');
            this.$el.append($('<td class="' + this.className + ' buttons"><img class="button edit pencil"/><img class="button delete cancel"/></td>'));
            this.trigger('render', this);
            return this;
        },
        events: {
            'click .button.delete.cancel': 'deleteModel',
            'click .button.edit.pencil': 'editModel'
        },
        deleteModel: function() {
            FreeswitchConfig.Site.Modals.ShowUpdating();
            this.model.destroy({
                success: function() { FreeswitchConfig.Site.Modals.HideUpdating(); },
                error: function() { alert('An error occured attempting to delete the Direct Line.'); FreeswitchConfig.Site.Modals.HideUpdating(); }
            });
        },
        editModel: function() {
            FreeswitchConfig.Site.Modals.ShowLoading();
            FreeswitchConfig.PBX.DirectLine.GenerateForm(null, this.model);
            FreeswitchConfig.Site.Modals.HideLoading();
        }
    }),
    CollectionView : Backbone.View.extend({
	    tagName : "table",
	    className : "FreeswitchConfig PBX DirectLine  CollectionView",
	    initialize : function(){
		    this.collection.on('reset',this.render,this);
		    this.collection.on('add',this.render,this);
		    this.collection.on('remove',this.render,this);
	    },
	    render : function(){
		    var el = this.$el;
		    el.html('');
		    var thead = $('<thead class="'+this.className+' header"></thead>');
		    el.append(thead);
		    thead.append('<tr></tr>');
		    thead = $(thead.children()[0]);
		    thead.append('<th className="'+this.className+' DialedNumber">Dialed Number</th>');
		    thead.append('<th className="'+this.className+' DialedContext">Dialed Context</th>');
		    thead.append('<th className="' + this.className + ' TransferTo">Transfer To</th>');
		    thead.append('<th className="' + this.className + ' Buttons">Actions</th>');
		    el.append('<tbody></tbody>');
		    el = $(el.children()[0]);
		    if(this.collection.length==0){
			    this.trigger('render',this);
		    }else{
			    var alt=false;
			    for(var x=0;x<this.collection.length;x++){
				    var vw = new FreeswitchConfig.PBX.DirectLine.View({model:this.collection.at(x)});
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
    }),
    GenerateForm: function(collection, model) {
        var isCreate = model == undefined;
        model = (model == undefined ? new FreeswitchConfig.PBX.DirectLine.Model() : model);
        var frm = FreeswitchConfig.Site.Form.GenerateForm(
            null,
            [
                new FreeswitchConfig.Site.Form.FormInput('dialedNumber', 'text', null, true, 'Dialed Number', null, model.get('DialedNumber')),
                new FreeswitchConfig.Site.Form.FormInput('dialedContext','select',[new FreeswitchConfig.Site.Form.SelectValue(FreeswitchConfig.CurrentDomain.get('InternalProfile').get('Context').id,FreeswitchConfig.CurrentDomain.get('InternalProfile').get('Context').get('Name')),
                    new FreeswitchConfig.Site.Form.SelectValue(FreeswitchConfig.CurrentDomain.get('ExternalProfile').get('Context').id,FreeswitchConfig.CurrentDomain.get('ExternalProfile').get('Context').get('Name'))],true,'Dialed Context',null,(model.attributes['DialedContext']==null ? null : model.attributes['DialedContext'].id)),
                new FreeswitchConfig.Site.Form.FormInput('transferTo', 'select', FreeswitchConfig.Core.ExtensionNumber.SelectList(), true, 'Transfer To', null, (model.get('TransferTo') == null ? null : model.get('TransferTo').id))
            ]
        );
        FreeswitchConfig.Site.Modals.ShowFormPanel(
            (isCreate ? 'Create Direct Line' : 'Edit Direct Line'),
            frm,
            [
                CreateButton(
                    'accept',
                    'Okay',
                    function(button, pars) {
                        FreeswitchConfig.Site.Modals.ShowUpdating();
                        var canSubmit = true;
                        var number = $(pars.frm.find('input[name="dialedNumber"]')[0]);
                        if (!FreeswitchConfig.Site.Validation.ValidateRequiredField(number)) {
                            canSubmit = false;
                        } else if (!FreeswitchConfig.Site.Validation.ValidateDialableNumberField(number)) {
                            canSubmit = false;
                        }
                        if (canSubmit) {
                            pars.model.set({
                                DialedNumber: number.val(),
                                TransferTo: new FreeswitchConfig.Core.ExtensionNumber.Model({ id: $(pars.frm.find('select[name="transferTo"]>option:selected')[0]).val() }),
                                DialedContext: new FreeswitchConfig.Core.Context.Model({id:$(pars.find('select[name="dialedContext"]>option:selected')[0]).val()})
                            });
                            if (pars.model.syncSave()) {
                                pars.model.attributes['TransferTo'].fetch();
                                if (pars.isCreate) {
                                    pars.collection.add(pars.model);
                                }
                                FreeswitchConfig.Site.Modals.HideUpdating();
                            } else {
                                FreeswitchConfig.Site.Modals.HideUpdating();
                                alert('The Direct line failed to update, please check all values');
                            }
                        } else {
                            FreeswitchConfig.Site.Modals.HideUpdating();
                        }
                    },
                    { frm: frm, model: model, collection: collection, isCreate: isCreate }
                ),
                CreateButton(
                    'cancel',
                    'Cancel',
                    function(button) {
                        FreeswitchConfig.Site.Modals.HideFormPanel();
                    }
                )
            ]
        );
    },
    GeneratePage: function(container) {
        container = $(container);
        FreeswitchConfig.Site.Modals.ShowLoading();
        var col = new FreeswitchConfig.PBX.DirectLine.Collection();
        FreeswitchConfig.Site.setChangedDomain({ collection: col }, function(event) { FreeswitchConfig.Site.Modals.ShowLoading(); event.data.collection.fetch(); });
        var vw = new FreeswitchConfig.PBX.DirectLine.CollectionView({ collection: col, attributes: { cellspacing: 0, cellpadding: 0} });
        container.append(CreateButton(
            'phone_add',
            'Add Direct Line',
            function(button, pars) {
                FreeswitchConfig.Site.Modals.ShowLoading();
                FreeswitchConfig.PBX.DirectLine.GenerateForm(pars.collection);
                FreeswitchConfig.Site.Modals.HideLoading();
            },
            { collection: col }
        ));
        container.append(vw.$el);
        vw.on('render', function() { FreeswitchConfig.Site.Modals.HideLoading(); });
        col.fetch();
    }
});