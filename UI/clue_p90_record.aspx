<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_p90_record.aspx.vb" Inherits="UI.clue_p90_record" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
     <script type="text/javascript">
        function detail() {

            window.parent.sw_local("p51_record.aspx?pid=<%=Master.DataPID%>", "Images/billing_32.png", true);

        }
       
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:panel ID="panContainer" runat="server" style="max-height: 270px; overflow-x: auto; overflow-y: auto; width: 100%;">
        <div>
           <img src="Images/proforma_32.png" />
           <asp:Label ID="ph1" runat="server" CssClass="clue_header_span"></asp:Label>
       </div>
        
        <table cellpadding="10" cellspacing="2">
         
            <tr>
                <td>Klient:</td>
                <td>
                    <asp:Label ID="p28Name" runat="server" CssClass="valbold"></asp:Label>

                </td>
                
            </tr>
            <tr>
                <td>Částka:</td>
                <td>
                    <asp:Label ID="p90Amount" runat="server" CssClass="valbold"></asp:Label>
                    <asp:Label ID="j27Code" runat="server" CssClass="val"></asp:Label>
                </td>
                
            </tr>
            <tr>
                <td>Uhrazeno:</td>
                <td>
                    <asp:Label ID="p90Amount_Billed" runat="server" CssClass="valbold"></asp:Label>
                   
                </td>
                
            </tr>
            <tr>
                <td>Datum úhrady:</td>
                <td>
                    <asp:Label ID="p90DateBilled" runat="server" CssClass="valbold"></asp:Label>
                   
                </td>
            </tr>
        </table>
        <div class="bigtext" style="width: 99%;">
            <asp:Label ID="p90Text1" runat="server"></asp:Label>
        </div>
        <div><b>Spárované faktury:</b></div>
        <table cellpadding="10">
        <asp:Repeater ID="rp1" runat="server">
            <ItemTemplate>
                <tr>
                    <td>
                        <%# Eval("p91Code")%>
                    </td>
                    <td align="right">
                        <%# Eval("p99Amount")%>
                    </td>
                    <td align="right">
                        <%# Eval("p99Amount_WithoutVat")%>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
        </table>

    </asp:panel>
</asp:Content>
