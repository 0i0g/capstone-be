using System;
using System.Threading.Tasks;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IDocumentTypeService
    {
        Task<IActionResult> UpdateDocumentType(UpdateDocumentTypeModel model);
        
        IActionResult GetDocumentType(int id);

        IActionResult GetAllDocumentType();
    }
}