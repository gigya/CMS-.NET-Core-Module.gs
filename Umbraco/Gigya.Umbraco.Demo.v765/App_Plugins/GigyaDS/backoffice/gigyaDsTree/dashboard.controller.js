angular.module("umbraco").controller("Gigya.DS.DashboardController",
	function ($scope, $routeParams, navigationService) {
        // reload tree in case user has added a new homepage
	    navigationService.syncTree({ tree: 'gigyaDsTree', path: [-2, -2], forceReload: true, activate: true });
	});