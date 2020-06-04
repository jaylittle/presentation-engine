import pengineHelpers from "./pengine.core.web.helpers";
import PEHeader from "./pengine.core.web.header.react.js";
import PEUploader from "./pengine.core.web.uploader.react.js";
import PEPostEditor from "./pengine.core.web.post.editor.react.js";
import PEArticleEditor from "./pengine.core.web.article.editor.react.js";
import PEResumeEditor from "./pengine.core.web.resume.editor.react.js";
import PESettingEditor from "./pengine.core.web.setting.editor.react.js";

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
pengineHelpers.assignComponentClickEvent(postEditorInstance, "post_view_button_edit", "post");
pengineHelpers.assignComponentClickEvent(postEditorInstance, "pengine-button-newpost", "post");
pengineHelpers.assignComponentClickEvent(articleEditorInstance, "article_view_button_edit", "article");
pengineHelpers.assignComponentClickEvent(articleEditorInstance, "pengine-button-newarticle", "article");
pengineHelpers.assignComponentClickEvent(resumeEditorInstance, "resume_view_button_edit", "resume");
pengineHelpers.assignComponentClickEvent(settingEditorInstance, "pengine-button-setting", "settings");
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
    window.pengineState.tokenExpires = response.body['expires'];
    window.pengineState.tokenExpiresMilliseconds = response.body['expires_in_milliseconds'];
    setupAutoTokenRefresh();
  });
}