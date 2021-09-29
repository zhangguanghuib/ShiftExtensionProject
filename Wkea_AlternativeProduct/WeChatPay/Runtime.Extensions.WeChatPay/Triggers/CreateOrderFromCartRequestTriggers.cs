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
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    /// <summary>
    /// Class that implements a post trigger for the GetCustomerDataRequest request type.
    /// </summary>
    public class CreateOrderFromCartRequestTriggers : IRequestTriggerAsync
    {
        /// <summary>
        /// Gets the supported requests for this trigger.
        /// </summary>
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(CreateOrderFromCartRequest) };
            }
        }

        /// <summary>
        /// Post trigger code to retrieve extension properties.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        public Task OnExecuted(Request request, Response response)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Pre trigger code.
        /// </summary>
        /// <param name="request">The request.</param>
        public Task OnExecuting(Request request)
        {
            var createOrderFromCartRequest = request as CreateOrderFromCartRequest;
            foreach (var cartTenderLine in createOrderFromCartRequest.CartTenderLines)
            {
                // The property will be finally passed into IPaymentProcessor.Authorize().
                cartTenderLine.SetProperty(WeChatPayConstants.PaymentProperties.CartId, createOrderFromCartRequest.CartId);
            }

            return Task.CompletedTask;
        }
    }
}
