var TBL_THONGTIN_THANHTOAN = 'tblListTTThanhToan';
var arrError = [];
var idContentView = "lstDataView";
var sUrlListView = "/DeNghiThanhToan/MucNganSachSearch";
var _paging = {};
var data_search = {};
var tt_thanhtoan = {};
var thanhtoan_chitiet = {};
var arr_thanhtoan_chitiet = [];
var type_action = 0; // 0: Thêm mới, 1 Sửa đổi, 2 Xem
var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var id_denghithanhtoan = GUID_EMPTY;
var listIDChiTietXoa = [];
var ERROR = 1;
var CONFIRM = 0;

function ChangePage(iCurrentPage = 1) {
    data_search = {};
    _paging.CurrentPage = iCurrentPage;
    data_search._paging = _paging;

    data_search.sLNS = $("<div/>").text($.trim($("#txtLNS").val())).html();
    data_search.sL = $("<div/>").text($.trim($("#txtL").val())).html();
    data_search.sK = $("<div/>").text($.trim($("#txtK").val())).html();
    data_search.sM = $("<div/>").text($.trim($("#txtM").val())).html();
    data_search.sTM = $("<div/>").text($.trim($("#txtTM").val())).html();
    data_search.sTTM = $("<div/>").text($.trim($("#txtTTM").val())).html();
    data_search.sNG = $("<div/>").text($.trim($("#txtNG").val())).html();
    data_search.sTNG = $("<div/>").text($.trim($("#txtTNG").val())).html();
    data_search.sNoiDung = $("<div/>").text($.trim($("#txtNoiDung").val())).html();
    GetListData();
}

function ResetChangePage(iCurrentPage = 1) {
    data_search = {};
    _paging.CurrentPage = iCurrentPage;
    data_search._paging = _paging;
    GetListData(iCurrentPage);
}

function GetListData() {
    $.ajax({
        type: "POST",
        dataType: "html",
        url: sUrlListView,
        data: { data: data_search },
        success: function (data) {
            $("#modalMucLucNganSach").html(data);
            AddConditionSearchMLNS();
        }
    });
}

function AddConditionSearchMLNS() {
    $('#txtLNS').val($("<div/>").html(data_search.sLNS).text());
    $('#txtL').val($("<div/>").html(data_search.sL).text());
    $('#txtK').val($("<div/>").html(data_search.sK).text());
    $('#txtM').val($("<div/>").html(data_search.sM).text());
    $('#txtTM').val($("<div/>").html(data_search.sTM).text());
    $('#txtTTM').val($("<div/>").html(data_search.sTTM).text());
    $('#txtNG').val($("<div/>").html(data_search.sNG).text());
    $('#txtTNG').val($("<div/>").html(data_search.sTNG).text());
    $('#txtNoiDung').val($("<div/>").html(data_search.sNoiDung).text());
}

function LoadThongTinCreate() {
    $('#dhopdong').hide();
    $('#dhodongusd').hide();
    $('#dhodongvnd').hide();
    $('#dduan').hide();
    $('#ddutoanusd').hide();
    $('#ddutoanvnd').hide();
    $('#iloainoidung').val(1).change();
}

function LoadThongTinUpdate() {
    $('#iloainoidung').val($('#hiloainoidung').val());
    $('#itenchuongtrinh').val($('#hitenchuongtrinh').val());
    $('#ichudautu').val($('#hichudautu').val());
    $('#ithanhtoantheo').val($('#hithanhtoantheo').val());
    $('#itenhopdong').val($('#hitenhopdong').val());
    $('#itenduan').val($('#hitenduan').val());
    $('#iloaidenghi').val($('#hiloaidenghi').val());
    $('#inamngansach').val($('#hinamngansach').val());
    $('#icoquanthanhtoan').val($('#hicoquanthanhtoan').val());
    $('#itigiadenghi').val($('#hitigiadenghi').val());
    $('#itigiapheduyet').val($('#hitigiapheduyet').val());
    $('#itrangthai').val($('#hitrangthai').val());
    $('#itendonvihuongthu').val($('#hitendonvihuongthu').val());

    if ($('#hithanhtoantheo').val() != null && $('#hithanhtoantheo').val() == 1) {
        $('#dhopdong').show();
        $('#dhodongusd').show();
        $('#dhodongvnd').show();
        $('#dduan').hide();
        $('#ddutoanusd').hide();
        $('#ddutoanvnd').hide();
    } else if ($('#hithanhtoantheo').val() == 2) {
        $('#dhopdong').hide();
        $('#dhodongusd').hide();
        $('#dhodongvnd').hide();
        $('#dduan').show();
        $('#ddutoanusd').show();
        $('#ddutoanvnd').show();
    } else {
        $('#dhopdong').hide();
        $('#dhodongusd').hide();
        $('#dhodongvnd').hide();
        $('#dduan').hide();
        $('#ddutoanusd').hide();
        $('#ddutoanvnd').hide();
    }
    onChangeTrangThai();
}

function onChangeDonVi() {
    var iIDonVi = $('#idonvi').val();
    $.ajax({
        url: "/DeNghiThanhToan/GetAllNhiemVuChiByDonVi",
        type: "POST",
        data: { iDDonVi: iIDonVi },
        dataType: "json",
        success: function (result) {
            if (result) {
                $('#itenchuongtrinh').empty().html(result.htmlCT);
                $("#itenchuongtrinh").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
                LoadThongTinLuyKe(0);
            }
        }
    })
}

function onChangeThanhToanTheo() {
    var inoidungthanhtoan = $('#ithanhtoantheo').val();
    if (inoidungthanhtoan == 1) {
        $('#dhopdong').show();
        $('#dhodongusd').show();
        $('#dhodongvnd').show();
        $('#dduan').hide();
        $('#ddutoanusd').hide();
        $('#ddutoanvnd').hide();
    } else if (inoidungthanhtoan == 2) {
        $('#dduan').show();
        $('#ddutoanusd').show();
        $('#ddutoanvnd').show();
        $('#dhodongusd').hide();
        $('#dhodongvnd').hide();
        $('#dhopdong').hide();
    } else {
        $('#dhodongusd').hide();
        $('#dhodongvnd').hide();
        $('#ddutoanusd').hide();
        $('#ddutoanvnd').hide();
        $('#dduan').hide();
        $('#dhopdong').hide();
    }
}

function onChangeChuDauTu() {
    var iID_NhiemVuChi = $('#itenchuongtrinh').val();
    var iID_ChuDauTu = $('#ichudautu').val();
    $.ajax({
        url: "/DeNghiThanhToan/GetThongTinDuAn",
        type: "POST",
        data: {
            iID_NhiemVuChi: iID_NhiemVuChi,
            iID_ChuDauTu: iID_ChuDauTu
        },
        dataType: "json",
        success: function (result) {
            if (result) {
                $('#itenduan').empty().html(result.htmlDA);
                $("#itenduan").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
                //$("#igtrhopdongusd").val('');
                //$("#igtrhopdongvnd").val('');
                $("#iddutoanusd").val('');
                $("#iddutoanvnd").val('');
                LoadThongTinLuyKe(0);
            }
        }
    })
}

function onChangeNhiemVuChi() {
    var iID_NhiemVuChi = $('#itenchuongtrinh').val();
    $.ajax({
        url: "/DeNghiThanhToan/GetThongTinHopDong",
        type: "POST",
        data: { iID_NhiemVuChi: iID_NhiemVuChi },
        dataType: "json",
        success: function (result) {
            if (result) {
                $('#itenhopdong').empty().html(result.htmlHD);
                $("#itenhopdong").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
                $("#igtrhopdongusd").val('');
                $("#igtrhopdongvnd").val('');
                $("#iddutoanusd").val('');
                $("#iddutoanvnd").val('');
                onChangeChuDauTu();
            }
        }
    })
}

function onChangeHopDong() {
    var idHopDong = $('#itenhopdong').val();
    $.ajax({
        url: "/DeNghiThanhToan/GetHopDongByID",
        type: "POST",
        data: { id: idHopDong },
        dataType: "json",
        success: function (data) {
            if (data && data.status == true) {
                $("#igtrhopdongusd").val(FormatNumber(data.result.fGiaTriUSD, 2));
                $("#igtrhopdongvnd").val(FormatNumber(data.result.fGiaTriVND, 0));
                LoadThongTinLuyKe(0);
            }
        }
    });
}

function onChangeDuAn() {
    var idDuAn = $('#itenduan').val();
    $.ajax({
        url: "/DeNghiThanhToan/GetDuAnByID",
        type: "POST",
        data: { id: idDuAn },
        dataType: "json",
        success: function (data) {
            if (data.status == true && data.status != null) {
                $("#iddutoanusd").val(FormatNumber(data.result.fGiaTriUSD));
                $("#iddutoanvnd").val(FormatNumber(data.result.fGiaTriVND));
                LoadThongTinLuyKe(0);
            }
        }
    });
}

function onChangeLoaiDeNghi() {
    var loaithanhtoan = $('#iloaidenghi').val();
    var str = " <option value=''>--Chọn cơ quan thanh toán--</option>";
    if (loaithanhtoan == 1) {
        str = str + " <option value='2'> Đơn vị cấp</option>";
    } else {
        str = str + " <option value='1'>CTC cấp</option>";
        str = str + " <option value='2'>Đơn vị cấp</option>";
    }
    $('#icoquanthanhtoan').empty().html(str);
    $("#icoquanthanhtoan").select2({ width: "100%", dropdownAutoWidth: true, matcher: FilterInComboBox });
}

function ThemDuLieuChiTiet() {
    var disabled_dn = "disabled";
    var disabled_pd = "disabled";
    if ($("#itrangthai").val() == 3) {
        disabled_pd = "";
        disabled_dn = "";
    } else {
        disabled_dn = "";
    }

    var str = "<tr>";
    /* Mục lục ngân sách */
    str = str + "<td class='text-center imuclucngansach' data-ingansach='' onclick='ShowMucLucNganSach(this)' style='cursor: pointer;' title='Chọn mục lục ngân sách'></td>";
    /* Nội dung chi */
    str = str + "<td><input type='text' class='form-control inoidungchi'></td>";
    /* Số kinh phí đề nghị thanh toán kỳ này USD */
    str = str + "<td><input type='text' class='form-control text-right txtMoneyNewUsd' id='sdenghiusd' " + disabled_dn + "></td>";
    /* Số kinh phí đề nghị thanh toán kỳ này VND */
    str = str + "<td><input type='text' class='form-control text-right txtMoneyNewVnd' id='sdenghivnd' " + disabled_dn + "></td>";
    /* Số kinh phí phê duyệt kỳ này USD */
    str = str + "<td><input type='text' class='form-control text-right txtMoneyNewUsd' id='sduocduyetusd' " + disabled_pd + "></td > ";
    /* Số kinh phí phê duyệt kỳ này VND */
    str = str + "<td><input type='text' class='form-control text-right txtMoneyNewVnd' id='sduocduyetvnd' " + disabled_pd + "></td>";
    /* Thao tác */
    str = str + "<td class ='text-center'><button class='btn-delete btn-icon' onclick='XoaDong(this, \"" + TBL_THONGTIN_THANHTOAN + "\")' type='button'><span class='fa fa-trash-o fa-lg' aria-hidden='true'></span></button></td>";
    str = str + "</tr>";

    $('#tblListTTThanhToan tbody').append(str);

    $(".txtMoneyNewUsd").keydown(function (event) {
        ValidateInputKeydown(event, this, 2);
    }).blur(function (event) {
        if (ValidateInputFocusOut(event, this, 2, null, null, true)) {
            UpdateTongThongTinThanhToan(this);
        }
    });

    $(".txtMoneyNewVnd").keydown(function (event) {
        ValidateInputKeydown(event, this, 1);
    }).blur(function (event) {
        if (ValidateInputFocusOut(event, this, 2, null, null, true)) {
            UpdateTongThongTinThanhToan(this);
        }
    });
}

function XoaDong(nutXoa, idBang) {
    var dongXoa = nutXoa.parentElement.parentElement;
    var iddong = $(dongXoa).data('idchitiet');
    if (iddong != null) {
        listIDChiTietXoa.push(iddong);
    }
    dongXoa.parentNode.removeChild(dongXoa);
    UpdateTongThongTinThanhToan(nutXoa);
}

function onChangeTiGiaPheDuyet() {
    var tigia = $('#itigiapheduyet').val();
    $('#tblListTTThanhToan tbody tr').each(function (index, tr) {
        var sotiennhap = $(tr).find('#sduocduyetusd').val();
        $.ajax({
            type: "POST",
            url: "/QLNH/DeNghiThanhToan/GetChuyenDoiTyGia",
            data: { matygia: tigia, sotiennhap: parseInt(UnFormatNumber(sotiennhap)), loaitiennhap: 1 },
            async: false,
            success: function (data) {
                $(tr).find('#sduocduyetvnd').val(FormatNumber(data.chuyendoi, 2));
            }
        });
        UpdateTongThongTinThanhToan(this);

    })
}

function onChangeTrangThai() {

    $('#tblListTTThanhToan tbody tr').each(function () {
        if ($("#itrangthai").val() == 3) {
            $(this).find('#sdenghiusd').removeAttr("disabled", "disabled");
            $(this).find('#sdenghivnd').removeAttr("disabled", "disabled");

            $(this).find('#sduocduyetusd').removeAttr("disabled", "disabled");
            $(this).find('#sduocduyetvnd').removeAttr("disabled", "disabled");
        }

        else {
            $(this).find('#sdenghiusd').removeAttr("disabled", "disabled");
            $(this).find('#sdenghivnd').removeAttr("disabled", "disabled");

            $(this).find('#sduocduyetusd').attr("disabled", "disabled");
            $(this).find('#sduocduyetvnd').attr("disabled", "disabled");
        }
    })
}

function UpdateTongThongTinThanhToan(td_dong) {

    //Check tỷ giá đề nghị, tỷ giá quy đổi
    var id_dong = $(td_dong).attr('id');
    var parentElemnt = td_dong.parentElement.parentElement
    var status = false;
    var loaitiennhap = 1;
    var matygia = GUID_EMPTY;
    var sotiennhap = $(parentElemnt).find('#' + id_dong).val();

    if (sotiennhap != undefined && sotiennhap != "") {
        sotiennhap = parseFloat(UnFormatNumber($(parentElemnt).find('#' + id_dong).val()));
        if (id_dong == "sdenghiusd" || id_dong == "sdenghivnd") {
            if ($("#itigiadenghi").val() == '') {
                arrError.push("Chưa chọn tỷ giá đề nghị. Vui lòng chọn tỉ giá để thực hiện quy đổi");
                showErr(CONFIRM);
                $(td_dong).val("");
            } else {
                if (id_dong == "sdenghivnd") {
                    loaitiennhap = 2;
                }
                matygia = $('#itigiadenghi').val();
                status = true;
            }
        }
        else {
            if (id_dong == "sduocduyetusd" || id_dong == "sduocduyetvnd") {
                if ($("#itigiapheduyet").val() == '') {
                    arrError.push("Chưa chọn tỷ giá phê duyệt. Vui lòng chọn tỉ giá để thực hiện quy đổi");
                    showErr(CONFIRM);
                    $(td_dong).val("");
                } else {
                    if (id_dong == "sduocduyetvnd") {
                        loaitiennhap = 2;
                    }
                    matygia = $('#itigiapheduyet').val();
                    status = true;
                }
            }
        }
    }
    else {
        if (id_dong == "sdenghiusd") {
            $(parentElemnt).find('#sdenghivnd').val("");
        }
        if (id_dong == "sdenghivnd") {
            $(parentElemnt).find('#sdenghiusd').val("");
        }
        if (id_dong == "sduocduyetusd") {
            $(parentElemnt).find('#sduocduyetvnd').val("");
        }
        if (id_dong == "sduocduyetvnd") {
            $(parentElemnt).find('#sduocduyetusd').val("");
        }
    }

    if (status) {
        $.ajax({
            type: "POST",
            url: "/QLNH/DeNghiThanhToan/GetChuyenDoiTyGia",
            data: { matygia: matygia, sotiennhap: parseFloat(UnFormatNumber(sotiennhap)), loaitiennhap: loaitiennhap },
            async: false,
            success: function (data) {
                if (id_dong == "sdenghiusd") {
                    $(parentElemnt).find('#sdenghivnd').val(FormatNumber(data.chuyendoi, 0));
                    $(parentElemnt).find('#sdenghiusd').val(FormatNumber(sotiennhap, 2));
                }
                if (id_dong == "sdenghivnd") {
                    $(parentElemnt).find('#sdenghiusd').val(FormatNumber(data.chuyendoi, 2));
                    $(parentElemnt).find('#sdenghivnd').val(FormatNumber(sotiennhap, 0));
                }
                if (id_dong == "sduocduyetusd") {
                    $(parentElemnt).find('#sduocduyetvnd').val(FormatNumber(data.chuyendoi, 0));
                    $(parentElemnt).find('#sduocduyetusd').val(FormatNumber(sotiennhap, 2));
                }
                if (id_dong == "sduocduyetvnd") {
                    $(parentElemnt).find('#sduocduyetusd').val(FormatNumber(data.chuyendoi, 2));
                    $(parentElemnt).find('#sduocduyetvnd').val(FormatNumber(sotiennhap, 0));
                }
            }
        });
    }
    //Cập nhập tổng số
    var tong_sdenghiusd = 0;
    var tong_sdenghivnd = 0;
    var tong_sduocduyetusd = 0;
    var tong_sduocduyetvnd = 0;
    $('#tblListTTThanhToan tbody tr').each(function () {
        var sdenghiusd = $(this).find('#sdenghiusd').val();
        var sdenghivnd = $(this).find('#sdenghivnd').val();
        var sduocduyetusd = $(this).find('#sduocduyetusd').val();
        var sduocduyetvnd = $(this).find('#sduocduyetvnd').val();

        tong_sdenghiusd = tong_sdenghiusd + parseFloat(UnFormatNumber(sdenghiusd == "" ? 0 : sdenghiusd));
        tong_sdenghivnd = tong_sdenghivnd + parseInt(UnFormatNumber(sdenghivnd == "" ? 0 : sdenghivnd));
        tong_sduocduyetusd = tong_sduocduyetusd + parseFloat(UnFormatNumber(sduocduyetusd == "" ? 0 : sduocduyetusd));
        tong_sduocduyetvnd = tong_sduocduyetvnd + parseInt(UnFormatNumber(sduocduyetvnd == "" ? 0 : sduocduyetvnd));
    })

    $('#stongdenghiusd').text(FormatNumber(tong_sdenghiusd, 2));
    $('#stongdenghivnd').text(FormatNumber(tong_sdenghivnd, 0));
    $('#stongduocduyetusd').text(FormatNumber(tong_sduocduyetusd, 2));
    $('#stongduocduyetvnd').text(FormatNumber(tong_sduocduyetvnd, 0));

    LoadThongTinChuyenKhoan();
   
}

function LoadThongTinChuyenKhoan() {
    var suffix = true;
    var sMonney;

    if ($("#itrangthai").val() == 3) {
        if ($("#iloainoidung").val() == 1) {
            sMonney = $("#stongduocduyetusd").text();
            suffix = false;
        }
        else {
            sMonney = $("#stongduocduyetvnd").text();
        }
    }
    else {
        if ($("#iloainoidung").val() == 1) {
            sMonney = $("#stongdenghiusd").text();
            suffix = false;
        }
        else {
            sMonney = $("#stongdenghivnd").text();
        }
    }

    if (sMonney == "") {
        $("#ichuyenkhoanso").val("");
        $("#ibangchu").val("");
    }
    else
    {
        if ($("#iloainoidung").val() == 1) {
            if ($("#itrangthai").val() == 3) {
                $("#ichuyenkhoanso").val($("#stongduocduyetusd").text());
            }
            else {
                $("#ichuyenkhoanso").val($("#stongdenghiusd").text());
            }

        }
        else {
            if ($("#itrangthai").val() == 3) {
                $("#ichuyenkhoanso").val($("#stongduocduyetvnd").text());
            }
            else {
                $("#ichuyenkhoanso").val($("#stongdenghivnd").text());
            }

        }
        //Thực hiện chuyển đổi tiền thành số
        var number = parseInt(UnFormatNumber(sMonney));
        ConvertNumberToString(number, suffix);
    }
}

function ConvertNumberToString(number, suffix) {
    $.ajax({
        type: "POST",
        url: "/QLNH/DeNghiThanhToan/ConvertNumberToText",
        data: { number: number, suffix: suffix },
        success: function (data) {
            if (data.status) {
                $("#ibangchu").val(data.result);
            }
        }
    });
}

function onBlurThongTinChuyenKhoan() {
    if ($.trim($("#ichuyenkhoanso").val()) == "") return;
    var fgiatri = parseFloat(UnFormatNumber($("#ichuyenkhoanso").val()));
    var suffix = true;
    if ($("#iloainoidung").val() == 1) {
        $("#ichuyenkhoanso").val(FormatNumber(fgiatri, 2));
        suffix = false;
    } else {
        $("#ichuyenkhoanso").val(FormatNumber(fgiatri, 0));
    }
    ConvertNumberToString(fgiatri, suffix);

}

function showErr(type) {
    $.ajax({
        type: "POST",
        url: "/Modal/OpenModal",
        data: { Title: "Thông báo", Messages: arrError, Category: type },
        success: function (data) {
            $("#divModalConfirm").html(data);
            arrError = [];
        }
    });
}

function ShowMucLucNganSach(td_dong) {
    td_dong_ngansach = td_dong;
    $('#modalMucLucNganSach').modal('toggle');
    $('#modalMucLucNganSach').modal('show');

    sUrlListView = '/QLNH/DeNghiThanhToan/MucLucNganSachSearch';
    idContentView = 'modalMucLucNganSach';
    //data_search._paging = null;

    $.ajax({
        type: "POST",
        dataType: "html",
        url: "/QLNH/DeNghiThanhToan/OpenMucLucNganSach",
        //data: { data: data_search },
        success: function (data) {
            $("#modalMucLucNganSach").html(data);
        }
    });
}

function ChonMucLucNganSach(tr_muclucngansach) {
    var value_content = $("<div/>").text($(tr_muclucngansach).find('.tdContent').data('content')).html();
    var value_item = $(tr_muclucngansach).find('.tdLNS').data('dong');
    var value_mangansach = $(tr_muclucngansach).find('.tdLNS').data('idmuclucngansach');
    $(td_dong_ngansach).html(value_item);
    $(td_dong_ngansach).closest("tr").find("input.inoidungchi").val($("<div/>").html(value_content).text());
    $(td_dong_ngansach).attr("data-ingansach", value_mangansach);
    $('#modalMucLucNganSach').modal('hide');
}

function onChangeTiGiaDeNghi() {
    var tigia = $('#itigiadenghi').val();
    $('#tblListTTThanhToan tbody tr').each(function (index, tr) {
        var sotiennhap = $(tr).find('#sdenghiusd').val();
        $.ajax({
            type: "POST",
            url: "/QLNH/DeNghiThanhToan/GetChuyenDoiTyGia",
            data: { matygia: tigia, sotiennhap: parseInt(UnFormatNumber(sotiennhap)), loaitiennhap: 1 },
            async: false,
            success: function (data) {
                $(tr).find('#sdenghivnd').val(FormatNumber(data.chuyendoi, 2));
            }
        });
        UpdateTongThongTinThanhToan(this);
    })
}

function onChangeTiGiaPheDuyet() {
    var tigia = $('#itigiapheduyet').val();
    $('#tblListTTThanhToan tbody tr').each(function (index, tr) {
        var sotiennhap = $(tr).find('#sduocduyetusd').val();
        $.ajax({
            type: "POST",
            url: "/QLNH/DeNghiThanhToan/GetChuyenDoiTyGia",
            data: { matygia: tigia, sotiennhap: parseInt(UnFormatNumber(sotiennhap)), loaitiennhap: 1 },
            async: false,
            success: function (data) {
                $(tr).find('#sduocduyetvnd').val(FormatNumber(data.chuyendoi, 0));
            }
        });
        UpdateTongThongTinThanhToan(this);
    })
}

function LoadThongTinLuyKe(type) {
    var id = $('#hidthanhtoan').val();
    var idonvi = $('#idonvi').val();
    var itenchuongtrinh = $('#itenchuongtrinh').val();
    var ichudautu = $('#ichudautu').val();
    var itenhopdong = $('#itenhopdong').val();
    var itenduan = $('#itenduan').val();

    $.ajax({
        url: "/DeNghiThanhToan/LayThongTinLuyKe",
        type: "POST",
        dataType: "json",
        async: false,
        data: {
            id: id,
            idonvi: idonvi,
            inhiemvuchi: itenchuongtrinh,
            ichudautu: ichudautu,
            ihopdong: itenhopdong,
            iduan: itenduan
        },
        success: function (result) {
            if (result.status == true) {
                if (result.thanhtoan.length > 0) {
                    var fLuyKeUSD = 0;
                    var fLuyKeVND = 0;
                    result.thanhtoan.forEach(function (item, index) {
                        fLuyKeUSD = fLuyKeUSD  + (item.fTongPheDuyet_USD == null ? 0 : item.fTongPheDuyet_USD) ;
                        fLuyKeVND = fLuyKeVND  + (item.fTongPheDuyet_VND == null ? 0 : item.fTongPheDuyet_VND);
                    })
                    if (type == 1) {
                        $('#iluykekinhphiusd').val(FormatNumber(fLuyKeUSD, 2));
                    } else if (type == 2) {
                        $('#iluykekinhphivnd').val(FormatNumber(fLuyKeVND));
                    } else {
                        $('#iluykekinhphiusd').val(FormatNumber(fLuyKeUSD, 2));
                        $('#iluykekinhphivnd').val(FormatNumber(fLuyKeVND));
                    }
                } else {
                    $('#iluykekinhphiusd').val('');
                    $('#iluykekinhphivnd').val('');
                }
            }
        }
    })
}

function checkTrungMaDeNghi() {
    var sodenghi = $('#isodenghi').val();
    var id_denghithanhtoan = $('#hidthanhtoan').val();
    if (id_denghithanhtoan == null || id_denghithanhtoan == '') {
        type_action = 0;
    } else {
        type_action = 1;
    }
    var result = false;
    $.ajax({
        type: "POST",
        dataType: "Json",
        url: "/QLNH/DeNghiThanhToan/CheckTrungMaDeNghi",
        data: { sodenghi: sodenghi, type_action: type_action, idenghi: id_denghithanhtoan },
        async: false,
        success: function (data) {
            if (data.status == true) {
                result = data.results;
            }
        }
    });
    return result;
}

function ValidateBeforeSave() {
    arrError = [];
    if ($('#idonvi').val() == "" || $('#idonvi').val() == GUID_EMPTY) {
        arrError.push('Thông tin đơn vị chưa được chọn!');
    }
    if ($.trim($('#isodenghi').val()) == "") {
        arrError.push('Thông tin số quyết định chưa được nhập!');
    }
    if ($('#iloainoidung').val() == "") {
        arrError.push('Thông tin loại nội dung chi chưa được chọn!');
    }
    if ($('#itenchuongtrinh').val() == "" || $('#itenchuongtrinh').val() == GUID_EMPTY) {
        arrError.push('Thông tin tên chương trình chưa được chọn!');
    }
    if ($('#ichudautu').val() == "" || $('#ichudautu').val() == GUID_EMPTY) {
        arrError.push('Thông tin chủ đầu tư chưa được chọn!');
    }
    if ($('#ithanhtoantheo').val() == "") {
        arrError.push('Thông tin thanh toán chưa được chọn!');
    }
    if ($('#iloaidenghi').val() == "") {
        arrError.push('Thông tin loại đề nghị chưa được chọn!');
    }
    if ($.trim($('#iquykehoach').val()) == "") {
        arrError.push('Thông tin quý kế hoạch chưa được nhập!');
    }
    if ($.trim($('#inamkehoach').val()) == "") {
        arrError.push('Thông tin năm kế hoạch chưa được nhập!');
    }
    if ($('#inamngansach').val() == "") {
        arrError.push('Thông tin năm ngân sách chưa được chọn!');
    }
    if ($('#icoquanthanhtoan').val() == "") {
        arrError.push('Thông tin cơ quan thanh toán chưa được chọn!');
    }
    if ($('#itigiadenghi').val() == "" || $('#itigiadenghi').val() == GUID_EMPTY) {
        arrError.push('Thông tin tỉ giá đề nghị chưa được chọn!');
    }
    if ($('#itigiapheduyet').val() == "" || $('#itigiapheduyet').val() == GUID_EMPTY) {
        arrError.push('Thông tin tỉ giá phê duyệt chưa được chọn!');
    }
    if ($('#itrangthai').val() == "") {
        arrError.push('Thông tin trạng thái chưa được chọn!');
    }
    if ($('#itendonvihuongthu').val() == "" || $('#itendonvihuongthu').val() == GUID_EMPTY) {
        arrError.push('Thông tin tên đơn vị hưởng thụ chưa được chọn!');
    }
    if ($.trim($('#ichuyenkhoanso').val()) == "") {
        arrError.push('Thông tin chuyển khoản bằng số chưa được nhập!');
    }
    if ($.trim($('#isodenghi').val()) != "" && $.trim($('#isodenghi').val()).length > 50) {
        arrError.push('Thông tin số đề nghị nhập quá 50 kí tự');
    }
    if ($.trim($('#ichuyenkhoanso').val()) != "" && UnFormatNumber($.trim($('#ichuyenkhoanso').val())).length > 100) {
        arrError.push('Thông tin chuyển khoản bằng số nhập quá 100 kí tự');
    }
    if ($.trim($('#isotaikhoan').val()) != "" && $.trim($('#isotaikhoan').val()).length > 100) {
        arrError.push('Thông tin số tài khoản nhập quá 100 kí tự');
    }
    if ($.trim($('#itai').val()) != "" && $.trim($('#itai').val()).length > 100) {
        arrError.push('Thông tin ngân hàng nhập quá 100 kí tự');
    }
    if ($.trim($('#ingaydenghi').val()) != "" && !dateIsValid($.trim($('#ingaydenghi').val()))) {
        arrError.push('Ngày đề nghị nhập không hợp lệ');
    }
    if (checkTrungMaDeNghi()) {
        arrError.push('Thông tin số đề nghị bị trùng');
    }
    

    if (arrError.length > 0) {
        return false;
    } else {
        return true;
    }
}

function GetData() {
    //lấy thông tin thanh toán
    tt_thanhtoan.ID = $('#hidthanhtoan').val();
    tt_thanhtoan.iID_DonVi = $('#slbDonVi').val();
    tt_thanhtoan.iID_MaPhongBan = $('#slbBQuanLy').val();
    tt_thanhtoan.iID_MaDonVi = $("<div/>").text($.trim($('#slbDonVi').find('option:selected').data('madonvi'))).html();
    tt_thanhtoan.sSoDeNghi = $("<div/>").text($.trim($('#isodenghi').val())).html();
    tt_thanhtoan.dNgayDeNghi = $('#ingaydenghi').val();
    tt_thanhtoan.iLoaiNoiDungChi = $('#iloainoidung').val();
    tt_thanhtoan.iID_KHCTBQP_NhiemVuChiID = $('#slbChuongTrinh').val();
    tt_thanhtoan.iID_ChuDauTuID = $('#ichudautu').val();
    tt_thanhtoan.iID_MaChuDauTu = $("<div/>").text($.trim($('#ichudautu').find('option:selected').data('machudautu'))).html();
    tt_thanhtoan.iThanhToanTheo = $('#ithanhtoantheo').val();
    if (tt_thanhtoan.iThanhToanTheo == 1) {
        tt_thanhtoan.iID_HopDongID = $('#itenhopdong').val();
        tt_thanhtoan.iID_DuAnID = null;
    }
    if (tt_thanhtoan.iThanhToanTheo == 2) {
        tt_thanhtoan.iID_DuAnID = $('#itenduan').val();
        tt_thanhtoan.iID_HopDongID = null;
    }
    if (tt_thanhtoan.iThanhToanTheo == 3) {
        tt_thanhtoan.iID_DuAnID = null;
        tt_thanhtoan.iID_HopDongID = null;
    }
    tt_thanhtoan.sLuyKeUSD = $('#iluykekinhphiusd').val() == "" ? "" : UnFormatNumber($('#iluykekinhphiusd').val());
    tt_thanhtoan.sLuyKeVND = $('#iluykekinhphivnd').val() == "" ? "" : UnFormatNumber($('#iluykekinhphivnd').val());
    tt_thanhtoan.iLoaiDeNghi = $('#iloaidenghi').val();
    tt_thanhtoan.iQuyKeHoach = $('#iquykehoach').val();
    tt_thanhtoan.iNamKeHoach = $('#inamkehoach').val();
    tt_thanhtoan.iNamNganSach = $('#inamngansach').val();
    tt_thanhtoan.iCoQuanThanhToan = $('#icoquanthanhtoan').val();
    tt_thanhtoan.iID_TiGiaDeNghiID = $('#itigiadenghi').val();
    tt_thanhtoan.iID_TiGiaPheDuyetID = $('#itigiapheduyet').val();
    tt_thanhtoan.iTrangThai = $('#itrangthai').val();
    tt_thanhtoan.iID_NhaThauID = $('#itendonvihuongthu').val();
    tt_thanhtoan.sChuyenKhoanBangSo = $.trim($('#ichuyenkhoanso').val()) == "" ? "" : UnFormatNumber($.trim($('#ichuyenkhoanso').val()));
    tt_thanhtoan.sChuyenKhoan_BangChu = $("<div/>").text($.trim($('#ibangchu').val())).html();
    tt_thanhtoan.sSoTaiKhoan = $("<div/>").text($.trim($('#isotaikhoan').val())).html();
    tt_thanhtoan.sNganHang = $("<div/>").text($.trim($('#itai').val())).html();
    tt_thanhtoan.sCanCu = $("<div/>").text($.trim($('#icancu').val())).html();

    //Lấy thông tin chi tiết thanh toán
    $('#tblListTTThanhToan tbody tr').each(function (index, tr) {
        var thanhtoan_chitiet = {};
        var ID = $(tr).data('idchitiet');
        var iID_MucLucNganSachID = $(tr).find('.imuclucngansach').data('ingansach');
        var iID_MLNS_ID = $(tr).find('.imuclucngansach').val();
        var sTenNoiDungChi = $("<div/>").text($.trim($(tr).find('.inoidungchi').val())).html();
        var sDeNghiCapKyNay_USD = $(tr).find('#sdenghiusd').val() == "" ? "" : UnFormatNumber($(tr).find('#sdenghiusd').val());
        var sDeNghiCapKyNay_VND = $(tr).find('#sdenghivnd').val() == "" ? "" : UnFormatNumber($(tr).find('#sdenghivnd').val());
        var sPheDuyetCapKyNay_USD = $(tr).find('#sduocduyetusd').val() == "" ? "" : UnFormatNumber($(tr).find('#sduocduyetusd').val());
        var sPheDuyetCapKyNay_VND = $(tr).find('#sduocduyetvnd').val() == "" ? "" : UnFormatNumber($(tr).find('#sduocduyetvnd').val());

        if (iID_MLNS_ID != "" || sTenNoiDungChi != "" || sDeNghiCapKyNay_USD != "" || sDeNghiCapKyNay_VND != "" || sPheDuyetCapKyNay_USD != "" || sPheDuyetCapKyNay_VND != "" || ID != "") {
            thanhtoan_chitiet.ID = ID;
            thanhtoan_chitiet.iID_MucLucNganSachID = iID_MucLucNganSachID;
            thanhtoan_chitiet.iID_MLNS_ID = iID_MLNS_ID;
            thanhtoan_chitiet.sTenNoiDungChi = sTenNoiDungChi;
            thanhtoan_chitiet.sDeNghiCapKyNay_USD = sDeNghiCapKyNay_USD;
            thanhtoan_chitiet.sDeNghiCapKyNay_VND = sDeNghiCapKyNay_VND;
            thanhtoan_chitiet.sPheDuyetCapKyNay_USD = sPheDuyetCapKyNay_USD;
            thanhtoan_chitiet.sPheDuyetCapKyNay_VND = sPheDuyetCapKyNay_VND;
            arr_thanhtoan_chitiet.push(thanhtoan_chitiet);
        }
    })
}

function SaveData() {
    if (ValidateBeforeSave()) {
        GetData();
        var data = {
            TT_ThanToan: tt_thanhtoan,
            lstTTThanToan_ChiTiet: arr_thanhtoan_chitiet
        }
        $.ajax({
            url: "/DeNghiThanhToan/SaveDeNghiThanhToan",
            type: "POST",
            data: { data: data, listIDChiTietXoa: listIDChiTietXoa.join(',') },
            success: function (r) {
                if (r == "True") {
                    window.location.href = "/QLNH/DeNghiThanhToan";
                } else {
                    arrError.push('Lỗi lưu dữ liệu');
                    showErr(ERROR);
                }
            }
        })
    } else {
        showErr(ERROR);
    }
}

function CancelSaveData() {
    window.location.href = "/QLNH/DeNghiThanhToan";
}

function ChangeBQuanLySelect() {
    var id = $("#slbChuongTrinh").val();
    $.ajax({
        type: "POST",
        url: "/QLNH/DeNghiThanhToan/GetBQuanLyTheoChuongTrinh",
        data: { id: id },
        success: function (data) {
            if (data) {
                $("#slbBQuanLy").empty().html(data.htmlQuanLy);
            }
        }
    });
}
function ChangeDonViSelect() {
    var id = $("#slbBQuanLy").val();
    $.ajax({
        type: "POST",
        url: "/QLNH/DeNghiThanhToan/GetDonViTheoBQuanLy",
        data: { id: id },
        success: function (data) {
            if (data) {
                $("#slbDonVi").empty().html(data.htmlDonVi);
            }
        }
    });
}