$(function () {
    $(".js-disabled").each(function () {
        $("input[type='text']", $(this)).prop('disabled', true)
        $("input[type='number']", $(this)).prop('disabled', true)
        $("input[type='email']", $(this)).prop('disabled', true)
        $("input[type='checkbox']", $(this)).prop('disabled', true)
        $("input[type='radio']", $(this)).prop('disabled', true)
        $("select", $(this)).prop('disabled', true)
        $("textarea", $(this)).prop('disabled', true)
        $("p", $(this)).prop('disabled', true)

        //CKEditor Readonly
        CKEDITOR.on("instanceReady", function (ev) {
            var editor = ev.editor;
            if ($("#" + editor.element.$.id).attr("readonly") == "readonly") {
                editor.setReadOnly(true);
            }
        });
    });

    $(".js-readonly").each(function () {
        $("input[type='text']", $(this)).prop('readonly', 'readonly')
        $("input[type='number']", $(this)).prop('readonly', 'readonly')
        $("input[type='email']", $(this)).prop('readonly', 'readonly')
        $("input[type='checkbox']", $(this)).prop('readonly', 'readonly')
        $("input[type='radio']", $(this)).prop('readonly', 'readonly')
        $("select", $(this)).prop('readonly', 'readonly')
        $("textarea", $(this)).prop('readonly', 'readonly')
        $("p", $(this)).prop('readonly', 'readonly')
    });

    $("input[type='checkbox'][readonly]").click(function () {
        return false;
    });
    $("input[type='radio'][readonly]").click(function () {
        return false;
    });

    $(".js-search-clear").click(function () {
        var row = $(this).closest(".row");
        row.find("input[type='text']").val("");
        row.find("input[type='date']").val("");
        row.find("input[type='checkbox']").prop("checked", false);
        row.find("select", row).val("");
        //$(".form-clear").addClass("d-none");

        //$(".DateCondition").each(function () {
        //    $(this).val("");
        //    DateConditionChange($(this));
        //    //$(this).find("option[text='(請選擇)']").attr("selected", true);
        //});
    });

    function DateConditionChange(dateCondition) {
        var div = dateCondition.closest("div");
        var DateFrom = $(".DateFrom", div);
        var Wave = $(".Wave", div);
        var DateTo = $(".DateTo", div);
        var Date = $(".js-Date", div);
        var DateConditionText = $("option:selected", dateCondition).text();  //       dateCondition.find(":selected").text();

        if (DateConditionText === "=" || DateConditionText === ">=" || DateConditionText === "<=") {
            DateFrom.show();
            Wave.hide();
            DateTo.hide();
        }
        else if (DateConditionText === "介於") {
            DateFrom.show();
            Wave.show();
            DateTo.show();
        }
        else if (DateConditionText === "Active" || DateConditionText === "Inactive") {
            Date.show();
        }
        else {
            DateFrom.hide();
            Wave.hide();
            DateTo.hide();
            Date.hide();
        }
    };
});