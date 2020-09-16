var $url = '/block/addLayerChannelAdd';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  channels: null,
  form: {
    channelIds: null
  }
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.channels = [res.channels];
      $this.form.channelIds = null;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function() {
    var $this = this;

    var channelIds = [];
    this.form.channelIds.forEach(function(arr) {
      var channelId = arr[arr.length - 1];
      channelIds.push(channelId);
    });

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId,
      channelIds: channelIds
    }).then(function (response) {
      var res = response.data;

      var channels = res.channels;
      channels.forEach(function (channel) {
        parent.$vue.addChannel(
          channel.id,
          channel.name
        );
      });
      
      utils.closeLayer();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});
