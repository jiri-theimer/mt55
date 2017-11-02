<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="x18_record.aspx.vb" Inherits="UI.x18_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign" Src="~/entityrole_assign.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">


        function hardrefresh(pid, flag) {
            document.getElementById("<%=HardRefreshPID.ClientID%>").value = pid;

            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdHardRefresh, "", False)%>;

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" ShowBaseLine="true">
        <Tabs>
            <telerik:RadTab Text="Vlastnosti" Selected="true" Value="core"></telerik:RadTab>
            <telerik:RadTab Text="Uživatelská pole" Value="ff"></telerik:RadTab>
            <telerik:RadTab Text="Ostatní" Value="other"></telerik:RadTab>

        </Tabs>
    </telerik:RadTabStrip>

    <telerik:RadMultiPage ID="RadMultiPage1" runat="server">
        <telerik:RadPageView ID="core" runat="server" Selected="true">
            <div class="content-box2">
                <div class="title">Povaha položek typu dokumentu</div>
                <div class="content">
                    <asp:RadioButtonList ID="x18IsManyItems" runat="server" RepeatDirection="Vertical" CellPadding="2" AutoPostBack="true">
                        <asp:ListItem Text="Dokument s formulářovými poli, přílohami a workflow šablonou (stovky i tisíce záznamů)" Value="1" Selected="true"></asp:ListItem>
                        <asp:ListItem Text="Kategorizace projektů/klientů/úkolů...maximálně desítky záznamů" Value="0"></asp:ListItem>
                    </asp:RadioButtonList>
                </div>
            </div>

            <table cellpadding="5" cellspacing="2">
                <tr>
                    <td>
                        <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Typ dokumentu:"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="x18Name" runat="server" Style="width: 400px;"></asp:TextBox>
                        <asp:Label ID="lblOrdinary" Text="Index pořadí:" runat="server" CssClass="lbl"></asp:Label>
                        <telerik:RadNumericTextBox ID="x18Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
                    </td>

                </tr>
                <tr>
                    <td>
                        <span class="lbl">Zkrácený název:</span>
                    </td>
                    <td>
                        <asp:TextBox ID="x18NameShort" runat="server"></asp:TextBox>
                    </td>
                </tr>

            </table>



            <div class="content-box2" style="margin-top: 20px;">
                <div class="title">
                    Vazba dokumentu na entity
                    <asp:DropDownList ID="x29ID_addX20" runat="server" AutoPostBack="false">
                        <asp:ListItem Text="--Vyberte entitu--" Value=""></asp:ListItem>
                        <asp:ListItem Text="Projekt" Value="141"></asp:ListItem>
                        <asp:ListItem Text="Klient" Value="328"></asp:ListItem>
                        <asp:ListItem Text="Úkol" Value="356"></asp:ListItem>
                        <asp:ListItem Text="Událost v kalendáři" Value="222"></asp:ListItem>
                        <asp:ListItem Text="Dokument" Value="223"></asp:ListItem>
                        <asp:ListItem Text="Vystavená faktura" Value="391"></asp:ListItem>
                        <asp:ListItem Text="Zálohová faktura" Value="390"></asp:ListItem>
                        <asp:ListItem Text="Osoba" Value="102"></asp:ListItem>
                        <asp:ListItem Text="Worksheet úkon" Value="331"></asp:ListItem>
                        <asp:ListItem Text="Jiný dokument" Value="223"></asp:ListItem>
                    </asp:DropDownList>

                    <asp:Button ID="cmdAddX20" runat="server" CssClass="cmd" Text="Vložit vybranou vazbu" />
                </div>
                <div class="content">
                    <table cellpadding="10">

                        <asp:Repeater ID="rpX20" runat="server">
                            <ItemTemplate>
                                <tr class="trHover" valign="top">
                                    <td>
                                        <div>
                                            <span>Vazba na entitu:</span>
                                        </div>
                                        <div>
                                            <span>Název vazby (nepovinné):</span>
                                        </div>

                                    </td>
                                    <td>
                                        <div>
                                            <asp:DropDownList ID="x29ID" runat="server" Enabled="false">
                                                <asp:ListItem Text="Projekt" Value="141"></asp:ListItem>
                                                <asp:ListItem Text="Klient" Value="328"></asp:ListItem>
                                                <asp:ListItem Text="Úkol" Value="356"></asp:ListItem>
                                                <asp:ListItem Text="Událost v kalendáři" Value="222"></asp:ListItem>
                                                <asp:ListItem Text="Dokument" Value="223"></asp:ListItem>
                                                <asp:ListItem Text="Vystavená faktura" Value="391"></asp:ListItem>
                                                <asp:ListItem Text="Zálohová faktura" Value="390"></asp:ListItem>
                                                <asp:ListItem Text="Osoba" Value="102"></asp:ListItem>
                                                <asp:ListItem Text="Worksheet úkon" Value="331"></asp:ListItem>
                                                <asp:ListItem Text="Jiný dokument" Value="223"></asp:ListItem>
                                            </asp:DropDownList>

                                        </div>
                                        <div>
                                            <asp:TextBox ID="x20Name" runat="server" Width="200px"></asp:TextBox>

                                            <asp:HiddenField ID="x29ID_EntityType" runat="server" />

                                        </div>
                                        <div>
                                            <asp:DropDownList ID="x20EntityTypePID" runat="server"></asp:DropDownList>
                                        </div>

                                    </td>

                                    <td>
                                        <div>
                                            <asp:CheckBox ID="x20IsMultiselect" runat="server" CssClass="chk" Text="Multi-Select (možnost zaškrtnout více položek najednou)" />
                                        </div>
                                        <div>
                                            <asp:CheckBox ID="x20IsEntryRequired" runat="server" CssClass="chk" Text="Povinná vazba k přiřazení" />
                                        </div>

                                        <div>
                                            <asp:CheckBox ID="x20IsClosed" runat="server" Text="Vazba uzavřena pro přiřazování" />
                                        </div>

                                    </td>
                                    <td>
                                        <div>
                                            <asp:DropDownList ID="x20EntryModeFlag" runat="server">
                                                <asp:ListItem Text="Vazbu vyplňovat odkazem na položku dokumentu (combo-list v záznamu entity)" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Vazbu vyplňovat odkazem na záznam entity (přímo v záznamu dokumentu)" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Vazbu generuje workflow stavový mechanismus" Value="3"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div>
                                            <asp:DropDownList ID="x20GridColumnFlag" runat="server">
                                                <asp:ListItem Text="Sloupec v přehledu záznamů entity" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Sloupec v přehledu dokumentů" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Sloupec v entitním přehledu i v přehledu položek" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Nezobrazovat jako sloupec" Value="4"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div>
                                            <asp:DropDownList ID="x20EntityPageFlag" runat="server">
                                                <asp:ListItem Text="Na stránce záznamu entity zobrazovat jako info" Value="1"></asp:ListItem>                                                
                                                <asp:ListItem Text="Na stránce záznamu entity zobrazovat jako odkaz" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Na stránce záznamu entity zobrazovat jako odkaz + tlačítko [Přidat]" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Na stránce záznamu entity nic nezobrazovat" Value="9"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div>
                                            <span>Pořadí:</span>
                                            <telerik:RadNumericTextBox ID="x20Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
                                        </div>
                                    </td>

                                    <td>
                                        <asp:ImageButton ID="del" runat="server" ImageUrl="Images/delete_row.png" ToolTip="Odstranit položku" CssClass="button-link" />
                                        <asp:HiddenField ID="p85id" runat="server" />
                                        <asp:HiddenField ID="x20ID" runat="server" />

                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                </div>
            </div>

            <asp:Panel ID="panX23" runat="server" CssClass="div6">
                <asp:RadioButtonList ID="opg1" runat="server" AutoPostBack="true" Visible="false" RepeatDirection="Vertical">
                    <asp:ListItem Text="Typ dokumentu bude mít vlastní zdroj položek" Value="1" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Typ dokumentu bude využívat již existující zdroj položek" Value="2"></asp:ListItem>
                </asp:RadioButtonList>

                <div style="margin-top: 20px;">
                    <asp:Label ID="lblX23ID" Text="Datový zdroj dokumentů:" runat="server" CssClass="lblReq"></asp:Label>
                    <uc:datacombo ID="x23ID" runat="server" DataTextField="x23Name" DataValueField="pid" IsFirstEmptyRow="true" AutoPostBack="true" Width="400px"></uc:datacombo>
                </div>
            </asp:Panel>



        </telerik:RadPageView>

        <telerik:RadPageView ID="ff" runat="server">
            <div class="content-box2" style="margin-top: 10px;">
                <div class="title">
                    <img src="Images/form.png" width="16px" height="16px" />
                    Rozšíření karty dokumentu o další pole
                    <asp:Button ID="cmdNewX16" runat="server" CssClass="cmd" Text="Přidat" />
                </div>
                <div class="content">
                    <table cellpadding="4">
                        <tr>
                            <th>Pole</th>
                            <th>Název</th>
                            <th></th>
                            <th>#</th>
                            <th>Možné hodnoty textového pole</th>
                            <th></th>
                        </tr>
                        <asp:Repeater ID="rpX16" runat="server">
                            <ItemTemplate>
                                <tr class="trHover" valign="top">
                                    <td>

                                        <asp:DropDownList ID="x16Field" runat="server">
                                            <asp:ListItem Text="--Obsazené pole--" Value=""></asp:ListItem>
                                            <asp:ListItem Text="Text 1" Value="o23FreeText01"></asp:ListItem>
                                            <asp:ListItem Text="Text 2" Value="o23FreeText02"></asp:ListItem>
                                            <asp:ListItem Text="Text 3" Value="o23FreeText03"></asp:ListItem>
                                            <asp:ListItem Text="Text 4" Value="o23FreeText04"></asp:ListItem>
                                            <asp:ListItem Text="Text 5" Value="o23FreeText05"></asp:ListItem>
                                            <asp:ListItem Text="Text 6" Value="o23FreeText06"></asp:ListItem>
                                            <asp:ListItem Text="Text 7" Value="o23FreeText07"></asp:ListItem>
                                            <asp:ListItem Text="Text 8" Value="o23FreeText08"></asp:ListItem>
                                            <asp:ListItem Text="Text 9" Value="o23FreeText09"></asp:ListItem>
                                            <asp:ListItem Text="Text 10" Value="o23FreeText10"></asp:ListItem>
                                            <asp:ListItem Text="Text 11" Value="o23FreeText11"></asp:ListItem>
                                            <asp:ListItem Text="Text 12" Value="o23FreeText12"></asp:ListItem>
                                            <asp:ListItem Text="Text 13" Value="o23FreeText13"></asp:ListItem>
                                            <asp:ListItem Text="Text 14" Value="o23FreeText14"></asp:ListItem>
                                            <asp:ListItem Text="Text 15" Value="o23FreeText15"></asp:ListItem>
                                            <asp:ListItem Text="Velký text (1000)" Value="o23BigText"></asp:ListItem>
                                            <asp:ListItem Text="Html editor" Value="o23HtmlContent"></asp:ListItem>
                                            <asp:ListItem Text="Číslo 1" Value="o23FreeNumber01"></asp:ListItem>
                                            <asp:ListItem Text="Číslo 2" Value="o23FreeNumber02"></asp:ListItem>
                                            <asp:ListItem Text="Číslo 3" Value="o23FreeNumber03"></asp:ListItem>
                                            <asp:ListItem Text="Číslo 4" Value="o23FreeNumber04"></asp:ListItem>
                                            <asp:ListItem Text="Číslo 5" Value="o23FreeNumber05"></asp:ListItem>
                                            <asp:ListItem Text="Datum 1" Value="o23FreeDate01"></asp:ListItem>
                                            <asp:ListItem Text="Datum 2" Value="o23FreeDate02"></asp:ListItem>
                                            <asp:ListItem Text="Datum 3" Value="o23FreeDate03"></asp:ListItem>
                                            <asp:ListItem Text="Datum 4" Value="o23FreeDate04"></asp:ListItem>
                                            <asp:ListItem Text="Datum 5" Value="o23FreeDate05"></asp:ListItem>
                                            <asp:ListItem Text="ANO/NE 1" Value="o23FreeBoolean01"></asp:ListItem>
                                            <asp:ListItem Text="ANO/NE 2" Value="o23FreeBoolean02"></asp:ListItem>
                                            <asp:ListItem Text="ANO/NE 3" Value="o23FreeBoolean03"></asp:ListItem>
                                            <asp:ListItem Text="ANO/NE 4" Value="o23FreeBoolean04"></asp:ListItem>
                                            <asp:ListItem Text="ANO/NE 5" Value="o23FreeBoolean05"></asp:ListItem>

                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="x16Name" runat="server" Width="250px"></asp:TextBox>
                                        <div>
                                            <span>Název sloupce:</span>
                                            <asp:TextBox ID="x16NameGrid" runat="server" Width="150px"></asp:TextBox>
                                        </div>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="x16IsEntryRequired" runat="server" Text="Povinné k vyplnění" />
                                        <div>
                                            <asp:CheckBox ID="x16IsGridField" runat="server" Text="Sloupec v přehledu" Checked="true" />
                                        </div>

                                    </td>
                                    <td>
                                        <telerik:RadNumericTextBox ID="x16Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="x16DataSource" runat="server" Width="600px" Height="42px" TextMode="MultiLine"></asp:TextBox>
                                        <div>
                                            <asp:CheckBox ID="x16IsFixedDataSource" runat="server" Text="Okruh hodnot je zafixován" />
                                            <span>Formát:</span><asp:TextBox ID="x16Format" runat="server"></asp:TextBox>
                                        </div>
                                        <div>
                                            <span>Šířka pole:</span>
                                            <asp:TextBox ID="x16TextboxWidth" runat="server" Width="40px"></asp:TextBox>(px)
                                            <span>Výška pole:</span>
                                            <asp:TextBox ID="x16TextboxHeight" runat="server" Width="40px"></asp:TextBox>(px)
                                        </div>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="del" runat="server" ImageUrl="Images/delete_row.png" ToolTip="Odstranit položku" CssClass="button-link" />
                                        <asp:HiddenField ID="p85id" runat="server" />
                                    </td>
                                </tr>

                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                </div>
            </div>

        </telerik:RadPageView>

        <telerik:RadPageView ID="other" runat="server" Style="margin-top: 10px;">
            <div class="content-box2">
                <div class="title">
                    <img src="Images/projectrole.png" width="16px" height="16px" />
                    <asp:Label ID="ph1" runat="server" Text="Oprávnění k dokumentům"></asp:Label>
                    <asp:Button ID="cmdAddX69" runat="server" CssClass="cmd" Text="Přidat" />
                </div>
                <div class="content">
                    <uc:entityrole_assign ID="roles1" runat="server" EntityX29ID="x18EntityCategory"></uc:entityrole_assign>
                    <div class="div6" style="clear: both; margin-top: 20px; border-top: dashed silver 1px; display: none;">
                        <asp:Label ID="lblOwner" runat="server" Text="Vlastník záznamu dokumentu:" CssClass="lblReq"></asp:Label>
                        <uc:person ID="j02ID_Owner" runat="server" Width="300px" Flag="all" />
                    </div>
                </div>
            </div>


            <div class="content-box2">
                <div class="title">
                    Vztah k souborovým přílohám
                </div>
                <div class="content">
                    <div class="div6">
                        <asp:RadioButtonList ID="x18UploadFlag" runat="server" RepeatDirection="Vertical" AutoPostBack="true">
                            <asp:ListItem Text="Bez rozšířených file-system funkcí" Value="0" Selected="true"></asp:ListItem>
                            <asp:ListItem Text="Podpora rozšířených file-system funkcí" Value="1"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                    <asp:Panel ID="panFileSystem" runat="server">

                        <div class="div6">
                            <span class="lbl">Omezení maximální velikosti souborové přílohy:</span>
                            <asp:DropDownList ID="x18MaxOneFileSize" runat="server">
                                <asp:ListItem Text="1 MB" Value="1048576"></asp:ListItem>
                                <asp:ListItem Text="2 MB" Value="2097152"></asp:ListItem>
                                <asp:ListItem Text="3 MB" Value="3145728" Selected="true"></asp:ListItem>
                                <asp:ListItem Text="4 MB" Value="4194304"></asp:ListItem>
                                <asp:ListItem Text="5 MB" Value="5242880"></asp:ListItem>
                                <asp:ListItem Text="6 MB" Value="6291456"></asp:ListItem>
                                <asp:ListItem Text="7 MB" Value="7340032"></asp:ListItem>
                                <asp:ListItem Text="10 MB" Value="10485760"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="div6">
                            <span>Povolené přípony nahrávaných souborů:</span>
                            <asp:TextBox ID="x18AllowedFileExtensions" runat="server" Width="400px"></asp:TextBox>
                            (čárkou oddělené)
                        </div>

                    </asp:Panel>
                    

                </div>
            </div>
            

            <div class="content-box2" style="margin-top: 60px;">
                <div class="title">Různé</div>
                <div class="content">
                    <div class="div6">
                        <asp:DropDownList ID="x18GridColsFlag" runat="server">
                            <asp:ListItem Text="Jako sloupce v přehledu dokumentů zobrazovat i [Název] a [Kód]" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Jako sloupce v přehledu dokumentů zobrazovat i [Kód]" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Jako sloupce v přehledu dokumentů zobrazovat i [Název]" Value="3"></asp:ListItem>
                            <asp:ListItem Text="Nezobrazovat ani [Název] ani [Kód]" Value="4"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <table cellpadding="5" cellspacing="2">

                        <tr>
                            <td colspan="2">
                                <asp:CheckBox ID="x18IsColors" runat="server" CssClass="chk" Text="Možnost rozlišovat dokumenty barvou" Checked="false" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:CheckBox ID="x18IsAllowEncryption" runat="server" Text="V záznamu dokumentu nabízet možnost zašifrovat obsah" />
                            </td>
                        </tr>

                        <tr>
                            <td>
                                <span>Vyplňování názvu položky:</span>
                            </td>
                            <td>
                                <asp:DropDownList ID="x18EntryNameFlag" runat="server">
                                    <asp:ListItem Text="Ručně" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Nevyplňovat" Value="2"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span>Vyplňování kódu položky:</span>
                            </td>
                            <td>
                                <asp:DropDownList ID="x18EntryCodeFlag" runat="server">
                                    <asp:ListItem Text="Ručně" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Nepoužívat" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Generovat automaticky v rámci všech dokumentů" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="Generovat automaticky v rámci projektu" Value="4"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span>Vyplňování pořadí:</span>
                            </td>
                            <td>
                                <asp:DropDownList ID="x18EntryOrdinaryFlag" runat="server">
                                    <asp:ListItem Text="Ručně" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Nepoužívat" Value="2"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblB01ID" Text="Workflow šablona:" runat="server"></asp:Label>
                            </td>
                            <td>
                                <uc:datacombo ID="b01ID" runat="server" AutoPostBack="false" DataTextField="b01Name" DataValueField="pid" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>


                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:CheckBox ID="x18IsCalendar" runat="server" Text="Typ dokumentu podporuje i kalendářové rozhraní (samostatný kalendář)" AutoPostBack="true" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblx18CalendarFieldStart" runat="server" Text="DB pole pro začátek kalendářové události"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="x18CalendarFieldStart" runat="server">
                                    <asp:ListItem Text=""></asp:ListItem>
                                    <asp:ListItem Text="Datum 1" Value="o23FreeDate01"></asp:ListItem>
                                    <asp:ListItem Text="Datum 2" Value="o23FreeDate02"></asp:ListItem>
                                    <asp:ListItem Text="Datum 3" Value="o23FreeDate03"></asp:ListItem>
                                    <asp:ListItem Text="Datum 4" Value="o23FreeDate04"></asp:ListItem>
                                    <asp:ListItem Text="Datum 5" Value="o23FreeDate05"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblx18CalendarFieldEnd" runat="server" Text="DB pole pro konec kalendářové události"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="x18CalendarFieldEnd" runat="server">
                                    <asp:ListItem Text=""></asp:ListItem>
                                    <asp:ListItem Text="Datum 1" Value="o23FreeDate01"></asp:ListItem>
                                    <asp:ListItem Text="Datum 2" Value="o23FreeDate02"></asp:ListItem>
                                    <asp:ListItem Text="Datum 3" Value="o23FreeDate03"></asp:ListItem>
                                    <asp:ListItem Text="Datum 4" Value="o23FreeDate04"></asp:ListItem>
                                    <asp:ListItem Text="Datum 5" Value="o23FreeDate05"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblx18CalendarFieldSubject" runat="server" Text="Pole pro název události v kalendáři:"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="x18CalendarFieldSubject" runat="server">
                                    <asp:ListItem Text="--Prázdné--" Value=""></asp:ListItem>
                                    <asp:ListItem Text="Název záznamu" Value="o23Name"></asp:ListItem>
                                    <asp:ListItem Text="Kód záznamu" Value="o23Code"></asp:ListItem>
                                    <asp:ListItem Text="Osoba" Value="j02_alias"></asp:ListItem>
                                    <asp:ListItem Text="Projekt" Value="p41_alias"></asp:ListItem>
                                    <asp:ListItem Text="Klient" Value="p28_alias"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblx18CalendarResourceField" runat="server" Text="Zdroj v timeline kalendáři:"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="x18CalendarResourceField" runat="server">
                                    <asp:ListItem Text="--Zdroje nevyužívat--" Value=""></asp:ListItem>
                                    <asp:ListItem Text="Osoba" Value="j02ID"></asp:ListItem>

                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span>Grafická ikona (16px):</span>
                            </td>
                            <td>
                                <asp:TextBox ID="x18Icon" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span>Grafická ikona (32px):</span>
                            </td>
                            <td>
                                <asp:TextBox ID="x18Icon32" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>Vztah k úvodní (dashboard) stránce:
                            </td>
                            <td>
                                <asp:DropDownList ID="x18DashboardFlag" runat="server">
                                    <asp:ListItem Text="--Žádný vztah--" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Tlačítko pro založení nového záznamu + odkaz na přehled/kalendář" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Pouze tlačítko pro založení nového záznamu" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Pouze odkaz na přehled/kalendář" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="Zobrazovat položky jako nástěnku" Value="4"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:CheckBox ID="x18IsClueTip" runat="server" Text="U přiřazené položky zobrazovat info-bublinu" Visible="false" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span>Svázané šablony tiskových sestav:</span>
                            </td>
                            <td>
                                <asp:TextBox ID="x18ReportCodes" runat="server" Width="600px"></asp:TextBox>
                                (čárkou oddělené kódy sestav)
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span>Plugin nahrazující editační formulář dokumentu:</span>
                            </td>
                            <td>
                                <asp:DropDownList ID="x31ID_Plugin" runat="server" DataValueField="pid" DataTextField="x31Name"></asp:DropDownList>
                                
                            </td>
                        </tr>
                    </table>
                </div>
            </div>

        </telerik:RadPageView>
    </telerik:RadMultiPage>

    <asp:HiddenField ID="hidGUID_x16" runat="server" />
    
    <asp:HiddenField ID="hidGUID_x20" runat="server" />
    <asp:HiddenField ID="HardRefreshPID" runat="server" />
    <asp:Button ID="cmdHardRefresh" runat="server" Style="display: none;" />

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
