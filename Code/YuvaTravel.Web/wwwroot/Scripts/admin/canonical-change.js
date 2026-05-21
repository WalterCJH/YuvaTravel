$(function () {

    function CanonicalChange() {
        if ($("#Canonical").val() === "") {
            $(".js-IsCanonicalAbsoluteUri").show();
        }
        else {
            $(".js-IsCanonicalAbsoluteUri").hide();
            $("#IsCanonicalAbsoluteUri").prop("checked", false);
        }
    }
    CanonicalChange();
    $("#Canonical").keyup(function () {
        CanonicalChange();
    });


    function IsCanonicalAbsoluteUriChange() {
        if ($("#IsCanonicalAbsoluteUri").prop("checked") === true) {
            $("#Canonical").val("");
            $("#Canonical").attr("disabled", true);
        }
        else {
            $("#Canonical").removeAttr("disabled");
        }
    }
    $("#IsCanonicalAbsoluteUri").change(function () {
        IsCanonicalAbsoluteUriChange();
    });
});