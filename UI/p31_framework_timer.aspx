<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="p31_framework_timer.aspx.vb" Inherits="UI.p31_framework_timer" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>
<%@ Register TagPrefix="uc" TagName="timer" Src="~/timer.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        @media screen and (max-width: 600px) {

        .RadComboBox {
        width: 300px !important;
    }

            
        }
    </style>

    <script src="Scripts/jquery.timer.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(document).ready(function () {

            function getTime() {
                var d = new Date;
                var hours = d.getHours();
                var mins = d.getMinutes();
                var secs = d.getSeconds();

                return ("0" + hours).slice(-2) + ":" + ("0" + mins).slice(-2) + ":" + ("0" + secs).slice(-2);
                //return h.substring(h.length-1,2) + ":" +m.substring(m.length-1,2)
            }
            setInterval(function () {
                $("#clock").html(getTime())
            }, 1000);

            $("#clock").html(getTime());
        });

        function hardrefresh(pid, flag) {
            
            location.replace("p31_framework_timer.aspx");

        }

       
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="content-box2">
        <div class="title">
            <img src="Images/stopwatch.png" alt="STOPKY" />
            <asp:Label ID="lblHeader" runat="server" CssClass="page_header_span" Text="Stopky"></asp:Label>
            <span id="clock" style="float:right;"></span>
        </div>
        <div class="content">
            <uc:timer ID="timer1" runat="server" IsPanelView="true" IsIFrame="true"></uc:timer>
        </div>
    </div>

</asp:Content>
