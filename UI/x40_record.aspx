<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="x40_record.aspx.vb" Inherits="UI.x40_record" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="5" cellspacing="2">
        <tr valign="top">
            <td>
                <asp:Label ID="Label3" runat="server" CssClass="lbl" Text="Stav zprávy:"></asp:Label>

            </td>
            <td>
                <asp:Label ID="x40State" runat="server" CssClass="valbold"></asp:Label>


            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="cmdChangeState" Text="Změnit stav zprávy" runat="server" CssClass="cmd" />



            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="cmdChangeStateOnNeedConfirm" Text="Změnit stav zprávy na [Čeká na odeslání]" runat="server" CssClass="cmd" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <img src="Images/Files/eml_24.png" />
                <asp:LinkButton ID="cmdEML" runat="server" Text="EML formát zprávy"></asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <img src="Images/Files/msg_24.png" />                
                <asp:LinkButton ID="cmdMSG" runat="server" Text="Otevřít v MS-OUTLOOK"></asp:LinkButton>
            </td>
        </tr>
        
        <tr>
            <td>
                <asp:Label ID="Label2" runat="server" CssClass="lbl" Text="Čas:"></asp:Label>

            </td>
            <td>

                <asp:Label ID="DateInsert" runat="server" CssClass="valbold"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label1" runat="server" CssClass="lbl" Text="Od:"></asp:Label>

            </td>
            <td>

                <asp:Label ID="x40SenderName" runat="server" CssClass="valbold"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblx40Recipient" runat="server" CssClass="lbl" Text="Komu:"></asp:Label>

            </td>
            <td>

                <asp:Label ID="x40Recipient" runat="server" CssClass="valbold"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblCC" runat="server" CssClass="lbl" Text="V kopii (Cc):"></asp:Label>

            </td>
            <td>
                <asp:Label ID="x40CC" runat="server" CssClass="valbold"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblBCC" runat="server" CssClass="lbl" Text="Skrytá kopie (Bcc):"></asp:Label>

            </td>
            <td>
                <asp:Label ID="x40BCC" runat="server" CssClass="valbold"></asp:Label>
            </td>
        </tr>

        <tr valign="top">
            <td>
                <asp:Label ID="lblSubject" runat="server" CssClass="lbl" Text="Předmět zprávy:"></asp:Label>

            </td>
            <td>
                <asp:Label ID="x40Subject" runat="server" CssClass="valbold"></asp:Label>
            </td>
        </tr>
      
    </table>    
    
    <asp:Repeater ID="rpAtt" runat="server">
        <ItemTemplate>
            <div class="div6">
                <asp:Image ID="img1" runat="server" ImageUrl="Images/attachment.png" Style="vertical-align: bottom;" />
                <asp:HyperLink ID="linkAtt" runat="server"></asp:HyperLink>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <div>
        <asp:Label ID="Timestamp" runat="server" CssClass="timestamp"></asp:Label>
    </div>
    <div class="div6" style="background-color: #ffffcc;border:solid 1px silver;">
        <asp:Label ID="x40Body" runat="server"  CssClass="val"></asp:Label>
    </div>
    <div class="div6">
        <asp:Button ID="cmdDelete" Text="Odstranit zprávu" runat="server" CssClass="cmd" Visible="false" />
    </div>
   
    <div class="div6">
        <asp:Label ID="x40ErrorMessage" runat="server" Font-Bold="true" ForeColor="Red" Font-Italic="true"></asp:Label>
    </div>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
