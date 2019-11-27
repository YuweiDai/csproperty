using CSCZJ.Core.Domain.Logging;

namespace CSCZJ.Data.Mapping.Logging
{
    public partial class ActivityLogMap : EntityTypeConfiguration<ActivityLog>
    {
        public ActivityLogMap()
        {
            this.ToTable("ActivityLog");
            this.HasKey(al => al.Id);
            this.Property(al => al.Comment).IsRequired();

            this.HasRequired(al => al.ActivityLogType)
                .WithMany()
                .HasForeignKey(al => al.ActivityLogTypeId);

            this.HasRequired(al => al.AccountUser)
                .WithMany()
                .HasForeignKey(al => al.AccountUserId);
        }
    }
}
