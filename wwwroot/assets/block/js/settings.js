var $url = '/block/settings';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  rules: null,
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: $this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.rules = res.rules;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDelete: function (item) {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url, {
      data: {
        siteId: $this.siteId,
        ruleId: item.id
      }
    }).then(function (response) {
      var res = response.data;

      $this.rules = res.rules;
      utils.success('拦截规则删除成功');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnAddClick: function () {
    location.href = utils.getPageUrl('block', 'add', {
      siteId: this.siteId
    });
  },

  btnEditClick: function (item) {
    location.href = utils.getPageUrl('block', 'add', {
      siteId: this.siteId,
      ruleId: item.id
    });
  },

  btnDeleteClick: function (item) {
    var $this = this;

    utils.alertDelete({
      title: '删除拦截规则',
      text: '此操作将删除拦截规则' + item.ruleName + '，确定吗？',
      callback: function () {
        $this.apiDelete(item);
      }
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});
