var _ = require('underscore');
var Vue = require('vue');

var pengineApp = new Vue.default({
  el: '#pengine-app',
  data: function() {
    var numberData = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
    var evenOnlyData = _.filter(numberData, function(element){ return element % 2 == 0; });
    console.log('evenOnlyData', evenOnlyData);
    return {
      message: 'Hello Presentation Engine Users!',
      evenOnlyOutput: evenOnlyData
    };
  }
});