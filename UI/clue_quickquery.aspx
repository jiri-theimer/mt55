<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_quickquery.aspx.vb" Inherits="UI.clue_quickquery" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
      
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="panContainer" runat="server" Style="max-height: 270px; overflow-x: auto; overflow-y: auto; width: 100%;">
        <div class="div6">
            <img src="Images/query_32.png" />
            <asp:Image ID="imgEntity" runat="server" />
            <asp:Label ID="ph1" runat="server" Text="Datový přehled" CssClass="clue_header_span"></asp:Label>

        </div>


        <asp:Panel ID="panJ71" runat="server" CssClass="content-box2">
            <div class="title"><asp:label id="lblHeader" runat="server" Text="Podmínka filtru"></asp:label></div>
            <div class="content">
                <div class="div6">
                    <asp:Label ID="lblBinFlag" runat="server" CssClass="valboldblue"></asp:Label>
                </div>
                <table cellpadding="3" cellspacing="2">
                    <asp:Repeater ID="rpJ71" runat="server">
                        <ItemTemplate>
                            <tr class="trHover">
                                <td style="min-width: 150px;">
                                    <asp:Label ID="j71FieldLabel" runat="server" CssClass="val" Style="color: Gray;"></asp:Label>


                                </td>
                                <td>
                                    <i>
                                        <asp:Label ID="nebo" runat="server" CssClass="val" Style="color: blue;" Text="nebo"></asp:Label>
                                    </i>
                                    <asp:Label ID="j71RecordName" runat="server" CssClass="val"></asp:Label>

                                    <asp:Label ID="j71RecordName_Extension" runat="server" CssClass="val" Style="color: red; margin-left: 40px;"></asp:Label>
                                </td>


                            </tr>
                        </ItemTemplate>

                    </asp:Repeater>
                </table>
            </div>
        </asp:Panel>

        <div style="padding: 6px; margin-top: 30px;">
            
                <asp:Label ID="lblTimeStamp" runat="server" CssClass="timestamp"></asp:Label>
           
        </div>

    </asp:Panel>
</asp:Content>
