window.onload = function () {
    var ctx = document.getElementById('delivery-vs-pickup').getContext('2d');
    var chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: ['January', 'Febuary', 'March', 'April', 'May'],
            datasets: [{
                label: 'Deliveries',
                backgroundColor: '#c51111',
                borderColor: '#a11111',
                data: [
                    1042,
                    1201,
                    821,
                    1101,
                    1201
                ],
                fill: false
            }]
        }
    });
};