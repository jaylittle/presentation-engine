import React from 'react';
import pengineHelpers from "./pengine.core.web.helpers";

class PEngineUploader extends React.Component {

  constructor(props) {
    super(props);

    this.state = {
      breadcrumbs: [],
      current: {
        folders: [ ],
        files: [ ],
      },
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

  reset = () => {
    this.setState({
      breadcrumbs: [],
      current: {
        folders: [ ],
        files: [ ],
      },
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
    });
  }

  fireEvent = (eventName) => {
    switch (eventName || "") {
      case "show":
        this.reset();
        document.body.style.overflow = 'hidden';
        window.scrollTo(0, 0);
        this.setState({
          visible: true
        });
        this.get();
        break;
    }
  }

  hide = () => {
    this.setState({
      visible: false
    });
    document.body.style.overflow = 'initial';
  }

  get = (path, skipBreadcrumb) => {
    this.setState({
      errors: [],
      current: {
        folders: [ ],
        files: [ ]
      }
    });
    path = path || '';
    while (path.indexOf('.') === 0 || path.indexOf('/') === 0) {
      path = path.substr(1);
    }
    let getUrl = `api/resource/folder/${path}`;
    return pengineHelpers.fetch(getUrl)
      .then(pengineHelpers.getCombinedJsonResponse, () => {
        this.pushError('An Network error prevented the folder from loading.');
      })
      .then(combined => {
        if (combined.response.ok) {
          this.loadFolder(combined.data, skipBreadcrumb);
          this.setState({
            uploadPath: `api/resource/file/${path}`
          });
        } else {
          this.pushError('An HTTP error prevented the folder from loading.');
        }
      });
    
  }

  loadFolder = (folderData, skipBreadcrumb) => {
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
        breadcrumbs: [ ...prevState.breadcrumbs, { title: folderData.name, path: folderData.relativePath }]
      }));
    }
    this.setState({
      current: folderData
    });
  }

  navigate = (breadcrumb) => {
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

  select = (entity, tracker) => {
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

  removeSelectionByPath = (path, tracker) => {
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
          files: (this.state.current.files).map((file) => {
            if (file.relativePath === path) {
              file.selected = false;
            }
            return file;
          }),
          folders: (this.state.current.folders).map((folder) => {
            if (folder.relativePath === path) {
              folder.selected = false;
            }
            return folder;
          })
        }
      }));
    }
  }

  processSelections = (operation) => {
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
      pengineHelpers.fetch(operationUrl, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
      })
      .then(pengineHelpers.getCombinedJsonResponse, () => {
        this.pushError('A Network error prevented your selections from being processed!');
      })
      .then(combined => {
        if (combined.response.ok) {
          this.setState({
            selectedFilePaths: [],
            selectedFolderPaths: [],
            mode: 'browser'
          });
          this.loadFolder(combined.data, true);
        } else {
          this.pushError('An HTTP error prevented your selections from being processed!');
        }
      });
    }
  }

  upload = (e) => {
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
      pengineHelpers.fetch(this.state.uploadPath, {
        method: 'POST',
        body: uploadFormData
      })
      .then(pengineHelpers.getCombinedJsonResponse, () => {
        this.pushError('A Network error prevented the files from uploading.');
      })
      .then(combined => {
        if (combined.response.ok) {
          this.loadFolder(combined.data, true);
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
      });
    }
  }

  prepNaming = (type, entity) => {
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

  completeNaming = (type, entity) => {
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
    let namingUrl = `api/resource/${this.state.naming.type}/${path}?newName=${this.state.naming.new}`;
    if (!errored) {      
      pengineHelpers.fetch(namingUrl, {
        method: this.state.naming.entity ? 'PUT' : 'POST'
      })
      .then(pengineHelpers.getCombinedJsonResponse, () => {
        this.pushError('A Network error prevented the naming operation from completing.');
      })
      .then(combined => {
        if (combined.response.ok) {
          this.loadFolder(combined.data, true);
          this.setState({
            mode: 'browser'
          });
        } else {
          this.pushError('An HTTP error prevented the naming operation from completing.');  
        }
      });
    }
  }

  pushError = (text) => {
    this.setState(prevState => ({
      errors: [ ...prevState.errors, { type: 'Error', text: text }]
    }));
  }

  updateNaming = (e) => {
    let newNew = e.target.value;
    this.setState(prevState => ({
      naming: {
        ...prevState.naming,
        new: newNew
      }
    }));
  }

  updateMode = (newMode) => {
    this.setState({
      mode: newMode
    });
  }

  render = () => {
    return (
      <div id="pengine-uploader">
        { 
          this.state.visible ?
            <div className="dialog-container" v-if="visible">
              <span className="form-header-text">
                <span>Uploader</span>
                {
                  this.state.mode === 'browser' ?
                    this.state.breadcrumbs.map((breadcrumb) => 
                      <span key={breadcrumb.title}>
                        &nbsp;&nbsp;
                        <a href="#" onClick={() => this.navigate(breadcrumb)}>{ breadcrumb.title }</a>
                      </span>
                    )
                  : null
                }
                {
                  this.state.mode === 'selections' ?
                    <span>
                    File &amp; Folder Selections
                    </span>
                  : null
                }
                {
                  this.state.mode === 'multiupload' ?
                    <span>
                      &nbsp;:&nbsp;
                      Upload Files to { this.state.current.relativePath }
                    </span>
                  : null
                }
                {
                  this.state.mode === 'naming' ?
                    <span>
                      { this.state.naming.title }
                    </span>
                  : null
                }
              </span>
              {
                this.state.errors ?
                  <ul className="form-errors">
                    {
                      this.state.errors.map((error, key) =>
                        <li key={key}>{ error.text }</li>
                      )
                    }
                  </ul>
                : null
              }
              <div>
                {
                  this.state.mode === 'multiupload' ?
                    <div className="form-container">
                      {
                        Array.from(Array(this.state.maxFiles).keys()).map((index, key) =>
                          <div className="edit-row" key={key}>
                            <div className="edit-label">{ 'File #' + (index + 1) }:</div>
                            <div className="edit-field">
                              <input type="file" id={ 'uploaderFile' + (index + 1) } />
                            </div>
                          </div>
                        )
                      }
                    </div>
                  : null
                }
                {
                  this.state.mode === 'naming' ?
                    <div className="form-container">
                      <div className="edit-row">
                        <div className="edit-label">New Name:</div>
                        <div className="edit-field">
                          <input type="text" value={this.state.naming.new} onChange={(e) => this.updateNaming(e)} />
                        </div>
                      </div>
                    </div>
                  : null
                }
                {
                  (this.state.mode !== 'multiupload' && this.state.mode !== 'naming') ?
                    <div className="list-container-overflow">
                      {
                        this.state.mode === 'selections' ?
                          <table className="list-table" cellSpacing="0" border="1">
                            <thead>
                              <tr>
                                <th>Type</th>
                                <th>Path</th>
                                <th>Actions</th>
                              </tr>
                            </thead>
                            <tbody>
                              {
                                this.state.selectedFolderPaths.map((selectedFolderPath) =>
                                  <tr key={selectedFolderPath}>
                                    <td>Folder</td>
                                    <td>{ selectedFolderPath }</td>
                                    <td>
                                      <a href="#" className="file-edit-button-view listbutton"
                                        onClick={() => this.removeSelectionByPath(selectedFolderPath, selectedFolderPaths)}>Remove</a>
                                    </td>
                                  </tr>
                                )
                              }
                              {
                                this.state.selectedFilePaths.map((selectedFilePath) =>
                                  <tr key={selectedFilePath}>
                                    <td>File</td>
                                    <td>{ selectedFilePath }</td>
                                    <td>
                                      <a href="#" className="file-edit-button-view listbutton"
                                        onClick={() => this.removeSelectionByPath(selectedFilePath, selectedFilePaths)}>Remove</a>
                                    </td>
                                  </tr>
                                )
                              }
                            </tbody>
                          </table>
                        : null
                      }
                      {
                        this.state.mode === 'browser' ?
                          <table className="list-table" cellSpacing="0" border="1">
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
                              {
                                this.state.breadcrumbs.length > 1 ?
                                  <tr>
                                    <td>Folder</td>
                                    <td onClick={() => this.navigate(this.state.breadcrumbs[this.state.breadcrumbs.length - 2])}>[Parent]</td>
                                    <td>N/A</td>
                                    <td>N/A</td>
                                    <td>
                                      &nbsp;
                                    </td>
                                  </tr>
                                : null
                              }
                              {
                                this.state.current.folders.map((folder) =>
                                  <tr key={folder.relativePath} className={ folder.selected ? 'selected' : null } onDoubleClick={() => this.select(folder, this.state.selectedFolderPaths)}>
                                    <td>Folder</td>
                                    <td onClick={(e) => this.get(folder.relativePath)}>{ folder.name }</td>
                                    <td>N/A</td>
                                    <td>N/A</td>
                                    <td>
                                      {
                                        this.state.breadcrumbs.length > 1 ?
                                          <a href="#" className="file-edit-button-view listbutton" onClick={() => this.prepNaming('folder', folder)}>[Rename]</a>
                                        : null
                                      }
                                    </td>
                                  </tr>
                                )
                              }
                              {
                                this.state.current.files.map((file) => 
                                  <tr key={file.relativePath} className={ file.selected ? 'selected' : null } onDoubleClick={() => this.select(file, this.state.selectedFilePaths)}>
                                    <td>File</td>
                                    <td>
                                      <a href={file.relativePath} className="file-edit-button-view listbutton" target="_blank">{ file.name }</a>
                                    </td>
                                    <td>{ file.modified }</td>
                                    <td>{ file.size }</td>
                                    <td>
                                      {
                                        this.state.breadcrumbs.length > 1 ?
                                          <a href="#" className="file-edit-button-view listbutton" onClick={() => this.prepNaming('file', file)}>[Rename]</a>
                                        : null
                                      }
                                    </td>
                                  </tr>
                                )
                              }
                            </tbody>
                          </table>
                        : null
                      }
                    </div>
                  : null
                }
              </div>
              <div className="panel">
                {
                  (this.state.breadcrumbs.length > 1 && this.state.mode === 'browser') ?
                  <div className="panel-right">
                    <button type="button" onClick={() => this.updateMode('selections')}>Selections</button>
                    <button type="button" onClick={() => this.updateMode('multiupload')}>Multi-Upload</button>
                    {
                      (this.state.breadcrumbs.length > 2 && !this.state.current.selected) ?
                        <button type="button" onClick={() => this.select(this.state.current, this.state.selectedFolderPaths)}>Select Folder</button>
                      : null
                    }
                    {
                      (this.state.breadcrumbs.length > 2 && this.state.current.selected) ?
                        <button type="button" onClick={() => this.select(this.state.current, this.state.selectedFolderPaths)}>Unselect Folder</button>
                      : null
                    }
                    {
                      (!this.state.selectedFilePaths.length && !this.state.selectedFolderPaths.length) ?
                        <button type="button" disabled={!this.state.selectedFilePaths.length && !this.state.selectedFolderPaths.length} onClick={() => this.processSelections('move')}>Move Selections to</button>
                      : null
                    }
                    {
                      (!this.state.selectedFilePaths.length && !this.state.selectedFolderPaths.length) ?
                        <button type="button" disabled={!this.state.selectedFilePaths.length && !this.state.selectedFolderPaths.length} onClick={() => this.processSelections('copy')}>Copy Selections to</button>
                      : null
                    }
                    <button type="button" onClick={() => this.prepNaming('folder', null)}>Create Folder</button>
                    <button type="button" onClick={this.hide}>Close</button>
                  </div>
                  : null
                }
                {
                  (this.state.breadcrumbs.length > 1 && this.state.mode === 'selections') ?
                    <div className="panel-right">
                      <button type="button" onClick={() => this.updateMode('browser')}>Browser</button>
                      <button type="button" onClick={() => this.updateMode('multiupload')}>Multi-Upload</button>
                      <button type="button" disabled={!this.state.selectedFilePaths.length && !this.state.selectedFolderPaths.length} 
                        onClick={() => this.processSelections('delete')}>
                          Delete Selections
                      </button>
                      <button type="button" onClick={this.hide}>Close</button>
                    </div>
                  : null
                }
                {
                  (this.state.breadcrumbs.length > 1 && this.state.mode === 'multiupload') ?
                    <div className="panel-right">
                      <button type="button" onClick={() => this.updateMode('selections')}>Selections</button>
                      <button type="button" onClick={() => this.updateMode('browser')}>Browser</button>
                      <button type="button" onClick={this.upload}>Upload Files</button>
                      <button type="button" onClick={this.hide}>Close</button>
                    </div>
                  : null
                }
                {
                  (this.state.breadcrumbs.length > 1 && this.state.mode === 'naming') ?
                  <div className="panel-right">
                    <button type="button" onClick={() => this.updateMode('selections')}>Selections</button>
                    <button type="button" onClick={() => this.updateMode('browser')}>Browser</button>
                    <button type="button" onClick={() => this.updateMode('multiupload')}>Multi-Upload</button>
                    <button type="button" onClick={this.completeNaming}>{ this.state.naming.title }</button>
                    <button type="button" onClick={this.hide}>Close</button>
                  </div>
                  : null
                }
                {
                  this.state.breadcrumbs.length <= 1 ?
                    <div className="panel-right">
                      <button type="button" onClick={this.hide}>Close</button>
                    </div>
                  : null
                }
              </div>
            </div>
          : null
        }
        {
          this.state.visible ? (
            <div id="modal-overlay"></div>
          ) : null
        }
      </div>
    );
  }
}

export default PEngineUploader;