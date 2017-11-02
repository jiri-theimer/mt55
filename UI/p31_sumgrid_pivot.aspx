<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p31_sumgrid_pivot.aspx.vb" Inherits="UI.p31_sumgrid_pivot" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    

    <telerik:RadClientExportManager runat="server" ID="RadClientExportManager1">
        <PdfSettings Author="MARKTIME" FileName="MARKTIME_PIVOT.pdf" PaperSize="A4" Landscape="true" />        
    </telerik:RadClientExportManager>
    <div id="divContainer" style="width:100%;">
    <telerik:RadPivotGrid runat="server" ID="pivot1" AllowPaging="true" PageSize="100" ShowColumnHeaderZone="true" AllowSorting="true" AllowFiltering="false" ShowRowHeaderZone="true" ShowFilterHeaderZone="true" ShowDataHeaderZone="true" AllowNaturalSort="false" Skin="Metro"
        FilterHeaderZoneText="Sem přetáhněte nevyužívané sloupce/veličiny" ColumnHeaderZoneText="Sem přetáhněte pivot pole" RowHeaderZoneText="Sem přetáhněte řádková pole" DataHeaderZoneText="Sem přetáhněte součtové veličiny" NoRecordsText="Žádná data k zobrazení">
        <DataCellStyle BackColor="white" />
        <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="false"></PagerStyle>
        <TotalsSettings GrandTotalText="Celkem" TotalValueFormat="{0:F2}" ValueTotalFormat="{0:F2}" />

        <Fields>
            <telerik:PivotGridRowField UniqueName="row1" TotalFormatString="{0:F2}" ></telerik:PivotGridRowField>
            <telerik:PivotGridRowField UniqueName="row2" TotalFormatString="{0:F2}" ></telerik:PivotGridRowField>

            <telerik:PivotGridReportFilterField UniqueName="c1"></telerik:PivotGridReportFilterField>
            <telerik:PivotGridReportFilterField UniqueName="c2"></telerik:PivotGridReportFilterField>
            <telerik:PivotGridReportFilterField UniqueName="c3"></telerik:PivotGridReportFilterField>
            <telerik:PivotGridReportFilterField UniqueName="c4"></telerik:PivotGridReportFilterField>
            <telerik:PivotGridReportFilterField UniqueName="c5"></telerik:PivotGridReportFilterField>
            <telerik:PivotGridReportFilterField UniqueName="c6"></telerik:PivotGridReportFilterField>
            <telerik:PivotGridReportFilterField UniqueName="c7"></telerik:PivotGridReportFilterField>
            <telerik:PivotGridReportFilterField UniqueName="c8"></telerik:PivotGridReportFilterField>
            <telerik:PivotGridReportFilterField UniqueName="c9"></telerik:PivotGridReportFilterField>
            
            


            <telerik:PivotGridAggregateField UniqueName="s1" Aggregate="Sum" DataFormatString="{0:F2}" GrandTotalAggregateFormatString="{0:F2}" TotalFormatString="{0:F2}">                
            </telerik:PivotGridAggregateField>
            <telerik:PivotGridAggregateField UniqueName="s2" Aggregate="Sum" DataFormatString="{0:F2}" GrandTotalAggregateFormatString="{0:F2}" TotalFormatString="{0:F2}">                
            </telerik:PivotGridAggregateField>
            <telerik:PivotGridAggregateField UniqueName="s3" Aggregate="Sum" DataFormatString="{0:F2}" GrandTotalAggregateFormatString="{0:F2}" TotalFormatString="{0:F2}">                
            </telerik:PivotGridAggregateField>
            <telerik:PivotGridAggregateField UniqueName="s4" Aggregate="Sum" DataFormatString="{0:F2}" GrandTotalAggregateFormatString="{0:F2}" TotalFormatString="{0:F2}">                
            </telerik:PivotGridAggregateField>
            <telerik:PivotGridAggregateField UniqueName="s5" Aggregate="Sum" DataFormatString="{0:F2}" GrandTotalAggregateFormatString="{0:F2}" TotalFormatString="{0:F2}">                
            </telerik:PivotGridAggregateField>
            <telerik:PivotGridAggregateField UniqueName="s6" Aggregate="Sum" DataFormatString="{0:F2}" GrandTotalAggregateFormatString="{0:F2}" TotalFormatString="{0:F2}">                
            </telerik:PivotGridAggregateField>
            <telerik:PivotGridAggregateField UniqueName="s7" Aggregate="Sum" DataFormatString="{0:F2}" GrandTotalAggregateFormatString="{0:F2}" TotalFormatString="{0:F2}">                
            </telerik:PivotGridAggregateField>
            <telerik:PivotGridAggregateField UniqueName="s8" Aggregate="Sum" DataFormatString="{0:F2}" GrandTotalAggregateFormatString="{0:F2}" TotalFormatString="{0:F2}">                
            </telerik:PivotGridAggregateField>
            <telerik:PivotGridAggregateField UniqueName="s9" Aggregate="Sum" DataFormatString="{0:F2}" GrandTotalAggregateFormatString="{0:F2}" TotalFormatString="{0:F2}">                
            </telerik:PivotGridAggregateField>
        </Fields>

        <ConfigurationPanelSettings EnableDragDrop="false" />
        <ClientSettings EnableFieldsDragDrop="true" ClientMessages-DragToReorder="Drag to reorder"></ClientSettings>
    </telerik:RadPivotGrid>
    </div>

    <div class="div6">
        <asp:Button ID="cmdRefreshChart" runat="server" CssClass="cmd" Text="Zobrazit graf"  Visible="false"/>
        <asp:DropDownList ID="cbxChartType" runat="server" AutoPostBack="true" ToolTip="Typ grafu" Visible="false">
            <asp:ListItem Text="Sloupcový" Value="1"></asp:ListItem>
            <asp:ListItem Text="Pruhový" Value="2"></asp:ListItem>
            <asp:ListItem Text="Čárový" Value="3"></asp:ListItem>
            <asp:ListItem Text="Plošný" Value="4"></asp:ListItem>
            
        </asp:DropDownList>
        <asp:DropDownList ID="cbxChartWidth" runat="server" AutoPostBack="true" ToolTip="Šířka grafu v pixelech" Visible="false">
            <asp:ListItem Text="800" Value="800"></asp:ListItem>
            <asp:ListItem Text="1000" Value="1000" Selected="true"></asp:ListItem>
            <asp:ListItem Text="1500" Value="1500"></asp:ListItem>
            <asp:ListItem Text="1900" Value="1900"></asp:ListItem>
        </asp:DropDownList>
    </div>
    
    <asp:Panel ID="panChart1" runat="server" Visible="false">
        <telerik:RadHtmlChart runat="server" ID="chart1" Width="1000px" Font-Size="Small">
            <ChartTitle Text="">
            </ChartTitle>            
            <PlotArea>                
                <Series>
                    <telerik:ColumnSeries Name="Hodiny Fa" DataFieldY="Sum1" >
                        <Appearance FillStyle-BackgroundColor="LightGreen"></Appearance>
                        <LabelsAppearance DataFormatString="{0:F2}"></LabelsAppearance>
                    </telerik:ColumnSeries>
                    <telerik:ColumnSeries Name="Hodiny NeFa" DataFieldY="Sum2">
                        <Appearance FillStyle-BackgroundColor="#ff9999"></Appearance>
                        <LabelsAppearance DataFormatString="{0:F2}"></LabelsAppearance>
                    </telerik:ColumnSeries>
                </Series>
                <XAxis DataLabelsField="Row1" Name="Klient">
                    <LabelsAppearance RotationAngle="0"></LabelsAppearance>
                    <MinorGridLines Visible="false" />
                    <MajorGridLines Visible="false" />
                </XAxis>
                <YAxis>
                    <MinorGridLines Visible="false" />
                    <MajorGridLines Visible="false" />
                </YAxis>
            </PlotArea>
        </telerik:RadHtmlChart>
    </asp:Panel>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">

    <script type="text/javascript">
        var $ = $telerik.$;

        function exportRadPivotGrid() {
            document.getElementById("divContainer").style.width = "800px";
            var exp = $find("<%= RadClientExportManager1.ClientID%>");
            exp.exportPDF($(".RadPivotGrid"));
            document.getElementById("divContainer").style.width = "100%";
        }
    </script>
</asp:Content>
