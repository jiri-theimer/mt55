<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="entityrole_assign.ascx.vb" Inherits="UI.entityrole_assign" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>


<asp:Repeater ID="rpX69" runat="server">
    <ItemTemplate>
    <div style="clear: both;">
        <div style="float: left; min-width: 170px;margin-bottom:10px;vertical-align:baseline;">


            <asp:DropDownList ID="x67id" runat="server" DataTextField="x67Name" DataValueField="pid" Style="width: 160px;"></asp:DropDownList>

        </div>
        <div style="float: left;margin-bottom:10px;  min-width: 210px;">

            <asp:Image ID="img1" runat="server" ImageUrl="Images/projectrole_team.png"></asp:Image>

            <uc:person ID="j02id" runat="server" Width="200px" Flag="all" />
            <asp:DropDownList ID="j11id" runat="server" DataTextField="j11Name" DataValueField="pid" Style="width: 200px;" Font-Bold="true"></asp:DropDownList>

        </div>
        <div style="float: left;margin-bottom:10px;">
            <asp:RadioButtonList ID="opgWho" RepeatDirection="Horizontal" AutoPostBack="true" runat="server" OnSelectedIndexChanged="who_changed" Font-Size="90%" ForeColor="gray">
                <asp:ListItem Text="Osoba (jednotlivec)" Value="j02" Selected="true"></asp:ListItem>
                <asp:ListItem Text="Tým osob" Value="j11"></asp:ListItem>
                <asp:ListItem Text="Všichni" Value="all"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div style="float: left;margin-bottom:10px;padding-left:10px;">
            <asp:ImageButton ID="del" runat="server" ImageUrl="Images/delete_row.png" ToolTip="Odstranit položku" CssClass="button-link" />
            <asp:HiddenField ID="p85id" runat="server" />
        </div>
    </div>
    </ItemTemplate>
</asp:Repeater>

<div style="">
    <asp:Label ID="lblMessageNoData" CssClass="infoNotificationRed" runat="server"></asp:Label>
</div>
<asp:HiddenField ID="hidX29ID" runat="server" />
<asp:HiddenField ID="hidGUID" runat="server" />
