/**
 * @license Copyright (c) 2003-2021, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here. For example:
    // config.language = 'fr';

    config.height = 600;
    config.uiColor = '#E0F2F4';
    //config.filebrowserBrowseUrl = '/Admin/Images/FileBrowse';
    config.filebrowserImageBrowseUrl = '/Admin/Images/CKEditorImageFileManager';
    //config.filebrowserVideoBrowseUrl = '/Admin/Images/CKEditorVideoFileManager';
    config.filebrowserUploadMethod = 'form';
    //config.filebrowserUploadUrl = '/Admin/Images/Upload';
    config.extraPlugins = 'contents,html5video';

    // video plugin需開啟此設定切換至原始碼模式才不會把內容清除
    config.allowedContent = true;
    //config.disallowedContent = 'img{width,height};img[width,height]';
    //config.extraAllowedContent = 'b;i;iframe';
    //config.extraPlugins = 'uploadimage';
    //config.uploadUrl = '/Admin/Images/Uploading';
    //config.imageUploadUrl = '/Admin/Images/Uploading';
    //config.coreStyles_bold = { element: 'b', overrides: 'strong'  }; // 輸入b，不會取代成strong
    //config.coreStyles_italic = { element: 'i', overrides: 'em' }; // 輸入i，不會取代成em
    //config.extraAllowedContent = "b";
};
CKEDITOR.dtd.$removeEmpty['i'] = false;
