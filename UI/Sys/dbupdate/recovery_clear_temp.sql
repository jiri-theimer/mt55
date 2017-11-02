if exists (select 1 from sysobjects where  id = object_id('recovery_clear_temp') and type = 'P')
 drop procedure recovery_clear_temp
GO








CREATE PROCEDURE [dbo].[recovery_clear_temp]
AS



truncate table p85TempBox


	
	




GO