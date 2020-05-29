import React from 'react';
import pengineHelpers from "./pengine.core.web.helpers";

class PEnginePostEditor extends React.Component {

  constructor(props) {
    super(props);

    this.state = {
      post: {
        guid: null,
        legacyID: null,
        name: 'New Post',
        data: '',
        iconFileName: '',
        visibleFlag: false,
        noIndexFlag: true,
        uniqueName: null,
        createdUTC: null,
        modifiedUTC: null,
      },
      peState: window.pengineState
    };
  }

  componentDidMount = () => {
    
  }
}

export default PEnginePostEditor;