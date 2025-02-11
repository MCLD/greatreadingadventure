const graPickerIcons = {
    type: "icons",
    time: "fas fa-clock",
    date: "fas fa-calendar",
    up: "fas fa-arrow-up",
    down: "fas fa-arrow-down",
    previous: "fas fa-chevron-left",
    next: "fas fa-chevron-right",
    today: "fas fa-calendar-check",
    clear: "fas fa-trash",
    close: "fas fa-xmark",
};

/* button spinners */
function ResetSpinners(target) {
    if (target != null) {
        target.removeClass("disabled");
        target.children(".fa-spinner").addClass("d-none");
    } else {
        $(".btn-spinner, .btn-spinner-no-validate").removeClass("disabled");
        $(".btn-spinner, .btn-spinner-no-validate").children(".fa-spinner").addClass("d-none");
    }
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

/* date and time */

function graGetLocalDate() {
    let localDate = new Date();
    localDate.setHours(0, 0, 0);
    return localDate;
}
function graPickerSetDate(datePicker, date) {
    datePicker.dates.setValue(datePicker.dates.parseInput(date), datePicker.dates.lastPickedIndex);
}

function graInitalizePickerDatetime(element) {
    let currentValue = new Date(element.dataset.currentValue);
    if (isNaN(currentValue)) {
        currentValue = undefined;
    }
    return new tempusDominus.TempusDominus(element, {
        allowInputToggle: true,
        defaultDate: currentValue,
        display: {
            buttons: {
                close: true,
                today: true,
            },
            components: {
                calendar: true,
                clock: true,
                date: true,
                decades: true,
                hours: true,
                minutes: true,
                month: true,
                seconds: false,
                year: true,
            },
            icons: graPickerIcons,
            sideBySide: true,
        },
        localization: {
            format: "MM/dd/yyyy h:mm T",
        },
        useCurrent: false,
    });
}

function graInitalizePickerDate(element) {
    let currentValue = new Date(element.dataset.currentValue);
    if (isNaN(currentValue)) {
        currentValue = undefined;
    }
    return new tempusDominus.TempusDominus(element, {
        allowInputToggle: true,
        defaultDate: currentValue,
        display: {
            buttons: {
                close: true,
                today: true,
            },
            components: {
                calendar: true,
                clock: false,
                date: true,
                decades: true,
                hours: false,
                minutes: false,
                month: true,
                seconds: false,
                year: true,
            },
            icons: graPickerIcons,
        },
        localization: {
            format: "L",
        },
        useCurrent: false,
    });
}
function graInitalizePickerTime(element) {
    let currentValue = new Date(element.dataset.currentValue);
    if (isNaN(currentValue)) {
        currentValue = undefined;
    }
    return new tempusDominus.TempusDominus(element, {
        allowInputToggle: true,
        defaultDate: currentValue,
        display: {
            buttons: {
                close: true,
            },
            components: {
                calendar: false,
                clock: true,
                date: false,
                decades: false,
                hours: true,
                minutes: true,
                month: false,
                seconds: false,
                year: false,
            },
            icons: graPickerIcons,
            viewMode: "clock",
        },
        localization: {
            format: "LT",
        },
        useCurrent: false,
    });
}

/* event binding */

window.onunload = function () {
    ResetSpinners();
};

$().ready(function () {
    if ($(".gra-carousel").length) {
        $(".gra-carousel").slick({
            slidesToShow: 4,
            slidesToScroll: 4,
            autoplay: false,
            dots: false,
            prevArrow:
                "<span class=\"far fa-2x fa-arrow-alt-circle-left gra-carousel-nav gra-carousel-prev\"></span>",
            nextArrow:
               "<span class=\"far fa-2x fa-arrow-alt-circle-right gra-carousel-nav gra-carousel-next\"></span>",
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
        graInitalizePickerDatetime(graDatetimePickers[i]);
    }

    let graDatePickers = document.querySelectorAll(".gra-picker-date");
    for (let i = 0; i < graDatePickers.length; i++) {
        graInitalizePickerDate(graDatePickers[i]);
    }

    let graTimePickers = document.querySelectorAll(".gra-picker-time");
    for (let i = 0; i < graTimePickers.length; i++) {
        graInitalizePickerTime(graTimePickers[i]);
    }
});
