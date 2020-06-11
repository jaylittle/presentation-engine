import PEHelpers from "./pengine.core.web.helpers";

class PEnginePostEditor extends React.Component {

  constructor(props) {
    super(props);

    this.state = {
      post: {
        legacyID: null,
        name: 'New Post',
        data: '',
        iconFileName: '',
        visibleFlag: false,
        noIndexFlag: true,
        uniqueName: null,
        createdUTC: null,
        modifiedUTC: null,
      },
      visible: false,
      errors: [ ],
      peState: window.pengineState
    };
  }

  componentDidMount = () => {
    this.processLocationHash();
  }

  processLocationHash = () => {
    if (window.location.hash && window.location.hash !== '' && window.location.hash.indexOf('#edit/') === 0) {
      let elements = window.location.hash.split('/');
      if (elements[1] === 'post') {
        this.fireEvent("edit", elements[1], (elements.length > 2 ? elements[2] : null));
      }
    }
  }

  fireEvent = (eventName, type, guid, data) => {
    switch (eventName || "")  {
      case "edit":
        PEHelpers.updateEditorLocationHash(type, guid);
        document.body.style.overflow = 'hidden';
        window.scrollTo(0, 0);

        this.reset();

        if (guid) {
          this.load(guid);
        }

        this.setState(prevState => ({
          ...prevState,
          visible: true
        }));
        break;
    }
  }

  pushError = (text) => {
    this.setState(prevState => ({
      errors: [ ...prevState.errors, { type: 'Error', text: text }]
    }));
  }

  reset = () => {
    this.setState(prevState => {
      prevState.post = {
        legacyID: null,
        name: 'New Post',
        data: '',
        iconFileName: '',
        visibleFlag: false,
        noIndexFlag: true,
        uniqueName: null,
        createdUTC: null,
        modifiedUTC: null,
      };
      return prevState;
    });
  }

  getUrl = (guid) => {
    return guid ? PEHelpers.fixUrl(`api/posts/${guid}`) : PEHelpers.fixUrl(`api/posts/`);
  }

  load = (guid) => {
    PEHelpers.fetch(this.getUrl(guid), {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
      body: null
    })
    .then(PEHelpers.getCombinedJsonResponse, () => {
      this.pushError('A Network error prevented the post from being fetched!');
    })
    .then(combined => {
      if (combined.response.ok) {
        combined.data.iconFileName = combined.data.iconFileName || '';
        this.setState(prevState => ({
          ...prevState,
          post: combined.data
        }));
      } else {
        this.pushError('An HTTP error prevented the post from being fetched!');
      }
    });
  }

  updatePostField = (e, fieldName) => {
    PEHelpers.updateStateField(this, e, [ 'post', fieldName ]);
  }

  save = (e) => {
    PEHelpers.fetch(this.getUrl(), {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(this.state.post),
    })
    .then(PEHelpers.getCombinedJsonResponse, () => {
      this.pushError('A Network error prevented the post from being saved!');
    })
    .then(combined => {
      if (combined.response.ok) {
        if (!this.state.post.guid && combined.data.guid) {
          PEHelpers.updateEditorLocationHash('post', combined.data.guid);
        }
        this.setState(prevState => ({
          ...prevState,
          post: combined.data
        }));
        window.location.reload();
      } else {
        if (combined.data && combined.data.logMessages) {
          this.setState(prevState => ({
            ...prevState,
            errors: combined.data.logMessages
          }));
        } else {
          this.pushError('An HTTP error prevented the post from being saved!');
        }
      }
    });
  }

  deleteConfirm = (e) => {
    let confirmMessage = `Are you sure you want to delete this post?`;
    if (this.state.post.name) {
      confirmMessage = `Are you sure you want to delete this post entitled "${this.state.post.name}"?`;
    }
    if (confirm(confirmMessage)) {
      this.delete();
    }
  }

  delete = (e) => {
    PEHelpers.fetch(this.getUrl(this.state.post.guid), {
      method: 'DELETE',
      headers: { 
        'Content-Type': 'application/json',
      },
      body: null
    })
    .then(PEHelpers.getCombinedJsonResponse, () => {
      this.pushError('A Network error prevented the post from being deleted!');
    })
    .then(combined => {
      if (combined.response.ok) {
        this.cancel();
        window.location.reload();
      } else {
        this.pushError('An HTTP error prevented the post from being deleted!');
      }
    });
  }  

  cancel = (e) => {
    this.reset();
    PEHelpers.updateEditorLocationHash();

    this.setState(prevState => ({
      ...prevState,
      visible: false
    }));

    document.body.style.overflow = 'initial';
  }

  render = () => {
    return (
      <div id="pengine-post-editor-content">
        {
          this.state.visible ?
            <div className="dialog-container">
              <span className="form-header-text">
                { this.state.post.guid ? ('Editing Post "' + this.state.post.name + '"') : 'Adding New Post' }
              </span>
              {
                (this.state.post.createdUTC || this.state.post.modifiedUTC) ?
                <span className="form-subheader-text">
                  Created: <span className="datetime-display">{ this.state.post.createdUTC }</span>
                  &nbsp; | &nbsp;
                  Modified: <span className="datetime-display">{ this.state.post.modifiedUTC }</span>
                </span>
                : null
              }
              {
                (this.state.errors) ?
                <ul className="form-errors">
                  {
                    this.state.errors.map((error, key) =>
                      <li key={key}>{ error.text }</li>
                    )
                  }
                </ul>
                :null
              }
              <div>
                <div className="form-container">
                  <div className="edit-row">
                    <div className="edit-label">Title:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-large" value={this.state.post.name} onChange={(e) => this.updatePostField(e, 'name')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label">Visible:</div>
                    <input type="checkbox" checked={this.state.post.visibleFlag} onChange={(e) => this.updatePostField(e, 'visibleFlag')} />
                  </div>
                  <div className="edit-row">
                    <div className="edit-label">Do Not Index:</div>
                    <input type="checkbox" checked={this.state.post.noIndexFlag} onChange={(e) => this.updatePostField(e, 'noIndexFlag')} />
                  </div>
                  <div className="edit-row">
                    <div className="edit-label">Icon:</div>
                    <div className="edit-field">
                      <select className="edit-control-normal" value={this.state.post.iconFileName} onChange={(e) => this.updatePostField(e, 'iconFileName')}>
                        <option value=""></option>
                        {
                          this.state.peState.iconList.map((icon, key) =>
                            <option key={key} value={icon}>{ icon }</option>
                          )
                        }
                      </select>
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label">Content:</div>
                    <div className="edit-field">
                      <textarea rows="20" className="edit-control" value={this.state.post.data} onChange={(e) => this.updatePostField(e, 'data')}>
                      </textarea>
                    </div>
                  </div>
                </div>
              </div>
              <div className="panel">
                <div className="panel-right">
                  <button type="button" onClick={(e) => this.save(e)}>Save</button>
                  {
                    (this.state.post && this.state.post.guid) ?
                    <button type="button" onClick={(e) => this.deleteConfirm(e)}>Delete</button>
                    : null
                  }
                  <button type="button" onClick={(e) => this.cancel(e)}>Cancel</button>
                </div>
              </div>
            </div>
          : null
        }
        {
          this.state.visible ?
            <div id="modal-overlay"></div>
          : null
        }
      </div>
    );
  }
}

export default PEnginePostEditor;