/* Site Wide Helpers */

(function() {
  var utcTargets = document.getElementsByClassName('datetime-display');
  for (var idx = 0; idx < utcTargets.length; idx++) {
    var epochTime = utcTargets[idx].getAttribute("data-epoch");
    if (epochTime && epochTime != "") {
      utcTargets[idx].innerHTML = convertToLocalTime(parseInt(epochTime));
    }
  }
})();

function convertToLocalTime(epochTime) {
  let tzOffset = (new Date()).getTimezoneOffset();
  let dt = new Date(epochTime);
  let year = dt.getFullYear();
  let month = dt.getMonth() + 1;
  let day = dt.getDate();
  let hour = dt.getHours();
  let min = dt.getMinutes();
  let ap = hour <= 11 ? "AM" : "PM";
  if (hour <= 0) {
    hour = 12;
  }
  else if (hour > 12) {
    hour = hour - 12;
  }
  return `${year}/${month}/${day} ${hour}:${padNumber(min, 2)} ${ap}`;
}

function padNumber(number, length) {
  var sNumber = number.toString();
  while (sNumber.length < length) {
    sNumber = `0${sNumber}`;
  }
  return sNumber;
}