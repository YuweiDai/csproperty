var app = getApp();
Page({
  data: {
    locationId: 0,
    sceneName: "",
    targetUrl: ""
  },
  onLoad(query) {
    var locationId = query.id;
    var sceneName = query.sname;
    var newUrl = app.globalData.webUrl + "?id=" + locationId;
    console.log(query);
    this.setData({
      locationId: locationId,
      targetUrl: newUrl,
      sceneName: sceneName
    });
  }
});
