import React from 'react';
import pengineHelpers from "./pengine.core.web.helpers";

class PEngineHeader extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      theme: window.pengineState.theme,
      themeList: window.pengineState.themeList,
      loginText: window.pengineState.loginText,
      loginUrl: pengineHelpers.fixUrl(window.pengineState.loginUrl),
    };
  }

  formSubmit = () => {
    document.getElementById('theme').submit();
  }

  themeUpdate = (e) => {
    if (this.state.theme !== e.target.value) {
      this.setState({
        theme: e.target.value
      });
      this.formSubmit();
    }
  }

  render = () => {
    return (
      <div className="panel">
        <div className="panel-left">
          <form id="theme" method="GET" action="ui/theme">
            <select name="selection" onChange={(e) => this.themeUpdate(e)} value={this.state.theme}>
              {
                this.state.themeList.map(
                  (theme, key) => {
                    return <option key={key} value={theme}>{theme}</option>
                  }
                )
              }
            </select>
          </form>
        </div>
        <div className="panel-right">
          <div className="panel-block">
            <button type="button" className="pengine-button-newpost">New Post</button>
            <button type="button" className="pengine-button-newarticle">New Article</button>
            <button type="button" className="pengine-button-setting">Settings</button>
            <button type="button" className="pengine-button-uploader">Uploader</button>
          </div>
          <div className="panel-block">
            <form method="get" action={this.state.loginUrl}>
              <button type="submit" className="pengine-button-search">{this.state.loginText}</button>
            </form>
          </div>
        </div>
      </div>
    );
  }
}

export default PEngineHeader;