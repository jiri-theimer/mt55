<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p36_record.aspx.vb" Inherits="UI.p36_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="5" cellspacing="2">
        <tr>
            <td colspan="2">
                <asp:RadioButtonList ID="opgScope" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                    <asp:ListItem Text="Zámek se vztahuje na konkrétní osobu" Value="j02" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Zámek se vztahuje na tým osob" Value="j11"></asp:ListItem>
                    <asp:ListItem Text="Zámek se vztahuje na všechny osoby" Value="all"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblPerson" runat="server" CssClass="lblReq" Text="Osoba:"></asp:Label></td>
            <td>
                <uc:person ID="j02ID" runat="server" Width="300px" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblTeam" Text="Tým osob:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="j11ID" runat="server" DataTextField="j11Name" DataValueField="pid" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>


            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblFrom" Text="Datum od:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <telerik:RadDatePicker ID="p36DateFrom" runat="server" RenderMode="Lightweight" Width="120px">
                    <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                </telerik:RadDatePicker>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblUntil" Text="Datum do:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <telerik:RadDatePicker ID="p36DateUntil" runat="server" RenderMode="Lightweight" Width="120px">
                    <DateInput ID="DateInput2" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                </telerik:RadDatePicker>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="p36IsAllSheets" runat="server" AutoPostBack="true" Text="Zámek se vztahuje na všechny worksheet sešity" />
            </td>
        </tr>
    </table>
    <div class="content-box2">
        <div class="title">
            <asp:label ID="ph1" runat="server" Text="Rozsah uzamknutých sešitů"/>
        </div>
        <div class="content">
            <asp:CheckBoxList ID="p34ids" runat="server" DataValueField="pid" DataTextField="p34Name" RepeatColumns="3" CellPadding="8" CellSpacing="2"></asp:CheckBoxList>
        </div>
    </div>
    
    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>

