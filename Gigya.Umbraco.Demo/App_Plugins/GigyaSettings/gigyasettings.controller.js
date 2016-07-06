angular.module("umbraco")
    .controller("Gigya.GigyaSettingsController", ['$scope', 'gigyaSettingsResource', function ($scope, gigyaSettingsResource) {
        console.log('scope', $scope);

        gigyaSettingsResource.get(0).then(function (response) {
            //$scope.
            alert('completed');
        });

        var model = {
            applicationKey: 1234
        }

        $scope.model = model;
    }]);