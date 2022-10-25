namespace Viettel.Domain.Json
{
    public class js_msg
    {
        public bool success { get; set; }
        public string title { get; set; }
        public string text { get; set; }

        /// <summary>
        /// success, info, warning, error | danger
        /// </summary>
        public string type { get; set; }
    }
}
