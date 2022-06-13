using System.Threading;
using System.Threading.Tasks;
using Data.Entities;
using Data_EF;
using EntityFrameworkCore.Triggered;

namespace Application.Trigger
{
    public class CustomerTrigger : IAfterSaveTrigger<Customer>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerTrigger(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public Task AfterSave(ITriggerContext<Customer> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Added)
            {
                context.Entity.Code = context.Entity.Inc.ToString("D4");
                _unitOfWork.SaveChanges();
            }
            
            return Task.CompletedTask;
        }
    }
}