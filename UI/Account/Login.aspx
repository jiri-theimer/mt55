<%@ Page Title="Log In" Language="vb" MasterPageFile="~/Anonym.Master" AutoEventWireup="false" CodeBehind="Login.aspx.vb" Inherits="UI.Login" %>

<%@ MasterType VirtualPath="~/Anonym.Master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">

    <script type="text/javascript">

        function loadresolution() {
            document.getElementById("<%=screenwidth.clientid%>").value = screen.width;
            document.getElementById("<%=screenheight.clientid%>").value = screen.height;
        }

        function testbrowser() {

            var ua = navigator.userAgent.toLowerCase();
            var s = "";

            if (ua.indexOf('msie 6.0') >= 0)
                s = "Přistupujete do systému z prohlížeče [Internet Explorer verze 6], který není podporován! Aktualizujte si IE nebo využijte jiný prohlížeč (Google Chrome, Firefox, Safari...).";

            if (ua.indexOf('msie 7.0') >= 0)
                s = "Přistupujete do systému z prohlížeče [Internet Explorer verze 7], který není podporován! Aktualizujte si IE nebo využijte jiný prohlížeč (Google Chrome, Firefox, Safari...).";


            document.getElementById("<%=lblBrowserInfo.clientid%>").innerText = s;

        }
    </script>


</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
 
    <asp:Label ID="lblAutoMessage" runat="server" ForeColor="Red"></asp:Label>
    


    <asp:Login ID="LoginUser" RememberMeSet="true" runat="server" EnableViewState="false" RenderOuterTable="false" FailureText="Přihlášení se nezdařilo - pravděpodobně chybné heslo nebo jméno.">

        <LayoutTemplate>

            <span style="color: red; padding: 10px;">

                <asp:Literal ID="FailureText" runat="server"></asp:Literal>

            </span>
            <asp:ValidationSummary ID="LoginUserValidationSummary" runat="server" CssClass="failureNotification"
                ValidationGroup="LoginUserValidationGroup" />



            <div class="content-box2">
                <div class="title">
                    <img src="../Images/unlock.png" alt="Přihlásit se"  />
                    Přihlásit se | Log In
                    
                </div>
                <div class="content">
                    <table cellpadding="10" id="responsive">
                        <tr>
                            <td id="rlbl">
                                <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" Text="Uživatelské jméno | Login:"></asp:Label>
                            </td>
                            <td>

                                <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                                    CssClass="failureNotification" ErrorMessage="Uživatelské jméno je povinné." ToolTip="Uživatelské jméno je povinné."
                                    ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td id="rlbl">
                                <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" Text="Heslo | Password:"></asp:Label>
                            </td>

                            <td>
                                <asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                    CssClass="failureNotification" ErrorMessage="Heslo je povinné. | Password is required." ToolTip="Heslo je povinné."
                                    ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>

                        <tr>
                            <td>
                                <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Přihlásit se" OnClientClick="loadresolution()" ValidationGroup="LoginUserValidationGroup" CssClass="cmd" Style="margin-left: 20px;" />
                            </td>
                            <td>


                                <asp:CheckBox ID="RememberMe" runat="server" Text="Zapamatovat si přihlášení" Checked="true" />
                            </td>

                        </tr>
                    </table>                    
                </div>
            </div>
            
        </LayoutTemplate>
    </asp:Login>

    <asp:Image ID="imgSplashScreen" runat="server" Visible="false" EnableViewState="false" />

    <div>
        <asp:Label ID="lblDomainAccount" runat="server"></asp:Label>
    </div>
    <div>
        <asp:Label ID="lblBrowserInfo" runat="server"></asp:Label>
    </div>
    <div>
        <asp:Label ID="lblAdInfo" runat="server" Style="color: Red;"></asp:Label>
    </div>

    <p>

        <asp:LinkButton ID="cmdPasswordRecovery" runat="server" Visible="false" Text="Obnova zapomenutého hesla" />
    </p>

    <asp:HiddenField ID="screenwidth" runat="server" />
    <asp:HiddenField ID="screenheight" runat="server" />
    <script type="text/javascript">
        testbrowser();
    </script>
</asp:Content>
