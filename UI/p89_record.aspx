<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p89_record.aspx.vb" Inherits="UI.p89_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="6" cellspacing="2">
        <tr>
            <td style="width:180px;">
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název typu zálohy:"></asp:Label></td>
            <td>
                <asp:TextBox ID="p89Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblX38ID" Text="Číselná řada dokladů:" runat="server" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="x38ID" runat="server" DataTextField="x38Name" DataValueField="pid" IsFirstEmptyRow="true" Width="250px"></uc:datacombo>
                <asp:Label ID="Label1" Text="Číselná řada DRAFT dokladů:" runat="server" CssClass="lbl"></asp:Label>
                <uc:datacombo ID="x38ID_Draft" runat="server" DataTextField="x38Name" DataValueField="pid" IsFirstEmptyRow="true" Width="200px"></uc:datacombo>

            </td>
        </tr>
         <tr>
            <td>
                <asp:Label ID="Label2" Text="Číselná řada DPP:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="x38ID_Payment" runat="server" DataTextField="x38Name" DataValueField="pid" IsFirstEmptyRow="true" Width="250px"></uc:datacombo>
               (Doklad přijaté platby zálohy)
            </td>
        </tr>
       
        <tr valign="top">
            <td>
                <asp:Label ID="lblP93ID" Text="Hlavička vystavovatele zálohy:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="p93ID" runat="server" AutoPostBack="false" DataTextField="p93Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>
                <span class="infoInForm">Hlavičku vystavovatele lze později měnit i ve faktuře.</span>
            </td>
        </tr>
        <tr valign="top">
            <td>
                <asp:Label ID="lblx31ID" Text="Výchozí sestava zálohy:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="x31ID" runat="server" AutoPostBack="false" DataTextField="NameWithCode" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>
                
            </td>
        </tr>
     <tr valign="top">
            <td>
                <asp:Label ID="Label3" Text="Sestava DPP:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="x31ID_Payment" runat="server" AutoPostBack="false" DataTextField="NameWithCode" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>
                (Sestava pro doklad přijaté platby zálohy)
            </td>
        </tr>
     
    </table>


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>



