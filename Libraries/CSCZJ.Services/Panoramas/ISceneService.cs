using CSCZJ.Core.Domain.Panoramas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Services.Messages
{
   public interface ISceneService
    {      
        void InsertPanoramaScene(PanoramaScene scene);

        void UpdatePanoramaScene(PanoramaScene scene);

        void DeletePanoramaScene(PanoramaScene scene);

        PanoramaScene GetPnoramaSceneById(int id);

        IQueryable<PanoramaScene> GetHotPanoramaScenes(int count);

        IQueryable<PanoramaScene> GetNewPanoramaScenes(int count);

        IQueryable<PanoramaScene> GetAllPanoramaScenes();


        IEnumerable<PanoramaScene> GetAllPanoramaScenesOrderByDistance(double lat,double lng,int pageSize=15,int index=0);
        
    }
}