<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Migration.aspx.cs" Inherits="Sitecore.Gigya.Module.sitecore_modules.Gigya.Migration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>Gigya Module Migration</h1>
            <asp:TextBox runat="server" TextMode="MultiLine" ID="Messages" />
            <br />
            <br />
            <asp:Button runat="server" ID="Migrate" Text="Migrate" OnClick="Migrate_Click" />
        </div>
    </form>
</body>
</html>
