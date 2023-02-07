
if( typeof angularConstants=='undefined'){
	console.log('no constants');
	var angularConstants={};
};

for (var prop in angularConstants) {
    angular.module('MomentarilyApp').constant(prop, angularConstants[prop]);
};
