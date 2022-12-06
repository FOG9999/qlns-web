using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.BaoHiemXaHoi
{
    public class NewDataSet
    {
        public List<DLKCB> DLKCB { get; set; }
    }

    public class DLKCB
    {
        public string STT { get; set; }
        public string ho_ten { get; set; }
        public string NGAYSINH { get; set; }
        public string GIOITINH { get; set; }
        public string CAPBAC { get; set; }
        public string CHUCVU { get; set; }
        public string MATHE { get; set; }
        public string NGAY_VAO_VIEN { get; set; }
        public string NGAY_RA_VIEN { get; set; }
        public int SO_NGAY_DTRI { get; set; }
        public string MABV { get; set; }
        public string TENBV { get; set; }
        public string MADV { get; set; }
        public string TENDV { get; set; }
        public string GHICHU { get; set; }
    }
}
