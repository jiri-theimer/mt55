<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p31_record_split.aspx.vb" Inherits="UI.p31_record_split" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>



<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="6">
        <tr>
            <td>Osoba:
            </td>
            <td>
                <asp:Label ID="Person" runat="server" CssClass="valbold"></asp:Label>
            </td>
            <td>Datum:
            </td>
            <td>
                <asp:Label ID="p31Date" runat="server" CssClass="valbold"></asp:Label>
            </td>
            <td>Hodiny:
            </td>
            <td>
                <asp:Label ID="p31Hours" runat="server" CssClass="valbold"></asp:Label>
            </td>

            <td>Projekt:
            </td>
            <td>
                <asp:Label ID="Project" runat="server" CssClass="valbold"></asp:Label>
            </td>
        </tr>
    </table>

    <div class="content-box2">
        <div class="title">První (zdrojový) úkon</div>
        <div class="content">
            <table>
                <tr>

                    <td>
                        <asp:Label ID="lblHours1" runat="server" Text="Hodiny:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <telerik:RadNumericTextBox ID="hours1" runat="server" Width="100px" NumberFormat-DecimalDigits="2"></telerik:RadNumericTextBox>

                    </td>

                </tr>
                <tr valign="top">
                    <td>
                        <asp:Label ID="lblTxt1" runat="server" Text="Popis:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txt1" runat="server" TextMode="MultiLine" Style="width: 400px; height: 100px;"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="content-box2">
        <div class="title">Druhý (nový) úkon</div>
        <div class="content">
            <table>
                <tr>

                    <td>
                        <asp:Label ID="lblHours2" runat="server" Text="Hodiny:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <telerik:RadNumericTextBox ID="hours2" runat="server" Width="100px" NumberFormat-DecimalDigits="2"></telerik:RadNumericTextBox>

                    </td>

                </tr>
                <tr valign="top">
                    <td>
                        <asp:Label ID="lblTxt2" runat="server" Text="Popis:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txt2" runat="server" TextMode="MultiLine" Style="width: 400px; height: 100px;"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
