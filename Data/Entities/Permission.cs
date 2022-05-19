using System;

namespace Data.Entities
{
    public class Permission
    {
        public Guid Id { get; set; }

        public string PermissionType { get; set; }

        public int Level { get; set; }

        #region UserGroup

        public Guid GroupId { get; set; }

        public UserGroup Group { get; set; }

        #endregion
    }
}