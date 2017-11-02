<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="x18_items.aspx.vb" Inherits="UI.x18_items" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function o23_record_in(pid,x18id) {

            dialog_master("o23_record.aspx?source=x18_items&x18id="+x18id+"&pid=" + pid, true)

        }

        function hardrefresh(pid, flag) {
            location.replace("x18_items.aspx?pid=<%=Master.DataPID%>")

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content-box2">
        <div class="title">
            <asp:Label ID="x18Name" runat="server"></asp:Label>
            <a href="javascript:o23_record_in(0,<%=Master.DataPID%>)" style="margin-left: 40px;">Přidat položku</a>
        </div>
        <div class="content">

            <asp:Repeater ID="rpO23" runat="server">
                <ItemTemplate>
                    <div class="badge_label" style="background-color: <%#Eval("o23BackColor")%>">
                        <a href="javascript:o23_record_in(<%# Eval("pid") %>,<%# Eval("x18id") %>)" style="color: <%#Eval("o23ForeColor")%>; text-decoration: <%#Eval("StyleDecoration")%>" title="Upravit/odstranit položku"><%# Eval("o23Name") %></a>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            
        </div>
    </div>

    <asp:HiddenField ID="hidX23ID" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
