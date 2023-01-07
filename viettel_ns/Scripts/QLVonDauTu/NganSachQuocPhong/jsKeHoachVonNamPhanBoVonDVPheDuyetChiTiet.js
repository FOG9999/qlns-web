var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var ERROR = 1;
var listKHVonNamDuocDuyetChiTiet = [];

function BangDuLieu_onKeypress_Delete(h, c) {
    
}

function BangDuLieu_XoaHang(cs) {
    if (cs != null && 0 <= cs && cs < Bang_nH) {
        Bang_arrHangDaXoa[cs] = !Bang_arrHangDaXoa[cs];
        Bang_HienThiDuLieu();
        return true;
    }

    return false;
}

function KHVonNamDD_ChiTiet_BangDuLieu_Save() {
    ValidateBeforeSave();
}

function ValidateBeforeSave() {
    var listItem = [];

    for (var i = 0; i < Bang_arrThayDoi.length; i++) {
        var object = {
            Id: "",
            iID_PhanBoVon_DonVi_PheDuyet_ID: "",
            iID_DuAnID: "",
            fGiaTriPhanBo: 0,
            fGiaTriThuHoi: 0,
            iID_DonViTienTeID: "",
            iID_TienTeID: 0,
            fTiGiaDonVi: 0,
            fTiGia: "",
            iID_LoaiCongTrinh: "",
            iId_Parent: "",
            bActive: "",
            ILoaiDuAn: "",
            sGhiChu: "",
            fGiaTriPhanBoDC: 0,
            iID_DuAn_HangMucID: ""
        };
        // gán giá trị mới cho kế hoạch vđt được duyệt chi tiết
        object.iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID"]];
        object.iID_PhanBoVon_DonVi_PheDuyet_ID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_PhanBoVon_DonVi_PheDuyet_ID"]];
        object.iID_DuAnID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_DuAnID"]];
        object.sMaDuAn = Bang_arrGiaTri[i][Bang_arrCSMaCot["sMaDuAn"]];
        object.sTenDuAn = Bang_arrGiaTri[i][Bang_arrCSMaCot["sTenDuAn"]];
        object.fGiaTriPhanBo = Bang_arrGiaTri[i][Bang_arrCSMaCot["fGiaTriPhanBo"]];
        object.fGiaTriThuHoi = Bang_arrGiaTri[i][Bang_arrCSMaCot['fGiaTriThuHoi']];
        object.iID_DonViTienTeID = Bang_arrGiaTri[i][Bang_arrCSMaCot['iID_DonViTienTeID']];
        object.iID_TienTeID = Bang_arrGiaTri[i][Bang_arrCSMaCot['iID_TienTeID']];
        object.fTiGiaDonVi = Bang_arrGiaTri[i][Bang_arrCSMaCot['fTiGiaDonVi']];
        object.fTiGia = Bang_arrGiaTri[i][Bang_arrCSMaCot['fTiGia']];
        object.iID_LoaiCongTrinh = Bang_arrGiaTri[i][Bang_arrCSMaCot['iID_LoaiCongTrinh']];
        object.iId_Parent = Bang_arrGiaTri[i][Bang_arrCSMaCot['iId_Parent']];
        //object.bActive = Bang_arrGiaTri[i][Bang_arrCSMaCot['bActive']];
        object.bActive = true;
        object.ILoaiDuAn = Bang_arrGiaTri[i][Bang_arrCSMaCot["sLoaiDuAn"]] == "Mở mới" ? 1 : 2;
        object.sGhiChu = Bang_arrGiaTri[i][Bang_arrCSMaCot["sGhiChu"]];
        object.iID_DonViQuanLyID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_DonViQuanLyID"]];
        object.fGiaTriDeNghi = Bang_arrGiaTri[i][Bang_arrCSMaCot["fGiaTriDeNghi"]];
        object.fGiaTriPhanBoDC = Bang_arrGiaTri[i][Bang_arrCSMaCot["fGiaTriPhanBoDC"]];
        object.iID_Parent = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_Parent"]];
        object.iID_DuAn_HangMucID = Bang_arrGiaTri[i][Bang_arrCSMaCot["iID_DuAn_HangMucID"]];

        listItem.push(object);
    }

    AjaxRequire(listItem);
}

function AjaxRequire(listItem) {
    var iIdKeHoachVonNamDXId = window.parent.document.getElementById("iID_KeHoachVonNamDeXuatIDTH").value;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/KeHoachVonNamPhanBoVonDVPheDuyet/ValidateBeforeSave",
        data: { aListModel: listItem, iID_KeHoachVonNamDeXuatID: iIdKeHoachVonNamDXId },
        async: false,
        success: function (data) {
            if (data.status == false) {
                var Title = "Lỗi lưu kế hoạch vốn năm được duyệt chi tiết"
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: data.listErrMess, Category: 1 },
                    async: false,
                    success: function (data) {
                        window.parent.loadModal(data);
                        return false;
                    }
                });
            } else {
                sessionStorage.removeItem("IIdParent");
                Bang_HamTruocKhiKetThuc();
            }
        }
    });
}

function BangDuLieu_onKeypress_F9(h, c) {
    $(".sheet-search input.input-search").val("");
    $(".sheet-search input.input-search").filter(":visible:first").focus();
    $(".btn-mvc").click();
}

function BangDuLieu_onKeypress_F10() {
    KHVonNamDD_ChiTiet_BangDuLieu_Save();
}

function BangDuLieu_onKeyPress_Enter() { }

function BangDuLieu_onKeypress_F2(h, c) {
    if (BangDuLieu_DuocSuaChiTiet && h < Bang_nH - 1) {
        BangDuLieu_ThemHangMoi(h + 1, h);
        //Gán các giá trị của hàng mới thêm =0
        //var arrTruongTien = "rSoTien".split(',');
        //for (var i = 0; i < arrTruongTien.length; i++) {
        //    var cs = Bang_arrCSMaCot[arrTruongTien[i]];
        //    Bang_GanGiaTriThatChoO(h + 1, cs, 0);
        //}

        //Gán các giá trị của hàng mới thêm = ""
        //var arrTruongChu = "sLNS,sL,sK,sM,sTM,sTTM,sNG,sTNG,sMoTa,iID_MaMucLucNganSach,sXauNoiMa,iNgay,iThang,iID_MaPhongBan,sTenPhongBan,iID_MaDonVi,sTenDonVi,iID_MaTinhChatCapThu,sTinhChatCapThu,sNoiDung".split(',');
        //for (var i = 0; i < arrTruongChu.length; i++) {
        //    var cs = Bang_arrCSMaCot[arrTruongChu[i]];
        //    Bang_GanGiaTriThatChoO(h + 1, cs, '');
        //}
        BangDuLieu_TinhTongCacHang();
    }
}

function BangDuLieu_ThemHangMoi(h, hGiaTri) {
    var csH = 0;
    if (h != null) {
        //Thêm 1 hàng mới vào hàng h
        csH = h;
    }
    else {
        //Thêm 1 hàng mới vào cuối bảng
        csH = Bang_nH;
        Bang_ThemHang(csH, hGiaTri);
    }
    Bang_ThemHang(csH, hGiaTri);
    //Sua MaHang="": Day la hang them moi
    Bang_arrMaHang[csH] = "";
    Bang_keys.fnSetFocus(csH, 0);
    BangDuLieu_TinhTongCacHang();
}

function Bang_onKeypress_F(strKeyEvent) {
    var h = Bang_keys.Row();
    var c = Bang_keys.Col() - Bang_nC_Fixed;
    var TenHam = Bang_ID + '_onKeypress_' + strKeyEvent;
    var fn = window[TenHam];
    if (typeof fn == 'function') {
        if (fn(h, c) == false) {
            return false;
        }
    }
}

function setFontWeight() {
    for (var j = 0; j < Bang_nH; j++) {
        var numChild = Bang_LayGiaTri(j, "numChild");
        if (numChild > 0) {
            Bang_GanGiaTriThatChoO_colName(j, "sFontBold", "bold");
            Bang_GanGiaTriThatChoO_colName(j, "sMauSac", "whitesmoke");
        }
    }
}

function BangDuLieu_onCellBeforeEdit(h, c) {
    console.log("BangDuLieu_onCellBeforeEdit");
}


function BangDuLieu_onKeypress_F2(h, c) {
    if (c == undefined) {
        BangDuLieu_ThemHangMoi(h, c);
    } else {
        BangDuLieu_ThemHangMoi(h + 1, h);
    }
}

function BangDuLieu_ThemHangMoi(h, hGiaTri) {
    var csH = 0;
    if (h != null) {
        //Thêm 1 hàng mới vào hàng h
        csH = h;
    }
    else {
        //Thêm 1 hàng mới vào cuối bảng
        csH = Bang_nH;
    }

    //TH là row cha thì cho copy như bình thường
    if (Bang_arrLaHangCha[hGiaTri] == true) {
        Bang_arrLaHangCha[hGiaTri] = false
        Bang_ThemHang(csH, hGiaTri);
        Bang_arrLaHangCha[hGiaTri] = true
    } else {
        Bang_ThemHang(csH, hGiaTri);
    }
    Bang_GanGiaTriThatChoO_colName(csH, "sFontBold", "");
    //Set ID dòng mới 
    var newId = uuidv4();

    if (hGiaTri != undefined) {
        var STTGiaTri = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["sSTT"]];
        var sTenGiaTri = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["sTen"]];
        var sLevelParent = Bang_arrGiaTri[hGiaTri][Bang_arrCSMaCot["iLevel"]];
        if (sLevelParent == "1") {
            Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sDuAnCha"], STTGiaTri + " - " + sTenGiaTri);
        } else {
            Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sDuAnCha"], "");
            for (var c = 0; c < Bang_nC; c++) {
                Bang_GanGiaTriThatChoO(h, c, "");
            }
            Bang_arrCSCha[h] = -1;
        }
    }

    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iLevel"], 2);
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["iID_KeHoach5Nam_DeXuat_ChiTietID"], "");
    Bang_GanGiaTriThatChoO(h, Bang_arrCSMaCot["sSTT"], "");

    //Sua MaHang="": Day la hang them moi
    Bang_arrMaHang[csH] = "";

    var itemParent = $("#ModelParentId").val();

    var c;
    for (c = 0; c < Bang_nC; c++) {
        if (itemParent != '' && itemParent != GUID_EMPTY) {
            if (c > 0 && c <= 23 && c != 9 && c != 10 && c != 11 && c != 13 && c != 15 && c != 17 && c != 19 && c != 21 && c != 22) {
                Bang_arrEdit[csH][c] = true;
            } else {
                Bang_arrEdit[csH][c] = false;
            }
        }
        else {
            if (c > 0 && c <= 23 && c != 9 && c != 10 && c != 16) {
                Bang_arrEdit[csH][c] = true;
            } else {
                Bang_arrEdit[csH][c] = false;
            }
        }
    }
    Bang_keys.fnSetFocus(csH, 1);
    Bang_GanGiaTriThatChoO_colName(h, "sMauSac", "#ffffff");
}

function BangDuLieu_HamTruocKhiKetThuc() {
    // clean listKHVonNamDuocDuyetChiTiet before save
    listKHVonNamDuocDuyetChiTiet = [];
    // tạo ra mảng các kh vốn được duyệt chi tiết từ bảng, để lưu lại
    Bang_arrGiaTri.forEach((row, rowIndex) => {
        let khVonDonViDuocDuyetChiTiet = {};
        khVonDonViDuocDuyetChiTiet.fGiaTriPhanBo = row[Bang_arrCSMaCot['fKHPhanBoNam']];
        khVonDonViDuocDuyetChiTiet.sGhiChu = row[Bang_arrCSMaCot['sGhiChu']];
        khVonDonViDuocDuyetChiTiet.iID_KeHoachVonNam_DuocDuyet_ChiTietID = row[Bang_arrCSMaCot['iID_KeHoachVonNam_DuocDuyet_ChiTietID']];
        khVonDonViDuocDuyetChiTiet.fGiaTriThuHoi = row[Bang_arrCSMaCot['fGiaTriThuHoi']];
        khVonDonViDuocDuyetChiTiet.iID_DonViTienTeID = row[Bang_arrCSMaCot['iID_DonViTienTeID']];
        khVonDonViDuocDuyetChiTiet.iID_TienTeID = row[Bang_arrCSMaCot['iID_TienTeID']];
        khVonDonViDuocDuyetChiTiet.fTiGia = row[Bang_arrCSMaCot['fTiGia']];
        khVonDonViDuocDuyetChiTiet.iID_LoaiCongTrinh = row[Bang_arrCSMaCot['iID_LoaiCongTrinh']];
        khVonDonViDuocDuyetChiTiet.iId_Parent = row[Bang_arrCSMaCot['iId_Parent']];
        khVonDonViDuocDuyetChiTiet.bActive = row[Bang_arrCSMaCot['bActive']];
        khVonDonViDuocDuyetChiTiet.ILoaiDuAn = row[Bang_arrCSMaCot['ILoaiDuAn']];
        khVonDonViDuocDuyetChiTiet.iID_DuAnID = row[Bang_arrCSMaCot['iID_DuAnID']];
        khVonDonViDuocDuyetChiTiet.iID_PhanBoVon_DonVi_PheDuyet_ID = row[Bang_arrCSMaCot['iID_PhanBoVon_DonVi_PheDuyet_ID']];
        khVonDonViDuocDuyetChiTiet.Id = row[Bang_arrCSMaCot['Id']];
        listKHVonNamDuocDuyetChiTiet.push(khVonDonViDuocDuyetChiTiet);
    })
    return true;
}
