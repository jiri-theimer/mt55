<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="person_or_team.ascx.vb" Inherits="UI.person_or_team" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>

<asp:Repeater ID="rp1" runat="server">
    <ItemTemplate>
        <div>
            <span>Jméno:</span>
            <uc:person id="j02id" runat="server" width="200px" flag="all" />
            <span>nebo tým:</span>
            <asp:DropDownList ID="j11id" runat="server" DataTextField="NameWithEmail" DataValueField="pid" Style="width: 200px;"></asp:DropDownList>
            <asp:ImageButton ID="del" runat="server" ImageUrl="Images/delete_row.png" ToolTip="Odstranit položku" CssClass="button-link" />
            <asp:HiddenField ID="p85id" runat="server" />
        </div>
    </ItemTemplate>
</asp:Repeater>

<asp:HiddenField ID="hidGUID" runat="server" />
<asp:HiddenField ID="hidInlineContent" runat="server" />
<asp:HiddenField ID="hidIsMobile" runat="server" Value="0" />