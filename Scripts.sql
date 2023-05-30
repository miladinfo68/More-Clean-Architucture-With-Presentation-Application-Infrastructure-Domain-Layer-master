USE master;
GO
IF EXISTS (SELECT 1 FROM sys.databases WHERE [name] = N'TradeDb')
BEGIN
    ALTER DATABASE TradeDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TradeDb;
END;
GO

----DROP DATABASE IF EXISTS TradeDb;

CREATE DATABASE TradeDb;
GO

ALTER DATABASE [TradeDb] COLLATE Arabic_100_BIN
GO

USE TradeDb
GO

CREATE TABLE dbo.Instrument
(
	Id INT identity(1,1)  ,
	[Name] VARCHAR(255) ,
	CONSTRAINT PK_InstrumentId PRIMARY KEY (Id)
);

CREATE TABLE dbo.Trade
(
	Id INT identity(1,1)  ,
	InstrumentId INT , 
	DateEn DATETIME , 
	[Open] DECIMAL(19,4) , 
	[High] DECIMAL(19,4), 
	[Low] DECIMAL(19,4) ,
	[Close] DECIMAL(19,4) ,
	CONSTRAINT PK_TradeId PRIMARY KEY (Id)
);
GO

CREATE TYPE dbo.Tvp_Trade AS TABLE( Id INT)
GO

ALTER TABLE dbo.Trade 
ADD CONSTRAINT FK_Trade_Instrument 
FOREIGN KEY(InstrumentId) 
REFERENCES dbo.Trade(Id) 
ON UPDATE  NO ACTION 
ON DELETE  NO ACTION 	

GO

ALTER TABLE dbo.Trade
NOCHECK CONSTRAINT FK_Trade_Instrument
GO

CREATE NONCLUSTERED INDEX NCIX_InstrumentId_DateEn ON Trade (InstrumentId ASC) INCLUDE (DateEn)
GO

CREATE OR ALTER VIEW dbo.vw_CustomizeSysFunctions
AS
SELECT 
 RAND()  AS [RandValue]
,NEWID() AS [GuidValue]
,RAND(CHECKSUM(NEWID())) AS [SeededRand]

GO




CREATE OR ALTER FUNCTION dbo.udf_GenerateRandomDateTime (@StartDate DATETIME='2022-01-01', @EndDate DATETIME='2022-12-31')
RETURNS DATETIME
AS
BEGIN
    --DECLARE @RandomDays INT = ABS(CHECKSUM(NEWID())) % DATEDIFF(DAY, @StartDate, @EndDate)
    DECLARE @NID UNIQUEIDENTIFIER = (SELECT v.GuidValue FROM dbo.vw_CustomizeSysFunctions v)
	DECLARE @RandomDays INT = ABS(CHECKSUM(@NID)) % DATEDIFF(DAY, @StartDate, @EndDate)
    DECLARE @RandomDateTime DATETIME = DATEADD(DAY, @RandomDays, @StartDate)

    RETURN @RandomDateTime
	   
END

GO

CREATE OR ALTER FUNCTION dbo.udf_GenerateRandomDecimal(@MinValue DECIMAL(19, 4)=1000.0000, @MaxValue DECIMAL(19, 4)=1200.0000)
RETURNS DECIMAL(19, 4)
AS
BEGIN
	DECLARE @RandomDecimal DECIMAL(19, 4)= CAST ( @MinValue + ((@MaxValue - @MinValue) * (SELECT v.SeededRand FROM dbo.vw_CustomizeSysFunctions v)) AS DECIMAL(19, 4) ) 
	RETURN @RandomDecimal
	--SELECT (CAST ( 1000.0000 + ((1200.0000 - 1000.0000) * (SELECT v.SeededRand FROM dbo.vw_CustomizeSysFunctions v)) AS DECIMAL(19, 4) ) )
	--SELECT CAST ( 1000.0000 + ((1200.0000 - 1000.0000) *  RAND(CHECKSUM(NEWID())) ) AS DECIMAL(19, 4) ) 
END


GO

/*




INSERT INTO Instrument 
VALUES('AAPL') , ('GOOGL');
GO

INSERT INTO Instrument([Name])VALUES
 (N'قچین')
,(N'حتاید')
,(N'شقدیر')
,(N'خساپا')
,(N'زرنان')
,(N'سخاف')
,(N'ایرانخ')
,(N'برنج')
,(N'سیمان')
,(N'شوینده')
,(N'AAPL') 
,(N'GOOGL')

GO  

INSERT INTO Trade 
VALUES
(1, '2020-01-01', 1001, 2001, 301, 401),
(1, '2020-01-02', 1002, 2002, 302, 402),
(1, '2020-01-03', 1003, 2003, 303, 403),
(2, '2020-01-01', 1004, 2004, 304, 404),
(2, '2020-01-03', 1005, 2005, 305, 405),
(5, '2020-01-01', 1006, 2006, 306, 406),
(1, '2021-01-01', 1007, 2007, 307, 407),
(1, '2022-01-01', 1007.12, 2007.23, 307.1, 407.44);
GO 

SELECT top 100 * FROM Instrument
SELECT top 100 * FROM Trade 

GO

SELECT * FROM dbo.Trade

Truncate Table  dbo.Instrument
SELECT * FROM dbo.Instrument


INSERT INTO Instrument ([Name])
SELECT TOP 1000 LEFT(REPLACE(NEWID(),'-',''),20) AS Random10
FROM sys.all_columns ac1
CROSS JOIN sys.all_columns ac2

GO


*/

GO
-------------------------------------
--EXEC SP_GetLastTrades

CREATE PROC SP_GetLastTrades
(
	@Id INT=-1 
	,@Tvp_Trade Tvp_Trade READONLY
)
AS
BEGIN
	--DECLARE @Id INT =11833
	--DECLARE @Tvp_Trade TABLE(Id INT)
	--INSERT INTO @Tvp_Trade(Id) VALUES(54204),(50196),(25161)

	--SELECT * FROM @Tvp_Trade

	;WITH cteLastTrades AS(

		SELECT t.rn,t.InstrumentId ,t.DateEn ,t.[Open] ,t.[High] ,t.[Low] ,t.[Close],t.Id
		FROM 
		(
			SELECT 	*,	ROW_NUMBER()OVER(PARTITION BY InstrumentId ORDER BY DateEn DESC) rn
			FROM Trade 
		) AS t
		WHERE 
		t.rn = 1
	)
	--SELECT * FROM cteLastTrades

	, cteFilteredTradesById AS (
		SELECT 	t.InstrumentId ,t.DateEn ,t.[Open] ,t.[High] ,t.[Low] ,t.[Close],t.Id
		FROM cteLastTrades t
		WHERE ((@Id <>-1 AND t.Id=@Id) OR (@Id=-1 ))
	)
	--SELECT * FROM cteFilteredTradesById

	, cteLastFilteredTrades AS (
		SELECT 	t.InstrumentId ,t.DateEn ,t.[Open] ,t.[High] ,t.[Low] ,t.[Close],t.Id
		FROM cteLastTrades t
		JOIN @Tvp_Trade tvp ON t.Id=tvp.Id
	)
	--SELECT * FROM cteLastFilteredTrades

	,  cteResult AS(

		SELECT i.[Name],t.DateEn ,t.[Open] ,t.[High] ,t.[Low] ,t.[Close],t.Id
		FROM
		(
			SELECT 	t.InstrumentId ,t.DateEn ,t.[Open] ,t.[High] ,t.[Low] ,t.[Close],t.Id
			FROM cteFilteredTradesById t
		
			UNION ALL

			SELECT 	ft.InstrumentId ,ft.DateEn ,ft.[Open] ,ft.[High] ,ft.[Low] ,ft.[Close],ft.Id
			FROM cteLastFilteredTrades ft
		) t
		JOIN Instrument i on t.InstrumentId = i.Id

	)

	SELECT * FROM cteResult

END 

GO

---- EXEC SP_GetLastTrades
--exec SP_BulkInsertTrades 1000000
--drop proc SP_BulkInsertTrades



CREATE PROC SP_BulkInsertTrades
(
	@TotalRecords INT = 10000000
	,@ChunkSize INT = 100000
)
AS
BEGIN
    SET NOCOUNT ON;
	
	--DECLARE @TotalRecords INT = 1000; 
	--DECLARE @ChunkSize INT = 501; 

	DECLARE @TotalChunkSize INT =0;
	DECLARE @Open DECIMAL(19, 4) 
			,@Colse DECIMAL(19, 4)
			,@High DECIMAL(19, 4)
			,@Low DECIMAL(19, 4);

	DECLARE @IsCompleted BIT =0;

	DECLARE @HighGap FLOAT =1.05 ;--> +5%
	DECLARE @LowGap FLOAT =0.95 ;--> -5%

	DECLARE @StartDate DATETIME ='2022-01-01'
	DECLARE @EndDate DATETIME ='2022-12-30'

	DECLARE @StartOpen DECIMAL(19, 4)=1800
	DECLARE @EndOpen DECIMAL(19, 4)=1500

	DECLARE @CandidateInstrumentId INT

	IF(@TotalRecords < @ChunkSize )	
		SET @ChunkSize = @TotalRecords;

	WHILE  @IsCompleted <>1
	BEGIN
		SET @TotalChunkSize = @TotalChunkSize + @ChunkSize;

		IF(@TotalRecords <= @TotalChunkSize)
		BEGIN
			SET @ChunkSize= @ChunkSize - ( @TotalChunkSize - @TotalRecords )
			SET @TotalChunkSize = @TotalRecords
			SET @IsCompleted =1
		END

		;WITH loop_LargeDataSet AS(
			SELECT TOP (@ChunkSize) 
				ROW_NUMBER() OVER (ORDER BY NEWID()) AS rn ,
				Id AS InstrumentId ,
				( SELECT DATEADD(DAY , ABS(CHECKSUM(NEWID())) % DATEDIFF(DAY, @StartDate, @EndDate) ,@StartDate)) AS DateEn,
				( SELECT CAST ( @StartOpen + ((@EndOpen - @StartOpen) *  RAND(CHECKSUM(NEWID())) ) AS DECIMAL(19, 4) )  ) AS [Open]
			FROM Instrument
			CROSS JOIN (
				SELECT TOP (@ChunkSize) 
				ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
				FROM sys.columns c1
				CROSS JOIN sys.columns c2
			) AS rnd
		)
		--SELECT * FROM loop_LargeDataSet

		,InstrumentChunkData AS (
			SELECT TOP (@ChunkSize)
				 r.rn
				,r. InstrumentId 
				,r.DateEn 
				,r.[Open] 
				,r.[Open] * (@LowGap)  AS [Low]
				,r.[Open] * (@HighGap) AS [High]
				,(SELECT dbo.udf_GenerateRandomDecimal(r.[Open] * @LowGap , r.[Open] * @HighGap )) AS [Close]
			FROM loop_LargeDataSet r
			ORDER BY NEWID()
		)

		INSERT INTO Trade (InstrumentId, DateEn, [Open], [High], [Low], [Close])
		SELECT t.InstrumentId, t.DateEn, t.[Open], t.[High], t.[Low], t.[Close] FROM InstrumentChunkData t
							
	END

	SELECT @TotalRecords
END

GO


--EXEC SP_GetRangeTrades 200
CREATE OR ALTER PROC SP_GetRangeTrades(@top INT=10000)
AS
BEGIN
	SELECT top (@top)
	t.Id
	--i.[Name] ,t.DateEn ,t.[Open] ,t.[High] ,t.[Low] ,t.[Close],t.Id
	FROM 
	(
		SELECT 	*,	ROW_NUMBER()OVER(PARTITION BY InstrumentId ORDER BY DateEn DESC) rn
		FROM Trade 
	) AS t
	JOIN Instrument i on t.InstrumentId = i.Id
	WHERE 
	t.rn = 1
END





/*

select  * from Trade

--truncate table Trade
--truncate table Instrument

*/


--SELECT NEWID() ,CHECKSUM(NEWID()) ,ABS(CHECKSUM(NEWID())) ,ABS(CHECKSUM(NEWID())) % 100 ,ABS(CHECKSUM(NEWID())) % 100 + 1
--SELECT RAND()  ,RAND() * 10 ,RAND() * 100 ,RAND() * 1000 





