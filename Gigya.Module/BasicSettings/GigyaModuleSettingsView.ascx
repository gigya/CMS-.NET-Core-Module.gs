<%@ Control Language="C#" %>
<%@ Register Assembly="Telerik.Sitefinity" Namespace="Telerik.Sitefinity.Web.UI.Fields" TagPrefix="sfFields" %>
<%@ Register Assembly="Telerik.Sitefinity" Namespace="Telerik.Sitefinity.Web.UI" TagPrefix="sf" %>
<%@ Register Assembly="Telerik.Sitefinity" Namespace="Telerik.Sitefinity.Web.UI.FieldControls" TagPrefix="sfFields" %>

<style>
    .sfTxt { margin-bottom: 10px; }
    .mapping-fields-table td { padding: 5px 0 5px 5px; vertical-align: middle; }
    .mapping-fields-table th { padding-left: 5px; }
    .mapping-fields-table tr td:first-child, .mapping-fields-table tr th:first-child { padding-left: 0; }
    .add-mapping-field { display: inline-block; margin-bottom: 10px; }
    .mapping-fields-table .sfTxt { width: auto; margin: 0; }
    .sfDropdownList { display: inline-block; margin-bottom: 10px; }
    .required { color: red; }
    select { padding: 2px 2px 3px 2px; }
    #session-management-wrapper br { display: none; }
    #session-management-wrapper br + input { margin-left: 10px; }
    #gigya-session-mode-wrapper, #gigya-session-mode-wrapper div { display: inline; }
    #session-timeout-wrapper, #session-timeout-wrapper div { display: inline; }
    .label-after-field { display: inline; }
    #gigya-session-mode-wrapper { margin-right: 10px; }
    #session-timeout-wrapper input { margin: 0; }
</style>

<div class="sfSettingsSection">
    <h2>
        <asp:Literal runat="server" ID="lGeneralSettings" Text="Gigya Settings" />
    </h2>
    <p>Settings will be verified before being saved to the database.</p>
</div>

<sf:ResourceLinks id="resourcesLinks1" runat="server">
	<sf:EmbeddedResourcePropertySetter Name="Telerik.Sitefinity.Resources.Themes.Default.Images.Loadings.sfLoadingFormBtns.gif"
		Static="true" ControlID="GigyaLoadingImage" ControlPropertyName="ImageUrl" />
</sf:ResourceLinks>

<div id="loading-view" class="sfLoadingFormBtns sfButtonArea">
	<sf:SfImage ID="GigyaLoadingImage" runat="server" AlternateText="<%$Resources:Labels, SavingImgAlt %>" />
</div>

<div id="gigya-settings-section" style="display: none;" class="sfSettingsSection">
    <sfFields:TextField ID="ApiKeyField" runat="server"
        DataFieldName="ApiKey"
        Title="API Key<span class='required'>*</span>"
        DisplayMode="Write"
        MaxCharactersCount="255"
        WrapperTag="div">
    </sfFields:TextField>
    <div id="profile-properties-wrapper" style="display: none;">
        <sfFields:TextField runat="server"
            DataFieldName="ProfileProperties"
            Title="Profile Properties"
            DisplayMode="Write"
            WrapperTag="div">
        </sfFields:TextField>
    </div>
    <sfFields:TextField ID="ApplicationKeyField" runat="server"
        DataFieldName="ApplicationKey"
        Title="Application Key<span class='required'>*</span>"
        DisplayMode="Write"
        MaxCharactersCount="255"
        WrapperTag="div">
    </sfFields:TextField>
    <div class="application-secret-wrapper" style="display: none;">
        <div class="application-secret-masked">
            <div class="sfTxtLbl sfInlineBlock">
                Application Secret<span class='required'>*</span>
            </div>
            <sfFields:TextField ID="ApplicationSecretMaskedField" runat="server"
                DataFieldName="ApplicationSecretMasked"
                DisplayMode="Read"
                MaxCharactersCount="255"
                WrapperTag="div">
            </sfFields:TextField>
            <a href="#" id="edit-application-secret">Edit</a>
        </div>
        <div style="display: none;" class="application-secret">
            <sfFields:TextField ID="ApplicationSecretField" runat="server"
                DataFieldName="ApplicationSecret"
                Title="Application Secret<span class='required'>*</span>"
                DisplayMode="Write"
                MaxCharactersCount="255"
                WrapperTag="div">
            </sfFields:TextField>
            <a href="#" id="cancel-application-secret">Cancel</a>
        </div>
    </div>

    <div class="sfTxtLbl sfInlineBlock">
        Language<span class='required'>*</span>
    </div>
    <div id="language-wrapper">
        <sfFields:ChoiceField ID="LanguageField" runat="server" DataFieldName="Language" DisplayMode="Write" RenderChoicesAs="DropDown">
            <Choices>
                <sfFields:ChoiceItem Text="Auto" Value="auto" />
                <sfFields:ChoiceItem Text="Other" Value="" />
                <sfFields:ChoiceItem Text="English (default)" Value="en" />
                <sfFields:ChoiceItem Text="Arabic" Value="ar" />
                <sfFields:ChoiceItem Text="Bulgarian" Value="br" />
                <sfFields:ChoiceItem Text="Catalan" Value="ca" />
                <sfFields:ChoiceItem Text="Chinese (Mandarin)" Value="zh-cn" />
                <sfFields:ChoiceItem Text="Chinese (Hong Kong)" Value="zh-hk" />
                <sfFields:ChoiceItem Text="Chinese (Taiwan)" Value="zh-tw" />
                <sfFields:ChoiceItem Text="Croatian" Value="hr" />
                <sfFields:ChoiceItem Text="Czech" Value="cs" />
                <sfFields:ChoiceItem Text="Danish" Value="da" />
                <sfFields:ChoiceItem Text="Dutch" Value="nl" />
                <sfFields:ChoiceItem Text="Dutch Informal" Value="nl-inf" />
                <sfFields:ChoiceItem Text="Finnish" Value="fi" />
                <sfFields:ChoiceItem Text="French" Value="fr" />
                <sfFields:ChoiceItem Text="French Informal" Value="fr-inf" />
                <sfFields:ChoiceItem Text="German" Value="de" />
                <sfFields:ChoiceItem Text="German Informal" Value="de-inf" />
                <sfFields:ChoiceItem Text="Greek" Value="el" />
                <sfFields:ChoiceItem Text="Hebrew" Value="he" />
                <sfFields:ChoiceItem Text="Hungarian" Value="hu" />
                <sfFields:ChoiceItem Text="Indonesian (Bahasa)" Value="id" />
                <sfFields:ChoiceItem Text="Italian" Value="it" />
                <sfFields:ChoiceItem Text="Japanese" Value="ja" />
                <sfFields:ChoiceItem Text="Korean" Value="ko" />
                <sfFields:ChoiceItem Text="Malay" Value="ms" />
                <sfFields:ChoiceItem Text="Norwegian" Value="no" />
                <sfFields:ChoiceItem Text="Persian (Farsi)" Value="fa" />
                <sfFields:ChoiceItem Text="Polish" Value="pl" />
                <sfFields:ChoiceItem Text="Portuguese" Value="pt" />
                <sfFields:ChoiceItem Text="Portuguese (Brazil)" Value="pt-br" />
                <sfFields:ChoiceItem Text="Romanian" Value="ro" />
                <sfFields:ChoiceItem Text="Russian" Value="ru" />
                <sfFields:ChoiceItem Text="Serbian (Cyrillic)" Value="sr" />
                <sfFields:ChoiceItem Text="Slovak" Value="sk" />
                <sfFields:ChoiceItem Text="Slovenian" Value="sl" />
                <sfFields:ChoiceItem Text="Spanish" Value="es" />
                <sfFields:ChoiceItem Text="Spanish Informal" Value="es-inf" />
                <sfFields:ChoiceItem Text="Spanish (Lat-Am)" Value="es-mx" />
                <sfFields:ChoiceItem Text="Swedish" Value="sv" />
                <sfFields:ChoiceItem Text="Tagalog" Value="tl" />
                <sfFields:ChoiceItem Text="Thai" Value="th" />
                <sfFields:ChoiceItem Text="Turkish" Value="tr" />
                <sfFields:ChoiceItem Text="Ukrainian" Value="uk" />
                <sfFields:ChoiceItem Text="Vietnamese" Value="vi" />
            </Choices>
        </sfFields:ChoiceField>
    </div>
    <div id="language-other-wrapper" style="display: none;">
        <div class="sfTxtLbl sfInlineBlock">
            Enter a new Gigya Language code<span class='required'>*</span>
        </div>
        <sfFields:TextField ID="LanguageOther" runat="server"
            DataFieldName="LanguageOther"
            DisplayMode="Write"
            MaxCharactersCount="20"
            WrapperTag="div">
        </sfFields:TextField>
    </div>
    <div id="language-fallback-wrapper">
        <sfFields:ChoiceField ID="LanguageFallback" runat="server" Title="Language Fallback (used if the Sitefinity language isn't available in Gigya)<span class='required'>*</span>" DataFieldName="LanguageFallback" DisplayMode="Write" RenderChoicesAs="DropDown">
            <Choices>
                <sfFields:ChoiceItem Text="English (default)" Value="en" />
                <sfFields:ChoiceItem Text="Arabic" Value="ar" />
                <sfFields:ChoiceItem Text="Bulgarian" Value="br" />
                <sfFields:ChoiceItem Text="Catalan" Value="ca" />
                <sfFields:ChoiceItem Text="Chinese (Mandarin)" Value="zh-cn" />
                <sfFields:ChoiceItem Text="Chinese (Hong Kong)" Value="zh-hk" />
                <sfFields:ChoiceItem Text="Chinese (Taiwan)" Value="zh-tw" />
                <sfFields:ChoiceItem Text="Croatian" Value="hr" />
                <sfFields:ChoiceItem Text="Czech" Value="cs" />
                <sfFields:ChoiceItem Text="Danish" Value="da" />
                <sfFields:ChoiceItem Text="Dutch" Value="nl" />
                <sfFields:ChoiceItem Text="Dutch Informal" Value="nl-inf" />
                <sfFields:ChoiceItem Text="Finnish" Value="fi" />
                <sfFields:ChoiceItem Text="French" Value="fr" />
                <sfFields:ChoiceItem Text="French Informal" Value="fr-inf" />
                <sfFields:ChoiceItem Text="German" Value="de" />
                <sfFields:ChoiceItem Text="German Informal" Value="de-inf" />
                <sfFields:ChoiceItem Text="Greek" Value="el" />
                <sfFields:ChoiceItem Text="Hebrew" Value="he" />
                <sfFields:ChoiceItem Text="Hungarian" Value="hu" />
                <sfFields:ChoiceItem Text="Indonesian (Bahasa)" Value="id" />
                <sfFields:ChoiceItem Text="Italian" Value="it" />
                <sfFields:ChoiceItem Text="Japanese" Value="ja" />
                <sfFields:ChoiceItem Text="Korean" Value="ko" />
                <sfFields:ChoiceItem Text="Malay" Value="ms" />
                <sfFields:ChoiceItem Text="Norwegian" Value="no" />
                <sfFields:ChoiceItem Text="Persian (Farsi)" Value="fa" />
                <sfFields:ChoiceItem Text="Polish" Value="pl" />
                <sfFields:ChoiceItem Text="Portuguese" Value="pt" />
                <sfFields:ChoiceItem Text="Portuguese (Brazil)" Value="pt-br" />
                <sfFields:ChoiceItem Text="Romanian" Value="ro" />
                <sfFields:ChoiceItem Text="Russian" Value="ru" />
                <sfFields:ChoiceItem Text="Serbian (Cyrillic)" Value="sr" />
                <sfFields:ChoiceItem Text="Slovak" Value="sk" />
                <sfFields:ChoiceItem Text="Slovenian" Value="sl" />
                <sfFields:ChoiceItem Text="Spanish" Value="es" />
                <sfFields:ChoiceItem Text="Spanish Informal" Value="es-inf" />
                <sfFields:ChoiceItem Text="Spanish (Lat-Am)" Value="es-mx" />
                <sfFields:ChoiceItem Text="Swedish" Value="sv" />
                <sfFields:ChoiceItem Text="Tagalog" Value="tl" />
                <sfFields:ChoiceItem Text="Thai" Value="th" />
                <sfFields:ChoiceItem Text="Turkish" Value="tr" />
                <sfFields:ChoiceItem Text="Ukrainian" Value="uk" />
                <sfFields:ChoiceItem Text="Vietnamese" Value="vi" />
            </Choices>
        </sfFields:ChoiceField>
    </div>

    <div class="sfInlineBlock" style="line-height: 1.6;">
        <span class="sfTxtLbl">Session Management<span class='required'>*</span></span>
        Choose whether Gigya or Sitefinity controls the login session. If Gigya controls the session, in site groups that share a single sign-on, the recommended setting is "Sliding Session", which extends/restarts the session every time a server call is made. 
        For more information, visit <a target="_blank" href="https://developers.gigya.com/display/GD/Managing+Session+Expiration">Gigya's documentation</a>.
    </div>
    <div id="session-management-wrapper" style="margin-top: 5px; margin-bottom: 5px;">
        <sfFields:ChoiceField ID="SessionManagement" Title="" runat="server" DataFieldName="SessionProvider" DisplayMode="Write" RenderChoicesAs="RadioButtons">
            <Choices>
                <sfFields:ChoiceItem Text="Gigya" Value="0" />
                <sfFields:ChoiceItem Text="Sitefinity" Value="1" />
            </Choices>
        </sfFields:ChoiceField>
    </div>
    <div id="gigya-session-mode-wrapper" style="display: none; margin-top: 5px;">
        <sfFields:ChoiceField ID="GigyaSession" Title="" runat="server" DataFieldName="GigyaSessionMode" DisplayMode="Write" RenderChoicesAs="DropDown">
            <Choices>
                <sfFields:ChoiceItem Text="Sliding session" Value="0" />
                <sfFields:ChoiceItem Text="Fixed duration" Value="1" />
                <sfFields:ChoiceItem Text="Valid forever" Value="2" />
                <sfFields:ChoiceItem Text="Until browser closes" Value="3" />
            </Choices>
        </sfFields:ChoiceField>
    </div>
    <div id="session-timeout-wrapper" style="margin-top: 10px;">
        <sfFields:TextField ID="SessionTimeout" runat="server"
            DataFieldName="SessionTimeout"
            Title="Session Duration"
            DisplayMode="Write"
            WrapperTag="div">
        </sfFields:TextField>
        <span class="label-after-field sfTxtLbl">Seconds</span>
    </div>
    <br />
    <div class="sfTxtLbl sfInlineBlock">
        Data Center<span class='required'>*</span>
    </div>
    <div id="data-center-wrapper">
        <sfFields:ChoiceField ID="DataCenterField" Title="" runat="server" DataFieldName="DataCenter" DisplayMode="Write" RenderChoicesAs="DropDown">
            <Choices>
                <sfFields:ChoiceItem Text="US" Value="us1.gigya.com" />
                <sfFields:ChoiceItem Text="EU" Value="eu1.gigya.com" />
                <sfFields:ChoiceItem Text="AU" Value="au1.gigya.com" />
                <sfFields:ChoiceItem Text="RU" Value="ru1.gigya.com" />
                <sfFields:ChoiceItem Text="CN" Value="cn1.gigya-api.cn" />
                <sfFields:ChoiceItem Text="Other" Value="" />
            </Choices>
        </sfFields:ChoiceField>
    </div>
    <div id="data-center-other-wrapper" style="display: none;">
        <div class="sfTxtLbl sfInlineBlock">
            Enter a new Data Center<span class='required'>*</span>
        </div>
        <sfFields:TextField ID="DataCenterOther" runat="server"
            DataFieldName="DataCenterOther"
            DisplayMode="Write"
            MaxCharactersCount="20"
            WrapperTag="div">
        </sfFields:TextField>
    </div>

    <sfFields:TextField ID="RedirectUrlField" runat="server"
        DataFieldName="RedirectUrl"
        Title="Redirect Url (where user is redirected to after login)"
        DisplayMode="Write"
        MaxCharactersCount="255"
        Tooltip="If blank the current page will be reloaded otherwise the user will be redirected to this url"
        WrapperTag="div">
    </sfFields:TextField>
    <sfFields:TextField ID="LogoutUrl" runat="server"
        DataFieldName="LogoutUrl"
        Title="Logout Url (where user is redirected to after logout)"
        DisplayMode="Write"
        MaxCharactersCount="255"
        Tooltip="If blank the current page will be reloaded otherwise the user will be redirected to this url"
        WrapperTag="div">
    </sfFields:TextField>
    <div id="mapping-field-wrapper" style="display: none;">
        <sfFields:TextField ID="MappingFieldsField" runat="server"
            DataFieldName="MappingFields"
            Title="Mapping Fields"
            DisplayMode="Write"
            WrapperTag="div">
        </sfFields:TextField>
    </div>
    <sfFields:TextField ID="GlobalParametersField" runat="server"
        DataFieldName="GlobalParameters"
        Title="Global Parameters (JSON)"
        DisplayMode="Write"
        Rows="10"
        Tooltip="These will be added to the global config JavaScript settings."
        WrapperTag="div">
    </sfFields:TextField>

    <div class="sfTxtLbl sfInlineBlock">
        Fields Mapping (UserId must be mapped to a unique field e.g. UID)<span class='required'>*</span>
    </div>
    <table class="mapping-fields-table">
        <thead>
            <tr>
                <th>Gigya Field</th>
                <th>Sitefinity Field</th>
                <th></th>
            </tr>
        </thead>
        <tbody>
            <tr style="display: none;">
                <td>
                    <input class="gigya-mapping-field sfTxt" type="text" />
                </td>
                <td>
                    <input class="sf-mapping-field sfTxt" type="text" />
                    <select class="sf-mapping-field"></select>
                </td>
                <td>
                    <a href="#" class="remove">Remove</a>
                </td>
            </tr>
        </tbody>
    </table>
    <a href="#" class="add-mapping-field">Add new mapping field</a>

    <div style="display: none;" id="loaded-field-wrapper">
        <sfFields:TextField runat="server"
            DataFieldName="LoadedField"
            DisplayMode="Write"
            WrapperTag="div">
        </sfFields:TextField>
    </div>
    <div style="display: none;" id="can-view-app-secret-wrapper">
        <sfFields:TextField runat="server"
            DataFieldName="CanViewApplicationSecret"
            DisplayMode="Write"
            WrapperTag="div">
        </sfFields:TextField>
    </div>

    <sfFields:ChoiceField ID="DebugModeField" Title="Debug Mode" runat="server" DataFieldName="DebugMode" CssClass="sfRadioList" DisplayMode="Write" RenderChoicesAs="SingleCheckBox">
        <Choices>
            <sfFields:ChoiceItem Text="Yes" Value="1" />
        </Choices>
    </sfFields:ChoiceField>

    <sfFields:ChoiceField ID="EnableRaasField" Title="Enable Module" runat="server" DataFieldName="EnableRaas" CssClass="sfRadioList" DisplayMode="Write" RenderChoicesAs="SingleCheckBox">
        <Choices>
            <sfFields:ChoiceItem Text="Yes" Value="1" />
        </Choices>
    </sfFields:ChoiceField>
</div>

<script>
    var gigyaSettings = {
        mappingTable: null,
        hiddenMappingField: null,
        gigyaSessionTimeoutWrapper: null,
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

            gigyaSettings.gigyaSessionTimeoutWrapper = $('#session-timeout-wrapper');
            gigyaSettings.mappingTable = $('.mapping-fields-table');
            gigyaSettings.hiddenMappingField = $('#mapping-field-wrapper input');

            $('.add-mapping-field').click(function () {
                gigyaSettings.addMappingRow('', '');
                return false;
            });

            gigyaSettings.mappingTable.on('change', 'select', function () {
                gigyaSettings.updateFieldMappings();
            });

            gigyaSettings.mappingTable.on('blur', 'input', function () {
                gigyaSettings.updateFieldMappings();
            }).on('click', '.remove', function () {
                if (gigyaSettings.mappingTable.find('tbody tr').length > 1) {
                    $(this).closest('tr').remove();
                    gigyaSettings.updateFieldMappings();
                }
                return false;
            });

            gigyaSettings.addProfileProperties();
            gigyaSettings.deserializeFieldMappings();

            gigyaSettings.addOtherWrapperChangeEvent('#data-center-wrapper select', '#data-center-other-wrapper', '', true);
            gigyaSettings.addLanguageChangeEvent('#language-wrapper select', '#language-other-wrapper', '#language-fallback-wrapper');

            $('.sfInheritanceMsg .sfLinkBtn').click(function () {
                setTimeout(function () {
                    gigyaSettings.toggleDisabledFields(!$('#data-center-wrapper select').is(':disabled'));
                }, 100);
            });
            
            var sessionManagementBr = $('#session-management-wrapper').find('br');
            
            var sessionWrapperElems = $('#session-management-wrapper input');
            sessionWrapperElems.change(function () {
                var $this = $(this);
                var show = $this.is(':checked') && $this.val() == '0';
                gigyaSettings.toggleGigyaSessionMode($this.val());

                if (show) {
                    $('#session-timeout-wrapper input').prop('checked', false);
                }
            });

            var checkedSessionElem = sessionWrapperElems.filter(':checked');
            gigyaSettings.toggleGigyaSessionMode(checkedSessionElem.val());
            
            var gigyaSessionModeWrapper = $('#gigya-session-mode-wrapper select');
            gigyaSessionModeWrapper.change(function () {
                var $this = $(this);
                var value = $this.val();
                gigyaSettings.toggleSessionTimeout(value);
            }).change();
        },
        toggleSessionTimeout: function (value) {
            var show = value == '0' || value == '1';

            if (show) {
                gigyaSettings.gigyaSessionTimeoutWrapper.fadeIn();
            } else {
                gigyaSettings.gigyaSessionTimeoutWrapper.fadeOut();
            }
        },
        toggleGigyaSessionMode: function (sessionProvider) {
            var show = sessionProvider == '0';
            var gigyaSessionModeWrapper = $('#gigya-session-mode-wrapper');
            if (show) {
                gigyaSessionModeWrapper.fadeIn();
                gigyaSettings.toggleSessionTimeout(gigyaSessionModeWrapper.find('select').val());
            } else {
                gigyaSessionModeWrapper.fadeOut();

                // session timeout always required if CMS mode
                gigyaSettings.gigyaSessionTimeoutWrapper.fadeIn();
            }
        },
        addLanguageChangeEvent: function (selectSelector, otherWrapperSelector, fallbackWrapperSelector) {
            var otherWrapper = $(otherWrapperSelector);
            var fallbackWrapper = $(fallbackWrapperSelector);

            var select = $(selectSelector).change(function () {
                var value = $(this).val();
                if (value == '') {
                    otherWrapper.fadeIn();
                    fallbackWrapper.hide();
                } else if (value == 'auto') {
                    otherWrapper.hide();
                    fallbackWrapper.fadeIn();
                } else {
                    otherWrapper.hide().find('input').val('');
                    fallbackWrapper.hide();
                }
            });

            // see if the user is using the dropdown or has overridden with an "other" value
            var otherVal = otherWrapper.find('input').val();
            var option = select.find('option').filter(function () {
                return $(this).val() === otherVal;
            });

            if (option.length != 1) {
                select.val('');
            }

            select.change();
        },
        addOtherWrapperChangeEvent: function (selectSelector, otherWrapperSelector, toggleValue, checkOptions) {
            var otherWrapper = $(otherWrapperSelector);
            var select = $(selectSelector).change(function () {
                if ($(this).val() == toggleValue) {
                    otherWrapper.fadeIn();
                } else {
                    otherWrapper.hide().find('input').val('');
                }
            });

            if (checkOptions) {
                // see if the user is using the dropdown or has overridden with an "other" value
                var otherVal = otherWrapper.find('input').val();
                var option = select.find('option').filter(function () {
                    return $(this).val() === otherVal;
                });

                if (option.length != 1) {
                    select.val('');
                }

                select.change();
            }
        },
        addMappingRow: function(gigyaValue, sitefinityValue, readOnly) {
            var firstRow = gigyaSettings.mappingTable.find('tbody tr:first');
            var newRow = firstRow.clone().show();

            var gigyaField = newRow.find('.gigya-mapping-field').val(gigyaValue);
            if (readOnly) {
                newRow.find('.sf-mapping-field').prop('disabled', true);
                newRow.find('.remove').hide();
            } else {
                newRow.find('.sf-mapping-field').prop('disabled', false);
                newRow.find('.remove').show();
            }

            newRow.find('.sf-mapping-field').val(sitefinityValue);
            gigyaSettings.mappingTable.find('tbody').append(newRow);
        },
        addProfileProperties: function () {
            var profileSelect = $('.mapping-fields-table select.sf-mapping-field');
            var profileProperties = $('#profile-properties-wrapper :text').val();
            if (profileProperties.length == 0) {
                profileSelect.remove();
            } else {
                var profilePropertiesMapped = $.parseJSON(profileProperties);
                for (var i = 0; i < profilePropertiesMapped.length; i++) {
                    profileSelect.append(
                        $('<option />')
                        .text(profilePropertiesMapped[i].Value)
                        .val(profilePropertiesMapped[i].Key)
                    );
                }
                $('.mapping-fields-table input.sf-mapping-field').remove();
            }
        },
        deserializeFieldMappings: function() {
            var mappingsRaw = gigyaSettings.hiddenMappingField.val();
            if (mappingsRaw.length == 0) {
                return;
            }

            var mappings = $.parseJSON(mappingsRaw);
            for (var i = 0; i < mappings.length; i++) {
                var mapping = mappings[i];
                gigyaSettings.addMappingRow(mapping.gigyaFieldName, mapping.cmsFieldName, mapping.required);
            }
        },
        updateFieldMappings: function () {
            var mappings = [];
            gigyaSettings.mappingTable.find('tbody tr:visible').each(function () {
                var $this = $(this);

                mappings.push({
                    gigyaFieldName: $this.find('.gigya-mapping-field').val(),
                    cmsFieldName: $this.find('.sf-mapping-field').val()
                });
            });

            gigyaSettings.hiddenMappingField.val(JSON.stringify(mappings));
        },
        toggleDisabledFields: function (enabled) {
            if (enabled) {
                $('#gigya-settings-section input, #gigya-settings-section textarea, #gigya-settings-section select').prop('disabled', false);
                $('#gigya-settings-section .add-mapping-field, #gigya-settings-section .remove, #edit-application-secret').show();
            } else {
                $('#gigya-settings-section input, #gigya-settings-section textarea, #gigya-settings-section select').prop('disabled', true);
                $('#gigya-settings-section .add-mapping-field, #gigya-settings-section .remove, #edit-application-secret').hide();
            }
        }
    };

    // poll to check if user allowed to see fields - I can't find a 'sitefinity data service complete' event
    window.loadedTimeout = setInterval(function () {
        if ($('#loaded-field-wrapper :text').val().length > 0) {
            clearInterval(window.loadedTimeout);

            gigyaSettings.init();

            if ($('#data-center-wrapper select').is(':disabled')) {
                gigyaSettings.toggleDisabledFields(false);
            }

            if ($('#can-view-app-secret-wrapper :text').val() === 'true') {
                $('.application-secret-wrapper').show();
            }

            $('#loading-view').hide();
            $('#gigya-settings-section').fadeIn(1000);
        }
    }, 500);
</script>