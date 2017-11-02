<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="persons.ascx.vb" Inherits="UI.persons" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<div class="content-box3" style="padding-bottom: 10px;">
    <div class="title"><img src="Images/persons.png" /><span style="margin-left:2px;">Výběr lidí, týmů a pozic</span></div>
    <div class="content">
        <asp:Label ID="lblMessage" runat="server" ForeColor="red"></asp:Label>
        <div style="clear: both;">
            <div>
                <asp:DropDownList ID="cbxPersonsRole" runat="server" AutoPostBack="true" Style="width: 100%;" Visible="false" DataValueField="pid" DataTextField="ItemText">
                </asp:DropDownList>
            </div>
            <asp:RadioButtonList ID="opgScope" runat="server" AutoPostBack="true">
                <asp:ListItem Text="Všechny osoby" Value="1" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Jedna nebo více osob" Value="2"></asp:ListItem>
                <asp:ListItem Text="Pouze já" Value="4"></asp:ListItem>
                <asp:ListItem Text="Pojmenovaný filtr osob" Value="3"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <asp:Panel ID="panQuery" runat="server">
            <asp:HyperLink ID="clue_query" runat="server" CssClass="reczoom" ToolTip="Detail filtru" Text="i"></asp:HyperLink>
            <asp:DropDownList ID="j70ID" runat="server" AutoPostBack="true" DataTextField="NameWithMark" DataValueField="pid" Style="width: 170px;"></asp:DropDownList>
        </asp:Panel>
        <asp:Panel ID="panManual" runat="server">
            <asp:Repeater ID="rpJ11" runat="server">
                <ItemTemplate>
                    <div>
                        <asp:ImageButton ID="cmdDelete" runat="server" ImageUrl="Images/delete.png" CommandName="remove" ToolTip="Odstranit položku" />
                        <asp:Label ID="j11Name" runat="server" ForeColor="Green"></asp:Label>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Repeater ID="rpJ07" runat="server">
                <ItemTemplate>
                    <div>
                        <asp:ImageButton ID="cmdDelete" runat="server" ImageUrl="Images/delete.png" CommandName="remove" ToolTip="Odstranit položku" />
                        <asp:Label ID="j07Name" runat="server" ForeColor="Red"></asp:Label>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Repeater ID="rpJ02" runat="server">
                <ItemTemplate>
                    <div>
                        <asp:ImageButton ID="cmdDelete" runat="server" ImageUrl="Images/delete.png" CommandName="remove" ToolTip="Odstranit položku" />
                        <asp:HyperLink ID="linkPerson" runat="server"></asp:HyperLink>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <div class="div6">
                <asp:LinkButton ID="linkClearAll" runat="server" Text="<img src='Images/delete.png'/>Vyčistit výběr osob"></asp:LinkButton>
            </div>


            <div>
                <uc:person ID="j02ID_Add" runat="server" Width="100%" AutoPostBack="true" Text="Hledat osobu..." />
            </div>

            <div>
                <uc:datacombo ID="j11ID_Add" runat="server" DataTextField="j11Name" DataValueField="pid" IsFirstEmptyRow="true" Width="100%" AutoPostBack="true"></uc:datacombo>
            </div>

            <div>
                <uc:datacombo ID="j07ID_Add" runat="server" DataTextField="j07Name" DataValueField="pid" IsFirstEmptyRow="true" Width="100%" AutoPostBack="true"></uc:datacombo>
            </div>




        </asp:Panel>
    </div>
</div>
<asp:HiddenField ID="hidHeader" runat="server" />
<asp:HiddenField ID="hidJ02IDs_All" runat="server" />
<asp:HiddenField ID="hidJ02IDs" runat="server" />
<asp:HiddenField ID="hidJ07IDs" runat="server" />
<asp:HiddenField ID="hidJ11IDs" runat="server" />

