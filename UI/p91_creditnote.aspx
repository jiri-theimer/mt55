<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p91_creditnote.aspx.vb" Inherits="UI.p91_creditnote" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="10">
        <tr>
            
            <td>
                <asp:Label ID="lblP92ID" runat="server" CssClass="lbl" Text="Typ faktury (opravného dokladu):"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="p92ID" runat="server" DataTextField="p92Name" DataValueField="pid" IsFirstEmptyRow="true"></uc:datacombo>
            </td>


        </tr>
       
    </table>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
