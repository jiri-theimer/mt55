<%@ Page Title="Change Password" Language="vb" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeBehind="ChangePassword.aspx.vb" Inherits="UI.ChangePassword" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div style="background-color: white; padding: 10px;">
        <table cellpadding="5">
            <tr>
                <td>
                    <img src="Images/password_32.png" />
                </td>
                <td>
                    <asp:Label ID="lblHeader" runat="server" CssClass="framework_header_span" Style="font-size: 200%;" Text="Změna přihlašovacího hesla"></asp:Label>

                </td>

            </tr>
        </table>



        <p class="infoNotification">
            Heslo je povinné zadat a jeho minimální délka je <%= Membership.MinRequiredPasswordLength %> znaků.
        </p>
        <asp:ChangePassword ID="ChangeUserPassword" runat="server" CancelDestinationPageUrl="~/" EnableViewState="false" RenderOuterTable="false"
            SuccessPageUrl="ChangePasswordSuccess.aspx">
            <ChangePasswordTemplate>
                <span class="failureNotification">
                    <asp:Literal ID="FailureText" runat="server"></asp:Literal>
                </span>
                <asp:ValidationSummary ID="ChangeUserPasswordValidationSummary" runat="server" CssClass="failureNotification"
                    ValidationGroup="ChangeUserPasswordValidationGroup" />
                <table cellpadding="10">

                    <tr>

                        <td>
                            <asp:Label ID="CurrentPasswordLabel" runat="server" AssociatedControlID="CurrentPassword">Původní heslo:</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="CurrentPassword" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" ControlToValidate="CurrentPassword"
                                CssClass="failureNotification" ErrorMessage="Musíte zadat původní heslo." ToolTip="Musíte zadat původní heslo."
                                ValidationGroup="ChangeUserPasswordValidationGroup">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword">Nové heslo:</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="NewPassword" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword"
                                CssClass="failureNotification" ErrorMessage="Musíte zadat nové heslo." ToolTip="Musíte zadat nové heslo."
                                ValidationGroup="ChangeUserPasswordValidationGroup">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword">Ověření nového hesla:</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="ConfirmNewPassword" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" ControlToValidate="ConfirmNewPassword"
                                CssClass="failureNotification" Display="Dynamic" ErrorMessage="Musíte zadat ověření nového hesla."
                                ToolTip="Musíte zadat ověření nového hesla." ValidationGroup="ChangeUserPasswordValidationGroup">*</asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="NewPassword" ControlToValidate="ConfirmNewPassword"
                                CssClass="failureNotification" Display="Dynamic" ErrorMessage="Ověření musí souhlasit s novým heslem."
                                ValidationGroup="ChangeUserPasswordValidationGroup">*</asp:CompareValidator>
                        </td>
                    </tr>
                </table>
                <p class="submitButton">
                    <asp:Button ID="CancelPushButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel" Visible="false" />
                    <asp:Button ID="ChangePasswordPushButton" runat="server" CommandName="ChangePassword" CssClass="cmd" Text="Uložit nové heslo"
                        ValidationGroup="ChangeUserPasswordValidationGroup" />
                </p>

            </ChangePasswordTemplate>
        </asp:ChangePassword>
    </div>
</asp:Content>
