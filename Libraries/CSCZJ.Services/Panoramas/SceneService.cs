using CSCZJ.Core;
using CSCZJ.Core.Data;
using CSCZJ.Core.Domain.Panoramas;
using CSCZJ.Services.Events;
using QZCHY.PanoramaQuzhou.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Services.Messages
{
   public class SceneService: ISceneService
    {
        private readonly IRepository<PanoramaScene> _sceneRepository;
        private readonly IRepository<PanoramaLocation> _locationRepository;
        private readonly IEventPublisher _eventPublisher;

        public SceneService(IRepository<PanoramaScene> sceneRepository, IRepository<PanoramaLocation> locationRepository, IEventPublisher eventPublisher)
        {
            this._sceneRepository = sceneRepository;
            this._locationRepository = locationRepository;
            this._eventPublisher = eventPublisher;
        }

        public PanoramaScene GetSceneById(int id)
        {
            var query = from v in _sceneRepository.Table
                        where v.Id == id
                        select v;
      
            return query.FirstOrDefault();
        }

        public void InsertPanoramaScene(PanoramaScene sence)
        {
            if (sence == null) throw new ArgumentNullException("sence");
            else
            {
                _sceneRepository.Insert(sence);
                _eventPublisher.EntityInserted(sence);
            }
        }

        public void UpdatePanoramaScene(PanoramaScene scene)
        {
            if (scene == null) throw new ArgumentNullException("scene");
            else
            {
                _sceneRepository.Update(scene);
                _eventPublisher.EntityUpdated(scene);
            }
        }

        public void DeletePanoramaScene(PanoramaScene location)
        {
            if (location == null) throw new ArgumentNullException("location");
            else
            {
                _sceneRepository.Delete(location);
                _eventPublisher.EntityDeleted(location);
            }
        }

        public PanoramaScene GetPnoramaSceneById(int id)
        {
            var scene = _sceneRepository.GetById(id);
            if (scene==null || scene.Deleted) return null;
            else return scene;
        }

        public IQueryable<PanoramaScene> GetHotPanoramaScenes(int count)
        {
            var query = from ps in _sceneRepository.TableNoTracking
                        where !ps.Deleted
                        select ps;

            query = query.OrderByDescending(ps => ps.Views).Take(count);

            return query;

        }

        public IQueryable<PanoramaScene> GetNewPanoramaScenes(int count)
        {
            var query = from ps in _sceneRepository.TableNoTracking
                        where !ps.Deleted
                        select ps;

            query = query.OrderByDescending(ps => ps.ProductionDate).Take(count);

            return query;
        }

        public IQueryable<PanoramaScene> GetAllPanoramaScenes()
        {
            var query = from ps in _sceneRepository.TableNoTracking
                        where !ps.Deleted
                        select ps;
            
            return query;
        }
        /// <summary>
        /// 将全景图按照由近及远排序
        /// </summary>
        /// <param name="lat">定位位置的纬度</param>
        /// <param name="lng">定位位置的经度</param>
        /// <returns></returns>
        public IEnumerable<PanoramaScene> GetAllPanoramaScenesOrderByDistance(double lat = 28.9721214555, double lng = 118.8898357316, int pageSize = 15, int index = 0)
        {
            var query = from ps in _sceneRepository.TableNoTracking
                        where !ps.Deleted
                        select ps;

            var panoramaScenes = query.ToList();
            
            var panoramaScenesOrderedResult = panoramaScenes
                .OrderBy(ps => GeographyHelper.GetDistance(ps.PanoramaLocation.Lat, ps.PanoramaLocation.Lng, lat, lng))
                .ThenByDescending(ps => ps.ProductionDate);

            var locationNameList = new List<string>();
            var resultPanomarScenesFilterResult = new List<PanoramaScene>();
            foreach (var scene in panoramaScenesOrderedResult)
            {
                if (locationNameList.Contains(scene.PanoramaLocation.Name))
                {
                    continue;
                }
                else
                {
                    resultPanomarScenesFilterResult.Add(scene);
                    locationNameList.Add(scene.PanoramaLocation.Name);
                }
            }

            var resultPanomarScenesResult = resultPanomarScenesFilterResult.Skip(index * pageSize).Take(pageSize);      
            return resultPanomarScenesResult;
        }     
    }
}
