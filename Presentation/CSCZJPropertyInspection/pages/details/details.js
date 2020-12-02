var app = getApp();
const util = require('../../utils/util');
var fromInitialData = false;
Page({
  data: {
    scrollHeight: 500,
    activeMoreInfoNav: 0,
    current: null,
    properties:[]
  },
  onLoad(query) {
    var that = this;

    console.log("detail query");
    console.log(query.propertyId);
    console.log("finish");

    var pId = app.globalData.initialPropertyId;
    if (pId > 0) fromInitialData = true;
    else if (pId == 0) pId = query.propertyId;

    var url = app.globalData.apiBaseUrl + "properties/" + pId;

    util.httpGet(url).then(function (res) { 
      that.setData({
        current: res.data
      });
    }).catch(function (err) {
      console.log(err);
      util.showToastPromisified({
        title: '未识别到有效的资产信息',
        icon: 'none',
        duration: 2000,
        mask: true
      }).then(function () {
        clearIntialPropertyId();
        util.redirectToPromisified({
          url: '/pages/index/index'
        });
      });
    });


    this.setData({
      scrollHeight: app.globalData.systemInfo.windowHeight - 102 / app.globalData.rpx,
    })
  },

  navTap: function (event) {
    console.log(event);

    var targetId = parseInt(event.target.dataset.id);
    if (targetId == this.data.activeMoreInfoNav) return;

    this.setData({
      activeMoreInfoNav: targetId
    })

  },

  navChange: function (event) {
    console.log(event);
    this.setData({
      activeMoreInfoNav: event.detail.current
    })
  },

  navToMap: function () {
    wx.navigateTo({
      url: "/pages/map/map"
    })
  },

  nanToPano: function (event) {
    wx.navigateTo({
      url: "/pages/panorama/panorama"
    })
  },

  navToIndex: function (event) {
    this.clearIntialPropertyId();
    wx.reLaunch({
      url: "/pages/index/index"
    })
  },

  sigIn: function (event) {
    var that = this;
    if (this.data.current.name == "原交通局办公楼") {
      wx.alert({
        title: '提示',
        content: '当前位置距离资产位置较远，无法打卡，请接近后再次尝试',
        buttonText: '我知道了',
        success: () => {
        },
      });
    }
    else {
      wx.confirm({
        title: '温馨提示',
        content: '您已进入巡查打卡范围，是要对资产【' + that.data.current.name + '】进行巡查打卡?',
        confirmButtonText: '拍照打卡',
        cancelButtonText: '暂不需要',
        success: (result) => {
          if (result.confirm) {
            wx.chooseImage({
              count: 1,
              sourceType: ['camera'],
              success: (res) => {
                wx.alert({
                  title: '打卡成功！'
                })
              },
            });
          }
        },
      });
    }
  },

  onShareAppMessage() {
    var title = this.data.current.name;
    return {
      title: title,
      desc: "资产巡查系统",
      path: "pages/index/index"
    };
  },

  //清除默认自带的参数
  clearIntialPropertyId(){
    if (fromInitialData) app.globalData.initialPropertyId = 0;
  }
});
