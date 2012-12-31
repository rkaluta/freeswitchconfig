CreateNameSpace('Org.Reddragonit.FreeSwitchConfig.Site.Home');

Org.Reddragonit.FreeSwitchConfig.Site.Home = {
    GeneratePage: function(container) {
        container = \$(container);
        var mainContainer = container;
        $components:{ comp | 
            $if(comp)$
            $if(comp.JSUrls)$
            $comp.JSUrls:{ js |
                loadjscssfile('$js$', 'js');
            }$
            $endif$
            $if(comp.CSSUrls)$
            $comp.CSSUrls:{ css |
                loadjscssfile('$css$', 'css');
            }$
            $endif$
            container = \$('<div class="HomePageComponentContainer"></div>');
            mainContainer.append(container);
            container.append('<div class="shadow"></div>');
            \$(container.children()[0]).append('<div class="HeaderBar">$comp.Title$</div>');
            \$(container.children()[0]).append('<div class="Content"></div>');
            container = \$(container.find('div.Content')[0]);
            $comp.ComponentRenderCode$
            $endif$
        }$
        mainContainer.append('<div class="clear"></div>');
    }
}