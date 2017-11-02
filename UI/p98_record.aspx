<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p98_record.aspx.vb" Inherits="UI.p98_record" %>

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
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název pravidla:"></asp:Label></td>
            <td>
                <asp:TextBox ID="p98Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="p98IsDefault" runat="server" Text="Výchozí pravidlo pro všechny faktury" />
            </td>
        </tr>
    </table>


    <div class="content-box2">
        <div class="title">
            <asp:Button ID="cmdAddRow" runat="server" CssClass="cmd" Text="Přidat řádek" />
        </div>
        <div class="content">
            <table cellpadding="5" cellspacing="2">
        <asp:Repeater ID="rp1" runat="server">
            <ItemTemplate>
                <tr valign="top">

                    <td>
                        <asp:Label ID="lblJ27ID" Text="Měna faktury:" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="j27ID" runat="server" DataTextField="j27Code" DataValueField="pid"></asp:DropDownList>
                    </td>
                    <td>
                        <asp:DropDownList ID="p97AmountFlag" runat="server">
                            <asp:ListItem Text="Částka bez DPH" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Částka vč. DPH" Value="3"></asp:ListItem>
                            <asp:ListItem Text="Částka DPH" Value="1"></asp:ListItem>
                        </asp:DropDownList>
                    </td>

                    <td>
                        <asp:Label ID="lblp97Scale" Text="Na kolik des.míst zaokrouhlovat:" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="p97Scale" runat="server">
                            <asp:ListItem Text="0" Value="0"></asp:ListItem>
                            <asp:ListItem Text="1" Value="1"></asp:ListItem>
                            <asp:ListItem Text="2" Value="2"></asp:ListItem>
                        </asp:DropDownList>
                    </td>


                    <td>
                        <asp:ImageButton ID="del" runat="server" ImageUrl="Images/delete_row.png" ToolTip="Odstranit položku" CssClass="button-link" />

                    </td>
                </tr>

            </ItemTemplate>

        </asp:Repeater>

    </table>
        </div>
    </div>
    



</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
