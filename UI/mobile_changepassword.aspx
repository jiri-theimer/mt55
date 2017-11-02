<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Mobile.Master" CodeBehind="mobile_changepassword.aspx.vb" Inherits="UI.mobile_changepassword" %>
<%@ MasterType VirtualPath="~/Mobile.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <div class="well">
            Změnit si přihlašovací heslo        
    </div>

    <div style="background-color: white; padding: 10px;">
        

        <p class="infoNotification">
            Heslo je povinné zadat a jeho minimální délka je <%= Membership.MinRequiredPasswordLength %> znaků.
        </p>
        <asp:ChangePassword ID="ChangeUserPassword" runat="server" CancelDestinationPageUrl="~/" EnableViewState="false" RenderOuterTable="false"
            SuccessPageUrl="mobile_myprofile.aspx">
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
