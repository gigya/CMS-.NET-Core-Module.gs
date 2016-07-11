angular.module("umbraco").controller("Gigya.GigyaSettingsEditController",
	function ($scope, $routeParams, gigyaTreeResource, notificationsService, $timeout, navigationService) {

	    $scope.loaded = false;

	    $scope.editApplicationSecret = false;
	    $scope.nodeId = $routeParams.id;

	    gigyaTreeResource.get($routeParams.id).then(function (response) {
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
	                notificationsService.success("Success", "Settings have been published");

	                if (response.data.Settings) {
	                    $scope.model = response.data.Settings;
	                }
	            } else {
	                notificationsService.error("Error", response.data.Error);
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