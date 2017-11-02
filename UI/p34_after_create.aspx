<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p34_after_create.aspx.vb" Inherits="UI.p34_after_create" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="5" cellspacing="2">
        <tr>
            <td>
                <asp:Label ID="lbl1" CssClass="lbl" runat="server" Text="Nový sešit:"></asp:Label>
            </td>
            <td>
                <asp:Label ID="p34Name" CssClass="valbold" runat="server"></asp:Label>
            </td>

            <td>
                <asp:Label ID="lbl2" CssClass="lbl" runat="server" Text="Formát vstupních dat:"></asp:Label>
            </td>
            <td>
                <asp:Label ID="p33Name" CssClass="valbold" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
    <div class="div6">
        <p class="infoNotification">
            <img src="Images/finish_32.png" />
            Do nového worksheet sešitu systém automaticky nenastavuje oprávění k zapisování úkonů!
        </p>
        <p class="infoNotification">
            Je třeba, aby administrátor zpřístupnil sešit v nastavení odpovídajících typů projektů. Navíc je třeba udělit oprávnění odpovídajícím projektovým rolím.
        </p>
        <p class="infoNotification">
            Na této stránce můžete rychle nastavit oprávnění k zapisování úkonů do sešitu. Nastavení můžete později upravit přes administraci systému.
        </p>
    </div>
    <uc:pageheader ID="ph1" runat="server" Text="Zaškrtněte typy projektů, v kterých má být povoleno zapisovat úkony." />
    <asp:CheckBoxList ID="p42ids" runat="server" DataValueField="pid" DataTextField="p42Name" RepeatColumns="3" CellPadding="8" CellSpacing="2"></asp:CheckBoxList>

    <uc:pageheader ID="ph2" runat="server" Text="Definujte oprávnění k sešitu pro projektové role." />

    <table cellpadding="5" cellspacing="2">
        <tr>
            <th>Projektová role</th>
            <th>Zapisování worksheet úkonů v projektu</th>
            <th>Manažerský dohled na projektový worksheet</th>
        </tr>
        <asp:Repeater ID="rp1" runat="server">
            <ItemTemplate>
                <tr>
                    <td>
                        <asp:Label ID="_x67Name" runat="server"></asp:Label>
                        <asp:HiddenField ID="_x67id" runat="server" />
                    </td>
                    <td>
                        <asp:DropDownList ID="_o28entryflag" runat="server" Style="width: 300px;">
                            <asp:ListItem Text="Nemá oprávnění zapisovat úkony do projektu" Value="0"></asp:ListItem>
                            <asp:ListItem Text="Má oprávnění zapisovat úkony do projektu" Value="1" Selected="true"></asp:ListItem>

                        </asp:DropDownList>
                    </td>
                    <td>

                        <asp:DropDownList ID="_o28permflag" runat="server">
                            <asp:ListItem Text="Pouze případný vlastní worksheet" Value="0" Selected="true"></asp:ListItem>
                            <asp:ListItem Text="Číst vše v rámci projektu" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Číst a upravovat vše v rámci projektu" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Číst a schvalovat vše v rámci projektu" Value="3"></asp:ListItem>
                            <asp:ListItem Text="Číst, upravovat a schvalovat vše v rámci projektu" Value="4"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
