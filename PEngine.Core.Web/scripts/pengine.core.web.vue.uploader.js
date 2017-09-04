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
          this.errors = [];
          this.current = {};
          path = path || '';
          while (path.indexOf('.') === 0 || path.indexOf('/') === 0) {
            path = path.substr(1);
          }
          let getUrl = `/api/resource/folder/${path}`;
          return this.$http.get(getUrl).then(response => {
            this.loadFolder(response.body, skipBreadcrumb);
            this.uploadPath = `./api/resource/file/${path}`;
          }, response => {
            this.pushError('An HTTP error prevented the folder from loading.');
          });
        },
        loadFolder(folderData, skipBreadcrumb) {
          if (folderData.files) {
            for (var fileIndex in folderData.files) {
              var fileSelectionIndex = this.selectedFilePaths.indexOf(folderData.files[fileIndex].relativePath);
              folderData.files[fileIndex].selected = (fileSelectionIndex >= 0);
            }
          }
          if (folderData.folders) {
            for (var folderIndex in folderData.folders) {
              var folderSelectionIndex = this.selectedFolderPaths.indexOf(folderData.folders[folderIndex].relativePath);
              folderData.folders[folderIndex].selected = (folderSelectionIndex >= 0);
            }
          }
          var fileSelectionIndex = this.selectedFilePaths.indexOf(folderData.relativePath);
          var folderSelectionIndex = this.selectedFolderPaths.indexOf(folderData.relativePath);
          folderData.selected = fileSelectionIndex >= 0 || folderSelectionIndex >= 0;
          if (!skipBreadcrumb) {
            this.breadcrumbs.push({ title: folderData.name, path: folderData.relativePath });
          }
          this.current = folderData;
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
          var trackerIndex = tracker.indexOf(entity.relativePath);
          if (trackerIndex >= 0) {
            tracker.splice(trackerIndex, 1);
            entity.selected = false;
          }
          else {
            tracker.push(entity.relativePath);
            entity.selected = true;
          }
          if (this.current.relativePath === entity.relativePath) {
            this.current.selected = entity.selected;
          }
        },
        removeSelectionByPath(path, tracker) {
          var trackerIndex = tracker.indexOf(path);
          if (trackerIndex >= 0) {
            tracker.splice(trackerIndex, 1);
            if (this.current.relativePath === path) {
              this.current.selected = false;
            }
            for (var file in this.current.files) {
              if (this.current.files[file].relativePath === path) {
                this.current.files[file].selected = false;
                break;
              }
            }
            for (var folder in this.current.folders) {
              if (this.current.folders[folder].relativePath === path) {
                this.current.folders[folder].selected = false;
                break;
              }
            }
          }          
        },
        processSelections(operation) {
          this.errors = [];
          var data = {
            filePaths: [],
            folderPaths: []
          };
          var selectionCount = 0;
          var path = this.current.relativePath || '';
          while (path.indexOf('.') === 0 || path.indexOf('/') === 0) {
            path = path.substr(1);
          }
          for (var selectedFilePath in this.selectedFilePaths) {
            data.filePaths.push(this.selectedFilePaths[selectedFilePath]);
            selectionCount++;
          }
          for (var selectedFolderPath in this.selectedFolderPaths) {
            data.folderPaths.push(this.selectedFolderPaths[selectedFolderPath]);
            selectionCount++;
          }
          var operationUrl = `./api/resource/selection/${operation}/${path}`;
          if (selectionCount <= 0) {
            this.pushError('You must select at least one file or folder!');
          }
          if (operation !== 'delete' || confirm(`Are you sure you wish to delete ${selectionCount} files/folders?`)) {
            this.$http.post(operationUrl, data).then(response => {
              this.selectedFilePaths = [];
              this.selectedFolderPaths = [];
              this.loadFolder(response.body, true);
              this.mode = 'browser';
            }, response => {
              this.pushError('An HTTP error prevented your selections from being processed!');
            });
          }
        },
        upload(e) {
          this.errors = [];
          var uploadFormData = new FormData();
          var uploadFileCtr = 0;
          for (var filePtr = 1; filePtr <= this.maxFiles; filePtr++) {
            var uploadFileId = 'uploaderFile' + filePtr;
            var uploadFileElement = document.getElementById(uploadFileId);
            if (uploadFileElement && uploadFileElement.files && uploadFileElement.files.length) {
              uploadFormData.append(uploadFileId, uploadFileElement.files[0]);
              uploadFileCtr++;
            }
          }
          if (uploadFileCtr <= 0) {
            this.pushError('You must select at least one file to upload!');
          }
          if (!this.errors.length) {
            this.$http.post(this.uploadPath, uploadFormData).then(response => {
              this.loadFolder(response.body, true);
              for (var filePtr = 1; filePtr <= this.maxFiles; filePtr++) {
                var uploadFileId = 'uploaderFile' + filePtr;
                var uploadFileElement = document.getElementById(uploadFileId);
                if (uploadFileElement && uploadFileElement.files && uploadFileElement.files.length) {
                  uploadFileElement.value = null;
                }
              }
              this.mode = 'browser';
            }, response => {
              this.pushError('An HTTP error prevented the files from uploading.');
            });
          }
        },
        prepNaming(type, entity) {
          this.naming.type = type;
          this.naming.entity = entity;
          if (entity) {
            this.naming.title = `Rename ${type} '${entity.name}`;
            this.naming.new = entity.name;
          }
          else {
            this.naming.title = `Create ${type} in ${this.current.relativePath}`;
            this.naming.new = 'New Entity';
          }
          this.mode = 'naming';
        },
        completeNaming(type, entity) {
          this.errors = [];
          if (!this.naming.new) {
            this.pushError('A new name is required');
          }
          if (this.naming.entity && this.naming.new && this.naming.new === this.naming.entity.name) {
            this.pushError('The new name cannot be the same as the old one!');
          }
          var path = this.naming.entity ? this.naming.entity.relativePath : this.current.relativePath;
          while (path.indexOf('.') === 0 || path.indexOf('/') === 0) {
            path = path.substr(1);
          }
          var namingUrl = `./api/resource/${this.naming.type}/${path}?newName=${this.naming.new}`;
          if (!this.errors.length) {
            var promise = this.naming.entity
              ? this.$http.put(namingUrl)
              : this.$http.post(namingUrl);

            promise.then(response => {
              this.loadFolder(response.body, true);
              this.mode = 'browser';
            }, response => {
              this.pushError('An HTTP error prevented the naming operation from completing.');
            });
          }
        },
        pushError(text) {
          this.errors.push({ type: 'Error', text: text });
        }
      },
      data: {
        breadcrumbs: [],
        current: {},
        uploadPath: '',
        selectedFolderPaths: [],
        selectedFilePaths: [],
        visible: false,
        mode: 'browser',
        maxFiles: 10,
        naming: {
          title: '',
          new: '',
          type: '',
          entity: null
        },
        errors: []
      }
    });
    return component;
  }
};