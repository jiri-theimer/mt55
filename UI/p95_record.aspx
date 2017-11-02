<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p95_record.aspx.vb" Inherits="UI.p95_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datalabel" Src="~/datalabel.ascx" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="3" cellspacing="2">
        <tr>
            <td>
                <uc:datalabel runat="server" ID="lblName" Text="Název:" GLX="83" IsRequired="true"></uc:datalabel>
            </td>
            <td>
                <asp:TextBox ID="p95name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <uc:datalabel runat="server" ID="lblCode" Text="Kód:"></uc:datalabel>
            </td>
            <td>
                <asp:TextBox ID="p95Code" runat="server" Style="width: 100px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <uc:datalabel runat="server" ID="Label2" Text="Index pořadí:" GLX="87"></uc:datalabel>
            </td>
            <td>

                <telerik:RadNumericTextBox ID="p95Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
    </table>

    <div class="content-box2">
        <div class="title">
            <asp:Label ID="ph1" runat="server" Text="Překlad do ostatních fakturačních jazyků" />
        </div>

        <asp:Panel ID="panLang" runat="server" CssClass="content">
            <table cellpadding="3" cellspacing="2">
                <tr>
                    <td>
                        <uc:datalabel runat="server" ID="lblLang1" Text="Jazyk 1:"></uc:datalabel>
                    </td>
                    <td>
                        <asp:TextBox ID="p95Name_BillingLang1" runat="server" Style="width: 400px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc:datalabel runat="server" ID="lblLang2" Text="Jazyk 2:"></uc:datalabel>
                    </td>
                    <td>
                        <asp:TextBox ID="p95Name_BillingLang2" runat="server" Style="width: 400px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc:datalabel runat="server" ID="lblLang3" Text="Jazyk 3:"></uc:datalabel>
                    </td>
                    <td>
                        <asp:TextBox ID="p95Name_BillingLang3" runat="server" Style="width: 400px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc:datalabel runat="server" ID="lblLang4" Text="Název (Jazyk 4):"></uc:datalabel>
                    </td>
                    <td>
                        <asp:TextBox ID="p95Name_BillingLang4" runat="server" Style="width: 400px;"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>

