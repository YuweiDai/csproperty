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
    },
    account: {}
  },

  //#region 页面事件集合

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var that = this;
    var wHeight = app.globalData.deviceInfo.windowHeight; //窗体高度
    var scrollHeight = wHeight - 54 - 1 - 120 / app.globalData.dpr;
    console.log(scrollHeight);
    that.setData({
      'layout.scrollHeight': scrollHeight
    });
    // 自定义加载图标
    Toast.loading({
      duration: 0,
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

        if (response.data == undefined || response.data == null) {
          that.setData({
            'initial.accountLoading': false
          });

          //#region  若为状态0则表示当前微信用户未绑定，专跳绑定页面
          wx.reLaunch({
            url: '../register/register',
          })
          //#endregion
        }

        that.setData({
          'account': response.data
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

  //#endregion

  //解绑微信账号
  unbindFinder: function () {

    app.wxp.showModal({
      title: '提示',
      content: '是否要解除与当前微信账号的绑定？'
    }).then(function (res) {
      if (res.confirm) {

        Toast.loading({
          duration: 0,
          mask: true,
          message: '解绑中...',
          forbidClick: true
        });

        return app.requestWithToken({
          url: app.globalData.apiUrl + "Systemmanage/Accounts/UnBinding",
          method: 'POST'
        });
      }
    }).then(function (res) {
      var response = res.data;
      if (response.code == "200") {
        wx.reLaunch({
          url: 'index'
        });

        Toast.clear();
      } else {
        Notify({
          message: response.message,
          duration: 2000
        });
        Toast.clear();
      }
    }, function () {
      Toast.clear();
    });
  },

  //资产列表滑动到底部
  scrolltolower: function (event) {
    console.log(event);
    this.setData({
      'page.index': this.data.page.index + 1
    });

    this.getProperties();
  },

  //资产搜索
  searchProperties: function (e) {
    console.log(e);
    this.setData({
      'page.index': 1,
      'page.pageSize': 15,
      'page.query': e.detail
    });

    this.getProperties(true);
  },

  //加载资产
  getProperties: function (reset = false) {
    var that = this;
    // 自定义加载图标
    var url = app.globalData.apiUrl + 'Properties/AllForWechat?' + 'pageSize=' + that.data.page.pageSize + "&pageIndex=" + that.data.page.index + "&time=" + Date.parse(new Date());
    Toast.loading({
      duration: 0,
      mask: true,
      message: '加载中...',
      forbidClick: true
    });
    if (that.data.page.query) url += '&query=' + that.data.page.query;

    app.requestWithToken({
      url: url,
    }).then(function (res) {
      var response = res.data;
      Toast.clear();

      var newProperties = reset ? response.data : that.data.properties.concat(response.data);

      that.setData({
        properties: newProperties
      })
    }, function (err) {
      Notify({
        type: 'danger',
        message: '获取资产错误！',
        duration: 2000
      });
      Toast.clear();
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