if exists (select 1 from sysobjects where  id = object_id('c11_insertrec') and type = 'P')
 drop procedure c11_insertrec
GO


CREATE procedure [dbo].[c11_insertrec]
@c11level int,@c11datefrom datetime,@c11dateuntil datetime,@week int,@month int,@quarter int,@year int
as


SET DATEFIRST 1

declare @code varchar(20),@id int,@parentid int,@name varchar(50),@s varchar(200)
declare @day int,@strid varchar(20)

if @year=-1
  set @year=year(@c11datefrom)

if @quarter=-1
  set @quarter=datepart(quarter,@c11datefrom)

if @month=-1
  set @month=month(@c11datefrom)


set @day=day(@c11datefrom)

if @week=-1
  set @week=datepart(week,@c11datefrom)

if @c11level=1
  begin
  	set @code=convert(varchar(4),@year)+'-00-00-000000-00'
	set @name='Rok '+convert(varchar(4),@year)
	set @strid=convert(varchar(10),@year-2000)+'0000000'
	set @quarter=0
	set @month=0
	set @week=0
	set @day=0
  end
if @c11level=2
  begin
  	set @code=convert(varchar(4),@year)+'-0'+convert(char(1),@quarter)+'-00-000000-00'
	set @strid=convert(varchar(10),@year-2000)+convert(char(1),@quarter)+'000000'
	set @name='Ètvrtletí '+convert(varchar(4),@year)+'-'+convert(char(1),@quarter)
  end

if @c11level=3
  begin
  	set @code=convert(varchar(4),@year)+'-0'+convert(char(1),@quarter)+'-'+right('0'+convert(varchar(2),@month),2)+'-000000-00'
	set @name='Mìsíc '+convert(varchar(4),@year)+'-'+convert(varchar(2),@month)
	set @strid=convert(varchar(10),@year-2000)+convert(char(1),@quarter)+right('0'+convert(varchar(2),@month),2)+'0000'
  end

if @c11level=4
  begin
    
    set @s=convert(varchar(4),@year)+right('0'+convert(varchar(2),@week),2)
    set @code=convert(varchar(4),@year)+'-0'+convert(char(1),@quarter)+'-'+right('0'+convert(varchar(2),@month),2)+'-'+@s+'-00'
    set @name='Týden '+convert(varchar(4),@year)+'-'+convert(varchar(2),@week)
    set @strid=convert(varchar(10),@year-2000)+convert(char(1),@quarter)+right('0'+convert(varchar(2),@month),2)+right('0'+convert(varchar(2),@week),2)+'00'
    
  end

if @c11level=5
  begin
    set @s=convert(varchar(4),@year)+right('0'+convert(varchar(2),@week),2)
    set @s=@s+'-'+right('0'+convert(varchar(2),@day),2)
    set @code=convert(varchar(4),@year)+'-0'+convert(char(1),@quarter)+'-'+right('0'+convert(varchar(2),@month),2)+'-'+@s
    set @name=convert(varchar(10),@c11datefrom,104)+' '+dbo.get_datename(@c11datefrom,0)
    set @strid=convert(varchar(10),@year-2000)+convert(char(1),@quarter)+right('0'+convert(varchar(2),@month),2)+right('0'+convert(varchar(2),@week),2)+right('0'+convert(varchar(2),@day),2)
  end



set @id=convert(int,@strid)

declare @ordinary int


set @ordinary=@id

select top 1 @parentid=c11id from c11statperiod where c11level=@c11level-1 and c11id<=@id and c11y=@year order by c11id desc

if @c11level=1
  set @parentid=0


insert into c11statperiod
(
c11id,c11parentid,c11name,c11code,c11ordinary,c11level,c11validfrom,c11validuntil,c11datefrom,c11dateuntil,c11y,c11q,c11m,c11w,c11d
)
values
(
@id,@parentid,@name,@code,@ordinary,@c11level,getdate(),convert(datetime,'01.01.3000',104),@c11datefrom,@c11dateuntil,@year,@year*100+@quarter,@year*100+@month,@year*100+@week,@day
)




GO