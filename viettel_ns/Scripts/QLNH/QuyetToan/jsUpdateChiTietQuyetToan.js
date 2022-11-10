var GUID_EMPTY = '00000000-0000-0000-0000-000000000000';
var CONFIRM = 0;
var ERROR = 1;
var errorNull = "\n";

$(document).ready(function () {
    $(".table-update tbody tr[data-rowsubmit='isData']").each(function () {
        $(this).find("input").each(function (index) {
            var checkValue = $(this).val()
            if (checkValue) {
                sumTongTable(this)
            }
        });
    });

});
function Save(isAddOrUpdate,idQuyetToan) {
    var result = [];
    $(".table-update tbody tr[data-rowsubmit='isData']").each(function () {
        var allValues = {};

        $(this).find("input").each(function (index) {
            const fieldName = $(this).attr("name");
            allValues[fieldName] = allReplace($(this).val()).replace('.', ',');

        });
        $(this).find("td").each(function (index) {
            const fieldName = $(this).data("getname");
            if (fieldName !== undefined) {
                if (fieldName == "fThuaThieuKinhPhiTrongNam_USD" || fieldName == "fThuaThieuKinhPhiTrongNam_VND"
                    || fieldName == "fQTKinhPhiDuocCap_TongSo_USD" || fieldName == "fQTKinhPhiDuocCap_TongSo_VND"
                    || fieldName == "fQTKinhPhiDuocCap_NamTruocChuyenSang_USD" || fieldName == "fQTKinhPhiDuocCap_NamTruocChuyenSang_VND"
                    || fieldName == "fQTKinhPhiDuocCap_NamNay_USD" || fieldName == "fQTKinhPhiDuocCap_NamNay_VND"
                ) {
                    allValues[fieldName] = allReplace($(this).html().replace(errorNull, "").trim()).replace('.', ',');
                } else {
                    const fieldValue = $(this).data("getvalue");
                    allValues[fieldName] = fieldValue;
                }

            }
        });
        result.push(allValues);

    })
    $.ajax({
        type: "POST",
        url: "/QLNH/QuyetToanNienDo/SaveDetail",
        data: { data: result, isAddOrUpdate: isAddOrUpdate, idQuyetToan: idQuyetToan},
        async: false,
        success: function (r) {
            if (r && r.bIsComplete) {
                window.location.href = "/QLNH/QuyetToanNienDo";
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
function onBlurSum(event, text, type, num) {
    if (ValidateInputFocusOut(event, text, type, num)) {
        disabledAll(text)
    }
}
function disabledAll(event) {
    var getClass = $(event).data("getclass");

    var getId = $(event).data("getid");
    var getParent = $(event).data("getparent");
    event.value = event.value ?? "";
    //level 1: dự án
    //level 2: hợp đồng
    //level 3: thanh toán
    disabledDataDiff(event)


    //tính tỉ giá USD-VND
    var getTiGiaMa = getClass.slice(-3);
    var getName = getClass.slice(0, -3);
    var getTiGiaChiTiet = $("#tiGiaChiTiet").val();

    let elThuaThieuUSD = $(".table-update tbody tr[data-rowsubmit='isData']").find("td[data-getid='" + getId + "'][data-getparent='" + getParent + "'][data-getclass='fThuaThieuKinhPhiTrongNam_USD']");
    let elThuaThieuVND = $(".table-update tbody tr[data-rowsubmit='isData']").find("td[data-getid='" + getId + "'][data-getparent='" + getParent + "'][data-getclass='fThuaThieuKinhPhiTrongNam_VND']");

    let elTongSoUSD = $(".table-update tbody tr[data-rowsubmit='isData']").find("td[data-getid='" + getId + "'][data-getparent='" + getParent + "'][data-getclass='fQTKinhPhiDuocCap_TongSo_USD']");
    let elTongSoVND = $(".table-update tbody tr[data-rowsubmit='isData']").find("td[data-getid='" + getId + "'][data-getparent='" + getParent + "'][data-getclass='fQTKinhPhiDuocCap_TongSo_VND']");
    let elKinhQuyetUSD = $(".table-update tbody tr[data-rowsubmit='isData']").find("input[data-getid='" + getId + "'][data-getparent='" + getParent + "'][data-getclass='KpqtUSD']");
    let elKinhQuyetVND = $(".table-update tbody tr[data-rowsubmit='isData']").find("input[data-getid='" + getId + "'][data-getparent='" + getParent + "'][data-getclass='KpqtVND']");

    let elKinhChuyenUSD = $(".table-update tbody tr[data-rowsubmit='isData']").find("input[data-getid='" + getId + "'][data-getparent='" + getParent + "'][data-getclass='KpdnUSD']");
    let elKinhChuyenVND = $(".table-update tbody tr[data-rowsubmit='isData']").find("input[data-getid='" + getId + "'][data-getparent='" + getParent + "'][data-getclass='KpdnVND']");

    if (getTiGiaMa == "VND") {
        let elInputUSD = $(".table-update tbody tr[data-rowsubmit='isData']").find("input[data-getid='" + getId + "'][data-getparent='" + getParent + "'][data-getclass='" + getName + "USD" + "']");

        let inputVND = parseFloat(UnFormatNumber($(event).val()));
        if (isNaN(inputVND)) {
            elInputUSD.val(null);
            if (getClass != 'KpnsnnUSD' && getClass != 'KpnsnnVND') {
                elThuaThieuUSD.html(null);
                elThuaThieuVND.html(null);
            }
            $(event).val(null);
            disabledDataDiff(elInputUSD)
        } else {
            $.ajax({
                type: "POST",
                data: { number: inputVND.toString(), numTiGia: getTiGiaChiTiet.toString() },
                url: '/QLNH/KeHoachChiTietBQP/CalcMoneyByTiGia',
                async: false,
                success: function (data) {
                    let result = FormatNumber(data.result, 2);
                    elInputUSD.val(result == '' ? null : result)

                    var tongUSD = elTongSoUSD.data("getvalue").trim() != "" ? elTongSoUSD.data("getvalue") : "0"
                    var tongVND = elTongSoVND.data("getvalue").trim() != "" ? elTongSoVND.data("getvalue") : "0"


                    var kinhQuyetUSD = elKinhQuyetUSD.val() != "" ? elKinhQuyetUSD.val() : "0"
                    var kinhChuyenUSD = elKinhChuyenUSD.val() != "" ? elKinhChuyenUSD.val() : "0"

                    var thuaUSD = parseFloat(allReplace(tongUSD)) - parseFloat(allReplace(kinhQuyetUSD)) - parseFloat(allReplace(kinhChuyenUSD))
                  

                    var thuaVND = parseFloat(allReplace(tongVND)) - parseFloat(allReplace(elKinhQuyetVND.val())) - parseFloat(allReplace(elKinhChuyenVND.val()))
                    if (getClass != 'KpnsnnUSD' && getClass != 'KpnsnnVND') {
                        elThuaThieuUSD.html(FormatNumber(thuaUSD, 2));
                        elThuaThieuVND.html(FormatNumber(thuaVND));
                    }

                    disabledDataDiff(elInputUSD)

                }
            });
        }
    } else {
        let elInputVND = $(".table-update tbody tr[data-rowsubmit='isData']").find("input[data-getid='" + getId + "'][data-getparent='" + getParent + "'][data-getclass='" + getName + "VND" + "']");
        let inputUSD = parseFloat(UnFormatNumber($(event).val()));
        if (isNaN(inputUSD)) {
            elInputVND.val(null);
            if (getClass != 'KpnsnnUSD' && getClass != 'KpnsnnVND') {
                elThuaThieuUSD.html(null);
                elThuaThieuVND.html(null);
            }
            disabledDataDiff(elInputVND)
            $(event).val(null);
        } else {
            $.ajax({
                type: "POST",
                data: { number: inputUSD.toString(), numTiGia: getTiGiaChiTiet.toString() },
                url: '/QLNH/KeHoachChiTietBQP/CalcMoneyByTiGia',
                async: false,
                success: function (data) {
                    let result = FormatNumber(data.result, 0);
                    elInputVND.val(result == '' ? null : result)

                    var tongUSD = elTongSoUSD.data("getvalue").trim() != "" ? elTongSoUSD.data("getvalue") : "0"
                    var tongVND = elTongSoVND.data("getvalue").trim() != "" ? elTongSoVND.data("getvalue") : "0"


                    var kinhQuyetUSD = elKinhQuyetUSD.val() != "" ? elKinhQuyetUSD.val() : "0"
                    var kinhChuyenUSD = elKinhChuyenUSD.val() != "" ? elKinhChuyenUSD.val() : "0"

                    var thuaUSD = parseFloat(allReplace(tongUSD)) - parseFloat(allReplace(kinhQuyetUSD)) - parseFloat(allReplace(kinhChuyenUSD))
                    var thuaVND = parseFloat(allReplace(tongVND)) - parseFloat(allReplace(elKinhQuyetVND.val())) - parseFloat(allReplace(elKinhChuyenVND.val()))


                    if (getClass != 'KpnsnnUSD' && getClass != 'KpnsnnVND') {
                        elThuaThieuUSD.html(FormatNumber(thuaUSD, 2));
                        elThuaThieuVND.html(FormatNumber(thuaVND, 0));
                    }
                    disabledDataDiff(elInputVND)

                }
            });
        }
    }
}
function disabledDataDiff(event) {
    var getClass = $(event).data("getclass");
    var getLevel = $(event).data("getlevel");
    var getId = $(event).data("getid");
    var tiGiaMa = getClass.slice(-3);
    var getParent = $(event).data("getparent");
    var getIdNhiemVuChi = $(event).closest("tr").find("td[data-getname='iID_KHCTBQP_NhiemVuChiID']").data("getvalue");
    var loaiNoiDungChiId = $(event).data("getLoaiNDChi");
    event.value = event.value ?? "";
    //level 1: dự án
    //level 2: hợp đồng
    //level 3: thanh toán
    if (getLevel == "1") {
        //tìm con level 2 hợp đồng để null
        $(".table-update tbody tr").find("input[data-getparent='" + getId + "'][data-getclass='" + getClass + "']").each(function () {
            const currentId = $(this).data("getid");
            if (getClass != "KpnsnnUSD" && getClass != "KpnsnnVND") {
                $(".table-update tbody tr").find("td[data-getparent='" + getId + "'][data-getclass='fThuaThieuKinhPhiTrongNam_USD']").each(function () {
                    $(this).html("");
                })
                $(".table-update tbody tr").find("td[data-getparent='" + getId + "'][data-getclass='fThuaThieuKinhPhiTrongNam_VND']").each(function () {
                    $(this).html("");
                })
                $(".table-update tbody tr").find("td[data-getparent='" + currentId + "'][data-getclass='fThuaThieuKinhPhiTrongNam_USD']").each(function () {
                    $(this).html("");
                })
                $(".table-update tbody tr").find("td[data-getparent='" + currentId + "'][data-getclass='fThuaThieuKinhPhiTrongNam_VND']").each(function () {
                    $(this).html("");
                })
            }
           
            $(this).val(null);

            //tìm con level 3 thanh toán để null
            $(".table-update tbody tr").find("input[data-getparent='" + currentId + "'][data-getclass='" + getClass + "']").each(function () {
                $(this).val("");
            })
            
        })

    } else if (getLevel == "2") {
        //tính sum tất cả level 2 
        var check = $(".table-update tbody tr").
            find("input[data-getparent='" + getParent + "'][data-getclass='" + getClass + "']").toArray()
            .reduce((partialSum, a) => {
                return partialSum + parseFloat(allReplace(a.value))
            }, 0);

        var checkLyKeUSD = $(".table-update tbody tr[data-rowsubmit='isData']").
            find("td[data-getparent='" + getParent + "'][data-getclass='fThuaThieuKinhPhiTrongNam_USD']").toArray()
            .reduce((partialSum, a) => {
                return partialSum + parseFloat(allReplace($(a).html().replace(errorNull, "").trim()))
            }, 0);
        var checkLyKeVND = $(".table-update tbody tr[data-rowsubmit='isData']").
            find("td[data-getparent='" + getParent + "'][data-getclass='fThuaThieuKinhPhiTrongNam_VND']").toArray()
            .reduce((partialSum, a) => {
                return partialSum + parseFloat(allReplace($(a).html().replace(errorNull, "").trim()))
            }, 0);


        //nếu còn thì null level 3 của level 2 đã nhập và null thừa thiếu kinh phí
        $(".table-update tbody tr").find("input[data-getparent='" + getId + "'][data-getclass='" + getClass + "']").each(function () {
            $(this).val("");
        })
        if (getClass != "KpnsnnUSD" && getClass != "KpnsnnVND") {
            $(".table-update tbody tr").find("td[data-getparent='" + getId + "'][data-getclass='fThuaThieuKinhPhiTrongNam_USD']").each(function () {
                $(this).html("");
            })
            $(".table-update tbody tr").find("td[data-getparent='" + getId + "'][data-getclass='fThuaThieuKinhPhiTrongNam_VND']").each(function () {
                $(this).html("");
            })
        }
        
        //sum level 1 của level 2 đã nhập 
        $(".table-update tbody tr").find("input[data-getid='" + getParent + "'][data-getclass='" + getClass + "']").val(tiGiaMa == "VND" ? FormatNumber(check, 0) : FormatNumber(check, 2));
        var isDataRowCha = $(".table-update tbody tr").find("input[data-getid='" + getParent + "'][data-getclass='" + getClass + "']").closest("tr").data("rowsubmit");
        if (isDataRowCha == "isNotData") {
            $(".table-update tbody tr").find("input[data-getid='" + getParent + "'][data-getclass='" + getClass + "']").next("span").html(tiGiaMa == "VND" ? FormatNumber(check, 0) : FormatNumber(check, 2))
        }
        $(".table-update tbody tr").find("td[data-getid='" + getParent + "'][data-getclass='fThuaThieuKinhPhiTrongNam_USD']").html(FormatNumber(checkLyKeUSD, 2));
        $(".table-update tbody tr").find("td[data-getid='" + getParent + "'][data-getclass='fThuaThieuKinhPhiTrongNam_VND']").html(FormatNumber(checkLyKeVND, 0));
    } else {
        //sum all level 4 của level 3 đấy còn value ko

        var check = $(".table-update tbody tr[data-rowsubmit='isData']").
            find("input[data-getparent='" + getParent + "'][data-getclass='" + getClass + "']").toArray()
            .reduce((partialSum, a) => {
                return partialSum + parseFloat(allReplace(a.value))
            }, 0);

        var checkLuyKeUSD = $(".table-update tbody tr[data-rowsubmit='isData']").
            find("td[data-getparent='" + getParent + "'][data-getclass='fThuaThieuKinhPhiTrongNam_USD']").toArray()
            .reduce((partialSum, a) => {
                return partialSum + parseFloat(allReplace($(a).html().replace(errorNull, "").trim()))
            }, 0);
        var checkLuyKeVND = $(".table-update tbody tr[data-rowsubmit='isData']").
            find("td[data-getparent='" + getParent + "'][data-getclass='fThuaThieuKinhPhiTrongNam_VND']").toArray()
            .reduce((partialSum, a) => {
                return partialSum + parseFloat(allReplace($(a).html().replace(errorNull, "").trim()))
            }, 0);
        let parrentId = "";
        var sumGran = 0;
        var sumGranLuyeKeUSD = 0;
        var sumGranLuyeKeVND = 0;
        //tính sum all level 3 cho level 2
        $(".table-update tbody tr").find("input[data-getid='" + getParent + "'][data-getloaindchi='" + loaiNoiDungChiId + "'][data-getclass='" + getClass + "']").each(function () {
            parrentId = $(this).data("getparent");
            var isData = $(this).closest("tr").data("rowsubmit");
            if (isData == "isNotData") {
                $(this).next("span").html(tiGiaMa == "VND" ? FormatNumber(check, 0) : FormatNumber(check, 2))
            }
            $(this).val(tiGiaMa == "VND" ? FormatNumber(check, 0) : FormatNumber(check, 2));
            $(".table-update tbody tr").find("input[data-getparent='" + parrentId + "'][data-getloaindchi='" + loaiNoiDungChiId + "'][data-getclass='" + getClass + "']").each(function () {
                sumGran += parseFloat(allReplace($(this).val()))
            })

        })

        $(".table-update tbody tr").find("td[data-getid='" + getParent + "'][data-getloaindchi='" + loaiNoiDungChiId + "'][data-getclass='fThuaThieuKinhPhiTrongNam_USD']").each(function () {
            parrentId = $(this).data("getparent");
            $(this).html(FormatNumber(checkLuyKeUSD, 2));

            $(".table-update tbody tr").find("td[data-getparent='" + parrentId + "'][data-getloaindchi='" + loaiNoiDungChiId + "'][data-getclass='fThuaThieuKinhPhiTrongNam_USD']").each(function () {
                sumGranLuyeKeUSD += parseFloat(allReplace($(this).html().replace(errorNull, "").trim()))
            })
        })
        $(".table-update tbody tr").find("td[data-getid='" + getParent + "'][data-getloaindchi='" + loaiNoiDungChiId + "'][data-getclass='fThuaThieuKinhPhiTrongNam_VND']").each(function () {
            parrentId = $(this).data("getparent");
            $(this).html(FormatNumber(checkLuyKeVND, 0));
            $(".table-update tbody tr").find("td[data-getparent='" + parrentId + "'][data-getloaindchi='" + loaiNoiDungChiId + "'][data-getclass='fThuaThieuKinhPhiTrongNam_VND']").each(function () {
                sumGranLuyeKeVND += parseFloat(allReplace($(this).html().replace(errorNull, "").trim()))
            })
        })

        //gán sum cho lv 2
        $(".table-update tbody tr").find("input[data-getid='" + parrentId + "'][data-getloaindchi='" + loaiNoiDungChiId + "'][data-getclass='" + getClass + "']").val(tiGiaMa == "VND" ? FormatNumber(sumGran, 0) : FormatNumber(sumGran, 2));
        var isDataRowCha = $(".table-update tbody tr").find("input[data-getid='" + parrentId + "'][data-getloaindchi='" + loaiNoiDungChiId + "'][data-getclass='" + getClass + "']").closest("tr").data("rowsubmit");
        if (isDataRowCha == "isNotData") {
            $(".table-update tbody tr").find("input[data-getid='" + parrentId + "'][data-getloaindchi='" + loaiNoiDungChiId + "'][data-getclass='" + getClass + "']").next("span").html(tiGiaMa == "VND" ? FormatNumber(sumGran, 0) : FormatNumber(sumGran, 2))
        }
        $(".table-update tbody tr").find("td[data-getid='" + parrentId + "'][data-getloaindchi='" + loaiNoiDungChiId + "'][data-getclass='fThuaThieuKinhPhiTrongNam_USD']").html(FormatNumber(sumGranLuyeKeUSD, 2));
        $(".table-update tbody tr").find("td[data-getid='" + parrentId + "'][data-getloaindchi='" + loaiNoiDungChiId + "'][data-getclass='fThuaThieuKinhPhiTrongNam_VND']").html(FormatNumber(sumGranLuyeKeVND, 0));

        //tính sum all level 3 cho level 1
        $(".table-update tbody tr").find("input[data-getid='" + getParent + "'][data-getclass='" + getClass + "']").each(function () {
            parrentId = $(this).data("getparent");
            var isData = $(this).closest("tr").data("rowsubmit");
            if (isData == "isNotData") {
                $(this).next("span").html(tiGiaMa == "VND" ? FormatNumber(check, 0) : FormatNumber(check, 2))
            }
            $(this).val(tiGiaMa == "VND" ? FormatNumber(check, 0) : FormatNumber(check, 2));
            $(".table-update tbody tr").find("input[data-getparent='" + parrentId + "'][data-getclass='" + getClass + "']").each(function () {
                sumGran += parseFloat(allReplace($(this).val()))
            })

        })

        $(".table-update tbody tr").find("td[data-getid='" + getParent + "'][data-getclass='fThuaThieuKinhPhiTrongNam_USD']").each(function () {
            parrentId = $(this).data("getparent");
            $(this).html(FormatNumber(checkLuyKeUSD, 2));

            $(".table-update tbody tr").find("td[data-getparent='" + parrentId + "'][data-getclass='fThuaThieuKinhPhiTrongNam_USD']").each(function () {
                sumGranLuyeKeUSD += parseFloat(allReplace($(this).html().replace(errorNull, "").trim()))
            })
        })
        $(".table-update tbody tr").find("td[data-getid='" + getParent + "'][data-getclass='fThuaThieuKinhPhiTrongNam_VND']").each(function () {
            parrentId = $(this).data("getparent");
            $(this).html(FormatNumber(checkLuyKeVND, 0));
            $(".table-update tbody tr").find("td[data-getparent='" + parrentId + "'][data-getclass='fThuaThieuKinhPhiTrongNam_VND']").each(function () {
                sumGranLuyeKeVND += parseFloat(allReplace($(this).html().replace(errorNull, "").trim()))
            })
        })

        //gán sum cho lv 1
        $(".table-update tbody tr").find("input[data-getid='" + parrentId + "'][data-getclass='" + getClass + "']").val(tiGiaMa == "VND" ? FormatNumber(sumGran, 0) : FormatNumber(sumGran, 2));
        var isDataRowCha = $(".table-update tbody tr").find("input[data-getid='" + parrentId + "'][data-getclass='" + getClass + "']").closest("tr").data("rowsubmit");
        if (isDataRowCha == "isNotData") {
            $(".table-update tbody tr").find("input[data-getid='" + parrentId + "'][data-getclass='" + getClass + "']").next("span").html(tiGiaMa == "VND" ? FormatNumber(sumGran, 0) : FormatNumber(sumGran, 2))
        }
        $(".table-update tbody tr").find("td[data-getid='" + parrentId + "'][data-getclass='fThuaThieuKinhPhiTrongNam_USD']").html(FormatNumber(sumGranLuyeKeUSD, 2));
        $(".table-update tbody tr").find("td[data-getid='" + parrentId + "'][data-getclass='fThuaThieuKinhPhiTrongNam_VND']").html(FormatNumber(sumGranLuyeKeVND, 0));
    }
    var dongCha = $(".table-update tbody tr[data-getlevel='0'][data-getparentid='" + getIdNhiemVuChi + "']");
    var setValue = {};
    //sum nhiem vu chi
    $(".table-update tbody tr[data-getsumid='1']").find("input[data-getgrandparent='" + getIdNhiemVuChi + "']").each(function () {
        const fieldName = $(this).attr("name");
        setValue[fieldName] = (setValue[fieldName] == undefined ? 0 : setValue[fieldName]) + parseFloat(allReplace($(this).val()));

    })
    $(".table-update tbody tr[data-getsumid='1']").find("td[data-getgrandparent='" + getIdNhiemVuChi + "']").each(function () {
        const fieldName = $(this).data("getname");
        setValue[fieldName] = (setValue[fieldName] == undefined ? 0 : setValue[fieldName]) + parseFloat(allReplace($(this).html().replace(errorNull, "").trim()));

    })
    $(dongCha).find("td").each(function () {

        var fieldNameInput = $(this).find("input").attr("name");
        var tiGiaMaInput = fieldNameInput != undefined ? fieldNameInput.slice(-3) : "";
        var fieldName = $(this).data("getname");
        var tiGiaMa = fieldName != undefined ? fieldName.slice(-3) : "";

        if (fieldName == "fThuaThieuKinhPhiTrongNam_USD" || fieldName == "fThuaThieuKinhPhiTrongNam_VND") {
            $(this).html(tiGiaMa == "VND" ? FormatNumber(setValue[fieldName], 0) : FormatNumber(setValue[fieldName], 2))
        } else {
            $(this).find("input").next("span").html(tiGiaMaInput == "VND" ? FormatNumber(setValue[fieldNameInput],0) : FormatNumber(setValue[fieldNameInput],2))
        }
    })
}
function sumTongTable(event) {
    var getParent = $(event).data("getparent");
    var getClass = $(event).data("getclass");
    var loaiNoiDungChiId = $(event).data("getloaindchi");
    var getIdNhiemVuChi = $(event).closest("tr").find("td[data-getname='iID_KHCTBQP_NhiemVuChiID']").data("getvalue");
    event.value = event.value ?? "";
    var dongCha = $(".table-update tbody tr[data-getlevel='0'][data-getparentid='" + getIdNhiemVuChi + "']");
    var setValue = {};
    var tiGiaMa = getClass.slice(-3);

    var check = $(".table-update tbody tr[data-rowsubmit='isData']").
        find("input[data-getparent='" + getParent + "'][data-getclass='" + getClass + "']").toArray()
        .reduce((partialSum, a) => {
            return partialSum + parseFloat(allReplace(a.value))
        }, 0);

    var checkLuyKeUSD = $(".table-update tbody tr[data-rowsubmit='isData']").
        find("td[data-getparent='" + getParent + "'][data-getclass='fThuaThieuKinhPhiTrongNam_USD']").toArray()
        .reduce((partialSum, a) => {
            return partialSum + parseFloat(allReplace($(a).html().replace(errorNull, "").trim()))
        }, 0);
    var checkLuyKeVND = $(".table-update tbody tr[data-rowsubmit='isData']").
        find("td[data-getparent='" + getParent + "'][data-getclass='fThuaThieuKinhPhiTrongNam_VND']").toArray()
        .reduce((partialSum, a) => {
            return partialSum + parseFloat(allReplace($(a).html().replace(errorNull, "").trim()))
        }, 0);

    //tính sum all level 4 cho level 3
    $(".table-update tbody tr").find("input[data-getid='" + getParent + "'][data-getclass='" + getClass + "']").each(function () {
        var isData = $(this).closest("tr").data("rowsubmit");
        if (isData == "isNotData") {
            $(this).next("span").html(tiGiaMa == "VND" ? FormatNumber(check, 0) : FormatNumber(check, 2))
        }
        $(this).val(tiGiaMa == "VND" ? FormatNumber(check, 0) : FormatNumber(check, 2));
    })

    $(".table-update tbody tr").find("td[data-getid='" + getParent + "'][data-getclass='fThuaThieuKinhPhiTrongNam_USD']").each(function () {
        $(this).html(FormatNumber(checkLuyKeUSD, 2));
    })
    $(".table-update tbody tr").find("td[data-getid='" + getParent + "'][data-getclass='fThuaThieuKinhPhiTrongNam_VND']").each(function () {
        $(this).html(FormatNumber(checkLuyKeVND, 0));
    })
    //sum nhiem vu chi
    $(".table-update tbody tr[data-getsumid='1']").find("input[data-getgrandparent='" + getIdNhiemVuChi + "']").each(function () {
        const fieldName = $(this).attr("name");
        setValue[fieldName] = (setValue[fieldName] == undefined ? 0 : setValue[fieldName]) + parseFloat(allReplace($(this).val()));

    })
    $(".table-update tbody tr[data-getsumid='1']").find("td[data-getgrandparent='" + getIdNhiemVuChi + "']").each(function () {
        const fieldName = $(this).data("getname");
        setValue[fieldName] = (setValue[fieldName] == undefined ? 0 : setValue[fieldName]) + parseFloat(allReplace($(this).html().replace(errorNull, "").trim()));

    })

    $(dongCha).find("td").each(function () {

        var fieldNameInput = $(this).find("input").attr("name");
        var tiGiaMaInput = fieldNameInput != undefined ? fieldNameInput.slice(-3) : "";
        var fieldName = $(this).data("getname");
        var tiGiaMa = fieldName != undefined ? fieldName.slice(-3) : "";

        if (fieldName == "fThuaThieuKinhPhiTrongNam_USD" || fieldName == "fThuaThieuKinhPhiTrongNam_VND") {
            $(this).html(tiGiaMa == "VND" ? FormatNumber(setValue[fieldName], 0) : FormatNumber(setValue[fieldName], 2))
        } else {
            $(this).find("input").next("span").html(tiGiaMaInput == "VND" ? FormatNumber(setValue[fieldNameInput], 0) : FormatNumber(setValue[fieldNameInput], 2))
        }
    })
}
function allReplace(obj) {
    var secondNum = "";
    if (obj != null && obj != "") {
        var firstNum = obj.toString().replace(/[.]/g, '');
        secondNum = firstNum.replace(/[,]/g, '.');
    } else {
        secondNum = "0"
    }
    return secondNum;
};
function Cancel() {
    window.location.href = "/QLNH/QuyetToanNienDo";
}
