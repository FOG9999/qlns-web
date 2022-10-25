var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';

// Đóng mở dòng
function ToogleRow(e) {
    let tdElement = $(e);
    let trElement = tdElement.closest('tr');
    let id = tdElement.data('id');
    let index = trElement.index();
    let hasValue = tdElement.data('ishaschild');

    let sTenNhiemVuChi = $.trim($("#txtTenNhiemVuChi").val());
    let iID_BQuanLyID = $("#iID_BQuanLyID").val();
    let iID_DonViID = $("#iID_DonViID").val();

    // Nếu đã lấy data thì chỉ ẩn hiên thôi, chưa có data thì lấy data.
    if (!hasValue) {
        $.ajax({
            type: "POST",
            data: { id: id, sTenNhiemVuChi: sTenNhiemVuChi, iID_BQuanLyID: iID_BQuanLyID, iID_DonViID: iID_DonViID },
            url: '/QLNH/DanhMucChuongTrinh/GetListBQPNhiemVuChiById',
            success: function (rs) {
                if (rs != null) {
                    tdElement.data('ishaschild', true);

                    // Add row
                    $("#tbodyListChuongTrinh tr").eq(index).after(rs.datas);
                    trElement.siblings('.child-' + id).fadeToggle();
                }
            }
        });
    } else {
        trElement.siblings('.child-' + id).fadeToggle();
    }
}

// Tìm kiếm
function GetListData(currentPage = 1) {
    _paging.CurrentPage = currentPage;
    var filter = {
        sTenNhiemVuChi: $.trim($("#txtTenNhiemVuChi").val()),
        iID_BQuanLyID: $("#iID_BQuanLyID").val(),
        iID_DonViID: $("#iID_DonViID").val()
    }

    $.ajax({
        type: "POST",
        url: "/QLNH/DanhMucChuongTrinh/TimKiem",
        data: { input: filter, paging: _paging },
        success: function (data) {
            // View result
            $("#lstDataView").html(data);

            // Gán lại data cho filter
            $("#txtTenNhiemVuChi").val($("<div/>").html(filter.sTenNhiemVuChi).text());
            $("#iID_BQuanLyID").val(filter.iID_BQuanLyID);
            $("#iID_DonViID").val(filter.iID_DonViID);
        }
    });
}

function ResetChangePage() {
    $("#txtTenNhiemVuChi").val("");
    $("#iID_BQuanLyID").val(GUID_EMPTY);
    $("#iID_DonViID").val(GUID_EMPTY);

    GetListData();
}