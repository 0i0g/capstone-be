using System;

namespace Application.RequestModels
{
    public class RemoveModel
    {
        public Guid Id { get; set; }

        public bool ForceRemoveRef { get; set; } = false;
    }
}