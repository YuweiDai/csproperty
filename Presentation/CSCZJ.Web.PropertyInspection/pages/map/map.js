const markers = [{
  id: 10,
  latitude: 28.9013820000,
  longitude: 118.5111880000,
  width: 50,
  height: 50,
}];

const includePoints = [{
  latitude: 28.9008090000,
  longitude: 118.5115590000,
}];

Page({
  data: {
    scale: 14,
    longitude: 120.131441,
    latitude: 30.279383,
    markers,
    includePoints,
    // controls: [{
    //   id: 5,
    //   position: {
    //     left: 0,
    //     top: 300 - 50,
    //     width: 50,
    //     height: 50,
    //   },
    //   clickable: true,
    // }],
  },
  onReady() {
    // 使用 wx.createMapContext 获取 map 上下文
    this.mapCtx = wx.createMapContext('map');
  },
  getCenterLocation() {
    this.mapCtx.getCenterLocation({
      success: (res) => {
        wx.alert({
          content: 'longitude:' + res.longitude + '\nlatitude:' + res.latitude,
        });
        console.log(res.longitude);
        console.log(res.latitude);
      },
    });
  },
  moveToLocation() {
    this.mapCtx.moveToLocation()
  },
  regionchange(e) {
    console.log('regionchange', e);
  },
  markertap(e) {
    console.log('marker tap', e);
  },
  controltap(e) {
    console.log('control tap', e);
  },
  tap() {
    console.log('tap:');
  },
  callouttap(e) {
    console.log('callout tap', e);
  },
  changeScale() {
    this.setData({
      scale: 8,
    });
  },
  restoreMarkers() {
    this.setData({
      markers,
      includePoints,
    });
  },
  changeCenter() {
    this.setData({
      longitude: 113.324520,
      latitude: 23.199994,
    });
  },
  setHeight() {
    this.setData({
      heightStyle: 'height:30px',
    });
  },
  changeMarkers() {
    this.setData({
      markers: [{
        id: 10,
        latitude: 30.538285,
        longitude: 104.074074,
        width: 50,
        height: 50,
      }],
      includePoints: [{
        latitude: 30.538285,
        longitude: 104.074074,
      }],
    });
  },
});
