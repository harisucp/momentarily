using System.Web.Optimization;
namespace Momentarily.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                "~/Scripts/Libs/angular.min.js",
                //"~/Scripts/Libs/angular.js",
                "~/Scripts/Libs/apeek.js",
                "~/Scripts/Libs/angular-touch.js",
                "~/Scripts/Libs/angular-carousel.min.js",
                "~/Scripts/Libs/angular-smooth-scroll.js",
                "~/Scripts/Libs/moment.min.js",
                "~/Scripts/Libs/angular-moment.js",
                "~/Scripts/Libs/moment-timezone.min.js",
                "~/Scripts/Libs/angular-sanitize.js",
                "~/Scripts/Libs/angular-startsRating.js",
                "~/Scripts/app.js",
                "~/Scripts/directives.js",
                "~/Scripts/factories.js",
                "~/Scripts/filters.js",
                //"~/Scripts/Libs/multiselect.js",
                  "~/Scripts/Libs/angularjs-dropdown-multiselect.min.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/scripts/angular-constants").Include(
                "~/Scripts/Libs/angular-constants.js"));
            bundles.Add(new ScriptBundle("~/bundles/datepickerMultiSelect").Include(
                "~/Scripts/Libs/gm.datepickerMultiSelect.js"));
            bundles.Add(new ScriptBundle("~/bundles/vimeoPlayer").Include(
                "~/Scripts/Libs/player.min.js"
                ));
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                "~/Content/css/vendor/bootstrap.css",
                "~/Content/css/vendor/font-awesome.css",
                "~/Content/css/vendor/datepicker.css",
                //"~/Content/css/angular-datepicker.css",
                "~/Content/css/index.css",
                "~/Content/css/vendor/ng-rateit.css",
                "~/Content/css/header1.css"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/Libs/ui-bootstrap-angular-min.js"));
            bundles.Add(new StyleBundle("~/bundles/cssHome").Include(
                "~/Content/css/home.css"));
            bundles.Add(new StyleBundle("~/bundles/cssSearch").Include(
                "~/Content/css/search_page.css"));
            bundles.Add(new StyleBundle("~/bundles/cssAccount").Include(
                "~/Content/css/account.css"));
            bundles.Add(new StyleBundle("~/bundles/cssHowItWorks").Include(
                "~/Content/css/how-it-works.css"));
            bundles.Add(new StyleBundle("~/bundles/register").Include(
                "~/Content/css/register.css"));
            bundles.Add(new StyleBundle("~/bundles/welcomeuser").Include(
               "~/Content/css/welcomeuser.css"));
            bundles.Add(new StyleBundle("~/bundles/carousel").Include(
                "~/Content/css/vendor/angular-carousel.css"));
            bundles.Add(new ScriptBundle("~/bundles/scripts/Home").Include(
                "~/Scripts/Libs/ApeekMap/LoadGoogleMapAPI.js",
                "~/Scripts/Libs/ApeekMap/autocompleteLocationDirective.js",
               "~/Scripts/Home/controllers.js"));
            bundles.Add(new ScriptBundle("~/Scripts/Listing").Include(
                "~/Scripts/Libs/ApeekMap/LoadGoogleMapAPI.js",
                "~/Scripts/Libs/ApeekMap/autocompleteLocationDirective.js",
                "~/Scripts/Libs/ApeekMap/apeekMapDirective.js",
                "~/Scripts/Listing/services.js",
                "~/Scripts/Listing/controllers.js",
                "~/Scripts/Listing/ListingEditController.js"));
            bundles.Add(new ScriptBundle("~/Scripts/User").Include(
                "~/Scripts/User/services.js",
                "~/Scripts/User/controllers.js"));
            bundles.Add(new ScriptBundle("~/Scripts/Messages").Include(
                "~/Scripts/Messages/services.js").Include(
                "~/Scripts/Messages/controllers.js"));

            bundles.Add(new ScriptBundle("~/Scripts/AdminMessages").Include(
               "~/Scripts/AdminMessages/services.js").Include(
               "~/Scripts/AdminMessages/controllers.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Search").Include(
                "~/Scripts/Libs/ApeekMap/LoadGoogleMapAPI.js",
                "~/Scripts/Libs/ApeekMap/GoogleMapAPIService.js",
                "~/Scripts/Libs/ApeekMap/autocompleteLocationDirective.js",
                "~/Scripts/Libs/ApeekMap/apeekMapDirective.js",
                "~/Scripts/User/services.js",
                "~/Scripts/Search/controllers.js",
                "~/Scripts/Search/services.js"));
            bundles.Add(new ScriptBundle("~/Scripts/Booking").Include(
                "~/Scripts/Libs/ApeekMap/LoadGoogleMapAPI.js",
                "~/Scripts/Libs/ApeekMap/apeekMapDirective.js",
                "~/Scripts/Booking/services.js").Include(
                "~/Scripts/Booking/controllers.js"));
            bundles.Add(new ScriptBundle("~/Scripts/Account").Include(
                "~/Scripts/Account/controllers.js"));
            bundles.Add(new ScriptBundle("~/bundles/scripts/Payment").Include(
               "~/Scripts/Payment/controllers.js"));
            bundles.Add(new ScriptBundle("~/bundles/Shared")
                .Include("~/Scripts/Shared/controllers.js")
               .Include("~/Scripts/Shared/services.js"));
            bundles.Add(new ScriptBundle("~/bundles/UI-select").Include(
              "~/Scripts/Libs/select.min.js"));
            bundles.Add(new StyleBundle("~/Content/css/UI-select").Include(
                "~/Content/css/vendor/select2.css"));
            bundles.Add(new StyleBundle("~/Content/css/request_to_book").Include(
                "~/Content/css/request_to_book.css"));
            //21 october 2019 changes below
            bundles.Add(new ScriptBundle("~/Scripts/Libs/jquery").Include(
          "~/Scripts/Libs/jquery.js"));
            bundles.Add(new ScriptBundle("~/Scripts/Account/jquery-date-dropdowns-min").Include(
             "~/Scripts/Account/jquery.date-dropdowns.min.js"));
            //bundles.Add(new ScriptBundle("~/Scripts/Account/jquery-date-dropdowns").Include(
            //    "~/Scripts/Account/jquery.date-dropdowns.js"));
            bundles.Add(new ScriptBundle("~/Scripts/Account/Select2").Include(
            "~/Scripts/Account/Select2.js"));
            bundles.Add(new ScriptBundle("~/Scripts/Libs/Bootsrap3.4.1").Include(
            "~/Scripts/Libs/Bootstrap340.js"));
            bundles.Add(new ScriptBundle("~/Scripts/Libs/Jquery1102").Include(
            "~/Scripts/Libs/Jquery1102.js"));
            bundles.Add(new ScriptBundle("~/Scripts/Libs/jquery1114").Include(
            "~/Scripts/Libs/jquery1114.js"));
            bundles.Add(new ScriptBundle("~/Scripts/Libs/GoogleJquery211").Include(
            "~/Scripts/Libs/GoogleJquery211.js"));
            bundles.Add(new ScriptBundle("~/Scripts/Libs/jqueryInputMask").Include(
            "~/Scripts/Libs/jqueryinputmask.min.js"));
            bundles.Add(new ScriptBundle("~/Scripts/Libs/Jquery341").Include(
          "~/Scripts/Libs/Jquery341.js"));
            bundles.Add(new ScriptBundle("~/Scripts/Libs/Bootstrap340").Include(
          "~/Scripts/Libs/Bootstrap340.js"));
            bundles.Add(new StyleBundle("~/Content/css/Login").Include(
              "~/Content/css/Login.css"));
            bundles.Add(new StyleBundle("~/Content/css/registermessagesent").Include(
              "~/Content/css/registermessagesent.css"));
            bundles.Add(new StyleBundle("~/Content/css/registermessagesents").Include(
              "~/Content/css/registermessagesent.scss"));
            bundles.Add(new StyleBundle("~/Content/css/messages").Include(
             "~/Content/css/messages.css"));
            bundles.Add(new StyleBundle("~/Content/css/jqueryUI").Include(
             "~/Content/css/jqueryUI.css"));

            //BundleTable.EnableOptimizations = true;

            //Admin Dashboard Bundle Files
            bundles.Add(new StyleBundle("~/bundles/adminCss").Include(
           "~/AdminContent/Admity/css/bootstrap.min.css",
           "~/AdminContent/Admity/css/feather.css",
           "~/AdminContent/Admity/css/style.css",
           "~/plugins/fontawesome-free/css/all.min.css"
           ));

            bundles.Add(new ScriptBundle("~/Scripts/adminJs").Include(
           "~/AdminContent/Admity/js/popper.min.js",
           "~/AdminContent/Admity/js/bootstrap.min.js",
           "~/AdminContent/Admity/js/Chart.js",
           "~/AdminContent/Admity/js/pcoded.min.js",
           "~/AdminContent/Admity/js/vartical-layout.min.js",
           "~/AdminContent/Admity/js/custom-dashboard.js",
           "~/AdminContent/Admity/js/script.min.js",
           "~/plugins/datatables/jquery.dataTables.js",
           "~/plugins/datatables-bs4/js/dataTables.bootstrap4.js",
           "~/AdminContent/Admity/js/AdminSideBar.js"
           ));
        }
    }
}