--select * from DOMSALESLINETOPROCESS

declare @domRunId    as bigint;
declare @partition   as bigint;
declare @dataareaid  as nvarchar(4);

set  @domRunId =   8382831563363208304; -- DOMRUNId
set  @partition =  5637144576;  --Partition
set  @dataareaid = 'usrt' -- DataAreaId

--Step 1: Get saleslines to be processed
DROP TABLE IF EXISTS #SalesLinesToProcessed

select T.DOMRUNID,  T.SALESID, soline.RECID, soline.DATAAREAID, soline.PARTITION 
into #SalesLinesToProcessed
from DOMLogs(nolock) as T 
join Salesline as soline on T.SALESID = soline.SALESID and T.SALESLINENUMBER = soline.LINENUM
where T.LOGMESSAGE like '%Preparing to process sales line number%' and T.DOMRUNID = @domRunId

select * from  #SalesLinesToProcessed

--Step 2:  Get Warehouse which can be for use of order fulfillment
DROP TABLE IF EXISTS #DOMAVAILINVENTLOCATION

select T.INVENTLOCATIONID, T.INVENTSITEID, T.DATAAREAID as InventLocationDataAreaId, T.RECID as InventLocationRecId,T.PARTITION, T.DATAAREAID, @domRunId as DomRunId
into #DOMAVAILINVENTLOCATION
from INVENTLOCATION as T
where T.InventLocationType = 0 and T.DATAAREAID = @dataareaid

select * from #DOMAVAILINVENTLOCATION


--Step 3: Check each warehouse, how many orders pending fulfillment in this warehouse
SELECT COUNT(T1.RECID),
              T2.INVENTLOCATIONRECID 
FROM SALESTABLE T1 CROSS 
JOIN #DOMAVAILINVENTLOCATION T2 
WHERE (((T1.PARTITION=5637144576) 
              AND (T1.DATAAREAID=N'USRT')) 
              AND ((T1.SALESSTATUS=1) 
              OR (T1.SALESSTATUS=2))) 
              AND (((T2.PARTITION=5637144576) 
              AND (T2.DATAAREAID=N'USRT')) 
              AND ((T2.INVENTLOCATIONDATAAREAID='usrt') 
              AND (T2.DOMRUNID=@domRunId))) 
              AND 
EXISTS (
SELECT 'X' 
FROM SALESLINE T3 CROSS 
JOIN INVENTDIM T4 CROSS 
JOIN RETAILSALESLINE T5 
WHERE (((T3.PARTITION=5637144576) 
              AND (T3.DATAAREAID=N'USRT')) 
              AND ((T1.SALESID=T3.SALESID) 
              AND ((T3.SALESSTATUS=1) 
              OR (T3.SALESSTATUS=2)))) 
              AND (((T4.PARTITION=5637144576) 
              AND (T4.DATAAREAID=N'USRT')) 
              AND ((T4.INVENTDIMID=T3.INVENTDIMID) 
              AND (T4.INVENTLOCATIONID=T2.INVENTLOCATIONID))) 
              AND (((T5.PARTITION=5637144576) 
              AND (T5.DATAAREAID=N'USRT')) 
              AND ((T3.RECID=T5.SALESLINE) 
              AND ((T5.FULFILLMENTSTATUS=10) 
              OR (T5.FULFILLMENTSTATUS=1)))) 
              AND 
              NOT (
EXISTS (
SELECT 'X' 
FROM #SalesLinesToProcessed T6 
WHERE (((T6.PARTITION=5637144576) 
              AND (T6.DATAAREAID=N'USRT')) 
              AND (((T6.DOMRUNID=@domRunId) 
              AND (T6.SALESID=T3.SALESID)) 
              AND (T6.RECID=T3.RECID)))))) 
GROUP BY T2.INVENTLOCATIONRECID 
              ORDER BY T2.INVENTLOCATIONRECID

-- Step 4:  Find all orders which pending fulfilment by each warehouse
SELECT T1.RECID, T1.SALESID,
       T2.INVENTLOCATIONRECID, T2.INVENTLOCATIONID
FROM SALESTABLE T1 CROSS 
JOIN #DOMAVAILINVENTLOCATION T2 
WHERE (((T1.PARTITION=5637144576) 
              AND (T1.DATAAREAID=@dataareaid)) 
              AND ((T1.SALESSTATUS=1) 
              OR (T1.SALESSTATUS=2))) 
              AND (((T2.PARTITION=@partition) 
              AND (T2.DATAAREAID=@dataareaid)) 
              AND ((T2.INVENTLOCATIONDATAAREAID=@dataareaid) 
              AND (T2.DOMRUNID=@domRunId))) 
              AND 
EXISTS (
SELECT 'X' 
FROM SALESLINE T3 CROSS 
JOIN INVENTDIM T4 CROSS 
JOIN RETAILSALESLINE T5 
WHERE (((T3.PARTITION=@partition) 
              AND (T3.DATAAREAID=@dataareaid)) 
              AND ((T1.SALESID=T3.SALESID) 
              AND ((T3.SALESSTATUS=1) 
              OR (T3.SALESSTATUS=2)))) 
              AND (((T4.PARTITION=@partition) 
              AND (T4.DATAAREAID=@dataareaid)) 
              AND ((T4.INVENTDIMID=T3.INVENTDIMID) 
              AND (T4.INVENTLOCATIONID=T2.INVENTLOCATIONID))) 
              AND (((T5.PARTITION=@partition) 
              AND (T5.DATAAREAID=@dataareaid)) 
              AND ((T3.RECID=T5.SALESLINE) 
              AND ((T5.FULFILLMENTSTATUS=10) 
              OR (T5.FULFILLMENTSTATUS=1)))) 
              AND 
              NOT (
EXISTS (
SELECT 'X' 
FROM #SalesLinesToProcessed T6 
WHERE (((T6.PARTITION=@partition) 
              AND (T6.DATAAREAID=@dataareaid)) 
              AND (((T6.DOMRUNID=@domRunId) 
              AND (T6.SALESID=T3.SALESID)) 
              AND (T6.RECID=T3.RECID)))))) 
              ORDER BY T2.INVENTLOCATIONRECID


DROP TABLE IF EXISTS #SalesLinesToProcessed
DROP TABLE IF EXISTS #DOMAVAILINVENTLOCATION
