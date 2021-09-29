namespace Contoso.Commerce.Runtime.EmailPreferenceSample
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Data;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

    public sealed class ValidateCouponUsageInOrderServiceRequestHandler : SingleAsyncRequestHandler<ValidateCouponUsageInOrderServiceRequest>
    {
        protected override async Task<Response> Process(ValidateCouponUsageInOrderServiceRequest request)
        {
            ThrowIf.Null(request, "request");

            //If this is recalled customer order
            // and no new cartline, neither existing cartline changed
            if (!string.IsNullOrEmpty(request.Transaction.SalesId) &&
                !isTransactionChanged(request.Transaction))
            {
                //Skip the validation
                return new NullResponse();   
            }
            else
            {
                // Run the OOB coupon validation logic
                return new NotHandledResponse();
            }
        }


        private bool isTransactionChanged(SalesTransaction transaction)
        {
            // new cart line added
            if (transaction.ActiveSalesLines.Any(salesLine => salesLine.IsProductLine && !salesLine.IsPriceLocked))
            {
                return true;
            }
            // existing cartline quantity changed
            if (transaction.ActiveSalesLines.All(salesLine =>
                salesLine.IsPriceLocked && salesLine.IsProductLine
                && salesLine.Quantity != salesLine.SavedQuantity))
            {
                return true;
            }

            return false;
        }
    }
}
