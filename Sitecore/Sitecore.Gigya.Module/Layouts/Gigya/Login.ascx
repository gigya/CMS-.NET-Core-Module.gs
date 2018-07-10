<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Login.ascx.cs" Inherits="Sitecore.Gigya.Module.Layouts.Gigya.Login" %>
<asp:PlaceHolder runat="server" ID="EmbeddedContainer"></asp:PlaceHolder>
<button id="button" runat="server" type="button" data-gigya-screen="Default-RegistrationLogin" data-gigya-start-screen="" class="gigya-cms-login">Login</button>
<input type="hidden" class="gigya-cms-logged-in-url" id="loggedinurl" runat="server" />