if exists (select 1 from sysobjects where  id = object_id('p91_fpr_recalc_invoice') and type = 'P')
 drop procedure p91_fpr_recalc_invoice
GO


CREATE procedure [dbo].[p91_fpr_recalc_invoice]
@p51id int
,@p91id INT

AS

if isnull(@p51id,0)=0 or isnull(@p91id,0)=0
 return

declare @vynosy float,@hodiny float,@body float,@vynosy_fixedcurrency float

select @vynosy=sum(p31Amount_WithoutVat_Invoiced)
,@vynosy_fixedcurrency=sum(case when a.j27ID_Billing_Invoiced=2 THEN p31Amount_WithoutVat_Invoiced else p31Amount_WithoutVat_Invoiced_Domestic END)
from p31worksheet a inner join p32activity b on a.p32id=b.p32id inner join p34ActivityGroup c on b.p34id=c.p34id
where a.p91id=@p91id and a.p70ID=4 AND c.p34IncomeStatementFlag=2 AND c.p33id IN (2,5)

select @hodiny=isnull(sum(p31Hours_Orig),0)
FROM p31worksheet a INNER JOIN p32Activity b ON a.p32ID=b.p32ID
where p91id=@p91id AND p70ID=6 and p32IsBillable=1

select @body=SUM(p52rate*p31Hours_Orig)
FROM 
p52PriceList_Item a 
INNER JOIN j02Person j02 on a.j02ID=j02.j02ID
INNER JOIN p31WorkSheet p31 ON j02.j02ID=p31.j02ID
WHERE a.p51id=@p51id AND p31.p91ID=@p91id

update p31WorkSheet set p31AKDS_FPR_BODY=p31Hours_Orig*p52rate
FROM
p52PriceList_Item a
INNER JOIN j02Person j02 on a.j02ID=j02.j02ID
INNER JOIN p31WorkSheet p31 ON j02.j02ID=p31.j02ID
WHERE a.p51id=@p51id AND p31.p91ID=@p91id

update p31WorkSheet set p31AKDS_FPR_PODIL=p31AKDS_FPR_BODY/@body
WHERE p91ID=@p91id

update p31WorkSheet set p31AKDS_FPR_OBRAT=p31AKDS_FPR_PODIL*@vynosy
,p31AKDS_FPR_OBRAT_FixedCurrency=p31AKDS_FPR_PODIL*@vynosy_fixedcurrency
WHERE p91ID=@p91id

GO