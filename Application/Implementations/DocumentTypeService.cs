using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Data.Entities;
using Data.Enums;
using Data_EF;
using Data_EF.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Utilities.Constants;

namespace Application.Implementations
{
    public class DocumentTypeService : BaseService, IDocumentTypeService
    {
        private readonly IDocumentTypeRepository _documentTypeRepository;
        private readonly IQueryable<DocumentType> _documentTypeQueryable;
        private readonly ILogger<DocumentTypeService> _logger;

        public DocumentTypeService(IServiceProvider provider, ILogger<DocumentTypeService> logger) : base(provider)
        {
            _documentTypeRepository = _unitOfWork.DocumentType;
            _documentTypeQueryable = _documentTypeRepository.GetAll();
            _logger = logger;
        }

        public async Task<IActionResult> UpdateDocumentType(UpdateDocumentTypeModel model)
        {
            var documentType = _documentTypeQueryable.FirstOrDefault(x => x.Id == model.Id);
            if (documentType == null)
            {
                return ApiResponse.BadRequest(MessageConstant.DocumentTypeNotFound);
            }

            documentType.Format = model.Format ?? documentType.Format;
            documentType.Length = model.Length ?? documentType.Length;

            _documentTypeRepository.Update(documentType);

            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public IActionResult GetDocumentType(int id)
        {
            var documentType = _documentTypeQueryable.Select(x => new DocumentTypeViewModel
            {
                Id = x.Id,
                Type = x.Type,
                Name = x.Name,
                Format = x.Format,
                Length = x.Length
            }).FirstOrDefault(x => x.Id == id);
            if (documentType == null)
            {
                return ApiResponse.NotFound(MessageConstant.DocumentTypeNotFound);
            }

            return ApiResponse.Ok(documentType);
        }

        public IActionResult GetAllDocumentType()
        {
            var documentTypes = _documentTypeRepository.GetAll().Select(x => new DocumentTypeViewModel
            {
                Id = x.Id,
                Type = x.Type,
                Name = x.Name,
                Format = x.Format,
                Length = x.Length
            }).ToList();

            return ApiResponse.Ok(documentTypes);
        }
    }
}