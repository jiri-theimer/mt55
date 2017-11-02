if exists (select 1 from sysobjects where  id = object_id('p31_aftersave') and type = 'P')
 drop procedure p31_aftersave
GO



CREATE    PROCEDURE [dbo].[p31_aftersave]
@p31id int
,@j03id_sys int
,@x45ids varchar(50) OUTPUT	---pøípadné události, které se mají notifikovat (èárkou oddìlené x45id)

AS

---automaticky se spouští po uložení worksheet záznamu
set @x45ids=''

declare @p31date datetime,@p32id int,@p41id int,@p34id int,@p71id int,@p70id int,@c11id int,@p33id int,@j02id_rec int
declare @j27id_billing_orig int,@j27id_internal int,@p31rate_billing_orig float,@p31rate_internal_orig float
declare @p31value_orig float,@p31amount_withoutvat_orig float,@p31vatrate_orig float,@p31amount_withvat_orig float
declare @p31amount_internal float,@p31amount_vat_orig float,@p91id int

select @c11id=a.c11ID,@p32id=a.p32ID,@p34id=p32.p34ID,@p71id=a.p71ID,@p70id=a.p70ID
,@p33id=p34.p33ID,@j02id_rec=a.j02ID,@p31date=a.p31date,@p41id=a.p41ID
,@p31value_orig=a.p31value_orig,@p91id=a.p91ID
FROM
p31Worksheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
WHERE a.p31ID=@p31id

if isnull(@p71id,0)<>0 or ISNULL(@p70id,0)<>0 or ISNULL(@p91id,0)<>0
 return	---pokud úkon prošel schvalováním nebo fakturací, není možné mìnit jeho atributy!!!!!
 

declare @c11id_find int

select top 1 @c11id_find=c11id from c11statperiod where c11level=5 and c11datefrom=@p31date  

-----statistické období c11-------------------------
if @c11id is null OR @c11id_find is null OR isnull(@c11id_find,0)<>isnull(@c11id,0)
 BEGIN  
  if @c11id_find is null
    begin
      if year(@p31date)>1950 and year(@p31date)<year(getdate())+40
        begin
		  declare @year int
		  set @year=year(@p31date)
          exec c11_yearrecovery @year

		  select top 1 @c11id_find=c11id from c11statperiod where c11level=5 and c11datefrom=@p31date
        end
    end
    
   update p31worksheet set c11ID=@c11id_find WHERE p31ID=@p31id
 END

if @p33id=1 or @p33id=3	---1 - èas, 3 - kusovník
 BEGIN        
	exec p31_getrate_tu 1, @p41id, @j02id_rec, @p32id, @j27id_billing_orig OUTPUT , @p31rate_billing_orig OUTPUT

	exec p31_getrate_tu 2, @p41id, @j02id_rec, @p32id, @j27id_internal OUTPUT , @p31rate_internal_orig OUTPUT  
	
	select @p31vatrate_orig=dbo.p32_get_vatrate(@p32id,@p41id,@p31date)
	
	set @p31amount_withoutvat_orig=@p31value_orig*@p31rate_billing_orig
	set @p31amount_vat_orig=@p31amount_withoutvat_orig*@p31vatrate_orig/100
	set @p31amount_withvat_orig=@p31amount_withoutvat_orig+@p31amount_vat_orig
	set @p31amount_internal=@p31value_orig*@p31rate_internal_orig
	
	update p31WorkSheet set p31amount_withoutvat_orig=@p31amount_withoutvat_orig,p31amount_vat_orig=@p31amount_vat_orig
	,p31amount_withvat_orig=@p31amount_withvat_orig,p31VatRate_Orig=@p31vatrate_orig
	,p31Amount_Internal=@p31amount_internal
	,p31Rate_Billing_Orig=@p31rate_billing_orig,p31Rate_Internal_Orig=@p31rate_internal_orig
	,j27ID_Billing_Orig=@j27id_billing_orig,j27ID_Internal=@j27id_internal
	WHERE p31ID=@p31id
	
 END
 

 ----otestování limitù k notifikaci
 declare @limit_hours float,@limit_fee float,@real_hours float,@real_fee float,@p28id int

 select @p28id=p28ID_Client,@limit_hours=convert(float,p41LimitHours_Notification),@limit_fee=convert(float,p41LimitFee_Notification)
 FROM p41Project
 WHERE p41ID=@p41id

 if @limit_hours>0 OR @limit_fee>0	---notifikaèní limity nastavené u projektu
  begin
    select @real_hours=sum(p31Hours_Orig),@real_fee=sum(p31Hours_Orig*p31Rate_Billing_Orig)
	FROM p31Worksheet
	where p41ID=@p41id AND getdate() BETWEEN p31ValidFrom AND p31ValidUntil

	if @real_hours>@limit_hours and @limit_hours>0
	 set @x45ids=@x45ids+',14110'

	if @real_fee>@limit_fee and @limit_fee>0
	 set @x45ids=@x45ids+',14111'
  end

set @limit_hours=0
set @limit_fee=0

if @p28id is not null
 select @limit_hours=convert(float,p28LimitHours_Notification),@limit_fee=convert(float,p28LimitFee_Notification) FROM p28Contact WHERE p28ID=@p28id

if @limit_hours>0 OR @limit_fee>0	---notifikaèní limit nastavené u klienta projektu
  begin
    select @real_hours=sum(p31Hours_Orig),@real_fee=sum(p31Hours_Orig*p31Rate_Billing_Orig)
	FROM p31Worksheet a INNER JOIN p41Project b ON a.p41ID=b.p41ID
	where b.p28ID_Client=@p28id AND getdate() BETWEEN p31ValidFrom AND p31ValidUntil

	if @real_hours>@limit_hours and @limit_hours>0
	 set @x45ids=@x45ids+',32810'

	if @real_fee>@limit_fee and @limit_fee>0
	 set @x45ids=@x45ids+',32811'
  end

if @x45ids<>''
 set @x45ids=right(@x45ids,len(@x45ids)-1)

GO