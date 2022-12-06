$(document).ready(function () {
    GetListData();
});

function CancelSaveData() {
    window.location.href = "/QLVonDauTu/VDTDMLoaiCongTrinh/Index";
}

function BtnInsertDataClick() {
    window.location.href = "/QLVonDauTu/VDTDMLoaiCongTrinh/Update/";
}

function SaveData() {
    var data = {};
    data.iID_LoaiCongTrinh = $("#txt_IdLoaiCongTrinh").val();
    data.iID_Parent = $("#txt_ParentId").val();
    data.sMaLoaiCongTrinh = $("#txt_MaLoaiCongTrinh").val();
    data.sTenVietTat = $("#txt_TenVietTat").val();
    data.sTenLoaiCongTrinh = $("#txt_TenLoaiCongTrinh").val();
    data.sMoTa = $("#txt_MoTa").val();
    data.iThuTu = $("#txt_ThuTu").val();

    if (!ValidateData(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/VDTDMLoaiCongTrinh/DMLoaiCongTrinhSave",
        data: { data: data },
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/QLVonDauTu/VDTDMLoaiCongTrinh/Index";
            } else {
                alert(r.sMessError);
            }
        }
    });
}

function DeleteItem(id) {
    if (!confirm("Chấp nhận xóa bản ghi ?")) return;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/VDTDMLoaiCongTrinh/DMLoaiCongTrinhDelete",
        data: { id: id },
        success: function (r) {
            alert(r.sMessError);
            if (r.bIsComplete) {
                GetListData();
            }
        }
    });
}

function ViewDetail(id) {
    window.location.href = "/QLVonDauTu/VDTDMLoaiCongTrinh/Detail/"+id;
}

function ValidateData(data) {
    var sMessError = "";

    if (data.sMaLoaiCongTrinh == null || data.sMaLoaiCongTrinh == "") {
        sMessError += "Mã loại công trình chưa nhập !\n";
    } else if (data.sMaLoaiCongTrinh.length > 100) {
        sMessError += "Mã loại công trình vượt quá số kí tự !\n";
    }

    if (data.sTenLoaiCongTrinh == null || data.sTenLoaiCongTrinh == "") {
        sMessError += "Tên loại công trình chưa nhập !\n";
    } else if (data.sTenLoaiCongTrinh.length > 600) {
        sMessError += "Tên loại công trình vượt quá số kí tự !\n";
    }

    if (data.sTenVietTat == null || data.sTenVietTat == "") {
        sMessError += "Tên viết tắt chưa nhập !\n";
    } else if (data.sTenVietTat.length > 200) {
        sMessError += "Tên viết tắt vượt quá số kí tự !\n";
    }

    if (sMessError != null && sMessError != "") {
        alert(sMessError);
        return false;
    }

    return true;
}

function GetListData() {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/VDTDMLoaiCongTrinh/GetListLoaiCongTrinhInPartial",
        success: function (r) {
            var columns = [
                { sField: "iID_LoaiCongTrinh", bKey: true },
                { sField: "iID_Parent", bParentKey: true },

                { sTitle: "Tên loại công trình", sField: "sMaLoaiCongTrinh-sTenLoaiCongTrinh", iWidth: "75%", sTextAlign: "left", bHaveIcon: 1 },
                { sTitle: "Thứ tự", sField: "iThuTu", iWidth: "10%", sTextAlign: "right" }];
            var button = { bUpdate: 1, bDelete: 1, bInfo: 1 };
            var sHtml = GenerateTreeTable(r.data, columns, button)

            $("#ViewTable").html(sHtml);
            filter();
        }
    });
}

function GetItemData(id) {
    window.location.href = "/QLVonDauTu/VDTDMLoaiCongTrinh/Update/" + id;
}