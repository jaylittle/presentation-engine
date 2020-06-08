let path = require('path');
let terserPlugin = require('terser-webpack-plugin');
let webpack = require('webpack');
let PROD = (process.env.NODE_ENV === 'production');

var plugins = [
];

if (PROD) {
  plugins.push(new terserPlugin({
    sourceMap: true,
    include: /\.min\.js$/
  }));
}

module.exports = {
  entry: {
    'pengine.core.web.3rdparty': './scripts/pengine.core.web.3rdparty.js',
    'pengine.core.web.main': './scripts/pengine.core.web.main.js',
    'pengine.core.web.sitewide': './scripts/pengine.core.web.sitewide.js'
  },
  mode: PROD ? 'production' : 'development',
  devtool: 'source-map',
  output: {
    filename: '[name].min.js',
    path: path.resolve(__dirname, 'wwwroot/dist')
  },
  resolve: {
    alias: {
    }
  },
  plugins: plugins,
  module: {
    rules: [
      {
        test: /\.js$/,
        exclude: /(node_modules)/,
        use: {
          loader: 'babel-loader',
          options: {
            presets: [ "@babel/preset-env", "@babel/preset-react" ],
            plugins: [ "@babel/plugin-proposal-class-properties" ]
          }
        }
      }
    ]
  }
};
