/* Site Wide Helpers */

(function() {
  if (document.getElementById('themeSubmit'))
  {
    document.getElementById('themeSubmit').style.display = 'none';
  }

  if (document.getElementById('scrollToTop'))
  {
    document.getElementById('scrollToTop').addEventListener("click", (e) => {
      window.scrollTo(0, 0);
      e.preventDefault();
    });
  }

  if (document.getElementById('pengine-button-quote-show'))
  {
    document.getElementById('pengine-button-quote-show').style.display = 'inline';
  }

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

window.showQuote = function() {
  document.getElementById('pengine-button-quote-show').style.display = 'none';
  document.getElementById('quote-text').style.display = 'block';
}