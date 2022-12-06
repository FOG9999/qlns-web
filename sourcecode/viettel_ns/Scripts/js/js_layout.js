$(document).ready(function () {
    $(".nav-link").on("click", function (event) {
        var urlRedirect = $(this).attr("href");
        event.preventDefault();
        var data = $(this).data("id")
        var bIsMenuLeft = $(this).data("collapse")
        if (bIsMenuLeft == 1) {
            ClickMenuLeft(data)
        }
        else
        {
            $.ajax({
                type: "POST",
                url: "/Menu/GetMenuLink",
                data: { iScreenId: data },
                success: function (r) {
                    if (r != null) {
                        $.ajax({
                            type: "POST",
                            url: "/Menu/SetIdSessionMenuLink",
                            data: { iScreenId: r.menu_id },
                            success: function (res) {
                                window.location.href = r.url;
                            }
                        });
                    }
                }
            });
        }
    });

    $("#sidebarCollapse").on("click", function () {
        if (this.classList.contains("active")) {
            $(this).removeClass("active");
            UnActiveMenuLeft();
        }
        else {
            $(this).addClass("active");
            ActiveMenuLeft();
        }
    });

    CloseMenuLeftWhenNotHaveData();
    HideHomeIcon();
});

function HideHomeIcon() {
    var count = $(".btn-breadcrumb").children().length;
    if (count == 1) {
        $(".btn-breadcrumb a").css("display", "none")
    }
}

$("#sidebarCollapse").trigger("click");

$(".nav-link").trigger("click");

//$(
//    function () {
//        $(".date").datetimepicker({
//            format: 'dd/MM/yyyy',
//            regional: "vi"
//        });
//    }
//)

function ClickMenuLeft(id) {
    if (!$("#sidebarCollapse").hasClass("active")) {
        $(".menu-child-left").css("display", "none");
        $("#lst-menu-" + id).css("display", "");
    }
}

function CloseMenuLeftWhenNotHaveData() {
    if ($(".menu-left li").length == 0) {
        if (!$("#sidebarCollapse").hasClass("active")) {
            ActiveMenuLeft();
            $(".menu-left").css("display", "none");
            return;
        }
    }
    UnActiveMenuLeft();
}

function ActiveMenuLeft() {
    $(".menu-left .menu-child-left").css("display", "none");
    $(".menu-left").width("54px");
    //$(".detail-placehold").width($(document).width() - 62);
    

    $(".menu-left span").css("display", "none");

    $(".sidebar-header h7").css("display", "none");
    $(".sidebar-header i").css("display", "");

    $(".menu-left .navbar-nav .nav-link").addClass("active");
    $(".dropdown-menu-hide .nav-link").removeClass("active");
    $(".dropdown-menu-hide").removeClass("active");
    $(".menu-left .navbar-nav i").addClass("fa-2x");
}

function UnActiveMenuLeft() {
    $(".menu-left").css("width", "20%");
    $(".detail-placehold").css("min-width", "80%");
    $(".menu-left span").css("display", "");
    $(".sidebar-header h7").css("display", "");
    $(".sidebar-header i.fa-angle-double-right").css("display", "none");
    $(".menu-left .navbar-nav .nav-link").removeClass("active");
    $(".dropdown-menu-hide").addClass("active");
    $(".menu-left .navbar-nav i").removeClass("fa-2x");
}


