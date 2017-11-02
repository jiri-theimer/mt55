<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="j03_mypage_setting.aspx.vb" Inherits="UI.j03_mypage_setting" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content-box2">
        <div class="title">
            Okruh gadgetů na osobní stránce
            <asp:Button ID="cmdAdd" runat="server" Text="Přidat gadget" CssClass="cmd" />
        </div>
        <div class="content">
            <table cellpadding="10">
               
                <asp:Repeater ID="rp1" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:Label ID="Index" runat="server"></asp:Label>
                                <asp:HiddenField ID="x57DockID" runat="server" />
                            </td>
                            <td>
                                <asp:DropDownList ID="x55ID" runat="server" DataValueField="pid" DataTextField="NameWithCode"></asp:DropDownList>
                            </td>
                          
                            <td>
                                <asp:ImageButton ID="del" runat="server" ImageUrl="Images/delete_row.png" ToolTip="Odstranit položku" CssClass="button-link" CommandName="delete" />
                                <asp:HiddenField ID="p85id" runat="server" />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
