<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Mobile.Master" CodeBehind="mobile_o22_framework.aspx.vb" Inherits="UI.mobile_o22_framework" %>

<%@ MasterType VirtualPath="~/Mobile.Master" %>
<%@ Register TagPrefix="uc" TagName="x18_readonly" Src="~/x18_readonly.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
                  
                    <li><a href="mobile_report.aspx?prefix=o22&pid=<%=Master.DataPID%>">Sestava</a></li>
                    
                    
                </ul>
               
            </div>

    </nav>



    <div class="container-fluid">
        <div id="row1" class="row">


            <div class="col-sm-6 col-md-4" style="padding-left: 1px; padding-right: 1px;">
                <div class="thumbnail">
                    <div class="caption">
                        <img src="Images/event.png" />
                        <asp:Label ID="RecordName" runat="server" Font-Bold="true"></asp:Label>

                    </div>
                    <table class="table table-hover">

                        <tr>
                            <td>
                                <span>Projekt:</span>
                            </td>
                            <td>
                                <asp:HyperLink ID="Project" runat="server" CssClass="alinked"></asp:HyperLink>

                            </td>
                        </tr>


                        <tr>
                            <td>Začátek:
                            </td>
                            <td>
                                <asp:Label ID="o22DateFrom" runat="server" CssClass="label label-success"></asp:Label>

                            </td>
                        </tr>
                        <tr>
                            <td>Konec:
                            </td>
                            <td>
                                <asp:Label ID="o22DateUntil" runat="server" ForeColor="Red" Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span>Typ:</span>
                            </td>
                            <td>
                                <asp:Label ID="o21Name" runat="server"></asp:Label>


                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label1" runat="server" CssClass="lbl" Text="Příjemce:"></asp:Label>
                            </td>
                            <td>
                                <asp:Repeater ID="rpO20" runat="server">
                                    <ItemTemplate>

                                        <span class="valboldblue" style="padding-right: 20px;"><%# Eval("Person")%><%# Eval("j11Name")%></span>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </td>
                        </tr>
                     
                        <tr>
                            
                            <td colspan="2">
                                <asp:Label ID="Timestamp" runat="server" Font-Italic="true"></asp:Label>
                            </td>

                        </tr>

                    </table>



                </div>
            </div>

            <div class="col-sm-6 col-md-4" style="padding-left: 1px; padding-right: 1px;">
                <div class="thumbnail">
                    <div class="caption">
                        Poznámka
                    </div>
                    <asp:Label ID="o22Description" runat="server" Font-Italic="true"></asp:Label>
                </div>
            </div>










            <asp:Panel ID="boxX18" runat="server" CssClass="col-sm-6 col-md-4" Style="padding-left: 1px; padding-right: 1px;">
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
