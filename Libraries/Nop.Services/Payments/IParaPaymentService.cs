using IPara.DeveloperPortal.Core.Entity;
using IPara.DeveloperPortal.Core.Request;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Payments.IPara;
using System.Collections.Generic;

namespace Nop.Services.Payments
{
    public interface IParaPaymentService
    {
        string ThreeDPaymentInit(IParaPaymentRequest iParaPaymentRequest, HttpContext httpContext);
        string ThreeDPaymentComplete(IFormCollection form, ThreeDPaymentCompleteRequest threeDPaymentCompleteRequest, List<Product> products);
    }
}
