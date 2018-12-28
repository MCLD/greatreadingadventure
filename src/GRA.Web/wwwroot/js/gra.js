window.onunload = function () {
    $(".btn-spinner, .btn-spinner-no-validate").removeClass("disabled");
    $(".btn-spinner, .btn-spinner-no-validate").children(".fa-spinner").addClass("hidden");
}

$(".btn-spinner").on("click", function (e) {
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

$().ready(function () {
    if ($(".gra-carousel").length) {
        $(".gra-carousel").slick({
            slidesToShow: 4,
            slidesToScroll: 4,
            autoplay: false,
            dots: false,
            prevArrow: "<span class=\"far fa-2x fa-arrow-alt-circle-left gra-carousel-nav gra-carousel-prev\"></span>",
            nextArrow: "<span class=\"far fa-2x fa-arrow-alt-circle-right gra-carousel-nav gra-carousel-next\"></span>",
            responsive: [
                {
                    breakpoint: 993,
                    settings: {
                        slidesToShow: 3,
                        slidesToScroll: 3
                    }
                },
                {
                    breakpoint: 768,
                    settings: {
                        slidesToShow: 2,
                        slidesToScroll: 2
                    }
                },
                {
                    breakpoint: 576,
                    settings: {
                        slidesToShow: 1,
                        slidesToScroll: 1
                    }
                }
            ]
        });
    }
});
