/*
AN7effects v1.5.5 - John Einselen (http://Iaian7.com)
MooTools v1.2.4 required
*/

function tableOfContents(h1, toc, top) {
    toc = $(toc);
    eln = '';
    els = toc.get('html');
    var toTop = '<a href="javascript:scrollToThis(\'WindowTop\')">' + top + '</a>';
    $$(h1).each(function(el) {
        eln = el.get('html');
        el.set('id', 'toc_' + eln);
        els += '<a href="javascript:scrollToThis(\'toc_' + eln + '\')">' + eln + '</a><br/>';
        if (top != '') { el.set('html', eln + ' ' + toTop) };
    });
    toc.set('html', els);
}

function scrollToThis(el) {
    var scrollFx = new Fx.Scroll(window);
    if (el == 'WindowTop') {
        scrollFx.toTop();
    } else {
        scrollFx.toElement($(el));
    }
}

function linkFade(lfDiv) {
    $$(lfDiv).each(function(div) {
        var mouseFxs = new Fx.Tween(div, { duration: 240, wait: false });
        div.set('opacity', 0.85); //Set the starting opacity
        div.addEvents({
            'mouseover': function() {
                mouseFxs.start('opacity', [0.85, 1]);
            },
            'mouseout': function() {
                mouseFxs.start('opacity', [1, 0.85]);
            }
        });
    });
}

function contentFade(cfFade, cfHide) {
    $$(cfFade).each(function(div) {
        var hide = div.getElement(cfHide);
        var mouseFx = new Fx.Tween(hide, { duration: 180, wait: false });
        hide.set('opacity', 0);
        div.addEvents({
            'mouseenter': function() {
                mouseFx.start('opacity', [0, 1]);
            },
            'mouseleave': function() {
                mouseFx.start('opacity', [1, 0]);
            }
        });
    });
}

function contentSlide(csSlide, csToggle, csHide) {
    $$(csSlide).each(function(div) {
        var link = div.getElement(csToggle);
        var hide = div.getElement(csHide);
        var fx = new Fx.Slide(hide, { duration: 240, mode: 'vertical' }).hide();
        link.addEvent('click', function() {
            fx.toggle();
        });
    });
}

function selfLink(slEl) {
    var a = $$(slEl);
    for (var i = 0; i < a.length; i++) {
        if (a[i].href.split("#")[0] == window.location.href.split("#")[0]) {
            (a[i]).setStyles({ color: '#000' });
        }
    }
}

window.addEvent('domready', function() {
    //	tableOfContents('h1','toc', '^');	// section header element, table of contents element ID, "to top" link (empty turns link off)
    linkFade('.an7_thumb img, .an7_thumb_left img, .an7_thumb_right img, .linkFade'); // (hover) fades links with [selector]
    contentFade('.fade', '.hide'); 			// (hover) shows / hides content [.hide] within div [.fade]
    contentSlide('.slide', '.toggle', '.hide'); // (click) shows / hides content [.hide] within [.slide] via toggle [.toggle]
    selfLink('#nav a'); 						// (onload) changes the style of links to the current page
    //	new Accordion($$('.menuToggle'), $$('.menuStretch'), {opacity: false});
});