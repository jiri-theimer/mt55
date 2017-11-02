<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Anonym.Master" CodeBehind="kickoff_after1.aspx.vb" Inherits="UI.kickoff_after1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="div6">
        <asp:Label ID="lblHeader" runat="server" CssClass="page_header_span" Text="První nastavení systému"></asp:Label>
    </div>

    <div class="content-box2">
        <div class="title">Základní informace o Vaší společnosti</div>
        <div class="content">

            <table cellpadding="10">

                <tr style="vertical-align:top;">
                    <td><span class="lblReq">Název Vaší společnosti:</span></td>
                    <td>
                        <asp:TextBox ID="txtCompany" runat="server" Width="400px"></asp:TextBox>
                    </td>
                    <td rowspan="6">
                        <asp:RadioButtonList ID="cbxBusiness" runat="server" RepeatDirection="Vertical" CellPadding="5">
                            <asp:ListItem Text="Advokátní kancelář" Value="AK"></asp:ListItem>
                            <asp:ListItem Text="Účetnictví | Daně | Audit" Value="UCTO"></asp:ListItem>
                            <asp:ListItem Text="Projektanti | Architekti" Value="PRO"></asp:ListItem>
                            <asp:ListItem Text="DTP | Grafika | Reklama | PR agentura" Value="MEDIA"></asp:ListItem>
                            <asp:ListItem Text="IT | SoftwareHouse" Value="IT"></asp:ListItem>
                            <asp:ListItem Text="Ostatní" Value="OTHER"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td>Ulice:</td>
                    <td>
                        <asp:TextBox ID="txtStreet" runat="server" Width="400px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Město:</td>
                    <td>
                        <asp:TextBox ID="txtCity" runat="server" Width="200px"></asp:TextBox>
                        <span>PSČ:</span>
                        <span><asp:TextBox ID="txtPostCode" runat="server" Width="50px"></asp:TextBox></span>
                    </td>
                </tr>
               
                <tr>
                    <td>IČ:</td>
                    <td>
                        <asp:TextBox ID="txtIC" runat="server" Width="100px"></asp:TextBox>
                        <span>DIČ:</span>
                        <span>
                            <asp:TextBox ID="txtDIC" runat="server" Width="100px"></asp:TextBox>
                        </span>
                    </td>
                </tr>
               
                <tr>
                    <td>Bankovní účet:</td>
                    <td>
                        <asp:TextBox ID="txtBankAccount" runat="server" Width="200px"></asp:TextBox>
                        <span>Kód banky:</span>
                        <asp:TextBox ID="txtBankCode" runat="server" Width="50px"></asp:TextBox>
                    </td>
                </tr>
            </table>

        </div>
    </div>
    <div class="content-box2">
        <div class="title">Uveďte název jednoho z vašich klientů + jeho 2 projekty</div>
        <div class="content">
            <table cellpadding="10">

                <tr>
                    <td><span class="lblReq">Název klienta:</span></td>
                    <td>
                        <asp:TextBox ID="txtClient" runat="server" Width="400px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td><span class="lbl">Název prvního projektu:</span></td>
                    <td>
                        <asp:TextBox ID="txtProject1" runat="server" Width="400px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td><span class="lbl">Název druhého projektu:</span></td>
                    <td>
                        <asp:TextBox ID="txtProject2" runat="server" Width="400px"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="content-box2">
        <div class="title">Výchozí fakturační nastavení</div>
        <div class="content">
            <table cellpadding="10">
                <tr>
                    <td>Výchozí fakturační měna:</td>
                    <td>
                        <asp:DropDownList ID="j27id" runat="server" DataTextField="j27Code" DataValueField="pid"></asp:DropDownList>
                    </td>
                    <td>Snížená sazba DPH:
                    </td>
                    <td>
                        <asp:TextBox ID="txtVatRateLow" runat="server" Width="50px" Text="15" Style="text-align: right;"></asp:TextBox>%
                    </td>
                    <td>Základní sazba DPH:
                    </td>
                    <td>
                        <asp:TextBox ID="txtVatRateStandard" runat="server" Width="50px" Text="21" Style="text-align: right;"></asp:TextBox>%
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="div6">
        <asp:Button ID="cmdGo" runat="server" CssClass="cmd" Text="Uložit nastavení a pokračovat ->" />
        <asp:Label ID="lblError" runat="server" ForeColor="red" Font-Size="120%" Font-Bold="true"></asp:Label>
    </div>

</asp:Content>
