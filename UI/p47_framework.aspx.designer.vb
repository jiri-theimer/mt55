﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated. 
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Partial Public Class p47_framework

    '''<summary>
    '''lblHeader control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents lblHeader As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''query_year control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents query_year As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''query_month control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents query_month As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''cmdPrevMonth control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents cmdPrevMonth As Global.System.Web.UI.WebControls.ImageButton

    '''<summary>
    '''cmdNextMonth control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents cmdNextMonth As Global.System.Web.UI.WebControls.ImageButton

    '''<summary>
    '''find_p41id control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents find_p41id As Global.UI.project

    '''<summary>
    '''cmdExport control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents cmdExport As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''cbxGroupBy control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents cbxGroupBy As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''panPersonScope control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents panPersonScope As Global.System.Web.UI.WebControls.Panel

    '''<summary>
    '''j02ID_Add control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents j02ID_Add As Global.UI.person

    '''<summary>
    '''cmdAppendJ02IDs control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents cmdAppendJ02IDs As Global.System.Web.UI.WebControls.LinkButton

    '''<summary>
    '''cmdReplaceJ02IDs control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents cmdReplaceJ02IDs As Global.System.Web.UI.WebControls.LinkButton

    '''<summary>
    '''j11ID_Add control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents j11ID_Add As Global.UI.datacombo

    '''<summary>
    '''j07ID_Add control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents j07ID_Add As Global.UI.datacombo

    '''<summary>
    '''grid1 control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents grid1 As Global.UI.datagrid

    '''<summary>
    '''cmdHardRefreshOnBehind control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents cmdHardRefreshOnBehind As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''hidJ02IDs control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents hidJ02IDs As Global.System.Web.UI.WebControls.HiddenField

    '''<summary>
    '''hidCurP41ID control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents hidCurP41ID As Global.System.Web.UI.WebControls.HiddenField

    '''<summary>
    '''hidHardRefreshFlag control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents hidHardRefreshFlag As Global.System.Web.UI.WebControls.HiddenField

    '''<summary>
    '''Master property.
    '''</summary>
    '''<remarks>
    '''Auto-generated property.
    '''</remarks>
    Public Shadows ReadOnly Property Master() As UI.Site
        Get
            Return CType(MyBase.Master, UI.Site)
        End Get
    End Property
End Class
