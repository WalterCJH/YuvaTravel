/*
 * @file Video plugin for CKEditor
 * Copyright (C) 2016 Wenpin Liao
 * 
 * == BEGIN LICENSE ==
 *
 * Licensed under the terms of any of the following licenses at your
 * choice:
 *
 *  - GNU General Public License Version 2 or later (the "GPL")
 *    http://www.gnu.org/licenses/gpl.html
 *
 *  - GNU Lesser General Public License Version 2.1 or later (the "LGPL")
 *    http://www.gnu.org/licenses/lgpl.html
 *
 *  - Mozilla Public License Version 1.1 or later (the "MPL")
 *    http://www.mozilla.org/MPL/MPL-1.1.html
 *
 * == END LICENSE ==
 * 
 */
(function () {
    CKEDITOR.plugins.add('html5video', {
        lang: ['en', 'es'],
        getPlaceholderCss: function () {
            return 'img.cke_video' +
                '{' +
                  'background-image: url(' + CKEDITOR.getUrl(this.path + 'images/placeholder.png') + ');' +
                  'background-position: center center;' +
                  'background-repeat: no-repeat;' +
                  'background-color:gray;' +
                  'border: 1px solid #a9a9a9;' +
                  'width: 80px;' +
                  'height: 80px;' +
                '}';
        },
        onLoad: function () {
            // v4
            if (CKEDITOR.addCss)
                CKEDITOR.addCss(this.getPlaceholderCss());

        },
        init: function (editor) {
            var lang = editor.lang.html5video;

            // Check for CKEditor 3.5
            if (typeof editor.element.data == 'undefined') {
                alert('The "video" plugin requires CKEditor 3.5 or newer');
                return;
            }

            CKEDITOR.dialog.add('html5video', this.path + 'dialogs/html5video.js');

            editor.addCommand('html5video', new CKEDITOR.dialogCommand('html5video'));

            editor.ui.addButton('html5video', {
                label: lang.toolbar,
                command: 'html5video',
                icon: this.path + 'images/icon.png'
            });

            // v3
            if (editor.addCss)
                editor.addCss(this.getPlaceholderCss());

            if (editor.addMenuItems) {
                editor.addMenuItems(
                  {
                      html5video:
                      {
                          label: lang.properties,
                          command: 'html5video',
                          group: 'flash'
                      }
                  });
            }

            editor.on('doubleclick', function (evt) {
                var element = evt.data.element;

                if (element.is('img') && element.data('cke-real-element-type') == 'video')
                    evt.data.dialog = 'html5video';
            });

            if (editor.contextMenu) {
                editor.contextMenu.addListener(function (element, selection) {
                    if (element && element.is('img') && !element.isReadOnly()
                        && element.data('cke-real-element-type') == 'video')
                        return { html5video: CKEDITOR.TRISTATE_OFF };
                });
            }

            // Add special handling for these items
            CKEDITOR.dtd.$empty['cke:source'] = 1;
            CKEDITOR.dtd.$empty['source'] = 1;
        },
        afterInit: function (editor) {
            var dataProcessor = editor.dataProcessor,
              htmlFilter = dataProcessor && dataProcessor.htmlFilter,
              dataFilter = dataProcessor && dataProcessor.dataFilter;

            // dataFilter : conversion from html input to internal data
            dataFilter.addRules(
              {
                  elements: {
                      $: function (realElement) {
                          if (realElement.name == 'video') {
                              realElement.name = 'cke:video';
                              for (var i = 0; i < realElement.children.length; i++) {
                                  if (realElement.children[i].name == 'source')
                                      realElement.children[i].name = 'cke:source'
                              }

                              var fakeElement = editor.createFakeParserElement(realElement, 'cke_video', 'video', false),
                                fakeStyle = fakeElement.attributes.style || '';

                              var width = realElement.attributes.width,
                                height = realElement.attributes.height,
                                poster = realElement.attributes.poster;

                              if (typeof width != 'undefined')
                                  fakeStyle = fakeElement.attributes.style = fakeStyle + 'width:' + CKEDITOR.tools.cssLength(width) + ';';

                              if (typeof height != 'undefined')
                                  fakeStyle = fakeElement.attributes.style = fakeStyle + 'height:' + CKEDITOR.tools.cssLength(height) + ';';

                              if (poster)
                                  fakeStyle = fakeElement.attributes.style = fakeStyle + 'background-image:url(' + poster + ');';

                              return fakeElement;
                          }
                      }
                  }

              }
            );
        }
    });

    var en = {
        toolbar: 'Video',
        dialogTitle: 'Video properties',
        widthRequired: 'Width field cannot be empty',
        heightRequired: 'Height field cannot be empty',
        properties: 'Edit video'
    };

    var es = {
        toolbar: 'Video',
        dialogTitle: 'Propiedades de video',
        widthRequired: 'La anchura no se puede dejar en blanco',
        heightRequired: 'La altura no se puede dejar en blanco',
        properties: 'Edit video'
    };

    // v3
    if (CKEDITOR.skins) {
        en = { html5video: en };
        es = { html5video: es };
    }

    // Translations
    CKEDITOR.plugins.setLang('html5video', 'en', en);

    CKEDITOR.plugins.setLang('html5video', 'es', es);
})();