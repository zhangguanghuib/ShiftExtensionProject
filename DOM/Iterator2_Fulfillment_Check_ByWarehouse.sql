
--iterator 2
declare @domRunId    as bigint =  -7010923937246784838; -- DOMRUNId
declare @partition   as bigint =  5637144576;  --Partition
declare @dataareaid  as nvarchar(4) =  N'USRT' -- DataAreaId

--select * from DOMAvailableInventLocation(nolock) T where T.DOMRUNID = -7010923937246784838
--select * from DOMSalesLineToProcess(nolock) T where T.DOMRUNID = -7010923937246784838 
--select * from DOMItemInventLocation(nolock) T where T.DOMRUNID = -7010923937246784838

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
FROM DOMSALESLINETOPROCESS T6 
WHERE (((T6.PARTITION=@partition) 
	AND (T6.DATAAREAID=@dataareaid)) 
	AND (((T6.DOMRUNID=@domRunId) 
	AND (T6.SALESID=T3.SALESID)) 
	AND (T6.SALESLINERECID=T3.RECID)))))) 
GROUP BY T2.INVENTLOCATIONRECID 
	ORDER BY T2.INVENTLOCATIONRECID


-- Step 2:  Find all orders which pending fulfilment by each warehouse
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
FROM DOMSALESLINETOPROCESS T6 
WHERE (((T6.PARTITION=@partition) 
	AND (T6.DATAAREAID=@dataareaid)) 
	AND (((T6.DOMRUNID=@domRunId) 
	AND (T6.SALESID=T3.SALESID)) 
	AND (T6.SALESLINERECID=T3.RECID)))))) 
	ORDER BY T2.INVENTLOCATIONRECID