using IPara.DeveloperPortal.Core;
using Nop.Core.Domain.Payments.IPara;

namespace Nop.Services.Payments
{
    public interface IParaPaymentService
    {
        Settings GetSettings();
    }
}
