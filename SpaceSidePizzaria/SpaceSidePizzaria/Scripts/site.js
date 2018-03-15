function confirmUpdate(message) {
    message = message ? message : "This is a prefab pizza, updating this pizza will change it's price.";
    return confirm(message);
}

$(function () {

    $("input[type='hidden']").each(function (index, element) {
        $(this).appendTo($(element).parent());
    });

    $('select').material_select();

    $('.dropdown-button').dropdown();

    $('.button-collapse').sideNav();
})