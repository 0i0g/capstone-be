using System.Threading;
using System.Threading.Tasks;
using Data.Entities;
using Data_EF;
using EntityFrameworkCore.Triggered;

namespace Application.Trigger
{
    public class BeginningVoucherDetailTrigger : IAfterSaveTrigger<BeginningVoucherDetail>
    {
        private readonly IUnitOfWork _unitOfWork;

        public BeginningVoucherDetailTrigger(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public Task AfterSave(ITriggerContext<BeginningVoucherDetail> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Added)
            {
                context.Entity.ProductName = context.Entity.Product.Name;
                _unitOfWork.SaveChanges();
            }
            
            return Task.CompletedTask;
        }
    }
}