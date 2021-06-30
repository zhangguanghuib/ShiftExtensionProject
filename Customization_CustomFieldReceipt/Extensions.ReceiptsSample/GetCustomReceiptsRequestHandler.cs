﻿/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace Contoso
{
    namespace Commerce.Runtime.ReceiptsSample
    {
        using System;
        using System.Collections.Generic;
        using System.Collections.ObjectModel;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

        /// <summary>
        /// The request handler for GetCustomReceiptsRequestHandler class.
        /// </summary>
        /// <remarks>
        /// This is a example of how to print custom types of receipts. In this example we are mimicking following scenario:
        /// if one or more special items were sold in a transaction, then we need to print a special custom receipt for each
        /// of these special items along with all original sales receipts.
        /// Here are some points of how to do this:
        /// 1. The user should only handle <see cref="ReceiptType"/> 'CustomReceiptType' here. All other types of receipts
        ///    are handled by CommerceRuntime.
        /// 2. The most important thing to do here is preparing the data which will be used to render the receipt later.
        /// 3. So far, only sales-transaction-based custom receipts are supported. This means the data your prepared at step 2
        ///    must be a <see cref="SalesOrder"/> object.
        /// </remarks>
        public class GetCustomReceiptsRequestHandler : SingleRequestHandler<GetCustomReceiptsRequest, GetReceiptResponse>
        {
            /// <summary>
            /// Processes the GetCustomReceiptsRequest to return the set of receipts. The request should not be null.
            /// </summary>
            /// <param name="request">The request parameter.</param>
            /// <returns>The GetReceiptResponse.</returns>
            protected override GetReceiptResponse Process(GetCustomReceiptsRequest request)
            {
                ThrowIf.Null(request, "request");
                ThrowIf.Null(request.ReceiptRetrievalCriteria, "request.ReceiptRetrievalCriteria");

                // 1. We need to get the sales order that we are print receipts for.
                var getCustomReceiptsRequest = new GetSalesOrderDetailsByTransactionIdServiceRequest(request.TransactionId, SearchLocation.Local);
                var getSalesOrderDetailsServiceResponse = this.Context.Execute<GetSalesOrderDetailsServiceResponse>(getCustomReceiptsRequest);

                if (getSalesOrderDetailsServiceResponse == null ||
                    getSalesOrderDetailsServiceResponse.SalesOrder == null)
                {
                    throw new DataValidationException(
                        DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_ObjectNotFound,
                        string.Format("Unable to get the sales order created. ID: {0}", request.TransactionId));
                }

                SalesOrder salesOrder = getSalesOrderDetailsServiceResponse.SalesOrder;

                Collection<Receipt> result = new Collection<Receipt>();

                // 2. Now we can handle any additional receipt here.
                switch (request.ReceiptRetrievalCriteria.ReceiptType) 
                {
                    // An example of getting custom receipts. 
                    case ReceiptType.CustomReceipt6:
                        {
                            IEnumerable<Receipt> customReceipts = this.GetCustomReceipts(salesOrder, request.ReceiptRetrievalCriteria);

                            result.AddRange(customReceipts);
                        }

                        break;

                    default:
                        // Add more logic to handle more types of custom receipt types.
                        break;
                }

                return new GetReceiptResponse(new ReadOnlyCollection<Receipt>(result));
            }

            /// <summary>
            /// An example to print all special custom receipts. If there are multiple special items
            /// were sold in a transaction, then we print a custom receipt for each of these items.
            /// </summary>
            /// <param name="salesOrder">The sales order that we are printing receipts for.</param>
            /// <param name="criteria">The receipt retrieval criteria.</param>
            /// <returns>A collection of receipts.</returns>
            private Collection<Receipt> GetCustomReceipts(SalesOrder salesOrder, ReceiptRetrievalCriteria criteria)
            {
                // Back up and clear existing sales lines because we want to print a custom receipt for each one of the sales lines.
                Collection<SalesLine> originalSalesLines = salesOrder.SalesLines;
                salesOrder.SalesLines = new Collection<SalesLine>();

                Collection<Receipt> result = new Collection<Receipt>();

                foreach (SalesLine salesLine in originalSalesLines)
                {
                    // Check if the item is a special item.
                    if (this.IsSpecialItem(salesLine))
                    {
                        // Add this special item back to the sales order so that we can print the custom receipt for this sales line.
                        salesOrder.SalesLines.Add(salesLine);

                        // Call receipt service to get the custom receipt.
                        var getReceiptServiceRequest = new GetReceiptServiceRequest(
                                salesOrder,
                                new Collection<ReceiptType> { criteria.ReceiptType },
                                salesOrder.TenderLines,
                                criteria.IsCopy,
                                criteria.IsPreview,
                                criteria.HardwareProfileId);
                        ReadOnlyCollection<Receipt> customReceipts = this.Context.Execute<GetReceiptServiceResponse>(getReceiptServiceRequest).Receipts;

                        // Add the custom receipt to the result collection.
                        result.AddRange(customReceipts);

                        // Clean the sales lines.
                        salesOrder.SalesLines.Clear();
                    }
                }

                return result;
            }

            /// <summary>
            /// A fake logic of determining if a sales line is special or not.
            /// </summary>
            /// <param name="salesLine">The sales line.</param>
            /// <returns>True if the item is special, false otherwise.</returns>
            private bool IsSpecialItem(SalesLine salesLine)
            {
                if (salesLine.ItemId.StartsWith("0", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
