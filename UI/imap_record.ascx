<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="imap_record.ascx.vb" Inherits="UI.imap_record" %>
<div class="div6">
    <asp:Label ID="Sender" runat="server"></asp:Label>
</div>
<div class="div6">
    <asp:Label ID="Subject" runat="server" CssClass="valbold"></asp:Label>
</div>
<div class="div6">
    <asp:Label ID="Timestamp" runat="server" CssClass="timestamp"></asp:Label>
</div>
<div class="div6">
    <img src="Images/Files/msg_24.png" />
    <asp:HyperLink ID="cmdMSG" runat="server" Text="Otevřít v MS-OUTLOOK"></asp:HyperLink>
</div>
<div class="div6">
    <img src="Images/Files/eml_24.png" />
    <asp:HyperLink ID="cmdEML" runat="server" Text="EML formát zprávy"></asp:HyperLink>
</div>
<asp:Label ID="lblAttHeader" Text="Přílohy" runat="server" CssClass="valbold"></asp:Label>
<asp:Repeater ID="rpIMAPAttachments" runat="server">
    <ItemTemplate>
        <div>
            <asp:Image ID="img1" runat="server" />
            <asp:HyperLink ID="att1" runat="server"></asp:HyperLink>
        </div>
    </ItemTemplate>
</asp:Repeater>
<asp:HiddenField ID="hidGUID" runat="server" />
