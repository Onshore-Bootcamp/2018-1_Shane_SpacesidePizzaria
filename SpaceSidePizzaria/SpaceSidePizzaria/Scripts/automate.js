window.onload = function () {
    console.log(window.location.pathname);

    setTimeout(function () {
        if (window.location.pathname === '/Order/MyOrders') {
            var pizzaNums = [8, 9, 10078, 10079, 10080];

            window.location = '/Cart/AddPizzaToCart/' + pizzaNums[Math.floor(Math.random() * pizzaNums.length)];
        } else if (window.location.pathname === '/Pizza') {
            window.location = '/Cart'
        } else if (window.location.pathname === '/Cart') {
            let checked = Math.random() > 0.50;
            let $form = $('#checkout-form');
            $('#PaymentPO_ForDelivery').prop('checked', checked);
            $('#PaymentPO_PayWithCash').prop('checked', true);
            $form.submit();
        }
    }, 1500);
};