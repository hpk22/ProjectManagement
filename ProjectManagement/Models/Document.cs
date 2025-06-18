using System;

namespace ProjectManagement.Models
{
    public class Document
    {
        public int DocumentID { get; set; }
        public int? ProjectID { get; set; }
        public int? TaskID { get; set; }
        public string DocumentName { get; set; }
        public string FilePath { get; set; }
        public int FileSize { get; set; }        // In bytes
        public string FileType { get; set; }     // e.g., .pdf, .docx
        public int UploadedBy { get; set; }
        public DateTime UploadDate { get; set; }
        public string Description { get; set; }
    }
}
