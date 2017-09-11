function setDefaultValues(btn, oldText)
{
    $("#IcoValidation").text("Sorry! We have not found this company identifier");
    $("#txtCompanyName").val("");
    $("#txtDic").val("");
    $("#txtCompanyPostalCode").val("");
    $("#AccountTypeDropDown").val('1');
    $("#companyDiv").hide();
    btn.text(oldText);
}

function disableFilledTextBoxes(data)
{
    if (data != null)
    {
        $("#WithoutIco").attr("disabled", "disabled");
    }
    if (data.Name != null)
    {
        $("#txtCompanyName").attr("disabled", "disabled");
    }
    if (data.Dic != null) {
        $("#txtDic").attr("disabled", "disabled");
    }
    if (data.CompanyType != null) {
        $("#AccountTypeDropDown").attr("disabled", "disabled");
    }
    if (data.PostCode != null) {
        $("#txtCompanyPostalCode").attr("disabled", "disabled");
    }
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
                    $("#txtDic").val(data.Dic);
                    $("#txtCompanyPostalCode").val(data.PostCode != null ? data.PostCode : "");
                    if (data.CompanyType === "SE") {
                        $("#AccountTypeDropDown").val('2');
                    } else {
                        $("#AccountTypeDropDown").val('3');
                    }
                    $("#companyDiv").fadeIn("slow");
                    btn.text(oldText);
                    disableFilledTextBoxes(data);
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
        $("#AccountTypeDropDown").attr("disabled", "disabled");
    });
    $("#WithIco").click(function () {
        $("#companyDiv").hide();
        $("#AccountTypeDropDown").removeAttr("disabled");
    });
});