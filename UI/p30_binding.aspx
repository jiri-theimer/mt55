<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p30_binding.aspx.vb" Inherits="UI.p30_binding" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="project" Src="~/project.ascx" %>
<%@ Register TagPrefix="uc" TagName="contact" Src="~/contact.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function j02_edit(j02id) {

            dialog_master("j02_record.aspx?iscontact=1&pid=" + j02id, true)

        }

        function hardrefresh(pid, flag) {
            var j02id = "";
            if (flag == "j02-save") {
                j02id = pid;
            }
            location.replace("p30_binding.aspx?masterprefix=<%=me.currentprefix%>&masterpid=<%=master.datapid%>&default_j02id=" + j02id)
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="panMasterRecord" runat="server" CssClass="content-box2" Style="padding-top: 10px;" Visible="false">
        <div class="title">
            Vybrat klienta nebo projekt, s kterým má být svázána kontaktní osoba
        </div>
        <div class="content">
            <span>Klient:</span>
            <uc:contact ID="p28id" runat="server" AutoPostBack="false" Width="400px" Flag="searchbox" />
            <div>
                <span>nebo</span>
            </div>
            <div>
                <span>Projekt:</span>
                <uc:project ID="p41id" runat="server" AutoPostBack="true" Width="400px" Flag="searchbox" />
            </div>
            <hr />
            <asp:Button ID="cmdContinue" runat="server" CssClass="cmd" Text="Pokračovat" />

        </div>
    </asp:Panel>
    <asp:Panel ID="panPersons" runat="server">
        <div class="div6">
            <table cellpadding="4">

                <tr style="vertical-align: top;">
                    <td>
                        <button type="button" onclick="j02_edit(0)">Založit úplně novou osobu</button>
                    </td>
                    <td>
                        <span>nebo vložit již zavedenou z adresáře lidí:</span>
                    </td>
                    <td>
                        <uc:person ID="j02ID" runat="server" flag="all2" Width="300px" AutoPostBack="false" />
                    </td>
                    <td>
                        <asp:DropDownList ID="p27ID" runat="server" DataTextField="p27Name" DataValueField="pid" Visible="false"></asp:DropDownList>
                    </td>
                    <td>
                        <asp:Button ID="cmdSave" runat="server" CssClass="cmd" Text="->Potvrdit vybranou osobu" />
                    </td>

                </tr>

            </table>
        </div>
        <div class="content-box2" style="padding-top: 10px;">
            <div class="title">
                <img alt="Osoba" src="Images/person.png" width="16px" height="16px" />
                <asp:Label ID="lblBoundHeader" runat="server" CssClass="framework_header_span" Text="Přiřazené kontaktní osoby"></asp:Label>

            </div>
            <asp:Panel ID="panP30" runat="server" CssClass="content">
                <table cellpadding="6">
                    <asp:Repeater ID="rpP30" runat="server">
                        <ItemTemplate>
                            <tr class="trHover">
                                <td>
                                    <asp:Label ID="p27Name" runat="server" CssClass="valbold"></asp:Label>
                                </td>
                                <td>
                                    <asp:HyperLink ID="clue_j02" runat="server" CssClass="reczoom" Text="i" title="Detail"></asp:HyperLink>
                                    <asp:Label ID="Person" runat="server" CssClass="valbold"></asp:Label>
                                </td>

                                <td>
                                    
                                    <asp:HyperLink ID="cmdJ02" runat="server" Text="Upravit/odstranit osobní profil"></asp:HyperLink>
                                    <asp:HiddenField ID="p30id" runat="server" />
                                </td>
                                <td>
                                    <asp:Image ID="imgDel" runat="server" ImageUrl="Images/delete.png" Style="margin-left: 20px;" />
                                    <asp:LinkButton ID="cmdDelete" runat="server" Text="Odstranit vazbu" CommandName="delete"></asp:LinkButton>
                                </td>
                                <td>
                                    <asp:Label ID="Message" runat="server" CssClass="val" Font-Italic="true"></asp:Label>
                                </td>
                            </tr>
                            
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </asp:Panel>
        </div>
        <table id="tabDefaultPerson" runat="server">
            <tr>
                <td>Výchozí kontaktní osoba pro vykazování úkonů:</td>
                <td>
                    <asp:DropDownList ID="j02ID_ContactPerson_DefaultInWorksheet" runat="server" DataValueField="j02ID" DataTextField="FullNameDesc" AutoPostBack="true"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>Kontaktní osoba na faktuře:</td>
                <td>
                    <asp:DropDownList ID="j02ID_ContactPerson_DefaultInInvoice" runat="server" DataValueField="j02ID" DataTextField="FullNameDesc" AutoPostBack="true"></asp:DropDownList>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:HiddenField ID="hidPrefix" runat="server" />

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
