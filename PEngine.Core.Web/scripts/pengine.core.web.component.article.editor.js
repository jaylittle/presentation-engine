import React from 'react';
import PEHelpers from "./pengine.core.web.helpers";

class PEngineArticleEditor extends React.Component {

  constructor(props) {
    super(props);

    this.state = {
      article: {
        legacyID: null,
        name: 'New Article',
        description: '',
        category: '',
        contentURL: '',
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
      prevState.article = {
        legacyID: null,
        name: 'New Article',
        description: '',
        category: '',
        contentURL: '',
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
    return guid ? PEHelpers.fixUrl(`api/articles/${guid}`) : PEHelpers.fixUrl(`api/articles/`);
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

  updateArticleField = (e, fieldName, subGroup, subGroupKey) => {
    PEHelpers.updateStateField(this, e, [ 'article', subGroup, subGroupKey, fieldName ]);
  }

  save = (e) => {
    PEHelpers.fetch(this.getUrl(), {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(this.state.article),
    })
    .then(PEHelpers.getCombinedJsonResponse, () => {
      this.pushError('A Network error prevented the article from being saved!');
    })
    .then(combined => {
      if (combined.response.ok) {
        if (!this.state.article.guid && combined.data.guid) {
          PEHelpers.updateEditorLocationHash('article', combined.data.guid);
        }
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
    PEHelpers.fetch(this.getUrl(this.state.article.guid), {
      method: 'DELETE',
      headers: { 
        'Content-Type': 'application/json',
      },
      body: null
    })
    .then(PEHelpers.getCombinedJsonResponse, () => {
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
    PEHelpers.updateEditorLocationHash();

    this.setState(prevState => ({
      ...prevState,
      visible: false
    }));

    document.body.style.overflow = 'initial';
  }

  addSection = (e) => {
    this.setState((prevState) => ({
      article: {
        ...prevState.article,
        sections: [
          ...prevState.article.sections,
          {
            name: 'New Section',
            data: '',
            sortOrder: 0,
            uniqueName: null,
            createdUTC: null,
            modifiedUTC: null,
          }
        ]
      }
    }));
  }

  deleteSectionConfirm = (e, sectionIndex) => {
    let confirmMessage = `Are you sure you want to delete this section?`;
    if (this.state.article.sections[sectionIndex] && this.state.article.sections[sectionIndex].name) {
      confirmMessage = `Are you sure you want to delete this section entitled "${this.state.article.sections[sectionIndex].name}"?`;
    }
    if (confirm(confirmMessage)) {
      return this.deleteSection(e, sectionIndex);
    }
  }

  deleteSection = (e, sectionIndex) => {
    this.setState(prevState => {
      if (sectionIndex >= 0) {
        prevState.article.sections.splice(sectionIndex, 1);
      }
      return prevState;
    });
  }

  moveSection = (e, sectionIndex, offset) => {
    this.setState(prevState => {
      if (sectionIndex >= 0) {
        let sectionObject = prevState.article.sections[sectionIndex];
        let newSectionIndex = sectionIndex + offset;
        prevState.article.sections.splice(sectionIndex, 1);
        prevState.article.sections.splice(newSectionIndex, 0, sectionObject);
        for (let currentSectionIndex in prevState.article.sections) {
          prevState.article.sections[currentSectionIndex].sortOrder = currentSectionIndex;
        }
      }
      return prevState;
    });
  }

  toggleSection = (e, sectionIndex) => {
    this.setState(prevState => {
      if (sectionIndex >= 0) {
        prevState.article.sections[sectionIndex].expanded = !prevState.article.sections[sectionIndex].expanded;
      }
      return prevState;
    });
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
                  <div className="edit-row">
                    <div className="edit-label">Title:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-large" value={this.state.article.name} onChange={(e) => this.updateArticleField(e, 'name')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label">Description:</div>
                    <div className="edit-field">
                      <textarea className="edit-control" rows="3" value={this.state.article.description} onChange={(e) => this.updateArticleField(e, 'description')}></textarea>
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label">Content URL:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-large" value={this.state.article.contentURL} onChange={(e) => this.updateArticleField(e, 'contentURL')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label">Category:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-large" value={this.state.article.category} onChange={(e) => this.updateArticleField(e, 'category')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label">Default Section:</div>
                    <div className="edit-field">
                      <select className="edit-control-large" value={this.state.article.defaultSection} onChange={(e) => this.updateArticleField(e, 'defaultSection')}>
                        <option value="">[Not Specified]</option>
                        {
                          this.state.article.sections.map((section, key) => 
                            <option key={key} value={section.name}>{ section.name }</option>
                          )
                        }
                      </select>
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label">Visible:</div>
                    <input type="checkbox" checked={this.state.article.visibleFlag} onChange={(e) => this.updateArticleField(e, 'visibleFlag')} />
                  </div>
                  <div className="edit-row">
                    <div className="edit-label">Do Not Index:</div>
                    <input type="checkbox" checked={this.state.article.noIndexFlag} onChange={(e) => this.updateArticleField(e, 'noIndexFlag')} />
                  </div>
                  <div className="edit-row">
                    <div className="edit-label">Hide Buttons:</div>
                    <input type="checkbox" checked={this.state.article.hideButtonsFlag} onChange={(e) => this.updateArticleField(e, 'hideButtonsFlag')} />
                  </div>
                  <div className="edit-row">
                    <div className="edit-label">Hide Dropdown:</div>
                    <input type="checkbox" checked={this.state.article.hideDropDownFlag} onChange={(e) => this.updateArticleField(e, 'hideDropDownFlag')} />
                  </div>
                  <div className="edit-row">
                    <div className="edit-label">Article Functions:</div>
                    <div className="edit-field">
                      <button type="button" id="section_edit_button_add" onClick={(e) => this.addSection(e)}>Add Section</button>
                    </div>
                  </div>
                </div>
                {
                  this.state.article.sections.map((section, key) => 
                    <div className="form-container border-top" key={key}>
                      <div className="edit-row">
                        <div className="edit-label">Section Name:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-large" value={section.name} onChange={(e) => this.updateArticleField(e, 'name', 'sections', key)}  />
                        </div>
                      </div>
                      <div className="edit-row">
                        <div className="edit-label">Section Functions:</div>
                        <div className="edit-field">
                          {
                            key !== 0 ?
                            <button type="button" onClick={(e) => this.moveSection(e, key, -1)}>Move Up</button>
                            : null
                          }
                          {
                            key !== (this.state.article.sections.length - 1) ?
                            <button type="button" onClick={(e) => this.moveSection(e, key, 1)}>Move Down</button>
                            : null
                          }
                          {
                            section.expanded ?
                            <button type="button" onClick={(e) => this.toggleSection(e, key)}>Hide</button>
                            :
                            <button type="button" onClick={(e) => this.toggleSection(e, key)}>Show</button>
                          }
                          <button type="button" id="section_edit_button_delete" onClick={(e) => this.deleteSectionConfirm(e, key)}>Delete</button>
                        </div>
                      </div>
                      {
                        section.expanded ?
                        <div className="edit-row">
                          <div className="edit-label">Content:</div>
                          <div className="edit-field">
                            <textarea rows="20" className="edit-control" value={section.data} onChange={(e) => this.updateArticleField(e, 'data', 'sections', key)}></textarea>
                          </div>
                        </div>
                        : null
                      }
                    </div>
                  )
                }
                
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