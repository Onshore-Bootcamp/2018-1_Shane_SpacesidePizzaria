
window.onload = function () {
    $(function () {
        $("table .select-wrapper li").click(function () {
            console.log("clicked");

            let $selectWrapper = $(this).parent().parent();
            let status = this.innerText;
            let orderID = $(this).parent().siblings("select").attr("id");

            $.ajax({
                type: "POST",
                url: '/Order/ChangeStatus?orderID=' + orderID + '&status=' + status,
                success: function (data) {
                    if (status == 'Completed') {
                        $selectWrapper.parent('td').append(status);
                        $selectWrapper.remove();
                    }
                    alert(data.message);
                },
                error: function () {
                    alert("Error");
                }
            });
        });
    });
}
