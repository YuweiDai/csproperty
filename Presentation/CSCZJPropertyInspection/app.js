//app.js
import {
  promisifyAll,
  promisify
} from 'miniprogram-api-promise';

App({
  wxp: {},
  globalData: {
    apiUrl: "http://localhost:8844/api/",
    // apiUrl: "http://qzgis.vaiwan.com/api/",
    // apiUrl: "https://www.qzgis.cn/dapi/",
    userInfo: null,
    deviceInfo: null,
    dpr: 2,
    clientHeight: 0,
    clientWidth: 0
  },

  onLaunch: function () {
    var that = this;

    console.log("app.js 开启wx to wxp");
    // promisify all wx's api
    promisifyAll(wx, this.wxp);

    wx.clearStorageSync(); //清楚缓存

    ///采用同步方式获取设备信息
    const res = wx.getSystemInfoSync();
    that.globalData.deviceInfo = res;
    that.globalData.dpr = 750 / res.windowWidth;
    that.globalData.clientHeight = res.windowHeight;
    that.globalData.clientWidth = res.windowWidth;

    // 获取用户信息
    wx.getSetting({
      success: res => {
        if (res.authSetting['scope.userInfo']) {
          // 已经授权，可以直接调用 getUserInfo 获取头像昵称，不会弹框
          wx.getUserInfo({
            success: res => {
              // 可以将 res 发送给后台解码出 unionId
              this.globalData.userInfo = res.userInfo

              // 由于 getUserInfo 是网络请求，可能会在 Page.onLoad 之后才返回
              // 所以此处加入 callback 以防止这种情况
              if (this.userInfoReadyCallback) {
                this.userInfoReadyCallback(res)
              }
            }
          })
        }
      }
    })
  },

  //验证本地存储的token是否可用
  validateToken: function () {
    var validation = true;

    //小程序登录，存储token
    var token = wx.getStorageSync('token');
    var expiredTime = wx.getStorageSync('expiredTime');

    if (!token) validation = false;
    else {
      if (!expiredTime) validation = false;
      else {
        var current = new Date();
        if (current >= expiredTime) validation = false;
      }
    }

    return validation;
  },

  //微信登录，获取token
  getToken: function () {
    var that = this;

    console.log("getToken")

    // 用户登录
    return that.wxp.login().then(function (res) {
      if (res.code) {
        //发起网络请求
        return that.wxp.request({
          url: that.globalData.apiUrl + 'Auth/Wechat',
          data: {
            code: res.code
          }
        });
      } else {
        console.log('登录失败！' + res.errMsg)
      }
    }).then(function (res) {
      //#region 处理获取的token
      var response = res.data;
      if (response.code == "200") {
        var token = response.data.token;
        var days = response.data.days;
        var current = new Date();
        var expiredTime = new Date(current.setDate(current.getDate() + days));
        console.log(expiredTime)

        wx.setStorageSync('token', token);
        wx.setStorageSync('expiredTime', expiredTime);
      }
      //#endregion
    }, function (err) {
      wxp.showToast({
        title: '获取Token发送错误！',
      });
    });
  },

  //发送请求附带登录token
  requestWithToken: function (paramas) {
    var that = this;

    //#region 封装http请求，自动附带token值
    var requestFn = function () {
      var token = wx.getStorageSync('token');
      var newParams = Object.assign({}, paramas);

      if (!(Object.prototype.toString.call(newParams.header) === '[object Object]')) {
        newParams.header = {
          'Authorization': 'Bear ' + token
        }
      } else {
        var newHeader = Object.assign(newParams.header, {
          'Authorization': 'Bear ' + token
        })
        newParams.header = newHeader;
      }

      return that.wxp.request(newParams);
    };
    //#endregion

    if (this.validateToken()) {
      return requestFn();
    } else {
      //无效先获取token，再请求
      return that.getToken(paramas).then(function () {
        return requestFn();
      }, function () {});
    }
  },

  //上传文件附带登录token
  uploadWithToken: function (paramas) {
    var that = this;

    if (this.validateToken()) {
      //#region token有效直接请求，附带token
      var token = wx.getStorageSync('token');
      var newParams = Object.assign({}, paramas);

      if (!(Object.prototype.toString.call(newParams.header) === '[object Object]')) {
        newParams.header = {
          'Authorization': 'Bear ' + token
        }
      } else {
        var newHeader = Object.assign(newParams.header, {
          'Authorization': 'Bear ' + token
        })
        newParams.header = newHeader;
      }
      return that.wxp.uploadFile(newParams);
    } else {
      //无效先获取token，再请求
      return that.getToken(paramas);
    }
  }
})


// .then(function (request) {

// });