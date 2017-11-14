<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" %>

<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register TagPrefix="uc" TagName="plugin_datatable" Src="~/plugin_datatable.ascx" %>
<%@ Register TagPrefix="uc" TagName="contact" Src="~/contact.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="project" Src="~/project.ascx" %>


<script runat="server">
    Protected Sub Page_Load() Handles Me.Load
        If Not Page.IsPostBack Then
            hidX18ID.Value = Request.Item("x18id")
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            Me.j02ID_Vypracoval.Value = Master.Factory.SysUser.j02ID.ToString
            Me.j02ID_Vypracoval.Text = Master.Factory.SysUser.PersonDesc
            hidBlank.Value = Request.Item("blank")
            If hidBlank.Value = "1" Then
                cmdClose.Visible = True
            Else
                cmdClose.Visible = False
            End If
            
            
            RefreshRecord()
            If Request.Item("clone") = "1" Then
                Master.DataPID = 0
            End If
            
        End If
    End Sub
    
    
    
    Protected Sub Page_LoadComplete() Handles Me.LoadComplete
        
        tr_o23FreeText07.Visible = False : tr_o23FreeText03.Visible = False : tr_o23FreeText06.Visible = False : tr_o23FreeText05.Visible = False
       
        Dim strDruhVystupu As String = o23FreeText02.SelectedValue
        
        Select Case strDruhVystupu
            Case "Ostatní"
                'tr_o23FreeText07.Visible = True
            Case "Ocenění"
                tr_o23FreeText03.Visible = True : tr_o23FreeText06.Visible = True : tr_o23FreeText05.Visible = True
                
        End Select
        
        Dim strPredmetOceneni As String = o23FreeText05.SelectedValue
        tr_o23FreeText04.Visible = False : tr_o23FreeText11.Visible = False : o23FreeBoolean01.Visible = False
        If strPredmetOceneni = "Podnik, společnost, podíl, akcie" Then
            tr_o23FreeText04.Visible = True : tr_o23FreeText11.Visible = True
            o23FreeBoolean01.Visible = True
        End If
        tr_o23FreeText12.Visible = o23FreeBoolean01.Checked
        tr_o23FreeText13.Visible = o23FreeBoolean01.Checked
        
        
    End Sub

    
    Private Sub RefreshRecord()
        
        If Master.DataPID = 0 Then
            cmdDelete.Visible = False
            Return
        End If
        Dim cRec As BO.o23Doc = Master.Factory.o23DocBL.Load(Master.DataPID)
        If cRec Is Nothing Then Return
        Dim x20IDs As New List(Of Integer)
        x20IDs.Add(11) : x20IDs.Add(12) : x20IDs.Add(13) : x20IDs.Add(14)
        Dim lisX19 As List(Of BO.x19EntityCategory_Binding) = Master.Factory.x18EntityCategoryBL.GetList_X19(Master.DataPID, x20IDs, True).ToList
        If lisX19.Exists(Function(p) p.x20ID = 11) Then
            Me.p41ID.Value = lisX19.First(Function(p) p.x20ID = 11).x19RecordPID.ToString
            Me.p41ID.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p41Project, Me.p41ID.Value, True)
        End If
        If lisX19.Exists(Function(p) p.x20ID = 12) Then
            Me.p28ID_Objednatel.Value = lisX19.First(Function(p) p.x20ID = 12).x19RecordPID.ToString
            Me.p28ID_Objednatel.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p28Contact, Me.p28ID_Objednatel.Value, True)
        End If
        If lisX19.Exists(Function(p) p.x20ID = 13) Then
            Me.p28ID_Prevzal.Value = lisX19.First(Function(p) p.x20ID = 13).x19RecordPID.ToString
            Me.p28ID_Prevzal.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p28Contact, Me.p28ID_Prevzal.Value, True)
        End If
        If lisX19.Exists(Function(p) p.x20ID = 14) Then
            Me.j02ID_Vypracoval.Value = lisX19.First(Function(p) p.x20ID = 14).x19RecordPID.ToString
            Me.j02ID_Vypracoval.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.j02Person, Me.j02ID_Vypracoval.Value, True)
        End If
        
        With cRec
            Me.PageHeader.Text = .NameWithCode
            If .IsClosed Then Me.PageHeader.Font.Strikeout = True
           
            Me.o23Name.Text = .o23Name
           
            UI.basUI.SelectDropdownlistValue(Me.o23FreeText02, .o23FreeText02)
            UI.basUI.SelectDropdownlistValue(Me.o23FreeText03, .o23FreeText03)
            UI.basUI.SelectDropdownlistValue(Me.o23FreeText04, .o23FreeText04)
            UI.basUI.SelectDropdownlistValue(Me.o23FreeText05, .o23FreeText05)
            UI.basUI.SelectDropdownlistValue(Me.o23FreeText06, .o23FreeText06)
            Me.o23FreeText07.Text=.o23FreeText07
           
           
            Me.o23FreeText07.Text = .o23FreeText07
            Me.o23FreeText10.Text = .o23FreeText10
            
            UI.basUI.SelectDropdownlistValue(Me.o23FreeText08, .o23FreeText08)
           
            UI.basUI.SelectDropdownlistValue(Me.o23FreeText11, .o23FreeText11)
            UI.basUI.SelectDropdownlistValue(Me.o23FreeText12, .o23FreeText12)
            UI.basUI.SelectDropdownlistValue(Me.o23FreeText13, .o23FreeText13)
            
           
        
            Me.o23FreeText14.Text = .o23FreeText14
            Me.o23FreeBoolean01.Checked = .o23FreeBoolean01
            
        End With
        
        
    End Sub
    
   
    
    
    
    

    Private Sub ReloadPage()
        Response.Redirect("bdo_o23_record_znalecka_zprava.aspx?pid=" & Master.DataPID.ToString)
    End Sub
        
    
    Private Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdSave.Click
        Dim cRec As BO.o23Doc = IIf(Master.DataPID <> 0, Master.Factory.o23DocBL.Load(Master.DataPID), New BO.o23Doc), intX18ID As Integer = BO.BAS.IsNullInt(hidX18ID.Value)
        Dim intP41ID As Integer = BO.BAS.IsNullInt(Me.p41ID.Value), intP28ID_Objednatel As Integer = BO.BAS.IsNullInt(Me.p28ID_Objednatel.Value), intP28ID_Prevzal As Integer = BO.BAS.IsNullInt(Me.p28ID_Prevzal.Value)
        Dim intJ02ID_Vypracoval As Integer = BO.BAS.IsNullInt(Me.j02ID_Vypracoval.Value)
        If intP41ID = 0 Or intP28ID_Objednatel = 0 Or intJ02ID_Vypracoval = 0 Or Trim(Me.o23Name.Text) = "" Then
            Master.Notify("[Projekt], [Objednatel] a [Vypracoval], [Název] jsou povinná pole.", UI.NotifyLevel.ErrorMessage) : Return
        End If
        With cRec
            If Master.DataPID = 0 Then
                Dim cX18 As BO.x18EntityCategory = Master.Factory.x18EntityCategoryBL.Load(intX18ID)
                .x23ID = cX18.x23ID
            End If
            
            .o23Name = Me.o23Name.Text
            
            .o23FreeText02 = Me.o23FreeText02.SelectedValue
            If tr_o23FreeText03.Visible Then
                .o23FreeText03 = Me.o23FreeText03.SelectedValue
            Else
                .o23FreeText03 = ""
            End If
            
            If tr_o23FreeText04.Visible Then
                .o23FreeText04 = Me.o23FreeText04.SelectedValue
            Else
                .o23FreeText04 = ""
            End If
            If tr_o23FreeText05.Visible Then
                .o23FreeText05 = Me.o23FreeText05.SelectedValue
            Else
                .o23FreeText05 = ""
            End If
            
            If tr_o23FreeText06.Visible Then
                .o23FreeText06 = Me.o23FreeText06.SelectedValue
            Else
                .o23FreeText06 = ""
            End If
            
            .o23FreeText07 = Me.o23FreeText07.Text
            .o23FreeText08 = Me.o23FreeText08.SelectedValue
            .o23FreeText10 = Me.o23FreeText10.Text
          
            .o23FreeText11 = Me.o23FreeText11.SelectedValue
            If Me.o23FreeBoolean01.Visible Then
                .o23FreeBoolean01 = Me.o23FreeBoolean01.Checked
            Else
                .o23FreeBoolean01 = False
            End If
            
            If tr_o23FreeText12.Visible Then
                .o23FreeText12 = Me.o23FreeText12.SelectedValue
            Else
                .o23FreeText12 = ""
            End If
            If tr_o23FreeText13.Visible Then
                .o23FreeText13 = Me.o23FreeText13.SelectedValue
            Else
                .o23FreeText13 = ""
            End If
            .o23FreeText14 = Me.o23FreeText14.Text
        End With
        Dim lisX19 As New List(Of BO.x19EntityCategory_Binding), x20IDs As New List(Of Integer)
        Dim c As New BO.x19EntityCategory_Binding
        c.x19RecordPID = intP41ID
        c.x20ID = 11
        lisX19.Add(c) : x20IDs.Add(11)
        c = New BO.x19EntityCategory_Binding
        c.x19RecordPID = intP28ID_Objednatel
        c.x20ID = 12
        lisX19.Add(c) : x20IDs.Add(12)
        c = New BO.x19EntityCategory_Binding
        c.x19RecordPID = intP28ID_Prevzal
        c.x20ID = 13
        lisX19.Add(c) : x20IDs.Add(13)
        c = New BO.x19EntityCategory_Binding
        c.x19RecordPID = intJ02ID_Vypracoval
        c.x20ID = 14
        lisX19.Add(c) : x20IDs.Add(14)
        
        With Master.Factory.o23DocBL
            If .Save(cRec, intX18ID, Nothing, lisX19, x20IDs, "") Then
                If Master.DataPID = 0 Then Master.DataPID = .LastSavedPID
                If hidBlank.Value = "1" Then
                    hidOper.Value = "closeandrefresh"
                End If
            End If
        End With
        
    End Sub
    
    
    Private Sub cmdDelete_Click(sender As Object, e As EventArgs) Handles cmdDelete.Click
        With Master.Factory.o23DocBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                RefreshRecord()
                If hidBlank.Value = "1" Then
                    hidOper.Value = "closeandrefresh"
                Else
                    Master.StopPage("Záznam byl odstraněn", False)
                End If
                
                
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="Plugin.css" />
    <link rel="stylesheet" type="text/css" href="Scripts/datatable/css/jquery.dataTables.css" />
    <script type="text/javascript" src="Scripts/datatable/jquery.dataTables.min.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            
            <%if hidOper.value="closeandrefresh" then%>
            //window.parent.hardrefresh(<%=Master.DataPID%>, "refresh");
            window.parent.open("../o23_fixwork.aspx?x18id=<%=hidX18ID.Value%>&pid=<%=Master.DataPID%>","_top");
            <%end if%>

        });

       


        function trydel() {

            if (confirm("Opravdu odstranit záznam?")) {
                return (true);
            }
            else {
                return (false);
            }
        }
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="10">
        <tr>
            <td id="top">
                <img src="Images/notepad_32.png" alt="Dokument" />
            </td>
            <td>
                <asp:Label ID="PageHeader" CssClass="page_header_span" runat="server"></asp:Label>

            </td>
            <td>
                <span class="valboldblue">Znalecká zpráva</span>
            </td>
        </tr>
    </table>
    <div class="div6">
        <asp:Button ID="cmdSave" runat="server" Text="Uložit změny" CssClass="cmd" />
        <asp:Button ID="cmdDelete" runat="server" Text="Odstranit" CssClass="cmd" OnClientClick="return trydel();" style="margin-left:50px;" />
        <button id="cmdClose" runat="server" type="button" style="margin-left:50px;" onclick="window.close()">Zavřít</button>
    </div>
    <table>
        <tr>
            <td>
                Projekt:
            </td>
            <td>
                <uc:project ID="p41ID" runat="server" Width="400px" AutoPostBack="false" Flag="" />
            </td>
        </tr>
        <tr>
            <td>
                Objednatel:
            </td>
            <td>
                <uc:contact ID="p28ID_Objednatel" runat="server" Width="400px" Flag="" />
            </td>
        </tr>
        <tr>
            <td>
                Převzal:
            </td>
            <td>
                <uc:contact ID="p28ID_Prevzal" runat="server" Width="400px" Flag="" />
                Kont.osoba: <asp:TextBox ID="o23FreeText14" runat="server" Width="200px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                Vypracoval:
            </td>
            <td>
                <uc:person ID="j02ID_Vypracoval" runat="server" Width="400px" Flag="" />
            </td>
        </tr>
    </table>

    <table cellpadding="6">
        <tr>
            <td style="min-width:180px;">
                <span class="lblReq">Název:</span>
            </td>
            <td>
                <asp:TextBox ID="o23Name" runat="server" Width="300px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>Předmět úkonu (Účel):
            </td>
            <td>
                <asp:TextBox ID="o23FreeText10" runat="server" Width="600px" Height="70px" TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
     
      
        <tr>
            <td>
                Jazyk výstupu:
            </td>
            <td>
                <asp:DropDownList ID="o23FreeText08" runat="server" AutoPostBack="false">
                    <asp:ListItem Text="Čeština" Value="Čeština" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Angličtina" Value="Angličtina"></asp:ListItem>
                    <asp:ListItem Text="Němčina" Value="Němčina"></asp:ListItem>
                    <asp:ListItem Text="Ostatní" Value="Ostatní"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                Druh výstupu:                
            </td>
            <td>
                <asp:DropDownList ID="o23FreeText02" runat="server" AutoPostBack="true">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="Ocenění" Value="Ocenění"></asp:ListItem>
                    <asp:ListItem Text="Ušlý zisk" Value="Ušlý zisk"></asp:ListItem>
                    <asp:ListItem Text="Trasferové ceny" Value="Trasferové ceny"></asp:ListItem>
                    <asp:ListItem Text="Kalkulace" Value="Kalkulace"></asp:ListItem>
                    <asp:ListItem Text="Analýza" Value="Analýza"></asp:ListItem>
                    <asp:ListItem Text="Ostatní" Value="Ostatní"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr id="tr_o23FreeText07" runat="server" visible="false">
            <td>
                Účel výstupu:
            </td>
            <td>
                <asp:TextBox ID="o23FreeText07" runat="server" Width="400px"></asp:TextBox>
                (pro výstup Ostatní)
            </td>
        </tr>
        <tr id="tr_o23FreeText03" runat="server">
            <td>
                Účel ocenění:              
            </td>
            <td>
                <asp:DropDownList ID="o23FreeText03" runat="server" AutoPostBack="false" Width="400px">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="Tržní hodnota" Value="Tržní hodnota"></asp:ListItem>
                    <asp:ListItem Text="Prodej" Value="Prodej"></asp:ListItem>
                    <asp:ListItem Text="Přeměna (Fúze)" Value="Přeměna (Fúze)"></asp:ListItem>
                    <asp:ListItem Text="Přeměna (Rozdělení)" Value="Přeměna (Rozdělení)"></asp:ListItem>
                    <asp:ListItem Text="Přeměna (Převod jmění na společníka)" Value="Přeměna (Převod jmění na společníka)"></asp:ListItem>
                    <asp:ListItem Text="Přeměna (Změna právní formy)" Value="Přeměna (Změna právní formy)"></asp:ListItem>
                    <asp:ListItem Text="Nepeněžitý vklad" Value="Nepeněžitý vklad"></asp:ListItem>
                    <asp:ListItem Text="Zjištěná cena" Value="Zjištěná cena"></asp:ListItem>
                    <asp:ListItem Text="Ostatní" Value="Ostatní"></asp:ListItem>

                </asp:DropDownList>
                (pro výstup Ocenění)
            </td>
        </tr>
        <tr id="tr_o23FreeText06" runat="server">
            <td>
                Lokalita:                     
            </td>
            <td>
                <asp:DropDownList ID="o23FreeText06" runat="server" AutoPostBack="false" Width="400px">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="Hlavní město Praha" Value="Hlavní město Praha"></asp:ListItem>
                    <asp:ListItem Text="Středočeský" Value="Středočeský"></asp:ListItem>
                    <asp:ListItem Text="Jihočeský" Value="Jihočeský"></asp:ListItem>
                    <asp:ListItem Text="Plzeňský" Value="Plzeňský"></asp:ListItem>
                    <asp:ListItem Text="Karlovarský" Value="Karlovarský"></asp:ListItem>
                    <asp:ListItem Text="Ústecký" Value="Ústecký"></asp:ListItem>
                    <asp:ListItem Text="Liberecký" Value="Liberecký"></asp:ListItem>
                    <asp:ListItem Text="Královehradecký" Value="Královehradecký"></asp:ListItem>
                    <asp:ListItem Text="Pardubický" Value="Pardubický"></asp:ListItem>
                    <asp:ListItem Text="Olomoucký" Value="Olomoucký"></asp:ListItem>
                    <asp:ListItem Text="Moravskoslezský" Value="Moravskoslezský"></asp:ListItem>
                    <asp:ListItem Text="Jihomoravský" Value="Jihomoravský"></asp:ListItem>
                    <asp:ListItem Text="Zlínský" Value="Zlínský"></asp:ListItem>
                    <asp:ListItem Text="Kraj Vysočina" Value="Kraj Vysočina"></asp:ListItem>
                    <asp:ListItem Text="Ostatní" Value="Ostatní"></asp:ListItem>
                </asp:DropDownList>
                (pro výstup Ocenění)
            </td>
        </tr>
        <tr id="tr_o23FreeText05" runat="server">
            <td>
                Druh předmětu ocenění:                    
            </td>
            <td>
                <asp:DropDownList ID="o23FreeText05" runat="server" AutoPostBack="true" Width="400px">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="Podnik, společnost, podíl, akcie" Value="Podnik, společnost, podíl, akcie"></asp:ListItem>
                    <asp:ListItem Text="Pohledávky, závazky" Value="Pohledávky, závazky"></asp:ListItem>
                    <asp:ListItem Text="Nemovitost (Bydlení)" Value="Nemovitost (Bydlení)"></asp:ListItem>
                    <asp:ListItem Text="Nemovitost (Administrativa)" Value="Nemovitost (Administrativa)"></asp:ListItem>
                    <asp:ListItem Text="Nemovitost (Výroba)" Value="Nemovitost (Výroba)"></asp:ListItem>
                    <asp:ListItem Text="Nemovitost (Zemědělství)" Value="Nemovitost (Zemědělství)"></asp:ListItem>
                    <asp:ListItem Text="Nemovitost (Pozemek)" Value="Nemovitost (Pozemek)"></asp:ListItem>
                    <asp:ListItem Text="Nemovitost (Ostatní)" Value="Nemovitost (Ostatní)"></asp:ListItem>
                    <asp:ListItem Text="Nehmotný majetek" Value="Nehmotný majetek"></asp:ListItem>
                    <asp:ListItem Text="Movitý majetek" Value="Movitý majetek"></asp:ListItem>
                    <asp:ListItem Text="Analýzy" Value="Analýzy"></asp:ListItem>
                    <asp:ListItem Text="Ostatní" Value="Ostatní"></asp:ListItem>                   
                </asp:DropDownList>
                (pro výstup Ocenění)
            </td>
        </tr>
        <tr id="tr_o23FreeText04" runat="server">
            <td>
                Obor:                
            </td>
            <td>
                <asp:DropDownList ID="o23FreeText04" runat="server" AutoPostBack="false" Width="400px">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="SEKCE A - ZEMĚDĚLSTVÍ, LESNICTVÍ A RYBÁŘSTVÍ" Value="SEKCE A - ZEMĚDĚLSTVÍ, LESNICTVÍ A RYBÁŘSTVÍ"></asp:ListItem>
                    <asp:ListItem Text="SEKCE B - TĚŽBA A DOBÝVÁNÍ" Value="SEKCE B - TĚŽBA A DOBÝVÁNÍ"></asp:ListItem>
                    <asp:ListItem Text="SEKCE C - ZPRACOVATELSKÝ PRŮMYSL" Value="SEKCE C - ZPRACOVATELSKÝ PRŮMYSL"></asp:ListItem>
                    <asp:ListItem Text="SEKCE D – VÝROBA A ROZVOD ELEKTŘINY, PLYNU, TEPLA A KLIMATIZOVANÉHO VZDUCHU" Value="SEKCE D – VÝROBA A ROZVOD ELEKTŘINY, PLYNU, TEPLA A KLIMATIZOVANÉHO VZDUCHU"></asp:ListItem>
                    <asp:ListItem Text="SEKCE E – ZÁSOBOVÁNÍ VODOU, ČINNOSTI SOUVISEJÍCÍ S ODPADNÍMI VODAMI, ODPADY A SANACEMI" Value="SEKCE E – ZÁSOBOVÁNÍ VODOU,ČINNOSTI SOUVISEJÍCÍ S ODPADNÍMI VODAMI, ODPADY A SANACEMI"></asp:ListItem>
                    <asp:ListItem Text="SEKCE F - STAVEBNICTVÍ" Value="SEKCE F - STAVEBNICTVÍ"></asp:ListItem>
                    <asp:ListItem Text="SEKCE G - VELKOOBCHOD A MALOOBCHOD,OPRAVY A ÚDRŽBA MOTOROVÝCH VOZIDEL" Value="SEKCE G - VELKOOBCHOD A MALOOBCHOD,OPRAVY A ÚDRŽBA MOTOROVÝCH VOZIDEL"></asp:ListItem>
                    <asp:ListItem Text="SEKCE H - DOPRAVA A SKLADOVÁNÍ" Value="SEKCE H - DOPRAVA A SKLADOVÁNÍ"></asp:ListItem>
                    <asp:ListItem Text="SEKCE J - INFORMAČNÍ A KOMUNIKAČNÍ ČINNOSTI" Value="SEKCE J - INFORMAČNÍ A KOMUNIKAČNÍ ČINNOSTI"></asp:ListItem>
                    <asp:ListItem Text="SEKCE K - PENĚŽNICTVÍ A POJIŠŤOVNICTVÍ" Value="SEKCE K - PENĚŽNICTVÍ A POJIŠŤOVNICTVÍ"></asp:ListItem>
                    <asp:ListItem Text="SEKCE L - ČINNOSTI V OBLASTI NEMOVITOSTÍ" Value="SEKCE L - ČINNOSTI V OBLASTI NEMOVITOSTÍ"></asp:ListItem>
                    <asp:ListItem Text="SEKCE M - PROFESNÍ, VĚDECKÉ A TECHNICKÉ ČINNOSTI" Value="SEKCE M - PROFESNÍ, VĚDECKÉ A TECHNICKÉ ČINNOSTI"></asp:ListItem>
                    <asp:ListItem Text="SEKCE N - ADMINISTRATIVNÍ A PODPŮRNÉ ČINNOSTI" Value="SEKCE N - ADMINISTRATIVNÍ A PODPŮRNÉ ČINNOSTI"></asp:ListItem>                   
                    <asp:ListItem Text="SEKCE O - VEŘEJNÁ SPRÁVA A OBRANA" Value="SEKCE O - VEŘEJNÁ SPRÁVA A OBRANA"></asp:ListItem>                   
                    <asp:ListItem Text="SEKCE P – VZDĚLÁVÁNÍ" Value="SEKCE P – VZDĚLÁVÁNÍ"></asp:ListItem>                   
                    <asp:ListItem Text="SEKCE Q - ZDRAVOTNÍ A SOCIÁLNÍ PÉČE" Value="SEKCE Q - ZDRAVOTNÍ A SOCIÁLNÍ PÉČE"></asp:ListItem>                   
                    <asp:ListItem Text="SEKCE R - KULTURNÍ, ZÁBAVNÍ A REKREAČNÍ ČINNOSTI" Value="SEKCE R - KULTURNÍ, ZÁBAVNÍ A REKREAČNÍ ČINNOSTI"></asp:ListItem>                   
                    <asp:ListItem Text="SEKCE S - OSTATNÍ ČINNOSTI" Value="SEKCE S - OSTATNÍ ČINNOSTI"></asp:ListItem>                   
                    <asp:ListItem Text="SEKCE T - ČINNOSTI DOMÁCNOSTÍ JAKO ZAMĚSTNAVATELŮ, ČINNOSTI DOMÁCNOSTÍ PRODUKUJÍCÍCH BLÍŽE NEURČENÉ VÝROBKY A SLUŽBY PRO VLASTNÍ POTŘEBU" Value="SEKCE T - ČINNOSTI DOMÁCNOSTÍ JAKO ZAMĚSTNAVATELŮ, ČINNOSTI DOMÁCNOSTÍ PRODUKUJÍCÍCH BLÍŽE NEURČENÉ VÝROBKY A SLUŽBY PRO VLASTNÍ POTŘEBU"></asp:ListItem>                                       
                </asp:DropDownList>
                (pro předmět ocenění Podnik)
            </td>
        </tr>
        <tr id="tr_o23FreeText11" runat="server">
            <td>
                Metoda ocenění:                     
            </td>
            <td>
                <asp:DropDownList ID="o23FreeText11" runat="server" AutoPostBack="false" Width="400px">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="Majetková" Value="Majetková"></asp:ListItem>
                    <asp:ListItem Text="Výnosová" Value="Výnosová"></asp:ListItem>
                    <asp:ListItem Text="Porovnávací" Value="Porovnávací"></asp:ListItem>                    
                </asp:DropDownList>
                (pro předmět ocenění Podnik)
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="o23FreeBoolean01" runat="server" Text="Bylo v rámci ocenění Podnik, společnost, podíl, akcie oceněna i nemovitost?" AutoPostBack="true" />
            </td>
        </tr>
        <tr id="tr_o23FreeText12" runat="server">
            <td>
                Druh nemovitosti:                     
            </td>
            <td>
                <asp:DropDownList ID="o23FreeText12" runat="server" AutoPostBack="false" Width="400px">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="Bydlení" Value="Bydlení"></asp:ListItem>
                    <asp:ListItem Text="Administrativa" Value="Administrativa"></asp:ListItem>
                    <asp:ListItem Text="Výroba" Value="Výroba"></asp:ListItem>                    
                    <asp:ListItem Text="Zemědělství" Value="Zemědělství"></asp:ListItem>                    
                    <asp:ListItem Text="Pozemek" Value="Pozemek"></asp:ListItem>                    
                    <asp:ListItem Text="Ostatní" Value="Ostatní"></asp:ListItem>                              
                </asp:DropDownList>
                
            </td>
        </tr>

        <tr id="tr_o23FreeText13" runat="server">
            <td>
                Lokalita nemovitosti:                     
            </td>
            <td>
                <asp:DropDownList ID="o23FreeText13" runat="server" AutoPostBack="false" Width="400px">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="Hlavní město Praha" Value="Hlavní město Praha"></asp:ListItem>
                    <asp:ListItem Text="Středočeský" Value="Středočeský"></asp:ListItem>
                    <asp:ListItem Text="Jihočeský" Value="Jihočeský"></asp:ListItem>
                    <asp:ListItem Text="Plzeňský" Value="Plzeňský"></asp:ListItem>
                    <asp:ListItem Text="Karlovarský" Value="Karlovarský"></asp:ListItem>
                    <asp:ListItem Text="Ústecký" Value="Ústecký"></asp:ListItem>
                    <asp:ListItem Text="Liberecký" Value="Liberecký"></asp:ListItem>
                    <asp:ListItem Text="Královehradecký" Value="Královehradecký"></asp:ListItem>
                    <asp:ListItem Text="Pardubický" Value="Pardubický"></asp:ListItem>
                    <asp:ListItem Text="Olomoucký" Value="Olomoucký"></asp:ListItem>
                    <asp:ListItem Text="Moravskoslezský" Value="Moravskoslezský"></asp:ListItem>
                    <asp:ListItem Text="Jihomoravský" Value="Jihomoravský"></asp:ListItem>
                    <asp:ListItem Text="Zlínský" Value="Zlínský"></asp:ListItem>
                    <asp:ListItem Text="Kraj Vysočina" Value="Kraj Vysočina"></asp:ListItem>
                    <asp:ListItem Text="Ostatní" Value="Ostatní"></asp:ListItem>
                </asp:DropDownList>
                
            </td>
        </tr>

    </table>

    <fieldset>
        <legend>Poznámky</legend>
        <asp:TextBox ID="o23BigText" runat="server" TextMode="MultiLine" Width="100%" Height="80px"></asp:TextBox>
    </fieldset>
    <asp:HiddenField ID="hidX18ID" runat="server" />
    <asp:HiddenField ID="hidBlank" runat="server" Value="0" />
    <asp:HiddenField ID="hidOper" runat="server" />

   
</asp:Content>
