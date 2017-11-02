if exists (select 1 from sysobjects where  id = object_id('GetNextMonth') and type = 'FN')
 drop function GetNextMonth
GO





CREATE FUNCTION [dbo].[GetNextMonth] (@d datetime)
--vrací datum o mìsíc posunutý vùèi @d
RETURNS datetime AS  
BEGIN 

declare @dx datetime

set @dx=dateadd(day,1,@d)

if month(@d)=month(@dx)
 RETURN(dateadd(mm,1,@d))

declare @m int
set @m=month(@dx)


while month(@dx)=@m
begin
 set @dx=dateadd(day,1,@dx)
 
end

set @dx=dateadd(day,-1,@dx)

RETURN(@dx)
	
END












GO