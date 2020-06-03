import React from 'react';
import pengineHelpers from "./pengine.core.web.helpers";

class PEngineArticleEditor extends React.Component {

  constructor(props) {
    super(props);

    this.state = {
      resume: {
        personals: [ ],
        objectives: [ ],
        skillTypes: [ ],
        educations: [ ],
        workHistories: [ ]
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
      if (elements[1] === 'resume') {
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
          this.load();
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
      prevState.resume = {
        personals: [ ],
        objectives: [ ],
        skillTypes: [ ],
        educations: [ ],
        workHistories: [ ]
      };
      return prevState;
    });
  }

  getUrl = () => {
    return pengineHelpers.fixUrl(`api/resume/`);
  }

  load = () => {
    pengineHelpers.fetch(this.getUrl(), {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
      body: null
    })
    .then(pengineHelpers.getCombinedJsonResponse, () => {
      this.pushError('A Network error prevented the resume from being fetched!');
    })
    .then(combined => {
      if (combined.response.ok) {
        this.setState(prevState => ({
          ...prevState,
          resume: combined.data
        }));
      } else {
        this.pushError('An HTTP error prevented the resume from being fetched!');
      }
    });
  }

  updateResumeRecordField = (e, subGroup, subGroupKey, fieldName, subSubGroup, subSubGroupKey) => {
    let fieldValue = e.target.value;
    if (e.target.type === 'checkbox' || e.target.type === 'radio') {
      fieldValue = e.target.checked;
    }
    this.setState(prevState => {
      if (subGroup && subSubGroup) {
        prevState[subGroup][subGroupKey][subSubGroup][subSubGroupKey][fieldName] = fieldValue;
      } else if (subGroup) {
        prevState[subGroup][subGroupKey][fieldName] = fieldValue;
        if (subGroup === 'skillTypes' && fieldName === 'type') {
         for (let currentSkillIndex in prevState[subGroup][subGroupKey].skills) {
          prevState[subGroup][subGroupKey].skills[currentSkillIndex].type = fieldValue;
         }
        }
      }
      return prevState;
    });
  }

  save = (e) => {
    pengineHelpers.fetch(this.getUrl(), {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(this.state.resume),
    })
    .then(pengineHelpers.getCombinedJsonResponse, () => {
      this.pushError('A Network error prevented the resume from being saved!');
    })
    .then(combined => {
      if (combined.response.ok) {
        this.setState(prevState => ({
          ...prevState,
          resume: combined.data
        }));
        window.location.reload();
      } else {
        if (combined.data && combined.data.logMessages) {
          this.setState(prevState => ({
            ...prevState,
            errors: combined.data.logMessages
          }));
        } else {
          this.pushError('An HTTP error prevented the resume from being saved!');
        }
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

  addRecord = (e, subGroup, subGroupKey, subSubGroup) => {
    let newObject = {};
    switch (subGroup) {
      case 'personals':
        newObject = {
          legacyID: null,
          fullName: '',
          address1: '',
          address2: '',
          city: '',
          state: '',
          zip: '',
          phone: '',
          fax: '',
          email: '',
          websiteURL: '',
        };
        break;
      case 'objectives':
        newObject = {
          legacyID: null,
          data: '',
        };
        break;
      case 'skillTypes':
        if (!subSubGroup) {
          newObject = {
            type: '',
            skills: [ ]
          };
        } else {
          newObject = {
            legacyID: null,
            type: '',
            name: '',
            hint: '',
            order: -1,
          };
        }
        break;
      case 'educations':
        newObject = {
          legacyID: null,
          institute: '',
          instituteURL: '',
          program: '',
          started: null,
          completed: null,
        };
        break;
      case 'workHistories':
        newObject = {
          legacyID: null,
          employer: '',
          employerURL: '',
          jobTitle: '',
          jobDescription: '',
          started: null,
          completed: null,
        };
        break;
    }
    this.setState((prevState) => {
      if (subGroup && subSubGroup) {
        prevState[subGroup][subGroupKey][subSubGroup] = prevState[subGroup][subGroupKey][subSubGroup] || [ ];
        prevState[subGroup][subGroupKey][subSubGroup].push(newobject);
      } else if (subGroup) {
        prevState[subGroup].push(newObject);
      }
      return prevState;
    });
  }

  deleteRecordConfirm = (e, name, subGroup, subGroupKey, subSubGroup, subSubGroupKey) => {
    let confirmMessage = `Are you sure you want to delete this ${name}?`;
    if (subGroup && subSubGroup) {
      if (this.state.resume[subGroup][subGroupkey] && this.state.resume[subGroup][subGroupKey][subSubGroup]
          && this.state.resume[subGroup][subGroupKey][subSubGroup][subSubGroupKey]
          && this.state.resume[subGroup][subGroupKey][subSubGroup][subSubGroupKey].name) {
        confirmMessage = `Are you sure you want to delete this ${name} entitled "${this.state.resume[subGroup][subGroupKey][subSubGroup][subSubGroupKey].name}"?`;
      }
    } else if (subGroup) {
      if (this.state.resume[subGroup][subGroupkey] && this.state.resume[subGroup][subGroupKey].name) {
        confirmMessage = `Are you sure you want to delete this ${name} entitled "${this.state.resume[subGroup][subGroupKey].name}"?`;
    }
    }
    if (confirm(confirmMessage)) {
      return this.deleteRecord(e, subGroup, subGroupKey, subSubGroup, subSubGroupKey);
    }
  }

  deleteRecord = (e, subGroup, subGroupKey, subSubGroup, subSubGroupKey) => {
    this.setState(prevState => {
      if (subGroup && subSubGroup) {
        prevState.resume[subGroup][subGroupKey][subSubGroup].splice(subSubGroupKey, 1);
      } else if (subGroup) {
        prevState.resume[subGroup].splice(subGroupKey, 1);
      }
      return prevState;
    });
  }

  moveRecord = (e, subGroup, subGroupKey, subSubGroup, subSubGroupKey, offset) => {
    this.setState(prevState => {
      if (subGroup && subSubGroup) {
        let subSubObject = prevState.resume[subGroup][subGroupKey][subSubGroup][subSubGroupKey];
        let newSubSubGroupKey = subSubGroupKey + offset;
        prevState.resume[subGroup][subGroupkey][subSubGroup].splice(subSubGroupKey, 1);
        prevState.resume[subGroup][subGroupkey][subSubGroup].splice(newSubSubGroupKey, 0, subSubObject);
        if (subGroup === 'skillTypes' && subSubGroup === 'skills') {
          for (let currentSubSubGroupKey in prevState.resume[subGroup][subGroupkey][subSubGroup]) {
            prevState.resume[subGroup][subGroupkey][subSubGroup][currentSubSubGroupKey].order = currentSubSubGroupKey;
          }
        }
      } else if (subGroup) {
        let subObject = prevState.resume[subGroup][subGroupKey];
        let newSubGroupKey = subGroupKey + offset;
        prevState.resume[subGroup].splice(subGroupKey, 1);
        prevState.resume[subGroup].splice(newSubGroupKey, 0, subObject);
      }
      return prevState;
    });
  }

  toggleRecord = (e, subGroup, subGroupKey) => {
    this.setState(prevState => {
      if (sectionIndex >= 0) {
        prevState.resume[subGroup][subGroupKey].expanded = !prevState.resume[subGroup][subGroupKey].expanded;
      }
      return prevState;
    });
  }

  render = () => {
    return (
      <div id="pengine-resume-editor-content">
        {
          this.state.visible ?
            <div className="dialog-container">
              <span className="form-header-text">
                Editing Resume
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
                {
                  this.state.resume.personals.map((personal, key) =>
                    <div className="form-container" key={key}>
                      <div className="edit-row">
                        <div className="edit-label">Full Name:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-large" value={personal.fullName} onChange={(e) => this.updateResumeRecordField(e, 'personals', key, 'fullName')} />
                        </div>
                      </div>
                      <div className="edit-row">
                        <div className="edit-label">Email:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-large" value={personal.email} onChange={(e) => this.updateResumeRecordField(e, 'personals', key, 'email')} />
                        </div>
                      </div>
                      <div className="edit-row">
                        <div className="edit-label">Web:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-large" value={personal.websiteURL} onChange={(e) => this.updateResumeRecordField(e, 'personals', key, 'websiteURL')} />
                        </div>
                      </div>
                      <div className="edit-row">
                        <div className="edit-label">Phone:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-large" value={personal.phone} onChange={(e) => this.updateResumeRecordField(e, 'personals', key, 'phone')} />
                        </div>
                      </div>
                      <div className="edit-row">
                        <div className="edit-label">Fax:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-large" value={personal.fax} onChange={(e) => this.updateResumeRecordField(e, 'personals', key, 'fax')} />
                        </div>
                      </div>
                      <div className="edit-row">
                        <div className="edit-label">Address:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-large" value={personal.address1} onChange={(e) => this.updateResumeRecordField(e, 'personals', key, 'address1')} />
                        </div>
                      </div>
                      <div className="edit-row">
                        <div className="edit-label">Address 2:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-large" value={personal.address2} onChange={(e) => this.updateResumeRecordField(e, 'personals', key, 'address2')} />
                        </div>
                      </div>
                      <div className="edit-row">
                        <div className="edit-label">City:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-large" value={personal.city} onChange={(e) => this.updateResumeRecordField(e, 'personals', key, 'city')} />
                        </div>
                      </div>
                      <div className="edit-row">
                        <div className="edit-label">State:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-large" value={personal.state} onChange={(e) => this.updateResumeRecordField(e, 'personals', key, 'state')} />
                        </div>
                      </div>
                      <div className="edit-row">
                        <div className="edit-label">Zip:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-large" value={personal.zip} onChange={(e) => this.updateResumeRecordField(e, 'personals', key, 'zip')} />
                        </div>
                      </div>
                    </div>
                  )
                }
                {
                  this.state.resume.objectives.map((objective, key) =>
                    <div className="form-container" key={key}>
                      <div className="edit-row">
                        <div className="edit-label">Content:</div>
                        <div className="edit-field">
                          <textarea className="edit-control" rows="10" value={objective.data} onChange={(e) => this.updateResumeRecordField(e, 'objectives', key, 'data')}></textarea>
                        </div>
                      </div>
                    </div>
                  )
                }
                {
                  this.state.resume.skillTypes.map((skillType, skillTypeKey) =>
                  <div className="form-container" key={skillTypeKey}>
                    <div className="edit-row">
                      <div className="edit-label">Type:</div>
                      <div className="edit-field">
                        <input type="text" className="edit-control-large" value={skillType.type} onChange={(e) => this.updateResumeRecordField(e, 'skillTypes', skillTypeKey, 'type')} />
                        <button type="button" onClick={(e) => this.addRecord(e, 'skillTypes', skillTypeKey, 'skills') }>Add Skill</button>
                      </div>
                    </div>
                    {
                      skillType.skills.map((skill, skillKey) =>
                      <div className="edit-row" key={skillKey}>
                        <div className="edit-label">Skill {index + 1}:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-large" value={skill.name} onChange={(e) => this.updateResumeRecordField(e, 'skillTypes', skillTypeKey, 'name', 'skills', skillKey)} />
                          <button type="button" onClick={(e) => this.deleteRecordConfirm(e, skill.name, 'skillTypes', skillTypeKey, 'skills', skillKey) }>Delete Skill</button>
                          <button type="button" onClick={(e) => this.moveRecord(e, 'skillTypes', skillTypeKey, 'skills', skillKey, -1)} disabled={skillKey <= 0}>Move Up</button>
                          <button type="button" onClick={(e) => this.moveRecord(e, 'skillTypes', skillTypeKey, 'skills', skillKey, 1)} disabled={skillKey >= skillType.skills.length - 1}>Move Down</button>
                        </div>
                      </div>
                      )
                    }
                  </div>
                  )
                }
                {
                  this.state.resume.educations.map((education, key) =>
                    <div className="form-container" key={key}>
                      <div className="edit-row">
                        <div className="edit-label">Institute:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-large" value={education.institute} onChange={(e) => this.updateResumeRecordField(e, 'educations', key, 'institute')} />
                        </div>
                      </div>
                      <div className="edit-row">
                        <div className="edit-label">Institute URL:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-large" value={education.instituteUrl} onChange={(e) => this.updateResumeRecordField(e, 'educations', key, 'instituteUrl')} />
                        </div>
                      </div>
                      <div className="edit-row">
                        <div className="edit-label">Program:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-large" value={education.program} onChange={(e) => this.updateResumeRecordField(e, 'educations', key, 'program')} />
                        </div>
                      </div>
                      <div className="edit-row">
                        <div className="edit-label">Started:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-normal datepicker" value={education.started} onChange={(e) => this.updateResumeRecordField(e, 'educations', key, 'started')} />
                        </div>
                      </div>
                      <div className="edit-row">
                        <div className="edit-label">Completed:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-normal datepicker" value={education.completed} onChange={(e) => this.updateResumeRecordField(e, 'educations', key, 'completed')} />
                        </div>
                      </div>
                    </div>
                  )
                }
                {
                  this.state.resume.workHistories.map((workHistory, key) =>
                    <div className="form-container" key={key}>
                      <div className="edit-row">
                        <div className="edit-label">Employer:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-large" value={workHistory.employer} onChange={(e) => this.updateResumeRecordField(e, 'workHistories', key, 'employer')} />
                        </div>
                      </div>
                      <div className="edit-row">
                        <div className="edit-label">Employer URL:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-large" value={workHistory.employerUrl} onChange={(e) => this.updateResumeRecordField(e, 'workHistories', key, 'employerUrl')} />
                        </div>
                      </div>
                      <div className="edit-row">
                        <div className="edit-label">Job Title:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-large" value={workHistory.jobTitle} onChange={(e) => this.updateResumeRecordField(e, 'workHistories', key, 'jobTitle')} />
                        </div>
                      </div>
                      <div className="edit-row">
                        <div className="edit-label">Started:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-normal datepicker" value={workHistory.started} onChange={(e) => this.updateResumeRecordField(e, 'workHistories', key, 'started')} />
                        </div>
                      </div>
                      <div className="edit-row">
                        <div className="edit-label">Completed:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-normal datepicker" value={workHistory.completed} onChange={(e) => this.updateResumeRecordField(e, 'workHistories', key, 'completed')} />
                        </div>
                      </div>
                      <div className="edit-row">
                        <div className="edit-label">Job Description:</div>
                        <div className="edit-field">
                          <textarea className="edit-control" rows="10" value={workHistory.jobDescription} onChange={(e) => this.updateResumeRecordField(e, 'workHistories', key, 'jobDescription')}></textarea>

                        </div>
                      </div>
                    </div>
                  )
                }
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

export default PEngineArticleEditor;