using CSCZJ.Core.Domain.Panoramas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Services.Messages
{
    public interface ILocationService
    {

        PanoramaLocation GetLocationById(int id);

        PanoramaLocation GetLocationByName(string name);

        PanoramaLocation GetLocationBySceneId(int sceneId);




        IQueryable<PanoramaLocation> GetAllLocations();
        




        void InsertPanoramaLocation(PanoramaLocation panoramaLocation);
        void UpdatePanoramaLocation(PanoramaLocation panoramaLocation);
    }
}
