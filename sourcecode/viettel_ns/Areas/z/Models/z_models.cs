using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
using Viettel.Domain.DomainModel;

namespace Viettel.Domain.DomainModel
{
    /// <summary>
    /// A class which represents the Z_ChungTu table.
    /// </summary>
    /*[Table("Z_ChungTu")]*/
    public partial class Z_ChungTu : NObject
    {

        public Z_ChungTu()
        {
            Z_File = new List<Z_File>();
        }


        [Key]
        public virtual Guid Id { get; set; }
        public virtual string Id_DonVi { get; set; }
        public virtual string Id_PhongBan { get; set; }
        public virtual string Id_DonVi_Ten { get; set; }
        public virtual string Id_DonVi_Nguon { get; set; }
        public virtual string Id_DonVi_Nguon_Ten { get; set; }
        public virtual int NamLamViec { get; set; }
        public virtual int SoChungTu { get; set; }
        public virtual DateTime NgayChungTu { get; set; }

        public virtual string Key { get; set; }

        public virtual string NoiDung { get; set; }
        public virtual string MoTa { get; set; }

        public virtual float TuChi { get; set; }
        public virtual string Json { get; set; }

        public virtual int iLoai { get; set; }
        public virtual string iLoai_MoTa { get; set; }
        public virtual bool Locked { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual string UserCreator { get; set; }
        public virtual DateTime? DateModified { get; set; }
        public virtual string IpModified { get; set; }
        public virtual string UserModifier { get; set; }
        public virtual int? AuditCount { get; set; }
        public virtual string AuditLog { get; set; }

        public virtual IEnumerable<Z_File> Z_File { get; set; }
    }


    /// <summary>
    /// A class which represents the Z_File table.
    /// </summary>
    /*[Table("Z_File")]*/
    public partial class Z_File : NObject
    {
        [Key]
        public virtual Guid Id { get; set; }
        public virtual Guid? Id_ChungTu { get; set; }
        public virtual string MoTa { get; set; }
        public virtual int iLoai { get; set; }
        public virtual byte[] FileData { get; set; }
        //public virtual string FileDataString { get; set; }

        public virtual string FileExt { get; set; }
        public virtual string FileName { get; set; }
        public virtual int? FileSize { get; set; }
        public virtual string FileSizeText => FileSize.GetValueOrDefault().ToStringBytes();

        public virtual string FilePath { get; set; }
    }

    public enum zType
    {

        // so kiem tra
        SoKiemTra = 10,
        SoKiemTra_DuToan = 11,

        // du toan
        DuToan = 1,

        // quyet toan
        QuyetToan = 2,

    }
}
