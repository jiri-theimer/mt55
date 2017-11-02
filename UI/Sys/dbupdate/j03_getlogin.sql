if exists (select 1 from sysobjects where  id = object_id('j03_getlogin') and type = 'FN')
 drop function j03_getlogin
GO








CREATE    FUNCTION [dbo].[j03_getlogin](@j03id int)
RETURNS nvarchar(50)
AS
BEGIN
  ---vrací login uživatele @j03id

 

  RETURN(select j03login FROM j03user where j03id=@j03id)
   
END




















GO