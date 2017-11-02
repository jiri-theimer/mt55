<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="x67_record.aspx.vb" Inherits="UI.x67_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="3" cellspacing="2">
        <tr>
            <td>
                <asp:Label ID="lblX29ID" Text="Entita:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="x29ID" runat="server" DataTextField="x29Name" DataValueField="x29ID" Enabled="False">
                    <asp:ListItem Text="Projekt" Value="141"></asp:ListItem>
                    <asp:ListItem Text="Úkol" Value="356"></asp:ListItem>
                    <asp:ListItem Text="Klient" Value="328"></asp:ListItem>
                    <asp:ListItem Text="Faktura" Value="391"></asp:ListItem>
                    <asp:ListItem Text="Dokument" Value="223"></asp:ListItem>
                    <asp:ListItem Text="Zálohová faktura" Value="390"></asp:ListItem>
                    <asp:ListItem Text="Tisková sestava" Value="931"></asp:ListItem>
                </asp:DropDownList>

            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblName" Text="Název role:" runat="server" AssociatedControlID="x67Name" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="x67Name" runat="server" Style="width: 400px;"></asp:TextBox>
                <asp:Label ID="lblOrdinary" Text="Index pořadí:" runat="server" CssClass="lbl" AssociatedControlID="x67Ordinary" Style="padding-left: 20px;"></asp:Label>
                <telerik:RadNumericTextBox ID="x67Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>

            </td>
        </tr>

    </table>

    <asp:Panel ID="panO28" runat="server" Visible="false" CssClass="content-box2">
        <div class="title">
            <asp:Label ID="ph2" runat="server" Text="Pracovní (worksheet) náplň projektové role v projektu" />
        </div>
        <div class="content">
            <table cellpadding="5" cellspacing="2">
                <tr>
                    <th>Sešit</th>
                    <th>Zapisování worksheet úkonů v projektu</th>
                    <th>Manažerský dohled na projektový worksheet</th>
                </tr>
                <asp:Repeater ID="rp1" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:Label ID="_p34name" runat="server"></asp:Label>
                                <asp:HiddenField ID="_p34id" runat="server" />
                            </td>
                            <td>
                                <asp:DropDownList ID="_o28entryflag" runat="server" Style="width: 300px;">
                                    <asp:ListItem Text="Nemá oprávnění zapisovat úkony do projektu" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Má oprávnění zapisovat úkony do projektu" Value="1" Selected="true"></asp:ListItem>
                                    <asp:ListItem Text="Za osobu zapisuje úkony jeho nadřízený" Value="4"></asp:ListItem>
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
        </div>
    </asp:Panel>

    <div class="content-box2">
        <div class="title">
            <asp:Label ID="ph1" runat="server" Text="Oprávnění role" />
        </div>
        <div class="content">
            <asp:CheckBoxList ID="x53ids" runat="server" DataValueField="pid" DataTextField="x53Name" RepeatColumns="2" CellPadding="8" CellSpacing="2"></asp:CheckBoxList>
        </div>
    </div>



</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>

