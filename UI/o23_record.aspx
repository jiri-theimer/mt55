<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="o23_record.aspx.vb" Inherits="UI.o23_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="fileupload" Src="~/fileupload.ascx" %>
<%@ Register TagPrefix="uc" TagName="fileupload_list" Src="~/fileupload_list.ascx" %>
<%@ Register TagPrefix="uc" TagName="import_object_item" Src="~/import_object_item.ascx" %>
<%@ Register TagPrefix="uc" TagName="mytags" Src="~/mytags.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function cbx1_OnClientSelectedIndexChanged(sender, eventArgs) {
            var combo = sender;
            var pid = combo.get_value();

        }
        function cbx1_OnClientItemsRequesting(sender, eventArgs) {
            var context = eventArgs.get_context();
            var combo = sender;

            if (combo.get_value() == "")
                context["filterstring"] = eventArgs.get_text();
            else
                context["filterstring"] = "";

            context["qry_value"] = "<%=Me.cbxType.SelectedValue%>";
            context["qry_field"] = "<%=Left(Me.cbxType.DataTextField, 3) + "id"%>";
            context["j03id"] = "<%=Master.Factory.SysUser.PID%>";
            context["flag"] = "search4o23";

            <%If Me.CurrentX29ID = BO.x29IdEnum.p41Project Then%>
            context["j02id_explicit"] = "<%=Master.Factory.SysUser.j02ID%>";
            <%End If%>
        }

        function file_preview(prefix, pid) {
            ///náhled na soubor            
            sw_everywhere("fileupload_preview.aspx?prefix=" + prefix + "&pid=" + pid, "Images/attachment.png", true);

        }

        function chklist(x16id,ctl) {
            ///checkbox-list
            var x18id = document.getElementById("<%=hidX18ID.clientID%>").value;
            var combo = $find(ctl);
            var val = combo.get_text();
           
            sw_everywhere("freefields_checkboxlist.aspx?prefix=x16&pid=" + x16id+"&x18id="+x18id+"&ctl="+ctl+"&value="+val, "Images/setting.png", true);
        }

        function hardrefresh(pid, flag, par1) {            
            var ctlID = flag;            
            var combo = $find(ctlID);
            
            combo.set_text(par1);
            
        }
    </script>
    <asp:PlaceHolder ID="place_x18JavascriptFile" runat="server"></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="panX20" runat="server" CssClass="content-box2">
        <div class="title">
            Vazby [<asp:Label ID="x18Name" runat="server"></asp:Label>]
        </div>
        <div class="content">
            <div>
                <asp:RadioButtonList ID="opgX20ID" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" DataValueField="x20ID" DataTextField="BindName"></asp:RadioButtonList>
            </div>
            <div>
                <asp:DropDownList ID="cbxType" runat="server" DataValueField="pid" AutoPostBack="true" Visible="false" ToolTip="Filtrovat rozsah hledaných záznamů"></asp:DropDownList>
                <telerik:RadComboBox ID="cbx1" runat="server" DropDownWidth="600px" EnableTextSelection="true" MarkFirstMatch="true" EnableLoadOnDemand="true" Text="Hledat..." Width="500px" OnClientItemsRequesting="cbx1_OnClientItemsRequesting" AutoPostBack="true">
                    <WebServiceSettings Method="LoadComboData" UseHttpGet="false" />
                </telerik:RadComboBox>

            </div>
        </div>


    </asp:Panel>

    <div style="overflow: auto; max-height: 200px; width: 700px;">
        <table cellpadding="5" cellspacing="2">
            <asp:Repeater ID="rpX19" runat="server">
                <ItemTemplate>
                    <tr class="trHover">
                        <td style="width: 140px;">
                            <asp:Label ID="Entity" runat="server" CssClass="lbl"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="RecordAlias" runat="server" CssClass="valbold"></asp:Label>
                            <asp:ImageButton ID="del" runat="server" CommandName="delete" ImageUrl="Images/delete.png" ToolTip="Odstranit vazbu" CssClass="button-link" />
                            <asp:HiddenField ID="p85id" runat="server" />
                        </td>
                    </tr>

                </ItemTemplate>
            </asp:Repeater>
        </table>
    </div>

    <table cellpadding="5" cellspacing="2">
        <tr>
            <td style="width: 140px;">
                <asp:Label ID="lblX23ID" Text="Zdroj:" runat="server" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="x23ID" runat="server" DataTextField="x23Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>

            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název:"></asp:Label></td>
            <td>
                <asp:TextBox ID="o23Name" runat="server" Style="width: 400px;"></asp:TextBox>

                <asp:Label ID="lblOrdinary" Text="Index pořadí:" runat="server" CssClass="lbl"></asp:Label>
                <telerik:RadNumericTextBox ID="o23Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblo23Code" runat="server" CssClass="lbl" Text="Kód:"></asp:Label></td>
            <td>
                <asp:TextBox ID="o23Code" runat="server"></asp:TextBox>
                <asp:Button ID="cmdChangeCode" runat="server" CssClass="cmd" Text="Změnit kód ručně" Visible="false" />
                <asp:Label ID="o23ArabicCode" runat="server"></asp:Label>
            </td>
        </tr>



    </table>

    <asp:Panel ID="panX16" runat="server">        
        <table cellpadding="5" cellspacing="2">
            <asp:Repeater ID="rpX16" runat="server">
                <ItemTemplate>
                    <tr style="vertical-align: top;">

                        <td style="min-width: 140px;">
                            <asp:HiddenField ID="x16IsEntryRequired" runat="server" />

                            <asp:Label ID="x16Name" runat="server" CssClass="lbl"></asp:Label>

                        </td>
                        <td>

                            <asp:TextBox ID="txtFF_Text" runat="server"></asp:TextBox>
                            <telerik:RadNumericTextBox ID="txtFF_Number" runat="server"></telerik:RadNumericTextBox>
                            <asp:CheckBox ID="chkFF" runat="server" ForeColor="Black" />

                            <telerik:RadDateTimePicker ID="txtFF_Date" runat="server" Width="130px">
                                <DateInput ID="DateInput1" DateFormat="d.M.yyyy" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                                <TimePopupButton Visible="false" />
                                <TimeView Interval="60" runat="server"></TimeView>
                            </telerik:RadDateTimePicker>
                            <telerik:RadComboBox ID="cbxFF" runat="server" ShowToggleImage="false" ShowDropDownOnTextboxClick="true" MarkFirstMatch="true" Width="400px" OnClientSelectedIndexChanged="cbxFF_OnClientSelectedIndexChanged">                               
                            </telerik:RadComboBox>
                            <button type="button" id="cmdChklist" runat="server" visible="false">...</button>

                            <asp:HiddenField runat="server" ID="x16Field" />
                            <asp:HiddenField runat="server" ID="x16ID" />
                            <asp:HiddenField runat="server" ID="hidType" />


                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>

        </table>

    </asp:Panel>

    <asp:Panel ID="panColors" runat="server">
        <table>
            <tr>
                <td>
                    <asp:Label ID="Label2" Text="Barva pozadí:" runat="server" CssClass="lbl"></asp:Label>

                </td>
                <td>
                    <telerik:RadColorPicker ID="o23BackColor" runat="server" CurrentColorText="Vybraná barva" NoColorText="Bez barvy" ShowIcon="true" Preset="Standard">
                        <telerik:ColorPickerItem Value="#F0F8FF"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#FAEBD7"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#00FFFF"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#7FFFD4"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#F0FFFF"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#F5F5DC"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#FFE4C4"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#00FFFF"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#FFFAF0"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#F8F8FF"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#FFD700"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#F0E68C"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#E6E6FA"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#FFB6C1"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#FFA500"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#AFEEEE"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#FFDAB9"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#87CEEB"></telerik:ColorPickerItem>
                        <telerik:ColorPickerItem Value="#FF6347"></telerik:ColorPickerItem>
                    </telerik:RadColorPicker>

                </td>
                <td>
                    <asp:Label ID="Label1" Text="Barva písma:" runat="server" CssClass="lbl"></asp:Label>
                </td>
                <td>
                    <telerik:RadColorPicker ID="o23ForeColor" runat="server" CurrentColorText="Vybraná barva" NoColorText="Bez barvy" ShowIcon="true" Preset="Standard">
                    </telerik:RadColorPicker>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="panHtmlEditor" runat="server" Visible="false">

        <telerik:RadEditor ID="o23HtmlContent" ForeColor="Black" Font-Names="Verdana" runat="server" Width="98%" Height="500px" ToolbarMode="Default" Language="cs-CZ" NewLineMode="Br" ContentAreaMode="Div">
            <Content>
     Text zprávy
            </Content>
            <Tools>
                <telerik:EditorToolGroup>
                    <telerik:EditorTool Name="Print" />
                    <telerik:EditorTool Name="ToggleScreenMode" />
                    <telerik:EditorSeparator />

                    <telerik:EditorTool Name="Undo" />
                    <telerik:EditorTool Name="Redo" />
                    <telerik:EditorSeparator />

                    <telerik:EditorTool Name="Cut" />
                    <telerik:EditorTool Name="Copy" />
                    <telerik:EditorTool Name="Paste" />
                    <telerik:EditorSeparator />

                    <telerik:EditorTool Name="FontName" />
                    <telerik:EditorTool Name="FontSize" />

                    <telerik:EditorTool Name="Underline" />
                    <telerik:EditorTool Name="Bold" />
                    <telerik:EditorTool Name="Italic" />
                    <telerik:EditorTool Name="ForeColor" />
                    <telerik:EditorTool Name="BackColor" />
                    <telerik:EditorSeparator />

                    <telerik:EditorTool Name="FormatBlock" />
                    <telerik:EditorTool Name="JustifyLeft" />
                    <telerik:EditorTool Name="JustifyCenter" />
                    <telerik:EditorTool Name="JustifyRight" />
                    <telerik:EditorTool Name="JustifyFull" />
                    <telerik:EditorTool Name="JustifyNone" />

                    <telerik:EditorTool Name="InsertUnorderedList" />
                    <telerik:EditorTool Name="InsertOrderedList" />

                    <telerik:EditorTool Name="InsertParagraph" />
                    <telerik:EditorTool Name="Indent" />
                    <telerik:EditorTool Name="Outdent" />


                    <telerik:EditorTool Name="InsertHorizontalRule" />

                    <telerik:EditorTool Name="InsertTable" />
                    <telerik:EditorTool Name="InsertLink" />

                    <telerik:EditorSeparator />
                    <telerik:EditorTool Name="PasteFromWord" />
                    <telerik:EditorTool Name="PasteFromWordNoFontsNoSizes" />
                    <telerik:EditorTool Name="PasteHtml" />


                </telerik:EditorToolGroup>
            </Tools>

        </telerik:RadEditor>
    </asp:Panel>

    <div class="div6" style="clear: both; margin-top: 20px;">
        <asp:Label ID="lblOwner" runat="server" Text="Vlastník záznamu:" CssClass="lblReq" Style="padding-right: 30px;"></asp:Label>
        <uc:person ID="j02ID_Owner" runat="server" Width="300px" Flag="all" />
    </div>
    <uc:mytags ID="tags1" ModeUi="1" Prefix="o23" runat="server" />

    <div class="div6">
        <asp:CheckBox ID="o23IsEncrypted" runat="server" Text="Obsah zašifrovat a ochránit heslem" AutoPostBack="true" />
    </div>
    <asp:Panel ID="panPassword" runat="server" Style="padding: 10px;">
        <table cellpadding="5">
            <tr>
                <td>
                    <asp:Label ID="lblPassword" runat="server" CssClass="lbl" Text="Heslo:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="o23password" runat="server" Style="width: 130px;" TextMode="Password"></asp:TextBox>
                </td>
                <td>
                    <asp:Label ID="lblVerify" runat="server" CssClass="lbl" Text="Ověření:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtVerify" runat="server" Style="width: 130px;" TextMode="Password"></asp:TextBox>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <uc:import_object_item ID="io1" runat="server" Visible="false"></uc:import_object_item>
    <asp:Panel ID="panUpload" runat="server" CssClass="content-box2">
        <div class="title">Nahrát přílohy dokumentu</div>
        <div class="content">
                    
            <uc:fileupload ID="upload1" runat="server" InitialFileInputsCount="1" EntityX29ID="b07Comment" />

            <uc:fileupload_list ID="uploadlist1" runat="server" />

            <div class="div6">
            <asp:HyperLink ID="filesPreview" runat="server" Text="<hr><img src='Images/attachment.png'/> Náhled na již uložené přílohy dokumentu" Visible="false"></asp:HyperLink>
            </div>
        </div>
    </asp:Panel>


    <asp:HiddenField ID="hidX18ID" runat="server" />
    <asp:HiddenField ID="hidX29ID" runat="server" />
    <asp:HiddenField ID="hidGUID_x19" runat="server" />
    <asp:HiddenField ID="hidJavascriptFile" runat="server" />

    <telerik:RadCalendar ID="SharedCalendar" runat="server" EnableMultiSelect="False" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
        <SpecialDays>
            <telerik:RadCalendarDay Repeatable="Today" ItemStyle-BackColor="SkyBlue"></telerik:RadCalendarDay>
        </SpecialDays>
    </telerik:RadCalendar>

    <asp:HiddenField ID="hidx18CalendarFieldStart" runat="server" />

    <asp:HiddenField ID="hidx18CalendarFieldEnd" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
    <script type="text/javascript">
        function cbxFF_OnClientSelectedIndexChanged(combo, eventArgs) {
            var item = eventArgs.get_item();
            //alert(item.get_text());
            //alert(combo.get_id());
        }
    </script>
</asp:Content>


