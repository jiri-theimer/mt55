<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="o41_record.aspx.vb" Inherits="UI.o41_record" %>

<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="3" cellspacing="2">
        <tr>
            <td>
                <asp:Label ID="Label1" Text="Název:" runat="server" CssClass="lblReq"></asp:Label></td>
            <td>
                <asp:TextBox ID="o41Name" runat="server" Style="width: 300px;"></asp:TextBox>
                (jakýkoliv výraz)
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label2" Text="Adresa serveru:" runat="server" CssClass="lblReq"></asp:Label></td>
            <td>
                <asp:TextBox ID="o41Server" runat="server" Style="width: 300px;"></asp:TextBox>

            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblo41Login" Text="Login účtu:" runat="server" CssClass="lblReq"></asp:Label></td>
            <td>
                <asp:TextBox ID="o41login" runat="server" Style="width: 300px;"></asp:TextBox>

            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblo41Password" Text="Heslo:" runat="server" CssClass="lbl"></asp:Label></td>
            <td>
                <asp:TextBox ID="o41Password" runat="server" Style="width: 300px;" TextMode="Password"></asp:TextBox>
                <asp:Button ID="cmdChangePWD" runat="server" Text="Změnit heslo" CssClass="cmd" Visible="false" />
            </td>
        </tr>

        <tr>
            <td>
                <asp:Label ID="Label3" Text="Port:" runat="server" CssClass="lbl"></asp:Label></td>
            <td>
                <asp:TextBox ID="o41Port" runat="server" Style="width: 50px;"></asp:TextBox>

            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblo41Folder" Text="Složka:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="o41Folder" runat="server" Text="inbox" Style="width: 300px;"></asp:TextBox>

            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="o41IsDeleteMesageAfterImport" runat="server" Text="Po zpracování zprávu v mailboxu nenávratně odstranit" Checked="true" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label4" Text="SSL mód:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="o41SslModeFlag" runat="server">
                    <asp:ListItem Text="Bez SSL" Value="0"></asp:ListItem>
                    <asp:ListItem Text="SSL implicit" Value="1"></asp:ListItem>
                    <asp:ListItem Text="SSL explicit" Value="2"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>

      


    </table>
    <p></p>
    <asp:Button ID="cmdTest" runat="server" CssClass="cmd" Text="Otestovat připojení k účtu" />

    <div class="content-box2" style="margin-top:20px;">
        <div class="title">
            Odesílat upozornění
        </div>
        <div class="content">

            <div class="div6">
                <span>Upozornění na nový úkol/dokument:</span>
                <asp:DropDownList ID="o41ForwardFlag_New" runat="server" AutoPostBack="true">
                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                    <asp:ListItem Value="1" Text="Odesílat osobám s rolí v úkolu/dokumentu"></asp:ListItem>
                    <asp:ListItem Value="2" Text="Odesílat na zde zadanou adresu"></asp:ListItem>
                </asp:DropDownList>
                <span>(lze nastavit i ve worfklow šabloně)</span>
                
                    <span>E-mail:</span>
                    <asp:TextBox ID="o41ForwardEmail_New" runat="server" Width="200px" Text="@" ToolTip="E-mail adresa"></asp:TextBox>
                
                
            </div>
            <hr />
            <div class="div6">
                <span>Upozornění na odpověď k existujícímu úkolu/dokumentu:</span>
                <asp:DropDownList ID="o41ForwardFlag_Answer" runat="server" AutoPostBack="true">
                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                    <asp:ListItem Value="1" Text="Odesílat osobám s rolí v úkolu/dokumentu"></asp:ListItem>
                    <asp:ListItem Value="2" Text="Odesílat na zde zadanou adresu"></asp:ListItem>
                </asp:DropDownList>
               
                    <span>E-mail:</span>
                    <asp:TextBox ID="o41ForwardEmail_Answer" runat="server" Width="200px" Text="@" ToolTip="E-mail adresa"></asp:TextBox>
                
            </div>
         
            <hr />
            <div class="div6">
                <span>E-mail adresa pro nepřiřazené zprávy:</span>
                <asp:TextBox ID="o41ForwardEmail_UnBound" runat="server" Width="200px"></asp:TextBox>
            </div>
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
