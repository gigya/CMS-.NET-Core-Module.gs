//adds the resource to umbraco.resources module:
angular.module('umbraco.resources').factory('gigyaDsTreeResource', ['$q', '$http', function ($q, $http) {
        return {
            get: function (id) {
                return $http.get("/umbraco/backoffice/GigyaDS/GigyaDsSettingsApi/Get/" + id);
            },
            save: function (data) {
                return $http.post("/umbraco/backoffice/GigyaDS/GigyaDsSettingsApi/Save", data);
            }
        };
    }]
);