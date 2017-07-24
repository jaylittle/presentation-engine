/* Generic Helpers */
module.exports = {
  fixUrl(currentUrl) {
    if (currentUrl.startsWith("/"))
    {
      var baseUrl = jQuery("base").attr("href");
      baseUrl = baseUrl.endsWith("/") ? baseUrl.substring(0, baseUrl.length - 1) : baseUrl;
      return `${baseUrl}${currentUrl}`;
    }
    return currentUrl;
  }
};