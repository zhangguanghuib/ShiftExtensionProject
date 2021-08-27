/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace Contoso.Commerce.Runtime.WeChatPay
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
    using Newtonsoft.Json;

    public class WeChatPayController : IController
    {
        [HttpPost]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Employee, CommerceRoles.Application)]
        public async Task<WeChatPayQrCodeInfo> GenerateWeChatPayQrCode(IEndpointContext context, string cartId)
        {
            CartSearchCriteria cartSearchCriteria = new CartSearchCriteria(cartId);
            var getCartRequest = new GetCartRequest(cartSearchCriteria, QueryResultSettings.SingleRecord);
            var getCartResponse = await context.ExecuteAsync<GetCartResponse>(getCartRequest).ConfigureAwait(false);

            var cardPaymentAcceptSettings = new CardPaymentAcceptSettings
            {
                AdaptorPath = "https://localhost:4000/Connectors/",
                HostPageOrigin = "https://localhost:4000",
                CardPaymentEnabled = false,
                CardTokenizationEnabled = true,
                HideBillingAddress = true,
                PaymentAmount = getCartResponse.Carts.Single().AmountDue,
            };

            var getCardPaymentAcceptPointServiceRequest = new GetCardPaymentAcceptPointServiceRequest(cardPaymentAcceptSettings);
            var commerceProperties = new List<CommerceProperty>
            {
                new CommerceProperty(WeChatPayConstants.PaymentProperties.CartId, cartId),
                new CommerceProperty(WeChatPayConstants.PaymentProperties.GenerateQrCode, true),
            };

            getCardPaymentAcceptPointServiceRequest.SetProperty(ExtensionsEntity.PaymentExtensionPropertiesKey, commerceProperties);

            var getCardPaymentAcceptPointServiceResponse = await context.ExecuteAsync<GetCardPaymentAcceptPointServiceResponse>(getCardPaymentAcceptPointServiceRequest).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<WeChatPayQrCodeInfo>(getCardPaymentAcceptPointServiceResponse.CardPaymentAcceptPoint.AcceptPageContent);
        }

        [HttpPost]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Employee, CommerceRoles.Application)]
        public async Task<WeChatPayResult> RetrieveWeChatPayResult(IEndpointContext context, string resultAccessCode, string cartId)
        {
            var wechatPayResult = new WeChatPayResult();

            var request = new RetrieveCardPaymentAcceptResultServiceRequest(cartId, settings: null, resultAccessCode);
            var extensionProperties = new List<CommerceProperty>
            {
                new CommerceProperty(WeChatPayConstants.PaymentProperties.VerifyPayment, true),
                new CommerceProperty(WeChatPayConstants.PaymentProperties.CartId, cartId)
            };
            request.SetProperty(ExtensionsEntity.PaymentExtensionPropertiesKey, extensionProperties);

            try
            {
                var response = await context.ExecuteAsync<RetrieveCardPaymentAcceptResultServiceResponse>(request).ConfigureAwait(false);
                wechatPayResult.CardPaymentAcceptResult = response.CardPaymentAcceptResult;
                wechatPayResult.StatusInfo = new WeChatPayStatusInfo
                {
                    Status = WeChatPayStatus.Success,
                };
            }
            catch (PaymentException paymentException)
            {
                string jsonErrorMessage = null;
                string fullErrorMessage = paymentException.PaymentSdkErrors?.FirstOrDefault()?.Message;
                if (fullErrorMessage != null && fullErrorMessage.StartsWith(WeChatPayConstants.PaymentErrorMessageMask))
                {
                    int left = WeChatPayConstants.PaymentErrorMessageMask.Length;
                    int right = fullErrorMessage.IndexOf(WeChatPayConstants.PaymentErrorMessageMask, left);
                    if (right > left)
                    {
                        jsonErrorMessage = fullErrorMessage.Substring(left, right - left);
                    }
                }

                if (jsonErrorMessage == null)
                {
                    throw paymentException;
                }
                else
                {
                    wechatPayResult.StatusInfo = JsonConvert.DeserializeObject<WeChatPayStatusInfo>(jsonErrorMessage);
                }
            }

            return wechatPayResult;
        }
    }
}
