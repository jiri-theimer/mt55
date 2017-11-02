if exists (select 1 from sysobjects where  id = object_id('GetNextQuarter') and type = 'FN')
 drop function GetNextQuarter
GO






CREATE FUNCTION [dbo].[GetNextQuarter] (@d datetime)
--vrac� datum o kvart�l posunut� v��i @d
RETURNS datetime AS  
BEGIN 

declare @dx datetime


set @dx=dbo.GetNextMonth(@d)
set @dx=dbo.GetNextMonth(@dx)
set @dx=dbo.GetNextMonth(@dx)

RETURN(@dx)
	
END













GO