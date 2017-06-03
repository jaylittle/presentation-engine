import 'babel-polyfill';
import Vue from 'vue';

new Promise(
  (resolve, reject) => {
    let pengineApp = new Vue({
      el: '#pengine-app',
      data: () => {
        let numberData = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        let evenOnlyData = numberData.filter(element => element % 2 == 0);
        console.log('evenOnlyData', evenOnlyData);
        return {
          message: 'Hello Presentation Engine Users!',
          evenOnlyOutput: evenOnlyData
        };
      }
    });
    resolve(pengineApp);
  }
).then(output => {
  console.log('post-promise', output);
});