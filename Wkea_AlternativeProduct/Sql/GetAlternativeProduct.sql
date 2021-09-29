

CREATE PROCEDURE [ext].[GETALTERNATIVEPRODUCT]
    @tvp_QueryResultSettings [crt].[QUERYRESULTSETTINGSTABLETYPE] READONLY,
	@nvc_DataAreaId      NVARCHAR(4),
    @nvc_MTItemId        NVARCHAR(100) = NULL,
    @bi_MTProduct        BIGINT = 0
AS
BEGIN
    SET NOCOUNT ON

	IF @bi_MTProduct IS NULL
	     set @bi_MTProduct = 0
	
	IF @nvc_MTItemId IS NOT NULL or @bi_MTProduct != 0 
		SELECT
			alt.ITEMID as ItemId,
			alt.PRODUCT as Product,
			alt.DATAAREAID as DataAreaId,
			mt.ITEMID as MTItemId,
			mt.PRODUCT as MTProduct
		FROM [ax].[INVENTTABLE] AS alt
		JOIN [ax].[INVENTTABLE] AS mt ON alt.ITEMID = mt.ALTITEMID AND alt.DATAAREAID = mt.DATAAREAID
		WHERE
			(@nvc_MTItemId is null or mt.ITEMID = @nvc_MTItemId) 
			AND
			(@bi_MTProduct = 0 or mt.PRODUCT = @bi_MTProduct)
		ORDER BY [alt].ITEMID
		OFFSET (SELECT TOP 1 [SKIP] FROM @tvp_QueryResultSettings) ROWS
		FETCH NEXT (SELECT TOP 1 [TOP] FROM @tvp_QueryResultSettings) ROWS ONLY
END;
GO


