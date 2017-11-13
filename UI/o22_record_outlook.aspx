<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="o22_record_outlook.aspx.vb" Inherits="UI.o22_record_outlook" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            <%If Request.Item("force") = "1" Then%>
            Handle_Download();
            <%end If%>
        });
        function Handle_Download() {
            var ics=document.getElementById("<%=hidIcsFile.ClientID%>").value;
            location.replace("binaryfile.aspx?tempfile="+ics);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="div6" style="height:40px;">
        
        <button type="button" id="cmdOdeslat" runat="server" onclick="Handle_Download()" style="font-size:x-large;" class="cmd" visible="false">Odeslat do OUTLOOK (nebo APPLE)</button>

        
    </div>

    <asp:panel ID="panRecord" runat="server" CssClass="content-box2">
        <div class="title">
            <img src="Images/calendar.png" />
            <asp:Label ID="lblHeader" runat="server" Text="Kalendářová událost"></asp:Label>
        </div>
        <div class="content">
            <table cellpadding="8">
                <tr>
                    <td colspan="2">
                        <asp:Label ID="o22Name" runat="server" CssClass="valboldblue"></asp:Label>
                    </td>
                </tr>
                
                <tr>
                    <td>
                        Kdy:
                    </td>
                    <td>
                        <asp:Label ID="o22DateFrom" runat="server" CssClass="valbold"></asp:Label>
                        
                        <asp:Label ID="o22DateUntil" runat="server" CssClass="valbold"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>Připomenutí (oznámení):</td>
                    <td>
                        <asp:Label ID="o22ReminderBeforeUnits" runat="server"></asp:Label>
                        <asp:Label ID="o22ReminderBeforeMetric" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>Kde:</td>
                    <td>
                        <asp:Label ID="o22Location" runat="server" ForeColor="Blue"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>Příjemci události:</td>
                    <td>
                        <asp:Label ID="Attendees" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
            <div class="div6">
                <asp:Label ID="o22Description" runat="server" Font-Italic="true"></asp:Label>
            </div>
        </div>
    </asp:panel>
    <asp:HiddenField ID="hidIcsFile" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
