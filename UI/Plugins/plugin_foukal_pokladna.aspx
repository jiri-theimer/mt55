<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" %>

<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register TagPrefix="uc" TagName="plugin_datatable" Src="~/plugin_datatable.ascx" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>

<script runat="server">
    Protected Sub Page_Load() Handles Me.Load
        
        If Not Page.IsPostBack Then
            With Master.Factory.j03UserBL
                .InhaleUserParams("periodcombo-custom_query", "pokladna_period")
                
                period1.SetupData(Master.Factory, .GetUserParam("periodcombo-custom_query"))
                period1.SelectedValue = .GetUserParam("pokladna_period")
            End With
        End If
    End Sub
    
    Protected Sub Page_LoadComplete() Handles Me.LoadComplete
        
        tab1.AddDbParameter("d1", period1.DateFrom)
        tab1.AddDbParameter("d2", period1.DateUntil)
        tab1.GenerateTable(Master.Factory, sql1.Value)
        
        tab2.AddDbParameter("d1", period1.DateFrom)
        tab2.AddDbParameter("d2", period1.DateUntil)
        tab2.GenerateTable(Master.Factory, sql2.Value)
    End Sub

    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        Master.Factory.j03UserBL.SetUserParam("pokladna_period", Me.period1.SelectedValue)
        
    End Sub
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="Plugin.css" />
    <link rel="stylesheet" type="text/css" href="Scripts/datatable/css/jquery.dataTables.css" />
    <script type="text/javascript" src="Scripts/datatable/jquery.dataTables.min.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {

           


        });


        function hardrefresh(pid, flag) {
            



        }

       
        function j01_create() {
            sw_local("../p31_record.aspx?p34id=5","",true);
        }
        function mp(j02id) {
            var p41id = 3;
            
            sw_local("../p31_record.aspx?p41id=" + p41id + "&p34id=5&j02id=" + j02id, "", true);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="10">
        <tr>
            <td id="top">
                <img src="../Images/plugin_32.png" />
            </td>
            <td>
                <span class="page_header_span">Pokladny</span>
            </td>
           <td>
               <uc:periodcombo ID="period1" runat="server" Width="220px"></uc:periodcombo>
           </td>
            <td>
                
            </td>
        </tr>
    </table>


    <div style="clear:both;"></div>

    
    
        <button type="button" onclick="j01_create()">Zapsat úkon</button>
        

    <div>
        <uc:plugin_datatable ID="tab1" TableID="grid1" runat="server" IsShowGrandTotals="true" NoDataMessage=""
        ColHeaders="Středisko|Dotace z banky|Vrácení hotovosti|Převod manažerům|Disponibilní hotovost|Vynaložené náklady"
        ColTypes="S|N|N|N|N|N" ColFlexSubtotals="0|11|11|11|11|11"
        TableCaption="Pokladny podle středisek" />

    

    </div>
    <div style="margin-top:15px;">
        <uc:plugin_datatable ID="tab2" TableID="grid2" runat="server" IsShowGrandTotals="true" NoDataMessage=""
        ColHeaders="Osoba|Příjem hotovosti|Vrácení hotovosti|Vynaložené náklady|Disponibilní hotovost"
        ColTypes="S|N|N|N|N" ColFlexSubtotals="0|11|11|11|11"
        TableCaption="Hotovostní operace podle osob" />

    

    </div>
   

    <asp:HiddenField ID="sql1" runat="server" Value="with rst
as (
SELECT p41.j18ID
,min(j18Name) as Stredisko
,sum(case when a.p32ID IN (56) then abs(p31Amount_WithVat_Orig) end) as Pokladna_Prijem
,sum(case when a.p32ID IN (52) then abs(p31Amount_WithVat_Orig) end) as Pokladna_Prevod_Manazerum
,sum(case when a.p32ID=53 then abs(p31Amount_WithVat_Orig) end) as Pokladna_Vraceni
,sum(case when p32.p34ID=3 AND a.j19ID=2 THEN p31Amount_WithVat_Orig end) as Naklady
FROM p31Worksheet a
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID
INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID
LEFT OUTER JOIN j18Region j18 ON p41.j18ID=j18.j18ID        
WHERE p31Date between @d1 AND @d2
GROUP BY p41.j18ID
)
select Stredisko
    ,Pokladna_Prijem
    ,Pokladna_Vraceni
    ,Pokladna_Prevod_Manazerum
,isnull(Pokladna_Prijem,0)+isnull(Pokladna_Vraceni,0)-isnull(Pokladna_Prevod_Manazerum,0)
,Naklady
from
rst
order by Stredisko" />

    
    <asp:HiddenField ID="sql2" runat="server" Value="with rst
as (
SELECT a.j02ID
,min(j02.j02LastName+' '+j02.j02FirstName) as Osoba
,sum(case when a.p32ID=52 then abs(p31Amount_WithVat_Orig) end) as Pokladna_Prijem
,sum(case when a.p32ID=53 then abs(p31Amount_WithVat_Orig) end) as Pokladna_Vraceni
,sum(case when p32.p34ID=3 AND a.j19ID=2 THEN p31Amount_WithVat_Orig end) as Naklady
FROM p31Worksheet a
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID
INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID      
WHERE p31Date between @d1 AND @d2
GROUP BY a.j02ID
)
select Osoba
    ,Pokladna_Prijem
    ,Pokladna_Vraceni
    ,Naklady
,isnull(Pokladna_Prijem,0)-isnull(Pokladna_Vraceni,0)-isnull(Naklady,0)
from
rst
WHERE Pokladna_Prijem is not null OR Naklady is not null
order by Osoba" />
 
</asp:Content>
