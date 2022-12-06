namespace VIETTEL.Models
{
    public class KeyViewModel<TValue, TKey>
        where TValue : class
        where TKey : class
    {
        public KeyViewModel(TValue value, string text)
        {
            Value = value;
            Text = text;

        }
        public KeyViewModel(TValue value, TKey key, string text) :
            this(value, text)
        {
            Key = key;
        }

        public TValue Value { get; set; }
        public TKey Key { get; set; }

        public string Text { get; set; }
    }
    public class KeyViewModel : KeyViewModel<string, string>
    {
        public KeyViewModel(string key, string text) : base(key, text)
        {
        }
        public KeyViewModel(string value, string key, string text) : base(value, key, text)
        {
        }
    }




}
