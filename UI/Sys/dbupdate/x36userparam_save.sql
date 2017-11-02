if exists (select 1 from sysobjects where  id = object_id('x36userparam_save') and type = 'P')
 drop procedure x36userparam_save
GO






CREATE   procedure [dbo].[x36userparam_save]
@j03id int					--id uživatele
,@x36key varchar(50)		--název klíèe
,@x36value nvarchar(500)	--hodnota klíèe

AS

--Uložení záznamu uživatelského parametru do tabulky x36UserParam

if isnull(@j03id,0)=0
  return

if isnull(@x36key,'')=''
  return

declare @pid int,@login nvarchar(50),@x36validuntil datetime

set @login=dbo.j03_getlogin(@j03id)
set @x36validuntil=convert(datetime,'01.01.3000',104)

select top 1 @pid=x36id from x36UserParam where j03id=@j03id and x36key like @x36key

if @pid is not null
   update x36UserParam set x36value=@x36value,x36dateupdate=getdate(),x36validuntil=@x36validuntil,x36userupdate=@login where x36id=@pid
else
   insert into x36UserParam(j03id,x36value,x36key,x36dateinsert,x36dateupdate,x36userinsert,x36userupdate,x36validfrom,x36validuntil) values(@j03id,@x36value,@x36key,getdate(),getdate(),@login,@login,getdate(),@x36validuntil)





























GO