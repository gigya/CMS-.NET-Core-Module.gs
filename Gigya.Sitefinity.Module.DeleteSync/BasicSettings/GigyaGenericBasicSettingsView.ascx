<%@ Control Language="C#" %>
<%@ Register Assembly="Telerik.Sitefinity" Namespace="Telerik.Sitefinity.Web.UI" TagPrefix="sf" %>
<%@ Register Assembly="Telerik.Sitefinity" Namespace="Telerik.Sitefinity.Configuration.Web.UI.Basic" TagPrefix="sf" %>
<%@ Register Assembly="Telerik.Sitefinity" Namespace="Telerik.Sitefinity.Web.UI.FieldControls" TagPrefix="sfFields" %>
<%@ Register Assembly="Telerik.Sitefinity" Namespace="Telerik.Sitefinity.Web.UI.Fields" TagPrefix="sfFields" %>

<sfFields:FormManager ID="formManager" runat="server" />
<sf:ResourceLinks id="resourcesLinks1" runat="server">
    <sf:EmbeddedResourcePropertySetter Name="Telerik.Sitefinity.Resources.Themes.Light.Images.Loadings.sfLoadingFormBtns.gif"
        Static="true" ControlID="loadingImage1" ControlPropertyName="ImageUrl" />
    <sf:ResourceFile Name="Styles/MaxWindow.css" />
</sf:ResourceLinks>
<sf:ClientLabelManager id="clientLabelManager" runat="server">
    <Labels>
        <sf:ClientLabel ClassId="Labels" Key="ChangesSuccessfullySaved" runat="server" />
    </Labels>
</sf:ClientLabelManager>

<sf:Message 
    runat="server" 
    ID="message" 
    ElementTag="div"  
    CssClass="sfMessage sfDialogMessage"
    RemoveAfter="5000"  
    FadeDuration="10" />

<sf:BasicSettingsSitePanel id="basicSettingsSitePanel" runat="server"></sf:BasicSettingsSitePanel>

<asp:PlaceHolder id="fieldsContainer" runat="server"></asp:PlaceHolder>

<p class="sfButtonArea" id="buttonsArea" runat="server">                        
    <asp:HyperLink id="btnSave" runat="server" CssClass="sfLinkBtn sfSave">
        <strong class="sfLinkBtnIn"><asp:Literal ID="Literal5" runat="server" Text='<%$ Resources:Labels, SaveChangesLabel %>' /></strong>
    </asp:HyperLink>
</p>
<div id="loadingView" runat="server" style="display: none;" class="sfLoadingFormBtns sfButtonArea">
    <sf:SfImage ID="loadingImage1" runat="server" AlternateText="<%$Resources:Labels, SavingImgAlt %>" />
</div>
<sfFields:FieldControlsBinder ID="fieldsBinder" runat="server" TargetId="fieldsBinder" DataKeyNames="Id"></sfFields:FieldControlsBinder>


