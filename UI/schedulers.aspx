<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="schedulers.aspx.vb" Inherits="UI.schedulers" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>



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

        });
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
            <div style="clear:both;"></div>
            <asp:Repeater ID="rp1" runat="server">
                <ItemTemplate>
                    <div>
                        <asp:CheckBox ID="chk1" runat="server" AutoPostBack="true" CssClass="chk" />
                        <asp:HiddenField ID="o25Code" runat="server" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:TextBox ID="txt1" runat="server" TextMode="MultiLine"></asp:TextBox>
            <div><asp:Button ID="cmd1" runat="server" Text="pokus" /></div>

        </div>
        <div id="right_panel" style="margin-left: 210px;">
            <iframe id="fra1" runat="server" width="99.5%" height="600px" frameborder="0" scrolling="no"></iframe>
        </div>

   

</asp:Content>
