namespace Task.API.Entities
{
    public class File
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public byte[]? FileContent { get; set; }
        public DateTime UploadTime { get; set; }
    }
}
