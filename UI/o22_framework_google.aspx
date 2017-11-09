<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="o22_framework_google.aspx.vb" Inherits="UI.o22_framework_google" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="myscheduler" Src="~/myscheduler.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        @media screen and (max-width: 900px) {

            #left_panel {
                width: 0px !important;
            }

            #right_panel {
                margin-left: 0px !important;
            }

            #left_panel {
                display: none !important;
            }
        }

        div.RadScheduler .rsMonthView .rsTodayCell {
            background-color: skyblue;
        }
    </style>


    <script type="text/javascript">
        $(document).ready(function () {

            <%If fra1.Visible Then%>
            var h1 = new Number;
            var h2 = new Number;
            var hh = new Number;

            h1 = $(window).height();

            ss = self.document.getElementById("offsetY");
            var offset = $(ss).offset();

            h2 = offset.top;

            <%If LCase(Request.Browser.Browser) = "ie" Then%>
            hh = h1 - h2 - 4;
            <%Else%>
            hh = h1 - h2 - 2;
            <%End If%>
            hh = hh - 7;
            self.document.getElementById("<%=fra1.clientid%>").height = hh + "px";
            <%End If%>
        });


        function hardrefresh(pid, flag) {



            location.replace("o22_framework_google.aspx");


        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="offsetY"></div>

    <div id="left_panel" style="float: left; width: 210px;">
        <div style="float: left;">
            <img src="Images/calendar_32.png" />
        </div>
        <div class="div6" style="float: left;">

            <span class="page_header_span">Kalendáře</span>
        </div>
        <div style="clear: both;"></div>

        <div style="background-color:white;padding:6px;border-top:solid 1px silver;border-bottom:solid 1px silver;">
            <asp:RadioButtonList ID="opgView" runat="server" AutoPostBack="true" RepeatDirection="Vertical">
                    <asp:ListItem Text="Termíny a úkoly" Value="1" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Integrované Google kalendáře" Value="2"></asp:ListItem>
                </asp:RadioButtonList>
        </div>


        <asp:Panel ID="panGoogle" runat="server">
            <asp:Repeater ID="rp1" runat="server">
                <ItemTemplate>
                    <div>
                        <asp:CheckBox ID="chk1" runat="server" AutoPostBack="true" CssClass="chk" />
                        <asp:HiddenField ID="o25Code" runat="server" />
                        <asp:HiddenField ID="o25id" runat="server" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <div class="div6">
                <asp:HyperLink ID="link2Google" runat="server" NavigateUrl="https://calendar.google.com" Target="_blank">Přejít do Google kalendářů</asp:HyperLink>
            </div>
        </asp:Panel>
        <div class="div6">
            <asp:HyperLink ID="linkNew" runat="server" NavigateUrl="javascript:contMenu('select_event_type.aspx',false)">Nová událost v kalendáři</asp:HyperLink>
        </div>
    </div>
    <div id="right_panel" style="margin-left: 210px;">
            <uc:myscheduler ID="cal1" runat="server" />
            <iframe id="fra1" runat="server" width="99.5%" height="600px" frameborder="0" scrolling="no"></iframe>
        </div>

   

</asp:Content>
