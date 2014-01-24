
function setupAutoComplete() {
    $('[id*=SearchBox]').autocomplete('AutoComplete.ashx', {
        dataType: "json",
        formatItem: function(data, i, max, value, term) {
            return value;
        },
        parse: function(data) {
            var array = new Array();
            for (var i = 0; i < data.length; i++) {
                var tempValue = data[i].ItemPath; // Data to show in drop down list
                var tempResult = data[i].Item; // Data put in text box
                array[array.length] = { data: data[i], value: tempValue, result: tempResult };
            }
            return array;
        },
        selectFirst: false
    }).result(function(event, item) {
        location.href = item.Page;
    });
}

/* for Firefox bug fix */
function doNothing() {
}