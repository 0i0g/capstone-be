using System;
using Application.Interfaces;

namespace Application.Implementations
{
    public class AttachmentService: BaseService, IAttachmentService
    {
        public AttachmentService(IServiceProvider provider) : base(provider)
        {
        }
    }
}