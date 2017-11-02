<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p91_pay.aspx.vb" Inherits="UI.p91_pay" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="10">
        <tr>
            <td>
                <asp:Label ID="Label1" runat="server" CssClass="lbl" Text="Datum úhrady:"></asp:Label>
            </td>
            <td>
                <telerik:RadDatePicker ID="p94Date" runat="server" Width="120px" DateInput-EmptyMessage="Povinný údaj." DateInput-EmptyMessageStyle-ForeColor="red">
                    <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                    <Calendar runat="server">
                                <SpecialDays>
                                    <telerik:RadCalendarDay Repeatable="Today" ItemStyle-BackColor="SkyBlue"></telerik:RadCalendarDay>
                                </SpecialDays>
                            </Calendar>
                </telerik:RadDatePicker>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblValue" runat="server" CssClass="lbl" Text="Částka úhrady:"></asp:Label>
            </td>
            <td>
                <telerik:RadNumericTextBox ID="p94Amount" runat="server" Width="140px" NumberFormat-DecimalDigits="2"></telerik:RadNumericTextBox>
                <asp:Label ID="j27Code" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblDescription" runat="server" CssClass="lbl" Text="Poznámka:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="p94Description" runat="server" Width="400px"></asp:TextBox>
            </td>
        </tr>
    </table>

    <fieldset>
        <legend>Historie úhrad faktury</legend>
        <table cellpadding="10">
            <asp:Repeater ID="rpHistory" runat="server">
                <ItemTemplate>
                    <tr>
                        <td>
                            <%#BO.BAS.FD(Eval("p94Date"), True)%>
                        </td>
                        <td style="font-weight:bold;text-align:right;">
                            <%#BO.BAS.FN(Eval("p94Amount"))%>
                        </td>
                        <td>
                            <asp:Button ID="cmdDelete" runat="server" CssClass="cmd" Text="Odstranit úhradu z faktury" />
                        </td>
                        <td>
                            <%#Eval("p94Description")%>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <tr>
                <td>Dluh:</td>
                <td align="right">
                    <asp:Label ID="p91Amount_Debt" runat="server" CssClass="valboldred"></asp:Label>
                </td>
                <td>

                </td>
                <td>

                </td>
            </tr>
        </table>
    </fieldset>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
