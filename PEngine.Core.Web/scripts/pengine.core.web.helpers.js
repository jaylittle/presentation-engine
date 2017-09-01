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
        editorInstance.$events.fire(eventName, { type: typeName, guid: e.target.getAttribute("data-guid") });
        e.preventDefault();
      });
    }
  }
};