function GenerateHtml(id, _class, pastetype, url, params) {
    $.ajax({
        type: 'POST',
        url: url,
        data: params,
        dataType: "html"
    })
        .done(function (result) {
            if (result) {
                var element;
                if (_class != null) {
                    element = $("." + _class);
                }
                else {
                    element = $("#" + id);
                }

                if (pastetype === "append") {
                    element.append(result);
                }
                else {
                    element.prepend(result);
                }

            }
        })
        .fail(function (xhr, ajaxOptions, thrownError) {
            console.log("Error in " + id + ":", thrownError);
        })
        .always(function () {
            // $("#footer").css("display", "none"); // hide loading info
        });
}