
document.addEventListener('DOMContentLoaded', function () {
    let form = $("#checkout-form");

    form.submit(function (event) {
        event.preventDefault();

        var $this = $(this),
            $modal = $('#update-modal'),
            $modalContent = $modal.find('.modal-content');

        // If the payment checkbox is checked.
        if ($('#PaymentPO_ForDelivery').prop('checked')) {
            // Send an ajax request to the sever.
            $.ajax({
                type: 'GET',
                url: '/Account/CheckUserDeliverability/',
                success: function (data) {
                    if (data.requiresUpdate) {
                        if (data.employee) {
                            $modal.empty().html('<p class="red-text">' + data.message + '</p>');
                        } else {
                            $.get("/Account/Update/", function (data) {
                                $modalContent.empty().append(data);
                                $('.modal').modal();
                                $modal.modal('open');
                                setTimeout(function () {
                                    Materialize.updateTextFields();
                                }, 100);
                            });
                        }
                    } else {
                        $this.off('submit').submit();
                    }
                },
                error: function (message) {
                    alert('A problem occurred while checking your account info.');
                }
            });
        } else {
            $this.off('submit').submit();
        }
    });

}, false);