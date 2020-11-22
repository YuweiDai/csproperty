using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCZJ.API.Models.Statistics
{
    public class OverviewStatistics
    {
        public int TotalCount { get; set; }

        public int ConstructCount { get; set; }

        public int LandCount { get; set; }

        public double TotalPrice { get; set; }

        public double ConstructPrice { get; set; }

        public double LandPrice { get; set; }

        public double ConstructArea { get; set; }

        public double ConstrcutLandArea { get; set; }

        public double LandArea { get; set; }

        public List<GovermentRent> GRentList { get; set; }
        public UsingState UsingState { get; set; }

        public List<int> TownNumber { get; set; }
    }

    public class GovermentRent {
        public string GName { get; set; }
        public List<int> RentPrice { get; set; }

        public double TotalHouseArea { get; set; }
        public double TotalLandArea { get; set; }

    }

    public class UsingState {

        public double ZYTatol { get; set; }
        public double CCTatol { get; set; }
        public double CZTatol { get; set; }

        public double CJTatol { get; set; }
        public double DPSYTatol { get; set; }
        public double XZTatol { get; set; }
    }
}