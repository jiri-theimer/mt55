<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="j03_messages.aspx.vb" Inherits="UI.j03_messages" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function o22_detail(pid) {
            window.open("dr.aspx?prefix=o22&pid=" + pid, "_top");
        }
        function o23_detail(pid) {
            window.open("dr.aspx?prefix=o23&pid=" + pid, "_top");
        }
        function p56_detail(pid) {
            window.open("dr.aspx?prefix=p56&pid=" + pid, "_top");
        }
        function p41_detail(pid) {
            window.open("p41_framework.aspx?pid=" + pid, "_top");
        }
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="boxO22" runat="server" CssClass="content-box2">
        <div class="title">
            <img src="Images/calendar.png" />
            <asp:Label ID="titleO22" runat="server" Text="Blízké události v kalendáři" Style="display: inline-block; min-width: 150px;"></asp:Label>
            
        </div>
        <div class="content">
            <table cellpadding="6">               
                <asp:Repeater ID="rpO22" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:hyperlink ID="cmdO22" runat="server"></asp:hyperlink>
                                
                            </td>
                            <td>
                                <asp:Label ID="Project" runat="server"></asp:Label>
                            </td>
                          
                            <td>
                                <span>Vlastník:</span>
                                <asp:Label ID="Owner" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
    </asp:Panel>
    
    <asp:Panel ID="boxP56" runat="server" CssClass="content-box2" style="margin-top:20px;">
        <div class="title">
            <img src="Images/task.png" />
            <asp:Label ID="titleP56" runat="server" Text="Blízké úkoly" Style="display: inline-block; min-width: 150px;"></asp:Label>
            
        </div>
        <div class="content">
            <table cellpadding="6">               
                <asp:Repeater ID="rpP56" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:hyperlink ID="cmdP56" runat="server"></asp:hyperlink>
                                
                            </td>
                            <td>
                                <asp:Label ID="Project" runat="server"></asp:Label>
                            </td>
                          
                            <td>
                                <span>Vlastník:</span>
                                <asp:Label ID="Owner" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
    </asp:Panel>

    <asp:Panel ID="boxO23" runat="server" CssClass="content-box2">
        <div class="title">
            <img src="Images/notepad.png" />
            <asp:Label ID="titleO23" runat="server" Text="Dokumenty s připomenutím" Style="display: inline-block; min-width: 150px;"></asp:Label>
            
        </div>
        <div class="content">
            <table cellpadding="6">               
                <asp:Repeater ID="rpO23" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:hyperlink ID="cmdO23" runat="server"></asp:hyperlink>
                                
                            </td>
                            <td>
                                <asp:Label ID="Project" runat="server"></asp:Label>
                            </td>
                          
                            
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
    </asp:Panel>

    <asp:Panel ID="boxP39" runat="server" CssClass="content-box2" style="margin-top:20px;">
        <div class="title">
            <img src="Images/worksheet_recurrence.png" />
            <asp:Label ID="titleP39" runat="server" Text="Generování opakovaných odměn/paušálů/úkonů" Style="display: inline-block; min-width: 150px;"></asp:Label>
            
        </div>
        <div class="content">
            <table cellpadding="6">               
                <asp:Repeater ID="rpP39" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:hyperlink ID="cmdProject" runat="server"></asp:hyperlink>
                            </td>
                            <td>
                                <asp:label ID="p39Text" runat="server" Font-Italic="true"></asp:label>
                                
                            </td>
                            <td>
                                <span>Kdy generovat:</span>
                                <asp:Label ID="p39DateCreate" runat="server" ForeColor="red"></asp:Label>
                            </td>
                            <td>
                                <span>Datum úkonu:</span>
                                <asp:Label ID="p39Date" runat="server" ForeColor="green"></asp:Label>
                            </td>
                            
                          
                            
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
    </asp:Panel>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
