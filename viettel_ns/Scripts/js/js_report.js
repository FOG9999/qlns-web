
/*
Mở báo cáo nhiều tab, mở lần lượt từ cuối lên đầu
để các báo cáo in theo tuần tự 1 -> n  
*/
function openTabs(i, count, siteArray, delay) {

    if (i < 0)
        return;

    if (delay == null || delay == undefined) {
        delay = 1000;
    }

    console.log(delay);
    setTimeout(function () {
        var url = siteArray[i];

        window.open(url, '_blank');
        console.log("index: " + i + "  url: " + url);

        i--;

        openTabs(i, count, siteArray, delay);
    },
    delay);
}

function openLinks(links, delay) {
    var count = links.length;
    openTabs(count - 1, count, links, delay);
}

function openNewTab(url) {
    window.open(url, '_blank');
    console.log("index: " + i + "  url: " + url);
}

function openTab(url) {
    window.open(url, '_blank');
    console.log("index: " + i + "  url: " + url);
}

/*

Lấy chuối giá trị được chọn trong danh sách checkboxs

*/
function getCheckedItems(id) {
    var items = [];

    $("input:checkbox[check-group='" + id + "']").each(function () {
        if (this.checked) {
            items.push(this.value);
        }
    });

    var result = items.join(",");
    return result;
}

/*
Chọn hoặc bỏ chọn tất cả trong danh sách checkboxs
*/
function checkListAll(value, group) {
    $("input:checkbox[check-group='" + group + "']").each(function (i) {
        this.checked = value;
    });

    checkItem(group, null, null);


    // bắn sự kiện
    changeListAll(group);
}

function changeListAll(group) {
    checkItem(group, null, null);
}

function checkFirstItem(group) {
    var index = 0;
    $("input:checkbox[check-group='" + group + "']").each(function () {
        if (index == 0) {
            this.checked = true;

            checkItem($(this).data("group"), $(this).data("value"));
        }

        index++;
    });
}

function checkItem(group, value) {

    $("#" + group).val(getCheckedItems(group));
}

function checkOnlyFirstItem(group) {
    var numberOfChecked = $("input:checkbox[check-group='" + group + "']").length;
    if (numberOfChecked == 1) {
        var index = 0;
        $("input:checkbox[check-group='" + group + "']").each(function () {
            if (index == 0) {
                this.checked = true;
                checkItem($(this).data("group"), $(this).data("value"), this);
            }

            index++;
        });
    }
}

function fillCheckboxList(id, group, url) {
    jQuery.ajaxSetup({ cache: false });
    console.log(url);

    var _id = "#" + id;
    $(_id).empty;

    $.getJSON(url, function (data) {
        $(_id).html(data);

        var check = $(_id).data("check");
        console.log("data-check:" + check);

        if (check == "first") {
            checkFirstItem(group);
        }
        else if (check == "first-only") {
            checkOnlyFirstItem(group);
        }
        else if (check == "all") {
            checkListAll(true, group);
        }
    });


    function fillCheckboxListPartial(id, group, url) {
        jQuery.ajaxSetup({ cache: false });
        console.log(url);

        var _id = "#" + id;
        $(_id).empty();

        $.ajax({
            type: "GET",
            url: url,
            success: function (data) {

                $(_id).html(data);

                var check = $(_id).data("check");
                if (check == "first") {
                    checkFirstItem(group);
                }
                else if (check == "first-only") {
                    checkOnlyFirstItem(group);
                }
                else if (check == "all") {
                    checkListAll(true, group);
                }
            },
        });

    }

}

function getStringValue(id) {
    var text = $(id).val()
                .replace(/\n/g, "%5Cn")
                //.replace(/\n/g, "%5Cr")
                .replace(/&/g, "%26");

    return text;
}
