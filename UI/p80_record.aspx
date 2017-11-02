<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p80_record.aspx.vb" Inherits="UI.p80_record" %>
<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="5" cellspacing="2">
        <tr>
            <td>
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název pravidla:"></asp:Label></td>
            <td>
                <asp:TextBox ID="p80Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
      
       <tr>
            <td colspan="2">
                <asp:CheckBox ID="p80IsExpenseSeparate" runat="server" Text="Výdaje 1:1" Checked="false" />
                
            </td>
        </tr>
       <tr>
            <td colspan="2">
                <asp:CheckBox ID="p80IsFeeSeparate" runat="server" Text="Pevné odměny 1:1" Checked="false" />
                
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="p80IsTimeSeparate" runat="server" Text="Časové úkony 1:1" Checked="false" />
                
            </td>
        </tr>
      
    </table>

    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>


