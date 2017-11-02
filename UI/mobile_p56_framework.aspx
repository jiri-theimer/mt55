<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Mobile.Master" CodeBehind="mobile_p56_framework.aspx.vb" Inherits="UI.mobile_p56_framework" %>
<%@ MasterType VirtualPath="~/Mobile.Master" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign_inline" Src="~/entityrole_assign_inline.ascx" %>
<%@ Register TagPrefix="uc" TagName="o23_list" Src="~/o23_list.ascx" %>
<%@ Register TagPrefix="uc" TagName="x18_readonly" Src="~/x18_readonly.ascx" %>
<%@ Register TagPrefix="uc" TagName="mobile_workflow_history" Src="~/mobile_workflow_history.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function o23_record(o23id) {
            location.replace("mobile_o23_framework.aspx?pid=" + o23id);

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
                
                <asp:hyperlink ID="RecordHeader" runat="server" CssClass="navbar-brand" style="text-decoration:underline;"></asp:hyperlink>

               
                
            </div>
            <div class="collapse navbar-collapse" id="myNavbarOnSite">
                <ul class="nav navbar-nav">                   
                    <li><a href="mobile_p31_framework.aspx?p56id=<%=Master.DataPID%>">Zapsat worksheet</a></li>
                    <li role="separator" class="divider"></li>
                    <li><a href="mobile_workflow_dialog.aspx?prefix=p56&pid=<%=Master.DataPID%>">Posunout/doplnit</a></li>
                    <li role="separator" class="divider"></li>
                    <li><a href="mobile_report.aspx?prefix=p56&pid=<%=Master.DataPID%>">Sestava</a></li>
                    
                    
                </ul>
               
            </div>

    </nav>



    <div class="container-fluid">
        <div id="row1" class="row">
           
            <button type="button" data-toggle="collapse" data-target="#history"><img src='Images/arrow_down.gif' />Historie</button>
            
            <div id="history"  class="collapse">
                <uc:mobile_workflow_history ID="history1" runat="server" />
            </div>

            <div class="col-sm-6 col-md-4" style="padding-left:1px;padding-right:1px;">
                <div class="thumbnail">
                    <div class="caption">
                        <img src="Images/task.png" />
                        <asp:Label ID="RecordName" runat="server"></asp:Label>

                    </div>
                    <table class="table table-hover">
                        <tr>
                            <td>
                                <span>Kód:</span>
                            </td>
                            <td>
                                <asp:Label ID="p56Code" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span>Projekt:</span>
                            </td>
                            <td>
                                <asp:HyperLink ID="Project" runat="server" CssClass="alinked"></asp:HyperLink>

                            </td>
                        </tr>

                        <tr id="trB02" runat="server">
                            <td>Workflow stav:
                            </td>
                            <td>
                                <asp:Label ID="b02Name" runat="server"></asp:Label>
                                <asp:HyperLink ID="linkWorkflow" runat="server" CssClass="btn btn-primary btn-xs" Text="Posunout/doplnit"></asp:HyperLink>
                            </td>
                        </tr>
                        <tr id="trp56PlanFrom" runat="server">
                            <td>Plán zahájení:
                            </td>
                            <td>
                                <asp:Label ID="p56PlanFrom" runat="server" CssClass="label label-success"></asp:Label>

                            </td>
                        </tr>
                        <tr>
                            <td>Termín:
                            </td>
                            <td>
                                <asp:Label ID="p56PlanUntil" runat="server" ForeColor="Red" Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span>Typ:</span>
                            </td>
                            <td>
                                <asp:Label ID="p57Name" runat="server"></asp:Label>


                            </td>
                        </tr>


                    </table>



                </div>
            </div>

            <div class="col-sm-6 col-md-4" style="padding-left:1px;padding-right:1px;">
                <div class="thumbnail">
                    <div class="caption">
                        Popis/zadání
                    </div>
                    <asp:Label ID="p56Description" runat="server" Font-Italic="true"></asp:Label>
                </div>
            </div>


            

            <div class="col-sm-6 col-md-4" style="padding-left:1px;padding-right:1px;">
                <div class="thumbnail">
                    
                    <div class="caption">
                        <img src="Images/projectrole.png" />
                        Přiřazené role
                    </div>
                    <table class="table table-hover">
                        <uc:entityrole_assign_inline ID="roles_project" runat="server" IsShowClueTip="false" IsRenderAsTable="true" EntityX29ID="p56Task" NoDataText="V úkolu nejsou přiřazeny role!"></uc:entityrole_assign_inline>
                    </table>
                    <div>
                        <span>Vlastník záznamu:</span>
                        <asp:Label ID="Owner" runat="server"></asp:Label>
                    </div>
                    <div>
                        <asp:Label ID="Timestamp" runat="server" Font-Italic="true"></asp:Label>
                    </div>
                </div>
            </div>

            <asp:Panel ID="boxP31" runat="server" CssClass="col-sm-6 col-md-4" style="padding-left:1px;padding-right:1px;">
                <div class="thumbnail">
                    <div class="caption">
                        <img src="Images/worksheet.png" />
                        <asp:HyperLink ID="cmdP31Grid" runat="server" Text="Worksheet přehled" CssClass="alinked"></asp:HyperLink>
                        <a href="mobile_p31_framework.aspx?source=task&p56id=<%=Master.DataPID%>" class="btn btn-primary btn-xs" style="float:right;">Nový</a>                  
                    </div>

                    <table cellpadding="6" class="table table-hover">
                        <tr>
                            <td>Vykázané hodiny:</td>
                            <td style="text-align: right;">
                                <asp:Label ID="Hours_Orig" runat="server" ></asp:Label>
                            </td>
                            <td></td>
                        </tr>

                        <tr id="trPlanHours" runat="server" visible="false">
                            <td>
                                <img src="Images/plan.png" />
                                Plán (limit) hodin:
                            </td>
                            <td style="text-align: right;">
                                <asp:Label ID="p56Plan_Hours" runat="server" ></asp:Label>

                            </td>
                            <td>
                                <asp:Label ID="PlanHoursSummary" runat="server" ></asp:Label>
                            </td>
                        </tr>
                        <tr id="trExpenses" runat="server" visible="false">
                            <td>Vykázané výdaje:</td>
                            <td style="text-align: right;">
                                <asp:Label ID="Expenses_Orig" runat="server" ></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                        <tr id="trPlanExpenses" runat="server" visible="false">
                            <td>
                                <img src="Images/finplan.png" />
                                Plán (limit) výdajů:
                            </td>
                            <td style="text-align: right;">
                                <asp:Label ID="p56Plan_Expenses" runat="server"></asp:Label>

                            </td>
                            <td>
                                <asp:Label ID="PlanExpensesSummary" runat="server" ></asp:Label>
                            </td>
                        </tr>
                    </table>

                </div>
            </asp:Panel>


            <asp:Panel ID="boxO23" runat="server" CssClass="col-sm-6 col-md-4" style="padding-left:1px;padding-right:1px;">
                <div class="thumbnail">
                    <div class="caption">
                        <img src="Images/notepad.png" />

                        <a href="mobile_grid.aspx?prefix=o23&masterprefix=p56&masterpid=<%=Master.DataPID%>" class="alinked">Dokumenty
                            <asp:Label runat="server" ID="CountO23" CssClass="badge"></asp:Label></a>

                    </div>
                    <table class="table table-hover">
                        <uc:o23_list ID="notepad1" runat="server" EntityX29ID="p56Task" IsShowClueTip="false"></uc:o23_list>
                    </table>
                </div>
            </asp:Panel>


            <asp:Panel ID="boxX18" runat="server" CssClass="col-sm-6 col-md-4" style="padding-left:1px;padding-right:1px;">
                <div class="thumbnail">
                    <div class="caption">
                        <img src="Images/label.png" />
                        Štítky
                        <asp:Label runat="server" ID="CountX18" CssClass="badge"></asp:Label>

                    </div>

                    <uc:x18_readonly id="labels1" runat="server"></uc:x18_readonly>

                </div>
            </asp:Panel>

            

        </div>
    </div>



</asp:Content>
