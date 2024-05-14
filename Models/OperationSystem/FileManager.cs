namespace VFM.Models.OperationSystem
{
    public class FileManager
    {
        public string icon { get; set; }
        public string fileName { get; set; }
        public string fullPath { get; set; }
        public string dateCreate { get; set; }
        public string dateChange { get; set; }
        public string size { get; set; }
        public bool isFile { get; set; }
    }
}
