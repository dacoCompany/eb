$(function () {
    var allData;
    var selectedCat;
    $("#selectedCategoryInput").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Manage/GetCategories',
                data: JSON.parse("{ \"prefix\": \"" + request.term + "\"}"),
                dataType: "json",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    allData = data;
                    response($.map(data, function (item) {
                        return item.Text;
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
        select: function (event, ui) {
            selectedCat = ui.item.value;
        },
        close: function () {
            $.each(allData, function (e, val) {
                if (selectedCat == val.Text) {
                    CreateCategoryTag(val.Value, val.Text);
                    allData = null;
                }
            });
        },
        minLength: 1,
    })
});

function CreateCategoryTag(value, text) {
    var tag = $('<div id="div_'+value+'" class="tags_blocks"><div id="name_'+value+'" class="tags_block-text">'+text+'</div><div id="'+value+'" class="tags_block-symbol">&#10006;</div></div>');
    var categories = [];
    $("#categoryArea .tags_block-symbol").each(function () {
        categories.push($(this).attr("id"));
    })
    if (categories.indexOf(value) == -1) {
        $('#categoryArea').append(tag);
    } else {
        $("#div_" + value).effect("highlight", { color: '#ff552b' }, 1000);
    }
    $("#selectedCategoryInput").val("");
    MapCategoryToHidden();
};

function MapCategoryToHidden() {
    var categories = [];
    $("#categoryArea .tags_block-symbol").each(function () {
        if ($(this).is(":visible")) {
            categories.push($(this).attr("id"));
        }
    })
    $("#hiddenSelectedCategoryInput").val(categories);
};

$("#categoryArea").on('click', ".tags_block-symbol", function () {
    var id = $(this).attr('id');
    var container = $("#" + id).parent().attr('id');
    $("#" + container).remove();
    MapCategoryToHidden();
});


$(function () {
    var allData;
    var selectedLanguage;
    $("#selectedLanguageInput").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Manage/GetLanguages',
                data: JSON.parse("{ \"prefix\": \"" + request.term + "\"}"),
                dataType: "json",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    allData = data;
                    response($.map(data, function (item) {
                        return item.Text;
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
        select: function (event, ui) {
            selectedLanguage = ui.item.value;
        },
        close: function () {
            $.each(allData, function (e, val) {
                if (selectedLanguage == val.Text) {
                    CreateLanguageTag(val.Value, val.Text);
                    allData = null;
                }
            });
        },
        minLength: 1,
    })
});

function CreateLanguageTag(value, text) {
    var tag = $('<div id="div_new_'+value+'" class="tags_blocks"><div id="name_new_'+value+'" class="tags_block-text">'+text+'</div><div id="'+value+'" class="tags_block-symbol">&#10006;</div></div>');
    var languages = [];
    var existingLanguages = [];
    $("#languageArea .tags_block-symbol").each(function() {
        languages.push($(this).attr("id"));
    });
    $("#existingLanguages .language_tags_block-symbol").each(function() {
        existingLanguages.push($(this).attr("id"));
    });
    if (languages.indexOf(value) == -1 && existingLanguages.indexOf(value) == -1) {
        $('#languageArea').append(tag);
    } else {
        $("#div_new_" + value).effect("highlight", { color: '#ff552b' }, 1000);
        $("#div_" + value).effect("highlight", { color: '#ff552b' }, 1000);
    }
    $("#selectedLanguageInput").val("");
    MapLanguageToHidden();
};

function MapLanguageToHidden() {
    var languages = [];
    $("#languageArea .tags_block-symbol").each(function () {
        if ($(this).is(":visible")) {
            languages.push($(this).attr("id"));
        }
    })
    $("#hiddenSelectedLanguageInput").val(languages);
};

$("#languageArea").on('click', ".tags_block-symbol", function () {
    var id = $(this).attr('id');
    var container = $("#" + id).parent().attr('id');
    $("#" + container).remove();
    MapLanguageToHidden();
});