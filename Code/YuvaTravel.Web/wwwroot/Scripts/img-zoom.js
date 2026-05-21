$(function () {
    $('.img-zoom-in').on('click', function () {
        var src = $(this).attr('src');
        $('.imgPreview img').attr('src', src);
        $('.imgPreview').show()
    });
    $('.imgPreview').on('click', function () {
        $('.imgPreview').hide()
    });
});

function setImgZoomIn(block) {
    $('.img-zoom-in', block).on('click', function () {
        var src = $(this).attr('src');
        $('.imgPreview img').attr('src', src);
        $('.imgPreview').show()
    });
};