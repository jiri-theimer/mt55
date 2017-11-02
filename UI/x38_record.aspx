<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="x38_record.aspx.vb" Inherits="UI.x38_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="3" cellspacing="2">
        <tr>
            <td style="width:180px;">
                <asp:Label ID="lblX29ID" Text="Entita:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="x29ID" runat="server" DataTextField="x29Name" DataValueField="x29ID" AutoPostBack="true">
                    <asp:ListItem Text="Projekt" Value="141"></asp:ListItem>
                    <asp:ListItem Text="Klient" Value="328"></asp:ListItem>
                    <asp:ListItem Text="Úkol" Value="356"></asp:ListItem>
                    <asp:ListItem Text="Daňová faktura" Value="391"></asp:ListItem>
                    <asp:ListItem Text="Zálohová faktura" Value="390"></asp:ListItem>
                    <asp:ListItem Text="Doklad přijaté platby zálohy (DPP)" Value="382"></asp:ListItem>
                    <asp:ListItem Text="Dokument" Value="223"></asp:ListItem>
                </asp:DropDownList>
                <asp:CheckBox ID="x38IsDraft" runat="server" Text="Tato řada se nabízí pro číslování [DRAFT] záznamů" />
            </td>
        </tr>

        <tr>
            <td>
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název číselné řady:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="x38Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="x38IsUseDbPID" runat="server" Text="Číslovat podle primárního klíče záznamu v databázi" AutoPostBack="true" />
            </td>
        </tr>
    </table>
    <table cellpadding="3" cellspacing="2" id="tabMore" runat="server">
        <tr>
            <td style="width:180px;">
                <asp:Label ID="lblOrdinary" Text="Rozsah nul pořadového čísla:" runat="server" CssClass="lbl"></asp:Label>

            </td>
            <td>

                <telerik:RadNumericTextBox ID="x38Scale" runat="server" NumberFormat-DecimalDigits="0" MaxValue="6" MinValue="3" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblx38ConstantBeforeValue" runat="server" CssClass="lbl" Text="Konstanta před pořadovým číslem:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="x38ConstantBeforeValue" runat="server" Style="width: 70px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblx38ConstantAfterValue" runat="server" CssClass="lbl" Text="Konstanta za pořadovým číslem:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="x38ConstantAfterValue" runat="server" Style="width: 70px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblx38MaskSyntax" runat="server" CssClass="lbl" Text="Vlastní funkce číselné řadky:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="x38MaskSyntax" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
       
        <tr>
            <td>
                <asp:Label ID="Label1" Text="Pořadové číslo musí začínat od hodnoty:" runat="server" CssClass="lbl"></asp:Label>

            </td>
            <td>

                <telerik:RadNumericTextBox ID="x38ExplicitIncrementStart" runat="server" NumberFormat-DecimalDigits="0" MaxValue="999999" MinValue="0" Width="150px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
    </table>
    <div class="div6">
        <div class="content-box2">
            <div class="title">Kdo může upravovat vygenerovaný kód v záznamu entity</div>
            <div class="content">
                <asp:RadioButtonList ID="x38EditModeFlag" runat="server" RepeatDirection="Vertical" CellPadding="10">
                    <asp:ListItem Text="Kód záznamu nelze upravovat" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Kód může upravovat vlastník záznamu" Value="2" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Kód může upravovat pouze administrátor" Value="3"></asp:ListItem>

                </asp:RadioButtonList>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>


