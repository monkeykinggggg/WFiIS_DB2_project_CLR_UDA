USE [Test];
GO

-- dropping aggregates
IF EXISTS (SELECT name
           FROM sysobjects
           WHERE name = 'Mode')
    DROP AGGREGATE Mode;
GO
IF EXISTS (SELECT name
           FROM sysobjects
           WHERE name = 'StdDev')
    DROP AGGREGATE StdDev;
GO
IF EXISTS (SELECT name
           FROM sysobjects
           WHERE name = 'Median')
    DROP AGGREGATE Median;
GO
IF EXISTS (SELECT name
           FROM sysobjects
           WHERE name = 'Quantile')
    DROP AGGREGATE Quantile;
GO
IF EXISTS (SELECT name
           FROM sysobjects
           WHERE name = 'Range')
    DROP AGGREGATE [Range];
GO

-- dropping assembly
IF EXISTS (SELECT name
           FROM sys.assemblies
           WHERE name = 'HapinessAggregates')
    DROP ASSEMBLY HapinessAggregates;
GO

-- creating assembly along with aggregates
CREATE ASSEMBLY HapinessAggregates
FROM 'C:\Users\Administrator\source\repos\Empty_Solution\UDAs\bin\Release\UDAs.dll'
WITH PERMISSION_SET = SAFE;
GO

CREATE AGGREGATE Mode(@Value float)
RETURNS FLOAT
EXTERNAL NAME HapinessAggregates.[UDAs.Mode];
GO

CREATE AGGREGATE StdDev(@Value float)
RETURNS FLOAT
EXTERNAL NAME HapinessAggregates.[UDAs.StdDev];
GO

CREATE AGGREGATE Median(@Value float)
RETURNS FLOAT
EXTERNAL NAME HapinessAggregates.[UDAs.Median];
GO


CREATE AGGREGATE Quantile(@Value float, @Q float)
RETURNS FLOAT
EXTERNAL NAME HapinessAggregates.[UDAs.Quantile];
GO

CREATE AGGREGATE Range(@Value float)
RETURNS FLOAT
EXTERNAL NAME HapinessAggregates.[UDAs.Range];
GO



