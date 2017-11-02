if exists (select 1 from sysobjects where  id = object_id('o22_delete') and type = 'P')
 drop procedure o22_delete
GO






CREATE   procedure [dbo].[o22_delete]
@j03id_sys int				--pøihlášený uživatel
,@pid int					--o22id
,@err_ret varchar(500) OUTPUT		---pøípadná návratová chyba

AS
--odstranìní záznamu milníku z tabulky o22Milestone


if isnull(@err_ret,'')<>''
 return 

BEGIN TRANSACTION

BEGIN TRY
	
	if exists(select o19ID FROM o19Milestone_NonPerson WHERE o22ID=@pid)
	 DELETE FROM o19Milestone_NonPerson WHERE o22ID=@pid

	if exists(select o20ID FROM o20Milestone_Receiver WHERE o22ID=@pid)
	 DELETE FROM o20Milestone_Receiver WHERE o22ID=@pid

	if exists(select o27ID FROM o27Attachment WHERE b07ID IN (SELECT b07ID FROM b07Comment WHERE x29ID=222 AND b07RecordPID=@pid))
	 DELETE FROM o27Attachment WHERE b07ID IN (SELECT b07ID FROM b07Comment WHERE x29ID=222 AND b07RecordPID=@pid)

    if exists(select b07ID FROM b07Comment WHERE x29ID=223 AND b07RecordPID=@pid)
	 DELETE FROM b07Comment WHERE x29ID=222 AND b07RecordPID=@pid

	delete from o22Milestone where o22ID=@pid

	COMMIT TRANSACTION

END TRY
BEGIN CATCH
  set @err_ret=dbo.parse_errinfo(ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE())
  ROLLBACK TRANSACTION
  
END CATCH  

















GO