-- tworzymy now¹ bazê danych
if exists ( select name
            from sys.databases
            where name = 'Test' )
    begin
        -- dla bezpieczeñstwa roz³¹czmy wszystkich poza nami
        alter database [Test] set single_user with rollback immediate;
        drop database [Test];
    end
go

create database [Test];
go

USE [Test];
GO

DROP PROCEDURE IF EXISTS usp_creating_tabs;
GO

SET NOCOUNT ON;
GO

IF OBJECT_ID('dbo.HappinessTab', 'U') IS NOT NULL
    DROP TABLE HappinessTab;
GO

CREATE TABLE HappinessTab
(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    CountryName NVARCHAR(100),
    Region NVARCHAR(100),
    Score FLOAT
);
GO

INSERT INTO HappinessTab (CountryName, Region, Score)
VALUES
('Finland', 'Western Europe', 7.84),
('Denmark', 'Western Europe', 7.62),
('Switzerland', 'Western Europe', 7.57),
('Iceland', 'Western Europe', 7.55),
('Netherlands', 'Western Europe', 7.46),
('Norway', 'Western Europe', 7.39),
('Sweden', 'Western Europe', 7.36),
('Luxembourg', 'Western Europe', 7.32),
('New Zealand', 'North America and ANZ', 7.28),
('Austria', 'Western Europe', 7.27),
('Australia', 'North America and ANZ', 7.18),
('Israel', 'Middle East and North Africa', 7.16),
('Germany', 'Western Europe', 7.16),
('Canada', 'North America and ANZ', 7.10),
('Ireland', 'Western Europe', 7.09),
('Costa Rica', 'Latin America and Caribbean', 7.07),
('United Kingdom', 'Western Europe', 7.06),
('Czech Republic', 'Central and Eastern Europe', 6.97),
('United States', 'North America and ANZ', 6.95),
('Belgium', 'Western Europe', 6.83),--
('France', 'Western Europe', 6.69),
('Bahrain', 'Middle East and North Africa', 6.65),
('Malta', 'Western Europe', 6.60),
('Taiwan Province of China', 'East Asia', 6.58),
('United Arab Emirates', 'Middle East and North Africa', 6.56),
('Saudi Arabia', 'Middle East and North Africa', 6.50),
('Spain', 'Western Europe', 6.49),
('Italy', 'Western Europe', 6.48),
('Slovenia', 'Central and Eastern Europe', 6.46),
('Guatemala', 'Latin America and Caribbean', 6.44),
('Uruguay', 'Latin America and Caribbean', 6.43),
('Singapore', 'Southeast Asia', 6.38),
('Kosovo', 'Central and Eastern Europe', 6.37),
('Slovakia', 'Central and Eastern Europe', 6.33),
('Brazil', 'Latin America and Caribbean', 6.33),
('Mexico', 'Latin America and Caribbean', 6.32),
('Jamaica', 'Latin America and Caribbean', 6.31),
('Lithuania', 'Central and Eastern Europe', 6.26),
('Cyprus', 'Western Europe', 6.22),
('Estonia', 'Central and Eastern Europe', 6.19),
('Panama', 'Latin America and Caribbean', 6.18),
('Uzbekistan', 'Commonwealth of Independent States', 6.18),
('Chile', 'Latin America and Caribbean', 6.17),
('Poland', 'Central and Eastern Europe', 6.17),
('Kazakhstan', 'Commonwealth of Independent States', 6.15),
('Romania', 'Central and Eastern Europe', 6.14),
('Kuwait', 'Middle East and North Africa', 6.11),
('Serbia', 'Central and Eastern Europe', 6.08),
('El Salvador', 'Latin America and Caribbean', 6.06),
('Mauritius', 'Sub-Saharan Africa', 6.05),
('Latvia', 'Central and Eastern Europe', 6.03),
('Colombia', 'Latin America and Caribbean', 6.01),
('Hungary', 'Central and Eastern Europe', 5.99),
('Thailand', 'Southeast Asia', 5.99),
('Nicaragua', 'Latin America and Caribbean', 5.97),
('Japan', 'East Asia', 5.94),
('Argentina', 'Latin America and Caribbean', 5.93),
('Portugal', 'Western Europe', 5.93),
('Honduras', 'Latin America and Caribbean', 5.92),
('Croatia', 'Central and Eastern Europe', 5.88),
('Philippines', 'Southeast Asia', 5.88),
('South Korea', 'East Asia', 5.85),
('Peru', 'Latin America and Caribbean', 5.84),
('Bosnia and Herzegovina', 'Central and Eastern Europe', 5.81),
('Moldova', 'Commonwealth of Independent States', 5.77),
('Ecuador', 'Latin America and Caribbean', 5.76),
('Kyrgyzstan', 'Commonwealth of Independent States', 5.74),
('Greece', 'Western Europe', 5.72),
('Bolivia', 'Latin America and Caribbean', 5.72),
('Mongolia', 'East Asia', 5.68),
('Paraguay', 'Latin America and Caribbean', 5.65),
('Montenegro', 'Central and Eastern Europe', 5.58),
('Dominican Republic', 'Latin America and Caribbean', 5.55),
('North Cyprus', 'Western Europe', 5.54),
('Belarus', 'Commonwealth of Independent States', 5.53),
('Russia', 'Commonwealth of Independent States', 5.48),
('Hong Kong S.A.R. of China', 'East Asia', 5.48),
('Tajikistan', 'Commonwealth of Independent States', 5.47),
('Vietnam', 'Southeast Asia', 5.41),
('Libya', 'Middle East and North Africa', 5.41),
('Malaysia', 'Southeast Asia', 5.38),
('Indonesia', 'Southeast Asia', 5.35),
('Congo (Brazzaville)', 'Sub-Saharan Africa', 5.34),
('China', 'East Asia', 5.34),
('Ivory Coast', 'Sub-Saharan Africa', 5.31),
('Armenia', 'Commonwealth of Independent States', 5.28),
('Nepal', 'South Asia', 5.27),
('Bulgaria', 'Central and Eastern Europe', 5.27),
('Maldives', 'South Asia', 5.20),
('Azerbaijan', 'Commonwealth of Independent States', 5.17),
('Cameroon', 'Sub-Saharan Africa', 5.14),
('Senegal', 'Sub-Saharan Africa', 5.13),
('Albania', 'Central and Eastern Europe', 5.12),
('North Macedonia', 'Central and Eastern Europe', 5.10),
('Ghana', 'Sub-Saharan Africa', 5.09),
('Niger', 'Sub-Saharan Africa', 5.07),
('Turkmenistan', 'Commonwealth of Independent States', 5.07),
('Gambia', 'Sub-Saharan Africa', 5.05),
('Benin', 'Sub-Saharan Africa', 5.05),
('Laos', 'Southeast Asia', 5.03),
('Bangladesh', 'South Asia', 5.03),
('Guinea', 'Sub-Saharan Africa', 4.98),
('South Africa', 'Sub-Saharan Africa', 4.96),
('Turkey', 'Middle East and North Africa', 4.58),
('Pakistan', 'South Asia', 4.93),
('Morocco', 'Middle East and North Africa', 4.92),
('Venezuela', 'Latin America and Caribbean', 4.89),
('Georgia', 'Commonwealth of Independent States', 4.89),
('Algeria', 'Middle East and North Africa', 4.89),
('Ukraine', 'Commonwealth of Independent States', 4.88),
('Iraq', 'Middle East and North Africa', 4.85),
('Gabon', 'Sub-Saharan Africa', 4.85),
('Burkina Faso', 'Sub-Saharan Africa', 4.83),
('Cambodia', 'Southeast Asia', 4.83),
('Mozambique', 'Sub-Saharan Africa', 4.79),
('Nigeria', 'Sub-Saharan Africa', 4.76),
('Mali', 'Sub-Saharan Africa', 4.72),
('Iran', 'Middle East and North Africa', 4.72),
('Uganda', 'Sub-Saharan Africa', 4.64),
('Liberia', 'Sub-Saharan Africa', 4.63),
('Kenya', 'Sub-Saharan Africa', 4.61),
('Tunisia', 'Middle East and North Africa', 4.60),
('Lebanon', 'Middle East and North Africa', 4.58),
('Namibia', 'Sub-Saharan Africa', 4.57),
('Palestinian Territories', 'Middle East and North Africa', 4.52),
('Myanmar', 'Southeast Asia', 4.43),
('Jordan', 'Middle East and North Africa', 4.40),
('Chad', 'Sub-Saharan Africa', 4.36),
('Sri Lanka', 'South Asia', 4.33),
('Swaziland', 'Sub-Saharan Africa', 4.31),
('Comoros', 'Sub-Saharan Africa', 4.29),
('Egypt', 'Middle East and North Africa', 4.28),
('Ethiopia', 'Sub-Saharan Africa', 4.28),
('Mauritania', 'Sub-Saharan Africa', 4.23),
('Madagascar', 'Sub-Saharan Africa', 4.21),
('Togo', 'Sub-Saharan Africa', 4.11),
('Zambia', 'Sub-Saharan Africa', 4.07),
('Sierra Leone', 'Sub-Saharan Africa', 3.85),
('India', 'South Asia', 3.82),
('Burundi', 'Sub-Saharan Africa', 3.78),
('Yemen', 'Middle East and North Africa', 3.66),
('Tanzania', 'Sub-Saharan Africa', 3.62),
('Haiti', 'Latin America and Caribbean', 3.62),
('Malawi', 'Sub-Saharan Africa', 3.60),
('Lesotho', 'Sub-Saharan Africa', 3.51),
('Botswana', 'Sub-Saharan Africa', 3.47),
('Rwanda', 'Sub-Saharan Africa', 3.42),
('Zimbabwe', 'Sub-Saharan Africa', 3.15),
('Afghanistan', 'South Asia', 2.52);
GO

SELECT TOP 10 * FROM HappinessTab ORDER BY Score DESC;
SELECT COUNT(*) AS TotalCountries FROM HappinessTab;




