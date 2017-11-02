if exists (select 1 from sysobjects where  id = object_id('parse_errinfo') and type = 'FN')
 drop function parse_errinfo
GO



CREATE    FUNCTION [dbo].[parse_errinfo](@ERROR_PROCEDURE nvarchar(500),@ERROR_LINE int,@ERROR_MESSAGE nvarchar(3000))
RETURNS nvarchar(4000)
AS
BEGIN
  ---vrací login uživatele @j03id
 declare @s nvarchar(4000)
 set @s='Procedure: '+@ERROR_PROCEDURE+char(13)+char(10)+'<hr>Line: '+convert(varchar(10),@ERROR_LINE)+char(13)+char(10)+'<hr>'+@ERROR_MESSAGE

 return(@s)
   
END























GO