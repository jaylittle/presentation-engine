import pengineHelpers from "./pengine.core.web.helpers";
Vue.use(VueResource);
Vue.use(VueEvents);
import peHeader from "./pengine.core.web.vue.header";
import peEditor from "./pengine.core.web.vue.editor";

let editorInstance = peEditor.create();
let headerInstance = peHeader.create();

/* Bind Events to the DOM */
jQuery(".post_view_button_edit").on("click", (e) => {
  editorInstance.$events.fire("edit", { type: "post", guid: jQuery(e.currentTarget).data("guid")})
  e.preventDefault();
});

jQuery(".article_view_button_edit").on("click", (e) => {
  editorInstance.methods.fire("edit", { type: "article", guid: jQuery(e.currentTarget).data("guid")})
  e.preventDefault();
});

jQuery(".resume_view_button_edit").on("click", (e) => {
  editorInstance.methods.fire("edit", { type: "resume"})
  e.preventDefault();
});