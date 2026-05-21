$(function () {
    $(".js-delete-confirm").click(function () {
        if (!confirm("刪除後無法復原，您確定要刪除嗎!?")) {
            return false;
        }
    });

});