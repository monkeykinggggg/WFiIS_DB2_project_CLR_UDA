USE [Test];
GO

DROP PROCEDURE IF EXISTS usp_GetRegions;
GO

CREATE PROCEDURE usp_GetRegions AS
BEGIN
	SELECT	ROW_NUMBER() OVER (ORDER BY Region) AS Id, Region
    FROM (
		SELECT DISTINCT  Region
		FROM dbo.HappinessTab
		WHERE Region IS NOT NULL
	) AS	DistinctRegions;
END;
GO


DROP PROCEDURE IF EXISTS usp_GetAggregatesNames;
GO

CREATE PROCEDURE usp_GetAggregatesNames AS
BEGIN
	SELECT o.name AS AggregateFunctionName
	FROM sys.assembly_modules am
	JOIN sys.objects o ON am.object_id = o.object_id
	WHERE o.type = 'AF'; 
END;
GO
