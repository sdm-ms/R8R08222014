
function pageLoad() {
    resizePageVertically();
}

function getBottom(item) {
    return item.offset().top + item.height();
}

function getBottomOfGroup(items) {
    var bottom = 0;
    items.each(function () {
        bottom = Math.max(bottom, getBottom($(this)));
    });
    return bottom;
}

// If we restore this page, must delete similar line from viewtbl.js and uncomment here: var mainPartOfPage = null;
function resizePageVertically() {
    if (mainPartOfPage == null)
        mainPartOfPage = $("#mainPartOfPage");
    var candidates = $(".possibleBottom");
    if (candidates.length > 0) {
        var bottomOfContent = getBottomOfGroup(candidates);
        var currentMainPartOfPageHeight = mainPartOfPage.height();
        var bottomOfMainPartOfPage = getBottom(mainPartOfPage);
        var increaseNeeded = (bottomOfContent + 30) - bottomOfMainPartOfPage;
        mainPartOfPage.height(currentMainPartOfPageHeight + increaseNeeded);
    }
}