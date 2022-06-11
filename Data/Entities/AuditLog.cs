using System;
using System.ComponentModel.DataAnnotations;
using Data.Enums;

namespace Data.Entities
{
    public class AuditLog
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public EnumAuditType Type { get; set; }

        [Required]
        public Guid ActorId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public string Content { get; set; }

        public Guid? EntityId { get; set; }

        public AuditLog()
        {
        }
        
        public AuditLog(Guid actor, EnumAuditType type, string content, DateTime createdAt, Guid? entityId = null)
        {
            ActorId = actor;
            Type = type;
            CreatedAt = createdAt;
            Content = content;
            EntityId = entityId;
        }
    }
}