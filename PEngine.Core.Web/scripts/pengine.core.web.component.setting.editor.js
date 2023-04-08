import PEHelpers from "./pengine.core.web.helpers";

class PEngineSettingEditor extends React.Component {

  constructor(props) {
    super(props);

    this.state = {
      settings: {
        ownerName: '',
        ownerEmail: '',
        defaultTitle: '',
        defaultDescription: '',
        pageLanguageCode: '',
        defaultTheme: '',
        logoFrontPage: '',
        perPagePostArchived: 0,
        perPagePostFront: 0,
        perPageSearchResults: 0,
        perPageRSS: 0,
        timeLimitForumPostEdit: 0,
        timeLimitForumToken: 0,
        timeLimitAdminToken: 0,
        disableResume: false,
        disableThemeSelection: false,
        disableQuotes: false,
        disableSearch: false,
        disableRSS: false,
        labelArchivedPostsButton: '',
        labelHomeButton: '',
        labelThemeButton: '',
        labelResumeButton: '',
        labelAdminLoginButton: '',
        labelAdminLogoffButton: '',
        labelPrintButton: '',
        labelQuoteButton: '',
        labelClippyButton : '',
        labelBackToHomeButton: '',
        labelReadTheRestButton: '',
        externalBaseUrl: '',
        twitterCardSite: '',
        userNameAdmin: '',
        cookieDomain: '',
        basePath: '',
        cacheControlSeconds: 0,
        hiddenThemeList: '',
        favIcon: '',
        additionalHeaders: '',
        errorMessageTitle: '',
        errorMessageNotFound: '',
        errorMessageForbidden: '',
        errorMessageException: '',
        summaryModeFront: true,
        newPasswordAdmin: {
          value: '',
          reset: false
        },
        disableSwagger: true,
        disableResponseCompression: false,
        disableInlineContent: false,
      },
      visible: false,
      errors: [ ],
      peState: window.pengineState
    };
  }

  componentDidMount = () => {
    PEHelpers.assignComponentClickEvent(this, "pengine-button-setting", "settings");
    this.processLocationHash();
  }

  processLocationHash = () => {
    if (window.location.hash && window.location.hash !== '' && window.location.hash.indexOf('#edit/') === 0) {
      let elements = window.location.hash.split('/');
      if (elements[1] === 'settings') {
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
        this.load(guid);

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
      prevState.settings = {
        ownerName: '',
        ownerEmail: '',
        defaultTitle: '',
        defaultDescription: '',
        pageLanguageCode: '',
        defaultTheme: '',
        logoFrontPage: '',
        perPagePostArchived: 0,
        perPagePostFront: 0,
        perPageSearchResults: 0,
        perPageRSS: 0,
        timeLimitForumPostEdit: 0,
        timeLimitForumToken: 0,
        timeLimitAdminToken: 0,
        disableResume: false,
        disableThemeSelection: false,
        disableQuotes: false,
        disableSearch: false,
        disableRSS: false,
        labelArchivedPostsButton: '',
        labelHomeButton: '',
        labelThemeButton: '',
        labelResumeButton: '',
        labelAdminLoginButton: '',
        labelAdminLogoffButton: '',
        labelPrintButton: '',
        labelQuoteButton: '',
        labelClippyButton : '',
        labelBackToHomeButton: '',
        labelReadTheRestButton: '',
        externalBaseUrl: '',
        twitterCardSite: '',
        userNameAdmin: '',
        cookieDomain: '',
        basePath: '',
        cacheControlSeconds: 0,
        hiddenThemeList: '',
        favIcon: '',
        additionalHeaders: '',
        errorMessageTitle: '',
        errorMessageNotFound: '',
        errorMessageForbidden: '',
        errorMessageException: '',
        summaryModeFront: true,
        newPasswordAdmin: {
          value: '',
          reset: false
        },
        disableSwagger: true,
        disableResponseCompression: false,
        disableInlineContent: false,
      };
      return prevState;
    });
  }

  getUrl = () => {
    return PEHelpers.fixUrl(`api/settings/`);
  }

  load = () => {
    PEHelpers.fetch(this.getUrl(), {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
      body: null
    })
    .then(PEHelpers.getCombinedJsonResponse, () => {
      this.pushError('A Network error prevented the settings from being fetched!');
    })
    .then(combined => {
      if (combined.response.ok) {
        combined.data.newPasswordAdmin = combined.data.newPasswordAdmin || {
          value: '',
          reset: false
        };
        combined.data.newPasswordAdmin.value = combined.data.newPasswordAdmin.value || '';
        this.setState(prevState => ({
          ...prevState,
          settings: combined.data
        }));
      } else {
        this.pushError('An HTTP error prevented the settings from being fetched!');
      }
    });
  }

  updateField = (e, fieldName, subGroup) => {
    PEHelpers.updateStateField(this, e, [ 'settings', subGroup, fieldName ]);
  }

  save = (e) => {
    PEHelpers.fetch(this.getUrl(), {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(this.state.settings),
    })
    .then(PEHelpers.getCombinedJsonResponse, () => {
      this.pushError('A Network error prevented the settings from being saved!');
    })
    .then(combined => {
      if (combined.response.ok) {
        combined.data.newPasswordAdmin = combined.data.newPasswordAdmin || {
          value: '',
          reset: false
        };
        combined.data.newPasswordAdmin.value = combined.data.newPasswordAdmin.value || '';
        this.setState(prevState => ({
          ...prevState,
          settings: combined.data
        }));
        window.location.reload();
      } else {
        if (combined.data && combined.data.logMessages) {
          this.setState(prevState => ({
            ...prevState,
            errors: combined.data.logMessages
          }));
        } else {
          this.pushError('An HTTP error prevented the settings from being saved!');
        }
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
      <div id="pengine-setting-editor-content">
        {
          this.state.visible ?
            <div className="dialog-container">
              <span className="form-header-text">
                Editing Settings
              </span>
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
                    <div className="edit-label-large">Owner Name / Email:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-normal" value={this.state.settings.ownerName} onChange={(e) => this.updateField(e, 'ownerName')} />
                      <input type="text" className="edit-control-normal" value={this.state.settings.ownerEmail} onChange={(e) => this.updateField(e, 'ownerEmail')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Language Code:</div>
                    <div className="edit-field">
                    <input type="text" className="edit-control-large" value={this.state.settings.pageLanguageCode} onChange={(e) => this.updateField(e, 'pageLanguageCode')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Default Title / Theme:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-normal" value={this.state.settings.defaultTitle} onChange={(e) => this.updateField(e, 'defaultTitle')} />
                      <select className="editctl" value={this.state.settings.defaultTheme} onChange={(e) => this.updateField(e, 'defaultTheme')}>
                        {
                          this.state.peState.themeList.map((theme, key) =>
                            <option key={key} value={theme}>{ theme }</option>
                          )
                        }
                      </select>
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Default Description:</div>
                    <div className="edit-field">
                      <textarea rows="3" className="edit-control" value={this.state.settings.defaultDescription} onChange={(e) => this.updateField(e, 'defaultDescription')}></textarea>
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Front Page Logo:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-large" value={this.state.settings.logoFrontPage} onChange={(e) => this.updateField(e, 'logoFrontPage')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Website Icon:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-large" value={this.state.settings.favIcon} onChange={(e) => this.updateField(e, 'favIcon')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Admin Login Label:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-normal" value={this.state.settings.labelAdminLoginButton} onChange={(e) => this.updateField(e, 'labelAdminLoginButton')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Admin Logoff Labels</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-normal" value={this.state.settings.labelAdminLogoffButton} onChange={(e) => this.updateField(e, 'labelAdminLogoffButton')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Home Label:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-normal" value={this.state.settings.labelHomeButton} onChange={(e) => this.updateField(e, 'labelHomeButton')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Back To Home Label:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-normal" value={this.state.settings.labelBackToHomeButton} onChange={(e) => this.updateField(e, 'labelBackToHomeButton')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Read The Rest Label:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-normal" value={this.state.settings.labelReadTheRestButton} onChange={(e) => this.updateField(e, 'labelReadTheRestButton')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Theme Label:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-normal" value={this.state.settings.labelThemeButton} onChange={(e) => this.updateField(e, 'labelThemeButton')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Quote Label:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-normal" value={this.state.settings.labelQuoteButton} onChange={(e) => this.updateField(e, 'labelQuoteButton')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Resume Label:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-normal" value={this.state.settings.labelResumeButton} onChange={(e) => this.updateField(e, 'labelResumeButton')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Archived Posts Label:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-normal" value={this.state.settings.labelArchivedPostsButton} onChange={(e) => this.updateField(e, 'labelArchivedPostsButton')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Arc. Posts Per Page:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-normal" value={this.state.settings.perPagePostArchived} onChange={(e) => this.updateField(e, 'perPagePostArchived')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Front Page Posts:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-normal" value={this.state.settings.perPagePostFront} onChange={(e) => this.updateField(e, 'perPagePostFront')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">RSS Post Count</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-normal" value={this.state.settings.perPageRSS} onChange={(e) => this.updateField(e, 'perPageRSS')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Search Results Per Page</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-normal" value={this.state.settings.perPageSearchResults} onChange={(e) => this.updateField(e, 'perPageSearchResults')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Admin Session Timeout</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-normal" value={this.state.settings.timeLimitAdminToken} onChange={(e) => this.updateField(e, 'timeLimitAdminToken')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Enable Optional Features:</div>
                    <div className="edit-field edit-checkbox-list">
                      <span>
                        <input type="checkbox" checked={this.state.settings.summaryModeFront} onChange={(e) => this.updateField(e, 'summaryModeFront')} />
                        Front Page Summary Mode
                      </span>
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Disable Default Features</div>
                    <div className="edit-field edit-checkbox-list">
                      <span>
                        <input type="checkbox" checked={this.state.settings.disableQuotes} onChange={(e) => this.updateField(e, 'disableQuotes')} />
                        Quote
                      </span>
                      <span>
                        <input type="checkbox" checked={this.state.settings.disableRSS} onChange={(e) => this.updateField(e, 'disableRSS')} />
                        Rss
                      </span>
                      <span>
                        <input type="checkbox" checked={this.state.settings.disableResume} onChange={(e) => this.updateField(e, 'disableResume')} />
                        Resume
                      </span>
                      <span>
                        <input type="checkbox" checked={this.state.settings.disableThemeSelection} onChange={(e) => this.updateField(e, 'disableThemeSelection')} />
                        Theme
                      </span>
                      <span>
                        <input type="checkbox" checked={this.state.settings.disableSearch} onChange={(e) => this.updateField(e, 'disableSearch')} />
                        Search
                      </span>
                      <span>
                        <input type="checkbox" checked={this.state.settings.disableSwagger} onChange={(e) => this.updateField(e, 'disableSwagger')} />
                        Swagger
                      </span>
                      <span>
                        <input type="checkbox" checked={this.state.settings.disableResponseCompression} onChange={(e) => this.updateField(e, 'disableResponseCompression')} />
                        Response Compression
                      </span>
                      <span>
                        <input type="checkbox" checked={this.state.settings.disableInlineContent} onChange={(e) => this.updateField(e, 'disableInlineContent')} />
                        Inline Content
                      </span>
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">External Base URL:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-large" value={this.state.settings.externalBaseUrl} onChange={(e) => this.updateField(e, 'externalBaseUrl')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Twitter Card Site:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-large" value={this.state.settings.twitterCardSite} onChange={(e) => this.updateField(e, 'twitterCardSite')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Cookie Domain:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-large" value={this.state.settings.cookieDomain} onChange={(e) => this.updateField(e, 'cookieDomain')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Cookie Path:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-large" value={this.state.settings.basePath} onChange={(e) => this.updateField(e, 'basePath')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Content Cache Seconds</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-normal" value={this.state.settings.cacheControlSeconds} onChange={(e) => this.updateField(e, 'cacheControlSeconds')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Hidden Theme List (comma delimited)</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-normal" value={this.state.settings.hiddenThemeList} onChange={(e) => this.updateField(e, 'hiddenThemeList')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Error Message Title</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-normal" value={this.state.settings.errorMessageTitle} onChange={(e) => this.updateField(e, 'errorMessageTitle')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Error Messsage - Not Found</div>
                    <div className="edit-field">
                      <textarea rows="5" className="edit-control" value={this.state.settings.errorMessageNotFound} onChange={(e) => this.updateField(e, 'errorMessageNotFound')}></textarea>
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Error Messsage - Forbidden</div>
                    <div className="edit-field">
                      <textarea rows="5" className="edit-control" value={this.state.settings.errorMessageForbidden} onChange={(e) => this.updateField(e, 'errorMessageForbidden')}></textarea>
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Error Messsage - Exception (Generic)</div>
                    <div className="edit-field">
                      <textarea rows="5" className="edit-control" value={this.state.settings.errorMessageException} onChange={(e) => this.updateField(e, 'errorMessageException')}></textarea>
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Additional Headers:</div>
                    <div className="edit-field">
                      <textarea rows="5" className="edit-control" value={this.state.settings.additionalHeaders} onChange={(e) => this.updateField(e, 'additionalHeaders')}></textarea>
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Admin UserName:</div>
                    <div className="edit-field">
                      <input type="text" className="edit-control-large" value={this.state.settings.userNameAdmin} onChange={(e) => this.updateField(e, 'userNameAdmin')} />
                    </div>
                  </div>
                  <div className="edit-row">
                    <div className="edit-label-large">Admin Pass</div>
                    <div className="edit-field edit-checkbox-list">
                      <span>
                        <input type="password" className="edit-control-normal" value={this.state.settings.newPasswordAdmin.value} onChange={(e) => this.updateField(e, 'value', 'newPasswordAdmin')} />
                      </span>
                      <span>
                        <input type="checkbox" checked={this.state.settings.newPasswordAdmin.reset} onChange={(e) => this.updateField(e, 'reset', 'newPasswordAdmin')} />
                        Blank
                      </span>
                    </div>
                  </div>
                </div>
              </div>
              <div className="panel">
                <div className="panel-right">
                  <button type="button" onClick={(e) => this.save(e)}>Save</button>
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

export default PEngineSettingEditor;