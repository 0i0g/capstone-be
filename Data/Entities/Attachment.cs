using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Attachment
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public float Size { get; set; }

        public string Description { get; set; }

        #region Avatar Of User

        public User User { get; set; }

        #endregion

        public string GetUrl()
        {
            return "NONE";
        }
    }
}
