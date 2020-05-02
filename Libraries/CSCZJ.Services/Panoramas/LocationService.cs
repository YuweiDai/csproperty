using CSCZJ.Core.Data;
using CSCZJ.Core.Domain.Panoramas;
using CSCZJ.Services.Events;
using System;
using System.Linq;

namespace CSCZJ.Services.Messages
{
    public  class LocationService:ILocationService
    {
        private readonly IRepository<PanoramaLocation> _locationRepository;
        private readonly IEventPublisher _eventPublisher;

        public LocationService(IRepository<PanoramaLocation> locationRepository, IEventPublisher eventPublisher)
        {
            this._locationRepository = locationRepository;
            this._eventPublisher = eventPublisher;
        }

        public IQueryable<PanoramaLocation> GetAllLocations()
        {
            var query = from pl in _locationRepository.TableNoTracking
                        where !pl.Deleted
                        select pl;

            return query;
        }

        public PanoramaLocation GetLocationById(int id)
        {
            var location = _locationRepository.GetById(id);
            if (location==null || location.Deleted) return null;

            return location;            
        }

        public PanoramaLocation GetLocationBySceneId(int sceneId)
        {
            throw new NotImplementedException();
        }

        public void InsertPanoramaLocation(PanoramaLocation panoramaLocation)
        {
            if (panoramaLocation == null) throw new ArgumentNullException("panoramaLocation");
            else
            {
                _locationRepository.Insert(panoramaLocation);
                _eventPublisher.EntityInserted(panoramaLocation);
            }
        }

        public PanoramaLocation GetLocationByName(string name)
        {
            var query = from pl in _locationRepository.Table
                        where !pl.Deleted && pl.Name==name
                        select pl;

            return query.FirstOrDefault();
        }

        public void UpdatePanoramaLocation(PanoramaLocation panoramaLocation)
        {
            if (panoramaLocation == null) throw new ArgumentNullException("panoramaLocation");
            else
            {
                _locationRepository.Update(panoramaLocation);
                _eventPublisher.EntityUpdated(panoramaLocation);
            }
        }
    }
}
