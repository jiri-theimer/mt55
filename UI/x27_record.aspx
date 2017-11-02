<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="x27_record.aspx.vb" Inherits="UI.x27_record" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="3" cellspacing="2">
       
        <tr>
            <td>
                <asp:Label ID="lblName" Text="Název skupiny:" runat="server" AssociatedControlID="x27Name" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="x27Name" runat="server" Style="width: 400px;"></asp:TextBox>
                
                
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblOrdinary" Text="Index pořadí:" runat="server" CssClass="lbl" AssociatedControlID="x27Ordinary" style="padding-left:20px;"></asp:Label>
            </td>
            <td>
                <telerik:RadNumericTextBox ID="x27Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>

            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
