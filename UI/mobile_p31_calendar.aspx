<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Mobile.Master" CodeBehind="mobile_p31_calendar.aspx.vb" Inherits="UI.mobile_p31_calendar" %>

<%@ MasterType VirtualPath="~/Mobile.Master" %>
<%@ Register TagPrefix="uc" TagName="timesheet_calendar" Src="~/timesheet_calendar.ascx" %>
<%@ Register TagPrefix="uc" TagName="mobile_p31_list" Src="~/mobile_p31_list.ascx" %>
<%@ Register TagPrefix="uc" TagName="project" Src="~/project.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        tr.trTextRow {
            border-bottom: solid 2px gray;
        }

        .task_in_table {
            background-color: #5bc0de;
            color: white;
            text-align: center;
            white-space: pre-wrap;
            padding: 4px;
            margin-bottom: 5px;
            font-weight: bold;
            border-radius: 4px;
            font-size: 90%;
        }

        .person_in_table {
            background-color: green;
            color: white;
            text-align: center;
            white-space: pre-wrap;
            padding: 4px;
            margin-bottom: 5px;
            font-weight: bold;
            border-radius: 4px;
            font-size: 90%;
        }
    </style>

    <script type="text/javascript">
        function hardrefresh(flag, value) {
            if (flag == "edit") {
                location.replace("mobile_p31_framework.aspx?source=calendar&pid=" + value);
            }
            if (flag == "new") {
                var d = document.getElementById("<%=me.hidCurDate.ClientID%>").value;
                location.replace("mobile_p31_framework.aspx?source=calendar&defdate=" + d);

            }


        }

        function p31_entry(p41id) {

            var d = document.getElementById("<%=me.hidCurDate.ClientID%>").value;

            location.replace("mobile_p31_framework.aspx?source=calendar&p41id=" + p41id + "&defdate=" + d);
        }

        function p41id_search(sender, eventArgs) {
            //var item = eventArgs.get_item();
            var p41id = <%=Me.p41id.ClientID%>_get_value();
            p31_entry(p41id);

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <nav class="navbar navbar-default" style="margin-bottom: 0px !important;">
            <div class="navbar-header">
                <button type="button" class="navbar-toggle" data-toggle="collapse" data-target="#myNavbarOnSite">
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>
                <img src="Images/worksheet.png" class="navbar-brand" />                
              
                <asp:label ID="lblRecordHeader" runat="server" CssClass="navbar-brand" Text="Worksheet kalendář"></asp:label>

               
                
            </div>
            <div class="collapse navbar-collapse" id="myNavbarOnSite">
                <ul class="nav navbar-nav">
                    <li><asp:HyperLink ID="linkNew" runat="server" NavigateUrl="javascript:p31_entry()" Text="<img src='Images/new.png' /> Zapsat nový"></asp:HyperLink></li>
                    
                    <li>
                    <span style="padding-left:20px;">Naposledy:</span>
                    <asp:HyperLink ID="LastWorksheet" runat="server" CssClass="alinked"></asp:HyperLink>
                    </li>
                           
                    
                   
                    
                    
                </ul>
                
            </div>


    </nav>



    <uc:timesheet_calendar ID="cal1" runat="server" />
    <div style="margin-top: 5px; margin-left: 5px;">
        <div class="btn-group">
            <button type="button" class="btn btn-primary" data-toggle="dropdown">
                Zapsat
                <asp:Label ID="lblCurDate" runat="server" CssClass="badge"></asp:Label>
                <span class="caret"></span>
            </button>
            <ul class="dropdown-menu" role="menu">
                <asp:Repeater ID="rp1" runat="server">
                    <ItemTemplate>
                        <li>
                            <asp:HyperLink ID="link1" runat="server"></asp:HyperLink>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>

            </ul>
        </div>

        <img src="Images/sum.png" />
        <asp:Label ID="Hours_All" runat="server"></asp:Label>
        <div>
            <uc:project ID="p41id" runat="server" Width="97%" Flag="p31_entry" AutoPostBack="false" OnClientSelectedIndexChanged="p41id_search" />
        </div>

    </div>





    <a id="record_list"></a>
    <uc:mobile_p31_list ID="list1" runat="server"></uc:mobile_p31_list>
    <asp:HiddenField ID="hidCurDate" runat="server" />

</asp:Content>
