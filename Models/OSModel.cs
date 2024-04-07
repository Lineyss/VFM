namespace VFM.Models
{
    public class OSModel
    {
        public string icon { get; set; }
        public string fileName { get; set; }
        public string fullPath { get; set; }
        public string dateCreate { get; set; }
        public string dateChange { get; set; }

        private long _size;
        public long size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
                isEmpty = _size <= 0;
            }
        }
        public bool isEmpty { get; set; }
    }
}
