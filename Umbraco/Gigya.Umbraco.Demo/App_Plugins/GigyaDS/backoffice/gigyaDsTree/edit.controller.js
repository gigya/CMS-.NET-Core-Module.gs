angular.module("umbraco").controller("Gigya.DS.EditController",
	function ($scope, $routeParams, gigyaDsTreeResource, notificationsService, $timeout, navigationService) {

	    $scope.loaded = false;
	    $scope.nodeId = $routeParams.id;

	    gigyaDsTreeResource.get($routeParams.id).then(function (response) {
	        $scope.data = response.data.Data;

	        $scope.model = response.data.Settings;
	        $scope.loaded = true;
	    });

	    $scope.publish = function (model) {
	        gigyaDsTreeResource.save(model).then(function (response) {
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

	    $scope.addMappingField = function () {
	        var model = {
	            GigyaName: '',
	            CmsName: '',
                Oid: ''
	        };

	        $scope.model.Mappings.push(model);
	    };

	    $scope.removeMappingField = function (field) {
	        $scope.model.Mappings = $scope.model.Mappings.filter(function (item) {
	            return item.CmsName !== field.CmsName;
	        });
	        return false;
	    };
	});