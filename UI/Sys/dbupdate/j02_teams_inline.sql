if exists (select 1 from sysobjects where  id = object_id('j02_teams_inline') and type = 'FN')
 drop function j02_teams_inline
GO


CREATE    FUNCTION [dbo].[j02_teams_inline](@j02id int)
RETURNS nvarchar(2000)
AS
BEGIN
  ---vrac� ��rkou odd�len� n�zvy t�m�, v kter�ch je osoba @j02id

 DECLARE @s nvarchar(2000) 

select @s=COALESCE(@s + ', ', '')+b.j11Name
  FROM j12Team_Person a INNER JOIN j11Team b ON a.j11ID=b.j11ID
  WHERE a.j02ID=@j02id AND b.j11IsAllPersons=0
  ORDER BY j11Name


RETURN(@s)
   
END


GO