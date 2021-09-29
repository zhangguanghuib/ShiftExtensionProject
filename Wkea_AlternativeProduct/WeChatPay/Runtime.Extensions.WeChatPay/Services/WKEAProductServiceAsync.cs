namespace WKEA.Commerce.Runtime.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Client;
    using Microsoft.Dynamics.Commerce.Runtime.Data;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using WKEA.Commerce.Runtime.Messages;
    using WKEA.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;

    public class WKEAProductServiceAsync : IRequestHandlerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[]
                {
                    typeof(WKEAProductExtendedDataRequest)
                };
            }
        }

        public async Task<Response> Execute(Request request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            Type reqType = request.GetType();
            if (reqType == typeof(WKEAProductExtendedDataRequest))
            {
                return await this.GetProductExtended((WKEAProductExtendedDataRequest)request).ConfigureAwait(false);
            }
            else
            {
                string message = string.Format(CultureInfo.InvariantCulture, "Request '{0}' is not supported.", reqType);
                Console.WriteLine(message);
                throw new NotSupportedException(message);
            }
        }

        private async Task<Response> GetProductExtended(WKEAProductExtendedDataRequest request)
        {
            ThrowIf.Null(request, "request");

            // Way 1:  View
            // PagedResult<WKEAProductExtended> results;

            //var query = new SqlPagedQuery(request.QueryResultSettings)
            //{
            //    DatabaseSchema = "ext",
            //    From = "WKEAPRODUCTEXTENDEDVIEW",
            //    OrderBy = "MTITEMID",
            //    Where = "MTITEMID = @itemId and MTPRODUCT = @productId AND DATAAREAID = @DataAreaId",
            //};
            // query.Parameters["@itemId"] = request.ItemdId;
            // query.Parameters["@productId"] = request.ProductId;
            // query.Parameters["@DataAreaId"] = request.RequestContext.GetChannelConfiguration().InventLocationDataAreaId;
            // return new WKEAProductExtendedDataReponse(results);

            // Way 2: Table join:
            //        var requiredColumns = new List<string>();
            //        requiredColumns.Add("ITEMID");
            //        requiredColumns.Add("PRODUCT");
            //        requiredColumns.Add("DATAAREAID");

            //        var query = new SqlPagedQuery(request.QueryResultSettings)
            //        {
            //            Select = new ColumnSet(requiredColumns.ToArray()),
            //            DatabaseSchema = "ax",
            //            From = "INVENTTABLE",
            //            OrderBy = "ITEMID",
            //            Alias = "alt"
            //        };

            //        List<string> whereClauses = new List<string>();
            //        whereClauses.Add("EXISTS (SELECT * FROM ax.INVENTTABLE as mt WHERE mt.ALTITEMID = alt.ITEMID and mt.DATAAREAID = alt.DATAAREAID and mt.ITEMID = @mtItemId and  mt.PRODUCT = @productId)");
            //        whereClauses.Add("alt.DATAAREAID = @DataAreaId");
            //        query.Where = string.Join(" AND ", whereClauses);

            //        query.Parameters["@mtItemId"] = request.ItemdId;
            //        query.Parameters["@productId"] = request.ProductId;
            //        query.Parameters["@DataAreaId"] = request.RequestContext.GetChannelConfiguration().InventLocationDataAreaId;

            //        using (var databaseContext = new DatabaseContext(request.RequestContext))
            //        {
            //            results = await databaseContext.ReadEntityAsync<WKEAProductExtended>(query).ConfigureAwait(false);
            //        }

            //        return new WKEAProductExtendedDataReponse(results);


            //Way3 : Store procedure mode (recommendation)
            var parameters = new ParameterSet();
            parameters["@nvc_MTItemId"] = request.ItemdId;
            parameters["@bi_MTProduct"] = request.ProductId;
            parameters["@nvc_DataAreaId"] = request.RequestContext.GetChannelConfiguration().InventLocationDataAreaId;
           
            PagedResult<WKEAProductExtended> WKEAProducts;
            using (var databaseContext = new DatabaseContext(request.RequestContext))
            {
                // Get alternative product:
                var results = await databaseContext.ExecuteStoredProcedureAsync<WKEAProductExtended>(
                    "ext.GETALTERNATIVEPRODUCT",
                    parameters,
                    QueryResultSettings.AllRecords).ConfigureAwait(false);

                WKEAProducts = results.Item2;
                if (WKEAProducts.Results.Count > 0)
                {
                    ProductManager productManager = ProductManager.Create(request.RequestContext.Runtime);
                    long altProductId = WKEAProducts.Results[0].Product;
                    PagedResult<MediaLocation> mediaLocations = productManager.GetMediaLocations(
                        request.RequestContext.GetChannel().RecordId,
                        0,
                        altProductId,
                        QueryResultSettings.AllRecords);
                    WKEAProducts.Results[0].Images = mediaLocations;
                }
            }
            return new WKEAProductExtendedDataReponse(WKEAProducts);
        }
    }
}
