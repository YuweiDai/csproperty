const app = getApp();
import Notify from '../../miniprogram_npm/@vant/weapp/notify/notify';
import Toast from '../../miniprogram_npm/@vant/weapp/toast/toast';
import Dialog from '../../miniprogram_npm/@vant/weapp/dialog/dialog';

Page({
  /**
   * 页面的初始数据
   */
  data: {
    layout: {
      registerFormsHeight: 200,
      scrolViewHeight: 300
    },
    userInfo: null,
    hasUserInfo: false,
    canIUse: wx.canIUse('button.open-type.getUserInfo')
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {

    var that = this;

    var wHeight = app.globalData.deviceInfo.windowHeight; //窗体高度
    var registerFormsHeight = wHeight - (10 * 2 + 420) / app.globalData.dpr;

    that.setData({
      'layout.registerFormsHeight': registerFormsHeight,
      'layout.scrolViewHeight': registerFormsHeight - 49
    });

    //#region 获取用户信息部分代码

    if (app.globalData.userInfo) {
      this.setData({
        userInfo: app.globalData.userInfo,
        hasUserInfo: true
      })
    } else if (this.data.canIUse) {
      // 由于 getUserInfo 是网络请求，可能会在 Page.onLoad 之后才返回
      // 所以此处加入 callback 以防止这种情况
      app.userInfoReadyCallback = res => {
        this.setData({
          userInfo: res.userInfo,
          hasUserInfo: true
        })
      }
    } else {
      // 在没有 open-type=getUserInfo 版本的兼容处理
      wx.getUserInfo({
        success: res => {

          app.globalData.userInfo = res.userInfo
          this.setData({
            userInfo: res.userInfo,
            hasUserInfo: true
          })
        }
      })
    }
    //#endregion
  },

  //表单提交
  submitForm: function (e) {
    var that = this;

    var name = e.detail.value.fname;
    var password = e.detail.value.fnumb;
    that.bindFinder(name, password);
  },

  //绑定学生
  bindFinder: function (name, password) {
    var that = this;

    if (that.data.userInfo == null) {
      Toast.fail('请点击授权获取用户头像昵称！');
      return;
    }

    if (name == undefined || name == null || name == '') {
      Toast('请输入要绑定的账号！');
      return;
    }

    if (password == undefined || password == null || password == '') {
      Toast('请输入要绑定账号的密码！');
      return;
    }

    //弹出确认信息对话框
    Dialog.confirm({
      title: "请确认您绑定的账号信息",
      message: "账号：" + name
    }).then(function () {
      //后台进行关联    
      var accountBindingModel = {
        userName: name,
        password: password,
        nickName: that.data.userInfo.nickName,
        avatarUrl: that.data.userInfo.avatarUrl
      }

      console.log(accountBindingModel);

      Toast.loading({
        duration: 0,
        mask: true,
        message: '绑定中...',
        forbidClick: true
      });

      app.requestWithToken({
        url: app.globalData.apiUrl + 'Systemmanage/Accounts/binding',
        method: 'POST',
        data: JSON.stringify(accountBindingModel)
      }).then(function (res) {
        Toast.clear();
        var response = res.data;

        if (response.code != "200") {
          Dialog.alert({
            title: '绑定失败',
            message: response.message
          });
        } else {
          Notify({
            type: 'success',
            message: response.message,
            duration: 1500
          });

          wx.reLaunch({
            url: '../index/index',
          })
        }
        console.log(res)
      }, function (err) {
        Toast.clear();
      });

    }).catch(function () {
      console.log("核对信息，取消了绑定操作！")
    });
  },

  //获取微信用户信息
  getUserInfo: function (e) {
    if (e.detail.userInfo) {
      app.globalData.userInfo = e.detail.userInfo;
      this.setData({
        userInfo: e.detail.userInfo,
        hasUserInfo: true
      });
    }
  }
})