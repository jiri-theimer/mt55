<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="isirinfo.aspx.vb" Inherits="UI.isirinfo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server" method="post" action="http://www.isir.info/api/importsubjects">
        <div>
            <input name="username" type="text" value="jiri.theimer@gmail.com" />
        </div>
        <div>
            <input name="password" type="text" value="123456" />
        </div>
        <div>
            <input name="subjects" type="text" value="<subjects><subject><firstname>Jiří</firstname><name>THEIMER</name><rc>1122233273</rc></subject><subject><name>ITELLIGENCE</name><ic>26718537</ic></subject></subjects>" />
        </div>

        <button type="button" onclick="form1.submit()">odeslat</button>
    </form>
</body>
</html>
