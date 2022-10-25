$(".currently").trigger("change");
var arrNamKeHoach = [];
var arrDonvi = [];
var CONFIRM = 0;
var ERROR = 1;
var arrNguonVon = [];
var arrDonVi = [];
var arrThanhToan = [];
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var isShowSearchDMLoaiCongTrinh = true;
var bIsViewDetail = 0;

//************** Index **************//
$(document).ready(function ($) {
    bIsViewDetail = $("#bIsViewDetail").val();
    LoadDataNamKeHoach();
    LoadDataThanhToan();
    LoadDataDonvi();
    LoadDataNguonVon();
});

function ChangePage(iCurrentPage = 1) {
    var txtSoChungTu = $.trim($("#txtSoChungTu").val());
    var drpDonVi = $("#drpDonViQuanLy option:selected").val();
    var iIdNguonVon = $("#drpNguonVon option:selected").val();
    var dNgayDeNghiFrom = $("#txtNgayLapFrom").val();
    var dNgayDeNghiTo = $("#txtNgayLapTo").val();
    var iNamKeHoach = $("#txtNamKeHoach").val();
    var sMaDonVi = "";
    var iIdDonVi = ""

    if (drpDonVi != undefined && drpDonVi != null && drpDonVi != "") {
        iIdDonVi = drpDonVi.split("|")[0];
        sMaDonVi = drpDonVi.split("|")[1];
    }
    //ChangeData();
    if ($('input[name=groupVoucher]:checked').val() == 1) {
        GetListDataTongHop();
    } else {
        GetListData(null, sMaDonVi, iIdNguonVon, dNgayDeNghiFrom, dNgayDeNghiTo, iNamKeHoach, iCurrentPage, drpDonVi, txtSoChungTu);
    }

}

function GetListDataTongHop() {
    var sSoDeNghi = $("#txtSodenghiList").val();
    var dNgayDeNghi = $("#txtNgaydenghiList").val();
    var iNamKeHoach = $(".selectNamKeHoachList select").val();
    var iLoaiThanhToan = $(".selectLoaiThanhToanList select").val();
    var iDonVi = $(".selectDonViList select").val();
    var sTenNguonVon = $("#selectNguonVonList select").val();

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/BcQuyetToanNienDo/GetListTongHopQuyetToan",
        data: { sSoDeNghi: sSoDeNghi, dNgayDeNghi: dNgayDeNghi, iNamKeHoach: iNamKeHoach, iLoaiThanhToan: iLoaiThanhToan, iDonVi: iDonVi, sTenNguonVon: sTenNguonVon },
        success: function (r) {
            var columns = [
                { sField: "iID_BCQuyetToanNienDoID", bKey: true },
                { sField: "iID_TongHopID", bParentKey: true },
                { sTitle: "Số đề nghị", sField: "sSoDeNghi", iWidth: "15%", sTextAlign: "left", bHaveIcon: 1 },
                { sTitle: "Ngày đề nghị", sField: "dNgayDeNghiStr", iWidth: "15%", sTextAlign: "center" },
                { sTitle: "Năm kế hoạch", sField: "iNamKeHoach", iWidth: "10%", sTextAlign: "center" },
                { sTitle: "Loại thanh toán", sField: "sThanhToan", iWidth: "10%", sTextAlign: "center" },
                { sTitle: "Đơn vị", sField: "sTenDonVi", iWidth: "15%", sTextAlign: "center" },
                { sTitle: "Tên nguồn vốn", sField: "sTenNguonVon", iWidth: "20%", sTextAlign: "center" },
            ];
            var button = { bUpdate: 1, bDelete: 1, bInfo: 1 };
            var sortedData = r.data.sort((a, b) => {
                if (a.iID_BCQuyetToanNienDoID < b.iID_BCQuyetToanNienDoID) {
                    return -1;
                }
                else return 1;
            })
            var sHtml = GenerateTreeTableNCCQ(sortedData, columns, button, true, false, isShowSearchDMLoaiCongTrinh)
            $("#txtSobannghi").text(r.data.length);
            $("#ViewTable").html(sHtml);
            $('.date')
                .datepicker({
                    todayBtn: "linked",
                    keyboardNavigation: false,
                    forceParse: false,
                    autoclose: true,
                    language: 'vi',
                    todayHighlight: true,
                });
            $("#txtSodenghiList").val(sSoDeNghi);
            $("#txtNgaydenghiList").val(dNgayDeNghi);
            $(".selectDonViList select").val(iDonVi);
            $(".selectNamKeHoachList select").val(iNamKeHoach);
            $(".selectLoaiThanhToanList select").val(iNamKeHoach);
            $(".selectNguonVonList select").val(sTenNguonVon);

        }
    });
}

function ViewDetailList(iIdBcQuyetToanId) {
    if (iIdBcQuyetToanId == null || iIdBcQuyetToanId == GUID_EMPTY) return;
    location.href = "/QLVonDauTu/BcQuyetToanNienDo/Update/" + iIdBcQuyetToanId + "?bIsViewDetail=1"
}

function LoadDataNamKeHoach() {
    $.ajax({
        async: false,
        url: "/QLVonDauTu/BcQuyetToanNienDo/GetListDropDownNamKeHoach",
        type: "POST",
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data.status == false) {
                return;
            }

            if (data.data != null)
                arrNamKeHoach = data.data;
        }
    });
}
function LoadDataDonvi() {
    $.ajax({
        async: false,
        url: "/QLVonDauTu/BcQuyetToanNienDo/GetDropdownDonVi",
        type: "POST",
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data.status == false) {
                return;
            }

            if (data.data != null)
                arrDonvi = data.data;
        }
    });
}
function LoadDataNguonVon() {
    $.ajax({
        async: false,
        url: "/QLVonDauTu/BcQuyetToanNienDo/GetDropdownNguonVon",
        type: "POST",
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data.status == false) {
                return;
            }
            if (data.data != null)
                arrNguonVon = data.data;
        }
    });
}
function LoadDataThanhToan() {
    $.ajax({
        async: false,
        url: "/QLVonDauTu/BcQuyetToanNienDo/GetDropdownThanhToan",
        type: "POST",
        dataType: "json",
        cache: false,
        success: function (data) {
            if (data.status == false) {
                return;
            }
            if (data.data != null)
                arrThanhToan = data.data;
        }
    });
}


function ChangeData() {
    var iIdDonViQuanLy = GUID_EMPTY;
    var iIdMaDonViQuanLy = null;
    var iIdNguonVon = null;
    var dNgayDeNghiFrom = null;
    var dNgayDeNghiTo = null;
    var iNamKeHoach = null;
    var iCurrentPage = 1;
    if ($("input[name=groupVoucher]:checked").val() == "0") {
        GetListData(iIdDonViQuanLy, iIdMaDonViQuanLy, iIdNguonVon, dNgayDeNghiFrom, dNgayDeNghiTo, iNamKeHoach, iCurrentPage);
        $("#tbListQuyetToanNienDo").css("display", "");
        $("#KeHoachVonNam").css("display", "");
        $("#ViewTable").css("display", "none");
        $("#KeHoachVonNamTongHop").css("display", "none");
        $("#Padding").css("display", "");

    } else {
        GetListDataTongHop();
        $("#tbListQuyetToanNienDo").css("display", "none");
        $("#ViewTable").css("display", "");
        $("#KeHoachVonNam").css("display", "none");
        $("#KeHoachVonNamTongHop").css("display", "");
        $("#Padding").css("display", "none");
    }
}

function GetListData(iIdDonViQuanLy, iIdMaDonViQuanLy, iIdNguonVon, dNgayDeNghiFrom, dNgayDeNghiTo, iNamKeHoach, iCurrentPage, drpDonVi, txtSoChungTu) {
    _paging.CurrentPage = iCurrentPage;
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/BcQuyetToanNienDo/QuyetToanNienDoView",
        data: {
            iIdMaDonViQuanLy: iIdMaDonViQuanLy,
            iIdNguonVon: iIdNguonVon,
            dNgayDeNghiFrom: dNgayDeNghiFrom,
            dNgayDeNghiTo: dNgayDeNghiTo,
            iNamKeHoach: iNamKeHoach,
            _paging: _paging,
            txtSoChungTu: txtSoChungTu
        },
        success: function (data) {
            $("#lstDataView").html(data);

            $("#txtSoChungTu").val(txtSoChungTu);
            $("#drpDonViQuanLy").val(drpDonVi);
            $("#drpNguonVon").val(iIdNguonVon);
            $("#txtNgayLapFrom").val(dNgayDeNghiFrom);
            $("#txtNgayLapTo").val(dNgayDeNghiTo);
            $("#txtNamKeHoach").val(iNamKeHoach);
        }
    });
}

function CreateHtmlSelectDonVi(value) {
    var htmlOption = "";
    arrDonvi.forEach(x => {
        if (value != undefined && value == x.iID_Ma)
            htmlOption += "<option value='" + x.iID_Ma + "' selected>" + x.sTen + "</option>";
        else
            htmlOption += "<option value='" + x.iID_Ma + "'>" + x.sTen + "</option>";
    })
    return "<select class='form-control'>" + htmlOption + "</option>";
}
function CreateHtmlSelectThanhToan(value) {
    var htmlOption = "";
    arrThanhToan.forEach(x => {
        if (value != undefined && value == x.valueId)
            htmlOption += "<option value='" + x.valueId + "' selected>" + x.labelName + "</option>";
        else
            htmlOption += "<option value='" + x.valueId + "'>" + x.labelName + "</option>";
    })
    return "<select class='form-control'>" + htmlOption + "</option>";
}
function CreateHtmlSelectNguonVon(value) {
    var htmlOption = "";
    arrNguonVon.forEach(x => {
        if (value != undefined && value == x.iID_MaNguonNganSach)
            htmlOption += "<option value='" + x.iID_MaNguonNganSach + "' selected>" + x.sTen + "</option>";
        else
            htmlOption += "<option value='" + x.iID_MaNguonNganSach + "'>" + x.sTen + "</option>";
    })
    return "<select class='form-control'>" + htmlOption + "</option>";
}
function CreateHtmlSelectNamKeHoach(value) {
    var htmlOption = "";
    arrNamKeHoach.forEach(x => {
        if (value != undefined && value == x.id)
            htmlOption += "<option value='" + x.id + "' selected>" + x.text + "</option>";
        else
            htmlOption += "<option value='" + x.id + "'>" + x.text + "</option>";
    })
    return "<select class='form-control'>" + htmlOption + "</option>";
}

function GenerateTreeTableNCCQ(data, columns, button, idTable, haveSttColumn = true, isShowSearchDMLoaiCongTrinh = false) {
    var sParentKey = '';
    var sKeyRefer = "";
    var sKey = ''
    var TableView = [];
    var sItemHiden = [];
    var iTotalCol = 1;
    TableView.push("<table class='table table-bordered table-parent' id='tableTongHop' style='margin-bottom: 0px'>");
    TableView.push("<thead class='header-search'>");
    TableView.push("<tr>");
    TableView.push("<th width='5%'></th>");
    TableView.push("<th width='15%'><input type='text' class='form-control clearable' placeholder='Số đề nghị' id='txtSodenghiList' autocomplete='off' /></th>");
    TableView.push("<th width='15%'><div class='input-group date'><input type='text' id='txtNgaydenghiList' class='form-control' value='' autocomplete='off' placeholder='dd/MM/yyyy' /><span class='btn-default input-group-addon input-calendar'><i class='fa fa-calendar' aria-hidden='true'></i></span></div ></th>");
    TableView.push("<th width='10%'><div class='selectNamKeHoachList'>" + CreateHtmlSelectNamKeHoach() + "</div></th>");
    TableView.push("<th width='10%'><div class='selectLoaiThanhToanList'>" + CreateHtmlSelectThanhToan() + "</div></th>");
    TableView.push("<th width='15%'><div class='selectDonViList'>" + CreateHtmlSelectDonVi() + "</div></th>");
    TableView.push("<th width='20%'><div class='selectNguonVonList'>" + CreateHtmlSelectNguonVon() + "</div></th>");
    TableView.push("<th width='10%'><button class='btn btn-info' onclick='GetListDataTongHop()'><i class='fa fa-search'></i>Tìm kiếm</button> </th>");
    TableView.push("</tr>");
    TableView.push("</thead>");
    TableView.push("<thead>");
    TableView.push("<th width='5%'></th>");

    $.each(columns, function (indexItem, value) {

        if (value.bKey) {
            sKey = value.sField;
        }

        if (value.bForeignKey) {
            sKeyRefer = value.sField;
        }

        if (value.bParentKey) {
            sParentKey = value.sField;
        }

        if (value.sTitle != null && value.sTitle != undefined && value.sTitle != '') {
            iTotalCol++;
            TableView.push("    <th width='" + value.iWidth + "'>" + value.sTitle + "</th>");
        } else {
            sItemHiden.push(value.sField);
        }
    });
    if (sKeyRefer == "") {
        sKeyRefer = sKey;
    }
    if (button.bUpdate == 1 || button.bDelete == 1 || button.bInfo == 1) {
        TableView.push("<th width='25%'>Thao tác</th>");
        iTotalCol++;
    }

    TableView.push("    </thead>");
    TableView.push("    <tbody>");

    if (data == undefined || data == null || data == []) {
        return TableView.join(" ");
    }
    var objCheck = SoftData(data, sKey, sKeyRefer, sParentKey);
    var index = 0;
    var sSpace = "";
    $.each(objCheck.result, function (indexItem, value) {
        var itemChild = $.map(data, function (n) { return n[sKey] == value ? n : null })[0];
        var dataRef = RecursiveTable(iTotalCol, index, itemChild, objCheck.parentData, columns, sItemHiden, button, sKey, sKeyRefer, data, sSpace, idTable, haveSttColumn)

        TableView.push(dataRef.sView);
        index = dataRef.index;
    });

    TableView.push("    </tbody>");
    TableView.push("</table>");
    return TableView.join(' ');
}


function RecursiveTable(iTotalCol, index, item, dicTree, columns, sItemHiden, button, sKey, sKeyRefer, data, sSpace, idTable, haveSttColumn = true) {
    index++;
    var TableView = [];
    TableView.push("<td width='5%'><input type='checkbox' name='cbQuyetToan' id='cbQuyetToan' data-id='" + item.iID_BCQuyetToanNienDoID + "' data-islock='" + item.bIsKhoa + "'/></td>");

    $.each(columns, function (indexItem, value) {
        if (value.sTitle != null && value.sTitle != undefined && value.sTitle != '') {
            if (value.sField == "iNamKeHoach") {
                TableView.push("<td align='" + value.sTextAlign + "' style='width:" + value.iWidth + "' data-getname='iNamKeHoach' data-getvalue='" + item['iNamKeHoach'] + "' class='" + (value.sClass == undefined ? "" : value.sClass) + "'>")
            } else {
                TableView.push("<td align='" + value.sTextAlign + "' style='width:" + value.iWidth + "' class='" + (value.sClass == undefined ? "" : value.sClass) + "'>")
            }

            // lay dong dau tien de hien thi icon va collape
            if (value.bHaveIcon == 1) {
                // neu co nhanh con , thi hien icon folder , neu khong co thi hien icon text
                if (dicTree[item[sKeyRefer]] != null) {
                    if (idTable != null && idTable != undefined) {
                        TableView.push(sSpace + "<i class='fa fa-caret-down' aria-hidden='true' data-toggle='collapse' data-target='#item-" + item[sKeyRefer] + "_" + idTable + "' aria-expanded='false' aria-controls='#item-" + item[sKeyRefer] + "_" + idTable + "'></i>")
                    } else {
                        TableView.push(sSpace + "<i class='fa fa-caret-down' aria-hidden='true' data-toggle='collapse' data-target='#item-" + item[sKeyRefer] + "' aria-expanded='false' aria-controls='#item-" + item[sKeyRefer] + "'></i>")
                    }

                    TableView.push("<i class='fa fa-folder-open' style='color:#ffc907'></i>")
                } else {
                    TableView.push(sSpace + "&nbsp;&nbsp;<i class='fa fa-file-text' style='color:#ffc907'></i>")
                }
                isFirst = false;
            }
            var itemText = [];
            $.each(value.sField.split('-'), function (indexField, valueField) {
                itemText.push(item[valueField]);
            });
            if (value.bHaveIcon != 1)
                TableView.push(itemText.join(" - "));
            else
                TableView.push(itemText.join(" - "));
            TableView.push("</td>");
        }
    });
    TableView.push("<td align='center' data-gettonghop='iID_TongHopID' data-gettonghopvalue='" + item.iID_TongHopID + "' hidden></td>")
    TableView.push(CreateButtonTable(item, sKey, sKeyRefer, button));
    TableView.push("</tr>");

    if (dicTree[item[sKeyRefer]] != null) {
        TableView.push("<tr>");
        if (idTable != null && idTable != undefined) {
            TableView.push("    <td colspan='" + iTotalCol + "' class='table-child collapse in' aria-expanded='true' style='padding:0px;' id='item-" + item[sKeyRefer] + "_" + idTable + "'>");
        } else {
            TableView.push("    <td colspan='" + iTotalCol + "' class='table-child collapse in' aria-expanded='true' style='padding:0px;' id='item-" + item[sKeyRefer] + "'>");
        }

        TableView.push("        <table class='table table-bordered'>");
        sSpace = sSpace + "&nbsp;&nbsp&nbsp;&nbsp"
        $.each(dicTree[item[sKeyRefer]], function (indexItem, value) {
            var itemChild = $.map(data, function (n) { return n[sKey] == value ? n : null })[0];
            var dataRef = RecursiveTable(iTotalCol, index, itemChild, dicTree, columns, sItemHiden, button, sKey, sKeyRefer, data, sSpace, idTable, haveSttColumn)

            TableView.push(dataRef.sView)
            index = dataRef.index;
        });

        TableView.push("        </table>");
        TableView.push("    </td>");
        TableView.push("</tr>");
    }
    return { sView: TableView.join(" "), index: index };
}

function CreateButtonTable(item, sKey, sKeyRefer, button) {
    var lstKey = [];
    lstKey.push("'" + item[sKey] + "'");
    if (button.bAddReferKey) {
        lstKey.push("'" + item[sKeyRefer] + "'");
    }
    var sParam = lstKey.join(",");

    var TableView = [];
    TableView.push("<td align='center' class='col-sm-12 col-btn' style='width:10%'>");
    TableView.push("<button type='button' class='btn-detail' onclick='ViewDetailsTongHop(`" + item.iID_BCQuyetToanNienDoID + "`)' data-toggle='modal' data-target='#modalQTNienDo'><i class='fa fa-eye fa-lg' aria-hidden='true'></i></button>");
    if (item.sTongHop != null && item.sTongHop != "") {
        TableView.push("<button type='button' class='btn-delete' onclick='Xoa(`" + item.iID_BCQuyetToanNienDoID + "`)' ><i class='fa fa-trash-o fa-lg' aria-hidden='true'></i></button>");
        TableView.push("<button type='button' class='btn-edit' onclick='UpdateTongHop(`" + item.iID_BCQuyetToanNienDoID + "`)' data-toggle='modal' data-target='#modalQTNienDo' > <i class='fa fa-pencil-square-o fa-lg' aria-hidden='true'></i></button>");
    }
    TableView.push("</td>")

    return TableView.join(' ');
}

function Xoa(id) {
    var Title = 'Xác nhận xóa quyết toán niên độ tổng hợp';
    var Messages = [];
    Messages.push('Bạn có chắc chắn muốn xóa?');
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

function Delete(id) {
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/BcQuyetToanNienDo/Xoa",
        data: { id: id },
        success: function (data) {
            if (data) {
                if (data.bIsComplete) {
                    ChangePage();
                } else {
                    if (data.sMessError != "") {
                        var Title = 'Lỗi xóa thông tin hợp đồng';
                        $.ajax({
                            type: "POST",
                            url: "/Modal/OpenModal",
                            data: { Title: Title, Messages: [data.sMessError], Category: ERROR },
                            success: function (res) {
                                $("#divModalConfirm").html(res);
                            }
                        });
                    }
                }
            }
        }
    });
}
function ViewDetailsTongHop(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/BcQuyetToanNienDo/ViewDetailsTongHop",
        data: { id: id },
        success: function (data) {
            $("#contentModalQTNienDo").html(data);
            $("#modalQTNienDoLabel").html('Chi tiết quyết toán niên độ  tổng hợp');
        }
    }); 
}
function UpdateTongHop(id) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/BcQuyetToanNienDo/GetModalUpdate",
        data: { id: id },
        success: function (data) {
            $("#contentModalQTNienDo").html(data);
            $("#modalQTNienDoLabel").html('Sửa quyết toán niên độ  tổng hợp');
        }
    });
}
function SaveTongHop(listId) {
    var data = GetDataTongHop();

    if (!ValidateData(data, true)) {
        return false;
    }
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/BcQuyetToanNienDo/SaveTongHop",
        data: { data: data, listId: listId },
        success: function (r) {
            if (r && r.bIsComplete) {
                window.location.href = "/QLVonDauTu/BcQuyetToanNienDo";
            } else {
                var Title = 'Lỗi lưu thông tin quyết toán niên độ';
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
function SaveQTNDTongHop(NamKeHoach) {
    var data = {};
    data.iID_BCQuyetToanNienDoID = $("#hidQTNienDoID").val();
    data.sTongHop = $("#IDTongHop").val();
    data.sSoDeNghi = $("<div/>").text($.trim($("#txtSoDeNghi").val())).html();
    data.dNgayDeNghi = $("<div/>").text($.trim($("#txtNgayDeNghi").val())).html();
    data.iNamKeHoach = NamKeHoach;
     console.log(data)
   
    if (!ValidateDatatonghop(data)) {
        return false;
    }
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/BcQuyetToanNienDo/QuyetToanNienDoTongHopSave",
        data: { data: data },
        success: function (r) {
            if (r.bIsComplete) {
                window.location.href = "/QLVonDauTu/BcQuyetToanNienDo";
            } else {
                var Title = 'Lỗi lưu quyết toán niên độ tổng hợp';
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
function TongHopModal() {
    //get all id row checkbox
    var returnError = 0;
    var listId = [];
    var listNamKeHoach = [];
    var listThanhToan = [];
    var listTongHop = [];
    var listNguonVon = [];
    var setTable;
    if ($('input[name=groupVoucher]:checked').val() == 1) {
        setTable = $("#tableTongHop");
    } else {
        setTable = $("#tbListQuyetToanNienDo");
    }
    setTable.find('tr').each(function () {
        if ($(this).find('input[type="checkbox"]').is(':checked')) {
            $(this).find("td").each(function (index) {
                const fieldName = $(this).data("getname");
                if (fieldName !== undefined) {
                    const fieldValue = $(this).data("getvalue");
                    if (listNamKeHoach.indexOf(fieldValue) === -1) {
                        listNamKeHoach.push(fieldValue);
                    }
                }
                const fieldTongHopName = $(this).data("gettonghop");
                if (fieldTongHopName !== undefined) {
                    const fieldValue = $(this).data("gettonghopvalue");
                    if (fieldValue != '' && fieldValue != null) {
                        listTongHop.push(fieldValue);
                    }
                }
                const fieldThanhToanName = $(this).data("getthanhtoan");
                if (fieldThanhToanName !== undefined) {
                    const fieldValue = $(this).data("getthanhtoanvalue");
                    if (listThanhToan.indexOf(fieldValue) === -1) {
                        listThanhToan.push(fieldValue);
                    }
                }
                const fieldNguonVonName = $(this).data("getnguonvon");
                if (fieldNguonVonName !== undefined) {
                    const fieldValue = $(this).data("getnguonvonvalue");
                    if (listNguonVon.indexOf(fieldValue) === -1) {
                        listNguonVon.push(fieldValue);
                    }
                }
            });
            var id = $(this).find('input[type="checkbox"]').data("id")
            var lock = $(this).find('input[type="checkbox"]').data("islock")
            if (lock == false || lock == "False") {
                returnError++;
            } else {
                if (listId.indexOf(id) === -1) {
                    listId.push(id);
                }
            }
        }
    });
    //alert error
    if (returnError > 0) {
        var Title = 'Vui lòng khóa quyết toán niên độ';
        var Error = "Bạn phải khóa quyết toán niên độ mới Tổng hợp!"
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: [Error], Category: ERROR },
            success: function (res) {
                $("#divModalConfirm").html(res);
            }
        });
    } else if (listThanhToan.length > 1) {
        var Title = 'Vui lòng chọn bản ghi cùng loại thanh toán';
        var Error = "Bạn phải chọn bản ghi cùng loại thanh toán mới tổng hợp!"
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: [Error], Category: ERROR },
            success: function (res) {
                $("#divModalConfirm").html(res);
            }
        });
    } else if (listNamKeHoach.length > 1) {
        var Title = 'Vui lòng chọn cùng năm kế hoạch';
        var Error = "Bạn phải chọn cùng năm kế hoạch mới Tổng hợp!"
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: [Error], Category: ERROR },
            success: function (res) {
                $("#divModalConfirm").html(res);
            }
        });
    } else if (listNguonVon.length > 1) {
        var Title = 'Vui lòng chọn cùng nguồn vốn';
        var Error = "Bạn phải chọn cùng nguồn vốn mới Tổng hợp!"
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: [Error], Category: ERROR },
            success: function (res) {
                $("#divModalConfirm").html(res);
            }
        });
    }
    else {
        //show modal tong hop
        if (listId.length > 0 && listId != null) {
            _paging.CurrentPage = 1;
            $.ajax({
                type: "POST",
                dataType: "html",
                url: "/QLVonDauTu/BcQuyetToanNienDo/GetModalTongHop",
                data: { listId: listId, _paging: _paging, namKeHoach: listNamKeHoach[0] },
                success: function (data) {
                    $("#modalQTNienDo").modal("show")
                    $("#contentModalQTNienDo").empty().html(data);
                    $("#modalQTNienDoLabel").empty().html('Tổng hợp quyết toán niên độ');
                    $('.date')
                        .datepicker({
                            todayBtn: "linked",
                            keyboardNavigation: false,
                            forceParse: false,
                            autoclose: true,
                            language: 'vi',
                            todayHighlight: true,
                        });
                }
            });
        } else {
            var Title = 'Vui lòng tích kế hoạch năm';
            var Error = "Bạn phải tích kế hoạch năm mới Tổng hợp!"
            $.ajax({
                type: "POST",
                url: "/Modal/OpenModal",
                data: { Title: Title, Messages: [Error], Category: ERROR },
                success: function (res) {
                    $("#divModalConfirm").html(res);
                }
            });
        }

    }

}

function GetDataTongHop() {
    var data = {};
    data.sSoDeNghi = $("#txtSoDeNghi").val();
    data.dNgayDeNghi = $("#txtNgayDeNghi").val();
    data.iNamKeHoach = $("#slbNamKeHoach").val() == 0 ? null : $("#slbNamKeHoach").val();
    return data;
}
function BtnInsertDataClick() {
    location.href = "/QLVonDauTu/BcQuyetToanNienDo/Update";
}

function GetItemDataList(id) {
    location.href = "/QLVonDauTu/BcQuyetToanNienDo/Update/" + id;
}

function DeleteItemList(iId) {
    if (confirm("Bạn có chắc chắn muốn xóa quyết toán này ?")) {
        $.ajax({
            url: "/QLVonDauTu/BcQuyetToanNienDo/DeleteBCQuyetToanNienDo",
            type: "GET",
            data: { iId: iId },
            success: function (data) {
                alert("Xóa thành công !");
                ChangePage();
            }
        });
    }
}

//************** Update **************//
function GetDonViQuanLy() {
    $.ajax({
        url: "/QLVonDauTu/BcQuyetToanNienDo/GetListDataDonViQuanLy",
        type: "GET",
        success: function (data) {
            if (data.results != null) {
                data.results.forEach(function (item) {
                    $("#drpDonViQuanLy").append("<option data-id='" + item.iID_Ma + "' value='" + item.iID_MaDonVi + "' >" + item.sTenLoaiDonVi + "</option>")
                });
                var iIdMaDonVi = $("#iIdMaDonVi").val();
                if (iIdMaDonVi != null && iIdMaDonVi != "") {
                    $("#drpDonViQuanLy").val(iIdMaDonVi);
                    $("#drpDonViQuanLy").change();
                }
            }
        }
    });
}

function ChangeVoucher() {
    if ($("input[name=groupVoucher]:checked").val() == "1") {
        $("#ViewTable").css("display", "");
        $("#ViewTablePhanTich").css("display", "none");
    } else {
        $("#ViewTable").css("display", "none");
        $("#ViewTablePhanTich").css("display", "");
    }
}

function ChangeLoaiThanhToan() {
    if ($("#drpNguonVon").val() != 1 || $("#drpLoaiThanhToan").val() != 1) {
        $("#grp_loaichungtu").css("display", "none");
    } else {
        $("#grp_loaichungtu").css("display", "");
    }
    $("#dxChungTu").prop("checked", true);
    ChangeVoucher();
    RenderGridView();
}

function RenderGridView() {
    var sMaDonViQuanLy = $("#drpDonViQuanLy").val();
    var iNamKeHoach = $("#txtNamKeHoach").val();
    var iIDNguonVonID = $("#drpNguonVon").val();
    var iIDBcQuyetToan = $("#iIDBcQuyetToan").val();
    var drpLoaiThanhToan = $("#drpLoaiThanhToan").val();
    if (drpLoaiThanhToan == null) return;
    RenderQuyetToanNienDoTongHop(drpLoaiThanhToan, iIDBcQuyetToan, sMaDonViQuanLy, iNamKeHoach, iIDNguonVonID);
    RenderQuyetToanNienDoPhanTich(drpLoaiThanhToan, iIDBcQuyetToan, sMaDonViQuanLy, iNamKeHoach, iIDNguonVonID);
}

BackIndex = () => {
    window.location.href = "/QLVonDauTu/BcQuyetToanNienDo/Index";
}

function RenderQuyetToanNienDoTongHop(drpLoaiThanhToan, iIDBcQuyetToan, sMaDonViQuanLy, iNamKeHoach, iIDNguonVonID) {
    var sUrl = "";
    if (drpLoaiThanhToan == 1)
        sUrl = "QuyetToanNienDoKHVN"
    else
        sUrl = "QuyetToanNienDoKHU";
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/BcQuyetToanNienDo/" + sUrl,
        data: {
            iIDBcQuyetToan: iIDBcQuyetToan,
            sMaDonVi: sMaDonViQuanLy,
            iNamKeHoach: iNamKeHoach,
            iIDNguonVonID: iIDNguonVonID
        },
        success: function (data) {
            $("#ViewTable").html(data);
            if (bIsViewDetail == 1) {
                $("#ViewTable input").attr("disabled", "disabled");
            }
            DinhDangSo("ViewTable");
        }
    });
}

function RenderQuyetToanNienDoPhanTich(drpLoaiThanhToan, iIDBcQuyetToan, sMaDonViQuanLy, iNamKeHoach, iIDNguonVonID) {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/BcQuyetToanNienDo/QuyetToanNienDoKHVN_PhanTich",
        data: {
            iIDBcQuyetToan: iIDBcQuyetToan,
            sMaDonVi: sMaDonViQuanLy,
            iNamKeHoach: iNamKeHoach,
            iIDNguonVonID: iIDNguonVonID
        },
        success: function (data) {
            $("#ViewTablePhanTich").html(data);
            if (bIsViewDetail == 1) {
                $("#ViewTablePhanTich input").attr("disabled", "disabled");
            }
            DinhDangSo("ViewTablePhanTich");
        }
    });
}

function ValidateForm() {
    var lstErrors = [];
    if ($("#drpDonViQuanLy").val() == undefined || $("#drpDonViQuanLy").val() == null) {
        lstErrors.push("Đơn vị quản lý chưa được nhập !");
    }
    if ($("#txtNamKeHoach").val() == undefined || $("#txtNamKeHoach").val() == null) {
        lstErrors.push("Năm kế hoạch chưa được nhập !");
    }
    if ($("#txtSoPheDuyet").val() == undefined || $("#txtSoPheDuyet").val() == null || $("#txtSoPheDuyet").val() == "") {
        lstErrors.push("Số quyết định chưa được nhập !");
    }
    if ($("#txtNgayPheDuyet").val() == undefined || $("#txtNgayPheDuyet").val() == null || $("#txtNgayPheDuyet").val() == "") {
        lstErrors.push("Ngày đề nghị chưa được nhập !");
    }
    if ($("#drpNguonVon").val() == undefined || $("#drpNguonVon").val() == null) {
        lstErrors.push("Nguồn vốn chưa được nhập !");
    }
    if ($("#drpLoaiThanhToan").val() == undefined || $("#drpLoaiThanhToan").val() == null) {
        lstErrors.push("Loại thanh toán chưa được nhập !");
    }
    if (lstErrors.length != 0) {
        alert(lstErrors.join("\n"));
        return false;
    }
    return true;
}

function Insert() {
    var data = {};
    var lstData = [];
    if (!ValidateForm()) return;

    data.iID_BCQuyetToanNienDoID = $("#iIDBcQuyetToan").val();
    data.sSoDeNghi = $("#txtSoPheDuyet").val();
    data.iCoQuanThanhToan = $("#drpCoQuanThanhToan").val();
    data.dNgayDeNghi = $("#txtNgayPheDuyet").val();
    data.iNamKeHoach = $("#txtNamKeHoach").val();
    data.iID_NguonVonID = $("#drpNguonVon").val();
    data.iID_MaDonViQuanLy = $("#drpDonViQuanLy").val();
    data.iID_DonViQuanLyID = $("#drpDonViQuanLy :selected").data("id");
    data.iLoaiThanhToan = $("#drpLoaiThanhToan").val();

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/BcQuyetToanNienDo/UpdateBcQuyetToanNienDo",
        data: {
            data: data,
        },
        success: function (r) {
            if (r.iIdBcQuyetToanNienDoId != null && r.iIdBcQuyetToanNienDoId != GUID_EMPTY) {
                fnSaveDataKeHoachVonNam(r.iIdBcQuyetToanNienDoId);
                fnSaveDataKeHoachVonUng(r.iIdBcQuyetToanNienDoId);
                alert("Thêm mới thành công !")
                location.href = "/QLVonDauTu/BcQuyetToanNienDo";
            } else {
                alert("Có lỗi xảy ra trong quá trình thêm mới !");
            }
        }
    });
}

function CancelSaveData() {
    location.href = "/QLVonDauTu/BcQuyetToanNienDo";
}

function SetupDataUpdate() {
    var iLoaiThanhToan = $("#iLoaiThanhToan").val();
    var iIdNguonVonId = $("#iIdNguonVonId").val();
    if (iLoaiThanhToan != null && iLoaiThanhToan != "") {
        $("#drpLoaiThanhToan").val(iLoaiThanhToan);
    }
    if (iIdNguonVonId != null && iIdNguonVonId != "") {
        $("#drpNguonVon").val(iIdNguonVonId);
    }
}

function CheckIsUpdate() {
    var iIDBcQuyetToan = $("#iIDBcQuyetToan").val();
    if (iIDBcQuyetToan == null || iIDBcQuyetToan == iIdEmpty) {
        $("#drpDonViQuanLy").removeAttr("disabled");
        $("#txtNamKeHoach").removeAttr("disabled");
        $("#txtNgayPheDuyet").removeAttr("disabled");
        $("#drpNguonVon").removeAttr("disabled");
        $("#drpLoaiThanhToan").removeAttr("disabled");
        $("#txtSoPheDuyet").removeAttr("disabled");
        $("#txtMoTa").removeAttr("disabled");
    } else {
        $("#drpDonViQuanLy").attr("disabled", "disabled");
        $("#txtNamKeHoach").attr("disabled", "disabled");
        $("#txtNgayPheDuyet").attr("disabled", "disabled");
        $("#drpNguonVon").attr("disabled", "disabled");
        $("#drpLoaiThanhToan").attr("disabled", "disabled");
        if (bIsViewDetail == 1) {
            $("#txtSoPheDuyet").attr("disabled", "disabled");
            $("#txtMoTa").attr("disabled", "disabled");
        }
    }
}

//------- Event KHVN
function fnChangeGiaTriTamUngDieuChinhGiam(e) {
    var fGiaTriTamUngDieuChinhGiam = $(e).val();
    if (fGiaTriTamUngDieuChinhGiam == null || !$.isNumeric(fGiaTriTamUngDieuChinhGiam)) {
        fGiaTriTamUngDieuChinhGiam = 0
    }
    var fTamUngTheoCheDoChuaThuHoiNamTruoc = $(e).closest("tr").find(".fTamUngTheoCheDoChuaThuHoiNamTruoc").text();
    var fTamUngNamTruocThuHoiNamNay = $(e).closest("tr").find(".fTamUngNamTruocThuHoiNamNay").text();
    var fTamUngTheoCheDoChuaThuHoiKeoDaiNamNay = $(e).closest("tr").find(".fTamUngTheoCheDoChuaThuHoiKeoDaiNamNay").text();
    var fTamUngTheoCheDoChuaThuHoiNamNay = $(e).closest("tr").find(".fTamUngTheoCheDoChuaThuHoiNamNay").text();
    $(e).val(Number(fGiaTriTamUngDieuChinhGiam).toLocaleString('vi-VN'));
    $(e).closest("tr").find(".fLuyKeTamUngChuaThuHoiChuyenSangNam").text(FormatNumber(
        parseFloat(UnFormatNumber(fTamUngTheoCheDoChuaThuHoiNamTruoc)) - parseFloat(UnFormatNumber(fGiaTriTamUngDieuChinhGiam)) - parseFloat(UnFormatNumber(fTamUngNamTruocThuHoiNamNay))
        + parseFloat(UnFormatNumber(fTamUngTheoCheDoChuaThuHoiKeoDaiNamNay)) + parseFloat(UnFormatNumber(fTamUngTheoCheDoChuaThuHoiNamNay))
    ));
}

function fnChangeGiaTriNamTruocChuyenNamSau(e) {
    var fGiaTriNamTruocChuyenNamSau = $(e).val();
    if (fGiaTriNamTruocChuyenNamSau == null || !$.isNumeric(fGiaTriNamTruocChuyenNamSau)) {
        fGiaTriNamTruocChuyenNamSau = 0
    }
    var fKHVNamTruocChuyenNamNay = $(e).closest("tr").find(".fKHVNamTruocChuyenNamNay").text();
    var fTongThanhToanVonKeoDaiNamNay = $(e).closest("tr").find(".fTongThanhToanVonKeoDaiNamNay").text();
    $(e).val(Number(fGiaTriNamTruocChuyenNamSau).toLocaleString('vi-VN'));
    $(e).closest("tr").find(".fVonConLaiHuyBoKeoDaiNamNay").text(FormatNumber(
        UnFormatNumber(fKHVNamTruocChuyenNamNay) - UnFormatNumber(fTongThanhToanVonKeoDaiNamNay) - UnFormatNumber(fGiaTriNamTruocChuyenNamSau)
    ));
}

function fnChangeGiaTriNamNayChuyenNamSau(e) {
    var fGiaTriNamNayChuyenNamSau = $(e).val();
    if (fGiaTriNamNayChuyenNamSau == null || !$.isNumeric(fGiaTriNamNayChuyenNamSau)) {
        fGiaTriNamNayChuyenNamSau = 0
    }
    var fKHVNamNay = $(e).closest("tr").find(".fKHVNamNay").text();
    var fTongKeHoachThanhToanVonNamNay = $(e).closest("tr").find(".fTongKeHoachThanhToanVonNamNay").text();
    $(e).val(Number(fGiaTriNamNayChuyenNamSau).toLocaleString('vi-VN'))
    $(e).closest("tr").find(".fVonConLaiHuyBoNamNay").text(FormatNumber(
        UnFormatNumber(fKHVNamNay) - UnFormatNumber(fTongKeHoachThanhToanVonNamNay) - UnFormatNumber(fGiaTriNamNayChuyenNamSau)
    ));
}

// Event KHVU
function onChangeGiaTriThuHoiTheoGiaiNganThucTe(e) {
    var fGiaTriThuHoiTheoGiaiNganThucTe = $(e).val();
    if (fGiaTriThuHoiTheoGiaiNganThucTe == null || !$.isNumeric(fGiaTriThuHoiTheoGiaiNganThucTe)) {
        fGiaTriThuHoiTheoGiaiNganThucTe = 0
    }
    $(e).val(Number(fGiaTriThuHoiTheoGiaiNganThucTe).toLocaleString('vi-VN'))
}

// Event Phan tich

function fnChangeDnQuyetToanNamTrc(e) {
    var FDnQuyetToanNamTrc = $(e).val();
    if (FDnQuyetToanNamTrc == null || !$.isNumeric(FDnQuyetToanNamTrc)) {
        FDnQuyetToanNamTrc = 0
    }
    var FDnQuyetToanNamNay = $(e).closest("tr").find(".FDnQuyetToanNamNay").text();

    $(e).closest("tr").find(".FSumSoDeNghiQuyetToan").text(FormatNumber(
        UnFormatNumber(FDnQuyetToanNamTrc) + UnFormatNumber(FDnQuyetToanNamNay)
    ));

    $(e).val(Number(FDnQuyetToanNamTrc).toLocaleString('vi-VN'))
}

function fnChangeDnQuyetToanNamNay(e) {
    var FDnQuyetToanNamNay = $(e).val();
    if (FDnQuyetToanNamNay == null || !$.isNumeric(FDnQuyetToanNamNay)) {
        FDnQuyetToanNamNay = 0
    }
    var FDnQuyetToanNamTrc = $(e).closest("tr").find(".FDnQuyetToanNamTrc").text();

    $(e).closest("tr").find(".FSumSoDeNghiQuyetToan").text(FormatNumber(
        UnFormatNumber(FDnQuyetToanNamTrc) + UnFormatNumber(FDnQuyetToanNamNay)
    ));

    $(e).val(Number(FDnQuyetToanNamNay).toLocaleString('vi-VN'))
}

function fnChangeDuToanThuHoi(e) {
    var FDuToanThuHoi = $(e).val();
    if (FDuToanThuHoi == null || !$.isNumeric(FDuToanThuHoi)) {
        FDuToanThuHoi = 0
    }
    $(e).val(Number(FDuToanThuHoi).toLocaleString('vi-VN'))
}

function ValidateData(data, isTongHop) {
    var Title = 'Lỗi thêm mới/chỉnh sửa quyết toán niên độ';
    var Messages = [];

    if (data.sSoDeNghi == null || data.sSoDeNghi == "") {
        Messages.push("Số đề nghị chưa nhập !");
    }

    if (data.dNgayDeNghi == null || data.dNgayDeNghi == 0) {
        Messages.push("ngày đề nghị chưa chọn !");
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

function ValidateDatatonghop(data) {
    var Title = 'Lỗi thêm mới/chỉnh sửa quyết toán niên độ';
    var Messages = [];

    if (data.sSoDeNghi == null || data.sSoDeNghi == "") {
        Messages.push("Số đề nghị chưa nhập !");
    }

    if (data.dNgayDeNghi == null || data.dNgayDeNghi == 0) {
        Messages.push("ngày đề nghị chưa chọn !");
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
function SoftData(data, sKey, sKeyRefer, sParent) {
    var result = [];
    var parentData = [];
    $.each(data, function (indexItem, value) {

        var itemCheck = $.map(data, function (n) { return n[sParent] == value[sKeyRefer] ? n : null })[0];

        if (itemCheck != null && value[sKeyRefer] != null) {
            if (parentData[value[sKeyRefer]] == undefined) {
                parentData[value[sKeyRefer]] = [];
            }
        }

        if (value[sParent] == null) {
            result.push(value[sKey])
            return true;
        } else {
            if (parentData[value[sParent]] == undefined) {
                parentData[value[sParent]] = [];
            }
            parentData[value[sParent]].push(value[sKey]);
        }
    });
    var objResult = { parentData: parentData, result: result };
    return objResult;
}


// View In báo cáo
ViewInBaoCao = () => {
    window.location.href = "/QLVonDauTu/BcQuyetToanNienDo/ViewInBaoCao/";
}
ValidateDataBaoCao = (data) => {
    var Title = 'Lỗi in báo cáo';
    var Messages = [];

    if (data.sLoaiThanhToan == null || data.sLoaiThanhToan == "") {
        Messages.push("Loại thanh toán chưa chọn !");
    }

    if (data.sTenNguonVon == null || data.sNguonVon == "") {
        Messages.push("Nguồn vốn chưa chọn !");
    }


    if (data.INamKeHoach == null || data.INamKeHoach == "") {
        Messages.push("Năm kế hoạch chưa nhập !");
    }

    //if (data.iIdMaDonViQuanLy == null || data.iIdMaDonViQuanLy == "") {
    //    Messages.push("Đơn vị quản lý chưa chọn !");
    //}

    if (data.sValueDonViTinh == null || data.sValueDonViTinh == "") {
        Messages.push("Đơn vị tính chưa chọn !");
    }

    if (Messages != null && Messages != undefined && Messages.length > 0) {
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
PrintBaoCao = (isPdf = true) => {
    var data = {};

    data.INamKeHoach = $("#txtNamKeHoach").val();
    data.sTenDonViQuanLy = $("#iID_DonViQuanLyID :selected").text();
    data.iID_DonViQuanLyID = $("#iID_DonViQuanLyID :selected").val();
    data.IIdNguonVonId = $("#iID_MaNguonNganSach :selected").val();
    data.sTenNguonVon = $("#iID_MaNguonNganSach :selected").text();
    data.ILoaiThanhToan = $("#ValueItemLoaiChungTu :selected").val();
    data.sLoaiThanhToan = $("#ValueItemLoaiChungTu :selected").text();
    data.sDonViTinh = $("#ValueItemDonViTinh :selected").text();
    data.sValueDonViTinh = $("#ValueItemDonViTinh :selected").val();
    data.txt_TieuDe1 = $("#txtHeader1").val();
    data.txt_TieuDe2 = $("#txtHeader2").val();
    data.txt_TieuDe3 = $("#txtHeader3").val();
    if (!ValidateDataBaoCao(data)) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/BcQuyetToanNienDo/PrintBaoCao",
        data: { dataPrintReport: data, isPdf: isPdf },
        success: function (data) {
            if (data.status) {
                window.open("/QLVonDauTu/BcQuyetToanNienDo/ExportReport/?iLoaiBaoCao=" + $("#ValueItemLoaiChungTu :selected").val() + "&pdf=" + data.isPdf);
            }
            else {
                var Title = 'Lỗi in báo cáo';
                var messErr = [];
                messErr.push(data.listErrMess);
                $.ajax({
                    type: "POST",
                    url: "/Modal/OpenModal",
                    data: { Title: Title, Messages: messErr, Category: ERROR },
                    success: function (data) {
                        $("#divModalConfirm").html(data);
                    }
                });
                return false;
            }
        }
    });
    return true;
}

function fnSaveDataKeHoachVonNam(iIdBcQuyetToanId) {
    var iLoaiBaoCao = $("#drpLoaiThanhToan").val();
    var lstData = [];
    $.each($("#ViewTable tbody tr"), function (index, item) {
        var obj = {};
        obj.iID_DuAnID = $(item).data("iidduan");
        if (iLoaiBaoCao == 1) {
            obj.fGiaTriNamTruocChuyenNamSau = UnFormatNumber($(item).find(".fGiaTriNamTruocChuyenNamSau").val());                           // 14
            obj.fGiaTriNamNayChuyenNamSau = UnFormatNumber($(item).find(".fGiaTriNamNayChuyenNamSau").val());                               // 20
            obj.fGiaTriTamUngDieuChinhGiam = UnFormatNumber($(item).find(".fGiaTriTamUngDieuChinhGiam").val());                            // 8
            obj.fTamUngChuaThuHoiTrcNamQuyetToan = UnFormatNumber($(item).find(".fTamUngTheoCheDoChuaThuHoiNamTruoc").text());              // 7
            obj.fThuHoiUngNamTrc = UnFormatNumber($(item).find(".fTamUngNamTruocThuHoiNamNay").text());                                     // 9
            obj.fChiTieuNamTrcChuyenSang = UnFormatNumber($(item).find(".fKHVNamTruocChuyenNamNay").text());                                // 10
            obj.fThanhToanKLHT_CTNamTrcChuyenSang = UnFormatNumber($(item).find(".fTongThanhToanSuDungVonNamTruoc").text());                // 12
            obj.fTamUngChuaThuHoi_CTNamTrcChuyenSang = UnFormatNumber($(item).find(".fTamUngTheoCheDoChuaThuHoiKeoDaiNamNay").text());      // 13
            obj.fChiTieuNamNay = UnFormatNumber($(item).find(".fKHVNamNay").text());                                                        // 16
            obj.fThanhToanKLHT_CTNamNay = UnFormatNumber($(item).find(".fTongThanhToanSuDungVonNamNay").text());                            // 18
            obj.fTamUngChuaThuHoi_CTNamNay = UnFormatNumber($(item).find(".fTamUngTheoCheDoChuaThuHoiNamNay").text());                      // 19

        } else if (iLoaiBaoCao == 2) {
            obj.fGiaTriUngChuyenNamSau = UnFormatNumber($(item).find(".fKHVUChuaThuHoiChuyenNamSau").text());
            obj.fGiaTriThuHoiTheoGiaiNganThucTe = UnFormatNumber($(item).find(".fGiaTriThuHoiTheoGiaiNganThucTe").val());
            obj.fLKThanhToanDenTrcNamQuyetToan = UnFormatNumber($(item).find(".fLuyKeThanhToanNamTruoc").text());
            obj.fKHUngTrcChuaThuHoiTrcNamQuyetToan = UnFormatNumber($(item).find(".fUngTruocChuaThuHoiNamTruoc").text());
            obj.fLKThanhToanDenTrcNamQuyetToan_KHUng = UnFormatNumber($(item).find(".fLuyKeThanhToanNamTruoc").text());
            obj.fThanhToan_KHUngNamTrcChuyenSang = UnFormatNumber($(item).find(".fVonKeoDaiDaThanhToanNamNay").text());
            obj.fThuHoiUngTruoc = UnFormatNumber($(item).find(".fThuHoiVonNamNay").text());
            obj.fKHUngNamNay = UnFormatNumber($(item).find(".fKHVUNamNay").text());
            obj.fThanhToan_KHUngNamNay = UnFormatNumber($(item).find(".fVonDaThanhToanNamNay").text());
        }
        lstData.push(obj);
    });
    if (lstData.length == 0) return;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/BcQuyetToanNienDo/UpdateBcQuyetToanNienDoChiTiet",
        async: false,
        data: { iIdBcQuyetToanId: iIdBcQuyetToanId, lstData: lstData },
        success: function (data) {

        }
    });
}

function fnSaveDataKeHoachVonUng(iIdBcQuyetToanId) {
    var lstData = [];
    $.each($("#ViewTablePhanTich tbody tr"), function (index, item) {
        var obj = {};
        obj.iID_DuAnID = $(item).data("iidduan");
        obj.fDuToanCNSChuaGiaiNganTaiKB = UnFormatNumber($(item).find(".FDuToanCnsChuaGiaiNganTaiKb").text());      // 1
        obj.fDuToanCNSChuaGiaiNganTaiDV = UnFormatNumber($(item).find(".FDuToanCnsChuaGiaiNganTaiDv").text());      // 2
        obj.fDuToanCNSChuaGiaiNganTaiCuc = UnFormatNumber($(item).find(".FDuToanCnsChuaGiaiNganTaiCuc").text());    // 3
        obj.fTuChuaThuHoiTaiCuc = UnFormatNumber($(item).find(".FTuChuaThuHoiTaiCuc").text());                      // 18
        obj.fChiTieuNamNayKB = UnFormatNumber($(item).find(".FChiTieuNamNayKb").text());                            // 6
        obj.fChiTieuNamNayLC = UnFormatNumber($(item).find(".FChiTieuNamNayLc").text());                            // 7
        obj.fSoCapNamTrcCS = UnFormatNumber($(item).find(".FSoCapNamTrcCs").text());                                // 10
        obj.fSoCapNamNay = UnFormatNumber($(item).find(".FSoCapNamNay").text());                                    // 11
        obj.fDnQuyetToanNamTrc = UnFormatNumber($(item).find(".FDnQuyetToanNamTrc").val());                         // 13
        obj.fDnQuyetToanNamNay = UnFormatNumber($(item).find(".FDnQuyetToanNamNay").val());                         // 14
        obj.fTuChuaThuHoiTaiDonVi = UnFormatNumber($(item).find(".FTuChuaThuHoiTaiDonVi").text());                  // 19
        obj.fDuToanThuHoi = UnFormatNumber($(item).find(".FDuToanThuHoi").val());                                   // 24
        lstData.push(obj);
    });
    if (lstData.length == 0) return;
    $.ajax({
        type: "POST",
        url: "/QLVonDauTu/BcQuyetToanNienDo/UpdateBcQuyetToanNienDoPhanTich",
        async: false,
        data: { iIdBcQuyetToanId: iIdBcQuyetToanId, lstData: lstData },
        success: function (data) {

        }
    });
}

function BtnDownloadDataClick() {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/BcQuyetToanNienDo/ImportData",
        success: function (data) {
            $("#contentModalQTNienDo").html(data);
            $("#modalQTNienDoLabel").html('Danh sách file báo cáo quyết toán niên độ');
            $('#modalQTNienDo').modal('show');
        }
    });
}
function loadGridListExcel() {
    var id_DonVi = document.getElementById("drpDonViQuanLyImport").value;
    var iNam = document.getElementById('txtNamKeHoachImport').value;
    var iLoaiThanhToan = document.getElementById('drpLoaiThanhToanImport').value;
    var Title = 'Thông báo';
    var Messages = [];
    if (id_DonVi == "" || id_DonVi == null) {
        Messages.push('Vui lòng chọn đơn vị!');
    }
    if (iNam == "" || iNam == null) {
        Messages.push('Vui lòng nhập Năm kế hoạch!');
    }
    if (iLoaiThanhToan == "" || iLoaiThanhToan == null) {
        Messages.push('Vui lòng nhập LoaiThanhToan!');
    }

    if (Messages != null && Messages != undefined && Messages.length > 0) {
        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: Messages, Category: 1 },
            success: function (data) {
                $("#divModalConfirm").empty().html(data);
            }
        });
        return false;
    }
    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLVonDauTu/BcQuyetToanNienDo/GetGridListExcelFromFTP",
        data: { idDonVi: id_DonVi, nam: iNam, loaiThanhToan: iLoaiThanhToan },
        success: function (data) {
            $("#contentModalQTNienDo").html(data);
            $("#modalQTNienDoLabel").html('Danh sách file báo cáo quyết toán niên dộ');
            $('#modalQTNienDo').modal('show');
            $("#drpDonViQuanLyImport").val(id_DonVi);
            $("#txtNamKeHoachImport").val(iNam);
            $("#drpLoaiThanhToanImport").val(iLoaiThanhToan);
        }
    });
}

function DownloadFile() {
    let lg = $("input[type='checkbox'][name='checkboxInRow']:checked").length;
    if (lg != 1) {
        var Title = 'Thông báo';
        var Messages = [];

        if (lg < 1) {
            Messages.push('Vui lòng chọn một file để thực hiện đồng bộ dữ liệu!');
        } else {
            Messages.push('Vui lòng chỉ chọn một file để thực hiện đồng bộ dữ liệu!');
        }

        $.ajax({
            type: "POST",
            url: "/Modal/OpenModal",
            data: { Title: Title, Messages: Messages, Category: 1 },
            success: function (data) {
                $("#divModalConfirm").empty().html(data);
            }
        });
    } else {
        let url = $("input[type='checkbox'][name='checkboxInRow']:checked").first().val();
        $('#modalKH5NamDeXuat').modal('hide');
        location.href = "/QLVonDauTu/BcQuyetToanNienDo/DownloadFileExcel?url=" + url;
    }
}

function DinhDangSo(tblName) {
    $("#" + tblName + " .sotien").each(function () {
        $(this).html(FormatNumber($(this).html().trim()) == "" ? 0 : FormatNumber($(this).html().trim()));
    })
    $("#" + tblName + " .currently").each(function () {
        $(this).val(FormatNumber($(this).val().trim()) == "" ? 0 : FormatNumber($(this).val().trim()));
    })
}