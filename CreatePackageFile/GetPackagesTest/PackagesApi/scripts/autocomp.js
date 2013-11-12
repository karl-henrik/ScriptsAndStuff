var g_Value = "";

$.postJSON = function (url, data, callback) {
    return jQuery.ajax({
        'type': 'POST',
        'url': url,
        'contentType': 'application/json',
        'data': data,
        'dataType': 'json',
        'success': callback,
        'error': function (d) {
            alert(d);
        }
    });

};


$(document).ready(function () {
    var obj = {};
    obj.PackagesArray = {};
   
    $("#makeList").click(function() {
        obj.PackagesArray = $('#packageList li').map(function(i, el) {
            var item = { package: $(el).text() };
            return item;
        });
        var list = {};
        list = { "packages": obj.PackagesArray.toArray() };
        $.postJSON("list/", JSON.stringify(list), function (d) {
                window.location = "/scriptFile/" + d;
        });
    });
    $("#add").click(function () {
        $("#packageList").append('<li>' + g_Value + '<button class="buttonStyle menuButton" type="button" onclick="$(this).parent().remove();"><img class="btnImg" src="Content/Remove.png"/></button></li>');
    });
});

$(function () {
    $.get("test/", function (data) {
        var arr = $.parseJSON(data);
        var str = new Array(0);
        for (var i = 0; i < arr.length; i++) {
            str.push(arr[i].Name);
        }

        $("#tags").autocomplete({
            source: str,
            select: function (event, ui) {
                g_Value = ui.item.label;

            }
        });
    });
});