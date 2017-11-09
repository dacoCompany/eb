$('#txtPostalCode').on('input', function () {
    var postalCode = $('#txtPostalCode').val();
    $('#hiddenPostalCode').val(postalCode);
});

$('#txtCompanyPostalCode').on('input', function () {
    var postalCode = $('#txtCompanyPostalCode').val();
    $('#hiddenCompanyPostalCode').val(postalCode);
});

