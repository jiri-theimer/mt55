<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_p51_record.aspx.vb" Inherits="UI.clue_p51_record" %>

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
           <img src="Images/billing_32.png" />
           <asp:Label ID="ph1" runat="server" CssClass="clue_header_span"></asp:Label>
       </div>
        <asp:HyperLink ID="cmdEdit" runat="server" NavigateUrl="javascript:detail()" Text="Upravit nastavení ceníku" style="position: absolute; top: 50px; left: 500px;"></asp:HyperLink>
        
        <div class="content-box2">
            <div class="title">
                Záznam ceníku
            </div>
            <div class="content">
                <table cellpadding="6">
            <tr>
                <td>Měna ceníku:</td>
                <td>
                    <asp:Label ID="j27Code" runat="server" CssClass="valbold"></asp:Label>

                </td>
                <td>Výchozí hodinová sazba:</td>
                <td>
                    <asp:Label ID="p51DefaultRateT" runat="server" CssClass="valbold" style="color:red;"></asp:Label>

                </td>
            </tr>

        </table>
            </div>
        </div>
        
        <div class="content-box2">
            <div class="title">
                Položkové sazby (vyjímky z výchozí sazby ceníku)
            </div>
            <div class="content">
                <table cellpadding="5">

                <asp:Repeater ID="rpP52" runat="server">
                    <ItemTemplate>
                        <tr valign="top">

                            <td>
                                <asp:Label ID="p34Name" runat="server"></asp:Label>
                                <div>
                                    <asp:Label ID="p32Name" runat="server"></asp:Label>
                                </div>
                            </td>
                            <td>
                                <asp:Label ID="Subject" runat="server"></asp:Label>
                            </td>
                            <td align="right">
                                <asp:Label ID="p52Rate" runat="server" style="font-weight:bold;color:red;"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="j27Code" runat="server"></asp:Label>
                            </td>
                            <td>
                                <i>
                                    <asp:Label ID="p52Name" runat="server"></asp:Label></i>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
            </div>
        </div>

       

        <asp:Panel ID="panMaster" runat="server" Visible="false">
            <fieldset>
                <legend>MASTER ceník</legend>
                <table cellpadding="5" cellspacing="2">
                    <tr>
                        <td>Nadřazený MASTER ceník:

                        </td>
                        <td>
                            <asp:Label ID="MasterPricelist" runat="server" CssClass="valbold"></asp:Label>
                        </td>
                        <td>
                            Výchozí sazba:
                        </td>
                        <td>
                            <asp:Label ID="MasterPricelist_DefaultRate" runat="server" CssClass="valbold" style="color:red;"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table cellpadding="5">
                    <asp:Repeater ID="rpP52_Master" runat="server">
                        <ItemTemplate>
                            <tr valign="top">

                                <td>
                                    <asp:Label ID="p34Name" runat="server"></asp:Label>
                                    <div>
                                        <asp:Label ID="p32Name" runat="server"></asp:Label>
                                    </div>
                                </td>
                                <td>
                                    <asp:Label ID="Subject" runat="server"></asp:Label>
                                </td>
                                <td align="right" style="font-weight:bold;color:red;">
                                    <asp:Label ID="p52Rate" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="j27Code" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <i>
                                        <asp:Label ID="p52Name" runat="server"></asp:Label></i>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </fieldset>


        </asp:Panel>
    </asp:panel>
</asp:Content>
