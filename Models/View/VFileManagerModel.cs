namespace VFM.Models.View
{
    public class VFileManagerModel
    {
        public int totalNumberItems { get; set; }
        public int totalNumberPages { get; set; }
        public int currentPage { get; set; }
        public int currentNumberItems { get; private set; }

        private List<OSModel> _currentItems;
        public List<OSModel> currentItems
        {
            get
            {
                return _currentItems;
            }
            set
            {
                _currentItems = value;
                currentNumberItems = _currentItems.Count();
            }
        }
    }
}
