/**
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
        using System.Linq;
        using System.Text;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

        /// <summary>
        /// The extended service to get custom receipt field.
        /// </summary>
        /// <remarks>
        /// To print custom receipt fields on a receipt, one must handle <see cref="GetSalesTransactionCustomReceiptFieldServiceRequest"/>
        /// and <see cref="GetCustomReceiptFieldServiceResponse"/>. Here are several points about how to do this.
        /// 1. CommerceRuntime calls this request if and only if it needs to print values for some fields that are not supported by default. So make
        ///    sure your custom receipt fields have different names than existing ones. Adding a prefix in front of custom filed names would
        ///    be a good idea. The value of custom filed name should match the value you defined in AX, on Custom Filed page.
        /// 2. User should handle content-related formatting. This means that if you want to print "$ 10.00" instead of "10" on the receipt, 
        ///    you must generate "$ 10.00" by yourself. You can call <see cref="GetFormattedCurrencyServiceRequest"/> to do this. There are also some
        ///    other requests designed to format other types of values such as numbers and date time. Note, the user DO NOT need to worry about alignment,
        ///    the CommerceRuntime will take care of that.
        /// 3. If any exception happened when getting the value of custom receipt fields, CommerceRuntime will print empty value on the receipt and the
        ///    exceptions will be logged.
        /// 4. So far, only sales-transaction-based custom receipts are supported. This means you can do customization for receipts when checking out
        ///    a normal sales transaction or creating/picking up a customer order.
        /// </remarks>
        public class GetCustomReceiptFieldsService : IRequestHandler
        {
            /// <summary>
            /// Gets the supported request types.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes
            {
                get
                {
                    return new[]
                    {
                        typeof(GetSalesTransactionCustomReceiptFieldServiceRequest),
                    };
                }
            }

            /// <summary>
            /// Executes the requests.
            /// </summary>
            /// <param name="request">The request parameter.</param>
            /// <returns>The GetReceiptServiceResponse that contains the formatted receipts.</returns>
            public Response Execute(Request request)
            {
                if (request == null)
                {
                    throw new ArgumentNullException("request");
                }

                Type requestedType = request.GetType();

                if (requestedType == typeof(GetSalesTransactionCustomReceiptFieldServiceRequest))
                {
                    return this.GetCustomReceiptFieldForSalesTransactionReceipts((GetSalesTransactionCustomReceiptFieldServiceRequest)request);
                }

                throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
            }

            /// <summary>
            /// Gets the custom receipt filed for all transaction-based receipts, such as SalesReceipt, CustomerOrderReceipt, PickupReceipt, CreditCardReceipt, and so on.
            /// </summary>
            /// <param name="request">The service request to get custom receipt filed.</param>
            /// <returns>The value of custom receipt filed.</returns>
            private GetCustomReceiptFieldServiceResponse GetCustomReceiptFieldForSalesTransactionReceipts(GetSalesTransactionCustomReceiptFieldServiceRequest request)
            {
                string receiptFieldName = request.CustomReceiptField;

                SalesOrder salesOrder = request.SalesOrder;
                SalesLine salesLine = request.SalesLine;
                TenderLine tenderLine = request.TenderLine;

                // Get the store currency.
                string currency = request.RequestContext.GetOrgUnit().Currency;

                string reasonCodeStr = string.Empty;

   /*             if (salesOrder.ExtensibleSalesTransactionType != ExtensibleSalesTransactionType.IncomeExpense || salesOrder.ExtensibleSalesTransactionType != ExtensibleSalesTransactionType.CustomerAccountDeposit)
                {
                    reasonCodeStr = GetReasonCodesReceiptPrintText(salesOrder.ReasonCodeLines, request.RequestContext);
                }*/

                string returnValue = string.Empty;
                switch (receiptFieldName)
                {
                    case "TIPAMOUNT":
                        {
                            // FORMAT THE VALUE
                            decimal tipAmount = salesOrder == null ? 0 : (salesOrder.TotalAmount * 0.18m);
                            returnValue = this.FormatCurrency(tipAmount, currency, request.RequestContext);
                        }

                        break;

                    case "ITEMNUMBER":
                        {
                            returnValue = salesLine == null ? string.Empty : "Custom_" + salesLine.ItemId;
                        }

                        break;

                    case "TENDERID":
                        {
                            returnValue = tenderLine == null ? string.Empty : "Custom_" + tenderLine.TenderTypeId;
                        }

                        break;

                    case "CUSTOMINFOCODE":
                        {
                            reasonCodeStr = GetReasonCodesReceiptPrintText(salesOrder.ReasonCodeLines, request.RequestContext);
                            returnValue = reasonCodeStr;
                        }

                        break;
                }

                return new GetCustomReceiptFieldServiceResponse(returnValue);
            }

            private string GetReasonCodesReceiptPrintText(IEnumerable<ReasonCodeLine> reasonCodeLines, RequestContext context)
            {
                StringBuilder returnValue = new StringBuilder();

                var reasonCodeIds = reasonCodeLines.Select(r => r.ReasonCodeId).Distinct(StringComparer.OrdinalIgnoreCase);

                if (reasonCodeIds.Any())
                {
                    GetReasonCodesServiceRequest reasonCodeServiceRequest = new GetReasonCodesServiceRequest(QueryResultSettings.AllRecords, reasonCodeIds);
                    var reasonCodeServiceResponse = context.Runtime.Execute<GetReasonCodesServiceResponse>(reasonCodeServiceRequest, context);
                    var reasonCodesById = reasonCodeServiceResponse.ReasonCodes.Results.ToDictionary(r => r.ReasonCodeId, r => r);

                    if (reasonCodesById.Any()) // if configuration still contains these infocodes
                    {
                        foreach (ReasonCodeLine reasonCodeLine in reasonCodeLines)
                        {
                            ReasonCode reasonCode;
                            if (reasonCodesById.TryGetValue(reasonCodeLine.ReasonCodeId, out reasonCode)
                                && (reasonCode.PrintInputNameOnReceipt || reasonCode.PrintInputToReceipt || reasonCode.PrintPromptToReceipt || reasonCode.PrintDescriptionOnReceipt))
                            {
                                string reasonCodeText = GetReasonCodeReceiptPrintText(reasonCodeLine, reasonCode, context);
                                if (returnValue.Length > 0)
                                {
                                    returnValue.Append(Environment.NewLine);
                                }

                                if (reasonCodeText.Length > 0)
                                {
                                    returnValue.Append(reasonCodeText);
                                }
                            }
                        }
                    }
                }

                return returnValue.ToString();
            }

            private string GetReasonCodeReceiptPrintText(ReasonCodeLine reasonCodeLine, ReasonCode reasonCode, RequestContext context)
            {
                List<string> reasonCodeLineText = new List<string>();
                StringBuilder returnValue = new StringBuilder();
                const string ReasonCodeSeparator = " - ";

                // Print infocode and subinfocode ids first
                if (reasonCode.PrintInputNameOnReceipt && (!string.IsNullOrWhiteSpace(reasonCodeLine.ReasonCodeId)))
                {
                    reasonCodeLineText.Add(reasonCodeLine.ReasonCodeId);
                }

                if (reasonCode.PrintInputNameOnReceipt && (!string.IsNullOrWhiteSpace(reasonCodeLine.SubReasonCodeId)))
                {
                    reasonCodeLineText.Add(reasonCodeLine.SubReasonCodeId);
                }

                // Then description
                if (reasonCode.PrintDescriptionOnReceipt && (!string.IsNullOrWhiteSpace(reasonCode.Description)))
                {
                    reasonCodeLineText.Add(reasonCode.Description);
                }

                // Print prompt next
                if (reasonCode.PrintPromptToReceipt && !string.IsNullOrWhiteSpace(reasonCode.Prompt))
                {
                    reasonCodeLineText.Add(reasonCode.Prompt);
                }

                // Print input value last
                if (reasonCode.PrintInputToReceipt)
                {
                    if (!string.IsNullOrWhiteSpace(reasonCodeLine.Information))
                    {
                        reasonCodeLineText.Add(reasonCodeLine.Information);
                    }
                    else if (new[] { ReasonCodeInputType.Numeric, ReasonCodeInputType.AgeLimit }.Contains(reasonCode.InputType))
                    {
                        reasonCodeLineText.Add(FormatNumber(reasonCodeLine.InformationAmount, context));
                    }
                }

                if (reasonCodeLineText.Count > 0)
                {
                    returnValue.Append(string.Join(ReasonCodeSeparator, reasonCodeLineText));
                }

                return returnValue.ToString();
            }

            private  string FormatNumber(decimal number, RequestContext context)
            {
                var request = new GetFormattedNumberServiceRequest(number);
                string result = context.Execute<GetFormattedContentServiceResponse>(request).FormattedValue;
                return result;
            }

            /// <summary>
            /// Formats the currency to another currency.
            /// </summary>
            /// <param name="value">The digital value of the currency to be formatted.</param>
            /// <param name="currencyCode">The code of the target currency.</param>
            /// <param name="context">The request context.</param>
            /// <returns>The formatted value of the currency.</returns>
            private string FormatCurrency(decimal value, string currencyCode, RequestContext context)
            {
                GetRoundedValueServiceRequest roundingRequest = null;

                string currencySymbol = string.Empty;

                // Get the currency symbol.
                if (!string.IsNullOrWhiteSpace(currencyCode))
                {
                    var getCurrenciesDataRequest = new GetCurrenciesDataRequest(currencyCode, QueryResultSettings.SingleRecord);
                    Currency currency = context.Runtime.Execute<EntityDataServiceResponse<Currency>>(getCurrenciesDataRequest, context).PagedEntityCollection.FirstOrDefault();
                    currencySymbol = currency.CurrencySymbol;
                }

                roundingRequest = new GetRoundedValueServiceRequest(value, currencyCode, 0, false);

                decimal roundedValue = context.Execute<GetRoundedValueServiceResponse>(roundingRequest).RoundedValue;

                var formattingRequest = new GetFormattedCurrencyServiceRequest(roundedValue, currencySymbol);
                string formattedValue = context.Execute<GetFormattedContentServiceResponse>(formattingRequest).FormattedValue;
                return formattedValue;
            }
        }
    }
}
