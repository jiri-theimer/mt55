if exists (select 1 from sysobjects where  id = object_id('x36userparam_get_mytag') and type = 'P')
 drop procedure x36userparam_get_mytag
GO



CREATE   procedure [dbo].[x36userparam_get_mytag]
@j03id int					--id u�ivatele
,@x36key varchar(50)		--n�zev kl��e
,@clear_after_read bit		--1 - ihned po p�e�ten� vy�istit hodnotu
,@x36value nvarchar(500) OUTPUT	--hodnota kl��e

AS

--Ulo�en� z�znamu u�ivatelsk�ho parametru do tabulky x36UserParam
set @x36value=null

if isnull(@j03id,0)=0
  return

if isnull(@x36key,'')=''
  return

declare @pid int


select top 1 @pid=x36id,@x36value=x36Value from x36UserParam where j03id=@j03id and x36key like @x36key

if @clear_after_read=1 AND @x36value is not null
 UPDATE x36UserParam SET x36value=NULL WHERE x36ID=@pid






























GO