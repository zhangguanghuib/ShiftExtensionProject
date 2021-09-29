namespace Contoso.Commerce.Runtime.WeChatPay
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

    /// <summary>
    /// Class that implements a post trigger for the GetProductsServiceRequest request.
    /// </summary>
    public class ProductServiceTriggerV2 : IRequestTriggerAsync
    {
        /// <summary>
        /// Gets the supported requests for this trigger.
        /// </summary>
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] 
                {
                    typeof(GetProductsServiceRequest),
                    typeof(SearchProductsRequest),
                    typeof(GetSalesOrderDetailsByTransactionIdServiceRequest),
                    typeof(GetCommerceListRequest)
                };
            }
        }


        /// <summary>
        /// Post trigger code to retrieve extension properties.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        public async Task OnExecuted(Request request, Response response)
        {
            await Task.CompletedTask;

            var channelType = request.RequestContext.GetChannelConfiguration().ExtChannelType;
            bool channelAllowed = (channelType == ExtensibleRetailChannelType.OnlineStore || channelType == ExtensibleRetailChannelType.SharePointOnlineStore);

            if (!channelAllowed)
            {
                return;
            }
            try
            {
                if (request is GetProductsServiceRequest)
                {
                    GetProductsServiceRequest req = (GetProductsServiceRequest)request;
                    GetProductsServiceResponse res = (GetProductsServiceResponse)response;

                    List<SimpleProduct> newProducts = new List<SimpleProduct>();
                    var wkeaProductRequest = new WKEAProductRequest();
                    wkeaProductRequest.ListSimpleProduct = res.Products.ToList();
                    var wkeaProductResponse = await req.RequestContext.ExecuteAsync<WKEAProductResponse>(wkeaProductRequest).ConfigureAwait(false);
                    if (wkeaProductResponse != null && wkeaProductResponse.WKEAProducts != null)
                    {
                        response = new GetProductsServiceResponse(wkeaProductResponse.WKEAProducts);
                    }
                }
                if (request is SearchProductsRequest)
                {
                    SearchProductsRequest req = (SearchProductsRequest)request;
                    var res = (EntityDataServiceResponse<ProductSearchResult>)response;
                    var wkeaProductRequest = new WKEAProductRequest();
                    wkeaProductRequest.ListProductSearchResult = res.PagedEntityCollection.ToList();

                    var wkeaProductResponse = await req.RequestContext.ExecuteAsync<WKEAProductResponse>(wkeaProductRequest).ConfigureAwait(false);
                    if (wkeaProductResponse != null && wkeaProductResponse.WKEAProductSearchResult != null)
                    {
                        response = new EntityDataServiceResponse<ProductSearchResult>(wkeaProductResponse.WKEAProductSearchResult);
                    }
                }

                if (request is GetCommerceListRequest)
                {
                    GetCommerceListResponse res = response as GetCommerceListResponse;
                    if (res.CommerceLists.TotalCount > 1)
                    {
                        PagedResult<CommerceList> pageResult = res.CommerceLists;
                        List<CommerceListContributor> CommerceListContributors = new List<CommerceListContributor>();
                        List<CommerceListInvitation> CommerceListInvitations = new List<CommerceListInvitation>();
                        IList<CommerceListLine> CommerceListLines = new List<CommerceListLine>();

                        foreach (CommerceList list in pageResult)
                        {
                            bool IsProduct = false;
                            var commerceListLines = list.CommerceListLines;
                            List<CommerceListLine> NewProduct = new List<CommerceListLine>();
                            foreach (var cl in list.CommerceListLines)
                            {
                                var isExists = CommerceListLines.Select(u => u.ProductId == cl.ProductId);
                                if (isExists.Count() == 0)
                                {
                                    NewProduct.Add(cl);
                                    IsProduct = true;
                                }
                            }
                            if (IsProduct)
                            {
                                CommerceListContributors.AddRange(list.CommerceListContributors);
                                CommerceListInvitations.AddRange(list.CommerceListInvitations);
                                CommerceListLines.AddRange(NewProduct);
                            }

                        }
                        res.CommerceLists.FirstOrDefault<CommerceList>().CommerceListLines.Clear();
                        res.CommerceLists.FirstOrDefault<CommerceList>().CommerceListLines.AddRange(CommerceListLines);
                        res.CommerceLists.FirstOrDefault<CommerceList>().CommerceListContributors.Clear();
                        res.CommerceLists.FirstOrDefault<CommerceList>().CommerceListContributors.AddRange(CommerceListContributors);
                        res.CommerceLists.FirstOrDefault<CommerceList>().CommerceListInvitations.Clear();
                        res.CommerceLists.FirstOrDefault<CommerceList>().CommerceListInvitations.AddRange(CommerceListInvitations);
                    }
                }
            }
            catch (Exception)
            {

                return;
            }

        }

        /// <summary>
        /// Pre trigger code.
        /// </summary>
        /// <param name="request">The request.</param>
        public async Task OnExecuting(Request request)
        {
            await Task.CompletedTask;
        }
    }
}
