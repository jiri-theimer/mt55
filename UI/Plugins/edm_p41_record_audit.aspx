<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" %>

<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register TagPrefix="uc" TagName="plugin_datatable" Src="~/plugin_datatable.ascx" %>



<script runat="server">
    Protected Sub Page_Load() Handles Me.Load
        If Not Page.IsPostBack Then
            
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            
             
            dat1.SelectedDate = Today
            dat2.SelectedDate = Today
            dat3.SelectedDate = Today
            
            
            RefreshRecord()
            
            With Master.Factory.j03UserBL
                .InhaleUserParams("edm_p41_record_audit-defs")
                Dim a() As String = Split(.GetUserParam("edm_p41_record_audit-defs"), "|")
                If UBound(a) > 1 Then
                    UI.basUI.SelectDropdownlistValue(Me.p32id_1, a(0))
                    UI.basUI.SelectDropdownlistValue(Me.p32id_2, a(1))
                    UI.basUI.SelectDropdownlistValue(Me.p32id_3, a(2))
                    UI.basUI.SelectDropdownlistValue(Me.p32id_4, a(3))
                    UI.basUI.SelectDropdownlistValue(Me.p32id_5, a(4))
                End If
                                        
            End With
        End If
    End Sub
    
    
    
    Protected Sub Page_LoadComplete() Handles Me.LoadComplete
        
        Dim sum As Double = BO.BAS.IsNullNum(Me.TotalAmount.Value)
        val1_result.Text = BO.BAS.FN(sum * BO.BAS.IsNullNum(Me.val1.Value))
        val2_result.Text = BO.BAS.FN(sum * BO.BAS.IsNullNum(Me.val2.Value))
        val3_result.Text = BO.BAS.FN(sum * BO.BAS.IsNullNum(Me.val3.Value))
        val4_result.Text = BO.BAS.FN(sum * BO.BAS.IsNullNum(Me.val4.Value))
        val5_result.Text = BO.BAS.FN(sum * BO.BAS.IsNullNum(Me.val5.Value))
        
    End Sub

    
    Private Sub RefreshRecord()
        Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(Master.DataPID)
        If cRec Is Nothing Then Return
        
        With cRec
            Me.PageHeader.Text = .FullName
            If .IsClosed Then Me.PageHeader.Font.Strikeout = True
           
        End With
        
        Dim mq As New BO.myQueryP32
        mq.p34ID = 4
        Dim lis As List(Of BO.p32Activity) = Master.Factory.p32ActivityBL.GetList(mq).ToList
        lis.Insert(0, New BO.p32Activity)
        
        Me.p32id_1.DataSource = lis
        Me.p32id_1.DataBind()
        Me.p32id_2.DataSource = lis
        Me.p32id_2.DataBind()
        Me.p32id_3.DataSource = lis
        Me.p32id_3.DataBind()
        Me.p32id_4.DataSource = lis
        Me.p32id_4.DataBind()
        Me.p32id_5.DataSource = lis
        Me.p32id_5.DataBind()
    End Sub
    
   
    
    
    
    

    Private Sub ReloadPage()
        Response.Redirect("edm_p41_record.aspx?pid=" & Master.DataPID.ToString)
    End Sub
        
    
    Private Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdSave.Click
        Dim sum As Double = BO.BAS.IsNullNum(Me.TotalAmount.Value), x As Integer = 0
        If sum <= 0 Then
            Master.Notify("Chybí částka celkové odměny.", UI.NotifyLevel.ErrorMessage) : Return
        End If
        Dim errs As New List(Of String)
        If p32id_1.SelectedIndex > 0 And Not dat1.IsEmpty Then
            If GenerateWorksheet(errs, CInt(p32id_1.SelectedValue), sum * BO.BAS.IsNullNum(Me.val1.Value), Me.dat1.SelectedDate) > 0 Then x += 1
        End If
        If p32id_2.SelectedIndex > 0 And Not dat2.IsEmpty Then
            If GenerateWorksheet(errs, CInt(p32id_2.SelectedValue), sum * BO.BAS.IsNullNum(Me.val2.Value), Me.dat2.SelectedDate) > 0 Then x += 1
        End If
        If p32id_3.SelectedIndex > 0 And Not dat3.IsEmpty Then
            If GenerateWorksheet(errs, CInt(p32id_3.SelectedValue), sum * BO.BAS.IsNullNum(Me.val3.Value), Me.dat3.SelectedDate) > 0 Then x += 1
        End If
        If p32id_4.SelectedIndex > 0 And Not dat4.IsEmpty Then
            If GenerateWorksheet(errs, CInt(p32id_4.SelectedValue), sum * BO.BAS.IsNullNum(Me.val4.Value), Me.dat4.SelectedDate) > 0 Then x += 1
        End If
        If p32id_5.SelectedIndex > 0 And Not dat5.IsEmpty Then
            If GenerateWorksheet(errs, CInt(p32id_5.SelectedValue), sum * BO.BAS.IsNullNum(Me.val5.Value), Me.dat5.SelectedDate) > 0 Then x += 1
        End If
        If errs.Count > 0 Then
            Master.Notify("Počet založených úkonů: " & x.ToString & "<br>" & String.Join("<hr>", errs), UI.NotifyLevel.WarningMessage)
        Else
            Master.Notify("Počet založených úkonů: " & x.ToString)
        End If
        
        Dim strDefs As String = Me.p32id_1.SelectedValue & "|" & Me.p32id_2.SelectedValue & "|" & Me.p32id_3.SelectedValue & "|" & Me.p32id_4.SelectedValue & "|" & Me.p32id_5.SelectedValue
        Master.Factory.j03UserBL.SetUserParam("edm_p41_record_audit-defs", strDefs)
    End Sub
    
    Private Function GenerateWorksheet(ByRef errs As List(Of String), intP32ID As Integer, dblAmount As Double, dat As Date) As Integer
        Dim c As New BO.p31WorksheetEntryInput
        c.p41ID = Master.DataPID
        c.p31Date = dat
        c.p34ID = Master.Factory.p32ActivityBL.Load(intP32ID).p34ID
        c.p32ID = intP32ID
        c.p31Text = "Odměna"
        c.j02ID = Master.Factory.SysUser.j02ID
        c.Amount_WithoutVat_Orig = dblAmount
        c.j27ID_Billing_Orig = 2
        With Master.Factory.p31WorksheetBL
            If .SaveOrigRecord(c, Nothing) Then
                Return .LastSavedPID
            Else
                errs.Add(.ErrorMessage)
                Return 0
            End If
        End With
    End Function
   
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="Plugin.css" />
    <link rel="stylesheet" type="text/css" href="Scripts/datatable/css/jquery.dataTables.css" />
    <script type="text/javascript" src="Scripts/datatable/jquery.dataTables.min.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {



        });



    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="10">
        <tr>
            <td id="top">
                <img src="Images/project_32.png" alt="Projekt" />
            </td>
            <td>
                <asp:Label ID="PageHeader" CssClass="page_header_span" runat="server"></asp:Label>

            </td>
            <td>
                <span class="valboldblue">Kalkulačka fakturační odměny za audit</span>
            </td>
        </tr>
    </table>
    <div class="div6">
        <span>Celková odměna:</span>
        <telerik:RadNumericTextBox ID="TotalAmount" runat="server" NumberFormat-DecimalDigits="0" Width="100px" ShowSpinButtons="false" AutoPostBack="true" Value="100000"></telerik:RadNumericTextBox>
    </div>

    <table cellpadding="6">
        <tr>
            <td>
                <span>Splátka 1:</span>
            </td>
            <td>
                <telerik:RadDatePicker ID="dat1" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                    <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                </telerik:RadDatePicker>
            </td>
            <td>
                <asp:DropDownList ID="p32id_1" runat="server" AutoPostBack="true" DataValueField="pid" DataTextField="p32Name" Width="300px"></asp:DropDownList>
            </td>
            <td>
                <telerik:RadNumericTextBox ID="val1" runat="server" NumberFormat-DecimalDigits="2" Width="50px" ShowSpinButtons="true" IncrementSettings-Step="0.1" AutoPostBack="true"></telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:Label ID="val1_result" runat="server" CssClass="valboldblue"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <span>Splátka 2:</span>
            </td>
            <td>
                <telerik:RadDatePicker ID="dat2" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                    <DateInput ID="DateInput2" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                </telerik:RadDatePicker>
            </td>
            <td>
                <asp:DropDownList ID="p32id_2" runat="server" AutoPostBack="true" DataValueField="pid" DataTextField="p32Name" Width="300px"></asp:DropDownList>
            </td>
            <td>
                <telerik:RadNumericTextBox ID="val2" runat="server" NumberFormat-DecimalDigits="2" Width="50px" ShowSpinButtons="true" IncrementSettings-Step="0.1" AutoPostBack="true"></telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:Label ID="val2_result" runat="server" CssClass="valboldblue"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <span>Splátka 3:</span>
            </td>
            <td>
                <telerik:RadDatePicker ID="dat3" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                    <DateInput ID="DateInput3" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                </telerik:RadDatePicker>
            </td>
            <td>
                <asp:DropDownList ID="p32id_3" runat="server" AutoPostBack="true" DataValueField="pid" DataTextField="p32Name" Width="300px"></asp:DropDownList>
            </td>
            <td>
                <telerik:RadNumericTextBox ID="val3" runat="server" NumberFormat-DecimalDigits="2" Width="50px" ShowSpinButtons="true" IncrementSettings-Step="0.1" AutoPostBack="true"></telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:Label ID="val3_result" runat="server" CssClass="valboldblue"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <span>Splátka 4:</span>
            </td>
            <td>
                <telerik:RadDatePicker ID="dat4" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                    <DateInput ID="DateInput4" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                </telerik:RadDatePicker>
            </td>
            <td>
                <asp:DropDownList ID="p32id_4" runat="server" AutoPostBack="true" DataValueField="pid" DataTextField="p32Name" Width="300px"></asp:DropDownList>
            </td>
            <td>
                <telerik:RadNumericTextBox ID="val4" runat="server" NumberFormat-DecimalDigits="2" Width="50px" ShowSpinButtons="true" IncrementSettings-Step="0.1" AutoPostBack="true"></telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:Label ID="val4_result" runat="server" CssClass="valboldblue"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <span>Splátka 5d:</span>
            </td>
            <td>
                <telerik:RadDatePicker ID="dat5" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                    <DateInput ID="DateInput5" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                </telerik:RadDatePicker>
            </td>
            <td>
                <asp:DropDownList ID="p32id_5" runat="server" AutoPostBack="true" DataValueField="pid" DataTextField="p32Name" Width="300px"></asp:DropDownList>
            </td>
            <td>
                <telerik:RadNumericTextBox ID="val5" runat="server" NumberFormat-DecimalDigits="2" Width="50px" ShowSpinButtons="true" IncrementSettings-Step="0.1" AutoPostBack="true"></telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:Label ID="val5_result" runat="server" CssClass="valboldblue"></asp:Label>
            </td>
        </tr>
    </table>

   <div class="div6">
       <asp:Button ID="cmdSave" runat="server" Text="Vygenerovat odměny v projektu" CssClass="cmd" />
   </div>


    <telerik:RadCalendar ID="SharedCalendar" runat="server" EnableMultiSelect="False" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">

        <SpecialDays>
            <telerik:RadCalendarDay Repeatable="Today" ItemStyle-BackColor="SkyBlue"></telerik:RadCalendarDay>
        </SpecialDays>
    </telerik:RadCalendar>
</asp:Content>
