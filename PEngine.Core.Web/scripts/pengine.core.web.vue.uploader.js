import pengineHelpers from "./pengine.core.web.helpers";

export default {
  create() {
    let component = new Vue({
      el: "#pengine-uploader",
      mounted() {
        this.$events.listen("show", eventData => {
          this.show();
        });
      },
      methods: {
        show() {
          document.body.style.overflow = 'hidden';
          window.scrollTo(0, 0);
          this.visible = true;
          this.get();
        },
        hide() {
          this.visible = false;
          document.body.style.overflow = 'initial';
        },
        get(path, skipBreadcrumb) {
          this.current = {};
          path = path || '';
          while (path.indexOf('.') === 0 || path.indexOf('/') === 0) {
            path = path.substr(1);
          }
          let getUrl = `/api/resource/folder/${path}`;
          return this.$http.get(getUrl).then(response => {
            if (response.body.files) {
              for (var fileIndex in response.body.files) {
                response.body.files[fileIndex].selected = !!this.selectedFilePaths[response.body.files[fileIndex].relativePath];
              }
            }
            if (response.body.folders) {
              for (var folderIndex in response.body.folders) {
                response.body.folders[folderIndex].selected = !!this.selectedFolderPaths[response.body.folders[folderIndex].relativePath];
              }
            }
            if (!skipBreadcrumb) {
              this.breadcrumbs.push({ title: response.body.name, path: response.body.relativePath });
            }
            this.current = response.body;
            this.uploadPath = `./api/resource/file/${path}`;
          }, response => {
            this.errors = [ { type: "Error", text: "An HTTP error prevented the folder from loading." } ];
          });
        },
        navigate(breadcrumb) {
          var newBreadcrumbs = [];
          for (var breadcrumbIndex in this.breadcrumbs) {
            newBreadcrumbs.push(this.breadcrumbs[breadcrumbIndex]);
            if (this.breadcrumbs[breadcrumbIndex] === breadcrumb) {
              break;
            }
          }
          this.breadcrumbs = newBreadcrumbs;
          this.get(breadcrumb.path, true);
        },
        select(entity, tracker) {
          if (tracker[entity.relativePath]) {
            delete tracker[entity.relativePath];
            entity.selected = false;
          }
          else {
            tracker[entity.relativePath] = true;
            entity.selected = true;
          }
        }
      },
      data: {
        breadcrumbs: [],
        current: {},
        uploadPath: '',
        selectedFolderPaths: {},
        selectedFilePaths: {},
        visible: false,
        errors: []
      }
    });
    return component;
  }
};