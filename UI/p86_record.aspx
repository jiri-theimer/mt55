<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p86_record.aspx.vb" Inherits="UI.p86_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="6" cellspacing="2">
        <tr>
            <td style="width: 180px;">
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název účtu:"></asp:Label></td>
            <td>
                <asp:TextBox ID="p86Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="width: 180px;">
                <asp:Label ID="Label5" runat="server" CssClass="lbl" Text="Název banky:"></asp:Label></td>
            <td>
                <asp:TextBox ID="p86BankName" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label1" runat="server" CssClass="lblReq" Text="Číslo účtu:"></asp:Label></td>
            <td>
                <asp:TextBox ID="p86BankAccount" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>
                <asp:Label ID="Label3" runat="server" CssClass="lbl" Text="Kód banky:"></asp:Label></td>
            <td>
                <asp:TextBox ID="p86BankCode" runat="server" Style="width: 70px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label2" runat="server" CssClass="lbl" Text="SWIFT kód:"></asp:Label></td>
            <td>
                <asp:TextBox ID="p86SWIFT" runat="server" Style="width: 400px;"></asp:TextBox>


            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label4" runat="server" CssClass="lbl" Text="IBAN kód:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="p86IBAN" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
        <tr valign="top">
            <td>
                <asp:Label ID="Label7" runat="server" CssClass="lbl" Text="Adresa banky:"></asp:Label>

            </td>
            <td>
                <asp:TextBox ID="p86BankAddress" runat="server" Style="width: 400px; height: 40px;" TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
    </table>
    <div class="div6">
        <span class="infoInForm">Měna účtu se nastavuje až v nastavení hlavičky vystavovatele faktury, kde přiřazujete účty k měnám.</span>
    </div>


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>





