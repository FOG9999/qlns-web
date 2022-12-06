using System.ComponentModel;

namespace VIETTEL.Models
{
    public class LNS
    {
        public const string NSQP = "1";
        public const string NSNN = "2";
        public const string NSDB = "3";
        public const string NSK = "4";

        public const string NSQP_Text = "nsqp";
        public const string NSNN_Text = "nsnn";
        public const string NSDB_Text = "nsdb";
        public const string NSK_Text = "nsk";
    }

    public enum LNSType
    {
        [Description("nsqp")]
        NSQP = 1,

        [Description("nsnn")]
        NSNN = 2,

        [Description("nsdb")]
        NSDB = 3,

        [Description("nsk")]
        NSK = 4,

        [Description("tn")]
        TN = 8
    }
}
