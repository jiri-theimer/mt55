<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p35_record.aspx.vb" Inherits="UI.p35_record" %>
<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<table cellpadding="3" cellspacing="2">	    
<tr>
	<td><asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název jednotky:"></asp:Label></td>
	<td>
	<asp:TextBox ID="p35Name" Runat="server" style="width:400px;"></asp:TextBox>
	</td>
</tr>
<tr>
	<td><asp:Label ID="Label1" runat="server" CssClass="lblReq" Text="Kód:"></asp:Label></td>
	<td>
	<asp:TextBox ID="p35Code" Runat="server" style="width:50px;"></asp:TextBox>
	</td>
</tr>
</table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>

