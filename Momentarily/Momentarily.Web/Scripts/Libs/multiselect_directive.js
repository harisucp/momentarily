
// Code goes here


angular.directive('multiSelect', function () {

    function link(scope, element) {
        var options = {
            enableClickableOptGroups: true,
            enableFiltering: true,
            buttonWidth: '100%',
            onChange: function () {
                //element.change();
            }
        };
        element.multiselect(options);
    }

    return {
        restrict: 'A',
        link: link
    };
});

angular.config(['$provide', function ($provide) {
    $provide.decorator('selectDirective', ['$delegate', function ($delegate) {
        var directive = $delegate[0];

        directive.compile = function () {

            function post(scope, element, attrs, ctrls) {
                directive.link.post.apply(this, arguments);

                var ngModelController = ctrls[1];
                if (ngModelController && attrs.multiSelect !== null && angular.isDefined(attrs.multiSelect)) {
                    originalRender = ngModelController.$render;
                    ngModelController.$render = function () {
                        originalRender();
                        element.multiselect('refresh');
                    };
                }
            }

            return {
                pre: directive.link.pre,
                post: post
            };
        };

        return $delegate;
    }]);
}]);