<template>
  <div id="pengine-header" class="panel">
    <div class="panel-left">
      <form id="theme" method="GET" action="/ui/theme">
        <select name="selection" onchange="this.form.submit()" v-model="state.theme">
          <option v-for="theme in state.themeList" v-bind:value="theme">{{theme}}</option>
        </select>
      </form>
    </div>
    <div class="panel-right">
      <div class="panel-block">
        <button type="button" class="pengine-button-newpost">New Post</button>
        <button type="button" class="pengine-button-newarticle">New Article</button>
        <button type="button" class="pengine-button-setting">Settings</button>
        <button type="button" class="pengine-button-uploader">Uploader</button>
      </div>
      <div class="panel-block">
        <form method="get" v-bind:action="fixedLoginUrl">
          <button type="submit" class="pengine-button-search">{{state.loginText}}</button>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
  import pengineHelpers from "./pengine.core.web.helpers";

  export default {
    name: "pengine-header",
    data() {
      return {
        state: window.pengineState
      };
    },
    computed: {
      fixedLoginUrl() {
        return pengineHelpers.fixUrl(this.state.loginUrl);
      }
    }
  };
</script>