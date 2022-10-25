var BangDuLieu_CoCotDuyet = false;
var BangDuLieu_CoCotTongSo = true;
var ERROR = 1;

function BangDuLieu_onCellAfterEdit(h, c) {
    var isMap = Bang_arrGiaTri[h][c];
    var IdReference = Bang_arrGiaTri[h][Bang_arrCSMaCot["iIDReference"]];
    var Level = Bang_arrGiaTri[h][Bang_arrCSMaCot["iLevel"]];
    var IdKH5NDXChiTiet = Bang_arrGiaTri[h][Bang_arrCSMaCot["iID_KeHoach5Nam_DeXuat_ChiTietID"]];
    var IdParent = Bang_arrGiaTri[h][Bang_arrCSMaCot["iID_ParentID"]];
    var LaHangCha = Bang_arrLaHangCha[h];
    //if (Level != 1 && Level != 2) {
    //    Bang_GanGiaTriThatChoO(h, c, !isMap);
    //}
    //checkedChiTietDuAn(isMap, IdReference);
    //if (Level == 2) {
    //    checkedChiTietDuAn(isMap, IdReference);
    //} else {
    //    if (isMap == 1) {
    //        checkedHDuAn(IdReference);
    //    } else {
    //        uncheckedHDuAn(IdReference);
    //    }
    //}

    var tickArr = GetTickTree();
    var maxLevel = Math.max.apply(Math, tickArr.map(function (o) { return parseInt(o.Level); }));
    handleTickTree(isMap, IdReference, IdKH5NDXChiTiet, IdParent, tickArr, parseInt(Level), maxLevel, LaHangCha);

    Bang_arrThayDoi[h][Bang_arrCSMaCot["iLevel"]] = true;
    Bang_arrThayDoi[h][Bang_arrCSMaCot["iIDReference"]] = true;
    Bang_arrThayDoi[h][Bang_arrCSMaCot["iID_ParentID"]] = true;
    
    return true;
}

function handleTickTree(isMap, IdReference, IdKH5NDXChiTiet, IdParent, tickArr, Level, maxLevel, LaHangCha) {
    var ticks = [], filterArr = [];
    var i, countTick = 0, countTickChecked = 0;
    if (((Level + 1) <= maxLevel) && LaHangCha) {
        for (i = (Level + 1); i <= maxLevel; i++) {
            if (i == 2) {
                filterArr = tickArr.filter(x => x.Level == i && x.iID_ParentID == IdKH5NDXChiTiet);
                if (filterArr.length > 0) ticks = ticks.concat(filterArr);
            } else {
                if (Level == 1) {
                    $.each(filterArr, function (index, item) {
                        var tempArr = tickArr.filter(x => x.Level == i && x.IdReference == item.IdReference);
                        ticks = ticks.concat(tempArr);
                    });
                } else {
                    filterArr = tickArr.filter(x => x.Level == i && x.IdReference == IdReference);
                    if (filterArr.length > 0) ticks = ticks.concat(filterArr);
                }
            }
        }
        $.each(ticks, function (index, item) {
            tickArr.filter(x => x.Hang == item.Hang)[0].IsMap = isMap;
            Bang_GanGiaTriThatChoO(item.Hang, 0, isMap);
            Bang_arrThayDoi[item.Hang][Bang_arrCSMaCot["iLevel"]] = true;
            Bang_arrThayDoi[item.Hang][Bang_arrCSMaCot["iIDReference"]] = true;
            Bang_arrThayDoi[item.Hang][Bang_arrCSMaCot["iID_ParentID"]] = true;
        });
    }
    ticks = [], filterArr = [];
    if ((Level - 1) > 0) {
        for (i = (Level - 1); i >= 1; i--) {
            if (i == 1) {
                if (Level >= 3) {
                    var tempArr = [];
                    $.each(filterArr, function (index, item) {
                        countTick = tickArr.filter(x => x.Level == (i + 1) && x.iID_ParentID == item.iID_ParentID).length;
                        countTickChecked = tickArr.filter(x => x.Level == (i + 1) && x.iID_ParentID == item.iID_ParentID && x.IsMap == 1).length;
                        tempArr = tempArr.concat(tickArr.filter(x => x.Level == i && x.iID_KeHoach5Nam_DeXuat_ChiTietID == item.iID_ParentID));
                    });
                    filterArr = tempArr;
                } else {
                    countTick = tickArr.filter(x => x.Level == (i + 1) && x.iID_ParentID == IdParent).length;
                    countTickChecked = tickArr.filter(x => x.Level == (i + 1) && x.iID_ParentID == IdParent && x.IsMap == 1).length;
                    filterArr = tickArr.filter(x => x.Level == i && x.iID_KeHoach5Nam_DeXuat_ChiTietID == IdParent);
                }
            } else {
                countTick = tickArr.filter(x => x.Level == (i + 1) && x.IdReference == IdReference).length;
                countTickChecked = tickArr.filter(x => x.Level == (i + 1) && x.IdReference == IdReference && x.IsMap == 1).length;
                filterArr = tickArr.filter(x => x.Level == i && x.IdReference == IdReference);
            }

            $.each(filterArr, function (index, item) {
                tickArr.filter(x => x.Hang == item.Hang)[0].IsMap = (countTick == countTickChecked) ? "1" : "0";
                Bang_GanGiaTriThatChoO(item.Hang, 0, (countTick == countTickChecked) ? "1" : "0");
                Bang_arrThayDoi[item.Hang][Bang_arrCSMaCot["iLevel"]] = true;
                Bang_arrThayDoi[item.Hang][Bang_arrCSMaCot["iIDReference"]] = true;
                Bang_arrThayDoi[item.Hang][Bang_arrCSMaCot["iID_ParentID"]] = true;
            });
        }
    }
}

function GetTickTree() {
    var tickArr = [];
    for (var h = 0; h < Bang_nH; h++) {
        var obj = {};
        obj.Hang = h;
        obj.Level = parseInt(Bang_arrGiaTri[h][Bang_arrCSMaCot["iLevel"]]);
        obj.IsMap = Bang_arrGiaTri[h][Bang_arrCSMaCot["isMap"]];
        obj.IdReference = Bang_arrGiaTri[h][Bang_arrCSMaCot["iIDReference"]];
        obj.iID_KeHoach5Nam_DeXuat_ChiTietID = Bang_arrGiaTri[h][Bang_arrCSMaCot["iID_KeHoach5Nam_DeXuat_ChiTietID"]];
        obj.iID_ParentID = Bang_arrGiaTri[h][Bang_arrCSMaCot["iID_ParentID"]];
        tickArr.push(obj);
    }
    return tickArr;
}

function checkedChiTietDuAn(isMap, IdReference) {
    for (var h = 0; h < Bang_nH; h++) {
        var IdReferenceChiTiet = Bang_arrGiaTri[h][Bang_arrCSMaCot["iIDReference"]];
        var LevelChiTiet = Bang_arrGiaTri[h][Bang_arrCSMaCot["iLevel"]];
        if (LevelChiTiet == 3 && IdReferenceChiTiet == IdReference) {
            Bang_GanGiaTriThatChoO(h, 0, isMap);
            Bang_arrThayDoi[h][Bang_arrCSMaCot["iLevel"]] = true;
            Bang_arrThayDoi[h][Bang_arrCSMaCot["iIDReference"]] = true;
            Bang_arrThayDoi[h][Bang_arrCSMaCot["iID_ParentID"]] = true;
        }
    }
}

function checkedHDuAn(IdReference) {
    for (var h = 0; h < Bang_nH; h++) {
        var LaHangCha = Bang_arrLaHangCha[h];
        var IdReferenceParent = Bang_arrGiaTri[h][Bang_arrCSMaCot["iIDReference"]];
        var LevelParent = Bang_arrGiaTri[h][Bang_arrCSMaCot["iLevel"]];
        if (LaHangCha && LevelParent == 2 && IdReferenceParent == IdReference) {
            Bang_GanGiaTriThatChoO(h, 0, 1);
            Bang_arrThayDoi[h][Bang_arrCSMaCot["iLevel"]] = true;
            Bang_arrThayDoi[h][Bang_arrCSMaCot["iIDReference"]] = true;
            Bang_arrThayDoi[h][Bang_arrCSMaCot["iID_ParentID"]] = true;
            return;
        }
    }
}

function uncheckedHDuAn(IdReference) {
    var count = 0;
    for (var h = 0; h < Bang_nH; h++) {
        var IdReferenceCheck = Bang_arrGiaTri[h][Bang_arrCSMaCot["iIDReference"]];
        var Level = Bang_arrGiaTri[h][Bang_arrCSMaCot["iLevel"]];
        var isMap = Bang_arrGiaTri[h][Bang_arrCSMaCot["isMap"]];
        if (Level == 3 && IdReferenceCheck == IdReference && isMap == 1) {
            count++;
            break;
        }
    }

    for (var h = 0; h < Bang_nH; h++) {
        var LaHangCha = Bang_arrLaHangCha[h];
        var IdReferenceParent = Bang_arrGiaTri[h][Bang_arrCSMaCot["iIDReference"]];
        var LevelParent = Bang_arrGiaTri[h][Bang_arrCSMaCot["iLevel"]];
        if (LaHangCha && LevelParent == 2 && IdReferenceParent == IdReference) {
            if (count == 0) {
                Bang_GanGiaTriThatChoO(h, 0, 0);
            } else {
                Bang_GanGiaTriThatChoO(h, 0, 1);
                Bang_arrThayDoi[h][Bang_arrCSMaCot["iLevel"]] = true;
                Bang_arrThayDoi[h][Bang_arrCSMaCot["iIDReference"]] = true;
                Bang_arrThayDoi[h][Bang_arrCSMaCot["iID_ParentID"]] = true;
            }
            return;    
        }
    }
}

function BangDuLieu_onKeypress_F2_NoRow(h, c) {
    var yes = confirm("F2: Đ/c chắc chắn muốn chọn toàn bộ dự án hiện tại?");
    if (!yes)
        return;

    CheckAll(1);
}

function BangDuLieu_onKeypress_F3_NoRow(h, c) {
    var yes = confirm("F3: Đ/c chắc chắn muốn bỏ chọn toàn bộ dự án hiện tại?");
    if (!yes)
        return;

    CheckAll(0);
}

function CheckAll(cs) {
    for (var i = 0; i < Bang_nH; i++) {
        Bang_GanGiaTriThatChoO_colName(i, "isMap", cs);
        Bang_arrThayDoi[i][Bang_arrCSMaCot["iLevel"]] = true;
        Bang_arrThayDoi[i][Bang_arrCSMaCot["iIDReference"]] = true;
        Bang_arrThayDoi[i][Bang_arrCSMaCot["iID_ParentID"]] = true;
    }
}

function KH5NamDX_ChiTiet_BangDuLieu_Save() {
    var isAddDxProject = true;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/DuAnKeHoachTrungHan/setActionAddDxProject",
        data: { isAddDxProject: isAddDxProject },
        async: false,
        success: function (data) {
            if (data.status == false) {
                var Title = "Lỗi chọn dự án cho kế hoạch trung hạn được duyệt"
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
    //DMNoiDungChi_BangDuLieu_Save();
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
    //if (strKeyEvent == "F10") {
    //    //Bang_HamTruocKhiKetThuc();
    //}
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