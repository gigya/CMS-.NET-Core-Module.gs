<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Register.ascx.cs" Inherits="Sitecore.Gigya.Module.Layouts.Gigya.Register" %>
<asp:PlaceHolder runat="server" ID="EmbeddedContainer"></asp:PlaceHolder>
<button id="button" runat="server" type="button" class="gigya-cms-register" data-gigya-screen="" data-gigya-start-screen="">Register</button>
<input type="hidden" class="gigya-cms-registered-logged-in-url" id="loggedinurl" runat="server" />

<%--@if (!string.IsNullOrEmpty(Model.ContainerId))
{
    <div id="@Model.GeneratedContainerId" data-gigya-container-id="@Model.ContainerId" class="gigya-cms-embedded-screen" data-gigya-screen="@Model.ScreenSet" data-gigya-start-screen="@Model.StartScreen"></div>
}
else
{
    <button type="button" class="gigya-cms-register" data-gigya-screen="@Model.ScreenSet" data-gigya-start-screen="@Model.StartScreen">@Model.Label</button>
}
<input type="hidden" class="gigya-cms-registered-logged-in-url" value="@Model.LoggedInUrl" />--%>