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
        Title="API Key"
        DisplayMode="Write"
        MaxCharactersCount="255"
        WrapperTag="div">
    </sfFields:TextField>
    <sfFields:TextField ID="ApplicationKeyField" runat="server"
        DataFieldName="ApplicationKey"
        Title="Application Key"
        DisplayMode="Write"
        MaxCharactersCount="255"
        WrapperTag="div">
    </sfFields:TextField>
    <div class="application-secret-wrapper" style="display: none;">
        <div class="application-secret-masked">
            <div class="sfTxtLbl sfInlineBlock">
                Application Secret
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
                Title="Application Secret"
                DisplayMode="Write"
                MaxCharactersCount="255"
                WrapperTag="div">
            </sfFields:TextField>
            <a href="#" id="cancel-application-secret">Cancel</a>
        </div>
    </div>

    <div class="sfTxtLbl sfInlineBlock">
        Language
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
            Enter a new Gigya Language code
        </div>
        <sfFields:TextField ID="LanguageOther" runat="server"
            DataFieldName="LanguageOther"
            DisplayMode="Write"
            MaxCharactersCount="20"
            WrapperTag="div">
        </sfFields:TextField>
    </div>
    <div id="language-fallback-wrapper">
        <sfFields:ChoiceField ID="LanguageFallback" runat="server" Title="Language Fallback (used if the Sitefinity language isn't available in Gigya)" DataFieldName="LanguageFallback" DisplayMode="Write" RenderChoicesAs="DropDown">
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

    <div class="sfTxtLbl sfInlineBlock">
        Data Center
    </div>
    <div id="data-center-wrapper">
        <sfFields:ChoiceField ID="DataCenterField" Title="" runat="server" DataFieldName="DataCenter" DisplayMode="Write" RenderChoicesAs="DropDown">
            <Choices>
                <sfFields:ChoiceItem Text="US" Value="us1" />
                <sfFields:ChoiceItem Text="EU" Value="eu1" />
                <sfFields:ChoiceItem Text="AU" Value="au1" />
                <sfFields:ChoiceItem Text="Other" Value="" />
            </Choices>
        </sfFields:ChoiceField>
    </div>
    <div id="data-center-other-wrapper" style="display: none;">
        <div class="sfTxtLbl sfInlineBlock">
            Enter a new Data Center
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
    <sfFields:TextField ID="SessionTimeout" runat="server"
        DataFieldName="SessionTimeout"
        Title="Session Expiration (seconds)"
        DisplayMode="Write"
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
        Fields Mapping
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

            gigyaSettings.mappingTable = $('.mapping-fields-table');
            gigyaSettings.hiddenMappingField = $('#mapping-field-wrapper input');

            $('.add-mapping-field').click(function () {
                gigyaSettings.addMappingRow('', '');
                return false;
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

            gigyaSettings.deserializeFieldMappings();

            gigyaSettings.addOtherWrapperChangeEvent('#data-center-wrapper select', '#data-center-other-wrapper', '');
            gigyaSettings.addLanguageChangeEvent('#language-wrapper select', '#language-other-wrapper', '#language-fallback-wrapper');

            $('.sfInheritanceMsg .sfLinkBtn').click(function () {
                setTimeout(function () {
                    gigyaSettings.toggleDisabledFields(!$('#data-center-wrapper select').is(':disabled'));
                }, 100);
            });
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
                newRow.find('input.sf-mapping-field').prop('disabled', true);
                newRow.find('.remove').hide();
            } else {
                newRow.find('input.sf-mapping-field').prop('disabled', false);
                newRow.find('.remove').show();
            }

            newRow.find('.sf-mapping-field').val(sitefinityValue);
            gigyaSettings.mappingTable.find('tbody').append(newRow);
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