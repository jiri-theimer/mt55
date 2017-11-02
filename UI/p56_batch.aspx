<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p56_batch.aspx.vb" Inherits="UI.p56_batch" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/PageHeader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="freefields" Src="~/freefields.ascx" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign" Src="~/entityrole_assign.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content-box2">
        <div class="title">Předmět hromadné operace</div>
        <div class="content">
            <asp:RadioButtonList ID="opgTarget" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" RepeatColumns="3" CellPadding="5">
                <asp:ListItem Text="Uzavřít úkoly (přesunout do archivu)" Value="bin"></asp:ListItem>
                <asp:ListItem Text="Otevřít úkoly (přesunout z archivu)" Value="restore"></asp:ListItem>
                <asp:ListItem Text="Aktualizovat obsazení role v úkolu" Value="role"></asp:ListItem>                
                <asp:ListItem Text="Aktualizovat typ úkolu" Value="p57id"></asp:ListItem>
                <asp:ListItem Text="Aktualizovat workflow stav úkolu" Value="b02id"></asp:ListItem>                
            </asp:RadioButtonList>

        </div>
    </div>
    <asp:RadioButtonList ID="opgComboMode" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
            <asp:ListItem Text="Aktualizovat hodnotu pole" Value="1" Selected="true"></asp:ListItem>
            <asp:ListItem Text="Vyčistit hodnotu pole u úkolu" Value="2"></asp:ListItem>
        </asp:RadioButtonList>

    <asp:Panel ID="panCombo" runat="server" Visible="false">
        
        <table>
            <tr>
                <td>
                    <asp:Label ID="lblCombo" runat="server" CssClass="lbl" Text="Středisko:"></asp:Label></td>
                <td>

                    <uc:datacombo ID="cbx1" runat="server" DataValueField="pid" IsFirstEmptyRow="true" AutoPostBack="false" Width="400px"></uc:datacombo>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="panFF" runat="server" Visible="false">
        <uc:freefields ID="ff1" runat="server" />
    </asp:Panel>
    <asp:Panel ID="panRoles" runat="server" Visible="false">
        <asp:Label ID="lblClearedX67ID" runat="server" Text="Vyčistit obsazení role v úkolu:"></asp:Label>
        <asp:DropDownList ID="cbxClearedX67ID" runat="server" DataValueField="pid" DataTextField="x67Name"></asp:DropDownList>
        <uc:entityrole_assign ID="roles1" runat="server" EntityX29ID="p56Task"></uc:entityrole_assign>
    </asp:Panel>

    <div class="content-box2" style="margin-top: 20px;">
        <div class="title">Vybrané záznamy úkolů (<asp:Label ID="lblCount" runat="server"></asp:Label>)</div>
        <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid"></uc:datagrid>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
