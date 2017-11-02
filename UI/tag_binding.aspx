<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="tag_binding.aspx.vb" Inherits="UI.tag_binding" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function requesting(sender, eventArgs) {
            var context = eventArgs.get_context();

            context["prefix"] = document.getElementById("<%=hidPrefix.ClientID%>").value;
        }

        function entryAdding(sender, eventArgs) {
            if (eventArgs.get_entry().get_value() == "") {
                eventArgs.set_cancel(true);
            }

        }

        function tags1_showall() {
            var autoComplete = $find("<%= tags1.ClientID%>")
            autoComplete.query("top100-" + document.getElementById("<%=hidPrefix.ClientID%>").value);

        }
        function cbxFind_OnClientItemsRequesting(sender, eventArgs) {
            var context = eventArgs.get_context();
            var combo = $find("<%= cbxFind.ClientID%>");

            context["filterstring"] = eventArgs.get_text();


            context["prefix"] = "all";

        }
        function close_and_refresh() {
            var autoComplete = $find("<%= tags1.ClientID%>");
            var o51ids = "";
            var entriesCount = autoComplete.get_entries().get_count();
            if (entriesCount == 0) {
                //nic nebylo vybráno
            }
            else {
                for (var i = 0; i < entriesCount; i++) {
                    o51ids = o51ids + "," + autoComplete.get_entries().getEntry(i).get_value();
                }

                o51ids = o51ids.substr(1, o51ids.length - 1);
            }
            window.parent.hardrefresh_mytags(o51ids);
            window.close();
        }
        function trydel() {

            if (confirm("Opravdu nenávratně odstranit tento štítek?")) {
                return (true);
            }
            else {
                return (false);
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:RadioButtonList ID="opgMulti" runat="server" RepeatDirection="Vertical" Visible="false">
        <asp:ListItem Text="Vybrané štítky přidat" Value="1"></asp:ListItem>
        <asp:ListItem Text="Vybranými štítky nahradit stávající" Value="2" Selected="true"></asp:ListItem>
        <asp:ListItem Text="U záznamů vyčistit oštítkování" Value="3"></asp:ListItem>
    </asp:RadioButtonList>
    <div style="padding: 10px;">

        <telerik:RadAutoCompleteBox ID="tags1" runat="server" RenderMode="Lightweight" EmptyMessage="Napište pár písmen k dohledání štítku" Width="450px" OnClientEntryAdding="entryAdding" OnClientRequesting="requesting">
            <WebServiceSettings Method="LoadTokenData" Path="~/Services/tag_service.asmx" />
            <Localization ShowAllResults="Zobrazit všechny výsledky" RemoveTokenTitle="Vyjmout štítek z výběru" />

        </telerik:RadAutoCompleteBox>
        <button type="button" onclick="tags1_showall()">Rozbalit vše</button>
    </div>

    <asp:Panel ID="panCreate" runat="server" CssClass="content-box2" Style="width: 450px; padding: 10px; margin-top: 50px;">
        <div class="title">
            <img src="Images/new.png" />
            Vytvořit nový štítek
            <asp:Button ID="cmdCreate" runat="server" CssClass="cmd" Text="Uložit a přidat" />
        </div>
        <div class="content">
            <span>Název štítku:</span>
            <asp:TextBox ID="txtCreate" runat="server" Width="400px"></asp:TextBox>
            <div>
                <asp:CheckBox ID="chkCreate4All" runat="server" CssClass="chk" Text="Použitelný pro všechny entity" Checked="true" AutoPostBack="true" />
                <telerik:RadComboBox ID="cbxScope" runat="server" CheckBoxes="true" Visible="false">
                    <Items>
                        <telerik:RadComboBoxItem Text="Projekty" Value="p41" />
                        <telerik:RadComboBoxItem Text="Klienti" Value="p28" />
                        <telerik:RadComboBoxItem Text="Úkoly" Value="p56" />
                        <telerik:RadComboBoxItem Text="Osoby" Value="j02" />
                        <telerik:RadComboBoxItem Text="Worksheet" Value="p31" />
                        <telerik:RadComboBoxItem Text="Faktury" Value="p91" />
                        <telerik:RadComboBoxItem Text="Dokumenty" Value="o23" />
                        <telerik:RadComboBoxItem Text="Zálohy" Value="p90" />
                    </Items>
                    <Localization AllItemsCheckedString="Všechny položky zaškrtnuty" ItemsCheckedString="x zaškrtnuto" />
                </telerik:RadComboBox>
            </div>
            <table>
                <tr valign="top">
                    <td>
                        <div>Barva pozadí:</div>
                        <div>
                            <telerik:RadColorPicker ID="colBackColor" runat="server" CurrentColorText="Vybraná barva" NoColorText="bez barvy" ShowIcon="false" Preset="none" RenderMode="Lightweight">
                                <telerik:ColorPickerItem Value="#FFFFFF"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#C0C0C0"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#808080"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#000000"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#FF0000"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#800000"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#FFFF00"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#808000"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#00FF00"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#008000"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#00FFFF"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#008080"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#0000FF"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#000080"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#FF00FF"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#800080"></telerik:ColorPickerItem>                                    
                            </telerik:RadColorPicker>
                        </div>
                    </td>
                    <td>
                        <div>Barva písma:</div>
                        <div>
                            <telerik:RadColorPicker ID="colForeColor" runat="server" CurrentColorText="Vybraná barva" NoColorText="bez barvy" ShowIcon="false" Preset="none" RenderMode="Lightweight">
                                <telerik:ColorPickerItem Value="#008080"></telerik:ColorPickerItem>
                                <telerik:ColorPickerItem Value="#FFFFFF"></telerik:ColorPickerItem>
                                <telerik:ColorPickerItem Value="#FF0000"></telerik:ColorPickerItem>
                                <telerik:ColorPickerItem Value="#FFFF00"></telerik:ColorPickerItem>
                                <telerik:ColorPickerItem Value="#0000FF"></telerik:ColorPickerItem>
                                <telerik:ColorPickerItem Value="#800000"></telerik:ColorPickerItem>
                                <telerik:ColorPickerItem Value="#FF00FF"></telerik:ColorPickerItem>
                            </telerik:RadColorPicker>
                        </div>
                    </td>
                </tr>
            </table>



        </div>
    </asp:Panel>

    <asp:Panel ID="panEdidt" runat="server" CssClass="content-box2" Style="width: 450px; padding: 10px; margin-top: 50px;">
        <div class="title">
            <img src="Images/edit.png" />
            Upravit nebo odstranit vybraný štítek
            <asp:Button ID="cmdSave" runat="server" CssClass="cmd" Text="Uložit změny" />
        </div>
        <div class="content">
            <telerik:RadComboBox ID="cbxFind" runat="server" AutoPostBack="true" Text="Najít štítek..." Width="350px" OnClientItemsRequesting="cbxFind_OnClientItemsRequesting" EnableTextSelection="true" MarkFirstMatch="true" EnableLoadOnDemand="true">
                <WebServiceSettings Method="LoadComboData" Path="~/Services/tag_service.asmx" UseHttpGet="false" />
            </telerik:RadComboBox>

            <asp:Panel ID="panRecord" runat="server" CssClass="div6">
                <asp:TextBox ID="o51Name" runat="server" Width="300px"></asp:TextBox>
                <div>
                    <asp:CheckBox ID="o51ScopeFlag" runat="server" CssClass="chk" Text="Použitelný pro všechny entity" Checked="true" AutoPostBack="true" />

                </div>
                <asp:Panel ID="panEntities" runat="server">
                    <asp:CheckBox ID="o51IsP41" Text="Projekty" runat="server" />
                    <asp:CheckBox ID="o51IsP28" Text="Klienti" runat="server" />
                    <asp:CheckBox ID="o51IsP56" Text="Úkoly" runat="server" />
                    <asp:CheckBox ID="o51IsP91" Text="Faktury" runat="server" />
                    <asp:CheckBox ID="o51IsP31" Text="Worksheet" runat="server" />
                    <asp:CheckBox ID="o51IsO23" Text="Dokumenty" runat="server" />
                    <asp:CheckBox ID="o51IsJ02" Text="Osoby" runat="server" />
                    <asp:CheckBox ID="o51IsP90" Text="Zálohy" runat="server" />
                </asp:Panel>

                <table>
                    <tr valign="top">
                        <td>
                            <div>Barva pozadí:</div>
                            <div>
                                <telerik:RadColorPicker ID="o51BackColor" runat="server" CurrentColorText="Vybraná barva" NoColorText="bez barvy" ShowIcon="false" Preset="none" RenderMode="Lightweight">
                                    <telerik:ColorPickerItem Value="#FFFFFF"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#C0C0C0"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#808080"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#000000"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#FF0000"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#800000"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#FFFF00"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#808000"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#00FF00"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#008000"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#00FFFF"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#008080"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#0000FF"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#000080"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#FF00FF"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#800080"></telerik:ColorPickerItem>                                    
                                </telerik:RadColorPicker>
                            </div>
                        </td>
                        <td>
                            <div>Barva písma:</div>
                            <div>
                                <telerik:RadColorPicker ID="o51ForeColor" runat="server" CurrentColorText="Vybraná barva" NoColorText="bez barvy" ShowIcon="false" Preset="none" RenderMode="Lightweight">
                                    <telerik:ColorPickerItem Value="#008080"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#FFFFFF"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#FF0000"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#FFFF00"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#0000FF"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#800000"></telerik:ColorPickerItem>
                                    <telerik:ColorPickerItem Value="#FF00FF"></telerik:ColorPickerItem>
                                </telerik:RadColorPicker>
                            </div>
                        </td>
                    </tr>
                </table>
                <div>
                    <asp:Button ID="cmdDelete" runat="server" Text="Odstranit štítek" CssClass="cmd" OnClientClick="return trydel();" />
                </div>
            </asp:Panel>

            <asp:Label ID="Timestamp" runat="server" CssClass="timestamp"></asp:Label>
        </div>
    </asp:Panel>

    <asp:HiddenField ID="hidPrefix" runat="server" />
    <asp:HiddenField ID="hidPIDs" runat="server" />
    <asp:HiddenField ID="hidMode" runat="server" />
    <asp:HiddenField ID="hidO51IDs" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
