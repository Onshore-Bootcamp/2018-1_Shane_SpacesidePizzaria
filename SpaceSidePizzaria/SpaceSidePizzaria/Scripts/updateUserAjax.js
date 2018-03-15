$("#update-ajax").submit(function (event) {
    event.preventDefault();

    var $this = $(this),
        $modal = $('#update-modal'),
        $modalContent = $($modal.find('.modal-content'));

    var formData = $this.serialize();

    $modalContent.empty().html('<div class="progress"><div class="indeterminate"></div></div>')

    // TODO: Change this delay if in production
    setTimeout(function () {
        $.post('/Account/Update/', formData)
            .done(function (data) {
                if ($modal.css('display') === 'none') {
                    $modal.modal('open');
                } else { /* The modal is already open */ }

                if (!data.updated) {
                    // A new form has been requested from the server.
                    $modalContent.empty().append(data);
                    Materialize.updateTextFields();
                }
                else {
                    $modalContent.empty().html('<h4 class="light">Successfully updated your account.</h4>');
                }
            })
            .fail(function () {
                alert("A problem occurred while updating your account please try again later.");
            });
    }, 2500);
});