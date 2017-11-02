<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="contactpersons.ascx.vb" Inherits="UI.contactpersons" %>

<asp:Repeater ID="rpP30" runat="server">
    <ItemTemplate>
        <div style="padding: 5px;">
            <img src="Images/person.png" />
            <asp:HyperLink ID="linkPP1" runat="server" CssClass="pp1"></asp:HyperLink>

            
            <asp:HyperLink ID="linkPerson" runat="server" CssClass="value_link" Target="_top"></asp:HyperLink>

            <asp:HyperLink ID="j02Email" runat="server" CssClass="wake_link"></asp:HyperLink>

            <asp:Label ID="j02JobTitle" runat="server" Font-Italic="true"></asp:Label>
            <asp:Label ID="j02Mobile" runat="server"></asp:Label>
        </div>
    </ItemTemplate>
</asp:Repeater>


<asp:HiddenField ID="hidIsShowClueTip" Value="1" runat="server" />
