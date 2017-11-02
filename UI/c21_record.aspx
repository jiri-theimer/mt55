<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="c21_record.aspx.vb" Inherits="UI.c21_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="10">
        <tr>

            <td style="width: 120px;">
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název fondu:"></asp:Label></td>
            <td>
                <asp:TextBox ID="c21Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
            <td>
                <asp:Label ID="lblOrdinary" Text="Index pořadí:" runat="server" CssClass="lbl"></asp:Label>

            </td>
            <td>
                <telerik:RadNumericTextBox ID="c21Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
    </table>
    <div class="div6">
        <asp:DropDownList ID="c21ScopeFlag" runat="server" AutoPostBack="true">
            <asp:ListItem Text="Základní pracovní kalendář" Value="1" Selected="true"></asp:ListItem>
            <asp:ListItem Text="Fond hodin je shodný s počtem vykázaných hodin" Value="3"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <asp:Panel ID="panScope1" runat="server">
        <table cellpadding="10">

            <tr>
                <td style="width: 120px;">
                    <asp:Label ID="lblc21Day1_Hours" Text="Pondělí:" runat="server" CssClass="lbl"></asp:Label></td>
                <td>

                    <telerik:RadNumericTextBox ID="c21Day1_Hours" runat="server" NumberFormat-DecimalDigits="2" Width="70px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
                    hod.
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label1" Text="Úterý:" runat="server" CssClass="lbl"></asp:Label></td>
                <td>

                    <telerik:RadNumericTextBox ID="c21Day2_Hours" runat="server" NumberFormat-DecimalDigits="2" Width="70px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
                    hod.
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label2" Text="Středa:" runat="server" CssClass="lbl"></asp:Label></td>
                <td>

                    <telerik:RadNumericTextBox ID="c21Day3_Hours" runat="server" NumberFormat-DecimalDigits="2" Width="70px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
                    hod.
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label3" Text="Čtvrtek:" runat="server" CssClass="lbl"></asp:Label></td>
                <td>

                    <telerik:RadNumericTextBox ID="c21Day4_Hours" runat="server" NumberFormat-DecimalDigits="2" Width="70px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
                    hod.
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label4" Text="Pátek:" runat="server" CssClass="lbl"></asp:Label></td>
                <td>

                    <telerik:RadNumericTextBox ID="c21Day5_Hours" runat="server" NumberFormat-DecimalDigits="2" Width="70px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
                    hod.
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label5" Text="Sobota:" runat="server" CssClass="lbl"></asp:Label></td>
                <td>

                    <telerik:RadNumericTextBox ID="c21Day6_Hours" runat="server" NumberFormat-DecimalDigits="2" Width="70px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
                    hod.
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label6" Text="Neděle:" runat="server" CssClass="lbl"></asp:Label></td>
                <td>

                    <telerik:RadNumericTextBox ID="c21Day7_Hours" runat="server" NumberFormat-DecimalDigits="2" Width="70px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
                    hod.
                </td>
            </tr>

        </table>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>


