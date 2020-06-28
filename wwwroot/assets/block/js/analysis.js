var $url = '/block/analysis';

var data = utils.init({
  labels: null,
  data: null
});

var methods = {
  apiGet: function() {
    var $this = this;

    $api.get($url).then(function(response) {
        var res = response.data;

        $this.days = res.days;
        $this.count = res.count;
      })
      .catch(function(error) {
        utils.error(error);
      })
      .then(function() {
        utils.loading($this, false);
      });
  }
};

Vue.component("line-chart", {
  extends: VueChartJs.Line,
  mounted: function() {
    this.renderChart(
      {
        labels: this.$root.days,
        datasets: [
          {
            label: "拦截次数",
            backgroundColor: "#f87979",
            data: this.$root.count
          }
        ]
      },
      {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
          yAxes: [
            {
              ticks: {
                beginAtZero: true
              }
            }
          ]
        }
      }
    );
  }
});

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function() {
    this.apiGet();
  }
});
