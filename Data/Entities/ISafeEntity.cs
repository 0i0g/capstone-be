using System;

namespace Data.Entities
{
    public interface ISafeEntity
    {
        public DateTime CreatedAt { get; set; }

        public Guid? CreatedById { get; set; }
        
        public User CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Guid? UpdatedById { get; set; }
        
        public User UpdatedBy { get; set; }

        public DateTime? DeletedAt { get; set; }

        public Guid? DeletedById { get; set; }
        
        public User DeletedBy { get; set; }

        public bool? IsDeleted { get; set; }
    }
}