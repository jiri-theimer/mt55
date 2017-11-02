<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="x35_record.aspx.vb" Inherits="UI.x35_record" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datalabel" src="~/datalabel.ascx"%>
<%@ Register TagPrefix="uc" TagName="pageheader" src="~/pageheader.ascx"%>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
<table cellpadding="5" cellspacing="2">
		        
		<tr>
			<td><uc:datalabel runat="server" ID="lblName" Text="Název parametru:" glx="83"></uc:datalabel></td>
			<td>
			<asp:Label ID="x35Description" runat="server" CssClass="valbold"></asp:Label>
            
			</td>
          
		</tr>
        <tr>
            <td><uc:datalabel runat="server" ID="lblValue" Text="Hodnota parametru:"></uc:datalabel></td>
            <td>
            <asp:TextBox ID="x35Value" Runat="server" style="width:300px;"></asp:TextBox>
            <uc:datacombo ID="x35Value_Combo" runat="server" Width="400px" DataValueField="pid" IsFirstEmptyRow="true" />
            </td>
        </tr>
</table>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
