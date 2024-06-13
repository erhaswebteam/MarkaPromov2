using IPara.DeveloperPortal.Core;
using IPara.DeveloperPortal.Core.Entity;
using IPara.DeveloperPortal.Core.Request;
using IPara.DeveloperPortal.Core.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Nop.Core;
using Nop.Core.Domain.Payments.IPara;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Nop.Services.Payments
{
    public class IParaPaymentManager : IParaPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IParaSettings _iparaSettings;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        public IParaPaymentManager(HttpClient httpClient, IOptions<IParaSettings> iparaSettings, IWebHelper webHelper, IWorkContext workContext)
        {
            _httpClient = httpClient;
            _iparaSettings = iparaSettings.Value;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        private Settings GetSettings()
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

        public string ThreeDPaymentInit(IParaPaymentRequest iParaPaymentRequest, HttpContext httpContext)
        {
            var orderId = new Random().Next(10000, 99999);
            var customer = _workContext.GetCurrentCustomerAsync().Result;
            Settings iParaSet = GetSettings();
            iParaSet.TransactionDate = DateTime.Now.ToString();

            var httpRequest = httpContext.Request;
            var host = httpRequest.Host.ToString(); // Hostname ve port
            var scheme = httpRequest.Scheme; // http veya https
            var pathBase = httpRequest.PathBase.ToString(); // Uygulama temel yolu

            var baseUrl = $"{scheme}://{host}{pathBase}";

            var request = new ThreeDPaymentInitRequest();
            request.OrderId = orderId.ToString() + "_" + customer.Id + "_" + iParaPaymentRequest.Amount.ToString("F").Replace(",", "").Replace(".", "");
            request.Echo = "Echo";
            request.Mode = iParaSet.Mode;
            request.Version = iParaSet.Version;
            request.Amount = iParaPaymentRequest.Amount.ToString("F").Replace(",", "").Replace(".", "");
            request.CardOwnerName = iParaPaymentRequest.CardOwnerName;
            request.CardNumber = iParaPaymentRequest.CardNumber.Replace(" ", "");
            request.CardExpireMonth = iParaPaymentRequest.CardExpireMonth;
            request.CardExpireYear = iParaPaymentRequest.CardExpireYear;
            request.Installment = iParaPaymentRequest.Installement;
            request.Cvc = iParaPaymentRequest.Cvc;

            request.PurchaserName = iParaPaymentRequest.CardOwnerName.Split(' ')[0];
            request.PurchaserSurname = iParaPaymentRequest.CardOwnerName ?? iParaPaymentRequest.CardOwnerName.Substring(iParaPaymentRequest.CardOwnerName.IndexOf(' '));
            request.PurchaserEmail = customer.Email; //"test@test.com";

            //canlı
            //Ssl olsun mu
            //var storeLocation = _webHelper.GetStoreLocation(true);

            //request.SuccessUrl = $"{storeLocation}IParaSuccess";
            //request.FailUrl = $"{storeLocation}IParaFail";

            //test
            request.SuccessUrl = baseUrl + "Payment/IParaSuccess"; //"https://localhost:7181/Payment/IParaSuccess";
            request.FailUrl = baseUrl + "Payment/IParaFail";//"https://localhost:7181/Payment/IParaFail";

            var result = ThreeDPaymentInitRequest.Execute(request, iParaSet);
            return result;
        }

        public ThreeDPaymentCompleteResponse ThreeDPaymentComplete(IFormCollection form, ThreeDPaymentCompleteRequest threeDPaymentCompleteRequest, List<Product> products)
        {
            Settings iParaSet = GetSettings();

            string[] orderNumber = form["orderId"].ToString().Split("_");

            ThreeDPaymentInitResponse paymentResponse = new ThreeDPaymentInitResponse();
            paymentResponse.OrderId = form["orderId"];
            paymentResponse.Result = form["result"];
            paymentResponse.Amount = form["amount"];
            paymentResponse.Mode = form["mode"];
            if (!string.IsNullOrEmpty(form["errorCode"]))
                paymentResponse.ErrorCode = form["errorCode"];

            if (!string.IsNullOrEmpty(form["errorMessage"]))
                paymentResponse.ErrorMessage = form["errorMessage"];

            if (!string.IsNullOrEmpty(form["transactionDate"]))
                paymentResponse.TransactionDate = form["transactionDate"];

            if (!string.IsNullOrEmpty(form["hash"]))
                paymentResponse.Hash = form["hash"];

            var customer = _workContext.GetCurrentCustomerAsync().Result;
            ThreeDPaymentCompleteResponse response = new ThreeDPaymentCompleteResponse();

            if (Helper.Validate3DReturn(paymentResponse, iParaSet))
            {
                var request = new ThreeDPaymentCompleteRequest();

                #region Request New
                request.OrderId = form["orderId"];
                request.Echo = "Echo";
                request.Mode = _iparaSettings.Mode;
                request.Amount = form["amount"]; // 100 tL
                request.CardOwnerName = threeDPaymentCompleteRequest.CardOwnerName;
                request.CardNumber = threeDPaymentCompleteRequest.CardNumber;
                request.CardExpireMonth = threeDPaymentCompleteRequest.CardExpireMonth;
                request.CardExpireYear = threeDPaymentCompleteRequest.CardExpireYear;
                request.Installment = "1";
                request.Cvc = threeDPaymentCompleteRequest.Cvc;
                request.ThreeD = "true";
                request.ThreeDSecureCode = form["threeDSecureCode"];
                #endregion

                #region Sipariş veren bilgileri
                request.Purchaser = new Purchaser();
                request.Purchaser.Name = threeDPaymentCompleteRequest.CardOwnerName.Split(' ')[0];
                request.Purchaser.SurName = threeDPaymentCompleteRequest.CardOwnerName ?? threeDPaymentCompleteRequest.CardOwnerName.Substring(threeDPaymentCompleteRequest.CardOwnerName.IndexOf(' ')); ;
                request.Purchaser.Email = customer.Email;
                #endregion

                request.Products = products;

                response = ThreeDPaymentCompleteRequest.Execute(request, iParaSet);
            }
            return response;
        }
    }
}
