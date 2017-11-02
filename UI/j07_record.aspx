<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="j07_record.aspx.vb" Inherits="UI.j07_record" %>
<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" src="~/pageheader.ascx"%>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<table cellpadding="3" cellspacing="2">	    
<tr>
	<td><asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název pozice:"></asp:Label></td>
	<td>
	<asp:TextBox ID="j07Name" Runat="server" style="width:400px;"></asp:TextBox>
	</td>
</tr>
<tr>
	<td><asp:Label ID="lblOrdinary" Text="Index pořadí:" runat="server" cssclass="lbl" AssociatedControlID="j07Ordinary"></asp:Label></td>
	<td>

	<telerik:RadNumericTextBox ID="j07Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
	</td>
</tr>
</table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
