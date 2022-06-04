using System;

namespace Data.Entities
{
    public class Test
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        #region User

        public Guid UserId { get; set; }

        public User User { get; set; }

        #endregion
    }
}