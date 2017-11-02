<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p31_clone.aspx.vb" Inherits="UI.p31_clone" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="project" Src="~/project.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="10">
        <tr>
            
            <td>
                <uc:project ID="p41id" runat="server" AutoPostBack="false" Width="400px" Flag="p31_entry" Text="Hledat projekt..." />
            </td>
            <td>
                <asp:Button ID="cmdSetProject" runat="server" Text="Doplnit všem vybraný projekt" CssClass="cmd" />
            </td>
        </tr>
    </table>

    <table cellpadding="2" cellspacing="2">
        <tr>
            <th>Datum</th>
            <th>Projekt</th>
            <th>Jméno</th>
            <th>Aktivita</th>
            <th>Hodiny</th>
            <th>Popis</th>
        </tr>
        <asp:Repeater ID="rp1" runat="server">
            <ItemTemplate>
                <tr class="trHover">
                    <td>
                        <telerik:RadDatePicker ID="p31Date" runat="server" Width="120px"  DateInput-EmptyMessage="Povinný údaj." DateInput-EmptyMessageStyle-ForeColor="red">
                            <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                        </telerik:RadDatePicker>
                    </td>
                    <td>
                        <asp:Label ID="Project" runat="server" CssClass="val" ForeColor="blue"></asp:Label>
                        <asp:HiddenField ID="p41id" runat="server" />
                    </td>
                    <td>
                        <asp:HiddenField ID="pid" runat="server" />
                        <asp:Label ID="Person" runat="server" CssClass="val"></asp:Label>
                    </td>
                    <td style="padding-left:20px;">
                        <asp:Label ID="p32Name" runat="server" CssClass="val"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="p31Value_Orig" runat="server" Style="width: 40px;"></asp:TextBox>
                    </td>
                    <td>
                        <asp:TextBox ID="p31Text" runat="server" Style="height: 40px; width: 400px;" TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>

    <telerik:RadCalendar ID="SharedCalendar1" runat="server" EnableMultiSelect="False" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False" ShowRowHeaders="false">

        <SpecialDays>
            <telerik:RadCalendarDay Repeatable="Today" ItemStyle-BackColor="SkyBlue"></telerik:RadCalendarDay>
        </SpecialDays>
    </telerik:RadCalendar>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
