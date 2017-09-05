function setDefaultValues(btn,oldText) {
    $("#IcoValidation").text("Sorry! We have not found this company identifier");
    $("#txtCompanyName").val("");
    $("#txtCompanyPostalCode").val("");
    $("#AccountTypeDropDown").val('1');
    $("#companyDiv").hide();
    btn.text(oldText);
}

$(function () {
    $("#validateBtn").click(function () {

        var btn = $("#validateBtn");
        var oldText = btn.text();
        btn.text('');
        btn.html('<span id=\"spn2\" class=\"glyphicon glyphicon-refresh glyphicon-refresh-animate\"></span>');
        var cn = $("#txtCompanyNumber").val().replace(" ", "");

        if (cn == null || cn.length < 8) {
            return;
        }

        $.ajax({
            url: "http://localhost:50198/api/SKRegister/GetCompanyDetailsById3",
            data: JSON.parse("{ \"id\": \"" + cn + "\"}"),
            dataType: "json",
            type: "GET",
            contentType: "application/json; charset=utf-8",
            success: function (data, textStatus, jqXHR) {
                if (jqXHR.status == "200" && data != null) {
                    $("#IcoValidation").text("");
                    $("#txtCompanyName").val(data.Name);
                    $("#txtCompanyPostalCode").val(data.PostCode != null ? data.PostCode : "");
                    if (data.CompanyType === "Živnostník") {
                        $("#AccountTypeDropDown").val('2');
                    } else {
                        $("#AccountTypeDropDown").val('3');
                    }
                    $("#companyDiv").fadeIn("slow");
                    btn.text(oldText);
                }
                else {
                    setDefaultValues(btn,oldText);
                }
            },
            error: function (response) {
                setDefaultValues(btn,oldText);
            },
            failure: function (response) {
                alert(response.responseText);
                btn.text(oldText);
            },
        });
    });
});



$(function () {
    $("#WithoutIco").click(function () {
        $("#companyDiv").show();
        $("#AccountTypeDropDown").val('1');
    });
    $("#WithIco").click(function () {
        $("#companyDiv").hide();
    });
});