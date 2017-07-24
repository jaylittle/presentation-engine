import pengineHelpers from "./pengine.core.web.helpers";

module.exports = {
  create() {
    return new Vue({
      el: "#pengine-editor",
      mounted() {
        this.$events.listen("edit", eventData => {
          console.log("edit", eventData);
          let oldRecord = this.record;
          let newRecord = {
            type: eventData.type,
            guid: eventData.guid,
            data: null,
            title: null
          };
          let url = null;
          switch (newRecord.type)
          {
            case 'post':
              url = pengineHelpers.fixUrl('/api/posts/');
              newRecord.data = {
                name: 'New Post'
              };
              newRecord.title = 'New Post';
              break;
          }
          if (newRecord.guid && url)
          {
            new Promise(
              (resolve, reject) => {  
                this.$http.get(`${url}${newRecord.guid}`).then(response => {
                  newRecord.data = response.body;
                  resolve();
                }, response => {
                  reject();
                });
              }
            ).then(output => {
              switch (newRecord.type)
              {
                case 'post':
                  newRecord.title = `Editing Post "${newRecord.data.name}"`;
                  break;
              }
              this.record = newRecord;
            });
          }
        });
      },
      methods: {
        cancel() {
          this.record = {
            type: 'none',
            guid: null,
            data: null,
            title: null
          };
        },
        save() {
          let url = null;
          switch (this.record.type)
          {
            case 'post':
              url = pengineHelpers.fixUrl('/api/posts/');
              break;
          }
          if (url) {
            console.log('saving data', this.record.data);
            this.$http.put(url, this.record.data).then(output => {
              console.log('saved data', output);
              this.cancel();
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
            title: null
          },
          state: window.pengineState
        };
      }
    });
  }
};