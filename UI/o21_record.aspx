<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="o21_record.aspx.vb" Inherits="UI.o21_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>

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
                    <asp:ListItem Text="Klient" Value="328"></asp:ListItem>
                    <asp:ListItem Text="Osoba" Value="102"></asp:ListItem>
                    <asp:ListItem Text="Daňová faktura" Value="391"></asp:ListItem>
                    <asp:ListItem Text="Zálohová faktura" Value="390"></asp:ListItem>

                </asp:DropDownList>

            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div class="content-box2">
                    <div class="title">Událost | Milník | Poznámka</div>
                    <div class="content">
                        <asp:RadioButtonList ID="o21Flag" runat="server" CellPadding="5">
                            <asp:ListItem Text="<img src='Images/milestone.png'/>Pouze jedno datum (termín) [DO] - vhodné pro definování milníků, termínů, lhůt, výročí apod." Value="1" Selected="true"></asp:ListItem>
                            <asp:ListItem Text="<img src='Images/event.png'/>Časový úsek definovaný rozsahem [OD] - [DO] - vhodné pro definování událostí, rezervací apod." Value="2"></asp:ListItem>
                            <asp:ListItem Text="<img src='Images/notepad.png'/>Přesné datum se vůbec neuvádí - pouze slovní popis milníku" Value="3"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>

            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="o21Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td></td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblOrdinary" Text="Index pořadí:" runat="server" CssClass="lbl" AssociatedControlID="o21Ordinary"></asp:Label>

            </td>
            <td>

                <telerik:RadNumericTextBox ID="o21Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>

