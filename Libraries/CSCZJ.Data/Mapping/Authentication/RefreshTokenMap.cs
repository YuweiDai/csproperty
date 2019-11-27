using CSCZJ.Core.Domain.Authentication;

namespace CSCZJ.Data.Mapping.Authentication
{
    public class RefreshTokenMap : EntityTypeConfiguration<RefreshToken>
    {
        public RefreshTokenMap()
        {
            this.HasKey(tc => tc.Id);
        }
    }
}
