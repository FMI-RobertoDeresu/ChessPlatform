"use strict";

angular.module("gameApp").controller('updateProfilePhotoModalController', function ($scope, $uibModalInstance, userService) {
    $scope.ok = function () {
        var formData = new FormData();

        formData.append('file', $scope.profilePhoto);

        userService.updateProfilePhoto(formData).then(function (response) {
            if (response.error == false) {
                $uibModalInstance.close();
            }
            else {
                $scope.errorMessage = response.errorMessage;
            }
        });
    }

    $scope.cancel = function () {
        $uibModalInstance.dismiss();
    }
}).directive('fileModel', ['$parse', function ($parse) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var model = $parse(attrs.fileModel);
            var modelSetter = model.assign;

            element.bind('change', function () {
                scope.$apply(function () {
                    modelSetter(scope, element[0].files[0]);
                });
            });
        }
    };
}]);;