var geoUtil = require('../../utils/WSCoordinate.js')
import Notify from '../../miniprogram_npm/@vant/weapp/notify/notify';
import Toast from '../../miniprogram_npm/@vant/weapp/toast/toast';

var app = getApp();
const util = require('../../utils/util');
var fromInitialData = false;
Page({
  data: {
    scrollHeight: 500,
    activeMoreInfoNav: 0,
    property: null,
    location: [],
    currentDist: 0,
    patrolDialogVisible: false,
    patrol: {
      message: "",
      imgs: []
    }
  },

  onLoad(query) {
    var that = this;
    var propertyId = 0;
    console.log(query);
    try {
      propertyId = parseInt(query.pId);
      if (isNaN(propertyId)) propertyId = 0;
    } catch (e) {
      propertyId = 0;
    }

    if (propertyId == 0) {
      wx.reLaunch({
        url: '../index/index',
      })
    }

    // 自定义加载图标
    Toast.loading({
      duration: 10000,
      mask: true,
      message: '加载中...',
      forbidClick: true
    });

    var url = app.globalData.apiUrl + 'Properties/' + propertyId;

    app.requestWithToken({
      url: url,
    }).then(function (res) {
      var response = res.data;
      if (response) {

        var lnglat = [];
        if (response.location) {
          //POINT (118.51692642056332 28.905588892905769)
          var coordStrs = response.location.replace('POINT (', '').replace(')', '').split(' ');
          lnglat.push(parseFloat(coordStrs[0]));
          lnglat.push(parseFloat(coordStrs[1]));
        }
        that.setData({
          property: response,
          location: lnglat
        });

        that.calculateDistance();
      }

      Toast.clear();
    });

    console.log(app.globalData.deviceInfo.windowHeight - 102 / app.globalData.dpr);

    this.setData({
      scrollHeight: app.globalData.deviceInfo.windowHeight - 102 / app.globalData.dpr,
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


  navToMap: function () {

    if (this.data.location.length != 2) return;

    var targetLocation = geoUtil.transformFromWGSToGCJ(this.data.location[1], this.data.location[0]);
    wx.openLocation({
      latitude: targetLocation.latitude,
      longitude: targetLocation.longitude,
    });
  },

  //显示巡查输入对话框
  sigIn: function (event) {
    this.setData({
      patrolDialogVisible: true
    })
  },

  onPatrolDialogClose() {
    this.setData({
      patrolDialogVisible: false
    })
  },

  //巡查意见变化
  onPatrolMsgChange: function (event) {
    this.setData({
      'patrol.message': event.detail
    });
  },

  //提交巡查
  submitPatrol: function () {
    var that = this;

    if (!this.data.patrol.message) {
      Toast('请输入巡查情况！');
      return;
    }

    if (this.data.patrol.imgs.length == 0) {
      Toast('请上传至少一张现场照片！');
      return;
    }

    //照片转换成base64

    var base64Array = [];
    this.data.patrol.imgs.forEach(function (item, index) {
      var filePath = item.url;
      var base64 = wx.getFileSystemManager().readFileSync(filePath, "base64");
      base64Array.push(base64);
    })

    var patrolCreateModel = {
      content: this.data.patrol.message,
      patrolPictures: base64Array,
      property_Id: this.data.property.id
    };

    //异步提示
    Toast.loading({
      duration: 0,
      mask: true,
      message: '提交中...',
      forbidClick: true
    });

    app.requestWithToken({
      url: app.globalData.apiUrl + 'properties/Patrol/Create',
      method: 'POST',
      data: JSON.stringify(patrolCreateModel)
    }).then(function (res) {
      var response = res.data;
      Notify({
        type: 'success',
        message: '巡查打卡成功！',
        duration: 2000
      });

      var property = that.data.property;
      property.patrols.unshift(response);

      //清空数据
      that.setData({
        property: property,
        patrol: {
          message: '',
          imgs: []
        }
      })

      Toast.clear();
    }, function (err) {
      Toast.clear();
    });
  },

  //预览图片
  previewImg: function (event) {

    var href = event.currentTarget.dataset.href;
    if (href) {
      wx.previewImage({
        current: href, // 当前显示图片的http链接
        urls: [href] // 需要预览的图片http链接列表
      })
    }
  },

  //上传
  healthCodeAfterRead: function (event) {
    const {
      file
    } = event.detail;

    var fileList = this.data.patrol.imgs;
    fileList.push({
      ...file,
      url: file.url
    });
    this.setData({
      'patrol.imgs': fileList
    });
  },
  //删除
  healthCodeDelete: function (event) {
    console.log(event)
    const {
      index
    } = event.detail;

    var fileList = this.data.patrol.imgs;
    fileList.splice(index, 1);
    this.setData({
      'patrol.imgs': fileList
    });
  },


  getUserLocation: function () {
    return app.wxp.getLocation({
      type: 'wgs84',
    }).then(function (res) {
      return res;
    }, function () {
      Notify({
        type: 'danger',
        message: '请点击小程序右上角【...】按钮，进入小程序设置，开启【使用我的地理位置】授权！。',
        duration: 2000
      });
      return {
        latitude: 0,
        longitude: 0,
        accuracy: 0
      };
    });
  },

  //计算距离
  calculateDistance: function () {
    var that = this;
    if (that.data.location.length != 2) return;

    that.getUserLocation().then(function (res) {
        var userLng = res.longitude;
        var userLat = res.latitude;
        var accuracy = res.accuracy;
        console.log("定位精度：" + accuracy);
        var distance = that.calculateDistanceBetweenTwoPoints(userLat, userLng, that.data.location[1], that.data.location[0]);
        that.setData({
          currentDist: parseInt(distance)
        })
      },
      function () {})
  },

  //计算两个点之间的位置，返回距离值
  calculateDistanceBetweenTwoPoints: function (lat1, lng1, lat2, lng2) {
    lat1 = lat1 || 0;
    lng1 = lng1 || 0;
    lat2 = lat2 || 0;
    lng2 = lng2 || 0;

    var rad1 = lat1 * Math.PI / 180.0;
    var rad2 = lat2 * Math.PI / 180.0;
    var a = rad1 - rad2;
    var b = lng1 * Math.PI / 180.0 - lng2 * Math.PI / 180.0;
    var r = 6378137;
    var distance = r * 2 * Math.asin(Math.sqrt(Math.pow(Math.sin(a / 2), 2) + Math.cos(rad1) * Math.cos(rad2) * Math.pow(Math.sin(b / 2), 2)));

    return parseInt(distance);
  },
});