//var umbracoApp = angular.module('umbraco', []);

angular.module("umbraco").controller("Gigya.GigyaSettingsEditController",
	function ($scope, gigyaTreeResource) {
	    $scope.loaded = false;

	    $scope.editApplicationSecret = false;
	    $scope.nodeId = $('#gigya-node-id').val();

	    gigyaTreeResource.get($scope.nodeId).then(function (response) {
	        $scope.data = response.data.Data;

	        $scope.model = response.data.Settings;
	        $scope.loaded = true;

	        if ($scope.model.CanViewApplicationSecret && ($scope.model.ApplicationSecretMasked == null || $scope.model.ApplicationSecretMasked == '')) {
	            $scope.editApplicationSecret = true;
	        }
	    });

	    $scope.publish = function (model) {
	        gigyaTreeResource.save(model).then(function (response) {
	            if (response.data.Success) {
	                noty({ text: 'Settings have been published', timeout: 5000, type: 'success', layout: 'bottomCenter' });

	                if (response.data.Settings) {
	                    $scope.model = response.data.Settings;
	                }
	            } else {
	                noty({ text: 'Error - ' + response.data.Error, timeout: 10000, type: 'error', layout: 'bottomCenter' });
	            }
	        });
	    };

	    $scope.toggleEditApplicationSecret = function () {
	        $scope.editApplicationSecret = !$scope.editApplicationSecret;

	        if (!$scope.editApplicationSecret) {
	            $scope.model.ApplicationSecret = '';
	        }
	    };

	    $scope.addMappingField = function () {
	        var model = {
	            GigyaFieldName: '',
	            CmsFieldName: ''
	        };

	        $scope.model.MappingFields.push(model);
	    };

	    $scope.removeMappingField = function (field) {
	        $scope.model.MappingFields = $scope.model.MappingFields.filter(function (item) {
	            return item.CmsFieldName !== field.CmsFieldName;
	        });
	        return false;
	    };
	});