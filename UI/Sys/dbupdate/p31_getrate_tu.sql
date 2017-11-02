if exists (select 1 from sysobjects where  id = object_id('p31_getrate_tu') and type = 'P')
 drop procedure p31_getrate_tu
GO





CREATE procedure [dbo].[p31_getrate_tu]
@pricelisttype int,@p41id int,@j02id int,@p32id int
,@ret_j27id int OUTPUT,@ret_rate float OUTPUT

---@pricelisttype=1 - fakturaèní ceník
---@pricelisttype=2 - nákladový ceník
AS

  set @pricelisttype=isnull(@pricelisttype,1)
   
  declare @p51id int,@p34id int,@j07id int,@isbillable bit,@p28id int,@p33id int
    
  set @ret_rate=0
  
  if @pricelisttype=1	--fakturaèní ceník
   begin
    select @p51id=p51ID_Billing,@p28id=p28id_client from p41Project where p41id=@p41id
    
    if @p51id is null and @p28id is not null
     select @p51id=p51ID_Billing from p28contact where p28id=@p28id
   end


  if @pricelisttype=2	--nákladový ceník
   begin
     select @p51id=p51ID_Internal,@p28id=p28id_client from p41Project where p41id=@p41id
     
     if @p51id is null and @p28id is not null
      select @p51id=p51ID_Internal from p28contact where p28id=@p28id
   end
    
  
  if @p51id is null
   begin
    select @ret_j27id=convert(int,x35Value) FROM x35GlobalParam WHERE x35Key LIKE 'j27ID_Invoice'

    return	--není ceník u projektu ani u klienta
   end
  

  select @j07id=isnull(j07ID,0) FROM j02Person where j02ID=@j02id
  
  select @p34id=a.p34id,@isbillable=a.p32IsBillable,@p33id=b.p33id
  FROM p32Activity a inner join p34activitygroup b on a.p34id=b.p34id
  where a.p32id=@p32id
  
  if @isbillable=0 and @pricelisttype=1
   begin
     set @ret_rate=0

	 if @p51id is not null
	  select @ret_j27id=j27ID FROM p51PriceList WHERE p51ID=@p51id

	 if @ret_j27id is null
      select @ret_j27id=convert(int,x35Value) FROM x35GlobalParam WHERE x35Key LIKE 'j27ID_Invoice'

     return	--nefakturovatelná aktivita -> nulová sazba (pravidlo platí u fakt.ceníku)
   end

   
  declare @p52id int,@rate_default_amount float,@p51id_master int

  select @ret_j27id=j27id,@rate_default_amount=case when @p33id=1 THEN p51DefaultRateT else isnull(p51DefaultRateU,0) END,@p51id_master=isnull(p51id_master,0)
  FROM p51PriceList where p51id=@p51id


  --------sazba podle aktivita+uživatel napøímo--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p34id=@p34id and p32id=@p32id
  and j02ID=@j02id
  ORDER BY p52IsMaster
  
  if isnull(@p52id,0)<>0
    return



  --------sazba podle aktivita+pozice osoby napøímo--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p34id=@p34id and p32id=@p32id
  and j07ID=@j07id
  ORDER BY p52IsMaster
  
  if isnull(@p52id,0)<>0
    return

  --------sazba podle aktivita--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p34id=@p34id and p32id=@p32id
  ORDER BY p52IsMaster
  
  if isnull(@p52id,0)<>0
    return
    
    
--------sazba podle uživatel bez aktivity--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p34id=@p34id and p32id is null
  and j02ID=@j02id
  ORDER BY p52IsMaster

  if isnull(@p52id,0)<>0
    return


--------sazba podle pozice osoby bez aktivity--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p34id=@p34id and p32id is null
  and j07ID=@j07id
  ORDER BY p52IsMaster

  if isnull(@p52id,0)<>0
    return

--------sazba podle sheet bez aktivity i personálního zdroje--------------
  select top 1 @p52id=p52id,@ret_rate=p52Rate
  FROM p52PriceList_Item
  where p51id IN (@p51id,@p51id_master) and p34id=@p34id and p32id is null and j02ID is null AND j07id is null
  ORDER BY p52IsMaster
  
  if isnull(@p52id,0)<>0
    return

----zde už se vrací výchozí sazba ceníku-----------
set @ret_rate=@rate_default_amount


















GO