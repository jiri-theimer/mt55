<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p31_record_router.aspx.vb" Inherits="UI.p31_record_router" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content-box2">
        <div class="title">
            Vyberte si worksheet záznam
        </div>
        <div class="content">
            <table cellpadding="10">
                <tr>
                   
                    <th>Datum</th>
                    <th>Osoba</th>
                    <th>Projekt</th>
                    <th>Aktivita</th>
                    <th>Hodnota</th>
                    <th>Popis</th>
                </tr>
                <asp:Repeater ID="rp1" runat="server">
                    <ItemTemplate>
                        <tr class="trHover" style="vertical-align:top;">
                           
                            <td style="width:100px">
                                <%# BO.BAS.FD(Eval("p31Date"))%>   
                                <div>
                                    <a href="p31_record.aspx?pid=<%# Eval("pid")%>">Přejít na záznam</a>
                                </div>
                            </td>
                            <td>
                                <%# Eval("Person")%>   
                            </td>
                            <td>
                                <%# Eval("ClientName")%>   
                                <div><%# Eval("p41Name")%> </div>
                            </td>
                        
                            <td>
                                <%# Eval("p34Name")%>   
                                <div>
                                 <%# Eval("p32Name")%> 
                                </div>
                                
                            </td>
                            <td align="right">
                                <%# Eval("p31Value_Orig")%>   
                            </td>
                            <td>
                                <%# BO.BAS.CrLfText2Html(Eval("p31Text"))%>   
                            </td>
                            
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
