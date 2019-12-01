App({
  onLaunch(options) {
    // 第一次打开
    // options.query == {number:1}  query=number%3D1&page=x%2Fy%2Fz
    console.info('App onLaunch');
    console.info(options);

    console.info(options.query.propertyId);
    console.info(options.path)

    if (options.query.propertyId != undefined && options.query.propertyId != null && options.query.propertyId != ""
      && !isNaN(parseInt(options.query.propertyId)))
      this.globalData.initialPropertyId = parseInt(options.query.propertyId);


    this.globalData.systemInfo = wx.getSystemInfoSync();
    console.log(this.globalData.systemInfo);
    this.globalData.rpx = 750 / this.globalData.systemInfo.windowWidth;
    console.log(this.globalData.initialPropertyId);
  },
  onShow(options) {
    // 从后台被 scheme 重新打开
    // options.query == {number:1}
  },

  globalData: {
    initialPropertyId: 9,
    systemInfo: {},
    // guid: "",
    rpx: 0.5,
    // //apiBaseUrl: "http://qzschy.vaiwan.com/api/",
    apiBaseUrl: "http://localhost:8087/api/",
    // resourceUrl: "https://www.qzgis.cn/assets/",
    // webUrl: "https://www.qzgis.cn/",
    // location: {
    //   initial: false,
    //   lng: 118.8546289036,
    //   lat: 28.9732601540,
    // }
  }
});
