<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_p41_record_billingsetting.aspx.vb" Inherits="UI.clue_p41_record_billingsetting" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="max-height: 270px; overflow-x: auto; overflow-y: auto; width: 100%;">
        <uc:pageheader ID="ph1" runat="server" Image="Images/billing.png" Text="Fakturační nastavení projektu" />

        <table cellpadding="10" cellspacing="2">

            <tr>
                <td style="width: 150px;">Fakturační jazyk:</td>
                <td>
                    <asp:Label ID="p87Name" runat="server" CssClass="valbold"></asp:Label>
                </td>

                <td>
                    <asp:Image ID="imgFlag" runat="server" />
                    <asp:Label ID="p87Name_add" runat="server" CssClass="infoInForm"></asp:Label>
                </td>

            </tr>
        </table>
        <asp:Panel ID="panTaxInvoiceScope" runat="server">
            <table cellpadding="10" cellspacing="2">
                <tr>
                    <td style="width: 150px;">Výchozí odběratel faktury:</td>
                    <td>
                        <asp:Label ID="Invoice_Receiver" runat="server" CssClass="valbold" Text="Klient projektu"></asp:Label>
                    </td>

                    <td>
                        <asp:Label ID="Invoice_Receiver_Add" runat="server" CssClass="infoInForm"></asp:Label>
                    </td>

                </tr>
                <tr>
                    <td>Výchozí typ faktury:</td>
                    <td>
                        <asp:Label ID="p92Name" runat="server" CssClass="valbold"></asp:Label>
                    </td>

                    <td>

                        <asp:Label ID="p92Name_add" runat="server" CssClass="infoInForm"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>Výchozí splatnost faktury:</td>
                    <td>
                        <asp:Label ID="DefaultMaturity" runat="server" CssClass="valbold"></asp:Label>
                    </td>

                    <td>

                        <asp:Label ID="DefaultMaturity_add" runat="server" CssClass="infoInForm"></asp:Label>
                    </td>
                </tr>

            </table>
         
            <asp:Panel ID="panInvoiceText1" runat="server" Visible="false" Style="width: 98%;">
                <fieldset style="background-color: #ffffcc;">
                    <legend>Výchozí text faktury</legend>
                    <div>
                        <asp:Label ID="p41InvoiceDefaultText1" runat="server"></asp:Label>

                    </div>
                </fieldset>
            </asp:Panel>
        </asp:Panel>
    </div>
</asp:Content>
