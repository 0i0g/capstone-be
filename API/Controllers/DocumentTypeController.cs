using System;
using System.Threading.Tasks;
using API.Configurations.Filter;
using Application.Interfaces;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authentication()]
    public class DocumentTypeController : BaseController
    {
        private readonly IDocumentTypeService _documentTypeService;

        public DocumentTypeController(IDocumentTypeService documentTypeService)
        {
            _documentTypeService = documentTypeService;
        }
        
        [PermissionRequired("Permission.DocumentType.Update", "Permission.System.BuManager")]
        [Route("documentType")]
        [HttpPut]
        public async Task<IActionResult> UpdateDocumentType(UpdateDocumentTypeModel model)
        {
            return await _documentTypeService.UpdateDocumentType(model);
        }

        [PermissionRequired("Permission.DocumentType.Read", "Permission.System.BuManager")]
        [Route("documentType")]
        [HttpGet]
        public IActionResult GetDocumentType(int id)
        {
            return _documentTypeService.GetDocumentType(id);
        }

        [PermissionRequired("Permission.DocumentType.Read", "Permission.System.BuManager")]
        [Route("documentType/all")]
        [HttpGet]
        public IActionResult GetAllDocumentType()
        {
            return _documentTypeService.GetAllDocumentType();
        }
    }
}