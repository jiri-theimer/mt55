<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Anonym.Master" CodeBehind="kickoff_after2.aspx.vb" Inherits="UI.kickoff_after2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="div6">
        <asp:Label ID="lblHeader" runat="server" CssClass="page_header_span" Text="Dokončení úvodního nastavení systému"></asp:Label>
    </div>
    <div>
        <asp:Label ID="lblMessage" runat="server" CssClass="infoNotificationRed"></asp:Label>
    </div>
    <div class="content-box2">
        <div class="title">Složka pro upload souborových příloh</div>
        <div class="content">
            Upload složka (server cesta):
            <asp:TextBox ID="txtUploadFolder" runat="server" Width="400px" Text="C:\INETPUB\WWWROOT\MARKTIME50_UPLOAD"></asp:TextBox>
            <div>
                <span class="infoInForm">Do server složky musíte nastavit file-systém oprávnění [Modify] pro IIS uživatele.</span>
            </div>
        </div>

    </div>
    <div class="content-box2">
        <div class="title">Aplikační URL adresa</div>
        <div class="content">
            <table>
                <tr>
                    <td>Oficiální URL adresa pro uživatele:
                    </td>
                    <td>
                        <asp:TextBox ID="AppHost" runat="server" Width="300px"></asp:TextBox>
                    </td>
                    <td>
                        <span class="infoInForm">URL, kterou bude systém uvádět jako odkaz v e-mail zprávách.</span>
                    </td>
                </tr>
                <tr valign="top">
                    <td>URL pro spouštění robota na pozadí:
                    </td>
                    <td>
                        <asp:TextBox ID="robot_host" runat="server" Width="300px"></asp:TextBox>
                    </td>
                    <td>
                        <span class="infoInForm">Robot běží na pozadí a stará se o automatické procesy jako workflow stavový mechanismus, rozesílání poštovních zpráv, generování opakovaných úkonů atd.</span>
                        <div>
                            <span class="infoInForm">URL robota musí být spustitelná uvnitř na IIS serveru (localhost).</span>
                        </div>
                    </td>
                </tr>
            </table>



        </div>
    </div>
    <p></p>
   
    <div class="div6">
        <span>Aplikační e-mail adresa:</span>
        <asp:TextBox ID="SMTP_SenderAddress" runat="server" Style="width: 150px;" Text="nasefirma@marktime.cz"></asp:TextBox>
    </div>
    <div>
        <span class="infoNotification">V konfiguračním souboru WEB.config nezapomeňte nastavit aplikační SMTP účet a file-system cestu na LOG soubory!</span>
    </div>

    

    <div class="div6">
        <asp:Button ID="cmdGo" runat="server" CssClass="cmd" Text="Uložit nastavení a pokračovat ->" />
        <asp:Label ID="lblError" runat="server" ForeColor="red" Font-Size="120%" Font-Bold="true"></asp:Label>
    </div>
</asp:Content>
