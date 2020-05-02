using CSCZJ.Core.Data;
using CSCZJ.Core.Domain.Panoramas;
using CSCZJ.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Services.Messages
{
   public class HotspotService:IHotspotService
    {
        private readonly IRepository<Hotspot> _hotspotRepository;
        private readonly IEventPublisher _eventPublisher;

        public HotspotService(IRepository<Hotspot> hotspotRepository, IEventPublisher eventPublisher) {

            this._hotspotRepository = hotspotRepository;
            this._eventPublisher = eventPublisher;
        }

        public void InsertHotspot(Hotspot hotspot)
        {
            if (hotspot == null) throw new ArgumentNullException("hotspot");
            else
            {
                _hotspotRepository.Insert(hotspot);
                _eventPublisher.EntityInserted(hotspot);
            }
        }

        public void UpdateHotspot(Hotspot hotspot)
        {
            if (hotspot == null) throw new ArgumentNullException("hotspot");
            else
            {
                _hotspotRepository.Update(hotspot);
                _eventPublisher.EntityUpdated(hotspot);
            }
        }

        public Hotspot GetHotspotById(int id)
        {
            var query = from v in _hotspotRepository.Table
                        where v.Id == id
                        select v;


            return query.FirstOrDefault();
        }

        public void DeleteHotspot(Hotspot hotspot)
        {
            if (hotspot == null) throw new ArgumentNullException("hotspot");
            else
            {
                _hotspotRepository.Delete(hotspot);
                _eventPublisher.EntityDeleted(hotspot);
            }
        }
    }
}
