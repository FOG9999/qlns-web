function SearchData(iCurrentPage = 1) {
    var maChiPhi = $("#txtMaChiPhi").val();
    var tenChiPhi = $("#txtTenChiPhi").val();
    GetListData(maChiPhi, tenChiPhi, iCurrentPage);
}

function GetListData(maChiPhi, tenChiPhi, iCurrentPage) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { maChiPhi: maChiPhi, tenChiPhi: tenChiPhi, _paging: _paging },
        success: function (data) {
            $("#lstDataView").html(data);
        }
    });
}
function GetItemData(id) {
    window.location.href = "/QLVonDauTu/QLDMLoaiChiPhi/Update/" + id;
}

function BtnDetailLoaiChiPhi(id) {
    window.location.href = "/QLVonDauTu/QLDMLoaiChiPhi/Detail/" + id;
}


function SaveData() {
    var data = {};
    data.iID_ChiPhi = $("#txt_ID_ChiPhi").val();
    data.sMaChiPhi = $("#txt_MaChiPhi").val();
    data.sTenVietTat = $("#txt_TenVietTat").val();
    data.sTenChiPhi = $("#txt_TenChiPhi").val();
    data.iThuTu = $("#txt_ThuTu").val();
    data.sMoTa = $("#txt_MoTa").val();

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLDMLoaiChiPhi/LoaiChiPhiSave",
        data: { data: data },
        success: function (r) {
            if (r == "True") {
                window.location.href = "/QLVonDauTu/QLDMLoaiChiPhi/Index";
            }
        }
    });
}

function ValidateData(data) {
    var sMessError = "";

    if (data.sTenChiPhi == null || data.sTenChiPhi == "") {
        sMessError += "Tên chi phí chưa nhập !\n";
    } else if (data.sTenChiPhi.length > 300) {
        sMessError += "Tên chi phí vượt quá số kí tự !\n";
    }

    if (data.sTenVietTat.length > 100) {
        sMessError += "Tên viết tắt vượt quá số kí tự !\n";
    }

    if (data.sMaChiPhi == null || data.sMaChiPhi == "") {
        sMessError += "Mã chi phí chưa nhập !\n";
    } else if (data.sMaChiPhi.length > 50) {
        sMessError += "Mã chi phí vượt quá số kí tự !\n";
    }

    if (data.iThuTu.length > 4) {
        sMessError += "Thứ tự vượt quá số kí tự !\n";
    }

    if (data.sMoTa.length > 500) {
        sMessError += "Tên viết tắt vượt quá số kí tự !\n";
    }

    if (sMessError != null && sMessError != "") {
        alert(sMessError);
        return false;
    }

    return true;
}

function CancelSaveData() {
    window.location.href = "/QLVonDauTu/QLDMLoaiChiPhi/Index";
}

function DeleteItem(id) {
    if (!confirm("Chấp nhận xóa bản ghi ?")) return;
    $.ajax({
        type: "POST",
        url: "/QLDMLoaiChiPhi/LoaiChiPhiDelete",
        data: { id: id },
        success: function (r) {
            if (r == "True") {
                SearchData();
            }
            else {

            }
        }
    });
}

function BtnInsertDataClick() {
    window.location.href = "/QLVonDauTu/QLDMLoaiChiPhi/Update/";
}
