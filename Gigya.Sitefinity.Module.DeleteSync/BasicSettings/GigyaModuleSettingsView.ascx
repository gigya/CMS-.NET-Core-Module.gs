<%@ Control Language="C#" %>
<%@ Register Assembly="Telerik.Sitefinity" Namespace="Telerik.Sitefinity.Web.UI.Fields" TagPrefix="sfFields" %>
<%@ Register Assembly="Telerik.Sitefinity" Namespace="Telerik.Sitefinity.Web.UI" TagPrefix="sf" %>
<%@ Register Assembly="Telerik.Sitefinity" Namespace="Telerik.Sitefinity.Web.UI.FieldControls" TagPrefix="sfFields" %>

<style>
    .sfTxt { margin-bottom: 10px; }
    .sfDropdownList { display: inline-block; margin-bottom: 10px; }
    .required { color: red; }
    select { padding: 2px 2px 3px 2px; }
    .label-after-field { display: inline; }
    #session-timeout-wrapper input { margin: 0; }
</style>

<div class="sfSettingsSection">
    <h2>
        <asp:Literal runat="server" ID="lGeneralSettings" Text="Gigya Delete Sync Settings" />
    </h2>
</div>

<sf:ResourceLinks id="resourcesLinks1" runat="server">
	<sf:EmbeddedResourcePropertySetter Name="Telerik.Sitefinity.Resources.Themes.Light.Images.Loadings.sfLoadingFormBtns.gif"
		Static="true" ControlID="GigyaLoadingImage" ControlPropertyName="ImageUrl" />
</sf:ResourceLinks>

<div id="loading-view" class="sfLoadingFormBtns sfButtonArea">
	<sf:SfImage ID="GigyaLoadingImage" runat="server" AlternateText="<%$Resources:Labels, SavingImgAlt %>" />
</div>

<div id="gigya-settings-section" style="display: none;" class="sfSettingsSection">
    <div style="margin-bottom: 5px;">
        <sfFields:ChoiceField Title="Enabled" runat="server" DataFieldName="Enabled" CssClass="sfRadioList" DisplayMode="Write" RenderChoicesAs="SingleCheckBox">
            <Choices>
                <sfFields:ChoiceItem Text="Yes" Value="1" />
            </Choices>
        </sfFields:ChoiceField>
    </div>
    <sfFields:ChoiceField Title="Action<span class='required'>*</span>" runat="server" DataFieldName="Action" DisplayMode="Write" RenderChoicesAs="DropDown">
        <Choices>
            <sfFields:ChoiceItem Text="Delete Notification" Value="0" />
            <sfFields:ChoiceItem Text="Full User Deletion" Value="1" />
        </Choices>
    </sfFields:ChoiceField>
    <div class="sfInlineBlock" style="line-height: 1.6; margin-bottom: 10px;">
        Full user deletion means all the data associated with this user will be permanently deleted from CMS's database. 
        Deletion notification means that the user is not deleted, but an indication is added that they were deleted from Gigya. In this case, you should usually add custom code to handle the user data in CMS's database. For more information, see Gigya's Developer Guide.
    </div>
    <sfFields:TextField runat="server"
        DataFieldName="FrequencyMins"
        Title="Delete Job frequency (minutes)<span class='required'>*</span>"
        DisplayMode="Write"
        MaxCharactersCount="255"
        Tooltip=""
        WrapperTag="div">
    </sfFields:TextField>
    <sfFields:TextField runat="server"
        DataFieldName="EmailsOnSuccess"
        Title="Email on success"
        DisplayMode="Write"
        MaxCharactersCount="255"
        Tooltip="Array of emails with comma separator."
        WrapperTag="div">
    </sfFields:TextField>
    <sfFields:TextField runat="server"
        DataFieldName="EmailsOnFailure"
        Title="Email on failure<span class='required'>*</span>"
        DisplayMode="Write"
        MaxCharactersCount="255"
        Tooltip="Array of emails with comma separator."
        WrapperTag="div">
    </sfFields:TextField>
    <sfFields:TextField runat="server"
        DataFieldName="S3BucketName"
        Title="S3 bucket name<span class='required'>*</span>"
        DisplayMode="Write"
        MaxCharactersCount="255"
        WrapperTag="div">
    </sfFields:TextField>
    <sfFields:TextField runat="server"
        DataFieldName="S3AccessKey"
        Title="S3 access key<span class='required'>*</span>"
        DisplayMode="Write"
        MaxCharactersCount="255"
        WrapperTag="div">
    </sfFields:TextField>
    <div class="application-secret-wrapper" style="display: none; margin-bottom: 5px;">
        <div class="application-secret-masked">
            <div class="sfTxtLbl sfInlineBlock">
                S3 secret key<span class='required'>*</span>
            </div>
            <sfFields:TextField runat="server"
                DataFieldName="S3SecretKeyMasked"
                DisplayMode="Read"
                MaxCharactersCount="255"
                WrapperTag="div">
            </sfFields:TextField>
            <a href="#" id="edit-application-secret">Edit</a>
        </div>
        <div style="display: none;" class="application-secret">
            <sfFields:TextField runat="server"
                DataFieldName="S3SecretKey"
                Title="S3 secret key<span class='required'>*</span>"
                DisplayMode="Write"
                MaxCharactersCount="255"
                WrapperTag="div">
            </sfFields:TextField>
            <a href="#" id="cancel-application-secret">Cancel</a>
        </div>
    </div>
    <sfFields:TextField runat="server"
        DataFieldName="S3ObjectKeyPrefix"
        Title="S3 object key prefix<span class='required'>*</span>"
        DisplayMode="Write"
        MaxCharactersCount="255"
        WrapperTag="div">
    </sfFields:TextField>
    <div style="display: none;" id="loaded-field-wrapper">
        <sfFields:TextField runat="server"
            DataFieldName="LoadedField"
            DisplayMode="Write"
            WrapperTag="div">
        </sfFields:TextField>
    </div>
    <div style="display: none;" id="can-view-app-secret-wrapper">
        <sfFields:TextField runat="server"
            DataFieldName="CanViewSecretKey"
            DisplayMode="Write"
            WrapperTag="div">
        </sfFields:TextField>
    </div>
</div>

<script>
    var gigyaSettings = {
        init: function () {
            $('#gigya-settings-section :text').attr('autocomplete', 'off');

            $('#edit-application-secret').click(function () {
                $('.application-secret-masked').hide();
                $('.application-secret').show();
                return false;
            });

            $('#cancel-application-secret').click(function () {
                $('.application-secret :text').val('');
                $('.application-secret-masked').show();
                $('.application-secret').hide();
                return false;
            });
        }
    };

    // poll to check if user allowed to see fields - I can't find a 'sitefinity data service complete' event
    window.loadedTimeout = setInterval(function () {
        if ($('#loaded-field-wrapper :text').val().length > 0) {
            clearInterval(window.loadedTimeout);

            gigyaSettings.init();

            if ($('#can-view-app-secret-wrapper :text').val() === 'true') {
                $('.application-secret-wrapper').show();
            }

            $('#loading-view').hide();
            $('#gigya-settings-section').fadeIn(1000);
        }
    }, 500);
</script>