CreateNameSpace('Org.Reddragonit.FreeSwitchConfig.Site.Home');
Org.Reddragonit.FreeSwitchConfig.Site.Home = \$.extend(Org.Reddragonit.FreeSwitchConfig.Site.Home,{
    GeneratePage: function(container) {
        container = \$(container);
        var mainContainer = container;
        $components:{ comp | 
            $if(comp)$
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
});