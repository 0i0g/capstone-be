using System;

namespace Application.RequestModels
{
    public class AddUserInToGroupModel
    {
        public Guid GroupId { get; set; }

        public Guid UserId { get; set; }
    }
}