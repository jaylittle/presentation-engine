/* Site Wide Helpers */

var utcTargets = document.getElementsByClassName('datetime-display');
for (var idx = 0; idx < utcTargets.length; idx++) {
  var targetUtcTime = convertToLocalTime(utcTargets[idx].innerHTML);
  utcTargets[idx].innerHTML = targetUtcTime;
}

function convertToLocalTime(utcDtString) {
  let tzOffset = (new Date()).getTimezoneOffset();
  utcDtString = utcDtString + (utcDtString.endsWith('Z') ? '' : 'Z');
  let dt = new Date(Date.parse(utcDtString) - (tzOffset * 60000));
  let year = dt.getUTCFullYear();
  let month = dt.getUTCMonth() + 1;
  let day = dt.getUTCDate();
  let hour = dt.getUTCHours();
  let min = dt.getUTCMinutes();
  let ap = hour <= 11 ? "AM" : "PM";
  if (hour <= 0) {
    hour = 12;
  }
  return `${year}/${padNumber(month, 2)}/${day} ${hour}:${padNumber(min, 2)} ${ap}`;
}

function padNumber(number, length) {
  var sNumber = number.toString();
  while (sNumber.length < length) {
    sNumber = `0${sNumber}`;
  }
  return sNumber;
}