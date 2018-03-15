$(function () {

    var $nav = $('#mobile-menu');

    driverNavChecker();

    $(window).resize(driverNavChecker);

    function driverNavChecker() {
        if (window.innerWidth < 992) {
            if ($('#sidenav-overlay').css('opacity') != '1') {
                $nav.css('transform', 'translateX(-100%)');
            } else { }
        } else {
            $nav.css('transform', 'translateX(0)');
        }
    }

});