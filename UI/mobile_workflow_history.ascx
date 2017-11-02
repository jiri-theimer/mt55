<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="mobile_workflow_history.ascx.vb" Inherits="UI.mobile_workflow_history" %>

<table class="table table-hover">
    <asp:Repeater ID="rp1" runat="server">
        <ItemTemplate>
            <tr>
                <td>
                    <div>
                        <%#Eval("b07WorkflowInfo")%>
                    </div>
                    <div>
                        <i><%#BO.BAS.CrLfText2Html(Eval("b07Value"))%></i>
                    </div>
                    <div style="padding-top:4px;">
                        <%#Eval("UserInsert")%>/<%#BO.BAS.FD(Eval("DateInsert"), True, True)%>                        
                    </div>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</table>
