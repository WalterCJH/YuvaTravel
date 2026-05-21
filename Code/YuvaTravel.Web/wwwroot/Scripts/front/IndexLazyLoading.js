$(function () {

    //var introduceLoading = false;
    //var featureArticleLoading = false;
    //var feedbackLoading = false;
    //var aboutTopLoading = false;
    //var newArticleLoading = false;
    //var hotArticleLoading = false;

    //setTimeout(function () {
    //    GenerateHtml("", "js-splide", "append", "/LazyLoad/_Banners", null);
    //}, 400);
    //setTimeout(function () {
    //    GenerateHtml("", "js-feature", "append", "/LazyLoad/_FeaturedArticles", null);
    //}, 700);

    //window.onscroll = function (ev) {
    //    if ((window.innerHeight + window.scrollY + 200) >= document.body.offsetHeight) {

    //        if (!introduceLoading) {
    //            GenerateHtml("", "js-introduce", "append", "/LazyLoad/_Introduces", null);
    //            introduceLoading = true;

    //            setTimeout(function () {
    //                // accordin list
    //                var accordinList = $("ul.accordin-list li");
    //                function toggleAccordion() {
    //                    accordinList.removeClass("active");
    //                    $(this).addClass("active");
    //                }
    //                accordinList.on("click", toggleAccordion);
    //            }, 500);
    //            return;
    //        }

    //        if (introduceLoading && !feedbackLoading) {
    //            GenerateHtml("", "js-feedback", "append", "/LazyLoad/_FeedBack", null);
    //            feedbackLoading = true;
    //            return;
    //        }

    //        if (feedbackLoading && !aboutTopLoading) {
    //            GenerateHtml("", "js-about-top", "append", "/LazyLoad/_AboutTop", null);
    //            aboutTopLoading = true;
    //            return;
    //        }

    //        if (aboutTopLoading) {
    //            if (!newArticleLoading) {
    //                GenerateHtml("", "js-article-block", "prepend", "/LazyLoad/_NewArticles", null);
    //                newArticleLoading = true;
    //            }
    //            if (!hotArticleLoading) {
    //                GenerateHtml("", "js-article-block", "append", "/LazyLoad/_HotArticles", null);
    //                hotArticleLoading = true;
    //            }
    //        }

    //        if (newArticleLoading && hotArticleLoading) {
    //            $("#footer").show();
    //        }
    //    }
    //};
});
