import pengineHelpers from "./pengine.core.web.helpers";

export default {
  create() {
    let editorHelpers = {
      processLocationHash() {
        if (window.location.hash && window.location.hash !== '' && window.location.hash.indexOf('#edit/') === 0) {
          let elements = window.location.hash.split('/');
          component.$events.fire("edit", { type: elements[1], guid: (elements.length > 2 ? elements[2] : null) });
        }
      },
      updateLocationHash(type, guid) {
        let myHash = '';
        if (type) {
          myHash = `#edit/${type}`;
          if (guid) {
            myHash += `/${guid}`;
          }
        }
        window.location.hash = myHash;
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

    let component = new Vue({
      el: "#pengine-editor",
      mounted() {
        this.$events.listen("edit", eventData => {
          this.editRecord(eventData.type, eventData.guid, eventData.data);
        });
      },
      methods: {
        editRecord(type, guid, data) {
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
        addTarget(type, property) {
          if (!property) {
            let newTarget = {};
            if (!this.record.data[type]) {
              this.record.data[type] = [];
            }
            switch (type) {
              case 'sections':
                newTarget['name'] = 'New Section';
                newTarget['data'] = 'New Section Data';
                break;
            }
            this.record.data[type].push(newTarget);
            this.record.state.editTarget = `${type}:${this.record.data[type].length - 1}`;
            this.record.state.newIndex = "";
          }
          else {
            if (!this.record.data[type][property]) {
              this.record.data[type][property] = [];
            }
            this.record.state.editTarget = `${type}:${property}`;
            this.record.state.newIndex = property;
          }
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
        renameIndex(subPropertyName) {
          let info = this.currentEditTargetInfo;
          if (!info.numeric && info.index != this.record.state.newIndex) {
            this.record.data[info.property][this.record.state.newIndex] = this.record.data[info.property][info.index];
            delete this.record.data[info.property][info.index];
            if (subPropertyName) {
              for (let subRecord in this.record.data[info.property][this.record.state.newIndex]) {
                this.record.data[info.property][this.record.state.newIndex][subRecord][subPropertyName] = this.record.state.newIndex;
              }
            }

            this.record.state.editTarget = `${info.property}:${this.record.state.newIndex}`;
          }
        },
        addIndexSubRecord() {
          let info = this.currentEditTargetInfo;
          if (this.record.data[info.property][info.index]) {
            let newSubRecord = {};
            switch (info.property) {
              case 'skills':
                newSubRecord['type'] = info.index;
                break;
            }
            this.record.data[info.property][info.index].push(newSubRecord);
          }
        },
        removeIndexSubRecord(subRecordIndex) {
          let info = this.currentEditTargetInfo;
          if (this.record.data[info.property][info.index] && this.record.data[info.property][info.index].length && this.record.data[info.property][info.index].length > subRecordIndex) {
            this.record.data[info.property][info.index].splice(subRecordIndex, 1);
          }
        }
      },
      data() {
        return {
          record: editorHelpers.getRecordDefaultData(),
          state: window.pengineState
        };
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
                this.addTarget(info.type);
              }
              else {
                this.addTarget(info.type, info.index);
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
    });
    editorHelpers.processLocationHash();
    return component;
  }
};