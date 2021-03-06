var $url = '/block/add';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  ruleId: utils.getQueryInt('ruleId'),
  rule: null,
  areaTypes: null,
  blockAreas: null,
  blockChannels: null,
  form: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url, {
      params: {
        siteId: this.siteId,
        ruleId: this.ruleId
      }
    }).then(function (response) {
      var res = response.data;

      $this.rule = res.rule;
      $this.areaTypes = res.areaTypes;
      $this.blockAreas = res.blockAreas;
      $this.blockChannels = res.blockChannels;
      $this.form = _.assign({
        siteId: $this.siteId
      }, $this.rule);
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
      rule: this.form,
      blockAreas: this.blockAreas,
      blockChannels: this.blockChannels,
    }).then(function (response) {
      var res = response.data;

      utils.success('设置保存成功');
      location.href = utils.getPageUrl('block', 'settings', {
        siteId: $this.siteId
      });
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

  addRange: function(isAllowList, range) {
    if (isAllowList) {
      this.form.allowList.push(range);
    } else {
      this.form.blockList.push(range);
    }
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

  handleRangeClose: function(isAllowList, range) {
    if (isAllowList) {
      this.form.allowList = _.remove(this.form.allowList, function(n) {
        return range !== n;
      });
    } else {
      this.form.blockList = _.remove(this.form.blockList, function(n) {
        return range !== n;
      });
    }
  },

  handleChannelClose: function(id) {
    this.blockChannels = _.remove(this.blockChannels, function(n) {
      return id !== n.id;
    });
  },

  btnAreaAddClick: function() {
    utils.openLayer({
      title: "新增区域",
      url: utils.getPageUrl('block', 'addLayerAreaAdd', {
        siteId: this.siteId
      }),
      width: 620,
      height: 400
    });
  },

  btnRangeAddClick: function(isAllowList) {
    utils.openLayer({
      title: "新增IP定位拦截",
      url: utils.getPageUrl('block', 'addLayerRangeAdd', {
        isAllowList: isAllowList
      }),
      width: 620,
      height: 400
    });
  },

  btnChannelAddClick: function() {
    utils.openLayer({
      title: "新增页面",
      url: utils.getPageUrl('block', 'addLayerChannelAdd', {
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
