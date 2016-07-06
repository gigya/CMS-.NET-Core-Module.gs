//adds the resource to umbraco.resources module:
angular.module('umbraco.resources').factory('gigyaSettingsResource', ['$q', '$http', function ($q, $http) {
        return {
            get: function (id) {
                return $http.post("/umbraco/backoffice/Gigya/GigyaSettingsApi/Get");
            }
        };
    }]
);