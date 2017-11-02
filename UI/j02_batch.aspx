<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="j02_batch.aspx.vb" Inherits="UI.j02_batch" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="freefields" Src="~/freefields.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content-box2">
        <div class="title">Předmět hromadné operace</div>
        <div class="content">
            <asp:RadioButtonList ID="opgTarget" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" RepeatColumns="3" CellPadding="5">
                <asp:ListItem Text="Přesunout osoby do archivu" Value="bin"></asp:ListItem>
                <asp:ListItem Text="Přesunout osoby z archivu" Value="restore"></asp:ListItem>               
                <asp:ListItem Text="Aktualizovat pozici/hladinu sazby" Value="j07ID"></asp:ListItem>
                <asp:ListItem Text="Aktualizovat pracovní fond" Value="c21ID"></asp:ListItem>
                <asp:ListItem Text="Aktualizovat středisko" Value="j18ID"></asp:ListItem>                
                <asp:ListItem Text="Aktualizovat stát" Value="j17ID"></asp:ListItem>
            </asp:RadioButtonList>

        </div>
    </div>
    <asp:RadioButtonList ID="opgComboMode" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
            <asp:ListItem Text="Aktualizovat hodnotu pole" Value="1" Selected="true"></asp:ListItem>
            <asp:ListItem Text="Vyčistit hodnotu pole u klienta" Value="2"></asp:ListItem>
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
   

    <div class="content-box2" style="margin-top: 20px;">
        <div class="title">
            Vybrané osoby (<asp:Label ID="lblCount" runat="server"></asp:Label>)
            <span style="padding-left:40px;font-style:italic;">Do hromadných operací lze vybrat pouze interní osoby.</span>
        </div>
        <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid"></uc:datagrid>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
