using IPara.DeveloperPortal.Core;
using Microsoft.Extensions.Options;
using Nop.Core.Domain.Payments.IPara;
using System;

namespace Nop.Services.Payments
{
    public class IParaPaymentManager : IParaPaymentService
    {
        private readonly IParaSettings _iparaSettings;

        public IParaPaymentManager(IOptions<IParaSettings> iparaSettings)
        {
            _iparaSettings = iparaSettings.Value;
        }

        public Settings GetSettings()
        {
            Settings iParaSet = new Settings();
            iParaSet.BaseUrl = _iparaSettings.BaseUrl;
            iParaSet.Mode = _iparaSettings.Mode;
            iParaSet.PrivateKey = _iparaSettings.PrivateKey;
            iParaSet.PublicKey = _iparaSettings.PublicKey;
            iParaSet.ThreeDInquiryUrl = _iparaSettings.ThreeDInquiryUrl;
            iParaSet.HashString = string.Empty;
            iParaSet.Version = _iparaSettings.Version;
            iParaSet.TransactionDate = DateTime.Now.ToString();
            return iParaSet;
        }
    }
}
