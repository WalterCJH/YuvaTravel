$(function () {
    var recommendData = { articleCategoryId: $("#ArticleCategoryId").val(), articleId: $("#ArticleId").val() };
    GenerateHtml("", "js-recommend", "append", "/LazyLoad/_RecommendedArticles", recommendData);
});
