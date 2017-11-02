<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Mobile.Master" CodeBehind="mobile_workflow_dialog.aspx.vb" Inherits="UI.mobile_workflow_dialog" %>

<%@ MasterType VirtualPath="~/Mobile.Master" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="person_or_team" Src="~/person_or_team.ascx" %>
<%@ Register TagPrefix="uc" TagName="mobile_workflow_history" Src="~/mobile_workflow_history.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function goback() {
            window.history.back();
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
                    <li><asp:HyperLink ID="linkBack" runat="server" Text="Jít zpět"></asp:HyperLink></li>
                    
                    
                    
                </ul>
               
            </div>

    </nav>

    <asp:LinkButton ID="cmdSave" runat="server" Text="Uložit změny" CssClass="btn btn-primary"></asp:LinkButton>
    <asp:LinkButton ID="cmdCancel" runat="server" Text="Zrušit změny" CssClass="btn btn-default"></asp:LinkButton>

    <div class="container-fluid">
        <div id="row1" class="row">
            <div class="col-sm-6 col-md-4" style="padding-left: 1px; padding-right: 1px;">
                <div class="thumbnail">


                    <div>
                        <asp:RadioButtonList ID="opgB06ID" runat="server" AutoPostBack="true" CssClass="chk" CellPadding="4" CellSpacing="2"></asp:RadioButtonList>
                    </div>
                    <asp:Panel ID="panNotify" runat="server" Visible="false" CssClass="content-box2">
                        <div class="title">
                            <img src="Images/email.png" />
                            Komu poslat notifikaci komentáře
                    <asp:Button ID="cmdAddNotifyReceiver" runat="server" CssClass="cmd" Text="Přidat" />
                        </div>
                        <div class="content">
                            <uc:person_or_team ID="receiver1" runat="server"></uc:person_or_team>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="panNominee" runat="server" Visible="false" CssClass="div6">
                        <asp:Button ID="cmdAddNominee" runat="server" Text="Přidat" CssClass="cmd" />
                        <asp:Repeater ID="rpNominee" runat="server">
                            <ItemTemplate>
                                <div>
                                    <asp:Image ID="img1" runat="server" ImageUrl="Images/projectrole_team.png"></asp:Image>
                                    <span>Osoba:</span>
                                    <uc:person ID="j02ID" runat="server" Width="200px" Flag="all" />
                                    <span>nebo tým osob:</span>
                                    <asp:DropDownList ID="j11id" runat="server" DataTextField="j11Name" DataValueField="pid" Style="width: 200px;" Font-Bold="true"></asp:DropDownList>

                                    <asp:ImageButton ID="del" runat="server" ImageUrl="Images/delete_row.png" ToolTip="Odstranit položku" CssClass="button-link" />
                                    <asp:HiddenField ID="p85id" runat="server" />
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                    </asp:Panel>

                    <asp:TextBox ID="b07Value" runat="server" TextMode="MultiLine" CssClass="form-control" Style="height: 90px; background-color: #ffffcc;"></asp:TextBox>

                </div>
            </div>
            <div>
                <uc:mobile_workflow_history ID="history1" runat="server" />
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hidPrefix" runat="server" />
    <asp:HiddenField ID="hidRecordPID" runat="server" />
</asp:Content>
