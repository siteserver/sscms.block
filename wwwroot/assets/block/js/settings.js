var $url = '/block/settings';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  config: null,
  blockAreas: null,
  blockChannels: null,
  form: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.config = res.config;
      $this.blockAreas = res.blockAreas;
      $this.blockChannels = res.blockChannels;
      $this.form = _.assign({}, $this.config);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId,
      config: this.form,
      blockAreas: this.blockAreas,
      blockChannels: this.blockChannels,
    }).then(function (response) {
      var res = response.data;

      utils.success('设置保存成功');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  addArea: function(id, name) {
    this.blockAreas.push({
      id: id,
      name: name
    });
  },

  addChannel: function(id, name) {
    this.blockChannels.push({
      id: id,
      name: name
    });
  },

  handleAreaClose: function(id) {
    this.blockAreas = _.remove(this.blockAreas, function(n) {
      return id !== n.id;
    });
  },

  handleChannelClose: function(id) {
    this.blockChannels = _.remove(this.blockChannels, function(n) {
      return id !== n.id;
    });
  },

  btnAreaAddClick: function() {
    utils.openLayer({
      title: "新增区域",
      url: utils.getPageUrl('block', 'settingsLayerAreaAdd', {
        siteId: this.siteId
      }),
      width: 620,
      height: 400
    });
  },

  btnChannelAddClick: function() {
    utils.openLayer({
      title: "新增页面",
      url: utils.getPageUrl('block', 'settingsLayerChannelAdd', {
        siteId: this.siteId
      }),
      width: 620,
      height: 400
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});
