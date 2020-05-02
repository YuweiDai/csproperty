using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QZCHY.PanoramaQuzhou.Core
{
    /// <summary>
    /// 地理空间计算辅助类
    /// </summary>
    public class GeographyHelper
    {

        /// <summary>
        /// 求赤道半径(单位m)
        /// </summary>
        private const double EARTH_RADIUS = 6378137;


        #region 私有方法
        private static double HaverSin(double theta)
        {
            var v = Math.Sin(theta / 2);
            return v * v;
        }
        /// <summary>
        /// (lat1,lng1)是全景图坐标；(lat2,lng2)是定位坐标
        /// </summary>
        /// <param name="lat1">全景图纬度</param>
        /// <param name="lng1">全景图经度</param>
        /// <param name="lat2">定位位置的纬度</param>
        /// <param name="lng2">定位位置的经度</param>
        /// <returns></returns>
        /// 
        private static double ConvertDegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        } 
        #endregion

        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            //经纬度转换成弧度
            lat1 = ConvertDegreesToRadians(lat1);
            lat2 = ConvertDegreesToRadians(lat2);
            lng1 = ConvertDegreesToRadians(lng1);
            lng2 = ConvertDegreesToRadians(lng2);
            //求差值
            var vLat = Math.Abs(lat1 - lat2);
            var vlng = Math.Abs(lng1 - lng2);
            var h = HaverSin(vLat) + Math.Cos(lat1) * Math.Cos(lat2) * HaverSin(vlng);
            var distance = 2 * EARTH_RADIUS * Math.Asin(Math.Sqrt(h));
            return distance;
        }
    }
}
