$(".btn-spinner").on("click", function(e) {
    if ($(this).parents("form:first").valid())
    {
        e.preventDefault();
        if (!$(this).hasClass("disabled"))
        {
            $(this).addClass("disabled");
            $(this).children(".fa-spinner").removeClass("hidden");
            $(this).parents("form:first").submit();
        }
    }
});

$(".btn-spinner-no-validate").on("click", function(e) {
    e.preventDefault();
    if (!$(this).hasClass("disabled"))
    {
        $(this).addClass("disabled");
        $(this).children(".fa-spinner").removeClass("hidden");
        $(this).parents("form:first").submit();
    }
});