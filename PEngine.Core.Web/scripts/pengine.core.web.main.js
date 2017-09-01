import pengineHelpers from "./pengine.core.web.helpers";
Vue.use(VueResource);
Vue.use(VueEvents);
import peHeader from "./pengine.core.web.vue.header";
import peEditor from "./pengine.core.web.vue.editor";
import peUploader from "./pengine.core.web.vue.uploader";

let editorInstance = peEditor.create();
let headerInstance = peHeader.create();
let uploaderInstance = peUploader.create();

/* Bind Events to the DOM */
pengineHelpers.assignComponentClickEvent(editorInstance, "post_view_button_edit", "post");
pengineHelpers.assignComponentClickEvent(editorInstance, "pengine-button-newpost", "post");
pengineHelpers.assignComponentClickEvent(editorInstance, "article_view_button_edit", "article");
pengineHelpers.assignComponentClickEvent(editorInstance, "pengine-button-newarticle", "article");
pengineHelpers.assignComponentClickEvent(editorInstance, "resume_view_button_edit", "resume");
pengineHelpers.assignComponentClickEvent(editorInstance, "pengine-button-setting", "settings");
pengineHelpers.assignComponentClickEvent(uploaderInstance, "pengine-button-uploader", "settings", "show");

/* Setup Automatic Token Refresh */
setupAutoTokenRefresh();

function setupAutoTokenRefresh()
{
  if (window.pengineState.tokenExpiresMilliseconds > 90000)
  {
    setTimeout(refreshToken, window.pengineState.tokenExpiresMilliseconds - 90000);
  }
  else
  {
    refreshToken();
  }
}

function refreshToken()
{
  Vue.http.get(pengineHelpers.fixUrl('/token/refresh')).then(response => {
    window.pengineState.tokenExpires = response.body.expires;
    window.pengineState.tokenExpiresMilliseconds = response.body.expiresInMilliseconds;
    setupAutoTokenRefresh();
  });
}