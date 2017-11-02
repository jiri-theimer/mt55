<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="periodcombo_setting.aspx.vb" Inherits="UI.periodcombo_setting" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="div6">
        <asp:Button ID="cmdAdd" runat="server" CssClass="cmd" Text="Přidat časové období" />
    </div>

    <table cellpadding="5" cellspacing="2">
        <tr>
            <th>Název</th>
            <th>Začátek</th>
            <th>Konec</th>
        </tr>
        <asp:Repeater ID="rp1" runat="server">
            <ItemTemplate>
                <tr id="trRow" runat="server">
                    <td>
                        <asp:TextBox ID="txtName" runat="server" style="width:150px;"></asp:TextBox>
                    </td>
                    <td>
                        <telerik:RadDatePicker ID="datFrom" runat="server" Width="120px" MaxDate="1.1.3000" MinDate="1.1.1900">
                            <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                        </telerik:RadDatePicker>
                    </td>
                 
                    <td>
                        <telerik:RadDatePicker ID="datUntil" runat="server" Width="120px" MaxDate="1.1.3000" MinDate="1.1.1900">
                            <DateInput ID="DateInput2" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                        </telerik:RadDatePicker>
                    </td>
                    <td style="padding-left:40px;">
                        <asp:ImageButton ID="del" runat="server" ImageUrl="Images/delete_row.png" ToolTip="Odstranit položku" CssClass="button-link" CommandName="delete" />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
    
    <div style="display:none;">
    <telerik:RadCalendar ID="SharedCalendar" runat="server" EnableMultiSelect="False" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
        <SpecialDays>
                    <telerik:RadCalendarDay Repeatable="Today" ItemStyle-BackColor="SkyBlue"></telerik:RadCalendarDay>
                </SpecialDays>
    </telerik:RadCalendar>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
