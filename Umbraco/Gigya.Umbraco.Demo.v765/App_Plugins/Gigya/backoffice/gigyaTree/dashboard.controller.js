angular.module("umbraco").controller("Gigya.GigyaSettingsDashboardController",
	function ($scope, $routeParams, navigationService) {
        // reload tree in case user has added a new homepage
	    navigationService.syncTree({ tree: 'gigyaTree', path: [-2, -2], forceReload: true, activate: true });
	});