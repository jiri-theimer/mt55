<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_p28_record.aspx.vb" Inherits="UI.clue_p28_record" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>
<%@ Register TagPrefix="uc" TagName="p28_address" Src="~/p28_address.ascx" %>
<%@ Register TagPrefix="uc" TagName="p28_medium" Src="~/p28_medium.ascx" %>
<%@ Register TagPrefix="uc" TagName="contactpersons" Src="~/contactpersons.ascx" %>
<%@ Register TagPrefix="uc" TagName="mytags" Src="~/mytags.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="max-height: 270px; overflow-x: auto; overflow-y: auto; width: 100%;">

        <div>
            <img src="Images/contact_32.png" />
            <asp:Label ID="ph1" runat="server" CssClass="clue_header_span"></asp:Label>
            <uc:mytags ID="tags1" ModeUi="2" Prefix="p28" runat="server" />
        </div>
        <div class="content-box2">
            <div class="title">
                Záznam klienta
            </div>
            <div class="content">
                <table cellpadding="6">
                    <tr id="trP29" runat="server">
                        <td>Typ klienta:</td>
                        <td>
                            <asp:Label ID="p29Name" runat="server" CssClass="valbold"></asp:Label></td>
                        <td>Vlastník:</td>
                        <td>
                            <asp:Label ID="Owner" runat="server" CssClass="valbold"></asp:Label></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td>IČ:</td>
                        <td>
                            <asp:Label ID="p28REGID" runat="server" CssClass="valbold"></asp:Label></td>
                        <td>DIČ:</td>
                        <td>
                            <asp:Label ID="p28VATID" runat="server" CssClass="valbold"></asp:Label></td>
                        <td>Nadřízený:
                            <asp:HyperLink ID="ParentContact" runat="server" Target="_top"></asp:HyperLink>
                        </td>
                    </tr>
                </table>
                
            </div>
        </div>
        <asp:Panel ID="panP30" runat="server" CssClass="content-box2">
            <div class="title">
                <img src="Images/person.png" style="margin-right: 10px;" />
                Kontaktní osoby klienta                        
            </div>
            <div class="content">
                <uc:contactpersons ID="persons1" runat="server" IsShowClueTip="false"></uc:contactpersons>
            </div>
        </asp:Panel>
        <div class="content-box2">
            <div class="title">
                <img src="Images/address.png" />
            </div>
            <div class="content">
                <uc:p28_address ID="address1" runat="server"></uc:p28_address>
            </div>
        </div>
        
        <asp:Panel ID="panMedium" runat="server" CssClass="content-box2">
            <div class="title">
                <img src="Images/email.png" />
                Kontaktní média
            </div>
            <div class="content">
                <uc:p28_medium ID="medium1" runat="server"></uc:p28_medium>
            </div>
        </asp:Panel>
        <asp:panel ID="panBilling" runat="server" cssclass="content-box2">
            <div class="title">
                <img src="Images/billing.png" />
                Fakturační nastavení
            </div>
            <div class="content">
                <table cellpadding="6">
                    <tr>
                        <td>Fakturační ceník:</td>
                        <td>
                            <asp:Label ID="p51Name_Billing" runat="server" CssClass="valbold"></asp:Label>

                        </td>
                        <td>Fakturační jazyk:</td>
                        <td>
                            <asp:Label ID="p87Name" runat="server" CssClass="valbold"></asp:Label>

                        </td>
                    </tr>
                    <tr>
                        <td>Výchozí typ faktury:</td>
                        <td>
                            <asp:Label ID="p92Name" runat="server" CssClass="valbold"></asp:Label>

                        </td>
                        <td>Počet dní splatnosti faktury:</td>
                        <td>
                            <asp:Label ID="p28InvoiceMaturityDays" runat="server" CssClass="valbold"></asp:Label>

                        </td>
                    </tr>
                </table>
            </div>
        </asp:panel>



    </div>
</asp:Content>
