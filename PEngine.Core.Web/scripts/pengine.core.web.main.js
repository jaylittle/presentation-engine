import pengineHelpers from "./pengine.core.web.helpers";
Vue.use(VueResource);
Vue.use(VueEvents);
import peHeader from "./pengine.core.web.vue.header";
import peEditor from "./pengine.core.web.vue.editor";

let editorInstance = peEditor.create();
let headerInstance = peHeader.create();

/* Bind Events to the DOM */
pengineHelpers.assignEditorClickEvent(editorInstance, "post_view_button_edit", "post");
pengineHelpers.assignEditorClickEvent(editorInstance, "pengine-button-newpost", "post");
pengineHelpers.assignEditorClickEvent(editorInstance, "article_view_button_edit", "article");
pengineHelpers.assignEditorClickEvent(editorInstance, "pengine-button-newarticle", "article");
pengineHelpers.assignEditorClickEvent(editorInstance, "resume_view_button_edit", "resume");
pengineHelpers.assignEditorClickEvent(editorInstance, "pengine-button-setting", "settings");

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