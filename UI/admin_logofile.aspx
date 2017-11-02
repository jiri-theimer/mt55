<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="admin_logofile.aspx.vb" Inherits="UI.admin_logofile" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <div class="content-box2">
        <div class="title">Nahrát grafické logo ve formátu PNG</div>
        <div class="content">
             <telerik:RadUpload ID="upload1" runat="server" InputSize="35" InitialFileInputsCount="1" AllowedFileExtensions="png" MaxFileInputsCount="1" MaxFileSize="50000" ControlObjectsVisibility="None">
                 <Localization Select="Procházet" />
            </telerik:RadUpload>
        </div>
    </div>
   
    <div class="content-box2">
        <div class="title">Náhled</div>
        <div class="content">
            <telerik:RadBinaryImage ID="imgLogoPreview" runat="server" ResizeMode="None" AlternateText="Chybí soubor grafického loga!" SavedImageName="marktime_customer_logo.png" />
        </div>
    </div>
    
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
