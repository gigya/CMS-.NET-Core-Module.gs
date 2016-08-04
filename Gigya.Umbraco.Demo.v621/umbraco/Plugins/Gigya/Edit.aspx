<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/umbraco/masterpages/umbracoPage.Master" %>
<%@ Register Namespace="umbraco.uicontrols" Assembly="controls" TagPrefix="umb" %>

<asp:Content ID="Content" ContentPlaceHolderID="body" runat="server">
    <script src="//ajax.googleapis.com/ajax/libs/angularjs/1.5.6/angular.min.js"></script>
    <script src="/scripts/jquery.noty.packaged.min.js"></script>
    <script src="/umbraco/plugins/Gigya/gigyaTree.resource.js"></script>
    <script src="/umbraco/plugins/Gigya/edit.controller.js"></script>
    <input type="hidden" value="<%= Request["id"] %>" id="gigya-node-id" />
    
    <style>
        .ng-hide { display: none !important; }
        .mapping-fields-table th { width: auto; }
    </style>

    <div id="gigya-umbraco-container" ng-app="umbraco" style="display: none;">
        <div ng-controller="Gigya.GigyaSettingsEditController">
        <umb:UmbracoPanel ID="Panel1" runat="server" hasMenu="true" Text="Edit Settings">
        
            <umb:Pane ID="Pane1" runat="server">
            
                <div ng-show="model.Id > 0">
                    <umb:PropertyPanel ID="PPanel3" runat="server" Text="Inherited from Global Settings">
                        <input type="checkbox" id="Inherited" name="Inherited" ng-model="model.Inherited" />
                    </umb:PropertyPanel>
                </div>

                <umb:PropertyPanel ID="PPanel1" runat="server" Text="API Key">
                    <input type="text" id="api-key" name="ApiKey" ng-disabled="model.Inherited" ng-model="model.ApiKey" class="input-large umb-textstring textstring" ng-required="true" required="required" />
                </umb:PropertyPanel>
            
                <umb:PropertyPanel ID="PPanel2" runat="server" Text="Application Key">                
                    <input type="text" id="application-key" name="ApplicationKey" ng-disabled="model.Inherited" ng-model="model.ApplicationKey" class="input-large umb-textstring textstring" ng-required="true" val-server="value" required="required" />
                </umb:PropertyPanel>
            
                <div ng-show="model.CanViewApplicationSecret">
                    <umb:PropertyPanel ID="PPanel4" runat="server" Text="Application Secret">
                        <span ng-show="model.Inherited || !editApplicationSecret" ng-bind="model.ApplicationSecretMasked"></span>
                        <input ng-show="!model.Inherited && editApplicationSecret" type="text" id="application-secret" name="ApplicationSecret" ng-model="model.ApplicationSecret" class="input-large umb-textstring textstring" ng-required="true" val-server="value" required="required" />
                        <button ng-show="!model.Inherited && !editApplicationSecret" type="button" class="btn btn-small" ng-click="toggleEditApplicationSecret()">Edit</button>
                        <button ng-show="!model.Inherited && editApplicationSecret" type="button" class="btn btn-small" ng-click="toggleEditApplicationSecret()">Cancel</button>
                    </umb:PropertyPanel>
                </div>

                <umb:PropertyPanel ID="PropertyPanel1" runat="server" Text="Language">
                    <select name="language" id="language" class="umb-editor umb-dropdown" ng-disabled="model.Inherited" ng-model="model.Language" ng-options="option.Name for option in data.LanguageOptions track by option.Code"></select>
                </umb:PropertyPanel>

                <div ng-show="model.Language.Code == 'Other'">
                    <umb:PropertyPanel ID="PropertyPanel2" runat="server" Text="Language Other">
                        <input type="text" id="language-other" name="LanguageOther" ng-disabled="model.Inherited" ng-model="model.LanguageOther" class="input-large umb-textstring textstring" val-server="value" />
                    </umb:PropertyPanel>
                </div>

                <div ng-show="model.Language.Code == 'auto'">
                    <umb:PropertyPanel ID="PropertyPanel3" runat="server" Text="Language Fallback">
                        <select name="language-fallback" id="language-fallback" ng-disabled="model.Inherited" class="umb-editor umb-dropdown" ng-options="option.Name for option in data.Languages track by option.Code" ng-model="model.LanguageFallback"></select>
                    </umb:PropertyPanel>
                </div>

                <umb:PropertyPanel ID="PropertyPanel4" runat="server" Text="Data Center">
                    <select id="data-center" name="data-center" ng-disabled="model.Inherited" ng-model="model.DataCenter" class="umb-editor umb-dropdown" required>
                        <option value="us1">US</option>
                        <option value="eu1">EU</option>
                        <option value="au1">AU</option>
                        <option value="ru1">RU</option>
                        <option value="Other">Other</option>
                    </select>
                </umb:PropertyPanel>

                <div ng-show="model.DataCenter == 'Other'">
                    <umb:PropertyPanel ID="PropertyPanel5" runat="server" Text="Data Center Other">
                        <input type="text" id="data-center-other" name="DataCenterOther" ng-disabled="model.Inherited" ng-model="model.DataCenterOther" class="input-large umb-textstring textstring" val-server="value" />
                    </umb:PropertyPanel>
                </div>

                <umb:PropertyPanel ID="PropertyPanel6" runat="server" Text="Redirect Url">
                    <input type="text" id="redirect-url" name="RedirectUrl" ng-disabled="model.Inherited" ng-model="model.RedirectUrl" class="input-large umb-textstring textstring" val-server="value" />
                </umb:PropertyPanel>

                <umb:PropertyPanel ID="PropertyPanel7" runat="server" Text="Logout Url">
                    <input type="text" id="logout-url" name="LogoutUrl" ng-disabled="model.Inherited" ng-model="model.LogoutUrl" class="input-large umb-textstring textstring" val-server="value" />
                </umb:PropertyPanel>

                <umb:PropertyPanel ID="PropertyPanel8" runat="server" Text="Session Timeout">
                    <input type="text" id="session-timeout" name="SessionTimeout" ng-disabled="model.Inherited" ng-model="model.SessionTimeout" class="input-large umb-textstring textstring" val-server="value" required="required" />
                </umb:PropertyPanel>

                <umb:PropertyPanel ID="PropertyPanel9" runat="server" Text="Fields Mapping">
                    <table class="mapping-fields-table">
                        <thead>
                            <tr>
                                <th>Gigya Field</th>
                                <th>Umbraco Field Alias</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr ng-repeat="row in model.MappingFields">
                                <td>
                                    <input class="gigya-mapping-field" ng-disabled="model.Inherited" type="text" ng-model="row.GigyaFieldName" />
                                </td>
                                <td>
                                    <input class="sf-mapping-field" ng-disabled="model.Inherited || row.Required" type="text" ng-model="row.CmsFieldName" />
                                </td>
                                <td>
                                    <button type="button" class="btn btn-small" ng-hide="model.Inherited || row.Required" ng-click="removeMappingField(row)">Remove</button>
                                </td>
                            </tr>
                        </tbody>
                    </table>

                    <button type="button" class="btn btn-small" ng-hide="model.Inherited" ng-click="addMappingField()">Add</button>
                    <br />
                    <br />
                </umb:PropertyPanel>

                <umb:PropertyPanel ID="PropertyPanel10" runat="server" Text="Global Parameters (JSON)">
                    <textarea id="global-params" name="GlobalParams" ng-disabled="model.Inherited" ng-model="model.GlobalParameters" class="input-large umb-textstring textstring" val-server="value" rows="5"></textarea>
                </umb:PropertyPanel>

                <umb:PropertyPanel ID="PropertyPanel11" runat="server" Text="Debug Mode">
                    <input type="checkbox" id="debug-mode" name="DebugMode" ng-disabled="model.Inherited" ng-model="model.DebugMode" val-server="value" />
                </umb:PropertyPanel>

                <umb:PropertyPanel ID="PropertyPanel12" runat="server" Text="Enable Module">
                    <input type="checkbox" id="enable-raas" name="EnableRaas" ng-disabled="model.Inherited" ng-model="model.EnableRaas" val-server="value" />
                </umb:PropertyPanel>
                
                <umb:PropertyPanel ID="PropertyPanel13" runat="server" Text="&nbsp;">
                    <div class="btn-group">
                        <button type="button" ng-click="publish(model)" data-hotkey="ctrl+s" class="btn btn-success">
                            Publish
                        </button>
                    </div>
                </umb:PropertyPanel>
            </umb:Pane>
    
        </umb:UmbracoPanel>
        </div>
    </div>

    <script>
        $('#gigya-umbraco-container').fadeIn('slow');
    </script>

</asp:Content>