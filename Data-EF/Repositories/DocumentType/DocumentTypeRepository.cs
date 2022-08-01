using Data.Entities;
using Microsoft.AspNetCore.Http;

namespace Data_EF.Repositories
{
    public class DocumentTypeRepository : Repository<DocumentType> , IDocumentTypeRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DocumentTypeRepository(AppDbContext db, IHttpContextAccessor httpContextAccessor) : base(db)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    }
}