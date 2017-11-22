$('.chosen-select').chosen({ width: '100%' });
jQuery('#newMemberBtn').on('click', function (event) {
    jQuery('#newMember').toggle(400);
    jQuery('#newRole').hide(400);
});
jQuery('#newRoleBtn').on('click', function (event) {
    jQuery('#newRole').toggle(400);
    jQuery('#newMember').hide(400);
});
jQuery('#newCategoryBtn').on('click', function (event) {
    jQuery('#categoryDiv').toggle(400);
});
jQuery('#newLanguageBtn').on('click', function (event) {
    jQuery('#languageDiv').toggle(400);
});

$('.tags_block-symbol').on('click', function () {
    var id = $(this).attr('id');
    var name = $("#name_" + id).text();
    if (window.globalVar > 1) {
        $.ajax({
            url: "/Manage/DeleteCategory",
            type: "POST",
            data: JSON.stringify({ category: name }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data === "OK") {
                    var container = $("#" + id).parent().attr('id');
                    $("#" + container).hide(250);
                    window.globalVar--;
                }
                else {
                    $('#categoryValidation').text(data);

                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }
        });
    }
    else {
        $('#categoryValidation').text("Minimalne jedna kategoria!");
        $('#categoryValidation').show(250);
        setTimeout(
            function () {
                $('#categoryValidation').hide(250);
            }, 3000);
    }
});

$('.language_tags_block-symbol').on('click', function () {
    var code = $(this).attr('id');
    $.ajax({
        url: "/Manage/DeleteLanguage",
        type: "POST",
        data: JSON.stringify({ code: code }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data === "OK") {
                var container = $("#" + code).parent().attr('id');
                $("#" + container).hide(250);
            }
            else {
                $('#languageValidation').text(data);
                $('#languageValidation').show(250);
                setTimeout(
                    function () {
                        $('#languageValidation').hide(250);
                    }, 3000);

            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
        }
    });
});

function DeleteMember(email) {
    $.ajax({
        type: 'POST',
        url: "/Manage/DeleteMember",
        data: JSON.stringify({ email: email }),
        contentType: 'application/json',
        success: function (data) {
            localStorage.setItem("isEditMember", true);
            if (data === "OK") {
                setTimeout(function () {
                    window.location.reload();
                }, 100)
            }
        }
    });
}

$('[id^=memberRole]').change(function () {
    var divId = this.id;
    var user = $("#" + divId).parent().attr('id')
    var selectedRole = $("#" + divId).find('option:selected').text();
    $.ajax({
        type: 'POST',
        url: "/Manage/ChangeMemberRole",
        data: JSON.stringify({ user: user, role: selectedRole }),
        contentType: 'application/json',
        success: function (data) {
            localStorage.setItem("isEditMember", true);
            if (data === "OK") {
                setTimeout(function () {
                    window.location.reload();
                }, 100)
            }
        }
    });
});

function AddNewMember() {
    var email = $("#newMemberText").val();
    var role = $("#newMemberDrop").val();
    $.ajax({
        type: 'GET',
        url: "/Manage/AddMemberToCompany",
        contentType: "application/json; charset=utf-8",
        data: { email: email, selectedRole: role },
        success: function (data) {
            localStorage.setItem("isEditMember", true);
            if (data === "OK") {
                setTimeout(function () {
                    window.location.reload();
                }, 100)
            }
            else {
                $('#memberValidation').text(data);
            }
        }
    });
}
function AddNewRole() {
    var role = $("#newRoleName").val();
    var permissions = GetPermissions();
    $.ajax({
        type: 'POST',
        url: "/Manage/AddCustomRoleToCompany",
        data: JSON.stringify({ roleName: role, permissions: permissions }),
        contentType: 'application/json',
        success: function (data) {
            localStorage.setItem("isEditMember", true);
            if (data === "OK") {
                setTimeout(function () {
                    window.location.reload();
                }, 100)
            }
        }
    });
}

function GetPermissions() {
    return [
        $("#AddMember").is(":checked") ? "AddMember" : "",
        $("#RemoveMember").is(":checked") ? "RemoveMember" : "",
        $("#AddGallery").is(":checked") ? "AddGallery" : "",
        $("#RemoveGallery").is(":checked") ? "RemoveGallery" : "",
        $("#AddAttachments").is(":checked") ? "AddAttachments" : "",
        $("#RemoveAttachments").is(":checked") ? "RemoveAttachments" : "",
        $("#Comment").is(":checked") ? "Comment" : "",
        $("#CreateDemand").is(":checked") ? "CreateDemand" : "",
        $("#EditDemand").is(":checked") ? "EditDemand" : "",
        $("#DeleteDemand").is(":checked") ? "DeleteDemand" : "",
        $("#ChangeSettings").is(":checked") ? "ChangeSettings" : "",
    ];
}