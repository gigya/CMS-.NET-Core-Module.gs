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

	    $scope.uidFieldMappingExists = function (checkEmpty) {
	        for (var i = 0; i < $scope.model.MappingFields.length; i++) {
	            if ($scope.model.MappingFields[i].GigyaFieldName == 'UID' && (!checkEmpty || $scope.model.MappingFields[i].CmsFieldName != '')) {
	                return true;
	            }
	        }

	        return false;
	    };

	    $scope.publish = function (model) {
	        $scope.model.submitted = true;

	        if (!$scope.model.Inherited) {
	            // not inherited so we need to validate that the UID field has been mapped
	            if (!$scope.uidFieldMappingExists(true)) {
	                notificationsService.error("Error", 'UID is required field mapping on Gigya field list.');

	                if (!$scope.uidFieldMappingExists(false)) {
	                    var model = {
                            GigyaRequired: true,
	                        GigyaFieldName: 'UID',
	                        CmsFieldName: ''
	                    };

	                    $scope.model.MappingFields.push(model);
	                }
	                return;
	            }
	        }

	        gigyaTreeResource.save(model).then(function (response) {
	            if (response.data.Success) {
	                $scope.model.submitted = false;
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