using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Nop.Core.Domain.Payments.IPara
{
    public class PaymentInfoRequest
    {
        public PaymentInfoRequest()
        {
            CreditCardTypes = new List<SelectListItem>();
            ExpireMonths = new List<SelectListItem>();
            ExpireYears = new List<SelectListItem>();
        }

        public string CreditCardType { get; set; }

        public IList<SelectListItem> CreditCardTypes { get; set; }

        public string CardholderName { get; set; }

        public string CardNumber { get; set; }

        public string ExpireMonth { get; set; }

        public string ExpireYear { get; set; }

        public IList<SelectListItem> ExpireMonths { get; set; }

        public IList<SelectListItem> ExpireYears { get; set; }

        public string CardCode { get; set; }
        public decimal TotalDebit { get; set; }
    }
}
