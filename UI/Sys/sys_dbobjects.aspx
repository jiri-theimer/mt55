<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="sys_dbobjects.aspx.vb" Inherits="UI.sys_dbobjects" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div class="gridheader" style="height:25px;">
<asp:Image runat="server" ID="imgFormHeader" Width="16px" Height="16px" ImageUrl="~/Images/record.png" />
<asp:Label ID="lblFormHeader" runat="server" CssClass="framework_header_span" style="margin-left:10px;" text="SYS administrace"></asp:Label>
</div>
<div style="padding:10px;display:none;">
<asp:Button ID="cmdRecalcTrees" runat="server" Text="Přepočítat stromové tabulky:" />
<asp:TextBox ID="txtTreeTables" runat="server" style="width:600px;" text="b07Comment" />
</div>
<div style="padding:10px;">
<asp:Button ID="cmdGenerate1" runat="server" Text="Vygenerovat Procedury+Funkce po souborech" />
</div>
<div style="padding:10px;">
<asp:Button ID="cmdGenerate2" runat="server" Text="Vygenerovat Procedury+Funkce do jediného souboru" />
</div>

<hr />
<div style="padding:10px;">
<asp:Button ID="cmdGenerateXmlDistributionFiles" runat="server" Text="Vygenerovat XML distribuční soubory" />
    <span>z databáze:</span><asp:TextBox ID="txtDatabaseSourceForXmlDistribution" runat="server" Text="marktime50_dm"></asp:TextBox>
</div>

<hr />
<div style="padding:10px;">

<div style="padding:10px;">
<asp:Button ID="cmdGenerate3" runat="server" Text="Porovnat Tabulky se srovnávací databází" />
</div>

Srovnávací databáze:
<asp:TextBox ID="txtCompareCon" runat="server" style="width:600px;" text="server=TIMER-THINK\SQLEXPRESS;database=MT50VZOR;uid=sa;pwd=a;"/>
</div>
<div style="padding:10px;">
<asp:Button ID="cmdBackup" runat="server" Text="Vytvořit db zálohu" />
</div>

<hr />
<div style="padding:10px;">
<asp:Button ID="cmdDump" runat="server" Text="Vytvořit DUMP výstup" />
    <asp:CheckBox ID="chkIncludeGO" runat="server" Text="Za každý INSERT řádek vložit GO" />
(Syntaxe: table|identity|truncate|where)
<span style="color:Red;">Příklad 1: x29Entity|0|1|</span>
<span style="color:green;">Příklad 2: a05Region|1|1|</span>
<span style="color:blue;">Příklad 3: a03Institution|1|0|a09id=6</span>
<div>
Definice pro DUMP výstup:
</div>
<div>
<asp:TextBox ID="txtDUMPDef" runat="server" TextMode="MultiLine" style="height:60px;width:90%;"></asp:TextBox>
</div>
Vygenerovaný DUMP výstup:
<div>
<asp:TextBox ID="txtDUMPResult" runat="server" TextMode="MultiLine" style="height:100px;width:90%;"></asp:TextBox>
</div>

</div>
<asp:Button ID="cmdNoIdentity" runat="server" Text="Výpis tabulek, které nemají nastavenou IDENTITY primárního klíče" />
<asp:CheckBox ID="chkUseCompareCon" runat="server" Text="Použít srovnávací připojovací řetězec" />




</asp:Content>
