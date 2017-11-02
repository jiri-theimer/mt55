<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="f01folder.ascx.vb" Inherits="UI.f01folder" %>
<asp:Repeater ID="rpFolders" runat="server">
    <ItemTemplate>
        <div class="div6" style="border-top:dashed 1px silver;">
            <img src="Images/folder_32.png" title="Projektová složka" />
            <asp:HyperLink ID="link1" runat="server"></asp:HyperLink>
        </div>
    </ItemTemplate>   
</asp:Repeater>
<asp:HiddenField ID="hidPrefix" runat="server" />