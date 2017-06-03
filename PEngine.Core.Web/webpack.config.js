var path = require('path');
var UglifyJSPlugin = require('uglifyjs-webpack-plugin');
var webpack = require('webpack');

module.exports = {
  entry: './scripts/pengine.core.web.js',
  devtool: 'source-map',
  output: {
    filename: 'app.bundle.min.js',
    path: path.resolve(__dirname, 'wwwroot/dist')
  },
  resolve: {
    alias: {
      'vue$': 'vue/dist/vue.esm.js'
    }
  },
  plugins: [
    new webpack.DefinePlugin({
      'process.env': {
        NODE_ENV: JSON.stringify('production')
      }
    }),
    new UglifyJSPlugin({
      compress: { warnings: false },
      sourceMap: true
    })
  ]
};
