if exists (select 1 from sysobjects where  id = object_id('p32_get_vatrate') and type = 'FN')
 drop function p32_get_vatrate
GO


CREATE  FUNCTION [dbo].[p32_get_vatrate](@p32id int,@p41id int,@dat datetime)
RETURNS float
AS
BEGIN
   
  declare @ret float,@x15id int,@j18id int,@p53id int,@p51id int,@j27id int,@j17id int

  if ISNULL(@p41id,0)<>0
   begin
    select @j18id=j18id,@p51id=p51ID_Billing from p41project where p41id=@p41id

	if @p51id is not null
	 select @j27id=j27ID FROM p51PriceList WHERE p51ID=@p51id

	if @j18id is not null
	 select @j17id=j17ID FROM j18Region WHERE j18ID=@j18id

   end

  if @j27id is null
   select @j27id=convert(int,x35Value) FROM x35GlobalParam WHERE x35Key LIKE 'j27ID_Domestic' AND ISNUMERIC(x35Value)=1

  if exists(select p32id from p32Activity where p32ID=@p32id and x15ID is not null)
    select @x15id=isnull(x15ID,@x15id) from p32activity where p32id=@p32id

  if @x15id is null
   select @x15id=convert(int,x35Value) from x35GlobalParam where x35Key LIKE 'x15ID'
  
  set @x15id=isnull(@x15id,3)

  
  if isnull(@j17id,0)<>0 
    select top 1 @p53id=p53id,@ret=p53value from p53VatRate where j27ID=@j27id AND j17ID=@j17id and x15id=@x15id and p53validfrom<=@dat AND p53validuntil>=@dat ORDER BY p53ValidFrom DESC
  

  if @p53id is null
    select top 1 @p53id=p53id,@ret=p53value from p53VatRate where j27ID=@j27id AND j17ID is null and x15id=@x15id and p53validfrom<=@dat AND p53validuntil>=@dat ORDER BY p53ValidFrom DESC

  
  if @p53id is not null
    RETURN(@ret)
    
  if @ret is null
   set @ret=0

  RETURN(@ret)
   
END



GO