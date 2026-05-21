$(function () {
    $(".js-btn-angle").click(function () {
        var data_angle = $(this).attr("data-angle");
        if (data_angle === "down") {
            $(this).attr("data-angle", "up");
            $(this).find("i").removeClass("fa-angle-down");
            $(this).find("i").addClass("fa-angle-up");
        }
        else {
            $(this).attr("data-angle", "down");
            $(this).find("i").removeClass("fa-angle-up");
            $(this).find("i").addClass("fa-angle-down");
        }
    });
    //var host = "www.long-huei.com";
    //if (window.location.hostname != host && window.location.hostname != "localhost") {
    //    location.href = "https://" + host;
    //}
})