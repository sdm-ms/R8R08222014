function pageLoad() {
    var prevButton = $("#prev").autoHilite();
    var nextButton = $("#next").autoHilite();
    $(".slideshow").cycle({
    fx: 'fade',
        timeout: 2000,
        speed: 500,
        pause: 1,
        prev: prevButton, 
        next: nextButton
    }).removeClass("slideshowInitialDisplay");
    $("#pause").click(pauseSlideShow).autoHilite();
    $("#resume").click(resumeSlideShow).autoHilite();
}

function pauseSlideShow()
{
    $('.slideshow').cycle('pause');
    $('#pause').hide();
    $('#resume').show();
}

function resumeSlideShow() {
    $('.slideshow').cycle('resume');
    $('#resume').hide();
    $('#pause').show();
}

//function onAfter(curr, next, opts) {
//    var index = opts.currSlide;
//    $('#prev')[index == 0 ? 'hide' : 'show']();
//    $('#next')[index == opts.slideCount - 1 ? 'hide' : 'show']();
//}
