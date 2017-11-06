<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="o25_record.aspx.vb" Inherits="UI.o25_record" %>
<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="3" cellspacing="2">
        <tr>
            <td>
                <asp:Label ID="Label4" Text="Druh IS:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="o25AppFlag" runat="server">
                    <asp:ListItem Text="Google kalendář" Value="1"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
     
        <tr>
            <td>
                <asp:Label ID="Label1" Text="Název:" runat="server" CssClass="lblReq"></asp:Label></td>
            <td>
                <asp:TextBox ID="o25Name" runat="server" Style="width: 400px;"></asp:TextBox>
                
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblCode" Text="ID kalendáře:" runat="server" CssClass="lblReq"></asp:Label></td>
            <td>
                <asp:TextBox ID="o25Code" runat="server" Style="width: 700px;"></asp:TextBox>

            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label2" Text="URL pro prohlížeče:" runat="server" CssClass="lbl"></asp:Label></td>
            <td>
                <asp:TextBox ID="o25Url" runat="server" Style="width: 700px;"></asp:TextBox>

            </td>
        </tr>


       

        
        
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="o25IsMainMenu" runat="server" Text="Zveřejnit odkaz na kalendář v hlavním aplikačním menu" Checked="true" />
            </td>
        </tr>
        
    </table>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
