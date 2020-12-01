// pages/index/index.js
// import Dialog from '../../miniprogram_npm/@vant/weapp/dialog/dialog';
import Notify from '../../miniprogram_npm/@vant/weapp/notify/notify';
import Toast from '../../miniprogram_npm/@vant/weapp/toast/toast';

const util = require('../../utils/util');
const app = getApp();

Page({

  //#region 页面事件集合

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var that = this;

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
      var response = res.data;
      if (response.code == "200") {

        if (response.data == "0" || response.data == "1") {
          that.setData({
            'initial.accountLoading': false
          });
          Toast.clear();
          //#region  若为状态0，1 则表示当前微信用户未绑定，专跳绑定页面
          wx.navigateTo({
            url: '../register/register?active=' + response.data,
          })
          //#endregion
        }

        that.setData({
          'account.status': response.data
        });

        //#region 加载checkinpoints数据
        app.requestWithToken({
          url: app.globalData.apiUrl + 'checkinpoints',
          header: {
            'content-type': 'application/json' // 默认值
          }
        }).then(function (res) {
          var response = res.data;
          if (response.code == "200") {
            var checkInPoints = response.data;

            //重新计算位置
            checkInPoints.forEach(function (item, index) {
              item.x = item.x * (app.globalData.clientWidth / 1080);
              item.y = item.y * (app.globalData.clientHeight / 1920);
              item.width = (app.globalData.clientWidth / 1080) * item.width;
              item.height = (app.globalData.clientHeight / 1920) * item.height;
            });

            that.setData({
              'initial.checkInPointsLoading': false,
              'checkInPoints': checkInPoints
            });

            if (!that.data.initial.accountLoading && !that.data.initial.checkInPointsLoading) {
              Toast.clear();
              //启动计算
              that.startCalculation();
            }
          } else {
            Notify({
              message: '获取信打卡点信息失败!',
              duration: 2000
            });

            Toast.clear();
          }
        }, function (err) {});
        //#endregion

        //#region 加载系统配置

        app.requestWithToken({
          url: app.globalData.apiUrl + 'Common/LoadSystemConfig',
        }).then(function (res) {
          var response = res.data;
          that.setData({
            'config.mock': response.data == "TRUE"
          })
        }, function (err) {});

        //#endregion
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

})