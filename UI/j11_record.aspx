<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="j11_record.aspx.vb" Inherits="UI.j11_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/PageHeader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" ShowBaseLine="true">
        <Tabs>
            <telerik:RadTab Text="Vlastnosti" Selected="true"></telerik:RadTab>
            <telerik:RadTab Text="Další nastavení"></telerik:RadTab>

        </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadMultiPage ID="RadMultiPage1" runat="server">
        <telerik:RadPageView ID="page1" runat="server" Selected="true">
            <div class="content-box2">
                <div class="title">
                    Tým osob
                </div>
                <div class="content">
                    <table cellpadding="6">
                        <tr>
                            <td>
                                <asp:Label ID="lblJ11Name" Text="Název týmu:" runat="server" CssClass="lblReq" AssociatedControlID="j11name"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="j11name" runat="server" Style="width: 300px;"></asp:TextBox>
                            </td>
                        </tr>

                    </table>

                </div>

            </div>

            <asp:Panel ID="panMembers" runat="server">
                <div style="padding: 6px;">
                    <asp:Label ID="lblAdd" runat="server" Text="Vybrat osobu:" CssClass="lbl"></asp:Label>
                    <uc:person ID="j02id_search" runat="server" Width="400px" />

                    <asp:Button ID="cmdAdd" runat="server" Text="Přidat do týmu" CssClass="cmd" />
                    <span style="padding-left: 40px;"></span>
                    <asp:Button ID="cmdRemoveSelected" runat="server" Text="Odebrat vybrané členy" CssClass="cmd" />
                </div>
                <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid" AllowMultiSelect="true"></uc:datagrid>

                <div class="div6">
                    <asp:Label ID="Label1" runat="server" Text="Vybrat pozici:" CssClass="lbl"></asp:Label>
                    <asp:DropDownList ID="j07ID" runat="server" DataTextField="j07Name" DataValueField="pid"></asp:DropDownList>
                    <asp:Button ID="cmdAddJ07" runat="server" Text="Přidat do týmu" CssClass="cmd" />
                </div>
            </asp:Panel>

            <asp:CheckBox ID="j11IsAllPersons" runat="server" Visible="false" />
        </telerik:RadPageView>
        <telerik:RadPageView ID="page2" runat="server">
            <table cellpadding="6">
                <tr>
                    <td>
                        <span class="lbl">Skupinová e-mail adresa:</span>
                    </td>
                    <td>
                        <asp:TextBox ID="j11Email" runat="server" Style="width: 200px;"></asp:TextBox>
                        <span class="infoInForm">E-mail adresa, na kterou se mají týmu odesílat poštovní zprávy.</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <span class="lbl">Adresa pro IMAP robota:</span>
                    </td>
                    <td>
                        <asp:TextBox ID="j11RobotAddress" runat="server" Style="width: 200px;"></asp:TextBox>
                        <span class="infoInForm">E-mail adresa, podle které IMAP robot pozná, že nový úkol/dokument zakládaný automaticky z poštovní zprávy má vztah k týmu.</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <span class="lbl">Název skupiny v doméně:</span>
                    </td>
                    <td>
                        <asp:TextBox ID="j11DomainAccount" runat="server" Style="width: 200px;"></asp:TextBox>
                        <span class="infoInForm">Lze využít pro definování oprávnění file-system složek v modulu [DOKUMENTY].</span>
                    </td>
                </tr>
            </table>
        </telerik:RadPageView>
    </telerik:RadMultiPage>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
