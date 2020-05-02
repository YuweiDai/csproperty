using CSCZJ.Core.Domain.Panoramas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Services.Messages
{
    public interface IHotspotService
    {

        void InsertHotspot(Hotspot hotspot);

        void UpdateHotspot(Hotspot hotspot);

        void DeleteHotspot(Hotspot hotspot);

        Hotspot GetHotspotById(int id);

    }
}
