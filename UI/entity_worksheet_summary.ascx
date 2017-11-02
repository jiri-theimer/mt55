<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="entity_worksheet_summary.ascx.vb" Inherits="UI.entity_worksheet_summary" %>
<table style="table-layout: fixed; padding: 3px;">
    <tr id="trRealFee" runat="server" visible="false">
        <td>Honorář z rozpracovaných hodin:</td>
        <td style="text-align: right;">
            <asp:Label ID="WaitingOnApproval_HoursFee" runat="server" CssClass="val"></asp:Label>
        </td>
        <td></td>
    </tr>
    <tr id="trLimitFee" runat="server" visible="false">
        <td>Limit rozpracovaného honoráře:</td>
        <td style="text-align: right;">
            <asp:Label ID="p41LimitFee_Notification" runat="server" CssClass="val" ForeColor="Red"></asp:Label>
        </td>
        <td>
            <img src="Images/finplan_32.png" />
            <asp:Label ID="Perc2" runat="server" CssClass="valboldblue"></asp:Label>
        </td>
    </tr>


    <tr>
        <td></td>
        <td style="text-align: right; font-size: 80%;">
            <asp:Label ID="lblHodiny" runat="server" Text="Hodiny" meta:resourcekey="lblHodiny"></asp:Label></td>

        <td style="text-align: right; font-size: 80%;">
            <asp:Label ID="lblOstatni" runat="server" Text="Ostatní" meta:resourcekey="lblOstatni"></asp:Label></td>
    </tr>


    <tr id="trWait4Approval" runat="server">

        <td style="text-align: right;">
            <asp:Label ID="WaitingOnApproval_Hours_Sum" runat="server" CssClass="val" ForeColor="red"></asp:Label>
        </td>
        <td style="text-align: right; padding-left: 20px;">
            <asp:Label ID="WaitingOnApproval_Other_Sum" runat="server" CssClass="val" ForeColor="red"></asp:Label>
        </td>
    </tr>

    <tr id="trLimitHours" runat="server" visible="false">
        <td>Limit rozpracovaných hodin:</td>
        <td style="text-align: right;">
            <asp:Label ID="p41LimitHours_Notification" runat="server" CssClass="val" ForeColor="Red"></asp:Label>
        </td>
        <td>
            <img src="Images/finplan_32.png" />
            <asp:Label ID="Perc1" runat="server" CssClass="valboldblue"></asp:Label>
        </td>
    </tr>
    <tr id="trWait4Invoice" runat="server">

        <td>
            <asp:Label ID="lblCekaNaFakturaci" runat="server" Text="Schváleno, čeká na fakturaci:" meta:resourcekey="lblCekaNaFakturaci"></asp:Label></td>
        <td style="text-align: right;">
            <asp:Label ID="WaitingOnInvoice_Hours_Sum" runat="server" CssClass="valbold" ForeColor="green"></asp:Label>
        </td>
        <td style="text-align: right; padding-left: 20px;">
            <asp:Label ID="WaitingOnInvoice_Other_Sum" runat="server" CssClass="valbold" ForeColor="green"></asp:Label>
        </td>
    </tr>
    <tr>

        <td>
            <asp:Label ID="lblCelkemVykazano" runat="server" Text="Celkem vykázáno:" meta:resourcekey="lblCelkemVykazano"></asp:Label>
        </td>
        <td style="text-align: right;">
            <asp:Label ID="p31Hours_Orig" runat="server" CssClass="val"></asp:Label>
        </td>
        <td></td>
    </tr>

</table>
