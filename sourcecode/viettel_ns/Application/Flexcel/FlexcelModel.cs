using FlexCel.Report;
using System;
using Viettel.Services;
using VIETTEL.Flexcel;

namespace VIETTEL.Application.Flexcel
{
    public class FlexcelModel
    {
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Now { get; set; }
        public int dvt { get; set; }

        public string TinhThanh { get; set; }

        public string header { get; set; }
        public string header1 { get; set; }
        public string header2 { get; set; }

        public string footer { get; set; }
        public string footer1 { get; set; }
        public string footer2 { get; set; }


        public FlexcelModel()
        {
            var now = DateTime.Now;
            var formatDate = "Ngày   {0}   tháng   {1}   năm {2}";
            var ngay = string.Format(formatDate, "......", "......", now.Year);

            Ngay = ngay;
            Thang = string.Format(formatDate, "......", now.Month, now.Year);
            Now = string.Format("Ngày {0} tháng {1} năm {2}", now.Day, now.Month, now.Year);
            TinhThanh = LocalizationService.Default.Translate("TinhThanh");



            dvt = 1000;
        }

        public void ToFlexcel(FlexCelReport fr)
        {
            fr.SetValue(this.GetPropertyName(() => Ngay), Ngay);
            //fr.SetValue(this.GetPropertyName(() => ngay), ngay);
            fr.SetValue(this.GetPropertyName(() => Thang), Thang);
            fr.SetValue(this.GetPropertyName(() => Now), Now);
            fr.SetValue("Minute", DateTime.Now.ToStringMinute());
            fr.SetValue(this.GetPropertyName(() => dvt), getDvtText(dvt));
            fr.SetValue(this.GetPropertyName(() => TinhThanh), TinhThanh);

            fr.SetValue("NgayIn", $"Ngày in: {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}");

            fr.SetValue(this.GetPropertyName(() => header), header);
            fr.SetValue(this.GetPropertyName(() => header1), header1);
            fr.SetValue(this.GetPropertyName(() => header2), header2);
            fr.SetValue(this.GetPropertyName(() => footer), footer);
            fr.SetValue(this.GetPropertyName(() => footer1), footer1);
            fr.SetValue(this.GetPropertyName(() => footer2), footer2);

            fr.SetValue(new
            {
                NgayThang = $"Ngày {DateTime.Now.Day.ToString("0#")} tháng {DateTime.Now.Month.ToString("0#")} năm {DateTime.Now.Year}",
            });
        }

        private string getDvtText(int dvt)
        {
            var dvtText = "đồng";
            if (dvt == 1000)
            {
                dvtText = "1.000 đồng";
            }
            else if (dvt == 1000000)
            {
                dvtText = "triệu đồng";
            }
            else if (dvt == 1000000000)
            {
                dvtText = "tỷ đồng";
            }
            return dvtText;
        }

    }
}
