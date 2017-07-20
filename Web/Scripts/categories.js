$(document).ready(function () {
    $("#Category").change(function () {
        $("#SubCategory").empty();
        $.ajax({
            type: 'POST',
            url: '@Url.Action("GetSubCategories", "Manage")',

            dataType: 'json',

            data: { id: $("#Category").val() },

            success: function (states) {

                $.each(states, function (i, state) {
                    $("#SubCategory").append('<option value="' + state.Value + '">' +
                        state.Text + '</option>');
                });
            },
            error: function (ex) {
                alert('Failed to retrieve states.' + ex);
            }
        });
        return false;
    })
});
$("#Category").change(function () {
    $("#SubCategory").attr("disabled", this.value == "0" || this.value == "");
});
$("#SubCategory").change(function () {
    document.getElementById('CatID').value = $("#SubCategory").val();
});