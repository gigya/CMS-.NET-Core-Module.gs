//adds the resource to umbraco.resources module:
angular.module('umbraco.resources').factory('gigyaTreeResource', ['$q', '$http', function ($q, $http) {
        return {
            get: function (id) {
                return $http.get("/umbraco/backoffice/Gigya/GigyaSettingsApi/Get/" + id);
            },
            save: function (data) {
                return $http.post("/umbraco/backoffice/Gigya/GigyaSettingsApi/Save", data);
            }
        };
    }]
);