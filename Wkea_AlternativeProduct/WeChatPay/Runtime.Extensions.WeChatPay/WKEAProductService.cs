namespace Contoso.Commerce.Runtime.WeChatPay
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Data;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Retail.Diagnostics;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using WKEA.Commerce.Runtime.Messages;

    public class WKEAProductService : IRequestHandler
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[]
                {
                    typeof(WKEAProductRequest)
                };
            }
        }
        public Response Execute(Request request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            Type reqType = request.GetType();
            if (reqType == typeof(WKEAProductRequest))
            {
                return this.AddProductExtensionData((WKEAProductRequest)request);
            }
            else
            {
                string message = string.Format(CultureInfo.InvariantCulture, "Request '{0}' is not supported", reqType);
                RetailLogger.Log.GenericWarningEvent(message);
                throw new NotSupportedException(message);
            }
        }

        private WKEAProductResponse AddProductExtensionData(WKEAProductRequest request)
        {
            ThrowIf.Null(request, "request");
            if (request.ListSimpleProduct != null)
            {
                List<SimpleProduct> requestData = request.ListSimpleProduct;

                QueryResultSettings queryResultSettings = QueryResultSettings.FirstRecord;
                using (DatabaseContext databaseContext = new DatabaseContext(request.RequestContext))
                {
                    foreach (SimpleProduct iSimpleProduct in requestData)
                    {
                        var parameters = new ParameterSet();
                        if (!string.IsNullOrEmpty(iSimpleProduct.ItemId))
                        {
                            parameters["@ItemID"] = iSimpleProduct.ItemId;
                        }
                        else if (iSimpleProduct.RecordId != 0)
                        {
                            parameters["@Product"] = iSimpleProduct.RecordId;
                            parameters["@DataAreaId"] = request.RequestContext.GetChannelConfiguration().InventLocationDataAreaId;
                            parameters["@ItemID"] = databaseContext.ExecuteQueryDataSet("select * from ax.INVENTTABLE where PRODUCT = @Product and DATAAREAID = @DataAreaId ", parameters).Tables[0].Rows[0]["ITEMID"];
                        }

                        var dt = databaseContext.ExecuteQueryDataSet("select * from ext.WKEA_INVENTITEMPURCHSETUP where itemid=@ItemID", parameters).Tables[0];

                        if (dt.Rows.Count > 0)
                        {
                            iSimpleProduct.ExtensionProperties.Add(new CommerceProperty("HIGHESTQTY", dt.Rows[0]["HIGHESTQTY"].ToString()));
                            iSimpleProduct.ExtensionProperties.Add(new CommerceProperty("LEADTIME", dt.Rows[0]["LEADTIME"].ToString()));
                            iSimpleProduct.ExtensionProperties.Add(new CommerceProperty("LOWESTQTY", dt.Rows[0]["LOWESTQTY"].ToString()));
                            iSimpleProduct.ExtensionProperties.Add(new CommerceProperty("STANDARDQTY", dt.Rows[0]["STANDARDQTY"].ToString()));
                            iSimpleProduct.ExtensionProperties.Add(new CommerceProperty("MODEL", dt.Rows[0]["MODEL"].ToString()));
                        }
                        else
                        {
                            iSimpleProduct.ExtensionProperties.Add(new CommerceProperty("HIGHESTQTY", "-1"));
                            iSimpleProduct.ExtensionProperties.Add(new CommerceProperty("LEADTIME", "-1"));
                            iSimpleProduct.ExtensionProperties.Add(new CommerceProperty("LOWESTQTY", "-1"));
                            iSimpleProduct.ExtensionProperties.Add(new CommerceProperty("STANDARDQTY", "-1"));
                            iSimpleProduct.ExtensionProperties.Add(new CommerceProperty("MODEL", "-1"));
                        }

                        dt = databaseContext.ExecuteQueryDataSet("select ISNULL(SUM(PostedQty + Received - Deducted + Registered - Picked),0) AS 'RealInventory' FROM  EXT.[WKEA_INVENTSUM] where itemid=@ItemID", parameters).Tables[0];

                        iSimpleProduct.ExtensionProperties.Add(new CommerceProperty("InventQty", dt.Rows[0][0].ToString()));
                        long CATEGORY;
                        string Series = f_GetProductSeries(databaseContext, iSimpleProduct.RecordId, out CATEGORY);

                        iSimpleProduct.ExtensionProperties.Add(new CommerceProperty("Series", Series));
                        iSimpleProduct.ExtensionProperties.Add(new CommerceProperty("SeriesId", CATEGORY));

                    }
                    ReadOnlyCollection<SimpleProduct> ReadOnlysource = new ReadOnlyCollection<SimpleProduct>(requestData);
                    PagedResult<SimpleProduct> PageData = new PagedResult<SimpleProduct>(ReadOnlysource);
                    return new WKEAProductResponse(PageData);
                }
            }
            if (request.ListProductSearchResult != null)
            {
                List<ProductSearchResult> requestData = request.ListProductSearchResult;

                QueryResultSettings queryResultSettings = QueryResultSettings.FirstRecord;
                using (DatabaseContext databaseContext = new DatabaseContext(request.RequestContext))
                {
                    foreach (ProductSearchResult iSProductSearchResult in requestData)
                    {
                        var parameters = new ParameterSet();
                        if (!string.IsNullOrEmpty(iSProductSearchResult.ItemId))
                        {
                            parameters["@ItemID"] = iSProductSearchResult.ItemId;
                        }
                        else if(iSProductSearchResult.RecordId != 0)
                        {
                            parameters["@Product"] = iSProductSearchResult.RecordId;
                            parameters["@DataAreaId"] = request.RequestContext.GetChannelConfiguration().InventLocationDataAreaId;
                            parameters["@ItemID"] = databaseContext.ExecuteQueryDataSet("select * from ax.INVENTTABLE where PRODUCT = @Product and DATAAREAID = @DataAreaId ", parameters).Tables[0].Rows[0]["ITEMID"];
                        }
                        
                        var dt = databaseContext.ExecuteQueryDataSet("select * from ext.WKEA_INVENTITEMPURCHSETUP where itemid=@ItemID", parameters).Tables[0];

                        if (dt.Rows.Count > 0)
                        {
                            iSProductSearchResult.ExtensionProperties.Add(new CommerceProperty("HIGHESTQTY", dt.Rows[0]["HIGHESTQTY"].ToString()));
                            iSProductSearchResult.ExtensionProperties.Add(new CommerceProperty("LEADTIME", dt.Rows[0]["LEADTIME"].ToString()));
                            iSProductSearchResult.ExtensionProperties.Add(new CommerceProperty("LOWESTQTY", dt.Rows[0]["LOWESTQTY"].ToString()));
                            iSProductSearchResult.ExtensionProperties.Add(new CommerceProperty("STANDARDQTY", dt.Rows[0]["STANDARDQTY"].ToString()));
                        }
                        else
                        {

                            iSProductSearchResult.ExtensionProperties.Add(new CommerceProperty("HIGHESTQTY", "-1"));
                            iSProductSearchResult.ExtensionProperties.Add(new CommerceProperty("LEADTIME", "-1"));
                            iSProductSearchResult.ExtensionProperties.Add(new CommerceProperty("LOWESTQTY", "-1"));
                            iSProductSearchResult.ExtensionProperties.Add(new CommerceProperty("STANDARDQTY", "-1"));
                        }

                        dt = databaseContext.ExecuteQueryDataSet("select ISNULL(SUM(PostedQty + Received - Deducted + Registered - Picked),0) AS 'RealInventory' FROM  EXT.[WKEA_INVENTSUM] where itemid=@ItemID", parameters).Tables[0];

                        iSProductSearchResult.ExtensionProperties.Add(new CommerceProperty("InventQty", dt.Rows[0][0].ToString()));
                        long CATEGORY;
                        string Series = f_GetProductSeries(databaseContext, iSProductSearchResult.RecordId, out CATEGORY);


                        iSProductSearchResult.ExtensionProperties.Add(new CommerceProperty("Series", Series));
                        iSProductSearchResult.ExtensionProperties.Add(new CommerceProperty("SeriesId", CATEGORY));

                    }

                    ReadOnlyCollection<ProductSearchResult> ReadOnlysource = new ReadOnlyCollection<ProductSearchResult>(requestData);
                    PagedResult<ProductSearchResult> PageData = new PagedResult<ProductSearchResult>(ReadOnlysource);
                    return new WKEAProductResponse(PageData);
                }
            }

            return null;
        }

        private string f_GetProductSeries(DatabaseContext databaseContext, long product, out long CATEGORY)
        {
            var parameters = new ParameterSet();
            parameters["@PRODUCT"] = product;
            string Series = "";
            CATEGORY = 0;
            try
            {
                string Sql = "";
                Sql = Sql + "select B.NAME,B.RECID from AX.ECORESPRODUCTCATEGORY A INNER JOIN AX.ECORESCATEGORY B ON A.CATEGORY = B.RECID";
                Sql = Sql + " inner join AX.ECORESCATEGORYATTRIBUTEGROUP c on a.CATEGORY = c.CATEGORY";
                Sql = Sql + " inner join AX.ECORESATTRIBUTEGROUP d on c.ATTRIBUTEGROUP = d.RECID";
                Sql = Sql + " where UPPER(d.NAME) = 'SERIES' AND PRODUCT = @Product";
                var dtProductCategory = databaseContext.ExecuteQueryDataSet(Sql, parameters).Tables[0];

                if (dtProductCategory.Rows.Count > 0)
                {
                    Series = (dtProductCategory.Rows[0][0].ToString());
                    CATEGORY = long.Parse(dtProductCategory.Rows[0][1].ToString());
                }
            }
            catch (Exception)
            {

            }

            return Series;
        }
    }
}
