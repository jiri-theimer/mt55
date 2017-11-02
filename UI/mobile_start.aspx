<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Mobile.Master" CodeBehind="mobile_start.aspx.vb" Inherits="UI.mobile_start" %>

<%@ MasterType VirtualPath="~/Mobile.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {

            document.getElementById("resolution").innerText = screen.width + "x" + screen.height;

        });

        function p31_entry() {
            location.replace("mobile_p31_framework.aspx?pid=0&source=calendar");
        }
        function p31_calendar() {
            location.replace("mobile_p31_calendar.aspx");
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
                
                
                
                        <img src="Images/task.png" class="navbar-brand" />
                        <a href="mobile_start.aspx" style="text-decoration:underline;" class="navbar-brand">Otevřené úkoly</a>
                        <span class="navbar-brand"><asp:Label runat="server" ID="CountP56" CssClass="badge"></asp:Label></span>
                        

               
                
            </div>
            <div class="collapse navbar-collapse" id="myNavbarOnSite">
                <ul class="nav navbar-nav">    
                    <li>
                        <span>Displej:</span>
                        <span id="resolution"></span>
                    </li>  
                    <li>
                        <span><%=Master.Factory.SysUser.Person%></span>
                        
                    </li>  
                    <li>
                        <asp:DropDownList ID="cbxScope" runat="server" AutoPostBack="true">
                            <asp:ListItem Text="Jsem řešitelem úkolů" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Jsem zakladatelem (vlastníkem) úkolů" Value="2"></asp:ListItem>
                        </asp:DropDownList>
                    </li>           
                    <li>
                        <asp:HyperLink ID="linkCreateTask" runat="server" NavigateUrl="mobile_p56_create.aspx?source=start" Text="<img src='Images/task.png' /> Nový úkol"></asp:HyperLink>
                    </li>
                    
                    
                </ul>
               
            </div>

    </nav>


    <div class="container-fluid">
        <div id="row1" class="row">
            
            

            <asp:Panel ID="panP56" runat="server" CssClass="col-sm-6 col-md-4" style="padding-left:1px;padding-right:1px;">
                <div class="thumbnail">
                 
                    <table class="table table-condensed">
                        <asp:Repeater ID="rp1" runat="server">
                            <ItemTemplate>
                                <tr style="background-color:whitesmoke;" id="trRow" runat="server">
                                    <td colspan="2">
                                        <span><%#Eval("p41Name")%></span>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <a href="mobile_p56_framework.aspx?pid=<%#Eval("pid")%>" class="alinked"><%#Eval("p56Code")%> - <i><%#Eval("p56Name")%></i></a>
                                    </td>
                                    <td>
                                        
                                        <span style="color:red;font-weight:bold;"><%#BO.BAS.FD(Eval("p56PlanUntil"),True,true)%></span>
                                        <span style="background-color:<%#Eval("b02Color")%>"><%#Eval("b02Name")%></span>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        
                                    </td>
                                </tr>
                                
                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                </div>
            </asp:Panel>


        </div>
    </div>
</asp:Content>
