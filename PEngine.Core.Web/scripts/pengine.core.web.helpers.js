/* Generic Helpers */

module.exports = {
  fixUrl(currentUrl) {
    if (currentUrl.startsWith("/"))
    {
      var baseUrl = document.getElementsByTagName('base')[0].href
      baseUrl = baseUrl.endsWith("/") ? baseUrl.substring(0, baseUrl.length - 1) : baseUrl;
      return `${baseUrl}${currentUrl}`;
    }
    return currentUrl;
  },
  assignComponentClickEvent(editorInstance, className, typeName, eventName) {
    eventName = eventName || "edit";
    var targets = document.getElementsByClassName(className);
    for (var idx = 0; idx < targets.length; idx++) {
      targets[idx].addEventListener("click", (e) => {
        if (editorInstance.$children) {
          editorInstance.$children[0].fireEvent(eventName, typeName, e.target.getAttribute("data-guid"));
        } else {
          editorInstance.fireEvent(eventName, typeName, e.target.getAttribute("data-guid"));
        }
        e.preventDefault();
      });
    }
  },
  getCombinedJsonResponse(response) {
    return response.json()
      .then(data => ({ response: response, data: data }), () => ({ response: response, data: null }))
      .catch(() => ({ response: response, data: data }));
  },
  fetchApplyGlobalOptions(options) {
    const update = { ...options };
    let xsrfToken = window.pengineState.xsrfToken;
    if (xsrfToken) {
      update.headers = {
        ...update.headers,
        'xsrf-form-token': xsrfToken,
      };
    }
    return update;
  },
  fetch(url, options) {
    return window.fetch(url, this.fetchApplyGlobalOptions(options));
  },
  updateEditorLocationHash(type, guid) {
    let myHash = '';
    if (type) {
      myHash = `#edit/${type}`;
      if (guid) {
        myHash += `/${guid}`;
      }
    }
    if (myHash != '' || !history) {
      window.location.hash = myHash;
    }
    else {
      history.pushState("", document.title, window.location.pathname + window.location.search);
    }
  },
  updateStateField(component, e, subKeys) {
    let fieldValue = e;
    if (e.target) {
      fieldValue = e.target.value;
      if (e.target.type === 'checkbox' || e.target.type === 'radio') {
        fieldValue = e.target.checked;
      }
    }
    let subKeyArray = subKeys.filter(subKey => subKey || subKey === 0);
    component.setState(prevState => {
      let subKeyPath = '';
      let errorFlag = false;
      var prevStatePart = prevState;
      for (let subKeyIndex = 0; subKeyIndex < subKeyArray.length; subKeyIndex++) {
        subKeyPath += ((subKeyPath ? '.' : '') + subKeyArray[subKeyIndex]);
        if (subKeyIndex < subKeyArray.length - 1) {
          prevStatePart = prevStatePart[subKeyArray[subKeyIndex]];
        }
        if (subKeyIndex === subKeyArray.length - 1) {
          prevStatePart[subKeyArray[subKeyIndex]] = fieldValue;
          break;
        } else if (!prevStatePart) {
          errorFlag = true;
          break;
        }
      }
      if (errorFlag) {
        throw `updateStateField cannot find specified state path of ${subKeyPath}`;
      }
      return prevState;
    });
  }
};