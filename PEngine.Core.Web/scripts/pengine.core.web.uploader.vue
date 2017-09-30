<template>
  <div id="pengine-uploader">
    <div class="dialog-container" v-if="visible">
      <span class="form-header-text">
        <span>Uploader</span>
        <span v-for="breadcrumb in breadcrumbs" v-if="mode === 'browser'">
          &nbsp;:&nbsp;
          <a href="#" v-on:click="navigate(breadcrumb)">{{ breadcrumb.title }}</a>
        </span>
        <span v-if="mode === 'selections'">
          File &amp; Folder Selections
        </span>
        <span v-if="mode === 'multiupload'">
          &nbsp;:&nbsp;
          Upload Files to {{ current.relativePath }}
        </span>
        <span v-if="mode === 'naming'">
          {{ naming.title }}
        </span>
      </span>
      <ul class="form-errors" v-if="errors">
        <li v-for="error in errors">{{ error.text }}</li>
      </ul>
      <div>
        <div class="form-container-overflow" v-if="mode === 'multiupload'">
          <div class="edit-row" v-for="n in maxFiles">
            <div class="edit-label">File #{{n}}:</div>
            <div class="edit-field">
              <input type="file" v-bind:id="'uploaderFile' + n">
            </div>
          </div>
        </div>
        <div class="form-container-overflow" v-if="mode === 'naming'">
          <div class="edit-row">
            <div class="edit-label">New Name:</div>
            <div class="edit-field">
              <input type="text" v-model="naming.new">
            </div>
          </div>
        </div>
        <div class="list-container-overflow" v-if="mode !== 'multiupload' && mode !== 'naming'">
          <table class="list-table" cellspacing="0" border="1" v-if="mode === 'selections'">
            <thead>
              <tr>
                <th>Type</th>
                <th>Path</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="selectedFolderPath in selectedFolderPaths">
                <td>Folder</td>
                <td>{{ selectedFolderPath }}</td>
                <td>
                  <a href="#" class="file-edit-button-view listbutton" v-on:click="removeSelectionByPath(selectedFolderPath, selectedFolderPaths)">Remove</a>
                </td>
              </tr>
              <tr v-for="selectedFilePath in selectedFilePaths">
                <td>File</td>
                <td>{{ selectedFilePath }}</td>
                <td>
                  <a href="#" class="file-edit-button-view listbutton" v-on:click="removeSelectionByPath(selectedFilePath, selectedFilePaths)">Remove</a>
                </td>
              </tr>
            </tbody>
          </table>
          <table class="list-table" cellspacing="0" border="1" v-if="mode === 'browser'">
            <thead>
              <tr>
                <th>Type</th>
                <th>Name</th>
                <th>Date</th>
                <th>Count/Size</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="breadcrumbs.length > 1">
                <td>Folder</td>
                <td v-on:click="navigate(breadcrumbs[breadcrumbs.length - 2])">[Parent]</td>
                <td>N/A</td>
                <td>N/A</td>
                <td>
                  &nbsp;
                </td>
              </tr>
              <tr v-for="folder in current.folders" v-bind:class="{ selected: folder.selected }" v-on:dblclick="select(folder, selectedFolderPaths)">
                <td>Folder</td>
                <td v-on:click="get(folder.relativePath)">{{ folder.name }}</td>
                <td>N/A</td>
                <td>N/A</td>
                <td>
                  <a href="#" class="file-edit-button-view listbutton" v-if="breadcrumbs.length > 1" v-on:click="prepNaming('folder', folder)">[Rename]</a>
                </td>
              </tr>
              <tr v-for="file in current.files" v-bind:class="{ selected: file.selected }" v-on:dblclick="select(file, selectedFilePaths)">
                <td>File</td>
                <td>
                  <a v-bind:href="file.relativePath" class="file-edit-button-view listbutton" target="_blank">{{ file.name }}</a>
                </td>
                <td>{{ file.modified }}</td>
                <td>{{ file.size }}</td>
                <td>
                  <a href="#" class="file-edit-button-view listbutton" v-if="breadcrumbs.length > 1" v-on:click="prepNaming('file', file)">[Rename]</a>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <div class="panel">
        <div class="panel-right" v-if="breadcrumbs.length > 1 && mode === 'browser'">
          <button type="button" v-on:click="mode = 'selections'">Selections</button>
          &nbsp;
          <button type="button" v-on:click="mode = 'multiupload'">Multi-Upload</button>
          &nbsp;
          <button type="button" v-if="breadcrumbs.length > 2 && !current.selected" v-on:click="select(current, selectedFolderPaths)">Select Folder</button>
          <button type="button" v-if="breadcrumbs.length > 2 && current.selected" v-on:click="select(current, selectedFolderPaths)">Unselect Folder</button>
          &nbsp;
          <button type="button" v-bind:disabled="!selectedFilePaths.length && !selectedFolderPaths.length" v-on:click="processSelections('move')">Move Selections to</button>
          &nbsp;
          <button type="button" v-bind:disabled="!selectedFilePaths.length && !selectedFolderPaths.length" v-on:click="processSelections('copy')">Copy Selections to</button>
          &nbsp;
          <button type="button" v-on:click="prepNaming('folder', null)">Create Folder</button>
          &nbsp;
          <button type="button" v-on:click="hide">Close</button>
        </div>
        <div class="panel-right" v-if="breadcrumbs.length > 1 && mode === 'selections'">
          <button type="button" v-on:click="mode = 'browser'">Browser</button>
          &nbsp;
          <button type="button" v-on:click="mode = 'multiupload'">Multi-Upload</button>
          &nbsp;
          <button type="button" v-bind:disabled="!selectedFilePaths.length && !selectedFolderPaths.length" v-on:click="processSelections('delete')">Delete Selections</button>
          &nbsp;
          <button type="button" v-on:click="hide">Close</button>
        </div>
        <div class="panel-right" v-if="breadcrumbs.length > 1 && mode === 'multiupload'">
          <button type="button" v-on:click="mode = 'selections'">Selections</button>
          &nbsp;
          <button type="button" v-on:click="mode = 'browser'">Browser</button>
          &nbsp;
          <button type="button" v-on:click="upload($event)">Upload Files</button>
          &nbsp;
          <button type="button" v-on:click="hide">Close</button>
        </div>
        <div class="panel-right" v-if="breadcrumbs.length > 1 && mode === 'naming'">
          <button type="button" v-on:click="mode = 'selections'">Selections</button>
          &nbsp;
          <button type="button" v-on:click="mode = 'browser'">Browser</button>
          &nbsp;
          <button type="button" v-on:click="mode = 'multiupload'">Multi-Upload</button>
          &nbsp;
          <button type="button" v-on:click="completeNaming()">{{ naming.title }}</button>
          &nbsp;
          <button type="button" v-on:click="hide">Close</button>
        </div>
        <div class="panel-right" v-if="breadcrumbs.length <= 1">
          <button type="button" v-on:click="hide">Close</button>
        </div>
      </div>
    </div>
    <div id="modal-overlay" v-if="visible"></div>
  </div>
</template>

<script>
  import pengineHelpers from "./pengine.core.web.helpers";

  export default {
    name: "pengine-uploader",
    mounted() {
    },
    methods: {
      fireEvent(eventName) {
        switch (eventName || "") {
          case "show":
            document.body.style.overflow = 'hidden';
            window.scrollTo(0, 0);
            this.visible = true;
            this.get();
            break;
        }
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
    data() {
      return {
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
      };
    }
  };
</script>