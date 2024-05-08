const { defineConfig } = require("@vue/cli-service");

module.exports = defineConfig({
  /*  indexPath: "public/index.html", */
  transpileDependencies: true,
  /*  publicPath: "", */
  chainWebpack: (config) => {
    config.plugin("html").tap((args) => {
      args[0].title = "Ferieregistrering";
      /*    args[0].filename = "[name].html"; */
      return args;
    });
  },
});
