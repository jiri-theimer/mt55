<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="report_modal.aspx.vb" Inherits="UI.report_modal" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="Telerik.ReportViewer.WebForms, Version=11.0.17.406, Culture=neutral, PublicKeyToken=a9d7983dfcc261be" Namespace="Telerik.ReportViewer.WebForms" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {

            $(".slidingDiv1").hide();
            $(".show_hide1").show();

            $('.show_hide1').click(function () {

                $(".slidingDiv1").slideToggle();
            });

            var h1 = new Number;
            var h2 = new Number;
            var hh = new Number;

            h1 = $(window).height();

            ss = self.document.getElementById("offsetY");
            var offset = $(ss).offset();

            h2 = offset.top;

            hh = h1 - h2 - 60 - 10;


            self.document.getElementById("divReportViewer").style.height = hh + "px";

            <%If Me.MultiPIDs<>"" then%>
            self.document.getElementById("divReportViewer").style.display = "none";
            <%end If%>


            
        })

        $(window).load(function () {
            <%If hidOutputFullPathPdf.Value<>"" then%>
            var s=document.getElementById("<%=hidOutputFullPathPdf.ClientID%>").value;
            document.getElementById("<%=hidOutputFullPathPdf.ClientID%>").value="";                               
            var url="binaryfile.aspx?disposition=inline&tempfile="+s;

            <%If LCase(Request.Browser.Browser) = "ie" Then%>
                
                url="binaryfile.aspx?tempfile="+s;
                $.alert("V prohlížeči Internet Explorer bohužel nefunguje PDF náhled.");
                
            <%End If%>
             

            dialog_master(url,true);

            <%end If%>

            try {
                //hackování reportviewer - přednastavení pdf exportu jako default
                $('[id*="ReportToolbar_ExportGr_FormatList_DropDownList"]')[0].selectedIndex =1;
            
                document.getElementById("MainContent_rv1_ReportToolbar_ExportGr_Export").className = "ActiveLink";
            
                ReportTextButton('MainContent_rv1_ReportToolbar_ExportGr_Export', 'Export', false, 'ActiveLink', 'DisabledLink');
            }
            catch (err) {
                //nic
            }
           
            

        })

        function hardrefresh(pid, flag) {

            document.getElementById("<%=hidHardRefreshFlag.clientid%>").value = flag;
            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdRefreshOnBehind, "", False)%>;

        }

        function x31_record() {
            dialog_master("x31_record.aspx?pid=<%=me.CurrentX31ID%>",true)
            
        }
        function rvprint() {
            <%=rv1.ClientID%>.PrintReport(); 
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="3" cellspacing="2">
        <tr>

            <td>
                <asp:Label ID="lblX31ID" runat="server" Text="<%$Resources:report_modal,Sestava%>" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="x31ID" runat="server" AutoPostBack="true" DataValueField="pid" DataTextField="NameWithFormat" Style="width: 350px;" BackColor="yellow"></asp:DropDownList>
            </td>
            <td>
                <uc:periodcombo ID="period1" runat="server" Width="250px" Visible="false"></uc:periodcombo>
            </td>

            <td>
                <asp:Button ID="cmdRefresh" runat="server" Text="<%$Resources:common,refresh%>" CssClass="cmd" />
            </td>
          

            <td align="right">
               
            </td>
        </tr>
    </table>

    <div id="offsetY"></div>
    <div class="slidingDiv1" style="padding: 10px;">
        <div class="content-box2">
            <div class="title">
                <img src="Images/merge.png" />
                <img src="Images/pdf.png" />
                <span>Sloučení sestavy s až 3 dalšími výstupy do jediného PDF dokumentu</span>                
            </div>
            <div class="content">
                <table cellpadding="3" cellspacing="2">
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Dále sloučit s:" CssClass="lbl"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="x31ID_Merge1" runat="server" AutoPostBack="false" DataValueField="pid" DataTextField="x31Name" Style="width: 400px;"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Dále sloučit s:" CssClass="lbl"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="x31ID_Merge2" runat="server" AutoPostBack="false" DataValueField="pid" DataTextField="x31Name" Style="width: 400px;"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Dále sloučit s:" CssClass="lbl"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="x31ID_Merge3" runat="server" AutoPostBack="false" DataValueField="pid" DataTextField="x31Name" Style="width: 400px;"></asp:DropDownList>
                        </td>
                    </tr>
                </table>                
                <asp:Button ID="cmdMergePDF_Download" runat="server" Text="Vygenerovat PDF dokument" CssClass="cmd"  />
                <asp:Button ID="cmdMergePDF_SendMail" runat="server" Text="Odeslat poštou" CssClass="cmd" style="margin-left:10px;" />
                <asp:Button ID="cmdMergePDF_Preview" runat="server" Text="Přejít na PDF náhled" CssClass="cmd" style="margin-left:10px;" />
                
            </div>
        </div>
    </div>
    <asp:Label ID="multiple_records" runat="server"></asp:Label>
    <asp:RadioButtonList ID="opgDocResultType" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" Visible="false">
        <asp:ListItem Text="PDF dokument" Value="pdf" Selected="true"></asp:ListItem>
        <asp:ListItem Text="DOCX dokument" Value="docx"></asp:ListItem>
        <asp:ListItem Text="RTF dokument" Value="rtf"></asp:ListItem>
    </asp:RadioButtonList>
    <asp:HyperLink ID="cmdDocMergeResult" runat="server" Text="Zobrazit výsledek" Visible="false"></asp:HyperLink>
    <asp:HyperLink ID="cmdXlsResult" runat="server" Text="XLS výstup" Visible="false"></asp:HyperLink>
    
    <div id="divReportViewer" style="height: 300px;">
        <telerik:ReportViewer ID="rv1" runat="server" Width="100%" Height="100%" ShowParametersButton="true" ShowHistoryButtons="false" ValidateRequestMode="Disabled" ShowExportGroup="true">                        
            <Resources PrintToolTip="Tisk" ExportSelectFormatText="Exportovat do zvoleného formátu" TogglePageLayoutToolTip="Přepnout na náhled k tisku" NextPageToolTip="Další strana" PreviousPageToolTip="Předchozí strana" RefreshToolTip="Obnovit" LastPageToolTip="Poslední strana" FirstPageToolTip="První strana" ></Resources>
        </telerik:ReportViewer>        
    </div>
    
    <asp:HiddenField ID="hidX29ID" runat="server" Value="141" />
    <asp:HiddenField ID="hidPrefix" runat="server" Value="p41" />
    <asp:HiddenField ID="hidHardRefreshFlag" runat="server" />
    <asp:HiddenField ID="hidHardRefreshPID" runat="server" />
    <asp:HiddenField ID="hidPIDS" runat="server" />
    <asp:LinkButton ID="cmdRefreshOnBehind" runat="server" Text="refreshonbehind" Style="display: none;"></asp:LinkButton>
    <asp:HiddenField ID="hidOutputFullPathPdf" runat="server" />

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
