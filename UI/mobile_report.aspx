<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Mobile.Master" CodeBehind="mobile_report.aspx.vb" Inherits="UI.mobile_report" %>

<%@ Register Assembly="Telerik.ReportViewer.WebForms, Version=11.0.17.406, Culture=neutral, PublicKeyToken=a9d7983dfcc261be" Namespace="Telerik.ReportViewer.WebForms" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>
<%@ MasterType VirtualPath="~/Mobile.Master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   

    <script type="text/javascript">
    $(document).ready(function () {
       
        })

    
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <nav class="navbar navbar-default" style="margin-bottom: 0px !important;" id="nav1" runat="server" visible="false">
            <div class="navbar-header">
                <button type="button" class="navbar-toggle" data-toggle="collapse" data-target="#myNavbarOnSite">
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>
                
                <asp:HyperLink ID="RecordHeader" runat="server" CssClass="navbar-brand" Font-Underline="true"></asp:HyperLink>
               
                
            </div>
           
    </nav>
   
    <asp:DropDownList ID="x31ID" runat="server" AutoPostBack="true" DataValueField="pid" DataTextField="NameWithFormat" Style="width: 100%;"></asp:DropDownList>
    <div style="padding:6px;">
        <asp:LinkButton ID="cmdRunDefaultReport" CssClass="btn btn-primary btn-xs" runat="server" Text="Zobrazit náhled vybrané sestavy" Visible="false"></asp:LinkButton>
    </div>
    <uc:periodcombo ID="period1" runat="server" Width="100%" Visible="false"></uc:periodcombo>
    <asp:HyperLink ID="cmdDocMergeResult" runat="server" Text="Zobrazit výsledek" Visible="false"></asp:HyperLink>
    <asp:HyperLink ID="cmdXlsResult" runat="server" Text="XLS výstup" Visible="false"></asp:HyperLink>
    <div id="offsetY"></div>
   
        <telerik:ReportViewer ID="rv1" runat="server" Width="100%" Height="400px" ShowParametersButton="true" ShowHistoryButtons="false" ValidateRequestMode="Disabled">            
            <Resources PrintToolTip="Tisk" ExportSelectFormatText="Exportovat do zvoleného formátu" TogglePageLayoutToolTip="Přepnout na náhled k tisku" NextPageToolTip="Další strana" PreviousPageToolTip="Předchozí strana" RefreshToolTip="Obnovit" LastPageToolTip="Poslední strana" FirstPageToolTip="První strana" ></Resources>
        </telerik:ReportViewer>
   

    <asp:HiddenField ID="hidX29ID" runat="server" />
    <asp:HiddenField ID="hidPrefix" runat="server" />
</asp:Content>
