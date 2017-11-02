if exists (select 1 from sysobjects where  id = object_id('x38_delete') and type = 'P')
 drop procedure x38_delete
GO









CREATE   procedure [dbo].[x38_delete]
@j03id_sys int				--p�ihl�en� u�ivatel
,@pid int					--x38id
,@err_ret varchar(500) OUTPUT		---p��padn� n�vratov� chyba

AS
--odstran�n� z�znamu ��seln� �ady z tabulky x38CodeLogic
declare @ref_pid int



SELECT TOP 1 @ref_pid=p42ID from p42ProjectType WHERE x38ID=@pid
if @ref_pid is not null
 set @err_ret='Minim�ln� jeden typ projektu je sv�zan� s touto ��selnou �adou.'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p29ID from p29ContactType WHERE x38ID=@pid
if @ref_pid is not null
 set @err_ret='Minim�ln� jeden typ kontaktu je sv�zan� s touto ��selnou �adou.'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p92ID from p92InvoiceType WHERE x38ID=@pid
if @ref_pid is not null
 set @err_ret='Minim�ln� jeden typ faktury je sv�zan� s touto ��selnou �adou.'

set @ref_pid=null
SELECT TOP 1 @ref_pid=p89ID from p89ProformaType WHERE x38ID=@pid
if @ref_pid is not null
 set @err_ret='Minim�ln� jeden typ z�lohy je sv�zan� s touto ��selnou �adou.'


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY	

	delete from x38CodeLogic where x38ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  
















GO