<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" %>

<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register TagPrefix="uc" TagName="plugin_datatable" Src="~/plugin_datatable.ascx" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>

<script runat="server">
    Protected Sub Page_Load() Handles Me.Load
        Master.IsMenuNever = True
        
        If Not Page.IsPostBack Then
            With Master.Factory.j03UserBL
                .InhaleUserParams("periodcombo-custom_query", "pokladna_period")
                
                period1.SetupData(Master.Factory, .GetUserParam("periodcombo-custom_query"))
                period1.SelectedValue = .GetUserParam("pokladna_period")
                
            End With
            Dim mq As New BO.myQueryJ02
            mq.IntraPersons = BO.myQueryJ02_IntraPersons.IntraOnly
            
            Me.j02id.DataSource = Master.Factory.j02PersonBL.GetList(mq).Where(Function(p) p.j07ID = 1 Or p.j07ID = 2 Or p.pid = Master.Factory.SysUser.j02ID)
            Me.j02id.DataBind()
            Me.j02id.SelectedValue = Master.Factory.SysUser.j02ID.ToString
        End If
    End Sub
    
    Protected Sub Page_LoadComplete() Handles Me.LoadComplete
        
      
        tab4.AddDbParameter("d1", period1.DateFrom)
        tab4.AddDbParameter("d2", period1.DateUntil)
        tab4.AddDbParameter("j02id", Me.j02id.SelectedValue)
        tab4.GenerateTable(Master.Factory, sql_manazeri.Value)
        
        tab1.AddDbParameter("d1", period1.DateFrom)
        tab1.AddDbParameter("d2", period1.DateUntil)
        tab1.AddDbParameter("j02id", Me.j02id.SelectedValue)
        tab1.GenerateTable(Master.Factory, sql1.Value)
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
            //alert(flag);



        }

       
        function p31_create(p34id) {
            var j02id = self.document.getElementById("<%=me.j02id.clientid%>").value;
            sw_local("../p31_record.aspx?p34id=" + p34id+"&j02id="+j02id,"",true);
        }
        function p31_edit(p31id) {            
            sw_local("../p31_record.aspx?pid=" + pid, "", true);
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
                <span class="page_header_span">Cashflow manažera</span>
            </td>
            <td>
                <asp:DropDownList ID="j02id" runat="server" AutoPostBack="true" DataValueField="pid" DataTextField="FullNameDesc"></asp:DropDownList>
            </td>
            <td>
                <button type="button" onclick="p31_create(3)">Zapsat výdaj</button>
            </td>
           <td>
               <uc:periodcombo ID="period1" runat="server" Width="220px"></uc:periodcombo>
           </td>
            <td>
                
            </td>
        </tr>
    </table>




    
   

    <div style="clear:both;"></div>
    <div>
        <uc:plugin_datatable ID="tab4" TableID="grid4" runat="server" IsShowGrandTotals="true" NoDataMessage=""
        ColHeaders="Jméno|Příjem hotovosti|Vrácení hotovosti|Vynaložené náklady bez DPH|Vč. DPH|Disponibilní hotovost"
        ColTypes="S|N|N|N|N|N" ColFlexSubtotals="0|11|11|11|11|11"
        TableCaption="Cashflow manažera" />
    </div>

    
    <div>
        <uc:plugin_datatable ID="tab1" TableID="grid1" runat="server" IsShowGrandTotals="true" NoDataMessage=""
        ColHeaders="|Jméno|Datum|Aktivita|Projekt|Částka bez DPH|Vč. DPH|Popis|Zapsal"
        ColTypes="S|S|D|S|S|N|N|S|S" ColFlexSubtotals="1|0|0|0|0|11|11|0|0"
        TableCaption="Posledních 20 výdajů manažera" />
    </div>
 

    <asp:HiddenField ID="sql_manazeri" runat="server" Value="
with rst
as (
SELECT a.j02ID
,min(j02LastName+' '+j02FirstName) as Osoba
,sum(case when a.p32ID=52 then abs(p31Amount_WithoutVat_Orig) end) as Pokladna_Prijem
,sum(case when a.p32ID=53 then abs(p31Amount_WithoutVat_Orig) end) as Pokladna_Vraceni
,sum(case when p32.p34ID=3 THEN p31Amount_WithoutVat_Orig end) as Naklady_BezDPH
        ,sum(case when p32.p34ID=3 THEN p31Amount_WithVat_Orig end) as Naklady_VcDPH
FROM p31Worksheet a
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID
WHERE p32.p34ID IN (3,5) AND a.j02ID=@j02id AND p31Date between @d1 AND @d2 AND a.j19ID=2
GROUP BY a.j02ID
)
select Osoba
    ,Pokladna_Prijem
    ,Pokladna_Vraceni
    ,Naklady_BezDPH
    ,Naklady_VcDPH
,isnull(Pokladna_Prijem,0)-isnull(Pokladna_Vraceni,0)-isnull(Naklady_VcDPH,0)
from
rst
order by Osoba" />

<asp:HiddenField ID="sql1" runat="server" Value="
select TOP 20 j19Name
,j02LastName+' '+j02FirstName
,p31Date
,p32Name
,p41TreePath
,p31Amount_WithoutVat_Orig
,p31Amount_WithVat_Orig
,p31Text
,convert(varchar(20),p31UserInsert)+'/'+convert(varchar(50),p31DateInsert) 
FROM p31Worksheet a
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID
INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID
INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID
LEFT OUTER JOIN j19PaymentType j19 ON a.j19ID=j19.j19ID                                                
WHERE p32.p34ID IN (3) AND a.j02ID=@j02id AND p31Date between @d1 AND @d2
order by j19Name,a.p31ID DESC" />

</asp:Content>


