<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="pokus.aspx.vb" Inherits="UI.pokus" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
  

    <script type="text/javascript">
       
       
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    

    <asp:Button ID="cmdPokus" runat="server" Text="pokus" />
    <hr />
    <asp:TextBox ID="txt1" runat="server" TextMode="MultiLine" Width="600px" Height="100px"></asp:TextBox>
</asp:Content>



