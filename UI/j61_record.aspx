<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="j61_record.aspx.vb" Inherits="UI.j61_record" %>

<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign" Src="~/entityrole_assign.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" ShowBaseLine="true" Skin="Default">
        <Tabs>
            <telerik:RadTab Text="Vlastnosti" Selected="true" Value="core"></telerik:RadTab>
            <telerik:RadTab Text="Ostatní" Value="other"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>


    <telerik:RadMultiPage ID="RadMultiPage1" runat="server">
        <telerik:RadPageView ID="core" runat="server" Selected="true">

            <table cellpadding="6" cellspacing="2">
                <tr>
                    <td>
                        <asp:Label ID="lblX29ID" Text="Entita:" runat="server" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="x29ID" runat="server" DataTextField="x29Name" DataValueField="x29ID" AutoPostBack="true">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="Osobní" Value="102"></asp:ListItem>
                            <asp:ListItem Text="Faktura" Value="391"></asp:ListItem>
                            <asp:ListItem Text="Klient" Value="328"></asp:ListItem>
                            <asp:ListItem Text="Projekt" Value="141"></asp:ListItem>
                            <asp:ListItem Text="Úkol" Value="356"></asp:ListItem>
                            <asp:ListItem Text="Dokument" Value="223"></asp:ListItem>
                        </asp:DropDownList>

                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název šablony:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="j61Name" runat="server" Style="width: 400px;"></asp:TextBox>
                        <asp:Label ID="lblOrdinary" Text="Index pořadí:" runat="server" CssClass="lbl" AssociatedControlID="j61Ordinary"></asp:Label>
                        <telerik:RadNumericTextBox ID="j61Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>

                    </td>
                </tr>

            </table>

            <asp:Panel ID="panPlainText" runat="server" CssClass="content-box2">
                <div class="title">
                    Text šablony
                </div>
                <div class="content">
                    <asp:TextBox ID="j61PlainTextBody" runat="server" TextMode="MultiLine" Style="width: 99%; height: 250px;"></asp:TextBox>
                </div>
            </asp:Panel>
            <table cellpadding="6">
                <tr>
                    <td>
                        Předmět poštovní zprávy:
                    </td>
                    <td>
                        <asp:TextBox ID="j61MailSubject" runat="server" Style="width: 500px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Příjemci zprávy:
                    </td>
                    <td>
                        <asp:TextBox ID="j61MailTO" runat="server" Style="width: 500px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        V kopii (CC):
                    </td>
                    <td>
                        <asp:TextBox ID="j61MailCC" runat="server" Style="width: 500px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Ve skryté kopii (BCC):
                    </td>
                    <td>
                        <asp:TextBox ID="j61MailBCC" runat="server" Style="width: 500px;"></asp:TextBox>
                    </td>
                </tr>
            </table>
              


            


            
        </telerik:RadPageView>
        <telerik:RadPageView ID="other" runat="server">
            <div class="div6">
                <asp:Label ID="lblOwner" runat="server" Text="Vlastník záznamu:" CssClass="lblReq"></asp:Label>
                <uc:person ID="j02ID_Owner" runat="server" Width="300px" Flag="all" />
            </div>

            <div class="content-box2">
                <div class="title">
                    <img src="Images/projectrole.png" width="16px" height="16px" />
                    <asp:Label ID="ph1" runat="server" Text="Další oprávnění k šabloně"></asp:Label>
                    <asp:Button ID="cmdAddX69" runat="server" CssClass="cmd" Text="Přidat" />
                </div>
                <div class="content">
                    <uc:entityrole_assign ID="roles1" runat="server" EntityX29ID="j61TextTemplate"></uc:entityrole_assign>

                </div>
            </div>
        </telerik:RadPageView>
    </telerik:RadMultiPage>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
