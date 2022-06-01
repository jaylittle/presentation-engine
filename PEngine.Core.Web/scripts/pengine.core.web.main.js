import PEHelpers from "./pengine.core.web.helpers";
import PEHeader from "./pengine.core.web.component.header.js";
import PEUploader from "./pengine.core.web.component.uploader.js";
import PEPostEditor from "./pengine.core.web.component.post.editor.js";
import PEArticleEditor from "./pengine.core.web.component.article.editor.js";
import PEResumeEditor from "./pengine.core.web.component.resume.editor.js";
import PESettingEditor from "./pengine.core.web.component.setting.editor.js";

/* Load React Components */
const headerRoot = ReactDOM.createRoot(document.getElementById('pengine-header'));
headerRoot.render(<PEHeader />);

const uploaderRoot = ReactDOM.createRoot(document.getElementById('pengine-uploader'));
uploaderRoot.render(<PEUploader />);

const postEditorRoot = ReactDOM.createRoot(document.getElementById('pengine-post-editor'));
postEditorRoot.render(<PEPostEditor />);

const articleEditorRoot = ReactDOM.createRoot(document.getElementById('pengine-article-editor'));
articleEditorRoot.render(<PEArticleEditor />);

const resumeEditorRoot = ReactDOM.createRoot(document.getElementById('pengine-resume-editor'));
resumeEditorRoot.render(<PEResumeEditor />);

const settingEditorRoot = ReactDOM.createRoot(document.getElementById('pengine-setting-editor'));
settingEditorRoot.render(<PESettingEditor />);

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
  PEHelpers.fetch(PEHelpers.fixUrl('/token/refresh'), {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
    body: null
  })
  .then(PEHelpers.getCombinedJsonResponse, () => {
    this.pushError('A Network error prevented the token from being refreshed!');
  })
  .then(combined => {
    if (combined.response.ok) {
      window.pengineState.tokenExpires = combined.data['expires'];
      window.pengineState.tokenExpiresMilliseconds = combined.data['expires_in_milliseconds'];
      setupAutoTokenRefresh();
    } else {
      this.pushError('An HTTP error prevented the token from being refreshed!');
    }
  });
}