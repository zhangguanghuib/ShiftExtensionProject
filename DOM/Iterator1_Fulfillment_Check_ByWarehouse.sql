--iterator 1
declare @domRunId    as bigint =  -2191811417208895358; -- DOMRUNId
declare @partition   as bigint =  5637144576;  --Partition
declare @dataareaid  as nvarchar(4) =  N'USRT' -- DataAreaId

--Step 1: Get saleslines to be processed
DROP TABLE IF EXISTS #SalesLinesToProcessed

select T.DOMRUNID,  T.SALESID, soline.RECID, soline.DATAAREAID, soline.PARTITION 
into #SalesLinesToProcessed
from DOMLogs(nolock) as T 
join Salesline as soline on T.SALESID = soline.SALESID and T.SALESLINENUMBER = soline.LINENUM
where T.LOGMESSAGE like '%Preparing to process sales line number%' and T.DOMRUNID = @domRunId

select * from  #SalesLinesToProcessed

--Step 2: Check each warehouse, how many orders pending fulfillment in this warehouse
SELECT COUNT(T1.RECID),
              T2.INVENTLOCATIONRECID 
FROM SALESTABLE T1 CROSS 
JOIN DOMAVAILABLEINVENTLOCATION T2 
WHERE (((T1.PARTITION=@partition) 
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
GROUP BY T2.INVENTLOCATIONRECID 
              ORDER BY T2.INVENTLOCATIONRECID

-- Step 3:  Find all orders which pending fulfilment by each warehouse
SELECT T1.RECID, T1.SALESID,
       T2.INVENTLOCATIONRECID, T2.INVENTLOCATIONID
FROM SALESTABLE T1 CROSS 
JOIN DOMAVAILABLEINVENTLOCATION T2 
WHERE (((T1.PARTITION=@partition) 
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

