if exists (select 1 from sysobjects where  id = object_id('j03_test_permission_global') and type = 'FN')
 drop function j03_test_permission_global
GO









CREATE    FUNCTION [dbo].[j03_test_permission_global](@j03id int,@x53value int)
RETURNS BIT
AS
BEGIN
  ---vrací 1, pokud uživatel @j03id disponuje oprávnìním v jeho globální roli (j04)
  declare @ret bit,@rolevalue varchar(50)
  set @ret=0

  select @rolevalue=x67.x67RoleValue
  FROM j03User a INNER JOIN j04UserRole j04 ON a.j04ID=j04.j04ID INNER JOIN x67EntityRole x67 ON j04.x67ID=x67.x67ID
  WHERE a.j03ID=@j03id

  if SUBSTRING(@rolevalue,@x53value,1)='1'
   set @ret=1
  
 
  
  RETURN(@ret)
   
END





















GO