<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="j03_mypage.aspx.vb" Inherits="UI.j03_mypage" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="Plugins/Plugin.css" />
    <link rel="stylesheet" type="text/css" href="Scripts/datatable/css/jquery.dataTables.css" />
    <script type="text/javascript" src="Scripts/datatable/jquery.dataTables.min.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {


        });

        function personalpage() {
            sw_master("j03_myprofile_defaultpage.aspx", "Images/plugin_32.png")


        }
        function setting() {
            sw_master("j03_mypage_setting.aspx", "Images/setting_32.png")


        }

        function hardrefresh(pid, flag) {

            location.replace("default.aspx");

        }

        function p31_record(pid, p41id) {
            sw_master("p31_record.aspx?pid=" + pid + "&p41id=" + p41id, "Images/worksheet_32.png")
        }
        function p56_ukon(p56id) {
            sw_master("p31_record.aspx?p56id=" + p56id, "Images/worksheet_32.png")
        }
        function p56_record(pid) {
            sw_master("clue_p56_record.aspx?noclue=1&pid=" + pid, "Images/task_32.png")
        }
        function o22_record(pid) {
            sw_master("clue_o22_record.aspx?noclue=1&pid=" + pid, "Images/milestone_32.png")
        }
        function sw_local(url, icon, is_maximize) {
            sw_master(url, icon, false)

        }


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <table width="100%">
        <tr>
            <td style="width: 35px;">
                <asp:Image ID="img1" runat="server" ImageUrl="Images/dashboard_32.png" />
            </td>
            <td>
                <asp:Label ID="lblHeader" runat="server" CssClass="page_header_span" Text="Osobní stránka"></asp:Label>
            </td>
            <td>
                <button type="button" onclick="setting()">Upravit obsah stránky</button>
            </td>
            <td>Skin:
                <asp:DropDownList ID="cbxSkin" runat="server" AutoPostBack="true">
                    <asp:ListItem Text="Silk" Value="Silk" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Metro" Value="Metro"></asp:ListItem>
                    <asp:ListItem Text="Simple" Value="Simple"></asp:ListItem>
                    <asp:ListItem Text="WebBlue" Value="WebBlue"></asp:ListItem>
                    <asp:ListItem Text="Windows7" Value="Windows7"></asp:ListItem>
                    <asp:ListItem Text="Outlook" Value="Outlook"></asp:ListItem>
                    <asp:ListItem Text="Vista" Value="Vista"></asp:ListItem>
                    <asp:ListItem Text="Sunset" Value="Sunset"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td align="right">
                <asp:Button ID="cmdSaveState" runat="server" CssClass="cmd" Text="Uložit aktuální rozložení gadgetů" />
            </td>

            <td></td>
            <td align="right">
                <asp:Button ID="cmdResetState" runat="server" CssClass="cmd" Text="Vyčistit rozložení gadgetů do výchozí stavu" />
            </td>
        </tr>
    </table>

    <telerik:RadDockLayout runat="server" ID="docklayout1" EnableViewState="true" Skin="Silk">
        <table>
            <tr>
                <td style="vertical-align: top;">
                    <telerik:RadDockZone runat="server" ID="zone1" Orientation="Vertical">
                        <telerik:RadDock ID="dock1" runat="server" DockMode="Docked">
                            <ContentTemplate>
                                <asp:PlaceHolder ID="place_dock1" runat="server"></asp:PlaceHolder>
                            </ContentTemplate>

                        </telerik:RadDock>
                        <telerik:RadDock ID="dock3" runat="server" DockMode="Docked">
                            <ContentTemplate>
                                <asp:PlaceHolder ID="place_dock3" runat="server"></asp:PlaceHolder>
                            </ContentTemplate>

                        </telerik:RadDock>
                        <telerik:RadDock ID="dock5" runat="server" DockMode="Docked">
                            <ContentTemplate>
                                <asp:PlaceHolder ID="place_dock5" runat="server"></asp:PlaceHolder>
                            </ContentTemplate>

                        </telerik:RadDock>
                    </telerik:RadDockZone>
                </td>
                <td style="vertical-align: top;">
                    <telerik:RadDockZone runat="server" ID="zone2" Orientation="Vertical">
                        <telerik:RadDock ID="dock2" runat="server" DockMode="Docked">
                            <ContentTemplate>
                                <asp:PlaceHolder ID="place_dock2" runat="server"></asp:PlaceHolder>
                            </ContentTemplate>

                        </telerik:RadDock>
                        <telerik:RadDock ID="dock4" runat="server" DockMode="Docked">
                            <ContentTemplate>
                                <asp:PlaceHolder ID="place_dock4" runat="server"></asp:PlaceHolder>
                            </ContentTemplate>

                        </telerik:RadDock>
                        <telerik:RadDock ID="dock6" runat="server" DockMode="Docked">
                            <ContentTemplate>
                                <asp:PlaceHolder ID="place_dock6" runat="server"></asp:PlaceHolder>
                            </ContentTemplate>

                        </telerik:RadDock>
                    </telerik:RadDockZone>
                </td>
                <td style="vertical-align: top;">
                    <telerik:RadDockZone runat="server" ID="zone3" Orientation="Vertical">
                        <telerik:RadDock ID="dock7" runat="server" DockMode="Docked">
                            <ContentTemplate>
                                <asp:PlaceHolder ID="place_dock7" runat="server"></asp:PlaceHolder>
                            </ContentTemplate>

                        </telerik:RadDock>
                        <telerik:RadDock ID="dock8" runat="server" DockMode="Docked">
                            <ContentTemplate>
                                <asp:PlaceHolder ID="place_dock8" runat="server"></asp:PlaceHolder>
                            </ContentTemplate>

                        </telerik:RadDock>
                        <telerik:RadDock ID="dock9" runat="server" DockMode="Docked">
                            <ContentTemplate>
                                <asp:PlaceHolder ID="place_dock9" runat="server"></asp:PlaceHolder>
                            </ContentTemplate>

                        </telerik:RadDock>
                    </telerik:RadDockZone>
                </td>
            </tr>
        </table>

    </telerik:RadDockLayout>


    <div class="commandcell">
        <a href="javascript:personalpage()">Zvolit si jinou osobní (výchozí) stránku</a>

    </div>
    <div class="commandcell" style="margin-left:30px;">
        <asp:HyperLink ID="cmdReadUpgradeInfo" runat="server" NavigateUrl="log_app_update.aspx" ImageUrl="Images/upgraded_32.png" ToolTip="Nedávno proběhla aktualizace MARKTIME. Přečti si informace o novinkách a změnách v systému."></asp:HyperLink>

        <a href="log_app_update.aspx">Historie novinek a změn v systému</a>
    </div>


    <asp:PlaceHolder ID="place99" runat="server"></asp:PlaceHolder>

</asp:Content>
