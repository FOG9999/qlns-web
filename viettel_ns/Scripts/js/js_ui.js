$(".dropdown-menu").mouseleave(function () {
    $(".dropdown").removeClass("open");
});


$(document).ready(function () {

    // refresh current page
    $('.btn-reload').click(function () {
        console.log("reload page");
        location.reload();
    });


    // close modal window if the default behavior do not work.
    $(".btn-close-modal").click(function () {
        //$("#editor").modal('hide');
        $(this).closest('.modal').modal('hide');
    })

})
