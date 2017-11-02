<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="j60_record.aspx.vb" Inherits="UI.j60_record" %>
<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>



<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="10">
        <tr>

            <td style="width: 120px;">
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název menu:"></asp:Label></td>
            <td>
                <asp:TextBox ID="j60Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
            
        </tr>
       
    </table>
    <asp:CheckBox ID="chkClone" runat="server" Text="Zkopírovat strukturu menu položek" visible="false"/>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
