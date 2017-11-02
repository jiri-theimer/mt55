Imports Microsoft.VisualBasic
Imports System.IO
Imports System.Xml

Public Class p91_export2pohoda
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p91_export2pohoda_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            Dim lisPars As New List(Of String)
            With lisPars
                .Add("periodcombo-custom_query")
                .Add("p91_export2pohoda_ic")
            End With
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Export faktur do účetnictví POHODA"

                .Factory.j03UserBL.InhaleUserParams(lisPars)
                Me.txtIC.Text = .Factory.j03UserBL.GetUserParam("p91_export2pohoda_ic")
                period1.SetupData(.Factory, .Factory.j03UserBL.GetUserParam("periodcombo-custom_query"))
                period1.SelectedValue = .Factory.j03UserBL.GetUserParam("p31_grid-period")

                If .DataPID <> 0 Then
                    Dim cRec As BO.p91Invoice = .Factory.p91InvoiceBL.Load(.DataPID)
                    .AddToolbarButton(String.Format("Vygenerovat XML soubor vybrané faktury [{0}]", cRec.p91Code), "ok", , "Images/ok.png")
                End If


                Me.p93ID.DataSource = .Factory.p93InvoiceHeaderBL.GetList(New BO.myQuery)
                Me.p93ID.DataBind()
                Me.p93ID.Items.Insert(0, "---Nefiltrovat---")
            End With




        End If
    End Sub


    Private Function CreateOneInvoice(ByVal strP91ID As String, strExportDir As String, strFilePrefix As String) As String
        Dim dbRow As DataRow


        Dim strSQL As String = "select p91.*,p93.*,'' as stredisko,p41.projekt as contract,j27.j27Code,p28.*,o38prim.*,p86.*,left(p91Text2,90) as note90"
        If BO.BAS.IsNullInt(Me.txtMaxTextSize.Text) > 0 Then
            strSQL += ",left(p91text1," & Me.txtMaxTextSize.Text & ") as text90"
        Else
            strSQL += ",left(p91text1,1000) as text90"
        End If
        strSQL += " from p91invoice p91"
        strSQL += " left outer join p28Contact p28 on p91.p28id=p28.p28id"
        'strSQL += " LEFT OUTER JOIN j03user p28_j03_owner on p28.j03ID_Creator=p28_j03_owner.j03id"
        strSQL += " LEFT OUTER JOIN p92InvoiceType p92 on p91.p92id=p92.p92id"
        strSQL += " left outer join p93InvoiceHeader p93 on p92.p93id=p93.p93id"
        strSQL += " LEFT OUTER JOIN o38Address o38prim ON p91.o38ID_Primary=o38prim.o38ID"
        strSQL += " LEFT OUTER JOIN (select " & strP91ID & " as p91id,* FROM p86BankAccount WHERE p86ID=dbo.p91_get_p86id(" & strP91ID & ")) p86 ON p91.p91ID=p86.p91ID"
        strSQL += " left outer join (select min(p41Code) as projekt,min(a.p91ID) as p91ID FROM p31worksheet a INNER JOIN p41Project b ON a.p41ID=b.p41ID WHERE a.p91ID=" & strP91ID & ") p41 ON p91.p91ID=p41.p91ID"
        strSQL += " left outer join j27Currency j27 ON p91.j27ID=j27.j27ID"
        strSQL += " where p91.p91id = " & strP91ID

        Dim pars As New List(Of BO.PluginDbParameter)
        Dim dt As DataTable = Master.Factory.pluginBL.GetDataTable(strSQL, pars)

        If dt Is Nothing Then
            Master.Notify("Operace se zastavuje")
            Return ""
        End If

        Dim doc As New XmlDocument
        Dim strInputDIR As String = BO.ASS.GetApplicationRootFolder & "\Plugins"

        Dim nd As XmlNode, s As String, ndHeader As XmlNode = Nothing, strFileName As String = ""
        Dim bolForeignCurrency As Boolean = False, strJ27Code As String = "", dblExchangeRate As Double = 0
        Try
            If dt.Rows(0).Item("j27id") = 2 Then
                'faktura v domácí měně
                doc.Load(strInputDIR & "\pohoda_vzor.xml")
            Else
                'faktura v zahraniční měně
                doc.Load(strInputDIR & "\pohoda_vzor_foreign.xml")
                bolForeignCurrency = True

            End If

        Catch ex As Exception
            Master.Notify(ex.Message)
        End Try


        For Each dbRow In dt.Rows
            If dbRow.Item("p91DateSupply").ToString = "" Or dbRow.Item("p91Code").ToString = "" Then
                Return ""
            End If
            If bolForeignCurrency Then
                strJ27Code = dbRow.Item("j27Code") & ""
                dblExchangeRate = dbRow.Item("p91ExchangeRate")
            End If


            strFileName = strFilePrefix & "_" & dbRow.Item("p91Code").ToString & ".xml"
            nd = doc.ChildNodes(1)

            s = nd.Name & "-" & nd.LocalName & "-" & nd.Value & "-" & nd.InnerText
            nd.Attributes("ico").Value = Trim(Me.txtIC.Text)     'IČ cílové POHODA databáze
            nd.Attributes("application").Value = "MARKTIME"
            nd.Attributes("note").Value = "import faktury"

            nd = nd.FirstChild
            nd.Attributes(0).Value = dbRow.Item("p91id").ToString
            nd = nd.FirstChild
            nd = nd.FirstChild
            ndHeader = nd
            s = nd.Name & "-" & nd.LocalName & "-" & nd.Value & "-" & nd.InnerText


            ChangeChild(ndHeader, "inv:symVar", dbRow.Item("p91Code").ToString)
            ChangeChild(ndHeader, "inv:date", Format(dbRow.Item("p91date"), "yyyy-MM-dd"))
            ChangeChild(ndHeader, "inv:dateTax", Format(dbRow.Item("p91DateSupply"), "yyyy-MM-dd"))
            ChangeChild(ndHeader, "inv:dateDue", Format(dbRow.Item("p91DateMaturity"), "yyyy-MM-dd"))
            ChangeChild(ndHeader, "inv:dateAccounting", Format(dbRow.Item("p91DateSupply"), "yyyy-MM-dd"))
            ChangeChild(ndHeader, "inv:text", dbRow.Item("text90").ToString)
            ChangeChild(ndHeader, "inv:note", dbRow.Item("note90").ToString)

            nd = GetChild(ndHeader, "inv:number")
            ChangeChild(nd, "typ:numberRequested", dbRow.Item("p91Code").ToString)

            'ChangeChild(ndHeader, "inv:numberOrder", "")
            'ChangeChild(ndHeader, "inv:dateOrder", "")

            nd = GetChild(ndHeader, "inv:partnerIdentity")
            nd = GetChild(nd, "typ:address")
            ChangeChild(nd, "typ:company", dbRow.Item("p91Client").ToString)
            ChangeChild(nd, "typ:division", "")
            ChangeChild(nd, "typ:name", "")
            ChangeChild(nd, "typ:city", dbRow.Item("p91ClientAddress1_City").ToString)
            ChangeChild(nd, "typ:street", dbRow.Item("p91ClientAddress1_Street").ToString)
            ChangeChild(nd, "typ:zip", dbRow.Item("p91ClientAddress1_ZIP").ToString)
            ChangeChild(nd, "typ:ico", dbRow.Item("p91Client_RegID").ToString)
            ChangeChild(nd, "typ:dic", dbRow.Item("p91Client_VatID").ToString)

            Dim strBA As String = dbRow.Item("p86BankAccount").ToString
            Dim strBankCode As String = dbRow.Item("p86BankCode").ToString


            nd = GetChild(ndHeader, "inv:account")
            ChangeChild(nd, "typ:accountNo", strBA)
            ChangeChild(nd, "typ:bankCode", strBankCode)

            nd = GetChild(ndHeader, "inv:centre")
            ChangeChild(nd, "typ:ids", dbRow.Item("stredisko").ToString)

            nd = GetChild(ndHeader, "inv:contract")
            ChangeChild(nd, "typ:ids", dbRow.Item("contract").ToString)



        Next
        dt.Clear()

     
        ''strSQL = "select left(p95name,90) as polozka"
        ''strSQL += ",p81Amount_WithoutVat as bezdph"
        ''strSQL += ",p81VatRate as dph_sazba"
        ''strSQL += ",p81Amount_Vat as dph_castka"
        ''strSQL += ",p81Amount_WithVat as vcdph,1 as Poradi"
        ''strSQL += " FROM p81InvoiceAmount a LEFT OUTER JOIN p95InvoiceRow p95 ON a.p95ID=p95.p95ID"
        ''strSQL += " WHERE a.p91id=" & strP91ID & " and a.p81Amount_WithoutVat<>0"
        ''strSQL += " UNION"
        ''strSQL += " SELECT 'Uhrazená záloha' as polozka,-1*(p91ProformaAmount_WithoutVat_None+p91ProformaAmount_WithoutVat_Low+p91ProformaAmount_WithoutVat_Standard) as bezdph"
        ''strSQL += ",p91ProformaAmount_VatRate as dph_sazba"
        ''strSQL += ",-1*(p91ProformaAmount_Vat_Low+p91ProformaAmount_Vat_Standard) as dph_castka"
        ''strSQL += ",-1*p91ProformaAmount as vcdph,1000 as Poradi"
        ''strSQL += " FROM p91Invoice WHERE p91ID=" & strP91ID & " AND isnull(p91ProformaAmount,0)<>0"
        ''strSQL += " UNION SELECT 'Zaokrouhlení' as Polozka,p91RoundFitAmount as bezdph,0 as dph_sazba,0 as dph_castka,p91RoundFitAmount as vcdph,2000 as Poradi FROM p91Invoice WHERE p91ID=9 AND isnull(p91RoundFitAmount,0)<>0"
        ''strSQL += " ORDER BY Poradi,dph_sazba"
        Dim x As Integer
        Dim ndDetail As XmlNode = ndHeader.NextSibling

        ''dt = Master.Factory.pluginBL.GetDataTable(strSQL, Nothing)
        dt = Master.Factory.pluginBL.GetDataTable("exec dbo.p91_get_cenovy_rozpis " & strP91ID & ",1,1,0", Nothing)
        Dim intColumns As Integer = dt.Columns.Count

        If intColumns > 1 Then
            Dim ndInvoiceItem As XmlNode
            For Each dbRow In dt.Rows
                If x > 0 Then
                    nd = GetChild(ndDetail, "inv:invoiceItem")
                    ndInvoiceItem = nd.Clone
                    ndDetail.InsertAfter(ndInvoiceItem, nd)
                Else
                    ndInvoiceItem = GetChild(ndDetail, "inv:invoiceItem")
                End If
                Dim strVatType As String = "high"
                If dbRow.Item("DPHSazba") < 20 Then strVatType = "low"
                If dbRow.Item("DPHSazba") = 0 Then strVatType = "none"

                ChangeChild(ndInvoiceItem, "inv:text", dbRow.Item("Oddil") & "")
                ChangeChild(ndInvoiceItem, "inv:quantity", "1")
                ChangeChild(ndInvoiceItem, "inv:rateVAT", strVatType)

                If Not bolForeignCurrency Then
                    nd = GetChild(ndInvoiceItem, "inv:homeCurrency")
                Else
                    nd = GetChild(ndInvoiceItem, "inv:foreignCurrency")
                End If
                ChangeChild(nd, "typ:unitPrice", FormatNumber(dbRow.Item("bezdph")))
                ChangeChild(nd, "typ:price", FormatNumber(dbRow.Item("bezdph")))
                ChangeChild(nd, "typ:priceVAT", FormatNumber(dbRow.Item("DPH")))
                ChangeChild(nd, "typ:priceSum", FormatNumber(dbRow.Item("vcdph")))

                nd = GetChild(ndInvoiceItem, "inv:accounting")
                ChangeChild(nd, "typ:ids", Trim(txtPredkontace.Text))
                
                x += 1
            Next
        End If

        If bolForeignCurrency Then
            Dim ndSummary As XmlNode = ndDetail.NextSibling
            Dim ndForeignCurrency As XmlNode = GetChild(ndSummary, "inv:foreignCurrency")

            ChangeChild(ndForeignCurrency.FirstChild, "typ:ids", strJ27Code)
            ChangeChild(ndForeignCurrency, "typ:rate", FormatNumber(dblExchangeRate))
        End If


        doc.Save(strExportDir & "\" & strFileName)

        Return strFileName
    End Function

    Private Function GetChild(ByVal ndMaster As XmlNode, ByVal strName As String) As XmlNode
        Dim i As Integer
        For i = 0 To ndMaster.ChildNodes.Count - 1
            If LCase(ndMaster.ChildNodes(i).Name) = LCase(strName) Then
                Return ndMaster.ChildNodes(i)
            End If
        Next
        Return Nothing
    End Function

    Private Sub ChangeChild(ByRef ndMaster As XmlNode, ByVal strName As String, ByVal strNewValue As String)
        Dim nd As XmlNode = GetChild(ndMaster, strName)

        nd.InnerText = strNewValue
    End Sub

    Private Function FormatNumber(ByVal dec As Decimal) As String
        Return Replace(dec.ToString, ",", ".")
    End Function

    Private Function GetRandomFilePrefix() As String
        Return Format(Now, "ddMMyyyyHHmmss")
    End Function
    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            If Not ValidateHeader() Then Return
            Master.Factory.j03UserBL.SetUserParam("p91_export2pohoda_ic", Trim(Me.txtIC.Text))

            Dim strFileName As String = CreateOneInvoice(Master.DataPID.ToString, Master.Factory.x35GlobalParam.TempFolder, GetRandomFilePrefix())
            Response.Redirect("binaryfile.aspx?tempfile=" & strFileName)
        End If
    End Sub

    Private Function ValidateHeader() As Boolean
        If Len(Trim(Me.txtIC.Text)) < 4 Then
            Master.Notify("Musíte zadat IČ organizace.", NotifyLevel.ErrorMessage)
            Return False
        End If
        Return True
    End Function

    Private Sub cmdGenerateBatch_Click(sender As Object, e As EventArgs) Handles cmdGenerateBatch.Click
        If Not ValidateHeader() Then Return
        Master.Factory.j03UserBL.SetUserParam("p91_export2pohoda_ic", Trim(Me.txtIC.Text))

        If period1.x21ID = BO.x21IdEnum._NoQuery Then
            Master.Notify("Musíte zvolit časové období.", NotifyLevel.WarningMessage)
            Return
        End If
        If Not System.IO.File.Exists(BO.ASS.GetApplicationRootFolder & "\Plugins\pohoda_vzor.xml") Then
            Master.Notify("V nastavení systému chybí soubor Plugins\pohoda_vzor.xml.")
            Return
        End If
        Dim strExportDir As String = Master.Factory.x35GlobalParam.TempFolder
        Dim strFilePrefix As String = GetRandomFilePrefix()

        Dim mq As New BO.myQueryP91
        mq.DateFrom = period1.DateFrom
        mq.DateUntil = period1.DateUntil
        mq.PeriodType = BO.myQueryP91_PeriodType.p91DateSupply
        If Me.p93ID.SelectedValue <> "" Then mq.p93ID = BO.BAS.IsNullInt(Me.p93ID.SelectedValue)

        Dim lis As IEnumerable(Of BO.p91Invoice) = Master.Factory.p91InvoiceBL.GetList(mq).Where(Function(p) p.p91IsDraft = False)
        If lis.Count = 0 Then
            Master.Notify("Pro vybrané období neexistuje ani jeden záznam faktury.", NotifyLevel.WarningMessage)
            Return
        End If
        For Each c In lis
            CreateOneInvoice(c.PID.ToString, strExportDir, strFilePrefix)
        Next
        Dim strTempFile As String = MergeAllInOne(strFilePrefix)
        Response.Redirect("binaryfile.aspx?tempfile=" & strTempFile)
    End Sub

    Private Function MergeAllInOne(strFilePrefix As String) As String
        Dim cF As New BO.clsFile, strDirExport As String = Master.Factory.x35GlobalParam.TempFolder
       
        
        Dim files As List(Of String) = cF.GetFileListFromDir(strDirExport, strFilePrefix & "*.xml")
        If files.Count = 0 Then
            Master.Notify(String.Format("Ve složce {0} není ani jeden XML soubor.", strDirExport), NotifyLevel.WarningMessage)
            Return ""
        End If
        Dim docMaster As New XmlDocument
        docMaster.Load(BO.ASS.GetApplicationRootFolder & "\Plugins\pohoda_vzor_allinone.xml")

        Dim ndMaster As XmlNode = docMaster.ChildNodes(1)

        For Each strFile As String In files
            Dim docOne As New XmlDocument
            docOne.Load(strDirExport & "\" & strFile)
            Dim node As XmlNode = docOne.ChildNodes(1)
            node = node.ChildNodes(0)


            Dim xx As XmlElement = docMaster.CreateElement("dat:dataPackItem", docMaster.DocumentElement.NamespaceURI)
            Dim oo As XmlAttribute = docMaster.CreateAttribute("version")
            oo.Value = "1.0"
            xx.Attributes.Append(oo)
            oo = docMaster.CreateAttribute("id")
            oo.Value = node.Attributes("id").Value
            xx.Attributes.Append(oo)


            xx.InnerXml = node.InnerXml
           
            ndMaster.AppendChild(xx)


        Next


        docMaster.Save(strDirExport & "\" & strFilePrefix & "_AllInOne.xml")
        Return strFilePrefix & "_AllInOne.xml"

    End Function
End Class