<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="entityrole_assign_preview.ascx.vb" Inherits="UI.entityrole_assign_preview" %>

<asp:Repeater ID="rpX69" runat="server">
    <ItemTemplate>
        <div style="clear: both;">
            <div style="float: left; min-width: 170px; margin-bottom: 3px; vertical-align: baseline;">



                <asp:Label ID="_x67name" runat="server" CssClass="val"></asp:Label>
            </div>
            <div style="float: left; padding-left:15px;min-width: 210px; margin-bottom: 3px; vertical-align: baseline;">

                <asp:Image ID="img1" runat="server" ImageUrl="Images/projectrole_team.png"></asp:Image>

                <asp:Label ID="_subject" runat="server" CssClass="val"></asp:Label>
            </div>


        </div>
    </ItemTemplate>
</asp:Repeater>

<div style="clear:both;">
<asp:Label ID="noData" CssClass="infoNotification" runat="server" Text="Bez přiřazení rolí."></asp:Label>
</div>
<asp:HiddenField ID="hidX29ID" runat="server" />
<asp:HiddenField ID="hidInhaledDataPID" runat="server" />
