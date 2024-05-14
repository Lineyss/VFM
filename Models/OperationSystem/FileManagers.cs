namespace VFM.Models.OperationSystem
{
    public class FileManagers
    {
        public int totalNumberItems { get; set; }
        public int totalNumberPages { get; set; }
        public int currentPage { get; set; }
        public int currentNumberItems { get; private set; }

        private List<FileManager> _currentItems;
        public List<FileManager> currentItems
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
