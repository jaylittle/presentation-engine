import pengineHelpers from "./pengine.core.web.helpers";
Vue.use(VueResource);
import peHeader from "./pengine.core.web.header.vue";
import peEditor from "./pengine.core.web.editor.vue";
import peUploader from "./pengine.core.web.uploader.vue";

let headerInstance = new Vue({
  el: '#pengine-header',
  render: h => h(peHeader)
});

let editorInstance = new Vue({
  el: '#pengine-editor',
  render: h => h(peEditor)
});

let uploaderInstance = new Vue({
  el: '#pengine-uploader',
  render: h => h(peUploader)
});

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