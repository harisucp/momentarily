
var temp = localStorage.getItem("temp");
var path = window.location.pathname;
var pathname = "";
var pathnlist = [];
pathnlist = path.split('/');
pathname = '/' + pathnlist[1] + '/' + pathnlist[2];

if (pathname.lastIndexOf('Index', pathname.length - 1) === pathname.length - 1 || pathname.lastIndexOf('undefined', pathname.length - 1) === pathname.length - 1) {
    jQuery("#pcoded").find(".pcoded-hasmenu").removeClass("pcoded-trigger");
    localStorage.setItem("CurrentPage", '');
}
else if (pathname.lastIndexOf('AdminDashboard/', pathname.length - 1) === pathname.length - 1) {
    jQuery("#pcoded").find(".pcoded-hasmenu").removeClass("pcoded-trigger");
    localStorage.setItem("CurrentPage", '');
}
else {
    jQuery(".pcoded").find("." + temp).addClass("pcoded-trigger");
}

var selector = '.pcoded-submenu li';
var url = window.location.href;
var target = url.split('/');

var urltarget = ('/' + target[target.length - 2] + '/' + target[target.length - 1]);
if (urltarget === "/AdminDashboard/Index" || urltarget === "/AdminDashboard/") {
    localStorage.setItem("CurrentPage", '');
} 

var flag = false;
$(selector).each(function () {
    var currentPage = localStorage.getItem("CurrentPage");
    var selectedLink = $(this).find('a').attr('href');
    var urlTarget = ('/' + target[target.length - 2] + '/' + target[target.length - 1]);
    if (selectedLink === currentPage) {
        flag = true;
        $(this).addClass('active');
        //$(this).removeClass('active').addClass('active');
    }
    else {
        $(this).removeClass('active');
       
    }
});

if (flag == false) {
    jQuery("#pcoded").find(".pcoded-hasmenu").removeClass("pcoded-trigger");
}

$('.pcoded-submenu li').click(function () {
    
    $('.pcoded-submenu li').removeClass("active");
    $(this).addClass("active");
    localStorage.setItem("CurrentPage", $(this).find('a').attr('href'));
});

$(".modules").on("click", function () {
    localStorage.setItem("temp", "moduleshasmenu");
});
$(".reports").on("click", function () {
    localStorage.setItem("temp", "reportshasmenu");
});

$(".parentModulesInnerIndex").on("click", function () {
    localStorage.setItem("temp", "moduleshasmenu");
    localStorage.setItem("CurrentPage", $(this).attr('href'));
});








