var script_path = "./public/scripts/"

module.exports = {
    entry: {
        reactboostraptest: script_path + "reactbootstraptest.js",
        stream: script_path + "stream.js"
    },
    output: {
        path: script_path,
        filename: "[name].bundle.js"
    },
    module: {
        loaders: [
            {
                test: /\.js$/,
                loader: 'babel-loader',
                exclude: /node_modules/,
                query: {
                    presets: ['es2015', 'react']
                }
            }
        ]
    }
};