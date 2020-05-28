import React from 'react';
import pengineHelpers from "./pengine.core.web.helpers";

class PEngineUploader extends React.Component {

  constructor(props) {
    super(props);

    this.state = {
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

    this.fireEvent = this.fireEvent.bind(this);
    this.hide = this.hide.bind(this);
    this.get = this.get.bind(this);
    this.loadFolder = this.loadFolder.bind(this);
    this.navigate = this.navigate.bind(this);
    this.select = this.select.bind(this);
    this.removeSelectionByPath = this.removeSelectionByPath.bind(this);
    this.processSelections = this.processSelections.bind(this);
    this.upload = this.upload.bind(this);
    this.prepNaming = this.prepNaming.bind(this);

    this.pushError = this.pushError.bind(this);
  }

  fireEvent(eventName) {
    switch (eventName || "") {
      case "show":
        document.body.style.overflow = 'hidden';
        window.scrollTo(0, 0);
        this.setState({
          visible: true
        });
        this.get();
        break;
    }
  }

  hide() {
    this.setState({
      visible: false
    });
    document.body.style.overflow = 'initial';
  }

  get(path, skipBreadcrumb) {
    this.setState({
      errors: [],
      current: {}
    });
    path = path || '';
    while (path.indexOf('.') === 0 || path.indexOf('/') === 0) {
      path = path.substr(1);
    }
    let getUrl = `api/resource/folder/${path}`;
    return fetch(getUrl).then(response => {
      if (response.ok) {
        this.loadFolder(response.json(), skipBreadcrumb);
        this.setState({
          uploadPath: `api/resource/file/${path}`
        });
      } else {
        this.pushError('An HTTP error prevented the folder from loading.');
      }
    }, () => {
      this.pushError('An Network error prevented the folder from loading.');
    });
  }

  loadFolder(folderData, skipBreadcrumb) {
    if (folderData.files) {
      for (let fileIndex in folderData.files) {
        let fileSelectionIndex = this.state.selectedFilePaths.indexOf(folderData.files[fileIndex].relativePath);
        folderData.files[fileIndex].selected = (fileSelectionIndex >= 0);
      }
    }
    if (folderData.folders) {
      for (let folderIndex in folderData.folders) {
        let folderSelectionIndex = this.state.selectedFolderPaths.indexOf(folderData.folders[folderIndex].relativePath);
        folderData.folders[folderIndex].selected = (folderSelectionIndex >= 0);
      }
    }
    let fileSelectionIndex = this.state.selectedFilePaths.indexOf(folderData.relativePath);
    let folderSelectionIndex = this.state.selectedFolderPaths.indexOf(folderData.relativePath);
    folderData.selected = fileSelectionIndex >= 0 || folderSelectionIndex >= 0;
    if (!skipBreadcrumb) {
      this.setState(prevState => ({
        breadcrumbs: [ ...prevState.breadcrumbs, { title: folderData.name, path: folderData.relativePath }],
        current: folderData
      }));
    }
  }

  navigate(breadcrumb) {
    let newBreadcrumbs = [];
    for (let breadcrumbIndex in this.state.breadcrumbs) {
      newBreadcrumbs.push(this.state.breadcrumbs[breadcrumbIndex]);
      if (this.state.breadcrumbs[breadcrumbIndex] === breadcrumb) {
        break;
      }
    }
    this.setState({
      breadcrumbs: newBreadcrumbs
    });
    this.get(breadcrumb.path, true);
  }

  select(entity, tracker) {
    let trackerIndex = tracker.indexOf(entity.relativePath);
    if (trackerIndex >= 0) {
      tracker.splice(trackerIndex, 1);
      entity.selected = false;
    }
    else {
      tracker.push(entity.relativePath);
      entity.selected = true;
    }
    if (this.state.current.relativePath === entity.relativePath) {
      this.setState(prevState => ({
        current: {
          ...prevState.current,
          selected: entity.selected
        }
      }));
    }
  }

  removeSelectionByPath(path, tracker) {
    let trackerIndex = tracker.indexOf(path);
    if (trackerIndex >= 0) {
      tracker.splice(trackerIndex, 1);
      if (this.state.current.relativePath === path) {
        this.setState(prevState => ({
          current: {
            ...prevState.current,
            selected: false
          }
        }));
      }
      this.setState(prevState => ({
        current: {
          ...prevState.current,
          files: this.state.current.files.map((file) => {
            if (file.relativePath === path) {
              file.selected = false;
            }
            return file;
          }),
          folders: this.state.current.folders.map((folder) => {
            if (folder.relativePath === path) {
              folder.selected = false;
            }
            return folder;
          })
        }
      }));
    }
  }

  processSelections(operation) {
    this.setState({
      errors: []
    });
    let data = {
      filePaths: [],
      folderPaths: []
    };
    let selectionCount = 0;
    let path = this.state.current.relativePath || '';
    while (path.indexOf('.') === 0 || path.indexOf('/') === 0) {
      path = path.substr(1);
    }
    for (let selectedFilePath in this.state.selectedFilePaths) {
      data.filePaths.push(this.state.selectedFilePaths[selectedFilePath]);
      selectionCount++;
    }
    for (let selectedFolderPath in this.state.selectedFolderPaths) {
      data.folderPaths.push(this.state.selectedFolderPaths[selectedFolderPath]);
      selectionCount++;
    }
    let operationUrl = `api/resource/selection/${operation}/${path}`;
    if (selectionCount <= 0) {
      this.pushError('You must select at least one file or folder!');
    }
    if (operation !== 'delete' || confirm(`Are you sure you wish to delete ${selectionCount} files/folders?`)) {
      fetch(operationUrl, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
      }).then(response => {
        if (response.ok) {
          this.setState({
            selectedFilePaths: [],
            selectedFolderPaths: [],
            mode: 'browser'
          });
          this.loadFolder(response.json(), true);
        } else {
          this.pushError('An HTTP error prevented your selections from being processed!');
        }
      }, () => {
        this.pushError('A Network error prevented your selections from being processed!');
      });
    }
  }

  upload() {
    this.setState({
      errors: []
    });
    let errored = false;
    let uploadFormData = new FormData();
    let uploadFileCtr = 0;
    for (var filePtr = 1; filePtr <= this.state.maxFiles; filePtr++) {
      let uploadFileId = 'uploaderFile' + filePtr;
      let uploadFileElement = document.getElementById(uploadFileId);
      if (uploadFileElement && uploadFileElement.files && uploadFileElement.files.length) {
        uploadFormData.append(uploadFileId, uploadFileElement.files[0]);
        uploadFileCtr++;
      }
    }
    if (uploadFileCtr <= 0) {
      this.pushError('You must select at least one file to upload!');
      errored = true;
    }
    if (!errored) {
      fetch(this.state.uploadPath, {
        method: 'POST',
        body: uploadFormData
      }).then(response => {
        if (response.ok) {
          this.loadFolder(response.json(), true);
          for (var filePtr = 1; filePtr <= this.state.maxFiles; filePtr++) {
            let uploadFileId = 'uploaderFile' + filePtr;
            let uploadFileElement = document.getElementById(uploadFileId);
            if (uploadFileElement && uploadFileElement.files && uploadFileElement.files.length) {
              uploadFileElement.value = null;
            }
          }
          this.setState({
            mode: 'browser'
          });
        } else {
          this.pushError('An HTTP error prevented the files from uploading.');  
        }
      }, () => {
        this.pushError('A Network error prevented the files from uploading.');
      });
    }
  }

  prepNaming(type, entity) {
    this.setState(prevState => ({
      naming: {
        ...prevState.naming,
        type: type,
        entity: entity,
        title: entity ? `Rename ${type} '${entity.name}` : `Create ${type} in ${prevState.current.relativePath}`,
        new: entity ? entity.name : 'New Entity'
      },
      mode: 'naming'
    }));
  }

  completeNaming(type, entity) {
    let errored = false;
    this.setState({
      errors: []
    });

    if (!this.state.naming.new) {
      this.pushError('A new name is required');
      errored = true;
    }
    if (this.state.naming.entity && this.state.naming.new && this.state.naming.new === this.state.naming.entity.name) {
      this.pushError('The new name cannot be the same as the old one!');
      errored = true;
    }
    let path = this.state.naming.entity ? this.state.naming.entity.relativePath : this.state.current.relativePath;
    while (path.indexOf('.') === 0 || path.indexOf('/') === 0) {
      path = path.substr(1);
    }
    let namingUrl = `api/resource/${this.naming.type}/${path}?newName=${this.naming.new}`;
    if (!errored) {      
      fetch(namingUrl, {
        method: this.naming.entity ? 'PUT' : 'POST'
      }).then(response => {
        if (response.ok) {
          this.loadFolder(response.json(), true);
          this.setState({
            mode: 'browser'
          });
        } else {
          this.pushError('An HTTP error prevented the naming operation from completing.');  
        }
      }, () => {
        this.pushError('A Network error prevented the naming operation from completing.');
      });
    }
  }

  pushError(text) {
    this.setState(prevState => ({
      errors: [ ...prevState.errors, { type: 'Error', text: text }]
    }));
  }

  render() {

  }
}

export default PEngineUploader;