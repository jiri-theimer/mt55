if exists (select 1 from sysobjects where  id = object_id('p91_fpr_recalc_all_invoices') and type = 'P')
 drop procedure p91_fpr_recalc_all_invoices
GO


CREATE procedure [dbo].[p91_fpr_recalc_all_invoices]
@p51id int
,@d1 datetime
,@d2 datetime
AS
declare @p91id int

if isnull(@p51id,0)=0
 begin
  if not exists(select x35ID FROM x35GlobalParam WHERE x35Key LIKE 'p51ID_FPR')
   return

  select @p51id=convert(int,x35Value) FROM x35GlobalParam WHERE x35Key LIKE 'p51ID_FPR'
 end


DECLARE curRPR CURSOR FOR 
SELECT p91id from p91invoice WHERE p91DateSupply BETWEEN @d1 AND @d2

OPEN curRPR
FETCH NEXT FROM curRPR 
INTO @p91id
WHILE @@FETCH_STATUS = 0
BEGIN
  exec [p91_fpr_recalc_invoice] @p51id,@p91id

  FETCH NEXT FROM curRPR 
  INTO @p91id
END
CLOSE curRPR
DEALLOCATE curRPR

GO