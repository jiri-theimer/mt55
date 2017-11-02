<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" %>

<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="plugin_datatable" Src="~/plugin_datatable.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>



<script runat="server">
    Protected Sub Page_Load() Handles Me.Load
        If Not Page.IsPostBack Then
            hidX18ID.Value = Request.Item("x18id")
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            Me.j02ID_Zadatel.Value = Master.Factory.SysUser.j02ID.ToString
            Me.j02ID_Zadatel.Text = Master.Factory.SysUser.PersonDesc
            o23FreeDate01.SelectedDate = Today
            o23FreeDate02.SelectedDate = Today
            
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
        
       
        
    End Sub

    
    Private Sub RefreshRecord()
        
        If Master.DataPID = 0 Then
            cmdDelete.Visible = False
            Return
        End If
        Dim cRec As BO.o23Doc = Master.Factory.o23DocBL.Load(Master.DataPID)
        If cRec Is Nothing Then Return
        Dim x20IDs As New List(Of Integer)
        x20IDs.Add(7)
        Dim lisX19 As List(Of BO.x19EntityCategory_Binding) = Master.Factory.x18EntityCategoryBL.GetList_X19(Master.DataPID, x20IDs, True).ToList
       
        If lisX19.Exists(Function(p) p.x20ID = 7) Then
            Me.j02ID_Zadatel.Value = lisX19.First(Function(p) p.x20ID = 7).x19RecordPID.ToString
            Me.j02ID_Zadatel.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.j02Person, Me.j02ID_Zadatel.Value, True)
        End If
        
        With cRec
            Me.PageHeader.Text = .NameWithCode
            If .IsClosed Then Me.PageHeader.Font.Strikeout = True
            Me.o23FreeBoolean01.Checked = .o23FreeBoolean01
            If Not .o23FreeDate01 Is Nothing Then
                Me.o23FreeDate01.SelectedDate = .o23FreeDate01
            End If
            If Not .o23FreeDate02 Is Nothing Then
                Me.o23FreeDate02.SelectedDate = .o23FreeDate01
            End If
          
           
        
            Me.o23BigText.Text = .o23BigText
            
        End With
        
        
    End Sub
    
   
    
    
    
    

    Private Sub ReloadPage()
        Response.Redirect("bdo_o23_record_dovolena.aspx?pid=" & Master.DataPID.ToString)
    End Sub
        
    
    Private Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdSave.Click
        Dim cRec As BO.o23Doc = IIf(Master.DataPID <> 0, Master.Factory.o23DocBL.Load(Master.DataPID), New BO.o23Doc), intX18ID As Integer = BO.BAS.IsNullInt(hidX18ID.Value)
        
        Dim intJ02ID_Zadatel As Integer = BO.BAS.IsNullInt(Me.j02ID_Zadatel.Value)
        If intJ02ID_Zadatel = 0 Or Me.o23FreeDate01.IsEmpty or Me.o23FreeDate02.IsEmpty Then
            Master.Notify("[Žadatel], [Začátek] a [Konec] jsou povinná pole.", UI.NotifyLevel.ErrorMessage) : Return
        End If
        If Me.o23FreeDate02.SelectedDate < Me.o23FreeDate01.SelectedDate Then
            Master.Notify("[Začátek] musí být menší nebo rovno [Konec].") : Return
        End If
        If o23FreeBoolean01.Checked Then
            If Me.o23FreeDate02.SelectedDate <> Me.o23FreeDate01.SelectedDate Then
                Master.Notify("Žádost na 1/2 den může být vyplněna pouze pro jeden kalendářní den ([Začátek] se musí rovnat [Konec].).")
                Return
            End If
        End If
        With cRec
            If Master.DataPID = 0 Then
                Dim cX18 As BO.x18EntityCategory = Master.Factory.x18EntityCategoryBL.Load(intX18ID)
                .x23ID = cX18.x23ID
            End If
            
            .o23BigText = Me.o23BigText.Text
            .o23FreeDate01 = Me.o23FreeDate01.SelectedDate
            .o23FreeDate02 = Me.o23FreeDate02.SelectedDate
            .o23FreeBoolean01 = Me.o23FreeBoolean01.Checked
        End With
        Dim lisX19 As New List(Of BO.x19EntityCategory_Binding), x20IDs As New List(Of Integer)
        Dim c As New BO.x19EntityCategory_Binding
        c.x19RecordPID = intJ02ID_Zadatel
        c.x20ID = 7
       
        lisX19.Add(c) : x20IDs.Add(9)
        
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

            <%If hidOper.Value = "closeandrefresh" Then%>
            //window.parent.hardrefresh(<%=Master.DataPID%>, "refresh");
            window.parent.location.replace("../o23_fixwork.aspx?x18id=<%=hidX18ID.Value%>&pid=<%=Master.DataPID%>");
            <%End If%>

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
                <span class="valboldblue">Žádost o dovolenou</span>
            </td>
        </tr>
    </table>
    <div class="div6">
        <asp:Button ID="cmdSave" runat="server" Text="Uložit změny" CssClass="cmd" />
        <asp:Button ID="cmdDelete" runat="server" Text="Odstranit" CssClass="cmd" OnClientClick="return trydel();" Style="margin-left: 50px;" />
        <button id="cmdClose" runat="server" type="button" style="margin-left: 50px;" onclick="window.close()">Zavřít</button>
    </div>
    <table cellpadding="8">


        <tr>
            <td>žadatel:
            </td>
            <td>
                <uc:person ID="j02ID_Zadatel" runat="server" Width="400px" Flag="" />
            </td>
        </tr>
        <tr>
            <td>Začátek:
            </td>
            <td>
                <telerik:RadDatePicker ID="o23FreeDate01" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                    <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                </telerik:RadDatePicker>
            </td>
        </tr>
        <tr>
            <td>Konec:
            </td>
            <td>
                <telerik:RadDatePicker ID="o23FreeDate02" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                    <DateInput ID="DateInput2" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                </telerik:RadDatePicker>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="o23FreeBoolean01" Text="Pouze 1/2 den" runat="server" AutoPostBack="true" />
            </td>
        </tr>
    </table>



    <fieldset>
        <legend>Poznámka</legend>
        <asp:TextBox ID="o23BigText" runat="server" TextMode="MultiLine" Width="100%" Height="80px"></asp:TextBox>
    </fieldset>
    <asp:HiddenField ID="hidX18ID" runat="server" />
    <asp:HiddenField ID="hidBlank" runat="server" Value="0" />
    <asp:HiddenField ID="hidOper" runat="server" />

    <telerik:RadCalendar ID="SharedCalendar" runat="server" EnableMultiSelect="False" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">

        <SpecialDays>
            <telerik:RadCalendarDay Repeatable="Today" ItemStyle-BackColor="SkyBlue"></telerik:RadCalendarDay>
        </SpecialDays>
    </telerik:RadCalendar>
</asp:Content>
