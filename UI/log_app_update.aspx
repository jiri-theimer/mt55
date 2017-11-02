<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="log_app_update.aspx.vb" Inherits="UI.log_app_update" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="background-color: white;">
        <div style="float: left;">
            <img src="Images/information_32.png" />
            <asp:Label ID="lblHeader" runat="server" CssClass="framework_header_span" Style="font-size: 200%;" Text="Historie novinek a změn v systému"></asp:Label>
        </div>


        <div style="clear: both;"></div>
        <div style="padding-left:10px;">
        <asp:PlaceHolder ID="place1" runat="server"></asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
