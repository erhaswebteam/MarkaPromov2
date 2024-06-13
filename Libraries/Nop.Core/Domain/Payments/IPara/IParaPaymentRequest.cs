namespace Nop.Core.Domain.Payments.IPara
{
    public class IParaPaymentRequest
    {
        public string CardOwnerName { get; set; }
        public string CardNumber { get; set; }
        public string CardExpireMonth { get; set; }
        public string CardExpireYear { get; set; }
        public decimal Amount { get; set; }
        public string Cvc { get; set; }
        public string Installement { get; set; }
    }
}
