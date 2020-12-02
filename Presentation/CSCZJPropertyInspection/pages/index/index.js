// pages/index/index.js
import Dialog from '../../miniprogram_npm/@vant/weapp/dialog/dialog';
import Notify from '../../miniprogram_npm/@vant/weapp/notify/notify';
import Toast from '../../miniprogram_npm/@vant/weapp/toast/toast';

const util = require('../../utils/util');
const app = getApp();

Page({

  data: {
    properties: [],
    layout: {
      scrollHeight: 500,
    },
    page: {
      index: 1,
      pageSize: 15,
      query: ""
    }
  },

  //#region 页面事件集合

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var that = this;
    var wHeight = app.globalData.deviceInfo.windowHeight; //窗体高度
    var scrollHeight = wHeight - (60 + 54 + 1);
    console.log(scrollHeight);
    that.setData({
      'layout.scrollHeight': scrollHeight
    });

    // 自定义加载图标
    Toast.loading({
      duration: 10000,
      mask: true,
      message: '加载中...',
      forbidClick: true
    });

    //获取用户状态
    app.requestWithToken({
      url: app.globalData.apiUrl + 'Systemmanage/Accounts/GetWechatStatus',
    }).then(function (res) {
      console.log(res);
      var response = res.data;
      if (response.code == "200") {
        Toast.clear();

        if (response.data == "0") {
          that.setData({
            'initial.accountLoading': false
          });

          //#region  若为状态0则表示当前微信用户未绑定，专跳绑定页面
          wx.navigateTo({
            url: '../register/register',
          })
          //#endregion
        }

        that.setData({
          'account.status': response.data
        });

        that.getProperties();

      } else {
        Notify({
          type: 'danger',
          message: res.message,
          duration: 2000
        });
        Toast.clear();
      }
    }, function (err) {
      Notify({
        type: 'danger',
        message: '初始化错误！',
        duration: 2000
      });
      Toast.clear();
    });
  },

  onShow: function () {},

  onHide: function () {

  },

  onUnload: function () {},

  //分享
  onShareAppMessage: function (res) {
    return {
      title: '资产巡查小程序',
      path: 'pages/index/index'
    }
  },

  //触底加载
  onReachBottom: function () {

  },

  //#endregion


  //加载资产
  getProperties: function () {
    var that = this;

    // string query = "", string sort = "", int pageSize = 15, int pageIndex = 1,
    var url = app.globalData.apiUrl + 'Properties/AllForWechat?' + 'pageSize=' + that.data.page.pageSize + "&pageIndex=" + that.data.page.index + "&time=" + Date.parse(new Date());
    app.requestWithToken({
      url: url,
    }).then(function (res) {
      var response = res.data;

      if (response.data.length > 0) {

        var newProperties = that.data.properties.concat(response.data);

        that.setData({
          properties: newProperties
        })
      } else {

      }
    });
  },

  //导航至详情页面
  navToDetail: function (event) {
    console.log(event);
    wx.navigateTo({
      url: '../details/details?pId=' + event.currentTarget.dataset.id
    })
  }
})