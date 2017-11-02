<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="dr.aspx.vb" Inherits="UI.dr" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
           
        })

        function sw_local(url,iconUrl,is_maximize){
            sw_master(url,iconUrl,is_maximize)
            
        }
       
        function hardrefresh(pid, flag) {
            location.replace("dr.aspx?pid=<%=ViewState("pid")%>&prefix=<%=ViewState("prefix")%>");

        }

        function loadSplitter(sender) {
            var h1 = new Number;
            var h2 = new Number;
            var h3 = new Number;

            h1 = $(window).height();

            var ss = self.document.getElementById("offsetY");
            var offset = $(ss).offset();

            h2 = offset.top;

            h3 = h1 - h2;

            sender.set_height(h3);

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="offsetY"></div>
    <telerik:RadSplitter ID="splitter1" runat="server" Width="100%" OnClientLoaded="loadSplitter">
        <telerik:RadPane ID="paneContent" runat="server" Width="100%" BackColor="white">
        </telerik:RadPane>
    </telerik:RadSplitter>
   
</asp:Content>
