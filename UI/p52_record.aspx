<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p52_record.aspx.vb" Inherits="UI.p52_record" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="5" cellspacing="2">
        <tr>
            <td>
                <asp:Label ID="lblP34ID" Text="Sešit:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="p34ID" runat="server" DataTextField="p34Name" DataValueField="pid" IsFirstEmptyRow="false" Width="250px" AutoPostBack="true"></uc:datacombo>
                <asp:CheckBox ID="p52IsPlusAllTimeSheets" runat="server" Text="Sazba platí i pro všechny ostatní časové sešity" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:RadioButtonList ID="opgSubject" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                    <asp:ListItem Text="Sazbu definovat pro konkrétní osobu" Value="j02" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Sazbu definovat pro pozici osoby" Value="j07"></asp:ListItem>
                    <asp:ListItem Text="Sazba platí pro všechny osoby" Value="all"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblSubject" Text="Osoba:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="j07ID" runat="server" DataTextField="j07Name" DataValueField="pid" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>
                <uc:person ID="j02ID" runat="server" Width="300px" Flag="all" />

            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="chkP32" Text="Sazbu definovat i pro konkrétní aktivitu z sešitu" AutoPostBack="true" runat="server"/>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblP32ID" Text="Aktivita:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="p32ID" runat="server" DataTextField="p32Name" DataValueField="pid" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblRate" Text="Sazba:" runat="server" CssClass="lbl" AssociatedControlID="p52Rate"></asp:Label>
            </td>
            <td>
                <telerik:RadNumericTextBox ID="p52Rate" runat="server" Width="120px"></telerik:RadNumericTextBox>
            </td>
        </tr>

        <tr>
            <td>
                <asp:Label ID="lblName" runat="server" CssClass="lbl" Text="Poznámka:"></asp:Label></td>
            <td>
                <asp:TextBox ID="p52Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hidIsp52IsPlusAllTimeSheets" Value="" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
