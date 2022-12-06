var CONFIRM = 0;
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var ERROR = 1;
var CURRENT_STATE = '';
var USE_LAST_NEW_ADJUST = false;
var lstPhongBan = [];

// Document Ready
$(document).ready(function () {

    // Event change Loại Kế Hoach form thêm mới, chỉnh sửa, điều chỉnh.
    $("#iLoai").on('change', function (e) {
        let value = $(this).val();
        let gdt = $("#formGiaiDoanTu");
        let gdd = $("#formGiaiDoanDen");
        let nkh = $("#formNamKeHoach");
        let pr = $("#formParrentID");
        if (value == 1) {
            gdt.removeClass('hidden');
            gdd.removeClass('hidden');
            nkh.addClass('hidden');
            pr.addClass('invisible');
        } else if (value == 2) {
            gdt.addClass('hidden');
            gdd.addClass('hidden');
            nkh.removeClass('hidden');
            pr.removeClass('invisible');
        } else if (value == 3) {
            gdt.removeClass('hidden');
            gdd.removeClass('hidden');
            nkh.addClass('hidden');
            pr.removeClass('invisible');
        } else {
            gdt.removeClass('hidden');
            gdd.removeClass('hidden');
            nkh.addClass('hidden');
            pr.addClass('invisible');
        }
    });

    $("#iID_ParentID").select2({
        width: '100%',
        matcher: FilterInComboBox
    });

    if (CURRENT_STATE == 'DETAIL') {
        $("#iID_BQuanLyID").select2({
            width: '100%',
            matcher: FilterInComboBox
        });

        $("#iID_BQuanLyID").on('change', function () {
            OpenModalDetail($("#hKHTTId").val(), 'DETAIL', true)
        });
    } else {
        $("[name='iID_BQuanLyIDSelect2']").select2({
            width: '100%',
            matcher: FilterInComboBox
        });
    }

    $('#modalKHTTCP').on('hidden.bs.modal', function () {
        USE_LAST_NEW_ADJUST = false;
    });


});

// Làm mới danh sách
function ResetChangePage() {
    $("#txtFromDateFitler").val('');
    $("#txtToDateFitler").val('');
    $("#txtSoKeHoachFilter").val('');
    $("#txtNgayBanHanhFilter").val('');

    GetListData(1);
}

// Phân trang event
function ChangePage(iCurrentPage = 1) {
    GetListData(iCurrentPage);
}

// Tìm kiếm
function GetListData(currentPage = 1) {
    _paging.CurrentPage = currentPage;
    var filter = {
        giaiDoanTu:  $.trim($("#txtFromDateFitler").val()),
        giaiDoanDen: $.trim($("#txtToDateFitler").val()),
        soKeHoach:   $("<div/>").text($.trim($("#txtSoKeHoachFilter").val())).html(),
        ngayBanHanh: $.trim($("#txtNgayBanHanhFilter").val())
    }

    $.ajax({
        type: "POST",
        url: "/QLNH/KeHoachTongTheTTCP/ListPage",
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

// Open model create, edit, update Kế hoạch TTCP
function OpenModal(id, state) {
    $("#modalKHTTCPLabel").html('');

    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/KeHoachTongTheTTCP/GetDetail",
        async: false,
        data: { id: id, state: state },
        success: function (data) {
            $("#contentModalKHTTCP").empty().html(data);
            if (state == 'CREATE') {
                $("#modalKHTTCPLabel").empty().html('Thêm mới quyết định tổng thể Thủ tướng Chính phủ');
            } else if (state == 'UPDATE') {
                $("#modalKHTTCPLabel").empty().html('Sửa quyết định tổng thể Thủ tướng Chính phủ');
            } else {
                $("#modalKHTTCPLabel").empty().html('Điều chỉnh quyết định tổng thể Thủ tướng Chính phủ');
            }
        }
    });

    CURRENT_STATE = state;
}

// Chuyển sang modal chi tiết nhiệm vụ chi
function Save() {
    let state = CURRENT_STATE;

    // Get data
    let data = {};
    data.iLoai = $("#iLoai").val();
    data.iGiaiDoanTu =  $.trim($("#txtGiaiDoanTu").val());
    data.iGiaiDoanDen = $.trim($("#txtGiaiDoanDen").val());
    data.iNamKeHoach =  $.trim($("#txtNamKeHoach").val());
    data.iID_ParentID = $("#iID_ParentID").val();
    data.fromYear =     $("#iID_ParentID").find(':selected').data('fromdate');
    data.toYear =       $("#iID_ParentID").find(':selected').data('todate');
    data.sSoKeHoach =   $("<div/>").text($.trim($("#txtSoKeHoach").val())).html();
    data.dNgayKeHoach = $("#txtNgayKeHoach").val();
    data.sMoTaChiTiet = $("<div/>").text($.trim($("#txtMoTaChiTiet").val())).html();
    data.bIsXoa = false;

    // Check state
    if (CURRENT_STATE == 'UPDATE') {
        // Validate
        if (!ValidateTTCP(data, "Lỗi cập nhật kế hoạch tổng thể Thủ tướng Chính phủ")) {
            return;
        }

        // Update thì truyền thêm ID để lấy data phía BE và update.
        data.id = $("#hIdKeHoachChiTietBQP").val();
    } else if (CURRENT_STATE == 'CREATE') {
        // Validate
        if (!ValidateTTCP(data, "Lỗi thêm mới kế hoạch tổng thể Thủ tướng Chính phủ")) {
            return;
        }

        // Thêm mới thì thêm các trường thông tin liên quan đến điều chỉnh
        data.bIsGoc = true;
        data.bIsActive = true;
        data.iLanDieuChinh = 0;
    } else {
        // Validate
        if (!ValidateTTCP(data, "Lỗi điều chỉnh kế hoạch tổng thể Thủ tướng Chính phủ")) {
            return;
        }

        // Điều chỉnh thì gán lại old ID cho data để lấy thông tin nhiệm vụ chi lên view.
        data.bIsGoc = false;
        data.bIsActive = true;
        data.iLanDieuChinh = parseInt($("#hiLanDieuChinh").val()) + 1;
        data.iID_ParentAdjustID = $("#hIdKeHoachChiTietBQP").val();
        data.ID = $("#hIdKeHoachChiTietBQP").val();
    }

    OpenModalDetail(data, CURRENT_STATE, false);
}

// View to detail
function OpenModalDetail(data, state, isFilter = false) {
    // Nếu là xem chi tiết thì data sẽ là Id của kế hoạch TTCP. Nếu ko thì data là model kế hoạch tổng thể TTCP.
    if (state == 'DETAIL') {
        USE_LAST_NEW_ADJUST = false;
        let temp = data;
        data = {
            ID: temp,
            iID_ParentID: null
        };

        if (isFilter) {
            data.iID_BQuanLyID = $("#iID_BQuanLyID").val();
        }
    }

    // View list nhiệm vụ chi
    $.ajax({
        type: "POST",
        url: "/QLNH/KeHoachTongTheTTCP/NhiemVuChiDetail",
        data: { input: data, state: state, isUseLastTTCP: USE_LAST_NEW_ADJUST },
        async: false,
        success: function (result) {
            $("#modalKHTTCP").modal('hide');
            // View result
            $("#lstDataView").html(result);
            if (isFilter) {
                $("#iID_BQuanLyID").val(data.iID_BQuanLyID);
            }
        }
    });

    CURRENT_STATE = state;

    // Lấy danh sách phòng ban để tạo lookup khi thêm dòng nhiệm vụ chi
    if (CURRENT_STATE != 'DETAIL') {
        $.ajax({
            type: "POST",
            url: "/QLNH/KeHoachTongTheTTCP/GetDataLookupDetail",
            async: false,
            success: function (result) {
                lstPhongBan = result.ListPhongBan;
            }
        });
    }
}

// Chuyển về màn danh sách kế hoạch TTCP
function ViewToList() {
    var filter = {
        giaiDoanTu: null,
        giaiDoanDen: null,
        soKeHoach: null,
        ngayBanHanh: null
    }

    $.ajax({
        type: "POST",
        url: "/QLNH/KeHoachTongTheTTCP/ListPage",
        data: { input: filter, paging: null },
        success: function (data) {
            // View result
            $("#lstDataView").html(data);
        }
    });
}

// Thêm dòng nhiệm vụ chi
function AddRowKeHoachChiTiet(e) {
    var sttNew = "1";
    var parentRowIndex = 1;
    if (e) {
        let parentRow = $(e).parents('tr');
        let stt = parentRow.find('td[name="sMaThuTu"]').text();

        // Disable input + button delete của dòng cha.
        parentRow.find('#fGiaTri').prop('disabled', true);
        parentRow.find('button.btn-delete').addClass('disabled');

        // Lấy số thứ tự mới và index dòng mới thêm vào.
        var { sttNew, parentRowIndex } = RefreshThuTuAndGetThuTuMoi(stt, parentRow.index());
    } else {
        let regexParent = new RegExp('^[0-9]+$');
        let arrStt = [];
        let hasValue = false;
        $("#tbodyNhiemVuChi tr td[name='sMaThuTu']").each(function (e) {
            let value = $(this).text();

            if (regexParent.test(value)) {
                arrStt.push({
                    value: value,
                    element: $(this)
                });
                hasValue = true;
            }
        });

        if (hasValue) {
            let maxElement = arrStt.sort((a, b) => parseInt(a.value) - parseInt(b.value))[arrStt.length - 1];
            sttNew = (parseInt(maxElement.value) + 1).toString();
        }
    }

    let rowHtml = `
        <tr>
            <td name="ID_NhiemVuChi" class="hidden"></td>
            <td align="left" name="sMaThuTu">` + sttNew + `</td>
            <td><input type="text" id="sTenNhiemVuChi" class="form-control" value="" autocomplete="off"></td>
            <td align="left">
                ` + CreateLookupBQuanLy() + `
            </td>
            <td align="right">
                <input type="text" id="fGiaTri" class="form-control" value="" maxlength="255" autocomplete="off" 
                        onblur="IsActionSumValueParent(event, this, 2, 2)" onkeydown="ValidateInputKeydown(event, this, 2)"/>
            </td>
            <td class="hidden" name="iID_ParentID"></td>
            <td align="center">
                <button type="button" class="btn-detail" onclick="AddRowKeHoachChiTiet(this)"><i class="fa fa-plus" aria-hidden="true"></i></button>
                <button type="button" class="btn-delete" onclick="RemoveRowKeHoachChiTiet(this)"><i class="fa fa-trash-o fa-lg" aria-hidden="true"></i></button>
            </td>
        </tr>`;

    // Add row
    if (e) {
        $("#tbodyNhiemVuChi tr").eq(parentRowIndex).after(rowHtml);
    } else {
        $("#tbodyNhiemVuChi").append(rowHtml);
    }
    ResetSelect2();
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
                rowParent.find('#fGiaTri').prop('disabled', false);
                rowParent.find('button.btn-delete').removeClass('disabled');
            }
        }
    } else {
        // Nếu nó là dòng cha lớn nhất (level 0) thì check dòng cùng cấp và update lại toàn bộ stt.
        let numOfRow = $('#tbodyNhiemVuChi tr').length;
        if (numOfRow != 0) {
            // Lấy các dòng level 0
            let regexCurrent = new RegExp('^[0-9]+$');
            let arrStt = [];
            let hasValue = false;
            $("#tbodyNhiemVuChi tr td[name='sMaThuTu']").each(function (e) {
                let value = $(this).text();

                if (regexCurrent.test(value)) {
                    arrStt.push({
                        value: value,
                        element: $(this)
                    });
                    hasValue = true;
                }
            });

            // Nêu có thì check valid thứ tự
            if (hasValue) {
                let maxElement = arrStt.sort((a, b) => parseInt(a.value) - parseInt(b.value))[arrStt.length - 1];
                let sttMax = parseInt(maxElement.value);

                // Nếu stt lớn nhất != độ dài mảng của những thằng cùng cấp (tương đương với việc xóa dòng ở trước dòng cuối cùng) thì gán lại stt.
                if (sttMax != arrStt.length) {
                    let newIndex = 1;
                    arrStt.forEach(x => {
                        let oldStt = x.element.text();
                        let newStt = newIndex;

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
                }
            }
        }
    }

    // Tính lại tổng tiền từ dòng hiện tại đổ lên.
    SumValueParent(null, stt);
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
function CreateLookupBQuanLy() {

    var html = '<select class="form-control" name="iID_BQuanLyIDSelect2">';

    lstPhongBan.forEach(x => {
        html += '<option value="' + x.Id + '">' + $("<div/>").text(x.DisplayName).html() + '</option>';
    });

    html += '</select>';

    return html;
}

// Tính tổng giá trị cho dòng cha
function SumValueParent(element, stt) {
    let valFormat = element ? FormatNumber(UnFormatNumber($(element).val()), 2) == '' ? 0.00 : FormatNumber(UnFormatNumber($(element).val()), 2) : 0.00;
    if (element) {
        $(element).val(valFormat);
    }

    var sttCurrentRow = '1';
    if (element) {
        sttCurrentRow = $(element).parents('tr').find("td[name='sMaThuTu']").text();
    } else {
        sttCurrentRow = stt;
    }
    
    // Lấy stt dòng hiện tại
    let indexOfDot = sttCurrentRow.lastIndexOf('.');
    if (indexOfDot != -1) {
        // Nêu có thể cắt chuỗi stt thì cắt để tìm stt của parent
        let sttParent = sttCurrentRow.substring(0, indexOfDot);
        let rowParent = undefined;

        // Tạo regex tìm dòng con
        const regexChild = new RegExp('^' + sttParent + '.[0-9]+$');
        let lstUSD = [];
        let lstVND = [];

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
                let valueUSD = UnFormatNumber($(this).find("#fGiaTri").val());

                lstUSD.push(valueUSD.toString());
            }
        });

        // Nếu có rowParent thì gán value
        if (rowParent) {
            $.ajax({
                type: "POST",
                data: { lstNumVND: lstVND, lstNumUSD: lstUSD },
                url: '/QLNH/KeHoachChiTietBQP/SumTwoList',
                async: false,
                success: function (data) {
                    rowParent.find("#fGiaTri").val(data.resultUSD == '' ? 0 : data.resultUSD);
                    SumValueParent(rowParent.find("#fGiaTri"));
                }
            });
        }
    }
}

// Lưu chi tiết nhiệm vụ chi
function SaveDetail() {
    let state = $("#currentState").val();
    let keHoachTongTheTTCP = $("<div/>").text($("#keHoachTongTheTTCP").val()).html();

    var tableNhiemVuChi = [];
    $("#tbodyNhiemVuChi tr").each(function (e) {
        let rowElement = $(this);

        let rowData = {};
        rowData.ID = $.trim(rowElement.find('td[name="ID_NhiemVuChi"]').text());
        rowData.sMaThuTu = $.trim(rowElement.find('td[name="sMaThuTu"]').text());
        rowData.sTenNhiemVuChi = $("<div/>").text($.trim(rowElement.find('#sTenNhiemVuChi').val())).html();
        rowData.iID_BQuanLyID = rowElement.find('[name="iID_BQuanLyIDSelect2"]').val();
        rowData.sGiaTri = UnFormatNumber($.trim(rowElement.find('#fGiaTri').val()).toString());
        rowData.iID_ParentID = rowElement.find('td[name="iID_ParentID"]').text();
        tableNhiemVuChi.push(rowData);
    });

    if (state == "CREATE") {
        if (!ValidateNhiemVuChi(tableNhiemVuChi, "Lỗi thêm mới thông tin chương trình kế hoạch tổng thể TTCP")) {
            return;
        }
    } else if (state == "UPDATE") {
        if (!ValidateNhiemVuChi(tableNhiemVuChi, "Lỗi cập nhật thông tin chương trình kế hoạch tổng thể TTCP")) {
            return;
        }
    } else {
        if (!ValidateNhiemVuChi(tableNhiemVuChi, "Lỗi điều chỉnh thông tin chương trình kế hoạch tổng thể TTCP")) {
            return;
        }
    }
    

    $.ajax({
        type: "POST",
        url: "/QLNH/KeHoachTongTheTTCP/SaveKHTongTheTTCP",
        data: { lstNhiemVuChis: tableNhiemVuChi, keHoachTongTheTTCP: keHoachTongTheTTCP, state: state },
        success: function (data) {
            if (data.result) {
                ViewToList();
            } else {
                var Title = 'Lỗi thêm mới kế hoạch tổng hợp Thủ tướng Chính phủ phê duyệt!';
                var Messages = ['Thêm mới kế hoạch tổng hợp Thủ tướng Chính phủ phê duyệt không thành công!'];
                if (state == 'UPDATE') {
                    Title = 'Lỗi cập nhật kế hoạch tổng hợp Thủ tướng Chính phủ phê duyệt!';
                    Messages = ['Cập nhật kế hoạch tổng hợp Thủ tướng Chính phủ phê duyệt không thành công!'];
                } else if (state == 'ADJUST') {
                    Title = 'Lỗi điều chỉnh kế hoạch tổng hợp Thủ tướng Chính phủ phê duyệt!';
                    Messages = ['Điều chỉnh kế hoạch tổng hợp Thủ tướng Chính phủ phê duyệt không thành công!'];
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

// Confirm xóa
function ConfirmDelete(id, sNam, sSoKeHoach) {
    var Title = 'Xác nhận xóa quyết định tổng thể TTCP phê duyệt';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa quyết định tổng thể: ' + $("<div/>").text(sSoKeHoach).html() + ' - ' + sNam + '?');
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
        url: "/QLNH/KeHoachTongTheTTCP/KH_TTCPDelete",
        data: { id: id },
        success: function (r) {
            if (r && r.bIsComplete) {
                location.reload();
            } else {
                var Title = 'Lỗi xóa kế hoạch tổng thể TTCP phê duyệt';
                var messErr = [];
                messErr.push(r.sMessError);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
            }
        }
    });
}

// Tìm ra thằng cha active và gán lại vào select.
function UpdateParentTTCP(iID_Parent_TTCP) {
    $.ajax({
        type: "POST",
        url: "/QLNH/KeHoachTongTheTTCP/FindParentTTCPActive",
        async: false,
        data: { id: iID_Parent_TTCP },
        success: function (data) {
            $("#iID_ParentID").val(data.result.ID).change();
            USE_LAST_NEW_ADJUST = true;
        }
    });
}

// Validate form TTCP
function ValidateTTCP(data, text) {
    var Title = text;
    var Messages = [];
    var hasValueFromYear = false;
    var hasValueToYear = false;
    var hasValueNamKH = false;
    var hasValueTTCPParent = false;

    // Check theo loại giai đoạn: 1 - theo giai đoạn, 2 - theo năm, 3 - theo giai đoạn con
    if (data.iLoai == 1) {
        if ($.trim(data.iGiaiDoanTu) == '') {
            Messages.push("Chưa có thông tin về giai đoạn từ, vui lòng nhập giai đoạn từ.");
        } else {
            hasValueFromYear = true;
        }
        if ($.trim(data.iGiaiDoanDen) == '') {
            Messages.push("Chưa có thông tin về giai đoạn đến, vui lòng nhập giai đoạn đến.");
        } else {
            hasValueToYear = true;
        }
    } else if (data.iLoai == 2) {
        if ($.trim(data.iNamKeHoach) == '') {
            Messages.push("Chưa có thông tin về năm kế hoạch, vui lòng nhập năm kế hoạch.");
        } else {
            hasValueNamKH = true;
        }
        if (data.iID_ParentID == GUID_EMPTY || $.trim(data.iID_ParentID) == '') {
            Messages.push("Chưa có thông tin về kế hoạch tổng thể TTCP cha, vui lòng chọn kế hoạch tổng thể TTCP cha.");
        } else {
            hasValueTTCPParent = true;
        }
    } else {
        if (data.iID_ParentID == GUID_EMPTY || $.trim(data.iID_ParentID) == '') {
            Messages.push("Chưa có thông tin về kế hoạch tổng thể TTCP cha, vui lòng chọn kế hoạch tổng thể TTCP cha.");
        }
        if ($.trim(data.iGiaiDoanTu) == '') {
            Messages.push("Chưa có thông tin về giai đoạn từ, vui lòng nhập giai đoạn từ.");
        } else {
            hasValueFromYear = true;
        }
        if ($.trim(data.iGiaiDoanDen) == '') {
            Messages.push("Chưa có thông tin về giai đoạn đến, vui lòng nhập giai đoạn đến.");
        } else {
            hasValueToYear = true;
        }
    }

    // Check số kế hoạch
    if ($.trim($("#txtSoKeHoach").val()) == "") {
        Messages.push("Chưa có thông tin về số kế hoạch, vui lòng nhập số kế hoạch.");
    }
    if ($.trim($("#txtSoKeHoach").val()) != "" && $.trim($("#txtSoKeHoach").val()).length > 100) {
        Messages.push("Số kế hoạch nhập quá 100 kí tự, vui lòng nhập lại số kế hoạch.");
    }

    // Check ngày kế hoạch
    if ($.trim($("#txtNgayKeHoach").val()) != "" && !dateIsValid($.trim($("#txtNgayKeHoach").val()))) {
        Messages.push("Ngày kế hoạch không hợp lệ, vui lòng nhập lại ngày kế hoạch.");
    }

    // Check fromYear <= ToYear. Nếu chọn theo giai đoạn và có giá trị cả 2 trường từ năm, đến năm thì check.
    if (data.iLoai != 2 && hasValueFromYear && hasValueToYear) {

        // Check fromYear <= ToYear
        if (data.iGiaiDoanTu - data.iGiaiDoanDen > 0) {
            Messages.push("Giai đoạn từ không được lớn hơn giai đoạn đến.");
        }

        if (data.iLoai == 3) {
            // Check theo giai đoạn con, nếu giai đoạn từ nhỏ hơn fromDate || giai đoạn từ lớn hơn toDate thì lỗi.
            if (data.iGiaiDoanTu - data.fromYear < 0 || data.iGiaiDoanTu - data.toYear > 0) {
                Messages.push("Giai đoạn từ không nằm trong giai đoạn của quyết định tổng thể TTCP cha.");
            }
            if (data.iGiaiDoanDen - data.fromYear < 0 || data.iGiaiDoanDen - data.toYear > 0) {
                Messages.push("Giai đoạn đến không nằm trong giai đoạn của quyết định tổng thể TTCP cha.");
            }
        }
    }

    // Check fromYear, toYear thuộc vào phạm vi của TTCP cha.
    if (data.iLoai == 2 && hasValueNamKH && hasValueTTCPParent) {
        // Check theo năm
        if (data.iNamKeHoach - data.fromYear < 0 || data.iNamKeHoach - data.toYear > 0) {
            Messages.push("Năm kế hoạch không nằm trong giai đoạn của quyết định tổng thể TTCP cha.");
        }
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

// Validate form Nhiệm vụ chi
function ValidateNhiemVuChi(lstNvc, text) {
    var Title = text;
    var Messages = [];

    // Check bảng nhiệm vụ chi đã có data.
    if (lstNvc.length == 0) {
        Messages.push("Kế hoạch hiện tại chưa có chương trình, nhiệm vụ chi. Vui lòng thêm chương trình, nhiệm vụ chi.");
    }

    lstNvc.forEach(x => {
        if ($.trim(x.sTenNhiemVuChi) == '') {
            Messages.push(`Chương trình, nhiệm vụ chi ${x.sMaThuTu} chưa có thông tin tên chương trình, nhiệm vụ chi.`);
        }
        if (x.iID_BQuanLyID == GUID_EMPTY || $.trim(x.iID_BQuanLyID) == '') {
            Messages.push(`Chương trình, nhiệm vụ chi ${x.sMaThuTu} chưa có thông tin ban quản lý.`);
        }
        if ($.trim(x.sGiaTri) == '') {
            Messages.push(`Chương trình, nhiệm vụ chi ${x.sMaThuTu} chưa có thông tin giá trị phê duyệt.`);
        } else if (!(x.sGiaTri > 0)) {
            Messages.push(`Chương trình, nhiệm vụ chi ${x.sMaThuTu} có giá trị phê duyệt không hợp lệ.`);
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

// Refresh detail
function RefreshDetail() {
    $("#iID_BQuanLyID").val(GUID_EMPTY);
    $("#iID_DonViID").val(GUID_EMPTY);

    var id = $("#hKHTTId").val();
    OpenModalDetail(id, 'DETAIL', true);
}

// Check validate money, nếu oke thì tính sum các dòng, ko thì thôi
function IsActionSumValueParent(event, textbox, type, nTofix) {
    if (ValidateInputFocusOut(event, textbox, type, nTofix)) {
        SumValueParent(textbox);
    }
}

function ResetValueStage() {
    $('#txtGiaiDoanTu').val('');
    $('#txtGiaiDoanDen').val('');
    $('#txtNamKeHoach').val('');
}

// Binding lại select2 khi thêm dòng.
function ResetSelect2() {
    $("select[name='iID_BQuanLyIDSelect2']").select2({
        width: '100%',
        matcher: FilterInComboBox
    });
}