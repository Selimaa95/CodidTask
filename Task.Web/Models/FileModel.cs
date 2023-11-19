﻿namespace Task.Web.Models
{
    public class FileModel
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public byte[]? FileContent { get; set; }
        public DateTime UploadTime { get; set; }
    }
}
