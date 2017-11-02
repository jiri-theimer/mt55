<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="x28_record.aspx.vb" Inherits="UI.x28_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="3" cellspacing="2">
        <tr>
            <td style="width:140px;">
                <asp:Label ID="lblX29ID" Text="Entita:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="x29ID" runat="server" DataTextField="x29Name" DataValueField="x29ID" Enabled="False" AutoPostBack="true">
                    <asp:ListItem Text="Projekt" Value="141"></asp:ListItem>
                    <asp:ListItem Text="Klient" Value="328"></asp:ListItem>
                    <asp:ListItem Text="Faktura" Value="391"></asp:ListItem>
                    <asp:ListItem Text="Zálohová faktura" Value="390"></asp:ListItem>
                    <asp:ListItem Text="Osoba" Value="102"></asp:ListItem>
                    <asp:ListItem Text="Worksheet úkon" Value="331"></asp:ListItem>                    
                    <asp:ListItem Text="Úkol" Value="356"></asp:ListItem>
                </asp:DropDownList>
                <span>Druh pole:</span>
                <asp:DropDownList ID="x28Flag" runat="server" AutoPostBack="true">
                    <asp:ListItem Value="1" Text="Formulářové pole" Selected="true"></asp:ListItem>
                    <asp:ListItem Value="2" Text="Pomocné pole pro přehledy/statistiky/filtry"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblName" Text="Název/popisek pole:" runat="server" AssociatedControlID="x28Name" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="x28Name" runat="server" Style="width: 400px;"></asp:TextBox>

            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label1" Text="Datový formát:" runat="server" CssClass="lblReq"></asp:Label></td>
            <td>
                <uc:datacombo ID="x24id" runat="server" DataTextField="x24name" DataValueField="pid" IsFirstEmptyRow="true" Width="100px" AutoPostBack="true"></uc:datacombo>
            </td>
        </tr>
    </table>
    <table cellpadding="3" cellspacing="2" id="tabFlag1" runat="server">
        <tr>
            <td style="width:140px;">
                <asp:Label ID="Label3" Text="Combo seznam:" runat="server" CssClass="lbl"></asp:Label></td>
            <td>
                <uc:datacombo ID="x23ID" runat="server" DataTextField="x23Name" DataValueField="pid" IsFirstEmptyRow="true" Width="300px" AutoPostBack="true"></uc:datacombo>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label4" Text="Skupina polí:" runat="server" CssClass="lbl"></asp:Label></td>
            <td>
                <uc:datacombo ID="x27ID" runat="server" DataTextField="x27name" DataValueField="pid" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>
            </td>
        </tr>
        
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="x28IsRequired" runat="server" Text="Povinné k vyplnění" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblx28TextboxWidth" Text="Šířka (px):" runat="server" CssClass="lbl" AssociatedControlID="x28TextboxWidth"></asp:Label></td>
            <td>
                <telerik:RadNumericTextBox ID="x28TextboxWidth" runat="server" MinValue="0" MaxValue="600" NumberFormat-DecimalDigits="0" IncrementSettings-Step="20" Width="50px" ShowSpinButtons="true">
                </telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblx28TextboxHeight" Text="Výška (px):" runat="server" CssClass="lbl" AssociatedControlID="x28TextboxHeight"></asp:Label></td>
            <td>
                <telerik:RadNumericTextBox ID="x28TextboxHeight" runat="server" MinValue="0" MaxValue="600" NumberFormat-DecimalDigits="0" IncrementSettings-Step="20" Width="50px" ShowSpinButtons="true">
                </telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr valign="top">
            <td>
                <asp:Label ID="lblx28DataSource" Text="Možné hodnoty:" runat="server" CssClass="lbl" AssociatedControlID="x28DataSource"></asp:Label></td>
            <td>
                <asp:TextBox ID="x28DataSource" runat="server" Style="width: 600px; height: 40px;" TextMode="MultiLine"></asp:TextBox>
                <div>
                    <asp:Label ID="lblItemsListDelimiter" Text="Oddělovač možných hodnot je středník" runat="server"></asp:Label>
                </div>
                <div>
                    <asp:CheckBox ID="x28IsFixedDataSource" runat="server" Text="Výběr z možných hodnot je fixní (klasický roletový výběr)" />
                </div>

            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label2" Text="Index pořadí:" runat="server" CssClass="lbl" AssociatedControlID="x28Ordinary"></asp:Label></td>
            <td>
                <telerik:RadNumericTextBox ID="x28Ordinary" runat="server" MinValue="0" MaxValue="200" NumberFormat-DecimalDigits="0" Value="0" Width="50px" ShowSpinButtons="true">
                </telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblx28field" Text="Fyzický název pole:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <asp:Label ID="x28field" Text="?" runat="server"></asp:Label>
            </td>
        </tr>

    </table>
    <table cellpadding="3" cellspacing="2" id="tabFlag2" runat="server">
        <tr>
            <td style="width:140px;">
                <asp:Label ID="lblx28Grid_Field" Text="Pole/sloupec (GRID):" runat="server" CssClass="lblReq"></asp:Label></td>
            <td>
                <asp:TextBox ID="x28Grid_Field" runat="server" style="width:200px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblx28Grid_SqlSyntax" Text="SQL syntaxe (GRID):" runat="server" CssClass="lbl"></asp:Label></td>
            <td>
                <asp:TextBox ID="x28Grid_SqlSyntax" runat="server" style="width:600px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblx28Grid_SqlFrom" Text="SQL FROM klauzule (GRID):" runat="server" CssClass="lbl"></asp:Label></td>
            <td>
                <asp:TextBox ID="x28Grid_SqlFrom" runat="server" style="width:600px;height:80px;" TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>  
         <tr>
            <td>
                <asp:Label ID="lblx28Query_Field" Text="Filtrovací sql pole:" runat="server" CssClass="lbl"></asp:Label></td>
            <td>
                <asp:TextBox ID="x28Query_Field" runat="server" style="width:200px;"></asp:TextBox>
            </td>
        </tr>      
        <tr>
            <td>
                <asp:Label ID="lblx28Query_SqlSyntax" Text="SQL syntaxe filtrovacího pole:" runat="server" CssClass="lbl"></asp:Label></td>
            <td>
                <asp:TextBox ID="x28Query_SqlSyntax" runat="server" style="width:600px;height:80px;" TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>  
        
    </table>
    <table cellpadding="3" cellspacing="2" id="tabPivot" runat="server">
        <tr>
            <td style="width:140px;">
                <asp:Label ID="Label9" Text="PIVOT SELECT Sql:" runat="server" CssClass="lbl"></asp:Label></td>
            <td>
                <asp:TextBox ID="x28Pivot_SelectSql" runat="server" style="width:600px;" TextMode="SingleLine"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label8" Text="PIVOT GroupBy Sql:" runat="server" CssClass="lbl"></asp:Label></td>
            <td>
                <asp:TextBox ID="x28Pivot_GroupBySql" runat="server" style="width:600px;" TextMode="SingleLine"></asp:TextBox>
            </td>
        </tr>
    </table>
    <div class="div6">
        <asp:CheckBox ID="x28IsPublic" runat="server" AutoPostBack="true" Text="Obsah pole je dostupný všem uživatelům s přístupem k záznamu" Checked="true" />
    </div>
    <asp:panel ID="panPublic" runat="server" cssclass="content-box2">
        <div class="title">
            Obsah pole bude přístupný pouze uživatelům s níže vybranou aplikační rolí nebo pozicí osoby
        </div>
        <div class="content">
            <asp:CheckBoxList ID="x28NotPublic_j04IDs" runat="server" DataTextField="j04Name" DataValueField="pid" RepeatColumns="5"></asp:CheckBoxList>
            <hr />
            <asp:CheckBoxList ID="x28NotPublic_j07IDs" runat="server" DataTextField="j07Name" DataValueField="pid" RepeatColumns="5"></asp:CheckBoxList>
        </div>
    </asp:panel>
    <div class="div6">
        <asp:CheckBox ID="x28IsAllEntityTypes" runat="server" AutoPostBack="true" Text="Pole je aplikovatelné pro všechny entitní typy" Checked="true" />
    </div>
    <asp:panel ID="panEntityTypes" runat="server" cssclass="content-box2">
        <div class="title">
            Pole se bude nabízet k vyplnění pouze u níže zaškrtlých typů entity:
        </div>
        <div class="content">
            <table cellpadding="10">

                <asp:Repeater ID="rp1" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:CheckBox ID="chkEntityType" runat="server" CssClass="chk" Font-Bold="true" />
                                <asp:HiddenField ID="x26EntityTypePID" runat="server" />
                                <asp:HiddenField ID="x29ID_EntityType" runat="server" />
                            </td>
                            <td>
                                <asp:CheckBox ID="x26IsEntryRequired" runat="server" Text="Pro tento typ je pole povinné k vyplnění" />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
    </asp:panel>
    <asp:panel ID="panHelp" runat="server" CssClass="content-box2">
        <div class="title">
            Nápověda k poli
        </div>
        <div class="content">
            <asp:TextBox ID="x28HelpText" runat="server" Width="98%" TextMode="MultiLine" Height="70px"></asp:TextBox>
        </div>
    </asp:panel>
     
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>

