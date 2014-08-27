

jQuery.fn.customHilite = function () {
    return this.each(function () {
        $(this).bind('mousedown', customHilite).bind('mouseup', undoCustomHilite).bind('mouseout', undoCustomHilite);
    });
}

jQuery.fn.customDisable = function () {
    return this.each(function () {
        $(this).unbind().replaceAttr("src", /-pushed./, "-disabled\.").replaceAttr("src", /-normal./, "-disabled\.");
    });
}
jQuery.fn.customEnable = function () {
    return this.each(function () {
        $(this).customHilite().replaceAttr("src", /-pushed./, "-normal\.").replaceAttr("src", /-disabled./, "-normal\.");
    });
}
jQuery.fn.autoHilite = function () {
    return this.each(function () {
        $(this).bind('mousedown', hiliteImage).bind('mouseup', unhiliteImage).bind('mouseout', unhiliteImage);
    });
}

function hiliteImage(event) {
    $(event.target).css('opacity', 0.50);
}

function unhiliteImage(event) {
    $(event.target).css('opacity', 1.0);
}

function customHilite(event) {
    $(event.target).replaceAttr("src", /-normal./, "-pushed\.");
}

function undoCustomHilite(event) {
    $(event.target).replaceAttr("src", /-pushed./, "-normal\.");
}

jQuery.fn.replaceAttr = function (theAttr, oldVal, newVal) {
    return this.each(function () {
        var currentVal = $(this).attr(theAttr);
        if (currentVal != null && currentVal != "") {
            currentVal = currentVal.replace(oldVal, newVal);
            $(this).attr(theAttr, currentVal);
        };
    });
}

var badPngSupport = (/MSIE ((5\.5)|6)/.test(navigator.userAgent) && navigator.platform == "Win32");
jQuery.fn.replacePngWithGif = function () {
    return this.each(function () {
        if (badPngSupport) {
            $(this).replaceAttr("src", /\.png/, "\.gif");
        }
    });
}