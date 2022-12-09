var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var CONFIRM = 0;
var lstDonVi = [];
var lstTiGiaChiTiet = [];
var tiGia = 0;
var isVNDtoUSD = false;
var USE_LAST_NEW_ADJUST = false;

// State: CREATE => tạo mới, UPDATE => Chỉnh sửa, ADJUST => Điều chỉnh
var CURRENT_STATE = '';

$(document).ready(function () {

    // Binding event on change select TTCP
    $("#iID_KHTongTheTTCPID").on('change', function () {
        FullActionToSelect();
    });

    // Event change Loại Kế Hoach form thêm mới, chỉnh sửa, điều chỉnh.
    $("#iLoai").on('change', function (e) {
        FullActionToSelect();
    });

    $("#iID_ParentID").select2({
        width: '100%',
        matcher: FilterInComboBox
    });

    $("#iID_KHTongTheTTCPID").select2({
        width: '100%',
        matcher: FilterInComboBox
    });

    $("#iID_BQuanLyID").select2({
        width: '100%',
        matcher: FilterInComboBox
    });

    $("select[name='iID_DonViID']").select2({
        width: '100%',
        matcher: FilterInComboBox
    });

    $("select[name='iID_TiGiaID']").select2({
        width: '100%',
        matcher: FilterInComboBox
    });

    $('#modalKHChiTietBQP').on('hidden.bs.modal', function () {
        USE_LAST_NEW_ADJUST = false;
    });
});

// #region =================================== Action view list ===================================

// Mở modal thêm mới, sửa, điều chỉnh
function OpenModal(id, state) {
    $("#modalKHChiTietBQPLabel").html('');

    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/KeHoachChiTietBQP/GetModal",
        async: false,
        data: { id: id, state: state },
        success: function (data) {
            $("#contentmodalKHChiTietBQP").empty().html(data);
            if (state == 'CREATE') {
                $("#modalKHChiTietBQPLabel").empty().html('Thêm mới quyết định tổng thể Thủ tướng Chính phủ');
            } else if (state == 'UPDATE') {
                $("#modalKHChiTietBQPLabel").empty().html('Sửa quyết định tổng thể Thủ tướng Chính phủ');
            } else {
                $("#modalKHChiTietBQPLabel").empty().html('Điều chỉnh quyết định tổng thể Thủ tướng Chính phủ');
            }
        }
    });

    CURRENT_STATE = state;
}

// Láy chi tiết tỉ giá khi thay đổi tỉ giá trên form thêm mới, sửa, điều chỉnh
function ChangeTiGiaSelect() {
    let tiGiaId = $("#iID_TiGiaID").val();
    $.ajax({
        type: "POST",
        url: "/QLNH/KeHoachChiTietBQP/ChangeTiGia",
        data: { tiGiaId: tiGiaId },
        success: function (data) {
            $("#tienTeQuyDoiID").html(data);
        }
    });
}

// Lưu thông tin trên form thêm mới, sửa, điều chỉnh
function Save() {
    let state = CURRENT_STATE;
    let elTTCP = $("#iID_KHTongTheTTCPID");
    let elPr = $("#iID_ParentID");

    // Get data
    let data = {};
    data.iLoai = $("#iLoai").val();
    data.iGiaiDoanTu = $.trim($("#txtGiaiDoanTu").val());
    data.iGiaiDoanDen = $.trim($("#txtGiaiDoanDen").val());
    data.iNamKeHoach = $.trim($("#txtNamKeHoach").val());
    data.iID_ParentID = elPr.val();
    data.iID_KHTongTheTTCPID = elTTCP.val();
    data.iID_TiGiaID = $("#iID_TiGiaID").val();
    data.sSoKeHoach = $("<div/>").text($.trim($("#txtSoKeHoach").val())).html();
    data.dNgayKeHoach = $("#txtNgayKeHoach").val();
    data.sMoTaChiTiet = $("<div/>").text($.trim($("#txtMoTaChiTiet").val())).html();
    data.bIsXoa = false;

    // Data check validate
    data.fromYearTTCP = elTTCP.find(':selected').data('fromdate');
    data.toYearTTCP = elTTCP.find(':selected').data('todate');
    data.iNamKeHoachTTCP = elTTCP.find(':selected').data('namkehoach');
    data.iLoaiTTCP = elTTCP.find(':selected').data('loaikehoach');
    data.fromYearBQP = elPr.find(':selected').data('fromdate');
    data.toYearBQP = elPr.find(':selected').data('todate');

    // Check state
    if (CURRENT_STATE == 'UPDATE') {
        // Update thì truyền thêm ID để lấy data phía BE và update.
        data.id = $("#hIdKeHoachChiTietBQP").val();

        // Validate
        if (!ValidateData(data, 'Lỗi sửa kế hoạch chi tiết Bộ Quốc phòng phê duyệt!')) {
            return;
        }
    } else if (CURRENT_STATE == 'CREATE') {
        // Thêm mới thì thêm các trường thông tin liên quan đến điều chỉnh
        data.bIsGoc = true;
        data.bIsActive = true;
        data.iLanDieuChinh = 0;

        // Validate
        if (!ValidateData(data, 'Lỗi thêm mới kế hoạch chi tiết Bộ Quốc phòng phê duyệt!')) {
            return;
        }
    } else {
        // Điều chỉnh thì gán lại old ID cho data để lấy thông tin nhiệm vụ chi lên view.
        data.bIsGoc = false;
        data.bIsActive = true;
        data.iLanDieuChinh = parseInt($("#hiLanDieuChinh").val()) + 1;
        data.iID_ParentAdjustID = $("#hIdKeHoachChiTietBQP").val();
        data.ID = $("#hIdKeHoachChiTietBQP").val();
        // Validate
        if (!ValidateData(data, 'Lỗi điều chỉnh kế hoạch chi tiết Bộ Quốc phòng phê duyệt!')) {
            return;
        }
    }

    OpenModalDetail(data, CURRENT_STATE);
}

// Validate data trên form thêm mới, sửa, điều chỉnh
function ValidateData(data, text) {
    var Title = text;
    var Messages = [];

    // Check theo loại giai đoạn
    //if (data.iLoai == 1) {
    //    if ($.trim(data.iGiaiDoanTu) == '') {
    //        Messages.push("Chưa có thông tin về giai đoạn từ, vui lòng nhập giai đoạn từ.");
    //    }
    //    if ($.trim(data.iGiaiDoanDen) == '') {
    //        Messages.push("Chưa có thông tin về giai đoạn đến, vui lòng nhập giai đoạn đến.");
    //    }
    //    if (data.iID_KHTongTheTTCPID == GUID_EMPTY || $.trim(data.iID_KHTongTheTTCPID) == '') {
    //        Messages.push("Chưa có thông tin về quyết định tổng thể TTCP phê duyệt, vui lòng chọn quyết định tổng thể TTCP phê duyệt.");
    //    }
    //} else if (data.iLoai == 2) {
    //    if ($.trim(data.iNamKeHoach) == '') {
    //        Messages.push("Chưa có thông tin về năm quyết định, vui lòng nhập năm quyết định.");
    //    }
    //    if ((data.iID_ParentID == GUID_EMPTY || $.trim(data.iID_ParentID) == '') && (data.iLoaiTTCP != 2)) {
    //        Messages.push("Chưa có thông tin về kế hoạch chi tiết BQP cha, vui lòng chọn kế hoạch chi tiết BQP cha.");
    //    }
    //    if (data.iID_KHTongTheTTCPID == GUID_EMPTY || $.trim(data.iID_KHTongTheTTCPID) == '') {
    //        Messages.push("Chưa có thông tin về quyết định tổng thể TTCP phê duyệt, vui lòng chọn quyết định tổng thể TTCP phê duyệt.");
    //    }
    //    if (data.iLoaiTTCP != 2) {
    //        if ((data.iID_ParentID != GUID_EMPTY && $.trim(data.iID_ParentID) != '') && data.iNamKeHoach) {
    //            if (data.iNamKeHoach - data.fromYearBQP < 0 || data.iNamKeHoach - data.toYearBQP > 0) {
    //                Messages.push("Năm kế hoạch không nằm trong giai đoạn của kế hoạch chi tiết BQP cha.");
    //            }
    //        }
    //    }
    //} else {
    //    if (data.iID_ParentID == GUID_EMPTY || $.trim(data.iID_ParentID) == '') {
    //        Messages.push("Chưa có thông tin về kế hoạch chi tiết BQP cha, vui lòng chọn kế hoạch chi tiết BQP cha.");
    //    }
    //    if (data.iID_KHTongTheTTCPID == GUID_EMPTY || $.trim(data.iID_KHTongTheTTCPID) == '') {
    //        Messages.push("Chưa có thông tin về quyết định tổng thể TTCP phê duyệt, vui lòng chọn quyết định tổng thể TTCP phê duyệt.");
    //    }
    //    if ($.trim(data.iGiaiDoanTu) == '') {
    //        Messages.push("Chưa có thông tin về giai đoạn từ, vui lòng nhập giai đoạn từ.");
    //    }
    //    if ($.trim(data.iGiaiDoanDen) == '') {
    //        Messages.push("Chưa có thông tin về giai đoạn đến, vui lòng nhập giai đoạn đến.");
    //    }
    //    if (data.iID_ParentID != GUID_EMPTY && $.trim(data.iID_ParentID) != '' && data.iGiaiDoanTu && data.iGiaiDoanDen) {
    //        if (data.iGiaiDoanTu - data.fromYearBQP < 0 || data.iGiaiDoanTu - data.toYearBQP > 0) {
    //            Messages.push("Giai đoạn từ không nằm trong giai đoạn của kế hoạch chi tiết BQP cha.");
    //        }
    //        if (data.iGiaiDoanDen - data.fromYearBQP < 0 || data.iGiaiDoanDen - data.toYearBQP > 0) {
    //            Messages.push("Giai đoạn đến không nằm trong giai đoạn của kế hoạch chi tiết BQP cha.");
    //        }
    //        if (data.iGiaiDoanTu - data.iGiaiDoanDen > 0) {
    //            Messages.push("Giai đoạn từ không được lớn hơn giai đoạn đến.");
    //        }
    //    }
    //}

    // Check tỉ giá
    //if (data.iID_TiGiaID == null || data.iID_TiGiaID == GUID_EMPTY) {
    //    Messages.push("Chưa có thông tin về tỉ giá, vui lòng chọn tỉ giá.");
    //}

    // Check giai doan
    if ($.trim(data.iGiaiDoanTu) == '') {
        Messages.push("Chưa có thông tin về giai đoạn từ, vui lòng nhập giai đoạn từ.");
    }
    if ($.trim(data.iGiaiDoanDen) == '') {
        Messages.push("Chưa có thông tin về giai đoạn đến, vui lòng nhập giai đoạn đến.");
    }
    if (data.iGiaiDoanTu - data.iGiaiDoanDen > 0) {
        Messages.push("Giai đoạn từ không được lớn hơn giai đoạn đến.");
    }
    // Check số kế hoạch
    if ($.trim($("#txtSoKeHoach").val()) == '') {
        Messages.push("Chưa có thông tin về số quyết định, vui lòng nhập số quyết định.");
    }
    if ($.trim($("#txtSoKeHoach").val()) != "" && $.trim($("#txtSoKeHoach").val()).length > 100) {
        Messages.push("Số quyết định nhập quá 100 kí tự, vui lòng nhập lại số quyết định.");
    }

    // Check ngày kế hoạch
    if ($.trim($("#txtNgayKeHoach").val()) != "" && !dateIsValid($.trim($("#txtNgayKeHoach").val()))) {
        Messages.push("Ngày quyết định không hợp lệ, vui lòng nhập lại ngày kế hoạch.");
    }

    if (Messages.length > 0) {
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: Messages, Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").html(data);
            }
        });
        return false;
    }

    return true;
}

// Làm mới danh sách
function ResetChangePage() {
    $("#txtFromDateFitler").val('');
    $("#txtToDateFitler").val('');
    $("#txtSoKeHoachFilter").val('');
    $("#txtNgayBanHanhFilter").val('');

    GetListData(1);
}

// Tìm kiếm
function GetListData(currentPage = 1) {
    _paging.CurrentPage = currentPage;
    var filter = {
        giaiDoanTu:     $.trim($("#txtFromDateFitler").val()),
        giaiDoanDen:    $.trim($("#txtToDateFitler").val()),
        soKeHoach:      $("<div/>").text($.trim($("#txtSoKeHoachFilter").val())).html(),
        ngayBanHanh:    $.trim($("#txtNgayBanHanhFilter").val())
    }

    $.ajax({
        type: "POST",
        url: "/QLNH/KeHoachChiTietBQP/TimKiem",
        data: { input: filter, paging: _paging },
        success: function (data) {
            // View result
            $("#lstDataView").html(data);

            // Gán lại data cho filter
            $("#txtFromDateFitler").val(filter.giaiDoanTu);
            $("#txtToDateFitler").val(filter.giaiDoanDen);
            $("#txtSoKeHoachFilter").val($("<div/>").html(filter.soKeHoach).text());
            $("#txtNgayBanHanhFilter").val(filter.ngayBanHanh);
        }
    });
}

// Phân trang event
function ChangePage(e) {
    GetListData(e);
} 

// Confirm xóa
function ConfirmDelete(id, sNam, sSoKeHoach) {
    var Title = 'Xác nhận xóa kế hoạch chi tiết Bộ Quốc phòng phê duyệt';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa kế hoạch chi tiết: ' + $("<div/>").text(sSoKeHoach).html() + ' - ' + sNam + '?');
    var FunctionName = "Delete('" + id + "')";
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: Title, Messages: Messages, Category: CONFIRM, FunctionName: FunctionName },
        success: function (data) {
            $("#divModalConfirm").empty().html(data);
        }
    });
}

// Xóa
function Delete(id) {
    $.ajax({
        type: "POST",
        url: "/QLNH/KeHoachChiTietBQP/DeleteKeHoachChiTietBQP",
        data: { id: id },
        success: function (data) {
            if (data) {
                ChangePage();
            } else {
                var Title = 'Lỗi xóa kế hoạch chi tiết Bộ Quốc phòng phê duyệt';
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: ['Không xóa được dữ liệu!'], Category: ERROR },
                    success: function (res) {
                        $("#divModalConfirm").html(res);
                    }
                });
            }
        }
    });
}

// Tìm ra thằng cha active và gán lại vào select.
function UpdateParentTTCP(iID_TTCP) {
    $.ajax({
        type: "POST",
        url: "/QLNH/KeHoachTongTheTTCP/FindParentTTCPActive",
        async: false,
        data: { id: iID_TTCP },
        success: function (data) {
            $("#iID_KHTongTheTTCPID").val(data.result.ID).change();
            USE_LAST_NEW_ADJUST = true;
        }
    });
}

// View to detail
function OpenModalDetail(data, state, isFilter = false) {

    if (state == 'DETAIL') {
        USE_LAST_NEW_ADJUST = false;
        let temp = data;
        data = {
            ID: temp,
            iID_KHTongTheTTCPID: null
        };

        if (isFilter) {
            data.iID_BQuanLyID = $("#iID_BQuanLyID").val();
            data.iID_DonViID = $("#iID_DonViID").val();
        }
    }

    // Get view detail
    $.ajax({
        type: "POST",
        url: "/QLNH/KeHoachChiTietBQP/ViewDetailKHChiTietBQP",
        data: { input: data, state: state, isUseLastTTCP: USE_LAST_NEW_ADJUST },
        async: false,
        success: function (result) {
            $("#modalKHChiTietBQP").modal('hide');
            // View result
            $("#lstDataView").html(result);
            if (isFilter) {
                $("#iID_BQuanLyID").val(data.iID_BQuanLyID);
                $("#iID_DonViID").val(data.iID_DonViID);
            }
        }
    });

    CURRENT_STATE = state;

    // Tính tỉ giá nếu là update, create, adjust
    //if (CURRENT_STATE != 'DETAIL') {

    //    // Lấy thông tin tỉ giá
    //    $.ajax({
    //        type: "POST",
    //        url: "/QLNH/KeHoachChiTietBQP/GetDataLookupDetail",
    //        data: { iID_TiGiaID: data.iID_TiGiaID },
    //        async: false,
    //        success: function (result) {
    //            lstDonVi = result.ListDonVi;
    //            lstTiGiaChiTiet = result.ListTiGiaChiTiet;
    //        }
    //    });

    //    // Check thông tin tỉ giá, điều kiện action
    //    let fromUSD = lstTiGiaChiTiet.find(x => $.trim(x.sMaTienTeGoc).toUpperCase() == 'USD');
    //    let fromVND = lstTiGiaChiTiet.find(x => $.trim(x.sMaTienTeGoc).toUpperCase() == 'VND');

    //    // Nếu mã tiền tệ gốc là USD thì enable input USD, disable VND và ngược lại.
    //    if (fromUSD) {
    //        let toVND = lstTiGiaChiTiet.find(x => $.trim(x.sMaTienTeQuyDoi).toUpperCase() == 'VND');
    //        $(".inpuFromUSD").prop('disabled', false);
    //        $(".inpuFromVND").prop('disabled', true);
    //        isVNDtoUSD = false;

    //        // Nếu có mã tiền tệ quy đổi là VND thì tính tỉ giá, không có thì mặc định là 0
    //        if (toVND) {
    //            tiGia = toVND.fTiGia;
    //        } else {
    //            tiGia = 0;
    //        }
    //    } else if (fromVND) {
    //        let toUSD = lstTiGiaChiTiet.find(x => $.trim(x.sMaTienTeQuyDoi).toUpperCase() == 'USD');
    //        $(".inpuFromUSD").prop('disabled', true);
    //        $(".inpuFromVND").prop('disabled', false);
    //        isVNDtoUSD = true;
    //        if (toUSD) {
    //            tiGia = toUSD.fTiGia;
    //        } else {
    //            tiGia = 0;
    //        }
    //    } else {
    //        // Nếu mã tiền tệ khác USD hoặc VND thì kiểm tra xem mã tiền tệ quy đổi có báo gồm VND và USD không? Nếu có thì tính tỉ giá.
    //        $(".inpuFromUSD").prop('disabled', false);
    //        $(".inpuFromVND").prop('disabled', true);
    //        isVNDtoUSD = false;
    //        let toUSD = lstTiGiaChiTiet.find(x => $.trim(x.sMaTienTeQuyDoi).toUpperCase() == 'USD');
    //        if (toUSD) {
    //            let toVND = lstTiGiaChiTiet.find(x => $.trim(x.sMaTienTeQuyDoi).toUpperCase() == 'VND');
    //            if (toVND) {
    //                tiGia = toVND.fTiGia / toUSD.fTiGia;
    //            } else {
    //                tiGia = 0;
    //            }
    //        } else {
    //            tiGia = 0;
    //        }
    //    }
    //}

    //// Khi đã có tỉ giá và lấy được view thì tính lại giá trị theo tỉ giá.
    //if (CURRENT_STATE == 'UPDATE' || CURRENT_STATE == 'ADJUST') {
    //    let oldTiGia = $("#hiID_TiGiaID").val();
    //    if (oldTiGia != data.iID_TiGiaID) {
    //        if (isVNDtoUSD) {
    //            CalcListTiGia('VND', $("input.inputFromVND:not(:disabled)"));
    //        } else {
    //            CalcListTiGia('VND', $("input.inputFromUSD:not(:disabled)"));
    //        }
    //    }
    //}
}

// Set data lookup BQP cha theo TTCP id.
function CreateLookupBQPByTTCPId(id, selected) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/KeHoachChiTietBQP/GetLookupBQPByTTCPId",
        async: false,
        data: { id: id },
        success: function (data) {
            let lstOpt = JSON.parse(data);
            $("#iID_ParentID").find('option').remove();
            lstOpt.forEach(x => {
                $("#iID_ParentID").append(`<option value="${x.Id}" ${selected == x.Id ? "selected" : ""} data-loaikehoach="${x.iLoai}" data-fromdate="${x.iGiaiDoanTu}" data-todate="${x.iGiaiDoanDen}" data-namkehoach="${x.iNamKeHoach}">
                            ${x.DisplayName}
                        </option>`);
            });
        }
    });
}

// Ẩn hiện, gán giá trị theo điều kiện.
function FullActionToSelect() {
    // Get element
    let elTTCP = $("#iID_KHTongTheTTCPID");     // Select TTCP.
    let elLoaiKH = $("#iLoai");                 // Select Loại kế hoạch.
    let gdt = $("#formGiaiDoanTu");             // Div-form giai đoạn từ.
    let gdd = $("#formGiaiDoanDen");            // Div-form giai đoạn đến.
    let nkh = $("#formNamKeHoach");             // Div-form năm kế hoạch.
    let pr = $("#formParrentID");               // Div-form kế hoạch BQP cha.
    let elGdt = $("#txtGiaiDoanTu");            // Input giai đoạn từ.
    let elGdd = $("#txtGiaiDoanDen");           // Input giai đoạn đến.
    let elNkh = $("#txtNamKeHoach");            // Input năm kế hoạch.
    //let elPr = $("#iID_ParentID");              // Input kế hoạch BQP cha.

    // Get value
    let fromYearTTCP = elTTCP.find(':selected').data('fromdate');
    let toYearTTCP = elTTCP.find(':selected').data('todate');
    let iNamKeHoachTTCP = elTTCP.find(':selected').data('namkehoach');
    let iLoaiKHTTCP = elTTCP.find(':selected').data('loaikehoach');

    if (iLoaiKHTTCP == 2) {
        $("#iLoai option[value='1']").attr("disabled", true);
        $("#iLoai option[value='3']").attr("disabled", true);
        elLoaiKH.val(2);
    } else {
        $("#iLoai option[value='1']").attr("disabled", false);
        $("#iLoai option[value='3']").attr("disabled", false);
    }

    let iLoaiInstance = elLoaiKH.val();
    let iID_KHTTCP = elTTCP.val();

    //let fromYearBQP = elPr.find(':selected').data('fromdate');
    //let toYearBQP = elPr.find(':selected').data('todate');
    //let iNamKeHoachBQP = elPr.find(':selected').data('namkehoach');
    //let iLoaiKHBQP = elPr.find(':selected').data('loaikehoach');

    elGdt.val('');
    elGdd.val('');
    elNkh.val('');

    if (iLoaiInstance == 1) {
        gdt.removeClass('hidden');
        gdd.removeClass('hidden');
        nkh.addClass('hidden');
        pr.addClass('invisible');

        if (iLoaiKHTTCP == 1 && fromYearTTCP && toYearTTCP) {
            elGdt.val(fromYearTTCP);
            elGdd.val(toYearTTCP);

            elGdt.attr('readonly', true);
            elGdd.attr('readonly', true);
        }
    } else if (iLoaiInstance == 2) {
        // nếu loại == 2 và khtt cũng có loại == 2 thì không show pr.
        if (iLoaiKHTTCP == 2) {
            gdt.addClass('hidden');
            gdd.addClass('hidden');
            nkh.removeClass('hidden');
            pr.addClass('invisible');

            elNkh.val(iNamKeHoachTTCP);
            elNkh.attr('readonly', true);
        } else {
            gdt.addClass('hidden');
            gdd.addClass('hidden');
            nkh.removeClass('hidden');
            pr.removeClass('invisible');

            elNkh.val('');
            elNkh.attr('readonly', false);
            CreateLookupBQPByTTCPId(iID_KHTTCP);
        }
    } else if (iLoaiInstance == 3) {
        gdt.removeClass('hidden');
        gdd.removeClass('hidden');
        nkh.addClass('hidden');
        pr.removeClass('invisible');

        elGdt.attr('readonly', false);
        elGdd.attr('readonly', false);
        CreateLookupBQPByTTCPId(iID_KHTTCP);
    } else {
        gdt.removeClass('hidden');
        gdd.removeClass('hidden');
        nkh.addClass('hidden');
        pr.addClass('invisible');

        elGdt.attr('readonly', false);
        elGdd.attr('readonly', false);
    }
}

// #endregion ======================================================================

// #region =================================== Action view detail ===================================

// Chuyển về màn danh sách kế hoạch BQP
function ViewToList() {
    var filter = {
        giaiDoanTu: null,
        giaiDoanDen: null,
        soKeHoach: null,
        ngayBanHanh: null
    }

    $.ajax({
        type: "POST",
        url: "/QLNH/KeHoachChiTietBQP/TimKiem",
        data: { input: filter, paging: null },
        success: function (data) {
            // View result
            $("#lstDataView").html(data);
        }
    });
}

// Lưu chi tiết nhiệm vụ chi
function SaveDetail(isNotCondition = false) {
    let state = $("#currentState").val();
    let keHoachChiTietBQP = $("<div/>").text($("#keHoachChiTietBQP").val()).html();

    var tableNhiemVuChi = [];
    $("#tbodyNhiemVuChi tr").each(function (e) {
        let rowElement = $(this);

        let rowData = {};
        rowData.ID = $.trim(rowElement.find('td[name="ID_NhiemVuChi"]').text());
        rowData.sMaThuTu = $.trim(rowElement.find('td[name="sMaThuTu"]').text());
        rowData.sTenNhiemVuChi = $("<div/>").text($.trim(rowElement.find('#sTenNhiemVuChi').val())).html();
        rowData.iID_BQuanLyID = rowElement.find('#iID_BQuanLyID').val();
        rowData.bIsTTCP = ($.trim(rowElement.find('td[name="bIsTTCP"]').text()).toLowerCase() === 'true');
        //if (!rowData.bIsTTCP) {
        //    rowData.iID_MaDonVi = $("<div/>").text(rowElement.find('select[name="iID_DonViID"]').find(':selected').data('madonvi')).html();
        //    rowData.iID_DonViID = rowElement.find('select[name="iID_DonViID"]').val();
        //    rowData.hasDonVi = true;
        //} else {
        //    let selectDonVi = rowElement.find('select[name="iID_DonViID"]');
        //    if (selectDonVi.length > 0) {
        //        rowData.iID_MaDonVi = $("<div/>").text(selectDonVi.find(':selected').data('madonvi')).html();
        //        rowData.iID_DonViID = selectDonVi.val();
        //        rowData.hasDonVi = true;
        //    } else {
        //        rowData.iID_MaDonVi = null;
        //        rowData.iID_DonViID = null;
        //        rowData.hasDonVi = false;
        //    }
        //}
        rowData.fGiaTriUSD = UnFormatNumber(rowElement.find('#fGiaTriBQP_USD').val());
        rowData.fGiaTriVND = UnFormatNumber(rowElement.find('#fGiaTriBQP_VND').val());
        rowData.iID_KHTTTTCP_NhiemVuChiID = rowElement.find('td[name="iID_KHTTTTCP_NhiemVuChiID"]').text();
        rowData.iID_ParentID = rowElement.find('td[name="iID_ParentID"]').text();
        rowData.fGiaTriTTCP_USD = UnFormatNumber(rowElement.find('td[name="fGiaTriTTCP_USD"]').text()) == '' ? 0 : UnFormatNumber(rowElement.find('td[name="fGiaTriTTCP_USD"]').text());
        tableNhiemVuChi.push(rowData);
    });

    if (state == "CREATE") {
        if (!ValidateNhiemVuChi(tableNhiemVuChi, "Lỗi thêm mới thông tin chương trình kế hoạch chi tiết BQP")) {
            return;
        }
    } else if (state == "UPDATE") {
        if (!ValidateNhiemVuChi(tableNhiemVuChi, "Lỗi cập nhật thông tin chương trình kế hoạch chi tiết BQP")) {
            return;
        }
    } else {
        if (!ValidateNhiemVuChi(tableNhiemVuChi, "Lỗi điều chỉnh thông tin chương trình kế hoạch chi tiết BQP")) {
            return;
        }
    }

    IsActionSaveNhiemVuChi(tableNhiemVuChi, keHoachChiTietBQP, state, isNotCondition);
}

// Thêm dòng nhiệm vụ chi
function AddRowKeHoachChiTiet(e) {
    let parentRow = $(e).parents('tr');
    let stt = parentRow.find('td[name="sMaThuTu"]').text();
    let tenPB = parentRow.find('td[name="sTenPhongBan"]').text();
    let idPB = parentRow.find('#iID_BQuanLyID').val();
    let iID_TTTCP_NhiemVuChiID = parentRow.find('td[name="iID_KHTTTTCP_NhiemVuChiID"]').text();
    let ParentIsTTCP = ($.trim(parentRow.find('td[name="bIsTTCP"]').text()).toLowerCase() === 'true');
    let selectDonViInParrentRow = parentRow.find('select[name="iID_DonViID"]');

    // Disable input + button delete của dòng cha.
    parentRow.find('#fGiaTriBQP_VND').prop('disabled', true);
    parentRow.find('#fGiaTriBQP_USD').prop('disabled', true);
    parentRow.find('button.btn-delete').addClass('disabled');

    // Nếu thêm dòng con mà dòng cha là TTCP và có DROPDOWN đơn vị thì xóa DROPDOWN đơn vị đi.
    if (ParentIsTTCP && selectDonViInParrentRow.length > 0) {
        // Xóa select2 và select element
        selectDonViInParrentRow.select2('destroy'); 
        selectDonViInParrentRow.remove();
    }

    // Lấy số thứ tự mới và index dòng mới thêm vào.
    let { sttNew, parentRowIndex } = RefreshThuTuAndGetThuTuMoi(stt, parentRow.index());

    let row = `
        <tr>
            <td name="ID_NhiemVuChi" class="hidden"></td>
            <td align="left" name="sMaThuTu">` + sttNew + `</td>
            <td align="left"><input type="text" id="sTenNhiemVuChi" class="form-control" value="" autocomplete="off" /></td>
            <td align="left" name="sTenPhongBan">` + $("<div/>").text(tenPB).html() + `</td>
            <td class="hidden"><input id="iID_BQuanLyID" class="form-control" value="` + idPB + `" maxlength="255" autocomplete="off"></td>
            <td align="left">` + CreateLookupDonVi() + `</td>
            <td align="right" name="fGiaTriTTCP_USD"></td>
            <td align="right" name="fGiaTriBQP_USD">
                <input type="text" id="fGiaTriBQP_USD" class="form-control inputFromUSD" value="" maxlength="255" autocomplete="off"
                    onblur="IsActionCalcTiGia(event, this, 2, 2, 'USD')" ` + (isVNDtoUSD ? 'disabled' : '') + `
                    onkeydown="ValidateInputKeydown(event, this, 2)"/>
            </td>
            <td align="right" name="fGiaTriBQP_VND">
                <input type="text" id="fGiaTriBQP_VND" class="form-control inputFromVND" value="" maxlength="255" autocomplete="off"
                    onblur="IsActionCalcTiGia(event, this, 2, 0, 'VND')" ` + (isVNDtoUSD ? '' : 'disabled') + `
                    onkeydown="ValidateInputKeydown(event, this, 1)" />
            </td>
            <td class="hidden" name="bIsTTCP">False</td>
            <td class="hidden" name="iID_KHTTTTCP_NhiemVuChiID">` + iID_TTTCP_NhiemVuChiID + `</td>
            <td class="hidden" name="iID_ParentID"></td>
            <td align="center">
                <button type="button" class="btn-detail" onclick="AddRowKeHoachChiTiet(this)"><i class="fa fa-plus" aria-hidden="true"></i></button>
                <button type="button" class="btn-delete" onclick="RemoveRowKeHoachChiTiet(this)"><i class="fa fa-trash-o fa-lg" aria-hidden="true"></i></button>
            </td>
        </tr>`;
    // Add row
    $("#tbodyNhiemVuChi tr").eq(parentRowIndex).after(row);

    ResetSelect2();
    //RebindingValidateMoney();
}

// Xóa dòng nhiệm vụ chi
function RemoveRowKeHoachChiTiet(e) {

    // Lấy dòng hiện tại
    let currentRow = $(e).parents('tr');
    let stt = currentRow.find('td[name="sMaThuTu"]').text();

    // Xóa dòng hiện tại thông qua index.
    let index = currentRow.index();
    $("#tbodyNhiemVuChi tr").eq(index).remove();

    // Tìm dòng cha nếu có thì chỉnh lại stt
    let indexOfDot = stt.lastIndexOf('.');
    if (indexOfDot != -1) {
        let sttParent = stt.substring(0, indexOfDot);
        RefreshThuTuAndGetThuTuMoi(sttParent, 0);

        // Sau khi xóa dòng thì tìm xem còn thằng nào cùng cấp không?
        const regex = new RegExp('^' + sttParent + '.[0-9]+$');
        let hasValue = false;
        let rowParent = undefined;
        $("#tbodyNhiemVuChi tr td[name='sMaThuTu']").each(function (e) {
            let value = $(this).text();

            // Nếu match với regex thì là có thằng cùng cấp
            if (regex.test(value)) {
                hasValue = true;
                return;
            }

            // Tiện tìm luôn dòng cha theo stt
            if (value == sttParent) {
                rowParent = $(this).parents('tr');
            }
        });

        // Nếu không có dòng nào cùng cấp thì enable lại function của dòng cha.
        if (!hasValue) {
            if (rowParent) {
                let isTTCP = ($.trim(rowParent.find('td[name="bIsTTCP"]').text()).toLowerCase() === 'true');
                // Nếu dòng cha là TTCP thì thêm dropdown đơn vị.
                if (isTTCP) {
                    let selectDonViInParrentRow = rowParent.find('select[name="iID_DonViID"]');
                    if (selectDonViInParrentRow.length > 0) {
                        selectDonViInParrentRow.remove();
                    }

                    let tdContainSelectDonVi = rowParent.find('td[name="cellSelectDonVi"]');
                    tdContainSelectDonVi.append(CreateLookupDonVi())
                    ResetSelect2();
                }

                //if (isVNDtoUSD) {
                //    rowParent.find('#fGiaTriBQP_VND').prop('disabled', false);
                //} else {
                //    rowParent.find('#fGiaTriBQP_USD').prop('disabled', false);
                //}
                rowParent.find('button.btn-delete').removeClass('disabled');
            }
        }
    }

    // Tính lại tổng tiền từ dòng hiện tại đổ lên.
    SumValueParent(stt);
}

// Get stt, index dòng thêm mới nhiệm vụ chi
function RefreshThuTuAndGetThuTuMoi(stt, index) {
    // Tạo regex tìm những dòng con của stt truyền vào.
    const regex = new RegExp('^' + stt + '.[0-9]+$');
    let arrMatch = [];
    let hasValue = false;

    // Add những dòng con vào 1 mảng
    $("#tbodyNhiemVuChi tr td[name='sMaThuTu']").each(function (e) {
        let value = $(this).text();
        if (regex.test(value)) {
            arrMatch.push({
                value: value,
                element: $(this)
            });
            hasValue = true;
        }
    });

    // Nếu tìm thấy những dòng con thì lấy index và stt để trả ra ngoài. Đồng thời sắp xếp lại.
    if (hasValue) {
        // Lấy dòng con có stt lớn nhất.
        let lastChildRow = arrMatch.sort((a, b) => parseInt(a.value.replace(stt + '.', '')) - parseInt(b.value.replace(stt + '.', '')))[arrMatch.length - 1];
        let maxStt = lastChildRow.value;
        let maxIndexStt = parseInt(maxStt.replace(stt + '.', ''));

        // Tính ra stt mới.
        let indexSttResult = maxIndexStt + 1;
        let indexRow = lastChildRow.element.parents('tr').index();

        // Lấy stt của dòng con lớn nhất để lấy index của dòng con lớn nhất.
        let sttChecker = lastChildRow.value;

        do {
            // Create regex cho dòng con
            let regexChecker = new RegExp('^' + sttChecker + '.[0-9]+$');
            let arrChecker = [];
            let hasValueChecker = false;

            // Tìm xem có dòng con hay không?
            $("#tbodyNhiemVuChi tr td[name='sMaThuTu']").each(function (e) {
                let value = $(this).text();
                if (regexChecker.test(value)) {
                    arrChecker.push({
                        value: value,
                        element: $(this)
                    });
                    hasValueChecker = true;
                }
            });

            // Nếu tìm thấy dòng con thì gán lại index và stt của dòng con để tiếp tục tìm kiếm
            if (hasValueChecker) {
                let lastChildRowChecker = arrChecker.sort((a, b) => parseInt(a.value.replace(sttChecker + '.', '')) - parseInt(b.value.replace(sttChecker + '.', '')))[arrMatch.length - 1];
                sttChecker = lastChildRowChecker.value;
                indexRow = lastChildRowChecker.element.parents('tr').index();
            } else {
                // Nếu không tìm thấy dòng con nào nữa thì break khỏi vòng lặp.
                break;
            }
        } while (true);

        // Nếu stt lớn nhất != độ dài mảng của những thằng con (tương đương với việc xóa dòng ở trước dòng cuối cùng) thì gán lại stt cho những thằng con.
        if (maxIndexStt != arrMatch.length) {
            let newIndex = 1;
            arrMatch.forEach(x => {
                let oldStt = x.element.text();
                let newStt = stt + '.' + newIndex;

                // Update toàn bộ những thằng con về đúng stt mới.
                $("#tbodyNhiemVuChi tr td[name='sMaThuTu']").each(function (e) {
                    let value = $(this).text();
                    if (value.startsWith(oldStt + '.')) {
                        $(this).text(newStt + value.substring(oldStt.length));
                    }
                });

                x.element.text(newStt);
                newIndex++;
            });
            indexSttResult = newIndex;
        }

        return {
            sttNew: stt + '.' + indexSttResult,
            parentRowIndex: indexRow
        };
    } else {
        // Nếu không có thằng con nào thì bắt đầu với 1.
        return {
            sttNew: stt + '.1',
            parentRowIndex: index
        };
    }
}

// Tạo lookup đơn vị
function CreateLookupDonVi() {

    var html = '<select class="form-control" name="iID_DonViID">';

    lstDonVi.forEach(x => {
        html += '<option data-madonvi="' + $("<div/>").text(x.iID_MaDonVi).html() + '" value="' + x.iID_Ma + '">' + $("<div/>").text(x.sMoTa).html() + '</option>';
    });

    html += '</select>';

    return html;
}

// Tính giá trị qua tỉ giá
function CalcTiGia(type, e) {
    let tr = $(e).parents('tr');
    let stt = tr.find("td[name='sMaThuTu']").text();
    if (isVNDtoUSD) {
        if (type == 'VND') {
            let elInputUSD = tr.find('.inputFromUSD');
            let inputVND = parseFloat(UnFormatNumber($(e).val()));
            if (isNaN(inputVND)) {
                elInputUSD.val(0);
                $(e).val(0);
            } else {
                $.ajax({
                    type: "POST",
                    data: { number: inputVND.toString(), numTiGia: tiGia.toString() },
                    url: '/QLNH/KeHoachChiTietBQP/CalcMoneyByTiGia',
                    async: false,
                    success: function (data) {
                        let result = FormatNumber(data.result);
                        elInputUSD.val(result == '' ? 0 : result);
                    }
                });
            }
        }
    } else {
        if (type == 'USD') {
            let elInputVND = tr.find('.inputFromVND');
            let inputUSD = parseFloat(UnFormatNumber($(e).val()));
            if (isNaN(inputUSD)) {
                elInputVND.val(0);
                $(e).val(0);
            } else {
                $.ajax({
                    type: "POST",
                    data: { number: inputUSD.toString(), numTiGia: tiGia.toString() },
                    url: '/QLNH/KeHoachChiTietBQP/CalcMoneyByTiGia',
                    async: false,
                    success: function (data) {
                        let result = FormatNumber(data.result, 0);
                        elInputVND.val(result == '' ? 0 : result);
                    }
                });
            }
        }
    }

    SumValueParent(stt);
}

// Tính tổng giá trị cho dòng cha
function SumValueParent(sttCurrentRow) {
    // Lấy stt dòng hiện tại
    let indexOfDot = sttCurrentRow.lastIndexOf('.');
    if (indexOfDot != -1) {
        // Nêu có thể cắt chuỗi stt thì cắt để tìm stt của parent
        let sttParent = sttCurrentRow.substring(0, indexOfDot);
        let rowParent = undefined;

        // Tạo regex tìm dòng con
        const regexChild = new RegExp('^' + sttParent + '.[0-9]+$');
        let lstVND = [];
        let lstUSD = [];

        // Load qua toàn bộ dòng trong bảng để tìm dòng con và lấy giá trị để tính tổng
        $("#tbodyNhiemVuChi tr").each(function () {
            let sttRow = $(this).find("td[name='sMaThuTu']").text();

            // Lấy dòng cha.
            if (sttRow == sttParent) {
                rowParent = $(this);
            }

            // Nếu dòng con match với regex thì get value.
            if (regexChild.test(sttRow)) {
                // Lấy giá trị của USD và VND rồi cộng vào sum
                let valueUSD = UnFormatNumber($(this).find("#fGiaTriBQP_USD").val());
                let valueVND = UnFormatNumber($(this).find("#fGiaTriBQP_VND").val());

                lstUSD.push(valueUSD.toString());
                lstVND.push(valueVND.toString());
            }
        });

        $.ajax({
            type: "POST",
            data: { lstNumVND: lstVND, lstNumUSD: lstUSD },
            url: '/QLNH/KeHoachChiTietBQP/SumTwoList',
            async: false,
            success: function (data) {
                // Nếu có rowParent thì gán value
                if (rowParent) {
                    rowParent.find("#fGiaTriBQP_USD").val(data.resultUSD == '' ? 0 : data.resultUSD);
                    rowParent.find("#fGiaTriBQP_VND").val(data.resultVND == '' ? 0 : data.resultVND);
                    SumValueParent(sttParent);
                }
            }
        });
    }
}

// Tính giá trị qua tỉ giá nhưng ko tính lại tổng
function CalcListTiGia(type, listInput) {

    var listTiGiaModel = [];
    if (type == 'VND') {
        listInput.each(function() {
            let inputVNDElement = $(this);

            // Lấy thông tin dòng, stt, inputUSD, inputVND
            let tr = $(inputVNDElement).parents('tr');
            let stt = tr.find("td[name='sMaThuTu']").text();
            let elInputUSD = tr.find('.inputFromUSD');
            let inputVND = parseFloat(UnFormatNumber($(inputVNDElement).val()));

            // Nếu inputVND ko phải số ( == '') hoặc bằng 0 thì gán giá trị = 0, ko phải tính.
            if (isNaN(inputVND) || inputVND === 0) {
                elInputUSD.val(0);
                $(inputVNDElement).val(0);
            } else {
                // Nếu giá trị != 0 thì đẩy vào list để tính 1 thể.
                let objTiGia = {
                    sMoney: inputVND.toString(),
                    sMaThuTu: stt,
                    iLevel: stt.split('.').length - 1,
                    iGroup: parseInt(stt.substring(0, stt.indexOf('.'))),
                    iIndexRow: tr.index()
                };
                listTiGiaModel.push(objTiGia);
            }
        });
    } else {
        listInput.each(function() {
            let inputUSDElement = $(this);

            // Lấy thông tin dòng, stt, inputUSD, inputVND
            let tr = $(inputUSDElement).parents('tr');
            let stt = tr.find("td[name='sMaThuTu']").text();
            let elInputVND = tr.find('.inputFromVND');
            let inputUSD = parseFloat(UnFormatNumber($(inputUSDElement).val()));

            // Nếu inputVND ko phải số ( == '') hoặc bằng 0 thì gán giá trị = 0, ko phải tính.
            if (isNaN(inputUSD) || inputUSD === 0) {
                elInputVND.val(0);
                $(inputUSDElement).val(0);
            } else {
                // Nếu giá trị != 0 thì đẩy vào list để tính 1 thể.
                let objTiGia = {
                    sMoney: inputUSD.toString(),
                    sMaThuTu: stt,
                    iLevel: stt.split('.').length - 1,
                    iGroup: parseInt(stt.substring(0, stt.indexOf('.'))),
                    iIndexRow: tr.index()
                };
                listTiGiaModel.push(objTiGia);
            }
        });
    }

    // Tính tỉ giá
    $.ajax({
        type: "POST",
        data: { datas: listTiGiaModel, numTiGia: tiGia.toString() },
        url: '/QLNH/KeHoachChiTietBQP/CalcListMoneyByTiGia',
        async: false,
        success: function (data) {
            // Gán lại giá trị và tính lại tổng
            data.result.forEach(x => {
                let money = FormatNumber(x.dResult);
                let row = $('#tbodyNhiemVuChi tr:eq(' + x.iIndexRow + ')');
                if (type == 'VND') {
                    let inputUSD = row.find('.inputFromVND');
                    inputUSD.val(money == '' ? 0 : money);
                } else {
                    let inputVND = row.find('.inputFromUSD');
                    inputVND.val(money == '' ? 0 : money);
                }

                // Tính lại stt
                SumValueParent(x.sMaThuTu);
            });
        }
    });
}

// Validate form Nhiệm vụ chi
function ValidateNhiemVuChi(lstNvc, text) {
    var Title = text;
    var Messages = [];

    // Check bảng nhiệm vụ chi đã có data.
    if (lstNvc.length == 0) {
        Messages.push("Quyết định hiện tại chưa có chương trình, nhiệm vụ chi. Vui lòng thêm chương trình, nhiệm vụ chi.");
    }

    lstNvc.forEach(x => {
        if ($.trim(x.sTenNhiemVuChi) == '') {
            Messages.push(`Chương trình, nhiệm vụ chi ${x.sMaThuTu} chưa có thông tin tên chương trình, nhiệm vụ chi.`);
        }
        if ($.trim(x.fGiaTriUSD) == '') {
            Messages.push(`Chương trình, nhiệm vụ chi ${x.sMaThuTu} chưa có thông tin giá trị BQP phê duyệt USD.`);
        } else if (!(x.fGiaTriUSD > 0)) {
            Messages.push(`Chương trình, nhiệm vụ chi ${x.sMaThuTu} có giá trị BQP phê duyệt USD không hợp lệ.`);
        }
        if ($.trim(x.fGiaTriVND) == '') {
            Messages.push(`Chương trình, nhiệm vụ chi ${x.sMaThuTu} chưa có thông tin giá trị BQP phê duyệt VND.`);
        } else if (!(x.fGiaTriVND > 0)) {
            Messages.push(`Chương trình, nhiệm vụ chi ${x.sMaThuTu} có giá trị BQP phê duyệt VND không hợp lệ.`);
        }
        if (x.hasDonVi) {
            if (x.iID_DonViID == GUID_EMPTY || x.iID_DonViID.trim() == '') {
                Messages.push(`Chương trình, nhiệm vụ chi ${x.sMaThuTu} chưa có thông tin đơn vị.`);
            }
        }
    });

    if (Messages.length > 0) {
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: Messages, Category: ERROR },
            success: function (data) {
                $("#divModalConfirm").html(data);
                $("#divModalConfirm #confirmModal div.modal-dialog").css("width", "45%");
                $("#divModalConfirm #confirmModal div#contentConfirmModal").css("max-height", "80vh").css("overflow", "auto");
            }
        });
        return false;
    }

    return true;
}

// Binding lại select2 khi thêm dòng.
function ResetSelect2() {
    $("select[name='iID_DonViID']").select2({
        width: '100%',
        matcher: FilterInComboBox
    });
}

// Refresh detail
function RefreshDetail() {
    $("#iID_BQuanLyID").val(GUID_EMPTY);
    $("#iID_DonViID").val(GUID_EMPTY);

    var id = $("#hIDKHCT").val();
    OpenModalDetail(id, 'DETAIL', true);
}

// Check validate number, nếu validate oke thì tính giá tiền quy đổi. Nếu validate ko ok thì ko tính. Dùng cho onblur
function IsActionCalcTiGia(event, textbox, typeValidate, nTofix, typeCalc) {
    // Nếu nhập ô USD thì validate ô USD, nếu ko có lỗi thì tính tỉ giá ô VND và ngược lại.
    if (ValidateInputFocusOut(event, textbox, typeValidate, nTofix)) {
        CalcTiGia(typeCalc, textbox);
    }
}

// Validate giá trị TTCP >= giá trị BQP.
function IsActionSaveNhiemVuChi(lstNhiemVuChis, keHoachChiTietBQP, state, isNotCondition) {
    if (isNotCondition) {
        // Nếu bỏ qua điểu kiện thì call ajax lưu luôn.
        CallAjaxSaveData(lstNhiemVuChis, keHoachChiTietBQP, state);
        return;
    }

    var lstSttInValid = [];
    lstNhiemVuChis.forEach(nvc => {
        if (nvc.bIsTTCP) {
            // Nếu giá trị bộ quốc phòng lớn hơn giá trị TTCP thì xác nhận có muốn tiếp tục lưu hay không?
            if (nvc.fGiaTriUSD - nvc.fGiaTriTTCP_USD > 0) {
                lstSttInValid.push(nvc.sMaThuTu);
            }
        }
    });
    var stringSttAlert = "";
    if (lstSttInValid.length > 0) {
        // Nếu xác định có dòng không hợp lệ thì show confirm popup
        stringSttAlert = lstSttInValid.join(', ');

        var Title = 'Xác nhận lưu kế hoạch chi tiết Bộ Quốc phòng phê duyệt';
        var Messages = [];
        Messages.push('Các nhiệm vụ chi: ' + stringSttAlert + ' đang có giá trị BQP phê duyệt lớn hơn giá trị TTCP phê duyệt.');
        Messages.push('Bạn có chắc chắn muốn lưu kế hoạch chi tiết Bộ Quốc phòng này?');

        $.ajax({
            type: "POST",
            url: "/QLNH/KeHoachChiTietBQP/ShowModalConfirmSave",
            data: { title: Title, messages: Messages },
            success: function (data) {
                $("#divModalConfirm").empty().html(data);
            }
        });
        return;
    } else {
        // Nếu không có dòng nào không hợp lệ thì lưu
        CallAjaxSaveData(lstNhiemVuChis, keHoachChiTietBQP, state);
        return;
    }
}

// Call ajax lưu all data
function CallAjaxSaveData(lstNhiemVuChis, keHoachChiTietBQP, state) {
    $.ajax({
        type: "POST",
        url: "/QLNH/KeHoachChiTietBQP/SaveKeHoachChiTietBQP",
        data: { lstNhiemVuChis: lstNhiemVuChis, keHoachChiTietBQP: keHoachChiTietBQP, state: state },
        success: function (data) {
            if (data.result) {
                ViewToList();
            } else {
                var Title = 'Lỗi thêm mới kế hoạch chi tiết Bộ Quốc phòng phê duyệt!';
                var Messages = ['Thêm mới kế hoạch chi tiết Bộ Quốc phòng phê duyệt không thành công!'];
                if (state == 'UPDATE') {
                    Title = 'Lỗi cập nhật kế hoạch chi tiết Bộ Quốc phòng phê duyệt!';
                    Messages = ['Cập nhật kế hoạch chi tiết Bộ Quốc phòng phê duyệt không thành công!'];
                } else if (state == 'ADJUST') {
                    Title = 'Lỗi điều chỉnh kế hoạch chi tiết Bộ Quốc phòng phê duyệt!';
                    Messages = ['Điều chỉnh kế hoạch chi tiết Bộ Quốc phòng phê duyệt không thành công!'];
                }

                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: Messages, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
            }
        }
    });
}

// #endregion ======================================================================