(function ($) {

    var components = new Array();


    function ComponentState() {
        this.componentID = 0;
        this.index = 0;
        this.partIDs = [];

        this.$stateField = null;
        this.$imgField = null;

        this.update = function () {
            var partID = this.partIDs[this.index];

            this.$stateField.val(partID);

            var imageUrl = "/images/AvatarParts/" + partID;
            imageUrl += ".png";

            this.$imgField.attr("src", imageUrl);
        }
    }
  

    $(document).ready(function () {
        var i = 0;

        for (var key in ASP_avatar_fields) {

            var component = new ComponentState();
            component.componentID = parseInt(key);

            component.$stateField = $("#" + ASP_avatar_fields[key]);
            component.$imgField = $("#componentImg" + key);

            component.partIDs = ASP_avatar_components[key];


            var selectedPartID = -1;
            
            if (component.$stateField.val().length != 0) {
                selectedPartID = parseInt(component.$stateField.val());
            }

            var index = component.partIDs.indexOf(selectedPartID);

            if (index != -1) {
                component.index = index;
            } else {
                component.index = 0;
            }


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