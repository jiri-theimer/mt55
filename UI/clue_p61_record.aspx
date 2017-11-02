<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_p61_record.aspx.vb" Inherits="UI.clue_p61_record" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="max-height: 270px; overflow-x: auto; overflow-y: auto; width: 100%;">
        <div>
            <img src="Images/setting_32.png" />
            <asp:Label ID="ph1" runat="server" CssClass="clue_header_span"></asp:Label>
        </div>
        <div class="content-box2">
            <div class="title">
                Seznam aktivit v klastru
            </div>
            <div class="content">
                <asp:Repeater ID="rp1" runat="server">
                    <ItemTemplate>
                        <li>
                            <%# Eval("p34Name")%>
                            -
                            <%# Eval("p32Name")%>

                        </li>


                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</asp:Content>
