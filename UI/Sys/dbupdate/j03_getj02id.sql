if exists (select 1 from sysobjects where  id = object_id('j03_getj02id') and type = 'FN')
 drop function j03_getj02id
GO








CREATE    FUNCTION [dbo].[j03_getj02id](@j03id int)
RETURNS int
AS
BEGIN
  ---vrací j02ID uživatele @j03id

 

  RETURN(select j02ID FROM j03user where j03id=@j03id)
   
END





















GO