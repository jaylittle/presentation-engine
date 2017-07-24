import pengineHelpers from "./pengine.core.web.helpers";

module.exports = {
  create() {
    return new Vue({
      el: "#pengine-header",
      data() {
        return {
          state: window.pengineState
        };
      },
      computed: {
        fixedLoginUrl() {
          return pengineHelpers.fixUrl(this.state.loginUrl);
        }
      }
    });
  }
};