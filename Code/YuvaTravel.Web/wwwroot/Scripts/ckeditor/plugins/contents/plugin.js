CKEDITOR.plugins.add( 'contents', {
    requires: 'widget',

	icons: 'contents',

	init: function( editor ) {
        editor.addContentsCss( window.document.location.origin + '/Content/Site.css' );
		CKEDITOR.dialog.add( 'contents', this.path + 'dialogs/contents.js' );

        editor.widgets.add( 'contents', {
			button: '插入文章目錄',

			template:
                '<div class="article-menu-wrap"></div>',

            //allowedContent:
            //'div(!article-menu-wrap,float-left,float-right,align-center);' +
            //'p(!toc-title);',

            dialog: 'contents',

			upcast: function( element ) {
				return element.name == 'div'
					&& element.hasClass( 'article-menu-wrap' );
			},

			init: function() {

                editor.on('saveSnapshot', function(evt) {
                    buildToc(this.element)
                }.bind(this));

                this.on('focus', function(evt) {
                    buildToc(this.element)
                }.bind(this));

                buildToc(this.element);

                console.log(this.element.hasClass( 'float-right' ) );

                if ( this.element.hasClass( 'float-left' ) )
					this.setData( 'align', 'float-left' );
				if ( this.element.hasClass( 'float-right' ) )
					this.setData( 'align', 'float-right' );
                if ( this.element.hasClass( 'toc_root' ) )
                    this.setData( 'chkInsertOpt', true);
			},

			data: function() {

				this.element.removeClass( 'float-left' );
				this.element.removeClass( 'float-right' );
                this.element.removeClass( 'toc_root' );

				if ( this.data.align )
					this.element.addClass(this.data.align );

                if ( this.data.chkInsertOpt )
                    this.element.addClass('toc_root');
            },

		} );

        function buildToc(element){
            //set everything up
            //element.setHtml('<i class="bi bi-list-ul"></i> 文章目錄<i class="bi bi-chevron-expand"></i>');
            element.setHtml('<button class="btn article-menu-box-btn" type="button" data-bs-toggle="collapse" data-bs-target="#collapseExample" aria-expanded="false" aria-controls="collapseExample"> <i class="bi bi-list-ul"></i> 文章目錄<i class="bi bi-chevron-expand"></i> </button>');
            //element.setHtml('<p class="toc-title">文章目錄2</p>');

            var tmp = new CKEDITOR.dom.element('div').setAttribute('class', 'collapse article-menu-box').setAttribute('id', 'collapseExample');

            Container = new CKEDITOR.dom.element('ol').addClass('article-menu-box-list');
            //Container = new CKEDITOR.dom.element( 'ol' );
            Container.appendTo(tmp);
            //Container.appendTo(element);

            if (element.hasClass( 'toc_root' )){
                findRoot = '> h1,> h2,> h3,> h4,> h5,> h6,';
            }else{
                findRoot = 'h1,h2,h3,h4,h5,h6,';
            }

            var headings = editor.editable().find(findRoot),
                parentLevel = 1,
                length = headings.count();

            //get each heading
            for (var i = 0 ; i < length ; ++i) {

                var currentHeading = headings.getItem( i ),
                    text = currentHeading.getText( ),
                    newLevel = parseInt(currentHeading.getName().substr(1,1));
                var diff = (newLevel - parentLevel);

                //set the start level incase it is not h1
                if(i === 0){diff = 0; parentLevel = newLevel;}

                //we need a new ul if the new level has a higher number than its parents number

                if (diff > 0) {
                    var containerLiNode = Container.getLast();
                    var ulNode = new CKEDITOR.dom.element( 'ol' );
                    ulNode.appendTo(containerLiNode);
                    Container = ulNode;
                    parentLevel = newLevel;
                }

                //we need to get a previous ul if the new level has a lower number than its parents number
                if (diff < 0) {
                    while (0 !== diff++) {
                        parent = Container.getParent().getParent();
                        Container = (parent.getName() === 'ol' ? parent : Container);
                    }
                    parentLevel = newLevel;
                }

                //we can add the list item if there is no difference

                //if(text === ''){text = 'empty'}

                if (text == null || text.trim() === ''){
                   text = '&nbsp;'
                }

                var id = text.replace(/ /g, "+");
                currentHeading.setAttribute( 'id', id );

                var liNode = CKEDITOR.dom.element.createFromHtml( '<li class="js-content-click"><a href="#'+id+'">'+text+'</a></li>' );
                liNode.appendTo(Container);
            }

            element.appendHtml(tmp.getOuterHtml());
        }
    }
} );




