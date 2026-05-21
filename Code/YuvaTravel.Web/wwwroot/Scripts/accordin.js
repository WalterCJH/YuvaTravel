$(function () {

    // accordin list
    var accordinList = $("ul.accordin-list li");
    function toggleAccordion() {
        accordinList.removeClass("active");
        $(this).addClass("active");
    }
    accordinList.on("click", toggleAccordion);
});