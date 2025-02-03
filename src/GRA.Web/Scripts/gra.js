window.onunload = function () {
    ResetSpinners();
};

function ResetSpinners(target) {
    if (target != null) {
        target.removeClass("disabled");
        target.children(".fa-spinner").addClass("d-none");
    } else {
        $(".btn-spinner, .btn-spinner-no-validate").removeClass("disabled");
        $(".btn-spinner, .btn-spinner-no-validate")
            .children(".fa-spinner")
            .addClass("d-none");
    }
}

function graGetLocalDate() {
    let localDate = new Date();
    localDate.setHours(0, 0, 0);
    localDate.setMinutes(localDate.getMinutes() - localDate.getTimezoneOffset());
    return localDate;
}

$(".btn-spinner").on("click", function (e) {
    if ($(this).hasClass("disabled")) {
        e.preventDefault();
        e.stopPropagation();
        e.stopImmediatePropagation();
    } else {
        var parentForm = $(this).closest("form");

        if (
            !$(this).hasClass("spinner-ignore-validation") &&
            parentForm.length > 0 &&
            !parentForm.valid()
        ) {
            e.preventDefault();
            e.stopPropagation();
            e.stopImmediatePropagation();
        } else {
            parentForm.find(".btn-spinner").addClass("disabled");
            $(this).children(".fa-spinner").removeClass("d-none");
        }
    }
});

$().ready(function () {
    if ($(".gra-carousel").length) {
        $(".gra-carousel").slick({
            slidesToShow: 4,
            slidesToScroll: 4,
            autoplay: false,
            dots: false,
            prevArrow:
                '<span class="far fa-2x fa-arrow-alt-circle-left gra-carousel-nav gra-carousel-prev"></span>',
            nextArrow:
                '<span class="far fa-2x fa-arrow-alt-circle-right gra-carousel-nav gra-carousel-next"></span>',
            responsive: [
                {
                    breakpoint: 993,
                    settings: {
                        slidesToShow: 3,
                        slidesToScroll: 3,
                    },
                },
                {
                    breakpoint: 768,
                    settings: {
                        slidesToShow: 2,
                        slidesToScroll: 2,
                    },
                },
                {
                    breakpoint: 576,
                    settings: {
                        slidesToShow: 1,
                        slidesToScroll: 1,
                    },
                },
            ],
        });
    }

    let graDatetimePickers = document.querySelectorAll(".gra-picker-datetime");
    for (let i = 0; i < graDatetimePickers.length; i++) {
        let currentValue = new Date(graDatetimePickers[i].dataset.currentValue);
        if (isNaN(currentValue)) {
            currentValue = undefined;
        }
        new tempusDominus.TempusDominus(graDatetimePickers[i],
            {
                allowInputToggle: true,
                defaultDate: currentValue,
                display: {
                    buttons: {
                        close: true,
                        today: true,
                    },
                    icons: {
                        type: 'icons',
                        time: 'fas fa-clock',
                        date: 'fas fa-calendar',
                        up: 'fas fa-arrow-up',
                        down: 'fas fa-arrow-down',
                        previous: 'fas fa-chevron-left',
                        next: 'fas fa-chevron-right',
                        today: 'fas fa-calendar-check',
                        clear: 'fas fa-trash',
                        close: 'fas fa-xmark'
                    },
                    sideBySide: true
                },
                useCurrent: false,
            });
    }
});
