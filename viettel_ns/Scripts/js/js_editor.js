(function ($) {
    $.fn.serializeFormJSON = function () {

        var o = {};
        var a = this.serializeArray();
        $.each(a, function () {
            if (o[this.name]) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    };
})(jQuery);


// longsam custom
function setFocus(focusElement) {

    var type = $(focusElement).attr('type');
    //console.log($(focusElement));

    if ($(focusElement).is("input")) {
        //if (type === "text" || type == "input") {
        var value = $(focusElement).val();
        $(focusElement).val("");
        $(focusElement).focus();
        $(focusElement).val(value);

        //console.log("focus:" + value);
    } else {
        $(focusElement).focus();
    }
}


$(document).ready(function () {

    $(document).ready(function () {
        //$(".focus-me").first().focus();
        setFocus(".focus-me:first");

        $.validator.methods.date = function (value, element) {
            //return this.optional(element) || moment(value, "DD/MM/YYYY", true).isValid();
            return this.optional(element);
        }

        $('.date')
          .datepicker({
              todayBtn: "linked",
              keyboardNavigation: false,
              forceParse: false,
              autoclose: true,
              language: 'vi',
              todayHighlight: true,
          });

        $(".date").attr("placeholder", "dd/mm/yyyy");


    });
})
