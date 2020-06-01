import React from 'react';
import pengineHelpers from "./pengine.core.web.helpers";

class PEngineArticleEditor extends React.Component {

  constructor(props) {
    super(props);

    this.state = {
      article: {
        legacyID: null,
        name: 'New Article',
        description: '',
        category: '',
        contentUrl: '',
        defaultSection: '',
        visibleFlag: false,
        noIndexFlag: true,
        uniqueName: null,
        hideDropDownFlag: false,
        hideButtonsFlag: false,
        createdUTC: null,
        modifiedUTC: null,
        sections: [
          {
            name: 'New Section',
            data: '',
            sortOrder: 0,
            uniqueName: null,
            createdUTC: null,
            modifiedUTC: null,
          }
        ]
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
      if (elements[1] === 'article') {
        this.fireEvent("edit", elements[1], (elements.length > 2 ? elements[2] : null));
      }
    }
  }

  fireEvent = (eventName, type, guid, data) => {
    switch (eventName || "")  {
      case "edit":
        pengineHelpers.updateEditorLocationHash(type, guid);
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
      prevState.article = {
        legacyID: null,
        name: 'New Article',
        description: '',
        category: '',
        contentUrl: '',
        defaultSection: '',
        visibleFlag: false,
        noIndexFlag: true,
        uniqueName: null,
        hideDropDownFlag: false,
        hideButtonsFlag: false,
        createdUTC: null,
        modifiedUTC: null,
        sections: [
          {
            name: 'New Section',
            data: '',
            sortOrder: 0,
            uniqueName: null,
            createdUTC: null,
            modifiedUTC: null,
          }
        ]
      };
      return prevState;
    });
  }

  getUrl = (guid) => {
    return guid ? pengineHelpers.fixUrl(`api/articles/${guid}`) : pengineHelpers.fixUrl(`api/articles/`);
  }

  load = (guid) => {
    pengineHelpers.fetch(this.getUrl(guid), {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
      body: null
    })
    .then(pengineHelpers.getCombinedJsonResponse, () => {
      this.pushError('A Network error prevented the article from being fetched!');
    })
    .then(combined => {
      if (combined.response.ok) {
        this.setState(prevState => ({
          ...prevState,
          article: combined.data
        }));
      } else {
        this.pushError('An HTTP error prevented the article from being fetched!');
      }
    });
  }

  updateArticleField = (e, fieldName) => {
    let fieldValue = e.target.value;
    if (e.target.type === 'checkbox' || e.target.type === 'radio') {
      fieldValue = e.target.checked;
    }
    this.setState(prevState => {
      prevState.article[fieldName] = fieldValue;
      return prevState;
    });
  }

  save = (e) => {
    pengineHelpers.fetch(this.getUrl(), {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(this.state.article),
    })
    .then(pengineHelpers.getCombinedJsonResponse, () => {
      this.pushError('A Network error prevented the article from being saved!');
    })
    .then(combined => {
      if (combined.response.ok) {
        this.setState(prevState => ({
          ...prevState,
          article: combined.data
        }));
        window.location.reload();
      } else {
        if (combined.data && combined.data.logMessages) {
          this.setState(prevState => ({
            ...prevState,
            errors: combined.data.logMessages
          }));
        } else {
          this.pushError('An HTTP error prevented the article from being saved!');
        }
      }
    });
  }

  deleteConfirm = (e) => {
    let confirmMessage = `Are you sure you want to delete this article?`;
    if (this.state.article.name) {
      confirmMessage = `Are you sure you want to delete this article entitled "${this.state.article.name}"?`;
    }
    if (confirm(confirmMessage)) {
      this.delete();
    }
  }

  delete = (e) => {
    pengineHelpers.fetch(this.getUrl(this.state.article.guid), {
      method: 'DELETE',
      headers: { 
        'Content-Type': 'application/json',
      },
      body: null
    })
    .then(pengineHelpers.getCombinedJsonResponse, () => {
      this.pushError('A Network error prevented the article from being deleted!');
    })
    .then(combined => {
      if (combined.response.ok) {
        this.cancel();
        window.location.reload();
      } else {
        this.pushError('An HTTP error prevented the article from being deleted!');
      }
    });
  }  

  cancel = (e) => {
    this.reset();
    pengineHelpers.updateEditorLocationHash();

    this.setState(prevState => ({
      ...prevState,
      visible: false
    }));

    document.body.style.overflow = 'initial';
  }

  render = () => {
    return (
      <div id="pengine-article-editor-content">
        {
          this.state.visible ?
            <div className="dialog-container">
              <span className="form-header-text">
                { this.state.article.guid ? ('Editing Article "' + this.state.article.name + '"') : 'Adding New Article' }
              </span>
              {
                (this.state.article.createdUTC || this.state.article.modifiedUTC) ?
                <span className="form-subheader-text">
                  Created: <span className="datetime-display">{ this.state.article.createdUTC }</span>
                  &nbsp; | &nbsp;
                  Modified: <span className="datetime-display">{ this.state.article.modifiedUTC }</span>
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
                </div>
              </div>
              <div className="panel">
                <div className="panel-right">
                  <button type="button" onClick={(e) => this.save(e)}>Save</button>
                  {
                    (this.state.article && this.state.article.guid) ?
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

export default PEngineArticleEditor;