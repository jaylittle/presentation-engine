import pengineHelpers from "./pengine.core.web.helpers";

export default {
  create() {
    return new Vue({
      el: "#pengine-editor",
      mounted() {
        this.$events.listen("edit", eventData => {
          this.editRecord(eventData.type, eventData.guid, eventData.data);
        });
      },
      methods: {
        editRecord(type, guid, data) {
          console.log("edit1", type, guid, data);
          let newRecord = this.initRecord(type, guid, data);
          if (this.isRecordGetable(newRecord) && newRecord.url)
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
              this.titleRecord(newRecord);
              this.record = newRecord;
            });
          }
          else {
            this.record = newRecord;
          }
        },
        initRecord(type, guid, data, errors) {
          let newRecord = {
            type: type,
            guid: guid ? guid : null,
            url: this.getRecordUrl(type),
            data: data ? data : {},
            title: null,
            errors: errors
          };
          this.titleRecord(newRecord);
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
          return newRecord;
        },
        titleRecord(newRecord) {
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
        isRecordGetable(newRecord) {
          return newRecord.guid || newRecord.type === 'resume' || newRecord.type === 'settings';
        },
        cancelRecord() {
          this.record = {
            type: 'none',
            guid: null,
            url: null,
            data: null,
            title: null,
            errors: []
          };
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
            console.log('deleting data', this.record.data);
            this.$http.delete(`${this.record.url}${this.record.guid}`).then(response => {
              console.log('deleted data', response);
              this.cancelRecord();
            }, response => {
              console.log('delete failed data', response);
              this.record.errors = response.body.logMessages ? response.body.logMessages : [ { type: "Error", text: "An HTTP error prevented the record from deleting." } ];
            });
          }
        },
        saveRecord() {
          if (this.record.url) {
            console.log('saving data', this.record.data);
            this.$http.put(this.record.url, this.record.data).then(response => {
              console.log('saved data', response);
              this.record.data = response.body;
              this.record.errors = [];
              this.titleRecord(this.record);
            }, response => {
              console.log('save failed data', response);
              this.record.errors = response.body.logMessages ? response.body.logMessages : [ { type: "Error", text: "An HTTP error prevented the record from updating." } ];
            });
          }
        }
      },
      data() {
        return {
          record: {
            type: 'none',
            guid: null,
            data: null,
            title: null,
            errors: []
          },
          state: window.pengineState
        };
      }
    });
  }
};