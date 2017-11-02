<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="import_object.aspx.vb" Inherits="UI.import_object" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="import_object_item" Src="~/import_object_item.ascx" %>
<%@ Register TagPrefix="uc" TagName="fileupload" Src="~/fileupload.ascx" %>
<%@ Register TagPrefix="uc" TagName="fileupload_list" Src="~/fileupload_list.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(window).load(function () {
            <%If Me.hidPopupUrl.Value <> "" Then%>
            sw_everywhere("<%=me.hidPopupUrl.value%>", "", true);
            <%End If%>


        })


        function repeat_popup() {
            sw_everywhere("<%=me.hidPopupUrl.value%>", "", true);
        }

        function sw_local(url, iconUrl, is_maximize) {
            sw_master(url, iconUrl, is_maximize)

        }

        function hardrefresh(pid, flag) {
            //nic

        }


        function close_curtab() {

            window.open('', '_parent', '');
            window.close();

        }

        function project_OnClientItemsRequesting(sender, eventArgs) {
            var context = eventArgs.get_context();
            var combo = sender;

            if (combo.get_value() == "")
                context["filterstring"] = eventArgs.get_text();
            else
                context["filterstring"] = "";

            context["j03id"] = "<%=Master.Factory.SysUser.PID%>";
            context["j02id_explicit"] = "<%=Master.Factory.SysUser.j02ID%>";
            context["flag"] = "searchbox";
        }
        function contact_OnClientItemsRequesting(sender, eventArgs) {
            var context = eventArgs.get_context();
            var combo = sender;

            if (combo.get_value() == "")
                context["filterstring"] = eventArgs.get_text();
            else
                context["filterstring"] = "";

            context["j03id"] = "<%=Master.Factory.SysUser.PID%>";
            context["flag"] = "searchbox";
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div style="background-color: white; padding: 10px;">
        <table cellpadding="5">
            <tr>
                <td>
                    <img src="Images/outlook_32.png" />
                </td>
                <td>
                    <asp:Label ID="lblTopHeader" runat="server" CssClass="framework_header_span" Text="Import z MS-OUTLOOK..."></asp:Label>
                    <button type="button" id="cmdPopup" runat="server" onclick="repeat_popup()" visible="false">Pokračovat</button>
                    <button type="button" id="Button1" runat="server" onclick="close_curtab()" style="margin-left: 100px;">Zavřít</button>
                </td>

            </tr>
        </table>

        <telerik:RadTabStrip ID="tabs1" runat="server" MultiPageID="RadMultiPage1" ShowBaseLine="true">
            <Tabs>
                <telerik:RadTab Text="Vytvořit dokument" Selected="true" Value="prefix" ImageUrl="Images/notepad.png"></telerik:RadTab>
                <telerik:RadTab Text="Nebo přiřadit jako poštu/komentář/přílohu" Value="b07" ImageUrl="Images/imap.png"></telerik:RadTab>
            </Tabs>
        </telerik:RadTabStrip>

        <telerik:RadMultiPage ID="RadMultiPage1" runat="server">
            <telerik:RadPageView ID="prefix" runat="server" Selected="true">
                <asp:Panel ID="panO23" runat="server" Visible="false">
                    <div class="div6">
                        <span>Vyberte typ dokumentu</span>
                    </div>
                    <asp:RadioButtonList ID="x18ID" runat="server" AutoPostBack="true" RepeatDirection="Vertical" DataValueField="pid" DataTextField="x18Name" CellPadding="6"></asp:RadioButtonList>
                </asp:Panel>
            </telerik:RadPageView>
            <telerik:RadPageView ID="b07" runat="server">
                <div class="content-box2">
                    <div class="title">
                        <asp:Literal ID="objectName" runat="server" Text="Zdroj"></asp:Literal>
                    </div>
                    <div class="content">
                        <uc:import_object_item ID="io1" runat="server"></uc:import_object_item>
                    </div>

                </div>
               




                <div class="content-box2" style="margin-top: 20px;">
                    <div class="title">
                        Cíl
                        <asp:Button ID="cmdSaveB07" runat="server" Font-Size="Large" Text="Uložit změny" CssClass="cmd" />
                    </div>
                    <div class="content">
                        <div>
                            <table>
                                <tr>
                                    <td>
                                        <asp:RadioButtonList ID="opgSearch" runat="server" AutoPostBack="true" RepeatDirection="Vertical">
                                            <asp:ListItem Text="Projekt" Value="p41" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="Klient" Value="p28"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                    <td>
                                        <telerik:RadComboBox ID="search_p41" runat="server" DropDownWidth="400" EnableTextSelection="true" MarkFirstMatch="true" EnableLoadOnDemand="true" Text="Hledat projekt..." Width="400px" OnClientItemsRequesting="project_OnClientItemsRequesting">
                                            <WebServiceSettings Method="LoadComboData" Path="~/Services/project_service.asmx" UseHttpGet="false" />
                                        </telerik:RadComboBox>
                                        <telerik:RadComboBox ID="search_p28" runat="server" DropDownWidth="400" EnableTextSelection="true" MarkFirstMatch="true" EnableLoadOnDemand="true" Text="Hledat klienta..." Width="400px" OnClientItemsRequesting="contact_OnClientItemsRequesting">
                                            <WebServiceSettings Method="LoadComboData" Path="~/Services/contact_service.asmx" UseHttpGet="false" />
                                        </telerik:RadComboBox>
                                    </td>
                                </tr>
                            </table>

                        </div>
                        
                        <fieldset style="width:600px;">
                            <legend>Uložit obsah jako</legend>
                            <asp:RadioButtonList ID="opgBodyFormat" runat="server" AutoPostBack="true" RepeatDirection="Vertical">
                            <asp:ListItem Text="Poštovní zpráva 1:1" Value="3" Selected="true"></asp:ListItem>
                            <asp:ListItem Text="Textový komentář s přílohami (možnost upravit)" Value="1"></asp:ListItem>
                            <asp:ListItem Text="HTML komentář s přílohami (bez úpravy komentáře)" Value="2"></asp:ListItem>
                        </asp:RadioButtonList>
                        </fieldset>
                        <asp:TextBox ID="b07Value_Mail" runat="server" Width="90%" ToolTip="Název/předmět zprávy" style="background-color: #ffffcc;"></asp:TextBox>
                        <asp:TextBox ID="b07Value" runat="server" TextMode="MultiLine" Style="width: 100%; height: 300px; font-family: 'Courier New';"></asp:TextBox>
                        <asp:Panel ID="panBodyHTML" runat="server" Style="background-color: lightskyblue">
                            <asp:Literal ID="b07BodyHTML" runat="server"></asp:Literal>
                        </asp:Panel>
                        <div style="margin-top: 20px; width: 100%;"></div>
                        <uc:fileupload_list ID="uploadlist1" runat="server" />
                        <uc:fileupload ID="upload1" runat="server" InitialFileInputsCount="1" EntityX29ID="b07Comment" />

                        
                    </div>


                </div>

            </telerik:RadPageView>
        </telerik:RadMultiPage>




        <asp:Panel ID="panObject" runat="server" CssClass="content-box2" Style="margin-top: 20px;">
            <div class="title">
                <img src="Images/outlook.png" />
                Zdrojový objekt
            </div>
            <div class="content">
                <table cellpadding="8">
                    <tr>
                        <td>Název:
                        </td>
                        <td>
                            <asp:Label ID="Subject" runat="server" CssClass="valbold"></asp:Label>
                        </td>

                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:HyperLink ID="linkMSG" runat="server" Text="Otevřít v MS-OUTLOOK"></asp:HyperLink>
                        </td>
                    </tr>
                </table>
                <div class="bigtext">
                    <asp:Literal ID="Body" runat="server"></asp:Literal>
                </div>

            </div>
        </asp:Panel>
    </div>

    <asp:HiddenField ID="hidPopupUrl" runat="server" />
    <asp:HiddenField ID="hidPrefix" runat="server" />
    <asp:HiddenField ID="hidGUID" runat="server" />
    <asp:HiddenField ID="hidP41ID" runat="server" />


</asp:Content>
