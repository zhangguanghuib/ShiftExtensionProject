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
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

    public class CreateSalesOrderServiceRequestTriggers : IRequestTriggerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(CreateSalesOrderServiceRequest) };
            }
        }

        public Task OnExecuted(Request request, Response response)
        {
            return Task.CompletedTask;
        }

        public Task OnExecuting(Request request)
        {
            // Uncomment the following line to simulate checkout failure. Then Void/Refund APIs of payment connector will be called so we can test if it works properly.
            // throw new Exception("Execution of CreateSalesOrderServiceRequest failed!");

            return Task.CompletedTask;
        }
    }
}
