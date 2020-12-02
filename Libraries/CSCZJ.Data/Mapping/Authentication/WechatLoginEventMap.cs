using CSCZJ.Core.Domain.Authentication;

namespace CSCZJ.Data.Mapping.Authentication
{
    public class WechatLoginEventMap : EntityTypeConfiguration<WechatLoginEvent>
    {
        public WechatLoginEventMap()
        {
            this.ToTable("WechatLoginEvents");
            this.Property(w => w.OpenId).IsRequired();
            this.Property(w => w.Session_key).IsRequired();
            this.Property(w => w.Token).IsRequired();
        }
    }
}
