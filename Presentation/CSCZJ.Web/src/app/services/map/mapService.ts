import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import 'leaflet';
import 'leaflet-draw';
import 'wicket';

declare var L: any; //leaflet
declare var Wkt: any;

@Injectable({
  providedIn: 'root'
})
export class MapService {

  config: {
    center: number[];
    level: number;
    propertyZoomLevel: number,
    indoorMapLevel: number
  }

  wktTools: any;

  styles: {
    property: any;
    propertyUnit: any;
    propertyUnitSelected: any;
    cells: {
      matchStyle: any;
      officeStyle: any;
      lendStyle: any;
      rentStyle: any;
      idelStyle: any;
    }
  }

  constructor(private http: HttpClient) {

    this.config = {
      center: [28.905527517199516, 118.50629210472107],
      level: 12,
      propertyZoomLevel: 17,
      indoorMapLevel: 22
    };

    //wkt工具初始化
    this.wktTools = new Wkt.Wkt();

    this.styles = {
      property:
      {
        stroke: true,
        color: '#FF0000',
        dashArray: "15,15",
        dashSpeed: 15,
        weight: 4,
        opacity: 1,
        fill: false
      },
      propertyUnit:
      {
        stroke: true,
        color: '#6666FF',
        weight: 2,
        opacity: 1,
        fill: true,
        fillColor: '#CCCCCC',
        fillOpacity: 0.5
      },
      propertyUnitSelected:
      {
        stroke: true,
        color: '#FF3333',
        weight: 5,
        opacity: 1,
        fill: true,
        fillColor: '#FF3333',
        fillOpacity: 0.5
      },
      cells: {
        matchStyle: {
          stroke: true,
          color: '#000000',
          weight: 0.8,
          opacity: 1,
          fill: true,
          fillColor: '#CCCCCC',
          fillOpacity: 0.8
        },
        officeStyle: {
          stroke: true,
          color: '#000000',
          weight: 0.8,
          opacity: 1,
          fill: true,
          fillColor: '#9933FF',
          fillOpacity: 0.8
        },
        lendStyle: {
          stroke: true,
          color: '#000000',
          weight: 0.8,
          opacity: 1,
          fill: true,
          fillColor: '#FFFF00',
          fillOpacity: 0.8
        },
        rentStyle: {
          stroke: true,
          color: '#000000',
          weight: 0.8,
          opacity: 1,
          fill: true,
          fillColor: '#3366FF',
          fillOpacity: 0.8
        },
        idelStyle: {
          stroke: true,
          color: '#000000',
          weight: 0.8,
          opacity: 1,
          fill: true,
          fillColor: '#FF3300',
          fillOpacity: 0.8
        },
      }
    }
  }

  //地图初始化
  mapInitialize(map) {
    var normal = this.getLayer("vector");
    var satellite = this.getLayer("img");

    normal.addTo(map);

    var baseMaps = {
      "矢量": normal,
      "影像": satellite
    }

    L.control.layers(baseMaps).addTo(map);

    // var iconLayersControl = new L.Control.Layers(
    //   [
    //     {
    //       title: '矢量', // use any string
    //       layer: normal, // any ILayer
    //       icon: '../../assets/images/iconLayers/sl.png' // 80x80 icon
    //     },
    //     {
    //       title: '影像',
    //       layer: satellite,
    //       icon: '../../assets/images/iconLayers/yx.png'
    //     }
    //   ], {
    //   position: 'bottomright'
    // }
    // );

    var zoomControl = map.zoomControl;
    zoomControl.setPosition("bottomleft");
  }

  //增加编辑工具条
  addEditControl(map: any, editableLayers: any) {
    var options = {
      position: 'topleft',
      draw: {
        polyline: false,
        polygon: true,
        circle: false,
        rectangle: false,
        marker: false,
        circlemarker: false
      },
      edit: {
        featureGroup: editableLayers, //REQUIRED!!
        remove: true
      }
    };

    var drawControl = new L.Control.Draw(options);
    map.addControl(drawControl);

    //#region 汉化

    // draw: {
    //   toolbar: {
    //     // #TODO: this should be reorganized where actions are nested in actions
    //     // ex: actions.undo  or actions.cancel
    //     actions: {
    //       title: 'Cancel drawing',
    //       text: 'Cancel'
    //     },
    //     finish: {
    //       title: 'Finish drawing',
    //       text: 'Finish'
    //     },
    //     undo: {
    //       title: 'Delete last point drawn',
    //       text: 'Delete last point'
    //     },
    //     buttons: {
    //       polyline: 'Draw a polyline',
    //       polygon: 'Draw a polygon',
    //       rectangle: 'Draw a rectangle',
    //       circle: 'Draw a circle',
    //       marker: 'Draw a marker',
    //       circlemarker: 'Draw a circlemarker'
    //     }
    //   },
    //   handlers: {
    //     circle: {
    //       tooltip: {
    //         start: 'Click and drag to draw circle.'
    //       },
    //       radius: 'Radius'
    //     },
    //     circlemarker: {
    //       tooltip: {
    //         start: 'Click map to place circle marker.'
    //       }
    //     },
    //     marker: {
    //       tooltip: {
    //         start: 'Click map to place marker.'
    //       }
    //     },
    //     polygon: {
    //       tooltip: {
    //         start: 'Click to start drawing shape.',
    //         cont: 'Click to continue drawing shape.',
    //         end: 'Click first point to close this shape.'
    //       }
    //     },
    //     polyline: {
    //       error: '<strong>Error:</strong> shape edges cannot cross!',
    //       tooltip: {
    //         start: 'Click to start drawing line.',
    //         cont: 'Click to continue drawing line.',
    //         end: 'Click last point to finish line.'
    //       }
    //     },
    //     rectangle: {
    //       tooltip: {
    //         start: 'Click and drag to draw rectangle.'
    //       }
    //     },
    //     simpleshape: {
    //       tooltip: {
    //         end: 'Release mouse to finish drawing.'
    //       }
    //     }
    //   }
    // },
    // edit: {
    //   toolbar: {
    //     actions: {
    //       save: {
    //         title: 'Save changes',
    //         text: 'Save'
    //       },
    //       cancel: {
    //         title: 'Cancel editing, discards all changes',
    //         text: 'Cancel'
    //       },
    //       clearAll: {
    //         title: 'Clear all layers',
    //         text: 'Clear All'
    //       }
    //     },
    //     buttons: {
    //       edit: 'Edit layers',
    //       editDisabled: 'No layers to edit',
    //       remove: 'Delete layers',
    //       removeDisabled: 'No layers to delete'
    //     }
    //   },
    //   handlers: {
    //     edit: {
    //       tooltip: {
    //         text: 'Drag handles or markers to edit features.',
    //         subtext: 'Click cancel to undo changes.'
    //       }
    //     },
    //     remove: {
    //       tooltip: {
    //         text: 'Click on a feature to remove.'
    //       }
    //     }
    //   }
    // }


    // #TODO: this should be reorganized where actions are nested in actions
    // ex: actions.undo  or actions.cancel
    // actions: {
    //   title: 'Cancel drawing',
    //   text: 'Cancel'
    // },
    // finish: {
    //   title: 'Finish drawing',
    //   text: 'Finish'
    // },
    // undo: {
    //   title: 'Delete last point drawn',
    //   text: 'Delete last point'
    // },
    // L.drawLocal.draw.toolbar.buttons = {
    //   polyline: 'Draw a polyline',
    //   polygon: '绘制多边形',
    //   rectangle: 'Draw a rectangle',
    //   circle: 'Draw a circle',
    //   marker: 'Draw a marker',
    //   circlemarker: 'Draw a circlemarker'
    // }
    // };

    L.drawLocal.draw.handlers.polygon = {
      tooltip: {
        start: '点击开始绘制多边形',
        cont: '点击继续绘制多边形',
        end: '点击第一个闭合多边形'
      }
    };

    ////#endregion
  }

  //获取指定类型的地图
  getLayer(layerType: string): any {
    var layerGroup = null;

    if (layerType == "img") {

      // 省级影像
      var provinceImg = L.tileLayer("http://ditu.zjzwfw.gov.cn/services/wmts/imgmap/default/oss?token=eeca2129-01a7-441a-8ff9-2a031c1ca84a&service=wmts&request=GetTile&version=1.0.0&LAYER=imgmap&tileMatrixSet=default028mm&TileMatrix={z}&TileRow={y}&TileCol={x}&style=default&format=image/jpgpng", {
        subdomains: ["0", "1", "2", "3", "4", "5", "6", "7"],
        minZoom: 7,
        maxZoom: 18,
        zoomOffset: 1
      });
      var provinceImgAnno = L.tileLayer("http://ditu.zjzwfw.gov.cn/services/wmts/imgmap_lab/default/oss?token=4f3f2d7a-e816-46cc-8333-467f3e71c258&service=wmts&request=GetTile&version=1.0.0&LAYER=TDT_ZJIMGANNO&tileMatrixSet=default028mm&TileMatrix={z}&TileRow={y}&TileCol={x}&style=default&format=image/jpgpng", {
        subdomains: ["0", "1", "2", "3", "4", "5", "6", "7"],
        minZoom: 7,
        maxZoom: 18,
        zoomOffset: 1
      });

      layerGroup = L.layerGroup([provinceImg, provinceImgAnno]);
    }
    else if (layerType == "vector") {
      // 省级矢量
      var provinceVec =
        L.tileLayer("http://ditu.zjzwfw.gov.cn/services/wmts/emap/default/oss?token=2c92920471b56e640171be7444540073&service=wmts&request=GetTile&version=1.0.0&LAYER=TDT_ZJEMAP&tileMatrixSet=default028mm&TileMatrix={z}&TileRow={y}&TileCol={x}&style=default&format=image/jpgpng", {
          subdomains: ["0", "1", "2", "3", "4", "5", "6", "7"],
          minZoom: 7,
          maxZoom: 18,
          zoomOffset: 1
        });
      var provinceVecAnno =
        L.tileLayer("http://ditu.zjzwfw.gov.cn/services/wmts/emap_lab/default/oss?token=2c92920471b56e640171be7537bd0074&service=wmts&request=GetTile&version=1.0.0&LAYER=TDT_ZJEMAPANNO&tileMatrixSet=default028mm&TileMatrix={z}&TileRow={y}&TileCol={x}&style=default&format=image/jpgpng", {
          subdomains: ["0", "1", "2", "3", "4", "5", "6", "7"],
          minZoom: 7,
          maxZoom: 18,
          zoomOffset: 1
        });
      layerGroup = L.layerGroup([provinceVec, provinceVecAnno]);
    }

    return layerGroup;
  }

  //wkt to layer
  wktToLayer(wktStr): any {
    this.wktTools.read(wktStr);
    return this.wktTools.toJson();
  }

  wktStrToObject(wktStr): any {
    this.wktTools.read(wktStr);
    return this.wktTools.toObject();
  }

  //geojson to layer
  geoJsonToWKT(geoJson: any): any {
    return this.wktTools.fromJson(geoJson).write();
  }

  getCellStyleByType(cellType: string): any {
    switch (cellType) {
      case "办公":
        return this.styles.cells.officeStyle;
      case "出借":
        return this.styles.cells.lendStyle;
      case "出租":
        return this.styles.cells.rentStyle;
      case "闲置":
        return this.styles.cells.idelStyle;
      default:
        return this.styles.cells.matchStyle;
    }
  }
}