$(function () {

    $('#AddRow_StructuredDataCommonQuestion').on('click', function () {
        var datetime = Date.now();
        $.get('/Admin/Articles/AddStructuredDataCommonQuestion?T=' + datetime).done(function (html) {
            $(html).insertBefore('#StructuredDataCommonQuestion');
        });
    });

    $("body").on("click", ".js-delete-row", function () {
        if (!confirm("刪除無法復原，您確定要刪除嗎?")) {
            return false;
        }
        $(this).closest(".js-data-row").remove();
    });

    function EventRow(_this) {
        if (_this.val() == '20') {
            $(".js-event-row").show();
        }
        else {
            $(".js-event-row").hide();
        }
    }

    EventRow($("#ArticleType"));
    $("#ArticleType").change(function () {
        EventRow($(this));
    });

    $(".sortable").sortable({
        items: ".js-sortable-row",
        handle: ".js-sortable-handle",
        cancel: ".js-sortable-cancel",
        placeholder: "sortable-highlight"
    });

});