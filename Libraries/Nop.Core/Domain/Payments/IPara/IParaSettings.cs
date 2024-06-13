namespace Nop.Core.Domain.Payments.IPara
{
    public class IParaSettings
    {
        public string BaseUrl { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string Mode { get; set; }
        public string Version { get; set; }
        public string ThreeDInquiryUrl { get; set; }
    }
}
