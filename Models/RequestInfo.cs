

namespace FalcockDownloaderWebSite.Models
{
    public class RequestInfo
    {
        public string URL { get; set; }

        public Format Format { get; set; }

        public string PathDirec { get; set; }

    }

    public enum Format
    {
        Mp3,
        Mp4,
        Wav,
        Mov

    }
}
