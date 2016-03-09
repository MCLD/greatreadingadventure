(function ($) {

    var components = new Array();


    function ComponentState() {
        this.componentID = 0;
        this.index = 0;
        this.partIDs = [];

        this.$stateField = null;
        this.$layerField = null;

        this.update = function () {
            var partID = this.partIDs[this.index];

            this.$stateField.val(partID);

            var imageUrl = "/images/AvatarParts/" + partID;
            imageUrl += ".png";

            this.$layerField.attr("src", imageUrl);
        }
    }
  

    $(document).ready(function () {
        var i = 0;

        for (var fieldSelector in ASP_avatar_fields) {

            var $field = $(fieldSelector);


            var $imgField = $("#" + $field.data("img"));

            var componentKey = $field.data("component");

            var component = new ComponentState();
            component.index = ASP_avatar_state[i];
            component.componentID = parseInt(componentKey);
            component.$stateField = $field;


        }


        for (var key in ASP_avatar_components) {

            component.partIDs = ASP_avatar_components[key];

            component.update();

            components[key] = component;

            i += 1;
        }


        $(".avatar-layer-btn-right").click(function (event) {
            var $target = $(event.target);
            var component = components[$target.data("component")];

            if (component.index + 1 < component.partIDs.length) {
                component.index += 1;
            }

            component.update();
        });

        $(".avatar-layer-btn-left").click(function (event) {
            var $target = $(event.target);
            var component = components[$target.data("component")];

            if (component.index > 0) {
                component.index -= 1;
            }

            component.update();
        });
    });




}) (jQuery);