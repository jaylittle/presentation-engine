import PEHelpers from "./pengine.core.web.helpers";
import PEHeader from "./pengine.core.web.header.js";
import PEUploader from "./pengine.core.web.uploader.js";
import PEPostEditor from "./pengine.core.web.post.editor.js";
import PEArticleEditor from "./pengine.core.web.article.editor.js";
import PEResumeEditor from "./pengine.core.web.resume.editor.js";
import PESettingEditor from "./pengine.core.web.setting.editor.js";

/* Load React Components */
ReactDOM.render(
  <PEHeader />,
  document.getElementById('pengine-header')
);

let uploaderInstance = ReactDOM.render(
  <PEUploader />,
  document.getElementById('pengine-uploader')
);

let postEditorInstance = ReactDOM.render(
  <PEPostEditor />,
  document.getElementById('pengine-post-editor')
);

let articleEditorInstance = ReactDOM.render(
  <PEArticleEditor />,
  document.getElementById('pengine-article-editor')
);

let resumeEditorInstance = ReactDOM.render(
  <PEResumeEditor />,
  document.getElementById('pengine-resume-editor')
);

let settingEditorInstance = ReactDOM.render(
  <PESettingEditor />,
  document.getElementById('pengine-setting-editor')
);

/* Bind Events to the DOM */
PEHelpers.assignComponentClickEvent(postEditorInstance, "post_view_button_edit", "post");
PEHelpers.assignComponentClickEvent(postEditorInstance, "pengine-button-newpost", "post");
PEHelpers.assignComponentClickEvent(articleEditorInstance, "article_view_button_edit", "article");
PEHelpers.assignComponentClickEvent(articleEditorInstance, "pengine-button-newarticle", "article");
PEHelpers.assignComponentClickEvent(resumeEditorInstance, "resume_view_button_edit", "resume");
PEHelpers.assignComponentClickEvent(settingEditorInstance, "pengine-button-setting", "settings");
PEHelpers.assignComponentClickEvent(uploaderInstance, "pengine-button-uploader", "settings", "show");

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
  Vue.http.get(PEHelpers.fixUrl('/token/refresh')).then(response => {
    window.pengineState.tokenExpires = response.body['expires'];
    window.pengineState.tokenExpiresMilliseconds = response.body['expires_in_milliseconds'];
    setupAutoTokenRefresh();
  });
}