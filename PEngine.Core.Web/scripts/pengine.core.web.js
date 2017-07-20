import "babel-polyfill";
import Vue from "vue";
import VueResource from "vue-resource";
import VueRouter from "vue-router";
import VueEvents from "vue-events";
import jQuery from "jquery";
Vue.use(VueResource);
Vue.use(VueRouter);
Vue.use(VueEvents);

new Promise(
  (resolve, reject) => {  
    resolve([true, false, true]);
  }
).then(output => {
  console.log("post-promise", output);
});

/* Generic Helpers */
let pengineHelpers = {
  fixUrl(currentUrl) {
    if (currentUrl.startsWith("/"))
    {
      var baseUrl = jQuery("base").attr("href");
      baseUrl = baseUrl.endsWith("/") ? baseUrl.substring(0, baseUrl.length - 1) : baseUrl;
      return `${baseUrl}${currentUrl}`;
    }
    return currentUrl;
  }
};

/* Vue Components */
Vue.component("header-component", {
  template: `
  <div class="panel-right">
    <form method="get" v-bind:action="fixedLoginUrl">
      <button type="submit" class="pengine-button-search">{{state.loginText}}</button>
    </form>
  </div>
  `,
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

Vue.component("editor-component", {
  mounted() {
    this.$events.listen("edit", eventData => console.log("edit", eventData));
  }
});

let mainComp = new Vue({
  el: "#pengine-component"
});

/* Bind Events to the DOM */
jQuery(".post_view_button_edit").on("click", (e) => {
  mainComp.$events.fire("edit", { type: "post", guid: jQuery(e.currentTarget).data("guid")})
  e.preventDefault();
});

jQuery(".article_view_button_edit").on("click", (e) => {
  mainComp.$events.fire("edit", { type: "article", guid: jQuery(e.currentTarget).data("guid")})
  e.preventDefault();
});

jQuery(".resume_view_button_edit").on("click", (e) => {
  mainComp.$events.fire("edit", { type: "resume"})
  e.preventDefault();
});