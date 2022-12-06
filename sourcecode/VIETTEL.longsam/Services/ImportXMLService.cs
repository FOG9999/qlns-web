using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;
using Viettel.Models.BaoHiemXaHoi;

namespace Viettel.Services
{
    public class ImportXMLService
    {
        #region Process

        #endregion

        #region Helper 
        public bool ValidateTypeData(int iLine, Guid iIdImportId, ValidateBhxhModel validate, string value, ref DataTable lstError)
        {
            if (validate.bIsNotNull && string.IsNullOrEmpty(value))
            {
                InsertError(iLine, iIdImportId, validate.sLabelName, "Không được để trống.", ref lstError);
                return false;
            }

            switch (validate.iType)
            {
                case (int)Constants.BaoHiemXHType.ITypes.String:
                    if (!string.IsNullOrEmpty(value) && validate.iMaxLength < value.Length)
                    {
                        InsertError(iLine, iIdImportId, validate.sLabelName, "Vuợt quá số kí tự.", ref lstError);
                        return false;
                    }
                    break;
                case (int)Constants.BaoHiemXHType.ITypes.Int:
                    int iConvert = 0;
                    if (!string.IsNullOrEmpty(value) && !int.TryParse(value, out iConvert))
                    {
                        InsertError(iLine, iIdImportId, validate.sLabelName, "Sai format.", ref lstError);
                        return false;
                    }
                    break;
                case (int)Constants.BaoHiemXHType.ITypes.Date:
                    if(!string.IsNullOrEmpty(value) && !CheckDateTime(value))
                    {
                        InsertError(iLine, iIdImportId, validate.sLabelName, "Sai ngày tháng.", ref lstError);
                        return false;
                    }
                    break;
            }
            return true;
        }

        private bool CheckDateTime(string dateTime)
        {
            if (string.IsNullOrEmpty(dateTime) || dateTime.Length != 8)
                return false;
            DateTime datePare = new DateTime();
            string year = dateTime.Substring(0, 4);
            string month = dateTime.Substring(4, 2);
            string day = dateTime.Substring(6, 2);
            if (DateTime.TryParse(string.Format("{0}/{1}/{2}", year, month, day), out datePare))
                return true;
            return false;
        }

        private void InsertError(int line, Guid iIdImportId, string sPropertyName, string sMessage, ref DataTable dtError)
        {
            DataRow dr = dtError.NewRow();
            dr["iLine"] = line;
            dr["iID_ImportID"] = iIdImportId;
            dr["sPropertyName"] = sPropertyName;
            dr["sMessage"] = sMessage;
            dtError.Rows.Add(dr);
        }
        #endregion
    }
}
