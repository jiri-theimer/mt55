<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Mobile.Master" CodeBehind="mobile_p91_framework.aspx.vb" Inherits="UI.mobile_p91_framework" %>

<%@ MasterType VirtualPath="~/Mobile.Master" %>
<%@ Register TagPrefix="uc" TagName="x18_readonly" Src="~/x18_readonly.ascx" %>

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

                    <li><a href="mobile_report.aspx?prefix=p91&pid=<%=Master.DataPID%>">Sestava</a></li>
                    <li role="separator" class="divider"></li>
                    
                 

                   
                    
                </ul>
               
            </div>

    </nav>
    <div style="background-color:whitesmoke;margin-bottom:6px;">
        <ul class="nav nav-pills">
                <li role="presentation" id="liP41" runat="server"><a href="mobile_grid.aspx?prefix=p41&masterprefix=p91&masterpid=<%=Master.DataPID%>">Fakturované projekty
            <asp:Label runat="server" ID="CountP41" CssClass="badge"></asp:Label></a></li>
                <li role="presentation" id="liP31" runat="server"><a href="mobile_grid.aspx?prefix=p31&masterprefix=p91&masterpid=<%=Master.DataPID%>&closed=0">Fakturované úkony
            <asp:Label runat="server" ID="CountP31" CssClass="badge"></asp:Label></a></li>


            </ul>
    </div>



    <div class="container-fluid">
        <div id="row1" class="row">
            <div class="col-sm-6 col-md-4" style="padding-left: 1px; padding-right: 1px;">
                <div class="thumbnail">
                    <div class="caption">
                        <img src="Images/invoice.png" />

                        <asp:HyperLink ID="cmdReportInvoice" runat="server" Text="Sestava dokladu" CssClass="alinked"></asp:HyperLink>
                        <asp:HyperLink ID="cmdReportAttachment" runat="server" Text="Sestava přílohy" CssClass="alinked" Style="margin-left: 10px;"></asp:HyperLink>
                    </div>
                    <table class="table table-hover">

                        <tr>
                            <td>
                                <span>Číslo dokladu:</span>
                            </td>
                            <td>
                                <asp:Label ID="p91Code" runat="server"></asp:Label>
                                <asp:Image ID="imgDraft" runat="server" ImageUrl="Images/draft_icon.gif" Visible="false" AlternateText="DRAFT záznam" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span>Typ:</span>
                            </td>
                            <td>
                                <asp:Label ID="p92Name" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr id="trB02" runat="server" visible="false">
                            <td>Workflow stav:
                            </td>
                            <td>
                                <asp:Label ID="b02Name" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span>Klient:</span>
                            </td>
                            <td>
                                <asp:HyperLink ID="Client" runat="server" CssClass="alinked"></asp:HyperLink>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span>Celkový dluh:</span>
                            </td>
                            <td style="text-align: right;">
                                <asp:Label ID="p91Amount_Debt" runat="server"></asp:Label>

                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span>Částka bez DPH:</span>
                            </td>
                            <td style="text-align: right;">
                                <asp:Label ID="p91Amount_WithoutVat" runat="server"></asp:Label>

                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span>Částka DPH:</span>
                            </td>
                            <td style="text-align: right;">
                                <asp:Label ID="p91Amount_Vat" runat="server"></asp:Label>

                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span>Datum vystavení:</span>
                            </td>
                            <td>
                                <asp:Label ID="p91Date" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span>Datum plnění:</span>
                            </td>
                            <td>
                                <asp:Label ID="p91DateSupply" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span>Datum splatnosti:</span>
                            </td>
                            <td>
                                <asp:Label ID="p91DateMaturity" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>

                </div>
            </div>
            
            <div class="col-sm-6 col-md-4" style="padding-left:1px;padding-right:1px;">
                <div class="thumbnail">
                    <div class="caption">
                        Text faktury                        
                    </div>
                    <asp:Label ID="p91Text1" runat="server" Font-Italic="true"></asp:Label>
                </div>
            </div>








            <asp:Panel ID="boxX18" runat="server" CssClass="col-sm-6 col-md-4" Visible="false" style="padding-left:1px;padding-right:1px;">
                <div class="thumbnail">
                    <div class="caption">
                        <img src="Images/label.png" />
                        Štítky
                        <asp:Label runat="server" ID="CountX18" CssClass="badge"></asp:Label>

                    </div>

                    <uc:x18_readonly ID="labels1" runat="server"></uc:x18_readonly>

                </div>
            </asp:Panel>

        </div>
    </div>
</asp:Content>
