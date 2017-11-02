if exists (select 1 from sysobjects where  id = object_id('x90_appendlog') and type = 'P')
 drop procedure x90_appendlog
GO




CREATE PROCEDURE [dbo].[x90_appendlog]
@x29id int
,@pid int
,@j03id_sys int

AS

declare @j02id_sys int,@validfrom datetime,@validuntil datetime,@dateinsert datetime,@x90id_last int,@validfrom_last datetime,@validuntil_last datetime

select @j02id_sys=dbo.j03_getj02id(@j03id_sys)

if @x29id=141
 select @validfrom=p41ValidFrom,@validuntil=p41ValidUntil,@dateinsert=p41DateInsert FROM p41Project WHERE p41ID=@pid

if @x29id=328
 select @validfrom=p28ValidFrom,@validuntil=p28ValidUntil,@dateinsert=p28DateInsert FROM p28Contact WHERE p28ID=@pid

if @x29id=102
 select @validfrom=j02ValidFrom,@validuntil=j02ValidUntil,@dateinsert=j02DateInsert FROM j02Person WHERE j02ID=@pid


declare @flag int
set @flag=2

select TOP 1 @x90id_last=x90ID,@validfrom_last=x90RecordValidFrom,@validuntil_last=x90RecordValidUntil
FROM x90EntityLog
WHERE x29ID=@x29id AND x90RecordPID=@pid
ORDER BY x90ID DESC

if @validuntil<>@validuntil_last AND getdate()>=@validuntil
 set @flag=3	---záznam byl pøesunut do koše

if @validuntil<>@validuntil_last AND getdate()<@validuntil
 set @flag=4	---záznam byl obnoven z koše

if not exists(select x90ID FROM x90EntityLog WHERE x29ID=@x29id AND x90RecordPID=@pid AND x90EventFlag=1)
BEGIN
 INSERT INTO x90EntityLog(x29ID,x90RecordPID,j03ID_Author,x90Date,x90EventFlag,j02ID_Author,x90RecordValidFrom,x90RecordValidUntil) VALUES(@x29id,@pid,@j03id_sys,@dateinsert,1,@j02id_sys,@dateinsert,convert(datetime,'01.01.3000',104))
END



INSERT INTO x90EntityLog(x29ID,x90RecordPID,j03ID_Author,x90Date,x90EventFlag,j02ID_Author,x90RecordValidFrom,x90RecordValidUntil)
VALUES(@x29id,@pid,@j03id_sys,getdate(),@flag,@j02id_sys,@validfrom,@validuntil)





GO