<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="import_object_item.ascx.vb" Inherits="UI.import_object_item" %>
<asp:Repeater ID="rp1" runat="server">
    <ItemTemplate>
        <div style="float:left;padding:6px;">
            
            
            <asp:HyperLink ID="link1" runat="server"></asp:HyperLink>
            <asp:Label ID="FileSize" runat="server"></asp:Label>
            <asp:HiddenField ID="p85id" runat="server" />
        </div>
    </ItemTemplate>    
</asp:Repeater>
<div style="clear:both;"></div>
<asp:HiddenField ID="hidGUID" runat="server" />
<asp:HiddenField ID="hidPrefix" runat="server" />