import 'babel-polyfill';
import Vue from 'vue';

new Promise(
  (resolve, reject) => {
    let pengineHeaderComponent = new Vue({
      el: '#pengine-header-component',
      data: () => {
        let numberData = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        let evenOnlyData = numberData.filter(element => element % 2 == 0);
        return {
          message: 'Placeholder header component',
          evenOnlyOutput: evenOnlyData
        };
      }
    });
    let pengineFooterComponent = new Vue({
      el: '#pengine-footer-component',
      data: () => {
        return {
          message: 'Placeholder footer component'
        };
      }
    });
    resolve([pengineHeaderComponent, pengineFooterComponent]);
  }
).then(output => {
  console.log('post-promise', output);
});