$(function () {
    $("#txtPostalCode").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Manage/GetPostalCodes',
                data: JSON.parse("{ \"prefix\": \"" + request.term + "\"}"),
                dataType: "json",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }))
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
            var respTxt = (i.item.value).substring(0, 5);
            $("#hiddenPostalCode").val(i.item.val);
            $("#txtPostalCode").val(respTxt);
        },
        minLength: 2
    });
});

$(function () {
    $("#txtCompanyPostalCode").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Manage/GetPostalCodes',
                data: JSON.parse("{ \"prefix\": \"" + request.term + "\"}"),
                dataType: "json",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }))
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
            var respTxt = (i.item.value).substring(0, 5);
            $("#hiddenCompanyPostalCode").val(i.item.val);
            $("#txtCompanyPostalCode").val(respTxt);
        },
        minLength: 2
    });
});