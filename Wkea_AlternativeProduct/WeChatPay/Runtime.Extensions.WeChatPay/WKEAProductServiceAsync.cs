namespace WKEA.Commerce.Runtime.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Data;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using WKEA.Commerce.Runtime.Messages;
    using WKEA.Commerce.Runtime.DataModel;

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

            PagedResult<WKEAProductExtended> results;

            var query = new SqlPagedQuery(request.QueryResultSettings)
            {
                DatabaseSchema = "ext",
                From = "WKEAPRODUCTEXTENDEDVIEW",
                OrderBy = "MTITEMID",
                Where = "MTITEMID = @itemId and MTPRODUCT = @productId AND DATAAREAID = @DataAreaId",
            };

            query.Parameters["@itemId"] = request.ItemdId;
            query.Parameters["@productId"] = request.ProductId;
            query.Parameters["@DataAreaId"] = request.RequestContext.GetChannelConfiguration().InventLocationDataAreaId;

            using (var databaseContext = new DatabaseContext(request.RequestContext))
            {
                results = await databaseContext.ReadEntityAsync<WKEAProductExtended>(query).ConfigureAwait(false);
            }

            return new WKEAProductExtendedDataReponse(results);
        }
    }
}
