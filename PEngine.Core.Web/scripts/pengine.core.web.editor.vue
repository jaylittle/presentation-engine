<template>
  <div id="pengine-editor">
    <div class="dialog-container" v-if="record.type && record.type != 'none'">
      <span class="form-header-text">
        {{ record.title }}
      </span>
      <span class="form-subheader-text" v-if="record.data.createdUTC || record.data.modifiedUTC">
          Created: <span class="datetime-display">{{record.data.createdUTC}}</span>
          &nbsp; | &nbsp;
          Modified: <span class="datetime-display">{{record.data.modifiedUTC}}</span>
      </span>
      <ul class="form-errors" v-if="record.errors">
        <li v-for="error in record.errors">{{ error.text }}</li>
      </ul>
      <div v-if="record.type && record.type == 'post'">
        <div class="form-container">
          <div class="edit-row">
            <div class="edit-label">Title:</div>
            <div class="edit-field"><input type="text" class="edit-control-large" v-model="record.data.name"></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Visible:</div>
            <input type="checkbox" v-model="record.data.visibleFlag">
          </div>
          <div class="edit-row">
            <div class="edit-label">Icon:</div>
            <div class="edit-field">
              <select class="edit-control-normal" v-model="record.data.iconFileName">
                <option value="" selected=""></option>
                <option v-for="icon in state.iconList" v-bind:value="icon">
                  {{ icon }}
                </option>
              </select>
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Content:</div>
            <div class="edit-field"><textarea rows="20" class="edit-control" v-model="record.data.data"></textarea></div>
          </div>
        </div>
      </div>
      <div v-if="record.type && record.type == 'article'">
        <div class="form-container">
          <div class="edit-row">
            <div class="edit-label">Editing:</div>
            <div class="edit-field">
              <select class="edit-control-large" v-model="record.state.editTarget">
                <option value="">[Article]</option>
                <option v-for="(section, index) in record.data.sections" v-bind:value="'sections:' + index">[Section] {{section.name}}</option>
              </select>
              <button type="button" id="section_edit_button_add" v-on:click="addTarget('sections')" v-if="currentEditTargetProperty === ''">Add Section</button>
              <button type="button" id="section_edit_button_delete" v-on:click="removeCurrentTarget()" v-if="currentEditTargetProperty === 'sections'">Delete Current Section</button>
            </div>
          </div>
        </div>
        <div v-if="currentEditTargetProperty === ''" class="form-container">
          <input type="hidden" id="article_edit_guid" name="Guid" :value="guid" />
          <div class="edit-row">
            <div class="edit-label">Title:</div>
            <div class="edit-field"><input type="text" class="edit-control-large" v-model="record.data.name"/></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Description:</div>
            <div class="edit-field"><textarea class="edit-control" rows="3" v-model="record.data.description"></textarea></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Content URL:</div>
            <div class="edit-field"><input type="text" class="edit-control-large" v-model="record.data.contentURL" /></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Category:</div>
            <div class="edit-field"><input type="text" class="edit-control-large" v-model="record.data.category" /></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Default Section:</div>
            <div class="edit-field">
              <select class="edit-control-large" v-model="record.data.defaultSection">
                <option value="">[Not Specified]</option>
                <option v-for="(section, index) in record.data.sections" v-bind:value="section.name">{{section.name}}</option>
              </select>
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Visible:</div>
            <input type="checkbox" v-model="record.data.visibleFlag" />
          </div>
          <div class="edit-row">
            <div class="edit-label">Hide Buttons:</div>
            <input type="checkbox" v-model="record.data.hideButtonsFlag" />
          </div>
          <div class="edit-row">
            <div class="edit-label">Hide Dropdown:</div>
            <input type="checkbox" v-model="record.data.hideDropDownFlag" />
          </div>
        </div>
        <div v-if="currentEditTargetProperty === 'sections'" class="form-container">
          <div class="edit-row">
            <div class="edit-label">Name:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-large" v-model="currentEditTarget.name"  />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Order:</div>
            <div class="edit-field">
              <button type="button" v-if="currentEditTargetPosition !== 'First'" v-on:click="moveCurrentTarget(-1)">Up</button>
              <button type="button" v-if="currentEditTargetPosition !== 'Last'" v-on:click="moveCurrentTarget(1)">Down</button>
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Content:</div>
            <div class="edit-field"><textarea rows="20" class="edit-control" v-model="currentEditTarget.data"></textarea></div>
          </div>
        </div>
      </div>
      <div v-if="record.type && record.type == 'resume'">
        <div class="form-container">
          <div class="edit-row">
            <div class="edit-label">Editing:</div>
            <div class="edit-field">
              <select class="edit-control-large" v-model="record.state.editTarget">
                <option value="personals:0">[Personal]</option>
                <option value="objectives:0">[Objective]</option>
                <option v-for="(skillType, index) in record.data.skillTypes" v-bind:value="'skillTypes:' + index">[Skill Type] {{skillType.type}}</option>
                <option v-for="(education, index) in record.data.educations" v-bind:value="'educations:' + index">[Education] {{education.instutute}} - {{education.program}}</option>
                <option v-for="(workHistory, index) in record.data.workHistories" v-bind:value="'workHistories:' + index">[Work History] {{workHistory.employer}} - {{workHistory.jobTitle}}</option>
              </select>
              <button type="button" v-on:click="removeCurrentTarget()" v-if="currentEditTargetProperty === 'skills'">Delete Skill Type</button>
              <button type="button" v-on:click="removeCurrentTarget()" v-if="currentEditTargetProperty === 'educations'">Delete Education</button>
              <button type="button" v-on:click="removeCurrentTarget()" v-if="currentEditTargetProperty === 'workHistories'">Delete Work History</button>
              <button type="button" v-on:click="addTarget('skillTypes')">Add Skill Type</button>
              <button type="button" v-on:click="addTarget('educations')">Add Education</button>
              <button type="button" v-on:click="addTarget('workHistories')">Add Work History</button>
            </div>
          </div>
        </div>
        <div v-if="currentEditTargetProperty === 'personals'" class="form-container">
          <div class="edit-row">
            <div class="edit-label">Full Name:</div>
            <div class="edit-field"><input type="text" class="edit-control-large" v-model="currentEditTarget.fullName" /></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Email:</div>
            <div class="edit-field"><input type="text" class="edit-control-large" v-model="currentEditTarget.email" /></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Web:</div>
            <div class="edit-field"><input type="text" class="edit-control-large" v-model="currentEditTarget.websiteURL" /></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Phone:</div>
            <div class="edit-field"><input type="text" class="edit-control-large" v-model="currentEditTarget.phone" /></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Fax:</div>
            <div class="edit-field"><input type="text" class="edit-control-large" v-model="currentEditTarget.fax" /></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Address:</div>
            <div class="edit-field"><input type="text" class="edit-control-large" v-model="currentEditTarget.address1" /></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Address 2:</div>
            <div class="edit-field"><input type="text" class="edit-control-large" v-model="currentEditTarget.address2" /></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">City:</div>
            <div class="edit-field"><input type="text" class="edit-control-large" v-model="currentEditTarget.city" /></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">State:</div>
            <div class="edit-field"><input type="text" class="edit-control-large" v-model="currentEditTarget.state" /></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Zip:</div>
            <div class="edit-field"><input type="text" class="edit-control-large" v-model="currentEditTarget.zip" /></div>
          </div>
        </div>
        <div v-if="currentEditTargetProperty === 'objectives'" class="form-container">
          <div class="edit-row">
            <div class="edit-label">Content:</div>
            <div class="edit-field"><textarea class="edit-control" rows="10" v-model="currentEditTarget.data"></textarea></div>
          </div>
        </div>
        <div v-if="currentEditTargetProperty === 'skillTypes'" class="form-container">
          <div class="edit-row">
            <div class="edit-label">Type:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-large" v-model="currentEditTarget.type" />
              <button type="button" v-on:click="renameIndex('type', 'skills', 'type')">Rename Type</button>
              <button type="button" v-on:click="addIndexSubRecord('skills', currentEditTarget.type)">Add Skill</button>
            </div>
          </div>
          <div class="edit-row" v-for="(skill, index) in currentEditTarget.skills">
            <div class="edit-label">Skill {{index + 1}}:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-large" v-model="skill.name" />
              <button type="button" v-on:click="removeIndexSubRecord('skills', index)">Delete Skill</button>
            </div>
          </div>
        </div>
        <div v-if="currentEditTargetProperty === 'educations'" class="form-container">
          <div class="edit-row">
            <div class="edit-label">Institute:</div>
            <div class="edit-field"><input type="text" class="edit-control-large" v-model="currentEditTarget.institute" /></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Institute URL:</div>
            <div class="edit-field"><input type="text" class="edit-control-large" v-model="currentEditTarget.instituteUrl" /></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Program:</div>
            <div class="edit-field"><input type="text" class="edit-control-large" v-model="currentEditTarget.program" /></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Started:</div>
            <div class="edit-field"><input type="text" class="edit-control-normal datepicker" v-model="currentEditTarget.started" /></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Completed:</div>
            <div class="edit-field"><input type="text" class="edit-control-normal datepicker" v-model="currentEditTarget.completed" /></div>
          </div>
        </div>
        <div v-if="currentEditTargetProperty === 'workHistories'" class="form-container">
          <div class="edit-row">
            <div class="edit-label">Employer:</div>
            <div class="edit-field"><input type="text" class="edit-control-large" v-model="currentEditTarget.employer" /></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Employer URL:</div>
            <div class="edit-field"><input type="text" class="edit-control-large" v-model="currentEditTarget.employerUrl" /></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Job Title:</div>
            <div class="edit-field"><input type="text" class="edit-control-large" v-model="currentEditTarget.jobTitle" /></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Started:</div>
            <div class="edit-field"><input type="text" class="edit-control-normal datepicker" v-model="currentEditTarget.started" /></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Completed:</div>
            <div class="edit-field"><input type="text" class="edit-control-normal datepicker" v-model="currentEditTarget.completed" /></div>
          </div>
          <div class="edit-row">
            <div class="edit-label">Job Description:</div>
            <div class="edit-field"><textarea class="edit-control" rows="10" v-model="currentEditTarget.jobDescription"></textarea></div>
          </div>
        </div>
      </div>
      <div v-if="record.type && record.type == 'settings'">
        <div class="form-container">
          <div class="edit-row">
            <div class="edit-label-large">Owner Name / Email:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-large" v-model="record.data.ownerName" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Owner Email:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-large" v-model="record.data.ownerEmail" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Default Title / Theme:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-normal" v-model="record.data.defaultTitle" />
              <select class="editctl" v-model="record.data.defaultTheme">
                <option v-for="theme in state.themeList" v-bind:value="theme">{{theme}}</option>
              </select>
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Front Page Logo:</div>
            <div class="edit-field"><input type="text" class="edit-control-large" v-model="record.data.logoFrontPage" /></div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Admin Login / Logoff Labels:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-normal" v-model="record.data.labelAdminLoginButton" />
              <input type="text" class="edit-control-normal" v-model="record.data.labelAdminLogoffButton" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Home / Theme Labels:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-normal" v-model="record.data.labelHomeButton" />
              <input type="text" class="edit-control-normal" v-model="record.data.labelThemeButton" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Quote / Clippy Button Labels:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-normal" v-model="record.data.labelQuoteButton" />
              <input type="text" class="edit-control-normal" v-model="record.data.labelClippyButton" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Resume Label:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-normal" v-model="record.data.labelResumeButton" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Forum / Registration Label:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-normal" v-model="record.data.labelForumButton" />
              <input type="text" class="edit-control-normal" v-model="record.data.labelForumRegisterButton" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Forum Login / Logoff Labels:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-normal" v-model="record.data.labelForumLoginButton" />
              <input type="text" class="edit-control-normal" v-model="record.data.labelForumLogoffButton" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Clippy Shortcut KeyCode:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-normal" v-model="record.data.clippyShortcutKeyCode" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Clippy Shortcut Key Count:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-normal" v-model="record.data.clippyShortcutKeyCount" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Clippy Quotes Mode:</div>
            <div class="edit-field edit-checkbox-list">
              <span><input type="checkbox" v-model="record.data.disableClippySmartAss" /></span>
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Clippy Random Chance (0 - 100)</div>
            <div class="edit-field">
              <input type="text" class="edit-control-normal" v-model="record.data.clippyRandomChance" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Arc. Posts Per Page:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-normal" v-model="record.data.perPagePostArchived" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Front Page Posts:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-normal" v-model="record.data.perPagePostFront" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">RSS Post Count</div>
            <div class="edit-field">
              <input type="text" class="edit-control-normal" v-model="record.data.perPageRSS" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Search Results Per Page</div>
            <div class="edit-field">
              <input type="text" class="edit-control-normal" v-model="record.data.perPageSearchResults" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Forum Threads / Posts Per Page:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-normal" v-model="record.data.perPageForumThreads" />
              <input type="text" class="edit-control-normal" v-model="record.data.perPageForumPosts" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Forum Login / Edit Timeout</div>
            <div class="edit-field">
              <input type="text" class="edit-control-normal" v-model="record.data.timeLimitForumToken" />
              <input type="text" class="edit-control-normal" v-model="record.data.timeLimitForumPostEdit" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Admin Session Timeout</div>
            <div class="edit-field">
              <input type="text" class="edit-control-normal" v-model="record.data.timeLimitAdminToken" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Disabled Features</div>
            <div class="edit-field edit-checkbox-list">
              <span><input type="checkbox" v-model="record.data.disableQuotes" /> Quote</span>
              <span><input type="checkbox" v-model="record.data.disableRSS" /> Rss</span>
              <span><input type="checkbox" v-model="record.data.disableResume" /> Resume</span>
              <span><input type="checkbox" v-model="record.data.disableThemeSelection" /> Theme</span>
              <span><input type="checkbox" v-model="record.data.disableSearch" /> Search</span>
              <span><input type="checkbox" v-model="record.data.disableClippyButton" /> Clippy Button</span>
              <span><input type="checkbox" v-model="record.data.disableClippyShortcut" /> Clippy Shortcut</span>
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">External Base URL:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-large" v-model="record.data.externalBaseUrl" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Twitter Card Site:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-large" v-model="record.data.twitterCardSite" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Cookie Domain:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-large" v-model="record.data.cookieDomain" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Cookie Path:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-large" v-model="record.data.cookiePath" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Admin UserName:</div>
            <div class="edit-field">
              <input type="text" class="edit-control-large" v-model="record.data.userNameAdmin" />
            </div>
          </div>
          <div class="edit-row">
            <div class="edit-label-large">Admin Pass</div>
            <div class="edit-field edit-checkbox-list">
              <span>
                <input type="password" class="edit-control-normal" v-model="record.data.newPasswordAdmin.value" />
              </span>
              <span>
                <input type="checkbox" v-model="record.data.newPasswordAdmin.reset" />
                Blank
              </span>
            </div>
          </div>
        </div>
      </div>
      <div class="panel">
        <div class="panel-right">
          <button type="button" v-on:click="saveRecord">Save</button>
          <button type="button" v-on:click="confirmDeleteRecord" v-if="record.data && record.data.guid">Delete</button>
          <button type="button" v-on:click="cancelRecord">Cancel</button>
        </div>
      </div>
    </div>
    <div id="modal-overlay" v-if="record.type && record.type != 'none'"></div>
  </div>
</template>

<script>
  import pengineHelpers from "./pengine.core.web.helpers";

  let editorHelpers = {
      updateLocationHash(type, guid) {
        let myHash = '';
        if (type) {
          myHash = `#edit/${type}`;
          if (guid) {
            myHash += `/${guid}`;
          }
        }
        if (myHash != '' || !history) {
          window.location.hash = myHash;
        }
        else {
          history.pushState("", document.title, window.location.pathname + window.location.search);
        }
      },
      getRecordDefaultData() {
        return {
          type: 'none',
          guid: null,
          data: null,
          title: null,
          errors: [],
          state: {
            editTarget: "",
            newIndex: ""
          }
        };
      },
      getRecordUrl(type) {
        switch (type)
        {
          case 'post':
            return pengineHelpers.fixUrl('/api/posts/');
            break;
          case 'article':
            return pengineHelpers.fixUrl('/api/articles/');
            break;
          case 'resume':
            return pengineHelpers.fixUrl('/api/resume/');
            break;
          case 'settings':
            return pengineHelpers.fixUrl('/api/settings/');
            break;
        }
        return null;
      },
      getRecordTitle(newRecord) {
        switch (newRecord.type)
        {
          case 'post':
            if (newRecord.data && newRecord.guid) {
              newRecord.title = `Editing Post "${newRecord.data.name}"`;
            }
            else {
              newRecord.title = 'Adding New Post';
            }
            break;
          case 'article':
            if (newRecord.data && newRecord.guid) {
              newRecord.title = `Editing Article "${newRecord.data.name}"`;
            }
            else {
              newRecord.title = 'Adding New Article';
            }
            break;
          case 'resume':
            newRecord.title = 'Editing Resume';
            break;
          case 'settings':
            newRecord.title = 'Editing Settings';
            break;
        }
      },
      initRecord(type, guid, data, errors) {
        let newRecord = editorHelpers.getRecordDefaultData();
        newRecord.type = type;
        newRecord.guid = guid ? guid : null;
        newRecord.url = editorHelpers.getRecordUrl(type);
        newRecord.data = data ? data : {};
        newRecord.errors = errors;
        editorHelpers.getRecordTitle(newRecord);
        if (!data) {
          switch (newRecord.type)
          {
            case 'post':
              newRecord.data.name = 'New Post';
              break;
            case 'article':
              newRecord.data.name = 'New Article';
              break;
          }
        }
        switch (newRecord.type) {
          case 'resume':
            newRecord.state.editTarget = 'personals:0';
            break;
        }
        return newRecord;
      },
      isRecordGetable(newRecord) {
        return newRecord.guid || newRecord.type === 'resume' || newRecord.type === 'settings';
      }
    };

  export default {
    name: "pengine-editor",
    mounted() {
      this.processLocationHash();
    },
    methods: {
      fireEvent(eventName, type, guid, data) {
        switch (eventName || "")  {
          case "edit":
            editorHelpers.updateLocationHash(type, guid);
            document.body.style.overflow = 'hidden';
            window.scrollTo(0, 0);

            let newRecord = editorHelpers.initRecord(type, guid, data);
            if (editorHelpers.isRecordGetable(newRecord) && newRecord.url)
            {
              new Promise(
                (resolve, reject) => {  
                  let getUrl = newRecord.url;
                  if (newRecord.guid) {
                    getUrl = `${newRecord.url}${newRecord.guid}`;
                  }
                  this.$http.get(getUrl).then(response => {
                    newRecord.data = response.body;
                    resolve();
                  }, response => {
                    newRecord.errors = [ { type: "Error", text: "An HTTP error prevented the record from loading." } ];
                    reject();
                  });
                }
              ).then(() => {
                editorHelpers.getRecordTitle(newRecord);
                this.record = newRecord;
              });
            }
            else {
              this.record = newRecord;
            }
            break;
        }
      },
      cancelRecord() {
        this.record = editorHelpers.getRecordDefaultData();
        editorHelpers.updateLocationHash();
        document.body.style.overflow = 'initial';
      },
      confirmDeleteRecord() {
        let confirmMessage = `Are you sure you want to delete this ${this.record.type} record?`;
        if (this.record.data.name) {
          confirmMessage = `Are you sure you want to delete this ${this.record.type} record entitled "${this.record.data.name}"?`;
        }
        if (confirm(confirmMessage)) {
          this.deleteRecord();
        }
      },
      deleteRecord() {
        if (this.record.url && this.record.guid) {
          this.$http.delete(`${this.record.url}${this.record.guid}`).then(response => {
            this.cancelRecord();
            window.location.reload();
          }, response => {
            this.record.errors = response.body.logMessages ? response.body.logMessages : [ { type: "Error", text: "An HTTP error prevented the record from deleting." } ];
          });
        }
      },
      saveRecord() {
        if (this.record.url) {
          this.$http.put(this.record.url, this.record.data).then(response => {
            editorHelpers.updateLocationHash(this.record.type, response.body.guid);
            window.location.reload();
          }, response => {
            this.record.errors = response.body.logMessages ? response.body.logMessages : [ { type: "Error", text: "An HTTP error prevented the record from updating." } ];
          });
        }
      },
      addTarget(type) {
        let newTarget = {};
        if (!this.record.data[type]) {
          this.record.data[type] = [];
        }
        switch (type) {
          case 'sections':
            newTarget['name'] = 'New Section';
            newTarget['data'] = 'New Section Data';
            break;
          case 'skillTypes':
            newTarget['type'] = 'New Skill Type';
            newTarget['skills'] = [ ];
            break;
        }
        this.record.data[type].push(newTarget);
        this.record.state.editTarget = `${type}:${this.record.data[type].length - 1}`;
        this.record.state.newIndex = "";
      },
      removeCurrentTarget() {
        let info = this.currentEditTargetInfo;
        if (info.set) {
          if (!info.numeric) {
            delete this.record.data[info.property][info.index]
          }
          else {
            this.record.data[info.property].splice(info.index, 1);
          }
          this.record.state.editTarget = "";
          this.record.state.newIndex = "";
        }
      },
      moveCurrentTarget(offset) {
        let info = this.currentEditTargetInfo;
        if (info.set) {
          if (info.numeric) {
            let currentTarget = this.currentEditTarget;
            let newIndex = info.index + offset;
            //Remove item from existing position
            this.record.data[info.property].splice(info.index, 1);
            //Add item at new position
            this.record.data[info.property].splice(newIndex, 0, currentTarget);
            //Update sort order properties on sub items
            for (let currentIndex in this.record.data[info.property]) {
              this.record.data[info.property][currentIndex].sortOrder = currentIndex;
            }

            this.record.state.editTarget = `${info.property}:${newIndex}`;
          }
        }
      },
      renameIndex(nameProperty, subRecordArrayProperty, subRecordNameProperty) {
        let info = this.currentEditTargetInfo;
        if (subRecordArrayProperty && subRecordNameProperty) {
          for (var subRecordIndex in this.record.data[info.property][info.index][subRecordArrayProperty]) {
            this.record.data[info.property][info.index][subRecordArrayProperty][subRecordIndex][subRecordNameProperty] = this.record.data[info.property][info.index][nameProperty];
          }
        }
      },
      addIndexSubRecord(subRecordArrayProperty, nameValue) {
        let info = this.currentEditTargetInfo;
        if (this.record.data[info.property][info.index][subRecordArrayProperty]) {
          let newSubRecord = {};
          switch (subRecordArrayProperty) {
            case 'skills':
              newSubRecord['type'] = nameValue;
              break;
          }
          this.record.data[info.property][info.index][subRecordArrayProperty].push(newSubRecord);
        }
      },
      removeIndexSubRecord(subRecordArrayProperty, subRecordIndex) {
        let info = this.currentEditTargetInfo;
        if (this.record.data[info.property][info.index][subRecordArrayProperty] 
          && this.record.data[info.property][info.index][subRecordArrayProperty].length
          && this.record.data[info.property][info.index][subRecordArrayProperty].length > subRecordIndex) {
          this.record.data[info.property][info.index][subRecordArrayProperty].splice(subRecordIndex, 1);
        }
      },
      processLocationHash() {
        if (window.location.hash && window.location.hash !== '' && window.location.hash.indexOf('#edit/') === 0) {
          let elements = window.location.hash.split('/');
          this.fireEvent("edit", elements[1], (elements.length > 2 ? elements[2] : null));
        }
      }
    },
    data() {
      return {
        record: editorHelpers.getRecordDefaultData(),
        state: window.pengineState
      }
    },
    computed: {
      currentEditTargetInfo() {
        let info = { property: null, index: null, numeric: false, set: false };
        if (typeof(this.record.state.editTarget) !== "undefined" && this.record.state.editTarget !== "") {
          let targetElements = this.record.state.editTarget.split(":");
          if (targetElements.length > 1) {
            info.property = targetElements[0];
            info.index = parseInt(targetElements[1]);
            if (isNaN(info.index)) {
              info.index = targetElements[1];
            }
            else {
              info.numeric = true;
            }
            info.set = true;
          }
        }
        this.record.state.newIndex = info.index;
        return info;
      },
      currentEditTarget() {
        let info = this.currentEditTargetInfo;
        if (info.set && this.record.data[info.property])
        {
          let data = this.record.data[info.property][info.index];
          while (!data) {
            if (info.numeric) {
              this.addTarget(info.property);
            }
            else {
              this.addTarget(info.property, info.index);
            }
            data = this.record.data[info.property][info.index];
          }
          return data;
        }
        return {};
      },
      currentEditTargetPosition() {
        let info = this.currentEditTargetInfo;
        if (info.set) {
          if (info.numeric && this.record.data[info.property] && this.record.data[info.property].length) {
            if (info.index === 0) {
              return "First";
            }
            else if (info.index === this.record.data[info.property].length - 1) {
              return "Last";
            }
            else {
              return "Between";
            }
          }
          else {
            return "Named";
          }
        }
        return "NA";
      },
      currentEditTargetProperty() {
        return this.currentEditTargetInfo.set ? this.currentEditTargetInfo.property : "";
      }
    }
  };
</script>