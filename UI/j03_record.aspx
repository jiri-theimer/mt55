<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="j03_record.aspx.vb" Inherits="UI.j03_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/PageHeader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {

           


        });

        
        function passwordrecovery() {
            if (confirm("Chcete uživateli vygenerovat nové přístupové heslo?")) {
                return (true);
            }
            else
                return (false);

        }

        function changelogin() {
            var s = window.prompt("Zadejte nové přihlašovací jméno (změnou dojde automaticky k vygenerování nového přístupového hesla)");

            if (s != '' && s != null) {
                self.document.getElementById("<%=hidNewLogin.clientid%>").value = s;

                return (true);
            }

            return (false);

        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="3" cellspacing="2">
        <tr>
            <td style="width: 160px;">
                <asp:Label ID="lblJ03Login" Text="Přihlašovací jméno (login):" runat="server" AssociatedControlID="j03login" CssClass="lblReq"></asp:Label></td>
            <td>
                <asp:TextBox ID="j03login" runat="server" Style="width: 300px;" Enabled="false"></asp:TextBox>
                <asp:CheckBox ID="j03IsLiveChatSupport" runat="server" Text="Zapnutá Live chat MARKTIME podpora" Visible="false" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="lblLoginWarning" runat="server" Style="color: Red;" />
            </td>
        </tr>

        <tr>
            <td>
                <asp:Label ID="lblRole" Text="Aplikační role:" runat="server" CssClass="lblReq" AssociatedControlID="j04id"></asp:Label></td>
            <td>
                <uc:datacombo ID="j04id" runat="server" DataTextField="j04name" DataValueField="pid" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>

            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblJ02ID" runat="server" Text="Osobní profil:" CssClass="lblReq"></asp:Label>

            </td>
            <td>
                <uc:person ID="j02ID" runat="server" Width="300px" />

            </td>
        </tr>
    </table>

    <div class="div6">
        <asp:CheckBox ID="j03IsMustChangePassword" runat="server" Text="Uživatel si musí okamžitě změnit přihlašovací heslo" />
    </div>
    <div class="div6">
        <asp:Label ID="Label1" runat="server" Text="Heslo expiruje dne:" CssClass="lbl"></asp:Label>
        <telerik:RadDatePicker ID="j03PasswordExpiration" runat="server" Width="120px">
            <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
        </telerik:RadDatePicker>
        <span class="infoInForm">Pokud datum expirace hesla není vyplněno, heslo platí na dobu neurčitou.</span>
    </div>

    <asp:Panel ID="panService" runat="server" CssClass="content-box2" Style="margin-top: 20px;">
        <div class="title">
            <asp:Label ID="ph3" runat="server" Text="Servisní funkce" />
        </div>
        <div class="content">
            <asp:Button ID="cmdRecoveryPassword" runat="server" Text="Vygenerovat nové přístupové heslo" CssClass="cmd" OnClientClick="return passwordrecovery()" />
            <asp:Button ID="cmdChangeLogin" runat="server" Text="Změnit přihlašovací jméno" CssClass="cmd" OnClientClick="return changelogin()" />
            <asp:Button ID="cmdDeleteUserParams" runat="server" Text="Vyčistit paměť (cache) v uživatelském profilu" CssClass="cmd" />
            <asp:Button ID="cmdRecoveryMembership" runat="server" Text="Opravit membership účet" CssClass="cmd" />


            <div>
                <asp:Button ID="cmdUnlockMembership" runat="server" Text="Odblokovat membership účet" CssClass="cmd" Visible="false" />
            </div>
            <asp:Panel ID="panPasswordRecovery" runat="server" Style="background-color: orange;" Visible="false">
                <div style="padding: 6px;">
                    <asp:Label ID="lblNewPasswordLabel" runat="server" Text="Nové přístupové heslo:" ForeColor="black"></asp:Label>

                    <asp:TextBox ID="txtNewPassword" runat="server" Style="width: 150px;"></asp:TextBox>
                    <asp:Button ID="cmdGeneratePasswordAgain" runat="server" Text="Pře-generovat heslo" CssClass="cmd" />
                    <span>Heslo můžete upravit (přepsat) a pře-generovat na tuto hodnotu.</span>
                    <asp:Button ID="cmdResetPasswordMessage" runat="server" Text="Odeslat uživateli zprávu o novém heslu" CssClass="cmd" />
                </div>
            </asp:Panel>

        </div>


    </asp:Panel>
    <asp:Panel ID="panSwitchUser" runat="server" CssClass="content-box2" Style="margin-top: 10px;" Visible="false">
        <div class="title">
            Přepnout si prostředí pod identitou uživatele
        </div>
        <div class="content">
            <span>PIN pro super-user operace:</span>
            <asp:TextBox ID="txtPIN" runat="server" TextMode="Password"></asp:TextBox>
            <asp:Button ID="cmdSwitch2User" runat="server" Text="Přepnout se na uživatele" CssClass="cmd" />
        </div>
        
    </asp:Panel>

    <asp:HiddenField ID="hidNewLogin" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
