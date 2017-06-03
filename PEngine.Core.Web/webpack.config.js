let path = require('path');
let UglifyJSPlugin = require('uglifyjs-webpack-plugin');
let webpack = require('webpack');

module.exports = {
  entry: ['babel-polyfill', './scripts/pengine.core.web.js'],
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
  ],
  module: {
    rules: [
      {
        test: /\.js$/,
        exclude: /(node_modules)/,
        use: {
          loader: 'babel-loader',
          options: {
            presets: ['env']
          }
        }
      }
    ]
  }
};
