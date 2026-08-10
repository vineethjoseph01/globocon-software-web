using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GloboconSoftwareWeb.ViewModels
{
    public class VideoUploadViewModel
    {
        [Required(ErrorMessage = "Please enter a video title.")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select an MP4 video file.")]
        public IFormFile VideoFile { get; set; } = null!;

        public int DisplayOrder { get; set; } = 0;

        public bool IsPortrait { get; set; } = false;

        public bool IsMalayalam { get; set; } = false;
    }

    public class VideoEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a video title.")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string ExistingFilePath { get; set; } = string.Empty;

        public string? ExistingThumbnailPath { get; set; }

        public IFormFile? NewVideoFile { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsPortrait { get; set; } = false;

        public bool IsMalayalam { get; set; } = false;
    }
}
