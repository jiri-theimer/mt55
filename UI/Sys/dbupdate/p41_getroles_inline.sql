if exists (select 1 from sysobjects where  id = object_id('p41_getroles_inline') and type = 'FN')
 drop function p41_getroles_inline
GO










CREATE    FUNCTION [dbo].[p41_getroles_inline](@p41id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrací èárkou oddìlené obsazení projektových rolí v projektu @p41id

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+ltrim(isnull(j02.j02FirstName+' '+j02.j02LastName,'')+isnull(' '+j11.j11Name,''))+' ('+x67Name+')'
  FROM x67EntityRole x67 INNER JOIN x69EntityRole_Assign x69 ON x67.x67ID=x69.x67ID
  LEFT OUTER JOIN j02Person j02 ON x69.j02ID=j02.j02ID
  LEFT OUTER JOIN j11Team j11 ON x69.j11ID=j11.j11ID
  WHERE x69.x69RecordPID=@p41id AND x67.x29ID=141
  ORDER BY x67Ordinary


RETURN(@s)
   
END

























GO