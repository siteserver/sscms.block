var blockScript =
  document.currentScript ||
  Array.prototype.slice.call(document.getElementsByTagName("script")).pop();

var apiUrl = blockScript.getAttribute("data-api-url") || "";
var siteId = blockScript.getAttribute("data-site-id") || "";

var blockApi = axios.create({
  baseURL: apiUrl + "/block/",
  params: { siteId: siteId },
  withCredentials: true
});

var bodyHtml = "";

var blockAuthen = function(password) {
  Swal.showLoading();
  blockApi
    .post("", {
      password: password
    })
    .then(function(response) {
      var res = response.data;

      if (res.value) {
        sessionStorage.setItem('ss-block-session-id', res.sessionId);

        document.body.innerHTML = bodyHtml;
        document.body.style.display = "block";
      } else {
        Swal.fire({
          title: "访问密码不正确！",
          type: "error",
          showConfirmButton: false,
          allowOutsideClick: false,
          allowEscapeKey: false
        });
      }
    })
    .catch(function(error) {
      console.log(error);
    });
};

blockApi
  .get("", {
    params: {
      sessionId: sessionStorage.getItem('ss-block-session-id')
    }
  })
  .then(function(response) {
    var res = response.data;

    var isAllowed = res.value;
    var blockMethod = res.blockMethod;
    var redirectUrl = res.redirectUrl;
    var warning = res.warning;

    if (!isAllowed) {
      if (blockMethod === "RedirectUrl") {
        location.href = redirectUrl;
      } else if (blockMethod === "Warning") {
        bodyHtml = document.body.innerHTML;
        document.body.innerHTML = "";
        document.body.style.display = "block";
        Swal.fire({
          title: warning,
          type: "error",
          showConfirmButton: false,
          allowOutsideClick: false,
          allowEscapeKey: false
        });
      } else if (blockMethod === "Password") {
        bodyHtml = document.body.innerHTML;
        document.body.innerHTML = "";
        document.body.style.display = "block";
        Swal.fire({
          title: "请输入访问密码",
          input: "password",
          showCancelButton: false,
          confirmButtonText: "验 证",
          allowOutsideClick: false,
          allowEscapeKey: false,
          inputValidator: function(value) {
            if (!value) {
              return "请输入访问密码！";
            } else {
              blockAuthen(value);
            }
          }
        });
      }
    } else {
      document.body.style.display = "block";
    }
  })
  .catch(function(error) {
    console.log(error);
  });
