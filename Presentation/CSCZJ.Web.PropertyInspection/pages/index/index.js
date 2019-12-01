var app = getApp();

Page({
  onLoad(query) {
    // 页面加载
    console.info(`Page onLoad with query: ${JSON.stringify(query)}`);

    if (app.globalData.initialPropertyId > 0)
      wx.navigateTo({
        url: '../detail/detail?propertyId=' + app.globalData.initialPropertyId
      })
  },
  onReady() {
    // 页面加载完成
  },
  onShow() {
    // 页面显示
  },
  onHide() {
    // 页面隐藏
  },
  onUnload() {
    // 页面被关闭
  },
  onTitleClick() {
    // 标题被点击
  },
  onPullDownRefresh() {
    // 页面被下拉
  },
  onReachBottom() {
    // 页面被拉到底部
  },
  onShareAppMessage() {
    // 返回自定义分享信息
    return {
      title: '资产扫一扫',
      desc: '资产巡查',
      path: 'pages/index/index',
    };
  },

  scanQRCode: function () {
    var properyId = 0;
    wx.scanCode({
      scanType: 'qrCode',
      success: (res) => {
        var qrCodeValue = res.result;
        var queryParams = qrCodeValue.replace("dingtalk://dingtalkclient/action/open_mini_app?", "").split('&');

        queryParams.forEach(function (item, index) {
          console.log(item);
          if (item.indexOf('query=') > -1) {
            var queryValue = decodeURIComponent(item.replace("query=", "")).replace("propertyId=", "");

            if (!isNaN(queryValue)) {
              properyId = parseInt(queryValue);
              return false;
            }
          }
        });

        if (properyId > 0) {
          wx.navigateTo({
            url: '../detail/detail?propertyId=' + properyId
          })
        }
        else {
          wx.showToast({
            type: 'fail',
            content: '未获取有效的资产Id',
            duration: 3000,
            success: () => {

            },
          });
        }

        console.log(properyId);

      },
    });
  }
});
