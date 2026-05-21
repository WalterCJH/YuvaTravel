CKEDITOR.dialog.add('html5video', function (editor) {
    var lang = editor.lang.html5video;

    function generateId() {
        var now = new Date();
        return 'video_' + now.getFullYear() + now.getMonth() + now.getDate() + now.getHours() + now.getMinutes() + now.getSeconds();
    }

    function commitValue(videoNode, extraStyles) {
        var value = this.getValue();

        if (!value && this.id == 'id')
            value = generateId();

        videoNode.setAttribute(this.id, value);

        if (!value)
            return;
        switch (this.id) {
            case 'posterSrc':
                extraStyles.backgroundImage = 'url(' + value + ')';
                break;
            case 'width':
                extraStyles.width = value + 'px';
                break;
            case 'height':
                extraStyles.height = value + 'px';
                break;
        }
    }

    function loadValue(videoNode) {
        if (videoNode)
            this.setValue(videoNode.getAttribute(this.id));
        else {
            if (this.id == 'id')
                this.setValue(generateId());
        }
    }

    function commitSrc(videoNode, extraStyles, videos) {
        var match = this.id.match(/(\w+)(\d)/),
          id = match[1],
          number = parseInt(match[2], 10);

        var video = videos[number] || (videos[number] = {});
        video[id] = this.getValue();
        if (number === 0)
            video["type"] = "video/mp4";
        else if (number === 1)
            video["type"] = "video/webm";
    }

    function loadSrc(videoNode, videos) {
        var match = this.id.match(/(\w+)(\d)/),
          id = match[1],
          number = parseInt(match[2], 10);

        var video = videos[number];
        if (!video)
            return;
        this.setValue(video[id]);
    }

    // To automatically get the dimensions of the poster image
    var onImgLoadEvent = function () {
        // Image is ready.
        var preview = this.previewImage;
        preview.removeListener('load', onImgLoadEvent);
        preview.removeListener('error', onImgLoadErrorEvent);
        preview.removeListener('abort', onImgLoadErrorEvent);

        this.setValueOf('videoInfo', 'width', preview.$.width);
        this.setValueOf('videoInfo', 'height', preview.$.height);
    };

    var onImgLoadErrorEvent = function () {
        // Error. Image is not loaded.
        var preview = this.previewImage;
        preview.removeListener('load', onImgLoadEvent);
        preview.removeListener('error', onImgLoadErrorEvent);
        preview.removeListener('abort', onImgLoadErrorEvent);
    };

    return {
        title: lang.dialogTitle,
        minWidth: 400,
        minHeight: 260,
        onShow: function () {
            // Clear previously saved elements.
            this.fakeImage = this.videoNode = null;
            // To get dimensions of poster image
            this.previewImage = editor.document.createElement('img');

            var fakeImage = this.getSelectedElement();
            if (fakeImage && fakeImage.data('cke-real-element-type') && fakeImage.data('cke-real-element-type') == 'video') {
                this.fakeImage = fakeImage;
                var videoNode = editor.restoreRealElement(fakeImage),
                    videos = [],
                    sourceList = videoNode.getElementsByTag('source', '');

                if (sourceList.count() == 0)
                    sourceList = videoNode.getElementsByTag('source', 'cke');

                for (var i = 0, length = sourceList.count() ; i < length ; i++) {
                    var item = sourceList.getItem(i);
                    videos.push({ src: item.getAttribute('src'), type: item.getAttribute('type') });
                }

                this.videoNode = videoNode;
                this.setupContent(videoNode, videos);
            } else {
                this.setupContent(null, []);
            }
        },
        onOk: function () {
            // If there's no selected element create one. Otherwise, reuse it
            var videoNode = null;
            if (!this.fakeImage) {
                videoNode = CKEDITOR.dom.element.createFromHtml('<cke:video></cke:video>', editor.document);
                videoNode.setAttributes(
                  {
                      controls: 'controls'
                  });
            }
            else {
                videoNode = this.videoNode;
            }

            var extraStyles = {}, videos = [];
            this.commitContent(videoNode, extraStyles, videos);

            var innerHtml = '', links = '',
              link = lang.linkTemplate || '',
              fallbackTemplate = lang.fallbackTemplate || '';
            for (var i = 0; i < videos.length; i++) {
                var video = videos[i];
                if (!video || !video.src)
                    continue;
                innerHtml += '<cke:source src="' + video.src + '" type="' + video.type + '" />';
                links += link.replace('%src%', video.src).replace('%type%', video.type);
            }
            videoNode.setHtml(innerHtml + fallbackTemplate.replace('%links%', links));

            // Refresh the fake image.
            var newFakeImage = editor.createFakeElement(videoNode, 'cke_video', 'video', false);
            newFakeImage.setStyles(extraStyles);
            if (this.fakeImage) {
                newFakeImage.replace(this.fakeImage);
                editor.getSelection().selectElement(newFakeImage);
            }
            else {
                // Insert it in a div
                var div = new CKEDITOR.dom.element('DIV', editor.document);
                editor.insertElement(div);
                div.append(newFakeImage);
            }
        },
        onHide: function () {
            if (this.previewImage) {
                this.previewImage.removeListener('load', onImgLoadEvent);
                this.previewImage.removeListener('error', onImgLoadErrorEvent);
                this.previewImage.removeListener('abort', onImgLoadErrorEvent);
                this.previewImage.remove();
                this.previewImage = null;		// Dialog is closed.
            }
        },
        contents: [
           {
               id: 'videoInfo',
               elements: [
                   {
                       type: 'text',
                       id: 'id',
                       label: "ID",
                       commit: commitValue,
                       setup: loadValue
                   },
                   {
                       type: 'hbox',
                       widths: ['50%', '50%'],
                       children: [
                         {
                             type: 'text',
                             id: 'width',
                             label: editor.lang.common.width,
                             'default': 400,
                             validate: CKEDITOR.dialog.validate.notEmpty(lang.widthRequired),
                             commit: commitValue,
                             setup: loadValue
                         },
                         {
                             type: 'text',
                             id: 'height',
                             label: editor.lang.common.height,
                             'default': 300,
                             validate: CKEDITOR.dialog.validate.notEmpty(lang.heightRequired),
                             commit: commitValue,
                             setup: loadValue
                         }
                       ]
                   },
                   {
                       type: 'hbox',
                       widths: ['80%', '20%'],
                       children: [
                          {
                              type: 'text',
                              id: 'posterSrc',
                              label: '海報圖檔',
                              commit: commitValue,
                              setup: loadValue,
                              onChange: function () {
                                  var dialog = this.getDialog(),
                                    newUrl = this.getValue();

                                  //Update preview image
                                  if (newUrl.length > 0)	//Prevent from load before onShow
                                  {
                                      dialog = this.getDialog();
                                      var preview = dialog.previewImage;

                                      preview.on('load', onImgLoadEvent, dialog);
                                      preview.on('error', onImgLoadErrorEvent, dialog);
                                      preview.on('abort', onImgLoadErrorEvent, dialog);
                                      preview.setAttribute('src', newUrl);
                                  }
                              }
                          },
                          {
                              type: 'button',
                              id: 'posterBrowse',
                              hidden: 'true',
                              style: 'display:inline-block;margin-top:14px;',
                              filebrowser:
                              {
                                  action: 'Browse',
                                  target: 'videoInfo:posterSrc',
                                  url: editor.config.filebrowserImageBrowseUrl || editor.config.filebrowserBrowseUrl
                              },
                              label: editor.lang.common.browseServer
                          }
                       ]
                   },
                   {
                       type: 'hbox',
                       widths: ['80%', '20%'],
                       children: [
                          {
                              type: 'text',
                              id: 'src0',
                              label: '影片-MP4檔案',
                              commit: commitSrc,
                              setup: loadSrc
                          },
                          {
                              type: 'button',
                              id: 'videoMp4Browse',
                              hidden: 'true',
                              style: 'display:inline-block;margin-top:14px;',
                              filebrowser:
                              {
                                  action: 'Browse',
                                  target: 'videoInfo:src0',
                                  url: editor.config.filebrowserVideoBrowseUrl || editor.config.filebrowserBrowseUrl,
                                  params: {
                                      videoType: "0",
                                  }
                              },
                              label: editor.lang.common.browseServer
                          }
                       ]
                   },
                   {
                       type: 'hbox',
                       widths: ['80%', '20%'],
                       children: [
                          {
                              type: 'text',
                              id: 'src1',
                              label: '影片-WebM檔案',
                              commit: commitSrc,
                              setup: loadSrc
                          },
                          {
                              type: 'button',
                              id: 'videoWebMBrowse',
                              hidden: 'true',
                              style: 'display:inline-block;margin-top:14px;',
                              filebrowser:
                              {
                                  action: 'Browse',
                                  target: 'videoInfo:src1',
                                  url: editor.config.filebrowserVideoBrowseUrl || editor.config.filebrowserBrowseUrl,
                                  params: {
                                      videoType: "1",
                                  }
                              },
                              label: editor.lang.common.browseServer
                          }
                       ]
                   }
               ]
           }
        ]
    };
});