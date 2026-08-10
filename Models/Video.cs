using System;
using System.ComponentModel.DataAnnotations;

namespace GloboconSoftwareWeb.Models
{
    public class Video
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ThumbnailPath { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        public int DisplayOrder { get; set; } = 0;

        public bool IsPortrait { get; set; } = false;

        public bool IsMalayalam { get; set; } = false;
    }
}
