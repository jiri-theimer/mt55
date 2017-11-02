<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="admin_workflow.aspx.vb" Inherits="UI.admin_workflow" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="TreeMenu" Src="~/TreeMenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/PageHeader.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {

            $(".slidingDiv1").hide();
            $(".show_hide1").show();

            $('.show_hide1').click(function () {
                $(".slidingDiv1").slideToggle();
            });

        });

        function lp(b01id) {
            location.replace("admin_workflow.aspx?b01id=" + b01id);
        }

        function sw(url) {
            sw_master(url, "Images/setting_32.png");
        }



        function b01_edit() {
          
            var pid = <%=Me.cbxB01ID.SelectedValue%>;
            
            if (pid == "" || pid == null) {
                alert("Není vybrán záznam šablony.");
                return
            }
            sw("b01_record.aspx?pid=" + pid);

        }

        function b02_new() {
            var b01id = <%=Me.cbxB01ID.SelectedValue%>;
            sw("b02_record.aspx?pid=0&b01id=" + b01id);

        }
        function b02_edit(b02id) {
            if (b02id == "" || b02id == null) {
                alert("Není vybrán stav.");
                return
            }

            sw("b02_record.aspx?pid=" + b02id);

        }

        function b01_new(bolContextMenu) {
            sw("b01_record.aspx?pid=0");

            if (bolContextMenu == false)
                return (false);
        }

        function b65_new() {
            var b01id = <%=Me.cbxB01ID.SelectedValue%>;
         sw("b65_record.aspx?pid=0&b01id=" + b01id);
         return (false);
     }

     function b06_new(bolContextMenu) {
         var b02id = document.getElementById("<%=hidcurb02id.clientid%>").value;
         sw("b06_record.aspx?pid=0&b02id=" + b02id);
         if (bolContextMenu == false)
             return (false);
     }

     function b06_edit(b06id) {
         sw("b06_record.aspx?pid=" + b06id);

     }

     function b65_edit(b65id) {
         sw("b65_record.aspx?pid=" + b65id);

     }


     function b01_clone() {
         var pid = <%=Me.cbxB01ID.SelectedValue%>;
         if (pid == "" || pid == null) {
             alert("Není vybrána šabona.");
             return
         }
         sw("b01_clone.aspx?pid=" + pid);

     }

    

     function hardrefresh(pid, flag) {
         if (flag == "b01-save") {
             
             document.getElementById("<%=hidcurb02id.clientid%>").value = "";
             document.getElementById("<%=hidcurb06id.clientid%>").value = "";
         }

         if (flag == "b02-save") {
             document.getElementById("<%=hidcurb02id.clientid%>").value = pid;
             document.getElementById("<%=hidcurb06id.clientid%>").value = "";
         }

         if (flag == "b06-save")
             document.getElementById("<%=hidcurb06id.clientid%>").value = pid;


        document.getElementById("<%=hidcurflag.clientid%>").value = flag;
         <%=Me.ClientScript.GetPostBackEventReference(Me.cmdRefreshOnBehind, "", False)%>;

     }

        function b02_click(b02id) {
            if (b02id == document.getElementById("<%=hidcurb02id.clientid%>").value)
                return;

            document.getElementById("<%=hidcurb02id.clientid%>").value = b02id;
            document.getElementById("<%=hidcurflag.clientid%>").value = "b02-change";
            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdRefreshOnBehind, "", False)%>;

        }


        function b06_click(b06id) {
            if (b06id == document.getElementById("<%=hidcurb06id.clientid%>").value)
                return;

            document.getElementById("<%=hidcurb06id.clientid%>").value = b06id;
            document.getElementById("<%=hidcurflag.clientid%>").value = "b06-change";
            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdRefreshOnBehind, "", False)%>;

        }


    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <div class="gridheader">
        <table width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td style="width: 20px;">
                    <asp:Image runat="server" ID="imgFormHeader" Width="16px" Height="16px" ImageUrl="Images/workflow.png" />
                </td>
                <td>
                    <asp:Label ID="lblFormHeader" runat="server" CssClass="framework_header_span" Style="margin-left: 10px;"></asp:Label>
                </td>

                <td>
                    <span>Pracovat s workflow šablonou:</span>
                    <asp:DropDownList ID="cbxB01ID" DataTextField="b01Name" DataValueField="pid" runat="server" AutoPostBack="true" Style="min-width: 200px;background-color:yellow;"></asp:DropDownList>
                </td>
                <td align="right">
                    <button type="button" id="cmdMore" class="show_hide1">
                        Nastavení
        <img src="Images/arrow_down.gif" />
                    </button>
                    <button type="button" onclick="b02_new()">Nový stav</button>
                    <button type="button" onclick="b01_edit()">Hlavička šablony</button>
                    <asp:Button ID="cmdNew" runat="server" Text="Nová šablona" CssClass="cmd" OnClientClick="return b01_new(false)" />

                    <asp:Button ID="cmdRefresh" runat="server" Text="Obnovit" CssClass="cmd" />

                </td>
            </tr>
        </table>
        <div class="slidingDiv1">
            <div class="content-box3">
                <div class="title">Různá nastavení</div>
                <div class="content">

                    <div class="div6">

                        <asp:CheckBox ID="chkIncludeNonActual" runat="server" AutoPostBack="true" Text="Zobrazovat i neplatné stavy a kroky" />
                    </div>
                    <div class="div6">
                        <asp:Button ID="cmdGenerateDump" runat="server" Text="Vyexportovat workflow šablonu do XML" CssClass="cmd" />
                    </div>




                    <div class="content-box2">
                        <div class="title">Importovat šablonu z XML souboru</div>
                        <div class="content">
                            <telerik:RadUpload ID="upload1" runat="server" InputSize="30" InitialFileInputsCount="1" RenderMode="Auto" Skin="Default" ControlObjectsVisibility="None" AllowedFileExtensions="xml" MaxFileInputsCount="1">
                                <Localization Add="Přidat" Delete="Odstranit" Select="Vybrat XML" Remove="Odstranit" />
                            </telerik:RadUpload>

                            <asp:Button ID="cmdImportXML" runat="server" Text="Naimportovat šablonu z XML" CssClass="cmd" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <p></p>
    <table>
        <tr valign="top">
            <td style="min-width: 300px;">
                <uc:TreeMenu ID="tree1" runat="server" SingleExpandPath="false"></uc:TreeMenu>
            </td>
            <td>
                <asp:Panel ID="panB02Rec" runat="server">
                    <div class="gridheader" style="text-align: center;">
                        <asp:HyperLink ID="ab02name" runat="server" Font-Bold="true"></asp:HyperLink>
                        <asp:Label ID="b02ident" runat="server" CssClass="val" Style="margin-left: 20px;"></asp:Label>
                    </div>


                    <asp:Repeater ID="rpB10" runat="server">
                        <ItemTemplate>
                            <div style="padding: 6px;">
                                <img src="Images/todo.gif" alt="Akce" />
                                <asp:Label ID="b09name" runat="server"></asp:Label>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <div class="div6">
                        <asp:Button ID="cmdNewB06" runat="server" Text="Nový krok v rámci stavu" CssClass="cmd" OnClientClick="return b06_new(false)" />
                    </div>

                    <asp:Repeater ID="rpB02_B06" runat="server">
                        <ItemTemplate>
                            <div style="padding: 6px;">
                                <asp:Image ID="imgB06" runat="server" ImageUrl="Images/bullet1.gif" />
                                <asp:HyperLink ID="b06name" runat="server"></asp:HyperLink>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                </asp:Panel>
            </td>
        </tr>
    </table>


    <asp:Panel ID="panB65" runat="server">
        <uc:pageheader ID="ph1" runat="server" Text="Šablony notifikačních zpráv" IsInForm="true" />
        <div>
            <asp:Button ID="cmdNewB65" runat="server" Text="Nová notifikační zpráva" CssClass="cmd" OnClientClick="return b65_new()" />
        </div>
        <table cellpadding="4" cellspacing="3">
            <asp:Repeater ID="rpB65" runat="server">
                <ItemTemplate>
                    <tr>
                        <td>
                            <img src="Images/messages.png" alt="Notifikační zpráva" />
                        </td>
                        <td>
                            <asp:HyperLink ID="b65name" runat="server"></asp:HyperLink>
                        </td>
                        <td>
                            <asp:Label ID="b65MessageSubject" runat="server" Style="font-style: italic;"></asp:Label>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </asp:Panel>


    <asp:LinkButton ID="cmdRefreshOnBehind" runat="server" Text="refreshonbehind" Style="display: none;"></asp:LinkButton>

    <asp:HiddenField ID="hidcurb02id" runat="server" />
    <asp:HiddenField ID="hidcurb06id" runat="server" />
    <asp:HiddenField ID="hidcurx29id" runat="server" />
    <asp:HiddenField ID="hidcurflag" runat="server" />
</asp:Content>

