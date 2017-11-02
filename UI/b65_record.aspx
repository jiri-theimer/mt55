<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="b65_record.aspx.vb" Inherits="UI.b65_record" %>
<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datacombo" src="~/datacombo.ascx"%>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<table cellpadding="3" cellspacing="2">		
    <tr>
            <td>
                <asp:Label ID="lblB01ID" runat="server" CssClass="lbl" Text="Workflow šablona:"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="b01ID" runat="server" DataTextField="b01Name" DataValueField="pid" AutoPostBack="true" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>
            </td>
        </tr>
	<tr>
		<td class="frif"><asp:Label ID="lblB65Name" Text="Název:" runat="server" cssclass="lblReq" AssociatedControlID="b65name"></asp:Label></td>
		<td>
		<asp:TextBox ID="b65name" Runat="server" style="width:300px;"></asp:TextBox>
		</td>
            
	</tr>  
   
    <tr>
		<td class="frif"><asp:Label ID="lblb65MessageSubject" Text="Předmět zprávy:" runat="server" cssclass="lblReq" AssociatedControlID="b65MessageSubject"></asp:Label></td>
		<td>
		<asp:TextBox ID="b65MessageSubject" Runat="server" style="width:600px;"></asp:TextBox>
		</td>
            
	</tr>  
   
</table>
<div style="padding:6px;">
<div>
<asp:Label ID="lblb65MessageBody" Text="Tělo zprávy:" runat="server" cssclass="lbl" AssociatedControlID="b65MessageBody"></asp:Label>
</div>
<asp:TextBox ID="b65MessageBody" Runat="server" TextMode="MultiLine" style="width:97%;height:300px;"></asp:TextBox>
</div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>


