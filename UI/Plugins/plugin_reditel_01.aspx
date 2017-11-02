<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" %>

<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register TagPrefix="uc" TagName="plugin_datatable" Src="~/plugin_datatable.ascx" %>

<script runat="server">
    Protected Sub Page_Load() Handles Me.Load
        If Not Page.IsPostBack Then
           
        End If
    End Sub
    
    Protected Sub Page_LoadComplete() Handles Me.LoadComplete
        
        Dim strSQL As String = "select TOP " & Me.cbxTOP.SelectedValue & " j03Login,j02lastname+' '+j02firstname,j90Date,j90ClientBrowser from j90LoginAccessLog a INNER JOIN j03User j03 ON a.j03ID=j03.j03ID LEFT OUTER JOIN j02Person j02 ON j03.j02ID=j02.j02ID order by a.j90ID DESC"
        tabJ90.GenerateTable(Master.Factory, strSQL)
        
        tabHours1.GenerateTable(Master.Factory, tabHours1_sql.Value)
        
        tabHours2.GenerateTable(Master.Factory, tabHours2_sql.Value)
    End Sub

    
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="Plugin.css" />
    <link rel="stylesheet" type="text/css" href="Scripts/datatable/css/jquery.dataTables.css" />
    <script type="text/javascript" src="Scripts/datatable/jquery.dataTables.min.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {

            oTable1 = $('#gridJ90').dataTable({
                bPaginate: false,
                bFilter: true,
                bStateSave: true,
                bSort: true,
                "aaSorting": [[2, "desc"]], // výchozí třídění podle třetího sloupce

                oLanguage: {
                    sUrl: 'Scripts/datatable/cs_CZ.txt'

                }

            });

            oTable1 = $('#gridHours1').dataTable({
                bPaginate: false,
                bFilter: true,
                bStateSave: false,
                bSort: false,

                oLanguage: {
                    sUrl: 'Scripts/datatable/cs_CZ.txt'

                }

            });

            oTable1 = $('#gridHours2').dataTable({
                bPaginate: false,
                bFilter: true,
                bStateSave: false,
                bSort: false,

                oLanguage: {
                    sUrl: 'Scripts/datatable/cs_CZ.txt'

                }

            });

            <%If Request.Item("caller") = "report_framework" Then%>
            document.getElementById("pp").style.display = "none";
            <%End If%>
        });


        function hardrefresh(pid, flag) {
            if (flag == "j03_myprofile") {
                window.parent.open("default.aspx", "_self");
            }



        }

        function personalpage() {
            sw_local("../j03_myprofile_defaultpage.aspx", "Images/plugin_32.png")


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
                <span class="page_header_span">Dohled ředitele na vykazování hodin</span>
            </td>
            <td>
                <a href="#topHours2">Hodiny lidí na projektech</a>
            </td>
            <td>
                <a href="#topJ90">Log přístupů do systému</a>
            </td>
            <td>
                <a id="pp" href="javascript:personalpage()">Zvolit si jinou osobní (výchozí) stránku</a>
            </td>
        </tr>
    </table>



    <uc:plugin_datatable ID="tabHours1" TableID="gridHours1" runat="server"
        ColHeaders="Pozice|Osoba|Dnes|Včera|Tento týden|Tento měsíc|Z toho fakturovatelné|Poslední přihlášení"
        ColHideRepeatedValues="1" ColTypes="S|S|N|N|N|N|N|DTX" ColFlexSubtotals="1|0|11|11|11|11|11|0"
        TableCaption="Vykázané hodiny v tomto měsíci podle pozic a lidí" />

    <asp:HiddenField ID="tabHours1_sql" runat="server" Visible="false" Value="SET DATEFIRST 1
declare @d0 datetime,@d_vcera datetime,@d1_tyden datetime,@d2_tyden datetime,@d1_mesic datetime,@d2_mesic datetime

set @d0=dbo.get_dateserial(year(getdate()),month(getdate()),day(getdate()),0,0)
set @d_vcera=dateadd(day,-1,@d0)
set @d1_tyden=DATEADD(DAY,1-DATEPART(WEEKDAY, @d0), @d0)
set @d2_tyden=dateadd(day,-1,DATEADD(week,1,@d1_tyden))
set @d1_mesic=dbo.get_dateserial(year(getdate()),month(getdate()),1,0,0)
set @d2_mesic=dateadd(day,-1,dateadd(month,1,@d1_mesic))


SELECT min(j07Name),min(j02lastname+' '+j02firstname)
,sum(case when p31date=@d0 THEN p31Hours_Orig END) as Hodiny_Dnes
,sum(case when p31date=@d_vcera THEN p31Hours_Orig END) as Hodiny_Vcera
,sum(case when p31date BETWEEN @d1_tyden AND @d2_tyden THEN p31Hours_Orig END) as Hodiny_Tyden
,sum(case when p31date BETWEEN @d1_mesic and @d2_mesic THEN p31Hours_Orig END) as Hodiny_Mesic
,sum(case when p31date BETWEEN @d1_mesic and @d2_mesic AND p32.p32IsBillable=1 THEN p31Hours_Orig END) as Hodiny_Mesic_Fakturovatelne
,min(pristupy.Naposledy)
from
p31worksheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID
LEFT OUTER JOIN j07PersonPosition j07 ON j02.j07ID=j07.j07ID
LEFT OUTER JOIN (select j03.j02ID,max(j90Date) as Naposledy FROM j90LoginAccessLog j90 INNER JOIN j03User j03 ON j90.j03ID=j03.j03ID GROUP BY j03.j02ID) pristupy ON j02.j02ID=pristupy.j02ID
WHERE a.p31date BETWEEN @d1_mesic and @d2_mesic OR a.p31date=@d_vcera
GROUP BY a.j02ID
ORDER BY min(j07Name), min(j02lastname+' '+j02firstname)" />

    <div style="text-align: center;" id="topHours2"><a href="#top">Nahoru</a></div>
    <uc:plugin_datatable ID="tabHours2" TableID="gridHours2" runat="server"
        ColHeaders="Osoba|Projekt|Dnes|Včera|Tento týden|Tento měsíc"
        ColHideRepeatedValues="1" ColTypes="S|S|N|N|N|N" ColFlexSubtotals="1|0|11|11|11|11"
        TableCaption="Vykázané hodiny v tomto měsíci podle lidí na projektech" />


    <asp:HiddenField ID="tabHours2_sql" Visible="false" runat="server" Value="
SET DATEFIRST 1
declare @d0 datetime,@d_vcera datetime,@d1_tyden datetime,@d2_tyden datetime,@d1_mesic datetime,@d2_mesic datetime

set @d0=dbo.get_dateserial(year(getdate()),month(getdate()),day(getdate()),0,0)
set @d_vcera=dateadd(day,-1,@d0)
set @d1_tyden=DATEADD(DAY,1-DATEPART(WEEKDAY, @d0), @d0)
set @d2_tyden=dateadd(day,-1,DATEADD(week,1,@d1_tyden))
set @d1_mesic=dbo.get_dateserial(year(getdate()),month(getdate()),1,0,0)
set @d2_mesic=dateadd(day,-1,dateadd(month,1,@d1_mesic))


SELECT min(j02lastname+' '+j02firstname)
,min(isnull(p28name+' - ','')+p41name)
,sum(case when p31date=@d0 THEN p31Hours_Orig END) as Hodiny_Dnes
,sum(case when p31date=@d_vcera THEN p31Hours_Orig END) as Hodiny_Vcera
,sum(case when p31date BETWEEN @d1_tyden AND @d2_tyden THEN p31Hours_Orig END) as Hodiny_Tyden
,sum(case when p31date BETWEEN @d1_mesic and @d2_mesic THEN p31Hours_Orig END) as Hodiny_Mesic
from
p31worksheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID
INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID LEFT OUTER JOIN p28Contact p28 ON p41.p28ID_Client=p28.p28ID
WHERE a.p31DATE BETWEEN @d1_mesic and @d2_mesic OR a.p31date=@d_vcera
GROUP BY a.j02ID,a.p41ID
ORDER BY min(j02lastname+' '+j02firstname),min(isnull(p28name+' - ','')+p41name)" />



    <div style="text-align: center;" id="topJ90"> 
        <a href="#top">Nahoru</a>       
        Maximální počet zobrazených záznamů přístupů:
                <asp:DropDownList ID="cbxTOP" runat="server" AutoPostBack="true">
                    <asp:ListItem Text="10" Value="10"></asp:ListItem>
                    <asp:ListItem Text="20" Value="20"></asp:ListItem>
                    <asp:ListItem Text="50" Value="50"></asp:ListItem>
                    <asp:ListItem Text="100" Value="100"></asp:ListItem>
                </asp:DropDownList>        
    </div>
    <uc:plugin_datatable ID="tabJ90" TableID="gridJ90" runat="server" ColHeaders="Login|Osoba|Kdy|Prohlížeč" FormatDateTime="u"
        ColTypes="S|S|DT|SX" TableCaption="Log posledních přístupů do systému" />



</asp:Content>
