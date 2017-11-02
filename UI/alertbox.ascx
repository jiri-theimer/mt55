<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="alertbox.ascx.vb" Inherits="UI.alertbox" %>
<div class="content-box1">
    <div class="title">
        <img src="Images/warning.png" />
        <span style="padding-left:10px;font-weight:bold;">Upozornění</span>
    </div>
    <div class="content" style="background-color:#FFF0F5;">
        
        <asp:Label ID="lblContent" runat="server"></asp:Label>
    </div>
</div>