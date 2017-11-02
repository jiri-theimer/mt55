<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="entityrole_assign_inline.ascx.vb" Inherits="UI.entityrole_assign_inline" %>
<asp:repeater ID="rpX69" runat="server">
    <ItemTemplate>
        <asp:PlaceHolder ID="place1" runat="server"></asp:PlaceHolder>
        <asp:Label ID="_x67name" runat="server" CssClass="valbold"></asp:Label>
        
        <asp:Label ID="_subject" runat="server"></asp:Label>
        <asp:hyperlink ID="role_clue" runat="server" CssClass="reczoom" Text="i" title="Detail projektové role"></asp:hyperlink>
       <asp:PlaceHolder ID="place2" runat="server"></asp:PlaceHolder>
    </ItemTemplate>
</asp:repeater>
<asp:Label ID="noData" runat="server" CssClass="infoNotificationRed"></asp:Label>
<asp:HiddenField ID="hidX29ID" runat="server" />
<asp:HiddenField ID="hidInhaledDataPID" runat="server" />
<asp:HiddenField ID="hidIsShowClueTip" Value="1" runat="server" />
<asp:HiddenField ID="hidIsRenderAsTable" Value="0" runat="server" />