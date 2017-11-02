<%@ Page Title="Home Page" Language="vb" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="UI._Default" %>

<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:HiddenField ID="hidURL" runat="server" />

    <script type="text/javascript">
        $(document).ready(function () {

            handleSAW();


            var url = document.getElementById("<%=hidURL.ClientID%>").value;
            location.replace(url)
        });
    </script>
</asp:Content>
