Public Class p31summary
    Inherits System.Web.UI.UserControl

    Public Property AllowShowRates As Boolean
        Get
            Return BO.BAS.BG(Me.hidAllowShowRates.Value)
        End Get
        Set(value As Boolean)
            Me.hidAllowShowRates.Value = BO.BAS.GB(value)
        End Set
    End Property
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub


    Public Sub RefreshData(lis As IEnumerable(Of BO.p31WorksheetBigSummary), intWorksheetState As Integer)
        Me.hidState.Value = intWorksheetState.ToString
        ''If intWorksheetState = 1 Then
        ''    'Me.lblFakturovat.Text = "Fakturovat"
        ''    lblRozpracovano.Visible = True
        ''    ''lblHeader.text = "Schváleno, čeká na fakturaci"
        ''Else
        ''    'Me.lblFakturovat.Text = "Vyfakturováno"
        ''    lblRozpracovano.Visible = False
        ''    ''lblHeader.text = "Vyfakturováno"
        ''End If
        rp1.DataSource = lis
        rp1.DataBind()


    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.p31WorksheetBigSummary = CType(e.Item.DataItem, BO.p31WorksheetBigSummary)

        With cRec
            If Me.hidState.Value = "1" Then
                e.Item.FindControl("hodiny_count")
                SV("hodiny_rozpracovano", .rozpracovano_hodiny, e, )
                SV("honorar_rozpracovano", .rozpracovano_honorar, e, .j27Code)
                SV("hodiny_fakturovat", .schvaleno_hodiny, e)
                SV("odmeny_rozpracovano", .rozpracovano_odmeny, e, .j27Code)
                SV("vydaje_rozpracovano", .rozpracovano_vydaje, e, .j27Code)

                SV("hodiny_fakturovat", .schvaleno_hodiny, e)
                If Me.hidAllowShowRates.Value = "1" Then
                    SV("honorar_fakturovat", .schvaleno_honorar, e, .j27Code)
                    SV("odmeny_fakturovat", .schvaleno_odmeny, e, .j27Code)
                    ''SV("odmeny_odpis", .schvaleno_odmeny_odpis, e, .j27Code)
                    ''SV("odmeny_pausal", .schvaleno_odmeny_pausal, e, .j27Code)
                    SV("vydaje_fakturovat", .schvaleno_vydaje, e, .j27Code)
                    ''SV("vydaje_odpis", .schvaleno_vydaje_odpis, e, .j27Code)
                    ''SV("vydaje_pausal", .schvaleno_vydaje_pausal, e, .j27Code)
                End If

                SV("hodiny_odpis", .schvaleno_hodiny_odpis, e)
                SV("hodiny_pausal", .schvaleno_hodiny_pausal, e)
               
            Else
                SV("hodiny_rozpracovano", 0, e)
                SV("honorar_rozpracovano", 0, e)
                SV("odmeny_rozpracovano", 0, e)
                SV("vydaje_rozpracovano", 0, e)

                SV("hodiny_fakturovat", .vyfakturovano_hodiny, e)
                If Me.hidAllowShowRates.Value = "1" Then
                    SV("honorar_fakturovat", .vyfakturovano_honorar, e, .j27Code)
                    SV("odmeny_fakturovat", .vyfakturovano_odmeny, e, .j27Code)
                    ''SV("odmeny_odpis", .vyfakturovano_odmeny_odpis, e, .j27Code)
                    ''SV("odmeny_pausal", .vyfakturovano_odmeny_pausal, e, .j27Code)
                    SV("vydaje_fakturovat", .vyfakturovano_vydaje, e, .j27Code)
                    ''SV("vydaje_odpis", .vyfakturovano_vydaje_odpis, e, .j27Code)
                    ''SV("vydaje_pausal", .vyfakturovano_vydaje_pausal, e, .j27Code)
                End If

                SV("hodiny_odpis", .vyfakturovano_hodiny_odpis, e)
                SV("hodiny_pausal", .vyfakturovano_hodiny_pausal, e)
               
            End If
        End With


    End Sub

    Private Sub SV(strControl As String, dbl As Double?, e As RepeaterItemEventArgs, Optional strJ27Code As String = "")
        If dbl Is Nothing Then
            e.Item.Controls.Remove(e.Item.FindControl(strControl)) : Return
        End If
        If dbl = 0 Then
            e.Item.Controls.Remove(e.Item.FindControl(strControl)) : Return
        End If

        With CType(e.Item.FindControl(strControl), Label)
            .Text = BO.BAS.FN(dbl)
            If strJ27Code <> "" Then
                .Text += " " & strJ27Code
            End If
        End With

        
    End Sub
End Class