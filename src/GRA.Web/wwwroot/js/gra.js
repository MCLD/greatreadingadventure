$(".btn-spinner").on("click", function(e) {
    if ($(this).parents("form:first").valid())
    {
        if ($(this).hasClass("disabled"))
        {
            e.preventDefault();
        }
        else
        {
            $(this).addClass("disabled");
            $(this).children(".fa-spinner").removeClass("hidden");
        }
    }
});

$(".btn-spinner-no-validate").on("click", function(e) {
    if ($(this).hasClass("disabled"))
    {
        e.preventDefault();
    }
    else
    {
        $(this).addClass("disabled");
        $(this).children(".fa-spinner").removeClass("hidden");
    }
});