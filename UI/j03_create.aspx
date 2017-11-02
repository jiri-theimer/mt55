<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="j03_create.aspx.vb" Inherits="UI.j03_create" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/PageHeader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="3" cellspacing="2">
        <tr>
            <td style="width: 160px;">
                <asp:Label ID="lblJ03Login" Text="Přihlašovací jméno (login):" runat="server" AssociatedControlID="j03login" CssClass="lblReq"></asp:Label></td>
            <td>
                <asp:TextBox ID="j03login" runat="server" Style="width: 300px;"></asp:TextBox>
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
                <asp:Label ID="lblPassword" runat="server" Text="Přístupové heslo:" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtPassword" runat="server" Style="width: 300px;" TextMode="SingleLine"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblVerify" runat="server" Text="Ověření hesla:" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtVerify" runat="server" Style="width: 300px;" TextMode="SingleLine"></asp:TextBox>
            </td>
        </tr>

    </table>
    <div class="div6">
        <asp:CheckBox ID="j03IsMustChangePassword" runat="server" Text="Uživatel si musí okamžitě změnit přihlašovací heslo" />
    </div>
    <div class="div6">
        <asp:Label ID="Label2" runat="server" Text="Heslo expiruje dne:" CssClass="lbl"></asp:Label>
        <telerik:RadDatePicker ID="j03PasswordExpiration" runat="server" Width="120px">
                    <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                </telerik:RadDatePicker>
        <span class="infoInForm">Pokud datum expirace hesla není vyplněno, heslo platí na dobu neurčitou.</span>
    </div>


    <div class="content-box2">
        <div class="title">
            <asp:Label ID="ph1" runat="server" Text="Osobní profil" />
        </div>
        <div class="content">
            <asp:RadioButtonList ID="opgJ02Bind" runat="server" AutoPostBack="true" RepeatDirection="Vertical">
                <asp:ListItem Text="Založit nový osobní profil uživatele" Value="1" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Svázat uživatele s již existujícím osobním profilem" Value="2"></asp:ListItem>
            </asp:RadioButtonList>
            <div style="width: 100%; border-top: dotted gray 1px;"></div>

            <asp:Panel ID="panSearchJ02" runat="server" Style="padding: 6px;">
                <asp:Label ID="lblJ02ID" runat="server" Text="Vyhledat osobní profil:" CssClass="lbl"></asp:Label>
                <uc:person ID="j02id_search" runat="server" />
            </asp:Panel>

            <asp:Panel ID="panNewJ02" runat="server">
                <table cellpadding="3" cellspacing="2">
                    <tr>
                        <td style="width: 160px;">
                            <asp:Label ID="lblTitle" Text="Titul:" runat="server" CssClass="lbl" AssociatedControlID="j02TitleBeforeName" meta:resourcekey="lblTitlex"></asp:Label>
                        </td>
                        <td>
                            <uc:datacombo ID="j02TitleBeforeName" runat="server" Width="80px" AllowCustomText="true" ShowToggleImage="false" Filter="Contains" DefaultValues="Bc.;BcA.;Ing.;Ing.arch.;MUDr.;MVDr.;MgA.;Mgr.;JUDr.;PhDr.;RNDr.;PharmDr.;ThLic.;ThDr.;Ph.D.;Th.D.;prof.;doc.;PaedDr.;Dr.;PhMr."></uc:datacombo>
                        </td>
                        <td>
                            <asp:Label ID="lblFirstName" Text="Jméno:" runat="server" CssClass="lblReq" AssociatedControlID="j02FirstName" meta:resourcekey="lblFirstName"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="j02FirstName" runat="server" Style="width: 100px;"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lblLastName" Text="Příjmení:" runat="server" CssClass="lblReq" AssociatedControlID="j02LastName" meta:resourcekey="lblLastName"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="j02LastName" runat="server" Style="width: 160px;"></asp:TextBox>
                        </td>
                        <td>
                            <uc:datacombo ID="j02TitleAfterName" runat="server" Width="70px" AllowCustomText="true" ShowToggleImage="false" Filter="Contains" DefaultValues="CSc.;DrSc.;dr. h. c.;DiS."></uc:datacombo>
                        </td>
                    </tr>
                </table>
                <table cellpadding="3" cellspacing="2">
                    <tr>
                        <td style="width: 150px;">
                            <asp:Label ID="lblj02Email" Text="E-mail adresa:" runat="server" CssClass="lblReq" AssociatedControlID="j02Email"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="j02Email" runat="server" Style="width: 300px;"></asp:TextBox>
                            <div>
                                <asp:RegularExpressionValidator ID="emailValidator" runat="server" Display="Dynamic" ForeColor="Red" ErrorMessage="Zadejte validní e-mail adresu." ValidationExpression="^[\w\.\-]+@[a-zA-Z0-9\-]+(\.[a-zA-Z0-9\-]{1,})*(\.[a-zA-Z]{2,4}){1,2}$" ControlToValidate="j02Email"></asp:RegularExpressionValidator>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblPosition" Text="Pozice (hladina sazby):" runat="server" CssClass="lbl"></asp:Label></td>
                        <td>
                            <uc:datacombo ID="j07ID" runat="server" DataTextField="j07Name" DataValueField="pid" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" Text="Pracovní fond:" runat="server" CssClass="lbl"></asp:Label></td>
                        <td>
                            <uc:datacombo ID="c21ID" runat="server" DataTextField="c21Name" DataValueField="pid" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblMobile" Text="Mobil:" runat="server" CssClass="lbl" AssociatedControlID="j02Mobile"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="j02Mobile" runat="server" Style="width: 300px;"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" Text="Pevný telefon:" runat="server" CssClass="lbl" AssociatedControlID="j02Phone"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="j02Phone" runat="server" Style="width: 300px;"></asp:TextBox>
                        </td>
                    </tr>

                </table>
            </asp:Panel>
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
