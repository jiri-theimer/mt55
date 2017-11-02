if exists (select 1 from sysobjects where  id = object_id('c26_aftersave') and type = 'P')
 drop procedure c26_aftersave
GO





CREATE   procedure [dbo].[c26_aftersave]
@c26id int
,@j03id_sys int
AS
---aktualizovat rozpis dnù ve fondu, kam má vliv den svátku

declare @j17id int

select @j17id=j17ID FROM c26Holiday WHERE c26ID=@c26id


declare @c21id int


DECLARE curC21 CURSOR FOR 
select c21ID from c21FondCalendar
OPEN curC21
FETCH NEXT FROM curC21  INTO @c21id
WHILE @@FETCH_STATUS = 0
BEGIN

 exec c21_recovery @c21id,null

FETCH NEXT FROM curC21 INTO @c21id
END

CLOSE curC21
DEALLOCATE curC21





DECLARE curC17 CURSOR FOR 
select j17id from j17Country
OPEN curC17
FETCH NEXT FROM curC17  INTO @j17id
WHILE @@FETCH_STATUS = 0
BEGIN

 exec c21_recovery @c21id,@j17id

FETCH NEXT FROM curC17 INTO @j17id
END

CLOSE curC17
DEALLOCATE curC17






GO