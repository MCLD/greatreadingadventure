(function ($) {

  

    function AvatarComponent() {
        this.index = 0;
        this.identifier = "";
    }

    var components = new Array();
    
    function update_avatar_layers() {
        $(".avatar-layer").each(function (index) {
            var identifier = $(this).data("component");
            var component = components[identifier];

            $(this).attr("src", component.images[component.index]);
        });
    }


    $(document).ready(function () {
        alert(ASP_components);

        for (var key in ASP_components) {
            var part = new AvatarComponent();
            part.index = 0;
            part.identifier = key;
            part.images = ASP_components[key];
            components[part.identifier] = part;
        }


        $(".avatar-layer-btn-right").click(function (event) {
            var $target = $(event.target);
            var component = components[$target.data("component")];

            if (component.index + 1 < component.images.length) {
                component.index += 1;
            }

            update_avatar_layers();
        });

        $(".avatar-layer-btn-left").click(function (event) {
            var $target = $(event.target);
            var component = components[$target.data("component")];

            if (component.index > 0) {
                component.index -= 1;
            }

            update_avatar_layers();
        });
    });




}) (jQuery);