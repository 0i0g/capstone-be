using System;
using System.Collections.Generic;

namespace Application.ViewModels.UserGroup
{
    public class SearchUserGroupViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool CanUpdate { get; set; }
        
        #region Permission

        public ICollection<string> Permissions { get; set; }

        #endregion     
    }
}