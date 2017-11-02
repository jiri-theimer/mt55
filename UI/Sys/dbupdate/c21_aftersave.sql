if exists (select 1 from sysobjects where  id = object_id('c21_aftersave') and type = 'P')
 drop procedure c21_aftersave
GO






CREATE   procedure [dbo].[c21_aftersave]
@c21id int
,@j03id_sys int
AS
---aktualizovat rozpis dnù ve fondu do regionálních instancí kalendáøù
declare @count int
select @count=count(*) from c21FondCalendar

if @count=1
 begin
   truncate table c22FondCalendar_Date
 end

exec c21_recovery @c21id,null

declare @j17id int

DECLARE curJ17 CURSOR FOR 
select j17id from j17Country
OPEN curJ17
FETCH NEXT FROM curJ17  INTO @j17id
WHILE @@FETCH_STATUS = 0
BEGIN

 exec c21_recovery @c21id,@j17id

FETCH NEXT FROM curJ17 INTO @j17id
END

CLOSE curJ17
DEALLOCATE curJ17






GO