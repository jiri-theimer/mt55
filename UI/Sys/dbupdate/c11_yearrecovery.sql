if exists (select 1 from sysobjects where  id = object_id('c11_yearrecovery') and type = 'P')
 drop procedure c11_yearrecovery
GO


CREATE  procedure [dbo].[c11_yearrecovery]
@year int
as


SET DATEFIRST 1

delete from c11statperiod where c11y=@year

declare @level int,@y int,@i int,@dats varchar(10),@d1 datetime,@d2 datetime,@firstmonday datetime,@firstthurday datetime

set @i=0
set @d1=convert(datetime,'01.01.'+convert(varchar(4),@year))
set @d2=dateadd(day,-1,dateadd(year,1,@d1))
set @firstmonday=@d1


EXEC c11_insertrec 1, @d1, @d2,-1,-1,-1,-1


while year(@d1)=@year
  begin
    if day(@d1)=1 and (month(@d1)=1 or month(@d1)=4 or month(@d1)=7 or month(@d1)=10)
      begin	---kvartály
	set @d2=dateadd(day,-1,dateadd(quarter,1,@d1))
    	EXEC c11_insertrec 2, @d1, @d2,-1,-1,-1,-1
      end

    if day(@d1)=1
      begin	---mìsíce
	set @d2=dateadd(day,-1,dateadd(month,1,@d1))
	EXEC c11_insertrec 3, @d1, @d2,-1,-1,-1,-1
      end

    
    set @d1=dateadd(day,1,@d1)
    set @i=@i+1
    
  end


---generovat tıdny----
set @firstthurday=convert(datetime,'01.01.'+convert(varchar(4),@year)) 

while datepart(weekday,@firstthurday)<>4
  begin
	set @firstthurday=dateadd(day,1,@firstthurday)
  end

set @firstmonday=dateadd(day,-3,@firstthurday)


set @d1=@firstmonday
set @i=1
declare @q int,@m int

while year(@d1)=@year or year(@d2)=@year
  begin
    set @d2=dateadd(day,6,@d1)

    if year(@d2)=@year
     begin
    	set @q=datepart(quarter,@d2)
    	set @m=month(@d2)
     end
    else
     begin
    	set @q=datepart(quarter,@d1)
    	set @m=month(@d1)
     end

    EXEC c11_insertrec 4, @d1, @d2,@i,@m,@q,@year

    set @d1=dateadd(day,7,@d1)
    set @i=@i+1
  end

---generování dnù---------
set @d1=convert(datetime,'01.01.'+convert(varchar(4),@year))

while year(@d1)=@year
  begin
    
    EXEC c11_insertrec 5, @d1, @d1,-1,-1,-1,-1
    set @d1=dateadd(day,1,@d1)
  end


---narovnání c11parentid dnù vùèi tıdnùm---
declare @w int

DECLARE curW CURSOR FOR 
	select c11id,c11datefrom,c11dateuntil,c11w from c11statperiod
	where (c11level=4 and c11y=@year) or (c11id in (select max(c11id) from c11statperiod where c11level=4 and c11y=@year-1))
	
	OPEN curW
	FETCH NEXT FROM curW 
	INTO @i,@d1,@d2,@w
	WHILE @@FETCH_STATUS = 0
	BEGIN		
	    update c11statperiod set c11parentid=@i,c11w=@w where c11level=5 and c11datefrom>=@d1 and c11datefrom<=@d2

   	  FETCH NEXT FROM curW 
   	  INTO @i,@d1,@d2,@w
	END
	CLOSE curW
	DEALLOCATE curW

---závìreèné èištìní
update c11statperiod set c11q=0,c11m=0,c11w=0,c11d=0 where c11y=@year and c11level=1
update c11statperiod set c11m=0,c11w=0,c11d=0 where c11y=@year and c11level=2
update c11statperiod set c11w=0,c11d=0 where c11y=@year and c11level=3
update c11statperiod set c11d=0 where c11y=@year and c11level=4


GO