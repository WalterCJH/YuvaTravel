$(function () {
    $("input[type='checkbox'].js-section-checkAll").change(function () {
        var section = $(this).closest(".js-section-div");
        var checked = $(this).prop("checked");
        section.find("input[type='checkbox']").each(function () {
            $(this).prop("checked", checked);
        });
    });
});