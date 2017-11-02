if exists (select 1 from sysobjects where  id = object_id('get_exchange_rate') and type = 'FN')
 drop function get_exchange_rate
GO



CREATE FUNCTION [dbo].[get_exchange_rate](@ratetype int,@dat datetime,@j27id_source int,@j27id_dest int)
RETURNS float
AS
BEGIN
  set @ratetype=isnull(@ratetype,1)		---@ratetype=1 - fakturaèní kurz, @ratetype=2 - fixní kurz

  if @j27id_source is null or @j27id_dest is null
    return(1)

  if @j27id_source=@j27id_dest
    RETURN(1)

  declare @j27id_domestic int	--domácí mìna, která definuje mìnové kurzy vùèi ostatním mìnám, výchozí je CZK

  if exists(select x35ID FROM x35GlobalParam WHERE x35Key like 'j27ID_Domestic')
   select @j27id_domestic=convert(int,x35Value) from x35GlobalParam WHERE x35Key like 'j27ID_Domestic'
  else
   set @j27id_domestic=2

   
  declare @ret float
  


 if @j27id_source<>@j27id_domestic
  begin
  	select TOP 1 @ret=m62rate/m62units
  	FROM m62ExchangeRate
  	WHERE j27id_slave=@j27id_source and m62date<=@dat and j27id_master=@j27id_domestic AND m62RateType=@ratetype
  	ORDER BY m62date desc

  end
else
  begin
	select TOP 1 @ret=(1/m62rate)*m62units
  	FROM m62ExchangeRate
  	WHERE j27id_slave=@j27id_dest and m62date<=@dat and j27id_master=@j27id_domestic AND m62RateType=@ratetype
  	ORDER BY m62date desc

  end


  set @ret=isnull(@ret,1)

  if @j27id_dest=@j27id_domestic or @j27id_source=@j27id_domestic
    RETURN(@ret)	--pøevod do nebo z domácí mìny



  declare @ret2 float
  select @ret2=dbo.get_exchange_rate(@ratetype,@dat,@j27id_dest,@j27id_domestic)
  
  set @ret=@ret/@ret2
  
  

  RETURN(@ret)

END

































GO