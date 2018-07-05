<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Migration.aspx.cs" Inherits="Sitecore.Gigya.Migration.sitecore_modules.Gigya.Migration" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Gigya Module Migration</title>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css" integrity="sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm" crossorigin="anonymous" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Gigya Module Migration</h1>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="Database">Database:</asp:Label>
                <asp:TextBox runat="server" CssClass="form-control" Text="master" ID="Database" />
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="Messages">Log:</asp:Label>
                <asp:TextBox runat="server" CssClass="form-control" TextMode="MultiLine" ID="Messages" ReadOnly="true" />
            </div>
            <asp:Button runat="server" CssClass="btn btn-primary" ID="Migrate" Text="Migrate" OnClick="Migrate_Click" />
        </div>
    </form>
</body>
</html>
