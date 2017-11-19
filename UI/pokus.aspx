<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="pokus.aspx.vb" Inherits="UI.pokus" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="contact" Src="~/contact.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
  


    <script type="text/javascript">
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table>
        <tr>
            <td>
                Pozice nově:
            </td>
            <td>
                <uc:datacombo ID="j07ID" runat="server" AutoPostBack="false" RemoteListPrefix="j07" Width="400px"></uc:datacombo>
            </td>
        </tr>
        <tr>
            <td>
                Aplikační role:
            </td>
            <td>
                <uc:datacombo ID="j04ID" runat="server" AutoPostBack="false" RemoteListPrefix="j04" Width="400px"></uc:datacombo>
            </td>
        </tr>
        <tr>
            <td>Klient:</td>
            <td>
                <uc:contact ID="p28ID" runat="server" Width="400px" Flag="client" />
            </td>
        </tr>
    </table>
    

    <asp:Button ID="cmdPokus" runat="server" Text="FlexiBee" />

    <asp:Button ID="cmdPostback" runat="server" Text="postback" />

</asp:Content>



