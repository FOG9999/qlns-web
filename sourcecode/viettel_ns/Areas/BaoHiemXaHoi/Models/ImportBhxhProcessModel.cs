using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Xml;
using Viettel.Domain.DomainModel;
using Viettel.Models.BaoHiemXaHoi;
using Viettel.Services;

namespace VIETTEL.Areas.BaoHiemXaHoi.Models
{
    public class ImportBhxhProcessModel
    {
        private ImportXMLService _xmlService = new ImportXMLService();
        private readonly IBaoHiemXaHoiService _bHXHService = BaoHiemXaHoiService.Default;

        public void ProcessImport(int iNamLamViec, ImportBHXHModel data, string sFilePath, ref bool bIsErrorDonVi)
        {

            if (!Directory.Exists(sFilePath))
            {
                Directory.CreateDirectory(sFilePath);
            }
            string sFileReader = Path.Combine(sFilePath, data.sFileNameServer);
            if (string.IsNullOrEmpty(data.sFileNameServer) || !File.Exists(sFileReader)) return;

            List<BHXH_BenhNhanTemp> lstOutput = new List<BHXH_BenhNhanTemp>();
            var lstProperty = typeof(BHXH_BenhNhanTemp).GetProperties();
            Dictionary<string, ValidateBhxhModel> dicValidate = ValidateBhxhModel.GetValidateBhxh().ToDictionary(n => n.sLabelName, n => n);
            List<BHXH_BenhNhanError> lstError = new List<BHXH_BenhNhanError>();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(sFileReader);
            XmlNodeList rowNodes = xmlDocument.SelectNodes("NewDataSet");

            DataTable tableBNTemp = new DataTable();
            tableBNTemp.TableName = "BHXH_BenhNhanTemp";
            foreach(var column in dicValidate.Values)
            {
                tableBNTemp.Columns.Add(column.sPropertyName, typeof(string));
            }
            tableBNTemp.Columns.Add(nameof(BHXH_BenhNhanTemp.iID_BenhNhanID), typeof(Guid));
            tableBNTemp.Columns.Add(nameof(BHXH_BenhNhanTemp.iID_ImportID), typeof(Guid));
            tableBNTemp.Columns.Add(nameof(BHXH_BenhNhanTemp.bError), typeof(bool));
            tableBNTemp.Columns.Add(nameof(BHXH_BenhNhanTemp.iLine), typeof(int));

            DataTable tableBNError = new DataTable();
            tableBNError.TableName = "BHXH_BenhNhanError";
            tableBNError.Columns.Add(nameof(BHXH_BenhNhanError.iID_ImportID), typeof(Guid));
            tableBNError.Columns.Add(nameof(BHXH_BenhNhanError.iLine), typeof(int));
            tableBNError.Columns.Add(nameof(BHXH_BenhNhanError.sPropertyName), typeof(string));
            tableBNError.Columns.Add(nameof(BHXH_BenhNhanError.sMessage), typeof(string));
            
            foreach (XmlNode row in rowNodes)
            {
                int iLine = 1;
                var nodeChild = row.ChildNodes;
                foreach (XmlNode rowC in nodeChild)
                {
                    if (rowC.Name != "DLKCB") continue;

                    DataRow dr = tableBNTemp.NewRow();
                    bool isError = false;

                    foreach (var keyValidate in dicValidate.Keys)
                    {
                        try
                        {
                            var validateItem = dicValidate[keyValidate];
                            var itemValue = (rowC[keyValidate] == null ? string.Empty : rowC[keyValidate].InnerText).Trim();
                            if (!_xmlService.ValidateTypeData(iLine, data.iID_ImportID, validateItem, itemValue, ref tableBNError))
                            {
                                isError = true;
                            }
                            dr[validateItem.sPropertyName] = itemValue;
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    dr["iLine"] = iLine;
                    dr["bError"] = isError;
                    dr["iID_ImportID"] = data.iID_ImportID;
                    tableBNTemp.Rows.Add(dr);
                    iLine++;
                }
            }
            var result = _bHXHService.SaveBenhNhanTempAndError(iNamLamViec, data.iThang, data.iID_ImportID, tableBNTemp, tableBNError, ref bIsErrorDonVi);
        }


    }
}