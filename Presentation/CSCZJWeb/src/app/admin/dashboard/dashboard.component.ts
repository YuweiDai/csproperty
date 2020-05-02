import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NgxEchartsModule } from 'ngx-echarts';
import { NzNotificationService } from 'ng-zorro-antd';
import { Router } from '@angular/router';
import * as echarts from 'echarts';
@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.less']
})
export class DashboardComponent implements OnInit {
  chartOption: object;
  linkoption: object;
  linkoption1: object;
  barOption: object;
  mapOption: object;
  mapLoaded = false;
  isVisible = false;
  isOkLoading = false;
  isClick = false;
  showloading=false;  //新增加，原来没

  constructor(private http: HttpClient, private notification: NzNotificationService, private router: Router) { }


  ngOnInit() {
    //加载叠域图
    this.chartOption = {
      title: {
        text: '单位出租面积堆叠区域图',
        x: 'center'
      },
      tooltip: {
        trigger: 'axis'
      },
      legend: {
        data: ['公路管理局', '道路运输管理所', '人力资源和社会保障局', '交通运输局', '粮食收储公司'],
        right: '5%'
      },
      toolbox: {
        feature: {
          saveAsImage: {}
        }
      },
      grid: {
        left: '0%',
        right: '0%',
        bottom: '3%',
        containLabel: true
      },
      xAxis: [
        {
          type: 'category',
          boundaryGap: false,
          data: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月']
        }
      ],
      yAxis: [
        {
          type: 'value'
        }
      ],
      series: [
        {
          name: '公路管理局',
          type: 'line',
          stack: '总量',
          areaStyle: { normal: {} },
          data: [120, 132, 101, 134, 90, 230, 210, 80, 100, 110, 150, 320]
        },
        {
          name: '道路运输管理所',
          type: 'line',
          stack: '总量',
          areaStyle: { normal: {} },
          data: [220, 182, 191, 234, 290, 330, 310, 500, 200, 100, 120, 130]
        },
        {
          name: '人力资源和社会保障局',
          type: 'line',
          stack: '总量',
          areaStyle: { normal: {} },
          data: [150, 232, 201, 154, 190, 330, 410, 100, 110, 120, 130, 150]
        },
        {
          name: '交通运输局',
          type: 'line',
          stack: '总量',
          areaStyle: { normal: {} },
          data: [320, 332, 301, 334, 390, 330, 320, 200, 240, 210, 200, 260]
        },
        {
          name: '粮食收储公司',
          type: 'line',
          stack: '总量',
          label: {
            normal: {
              show: true,
              position: 'top'
            }
          },
          areaStyle: { normal: {} },
          data: [820, 932, 901, 934, 1290, 1330, 1320, 1000, 1500, 1200, 1280, 1520]
        }
      ]
    };

    //加载柱状图
    this.linkoption = {
      title: {
        text: '单位房产面积柱形图',
        x: 'center'
      },
      color: ['#3398DB'],
      //气泡提示框，常用于展现更详细的数据
      tooltip: {
        trigger: 'axis',
        axisPointer: { // 坐标轴指示器，坐标轴触发有效
          type: 'shadow' // 默认为直线，可选为：'line' | 'shadow'
        }
      },
      toolbox: {
        show: true,
        feature: {
          //显示缩放按钮
          dataZoom: {
            show: true
          },
          //显示折线和块状图之间的切换
          magicType: {
            show: true,
            type: ['bar', 'line']
          },
          //显示是否还原
          restore: {
            show: true
          },
          //是否显示图片
          saveAsImage: {
            show: true
          }
        }
      },
      grid: {
        left: '3%',
        right: '4%',
        bottom: '3%',
        containLabel: true
      },
      xAxis: [{
        type: 'category',
        data: ['交通运输局', '公路管理局', '运输管理所', '人力社保社保局'],
        axisTick: {
          alignWithLabel: true
        },
        axisLabel: {
          interval: 0,
          rotate: 20
        },
      }],
      yAxis: [{
        name: "房产总面积",
        type: 'value'
      }],
      series: [{
        name: '房产总面积',
        type: 'bar',
        barWidth: '40%',
        label: {
          normal: {
            show: true
          }
        },
        data: [21231, 1212, 21231, 3213]
      }]
    };

    //加载柱状图1
    this.linkoption1 = {
      title: {
        text: '单位土地面积柱形图',
        x: 'center'
      },
      color: ['#30a0aa'],
      //气泡提示框，常用于展现更详细的数据
      tooltip: {
        trigger: 'axis',
        axisPointer: { // 坐标轴指示器，坐标轴触发有效
          type: 'shadow' // 默认为直线，可选为：'line' | 'shadow'
        }
      },
      toolbox: {
        show: true,
        feature: {
          //显示缩放按钮
          dataZoom: {
            show: true
          },
          //显示折线和块状图之间的切换
          magicType: {
            show: true,
            type: ['bar', 'line']
          },
          //显示是否还原
          restore: {
            show: true
          },
          //是否显示图片
          saveAsImage: {
            show: true
          }
        }
      },
      grid: {
        left: '3%',
        right: '4%',
        bottom: '3%',
        containLabel: true
      },
      xAxis: [{
        type: 'category',
        data: ['交通运输局', '公路管理局', '运输管理所', '人力社保社保局'],
        axisTick: {
          alignWithLabel: true
        },
        axisLabel: {
          interval: 0,
          rotate: 20
        },
      }],
      yAxis: [{
        name: "房产总面积",
        type: 'value'
      }],
      series: [{
        name: '房产总面积',
        type: 'bar',
        barWidth: '40%',
        label: {
          normal: {
            show: true
          }
        },
        data: [11000, 25421, 1500, 5632]
      }]
    };
    //加载饼状图
    this.barOption = {
      title: {
        text: '使用现状饼状图',
        subtext: '单位为资产数量',
        x: 'center'
      },
      tooltip: {
        trigger: 'item',
        formatter: '{a} <br/>{b} : {c} ({d}%)'
      },
      legend: {
        x: 'center',
        y: 'bottom',
        data: ['自用', '出租', '出借', '拆除', '调配使用', '闲置']
      },
      calculable: true,
      series: [
        {
          name: '使用现状',
          type: 'pie',
          radius: [50, 110],
          roseType: 'area',
          data: [
            { value: 350, name: '自用' },
            { value: 250, name: '出租' },
            { value: 150, name: '出借' },
            { value: 50, name: '拆除' },
            { value: 200, name: '调配使用' },
            { value: 100, name: '闲置' }

          ]
        }
      ]
    };
    console.log(123);
    //加载地图图表
    this.http.get('../../../../assets/js/CS.json').subscribe(geoJson => {

      this.mapLoaded = true;
      // register map:
      echarts.registerMap('常山', geoJson);

      this.mapOption = {
        title: {
          text: '常山县乡镇资产区域分布（建筑面积）',
          x: 'center'
        },
        tooltip: {
          trigger: 'item',
          formatter: '{b}<br/>{c} (平方米)'
        },
        toolbox: {
          show: true,
          orient: 'vertical',
          left: 'right',
          top: 'center',
          feature: {
            dataView: { readOnly: false },
            restore: {},
            saveAsImage: {}
          }
        },
        visualMap: {
          min: 800,
          max: 50000,
          text: ['High', 'Low'],
          realtime: false,
          calculable: true,
          inRange: {
            color: ['lightskyblue', 'yellow', 'orangered']
          }
        },
        series: [
          {
            name: '常山乡镇区域分布',
            type: 'map',
            mapType: '常山', // map type should be registered
            itemStyle: {
              normal: { label: { show: true } },
              emphasis: { label: { show: true } }
            },
            data: [
              { name: '白石镇', value: 20057.34 },
              { name: '天马街道', value: 15477.48 },
              { name: '同弓乡', value: 31686.1 },
              { name: '球川镇', value: 6992.6 },
              { name: '金川街道', value: 44045.49 },

              { name: '青石镇', value: 20057.34 },
              { name: '紫港街道', value: 15477.48 },
              { name: '招贤镇', value: 31686.1 },
              { name: '何家乡', value: 6992.6 },
              { name: '大桥头乡', value: 44045.49 },

              { name: '辉埠镇', value: 6992.6 },
              { name: '东案乡', value: 44045.49 },
              { name: '新昌乡下', value: 6992.6 },
              { name: '新昌乡上', value: 44045.49 },
              { name: '芳村镇', value: 44045.49 }
            ],
            nameMap: {
              '白石镇': '白石镇',
              '天马街道': '天马街道',
              '同弓乡': '同弓乡',
              '球川镇': '球川镇',
              '金川街道': '金川街道',
              '青石镇': '青石镇',
              '紫港街道': '紫港街道',
              '招贤镇': '招贤镇',
              '何家乡': '何家乡',
              '大桥头乡': '大桥头乡',
              '辉埠镇': '辉埠镇',
              '东案乡': '东案乡',
              '新昌乡下': '新昌乡下',
              '新昌乡上': '新昌乡上',
              '芳村镇': '芳村镇'
            }
          }
        ]

      }


    })

    if (this.isClick == false) {

      setTimeout(() => {
        this.isVisible = true;

      }, 3000);
    }



  }

  handleOk(): void {
    this.isOkLoading = true;
    setTimeout(() => {
      this.isVisible = false;
      this.isOkLoading = false;
    }, 1000);
    this.router.navigate(['/admin/properties/rentlist'], { queryParams: { id: 1 } });
    this.isClick = true;
  }

  handleCancel(): void {
    this.isVisible = false;
  }

}
