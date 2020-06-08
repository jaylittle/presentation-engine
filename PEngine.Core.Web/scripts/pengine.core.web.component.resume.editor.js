import React from 'react';
import PEHelpers from "./pengine.core.web.helpers";

class PEngineResumeEditor extends React.Component {

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
        PEHelpers.updateEditorLocationHash(type, guid);
        document.body.style.overflow = 'hidden';
        window.scrollTo(0, 0);

        this.reset();
        this.load();
        
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
    return PEHelpers.fixUrl(`api/resume/`);
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
      this.pushError('A Network error prevented the resume from being fetched!');
    })
    .then(combined => {
      if (combined.response.ok) {
        for (let educationIndex in combined.data.educations) {
          combined.data.educations[educationIndex].started = combined.data.educations[educationIndex].started || '';
          combined.data.educations[educationIndex].completed = combined.data.educations[educationIndex].completed || '';
        }
        for (let workHistoryIndex in combined.data.workHistories) {
          combined.data.workHistories[workHistoryIndex].started = combined.data.workHistories[workHistoryIndex].started || '';
          combined.data.workHistories[workHistoryIndex].completed = combined.data.workHistories[workHistoryIndex].completed || '';
        }
        this.setState(prevState => ({
          ...prevState,
          resume: combined.data
        }));
        //Add default personal and objective records if payload doesn't contain them
        if (!combined.data.personals.length) {
          this.addRecord(null, 'personals');
        }
        if (!combined.data.objectives.length) {
          this.addRecord(null, 'objectives');
        }
      } else {
        this.pushError('An HTTP error prevented the resume from being fetched!');
      }
    });
  }

  updateResumeRecordField = (e, subGroup, subGroupKey, fieldName, subSubGroup, subSubGroupKey) => {
    PEHelpers.updateStateField(this, e, [ 'resume', subGroup, subGroupKey, subSubGroup, subSubGroupKey, fieldName ]);
    if (subGroup === 'skillTypes' && fieldName === 'type') {
      for (let currentSkillIndex in this.state.resume[subGroup][subGroupKey].skills) {
        PEHelpers.updateStateField(this, e, [ 'resume', subGroup, subGroupKey, 'skills', currentSkillIndex, 'type' ]);
      }
    }
  }

  save = (e) => {
    PEHelpers.fetch(this.getUrl(), {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(this.state.resume),
    })
    .then(PEHelpers.getCombinedJsonResponse, () => {
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
    PEHelpers.updateEditorLocationHash();

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
            type: this.state.resume[subGroup][subGroupKey].type,
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
          started: '',
          completed: '',
        };
        break;
      case 'workHistories':
        newObject = {
          legacyID: null,
          employer: '',
          employerURL: '',
          jobTitle: '',
          jobDescription: '',
          started: '',
          completed: '',
        };
        break;
    }
    newObject.expanded = true;
    this.setState((prevState) => {
      if (subGroup && subSubGroup) {
        prevState.resume[subGroup][subGroupKey][subSubGroup].push(newObject);
      } else if (subGroup) {
        prevState.resume[subGroup].push(newObject);
      }
      return prevState;
    });
  }

  deleteRecordConfirm = (e, type, title, subGroup, subGroupKey, subSubGroup, subSubGroupKey) => {
    let confirmMessage = `Are you sure you want to delete this ${type}?`;
    if (title) {
      confirmMessage = `Are you sure you want to delete this ${type} entitled "${title}"?`;
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
        prevState.resume[subGroup][subGroupKey][subSubGroup].splice(subSubGroupKey, 1);
        prevState.resume[subGroup][subGroupKey][subSubGroup].splice(newSubSubGroupKey, 0, subSubObject);
        if (subGroup === 'skillTypes' && subSubGroup === 'skills') {
          for (let currentSubSubGroupKey in prevState.resume[subGroup][subGroupKey][subSubGroup]) {
            prevState.resume[subGroup][subGroupKey][subSubGroup][currentSubSubGroupKey].order = currentSubSubGroupKey;
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
      prevState.resume[subGroup][subGroupKey].expanded = !prevState.resume[subGroup][subGroupKey].expanded;
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
                <div className="form-container">
                  <div className="edit-row">
                    <div className="edit-label">Resume Functions:</div>
                    <div className="edit-field">
                      <button type="button" id="skill_type_edit_button_add" onClick={(e) => this.addRecord(e, 'skillTypes')}>Add Skill Type</button>
                      <button type="button" id="education_edit_button_add" onClick={(e) => this.addRecord(e, 'educations')}>Add Education</button>
                      <button type="button" id="workhistory_edit_button_add" onClick={(e) => this.addRecord(e, 'workHistories')}>Add Work History</button>
                    </div>
                  </div>
                </div>
                {
                  this.state.resume.personals.map((personal, key) =>
                    <div className="form-container border-top" key={key}>
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
                    <div className="form-container border-top" key={key}>
                      <div className="edit-row">
                        <div className="edit-label">Objective Statement:</div>
                        <div className="edit-field">
                          <textarea className="edit-control" rows="10" value={objective.data} onChange={(e) => this.updateResumeRecordField(e, 'objectives', key, 'data')}></textarea>
                        </div>
                      </div>
                    </div>
                  )
                }
                {
                  this.state.resume.skillTypes.map((skillType, skillTypeKey) =>
                  <div className="form-container border-top" key={skillTypeKey}>
                    <div className="edit-row">
                      <div className="edit-label">Skill Type:</div>
                      <div className="edit-field">
                        <input type="text" className="edit-control-large" value={skillType.type} onChange={(e) => this.updateResumeRecordField(e, 'skillTypes', skillTypeKey, 'type')} />
                        <button type="button" onClick={(e) => this.deleteRecordConfirm(e, 'Skill Type', skillType.type, 'skillTypes', skillTypeKey) }>Delete Skill Type</button>
                        <button type="button" onClick={(e) => this.addRecord(e, 'skillTypes', skillTypeKey, 'skills') }>Add Skill</button>
                        {
                          skillType.expanded ?
                          <button type="button" onClick={(e) => this.toggleRecord(e, 'skillTypes', skillTypeKey)}>Hide</button>
                          :
                          <button type="button" onClick={(e) => this.toggleRecord(e, 'skillTypes', skillTypeKey)}>Show</button>
                        }
                      </div>
                    </div>
                    {
                      skillType.expanded ? (
                        skillType.skills.map((skill, skillKey) =>
                        <div className="edit-row" key={skillKey}>
                          <div className="edit-label">Skill {skillKey + 1}:</div>
                          <div className="edit-field">
                            <input type="text" className="edit-control-large" value={skill.name} onChange={(e) => this.updateResumeRecordField(e, 'skillTypes', skillTypeKey, 'name', 'skills', skillKey)} />
                            <button type="button" onClick={(e) => this.deleteRecordConfirm(e, 'Skill', skill.type + ' - ' + skill.name, 'skillTypes', skillTypeKey, 'skills', skillKey) }>Delete Skill</button>
                            <button type="button" onClick={(e) => this.moveRecord(e, 'skillTypes', skillTypeKey, 'skills', skillKey, -1)} disabled={skillKey <= 0}>Move Up</button>
                            <button type="button" onClick={(e) => this.moveRecord(e, 'skillTypes', skillTypeKey, 'skills', skillKey, 1)} disabled={skillKey >= skillType.skills.length - 1}>Move Down</button>
                          </div>
                        </div>
                        )
                      ) : null
                    }
                  </div>
                  )
                }
                {
                  this.state.resume.educations.map((education, key) =>
                    <div className="form-container border-top" key={key}>
                      <div className="edit-row">
                        <div className="edit-label">Education Institute:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-large" value={education.institute} onChange={(e) => this.updateResumeRecordField(e, 'educations', key, 'institute')} />
                          <button type="button" onClick={(e) => this.deleteRecordConfirm(e, 'Education', education.institute + ' - ' + education.program, 'educations', key) }>Delete Education</button>
                          {
                            education.expanded ?
                            <button type="button" onClick={(e) => this.toggleRecord(e, 'educations', key)}>Hide</button>
                            :
                            <button type="button" onClick={(e) => this.toggleRecord(e, 'educations', key)}>Show</button>
                          }
                        </div>
                      </div>
                      {
                        education.expanded ?
                          <div className="edit-row">
                            <div className="edit-label">Institute URL:</div>
                            <div className="edit-field">
                              <input type="text" className="edit-control-large" value={education.instituteURL} onChange={(e) => this.updateResumeRecordField(e, 'educations', key, 'instituteURL')} />
                            </div>
                          </div>
                          : null
                        }
                        {
                          education.expanded ?
                          <div className="edit-row">
                            <div className="edit-label">Program:</div>
                            <div className="edit-field">
                              <input type="text" className="edit-control-large" value={education.program} onChange={(e) => this.updateResumeRecordField(e, 'educations', key, 'program')} />
                            </div>
                          </div>
                          : null
                        }
                        {
                          education.expanded ?
                          <div className="edit-row">
                            <div className="edit-label">Started:</div>
                            <div className="edit-field">
                              <input type="date" className="edit-control-normal datepicker" value={education.started} onChange={(e) => this.updateResumeRecordField(e, 'educations', key, 'started')} />
                            </div>
                          </div>
                          : null
                        }
                        {
                          education.expanded ?
                          <div className="edit-row">
                            <div className="edit-label">Completed:</div>
                            <div className="edit-field">
                              <input type="date" className="edit-control-normal datepicker" value={education.completed} onChange={(e) => this.updateResumeRecordField(e, 'educations', key, 'completed')} />
                            </div>
                          </div>
                          : null
                        }
                    </div>
                  )
                }
                {
                  this.state.resume.workHistories.map((workHistory, key) =>
                    <div className="form-container border-top" key={key}>
                      <div className="edit-row">
                        <div className="edit-label">Work History Employer:</div>
                        <div className="edit-field">
                          <input type="text" className="edit-control-large" value={workHistory.employer} onChange={(e) => this.updateResumeRecordField(e, 'workHistories', key, 'employer')} />
                          <button type="button" onClick={(e) => this.deleteRecordConfirm(e, 'Work History', workHistory.employer + ' - ' + workHistory.jobTitle, 'workHistories', key) }>Delete Work History</button>
                          {
                            workHistory.expanded ?
                            <button type="button" onClick={(e) => this.toggleRecord(e, 'workHistories', key)}>Hide</button>
                            :
                            <button type="button" onClick={(e) => this.toggleRecord(e, 'workHistories', key)}>Show</button>
                          }
                        </div>
                      </div>
                      {
                        workHistory.expanded ?
                        <div className="edit-row">
                          <div className="edit-label">Employer URL:</div>
                          <div className="edit-field">
                            <input type="text" className="edit-control-large" value={workHistory.employerURL} onChange={(e) => this.updateResumeRecordField(e, 'workHistories', key, 'employerURL')} />
                          </div>
                        </div>
                        : null
                      }
                      {
                        workHistory.expanded ?
                        <div className="edit-row">
                          <div className="edit-label">Job Title:</div>
                          <div className="edit-field">
                            <input type="text" className="edit-control-large" value={workHistory.jobTitle} onChange={(e) => this.updateResumeRecordField(e, 'workHistories', key, 'jobTitle')} />
                          </div>
                        </div>
                        : null
                      }
                      {
                        workHistory.expanded ?
                        <div className="edit-row">
                          <div className="edit-label">Started:</div>
                          <div className="edit-field">
                            <input type="date" className="edit-control-normal datepicker" value={workHistory.started} onChange={(e) => this.updateResumeRecordField(e, 'workHistories', key, 'started')} />
                          </div>
                        </div>
                        : null
                      }
                      {
                        workHistory.expanded ?
                        <div className="edit-row">
                          <div className="edit-label">Completed:</div>
                          <div className="edit-field">
                            <input type="date" className="edit-control-normal datepicker" value={workHistory.completed} onChange={(e) => this.updateResumeRecordField(e, 'workHistories', key, 'completed')} />
                          </div>
                        </div>
                        : null
                      }
                      {
                        workHistory.expanded ?
                        <div className="edit-row">
                          <div className="edit-label">Job Description:</div>
                          <div className="edit-field">
                            <textarea className="edit-control" rows="10" value={workHistory.jobDescription} onChange={(e) => this.updateResumeRecordField(e, 'workHistories', key, 'jobDescription')}></textarea>

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

export default PEngineResumeEditor;