// file upload
$(document).on("change", ":file", function () {
    var input = $(this),
        numFiles = input.get(0).files ? input.get(0).files.length : 1,
        label = input.val().replace(/\\/g, "/").replace(/.*\//, "");
    input.trigger("fileselect", [numFiles, label]);
});

$(":file").on("fileselect", function (event, numFiles, label) {
    var input = $(this).parents(".input-group").find(":text"),
        log = numFiles > 1 ? numFiles = numFiles + " files selected" : label;

    var button = $(this).parent();

    if (input.length) {
        input.val(log);
        button.removeClass("btn-default");
        button.addClass("btn-success");
    }
    else {
        input.val("");
        button.addClass("btn-default");
        button.removeClass("btn-success");
    }
});

// bootstrap tooltip
$('[data-toggle="tooltip"]').tooltip();