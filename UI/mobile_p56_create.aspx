<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Mobile.Master" CodeBehind="mobile_p56_create.aspx.vb" Inherits="UI.mobile_p56_create" %>
<%@ MasterType VirtualPath="~/Mobile.Master" %>
<%@ Register TagPrefix="uc" TagName="project" Src="~/project.ascx" %>
<%@ Register TagPrefix="uc" TagName="person_or_team" Src="~/person_or_team.ascx" %>
<%@ Register TagPrefix="uc" TagName="datepicker" Src="~/datepicker.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
           


        });
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
              
                <asp:label ID="lblRecordHeader" runat="server" CssClass="navbar-brand" Text="Vytvořit úkol"></asp:label>

               
                
            </div>
            <div class="collapse navbar-collapse" id="myNavbarOnSite">
                <ul class="nav navbar-nav">
                    <li><asp:HyperLink ID="linkCurProject" runat="server" NavigateUrl="mobile_p41_framework.aspx"></asp:HyperLink></li>
                </ul>
            </div>

    </nav>



    <uc:project ID="p41id" runat="server" Width="99%" Flag="createtask" AutoPostBack="true" />

    <asp:panel ID="panRecord" runat="server"  CssClass="panel panel-default">   
        <asp:LinkButton ID="cmdSave" runat="server" Text="Uložit změny" CssClass="btn btn-primary"></asp:LinkButton>
    <asp:LinkButton ID="cmdCancel" runat="server" Text="Zrušit změny" CssClass="btn btn-default"></asp:LinkButton>
             
        <div class="panel-body">
            <asp:Panel ID="panType" runat="server">
                <span>Typ úkolu:</span>
                <asp:DropDownList ID="p57ID" runat="server" CssClass="form-control" DataTextField="p57Name" DataValueField="pid"></asp:DropDownList>
            </asp:Panel>
            <div>
                <span>Název:</span>
                <asp:TextBox ID="p56Name" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <div>
                <span>Termín splnění:</span>
                <uc:datepicker ID="p56PlanUntil" runat="server" IsUseTimepicker="true" IsTimePer30Minutes="true" starthour="8" endhour="22"></uc:datepicker>
          
            </div>
            <div>
                <span>Plán zahájení:</span>
                <uc:datepicker ID="p56PlanFrom" runat="server" IsUseTimepicker="true" IsTimePer30Minutes="true" starthour="8" endhour="22"></uc:datepicker>
          
            </div>
            <div>
                <span>Řešitel (příjemce):</span>
                <asp:LinkButton ID="cmdAddReceiver" runat="server" CssClass="btn btn-primary btn-xs" Text="Přidat"></asp:LinkButton>
                <asp:DropDownList ID="x67ID" runat="server" DataValueField="pid" DataTextField="x67Name"></asp:DropDownList>
                <uc:person_or_team ID="receiver1" runat="server"></uc:person_or_team>
            </div>

            <div>
                <span>Podrobný popis:</span>
                <asp:TextBox ID="p56Description" runat="server" CssClass="form-control" Style="height: 90px; background-color: #ffffcc;" TextMode="MultiLine"></asp:TextBox>
            </div>
            <div>
                <asp:CheckBox ID="p56IsNoNotify" runat="server" Text="Vypnout automatické notifikační zprávy" />
            </div>
        </div>
    </asp:panel>


    <asp:HiddenField ID="hidRef" runat="server" />
</asp:Content>
