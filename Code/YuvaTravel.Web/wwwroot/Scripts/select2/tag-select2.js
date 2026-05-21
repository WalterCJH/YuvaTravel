$(function () {

    // 輸入標籤
    $(".js-select2-tag").select2({
        //data: empArray,
        minimumInputLength: 1,
        //multiple: true,
        placeholder: "輸入標籤",
        //theme: "classic",
        language: {
            inputTooShort: function () {
                return '請至少輸入一個字';
            },
            noResults: function () {
                return "#找不到標籤";
            }
        },
        tags: true,
        tokenSeparators: [',', ' '],
        //createTag: function (params) {
        //    // Don't offset to create a tag if there is no @@ symbol
        //    if (params.term.indexOf('@@') === -1) {
        //        // Return null to disable tag creation
        //        return null;
        //    }

        //    return {
        //        id: params.term,
        //        text: params.term,
        //        newTag: true // add additional parameters
        //    }
        //},
        //createTag: function (params) {
        //    var term = $.trim(params.term);

        //    if (term === '') {
        //        return null;
        //    }

        //    return {
        //        id: term,
        //        text: term,
        //        newTag: true // add additional parameters
        //    }
        //},
        ajax: {
            url: "/Admin/Tags/QuerySelectTags",
            dataType: "json",
            data: function (params) {
                return {
                    searchTerm: params.term
                }
            },
            processResults: function (data, params) {
                return {
                    results: data
                };
            }
        }
    });

    // 選取後移除現有資料，在最後面加上
    $(".js-select2-tag").on("select2:select", function (evt) {
        var data = evt.params.data;
        $("li.select2-selection__choice[title='" + data.text + "']").remove();
        $("option[value='" + data.id + "']", this).remove();
        var option = new Option(data.text, data.id, true, true);
        $(this).append(option).trigger('change');
    });

    // 取得原有標籤資料
    if ($(".TagIds").length > 0) {
        var dataArray = [];
        $(".TagIds").each(function () {
            dataArray.push($(this).text());
        });

        var tagSelect = $('.js-select2-tag');
        $.ajax({
            type: 'Post',
            url: '/Admin/Tags/QuerySelectTagsForTagId',
            data: JSON.stringify(dataArray),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        }).then(function (datas) {
            // create the option and append to Select2
            datas.forEach(function (data) {
                var option = new Option(data.text, data.id, true, true);
                tagSelect.append(option).trigger('change');
            });

            // manually trigger the `select2:select` event
            //tagSelect.trigger({
            //    type: 'select2:select',
            //    params: {
            //        data: data
            //    }
            //});
        });
    }

});