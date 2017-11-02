<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p91_pay_aboimport.aspx.vb" Inherits="UI.p91_pay_aboimport" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <div class="content-box2">
        <div class="title">Nahrát ABO soubor (přípona GPC)</div>
        <div class="content">
             <telerik:RadUpload ID="upload1" runat="server" InputSize="35" InitialFileInputsCount="1" AllowedFileExtensions="gpc" MaxFileInputsCount="1" MaxFileSize="500000" ControlObjectsVisibility="None">
                 <Localization Select="Procházet" />
            </telerik:RadUpload>
        </div>
    </div>

    <table cellpadding="5" cellspacing="3">
    <asp:Repeater ID="rp1" runat="server">
        <ItemTemplate>
            <tr class="trHover">
                <td>
                    <asp:Image ID="img1" runat="server" />
                </td>
                <td>
                    <asp:Label ID="CisloProtiUctu" runat="server"></asp:Label>
                </td>
                <td align="right">
                    <asp:Label ID="Castka" runat="server" CssClass="valboldred"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="VSymbol" runat="server" CssClass="valboldblue"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="DoplnujiciUdaj" runat="server" Font-Italic="true"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="Vysledek" runat="server" Font-Size="Smaller"></asp:Label>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    </table>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
