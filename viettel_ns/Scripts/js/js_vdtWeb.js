var jsNumber_KieuSoTiengViet = true;

//Hàm filter  selectlist
function FilterSelectList() {
    $("select").each(function (index, item) {
        $(item).select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });

    });

}

//Hàm loại bỏ khoảng trắng thẻ input và textare
function TrimSpaceAllTextInput() {
    $('input[type=text]').blur(function (e) {
        let currVal = e.target.value;
        $(this).val($.trim(currVal));
    });

    $('textarea').blur(function (e) {
        let currVal2 = e.target.value;
        $(this).val($.trim(currVal2));
    })
}

////Hàm kiểm tra định dạng name theo tên class
function ValidateYear() {
    $('input.yearFormat')
        .blur(function (event) {
            ValidateInputFocusOut(event, this, 6)
        })
        .keydown(function (event) {
            ValidateInputKeydown(event, this, 1)
        })
}

//Hàm kiểm tra định dạng date theo tên class
function ValidateDateTime() {
    $('input.dateFormat')
        .blur(function (event) {
            ValidateInputFocusOut(event, this, 3)
        })
        .keydown(function (event) {
            ValidateInputKeydown(event, this, 3)
        })
}

// Hàm filterInput khi thêm mới các dòng con
function ValidateInputOnTable() {
    $('.btnAdd').click(function (e) {
        setTimeout(function () {
            FilterSelectList();
        })
    })
}

// Hàm kiểm tra ngày bắt đầu < ngày kết thúc
function ValidationStartDateEndDate() {
    var txtTuNgay = $('input.startDate')[0].value;
    var txtDenNgay = $('input.endDate')[0].value;
    if (txtTuNgay == "" || txtDenNgay == "") return true;
    var dTuParts = txtTuNgay.split("/");
    var dDenParts = txtDenNgay.split("/");
    var dTuNgay = new Date(dTuParts[2], dTuParts[1] - 1, dTuParts[0]);
    var dDenNgay = new Date(dDenParts[2], dDenParts[1] - 1, dDenParts[0]);
    if (dTuNgay > dDenNgay) {
        return false;

    }
    return true;

}

// Hàm kiểm tra giai đoạn bắt đầu < giai đoạn kết thúc

function ValidationStartYearEndYear() {
    var txtStartYear = $('input.startYear')[0].value;
    var txtEndYear = $('input.endYear')[0].value;
    if (txtStartYear == "" || txtEndYear == "") return true;

    var fStarYear = ParseNumber(txtStartYear);
    var fEndYear = ParseNumber(txtEndYear);
    if (fStarYear > fEndYear) {
        return false;
    }
    return true;
}

// Gọi hàm  kiểm tra giai đoạn bắt đầu < giai đoạn kết thúc
function ValidateStartYearEndYear() {
    $('.startYear').change(function (e) {
        setTimeout(function () {
            if (!ValidationStartYearEndYear()) {
                $('input.startYear')[0].setCustomValidity(`Giai đoạn bắt đầu không được lớn hơn giai đoạn kết thúc!`);
                $('input.startYear')[0].reportValidity();
            }
        }, 1)
    })

    $('.endYear').change(function (e) {
        setTimeout(function () {
            if (!ValidationStartYearEndYear()) {
                $('input.endYear')[0].setCustomValidity(`Giai đoạn kết thúc không được bé hơn giai đoạn bắt đầu! `);
                $('input.endYear')[0].reportValidity();
            }
        }, 1)
    })
}

// Gọi hàm kiểm tra giai đoạn bắt đầu < giai đoạn kết thúc
function ValidateStartDateEndDate() {
    $('.endDate').change(function (e) {
        setTimeout(function () {
            if (!ValidationStartDateEndDate()) {
                $('input.startDate')[0].setCustomValidity(`Ngày bắt đầu không được lớn hơn ngày kết thúc!`);
                $('input.startDate')[0].reportValidity();
            }
        }, 1)
    })

    $('.startDate').change(function (e) {
        setTimeout(function () {
            if (!ValidationStartDateEndDate()) {
                $('input.endDate')[0].setCustomValidity(`Ngày kết thúc không được bé hơn ngày bắt đầu! `);
                $('input.endDate')[0].reportValidity();
            }
        }, 1)
    })
}

//Hàm kiểm tra maxlength theo đầu vào
function ValidateMaxLength(textbox, maxlength) {
    textbox.setCustomValidity('');
    textbox.setAttribute("maxlength", maxlength);
    if (textbox.value.length >= maxlength) {
        textbox.setCustomValidity(`Độ dài ký tự tối đa là ${maxlength}`);
        textbox.reportValidity();
    }
}

//Hàm chuyển từ số có định dạng về dạng không định dạng
function UnFormatNumber(value) {
    //value = value.toString();
    if (value == undefined || value == null || value == "") {
        return value;
    }
    value = value.toString();
    if (jsNumber_KieuSoTiengViet) {
        value = value.replace(/\./gi, "")
        value = value.replace(/\,/gi, ".")
    }
    else {
        value = value.replace(/\,/gi, "")
    }
    //obj.value = obj.value.replace(/,/gi, ""); //hàm ban đầu

    console.log("kiemtraso: " + value);
    return value;
}

//Chuyển 1 xâu định dạng số về kiểu số
function ParseNumber(value) {
    var vR = 0;

    if (IsNumber(value)) {
        vR = UnFormatNumber(value);
        vR = parseFloat(vR);
    }
    return vR;
}

//Hàm kiểm tra giá trị có phải là kiểu số hay không
function IsNumber(value) {
    var v = value;
    v = UnFormatNumber(v);
    return isFinite(v) && (v != "");
}

function Tooltip() {
    var facopy = document.querySelectorAll(".fa-copy.fa-lg");
    var faeye = document.querySelectorAll(".fa-eye.fa-lg");
    var fapencil = document.querySelectorAll(".fa-pencil-square-o.fa-lg");   
    var fatrashlg = document.querySelectorAll(".fa-trash-o.fa-lg");
    var fatrashfile = document.querySelectorAll(".fa-trash-o.fa-file");
    var falock = document.querySelectorAll(".fa-lock.fa-lg");
    //var fadownload = document.querySelectorAll(".fa-download.fa-lg");
    if (facopy != null) {
        $.each(facopy, function (index, item) {
            item.setAttribute("title", "Điều chỉnh");
        });
    }
    if (faeye != null) {
        $.each(faeye, function (index, item) {
            item.setAttribute("title", "Chi tiết");
        });
    }
    if (fapencil != null) {
        $.each(fapencil, function (index, item) {
            item.setAttribute("title", "Sừa");
        });
    }
    if (fatrashlg != null) {
        $.each(fatrashlg, function (index, item) {
            item.setAttribute("title", "Xóa");
        });
    }
    if (fatrashfile != null) {
        $.each(fatrashfile, function (index, item) {
            item.setAttribute("title", "Export");
        });
    }
    if (falock != null) {
        $.each(falock, function (index, item) {
            item.setAttribute("title", "Khóa");
        });
    }
    //if (fadownload != null) {
    //    $.each(fadownload, function (index, item) {
    //        item.setAttribute("title", "Gửi dữ liệu");
    //    });
    //}
}

function CallFunctionCommon() {
    FilterSelectList();
    TrimSpaceAllTextInput();
    ValidateYear();
    ValidateDateTime();
    ValidateStartDateEndDate();
    ValidateInputOnTable();
    ValidateStartYearEndYear();
    Tooltip();
}

CallFunctionCommon();
