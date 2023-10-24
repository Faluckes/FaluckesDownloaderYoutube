using FalcockDownloaderWebSite.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Diagnostics;
using System.Media;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Tweetinvi;
using Tweetinvi.Parameters;
using Tweetinvi.Models;
using System.Net;

namespace FalcockDownloaderWebSite.Controllers
{


    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;


        public HomeController(IWebHostEnvironment env)
        {
            _env = env;
        }

        #region Pages
        public IActionResult Index()
        {



            Log.Information("Abrindo Download de video...");


            //BORA TRATAR ESSA EXCEÇÕES PARCEIRO!!!!!!
            //BORA TRATAR ESSA EXCEÇÕES PARCEIRO!!!!!!


            RequestInfo request = new RequestInfo();
            request.URL = "";

            return View(request);
        }
        public IActionResult Twitter()
        {
            RequestInfo request = new RequestInfo();
            request.URL = "";
            Log.Information("Abrindo Twitter!");
            return View(request);
        }
        public IActionResult Instagram()
        {
            Log.Information("Abrindo Instagram!");
            return View("Instagram");
        }
        public IActionResult Facebook()
        {
            Log.Information("Abrindo Facebook!");
            return View("Facebook");
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Converter()
        {
            Log.Information("Abrindo Converter!");
            return View();
        }
        #endregion


        #region CodesDownload

        [HttpPost]
        public async Task<IActionResult> Salvar(RequestInfo request, string caminho)
        {
            if (string.IsNullOrEmpty(request.URL))
            {
                var error = new ArgumentNullException(request.URL);
                Log.Fatal(error.Message);
            }
            try
            {
                var youtube = new YoutubeClient();
                Log.Information("Tentando obter a URL...");
                var video = await youtube.Videos.GetAsync(request.URL);
                var title = video.Title.Replace("|", "").Replace("?", "");
                Log.Information($"Nome do vídeo: {title}");
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(request.URL);
                Log.Information("Obtendo Informações do vídeo para inicializar o Download!");
                string Url = request.URL;

                // Expressão regular para extrair o ID do vídeo
                Regex regex = new Regex(@"^.*(?:youtube.com\/|v\/|u\/\w\/|embed\/|watch\?v=)([^#\&\?]*).*");

                // Executa a expressão regular na URL do vídeo
                Match match = regex.Match(Url);

                // Obtém o ID do vídeo a partir da correspondência encontrada
                string videoId = match.Groups[1].Value;
                Log.Information($"Id do vídeo: {videoId}");

                var embedUrl = $"https://youtube.com/embed/{videoId}";
                Log.Information($"Url do Embed: {embedUrl}");
                string embedCode = $"<iframe id=\"videoYt\" width=\"560\" height=\"315\" src=\"{embedUrl}\" frameborder=\"0\" allowfullscreen></iframe>";

                ViewBag.EmbedCode = embedCode;
                byte[] conteudoVideo;


                switch (request.Format)
                {
                    case Format.Mp3:

                        var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                        var stream = await youtube.Videos.Streams.GetAsync(streamInfo);
                        await youtube.Videos.Streams.DownloadAsync(streamInfo, $@"C:\Users\Faluckes\Downloads{title}.{streamInfo}.mp3");
                        Log.Information("Vídeo baixando em MP3");
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            conteudoVideo = memoryStream.ToArray();
                        }
                        return File(conteudoVideo, "video/mp3", $"{video.Title}.mp3");
                        break;
                    case Format.Mp4:
                        var videoStreamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();
                        var videoStream = await youtube.Videos.Streams.GetAsync(videoStreamInfo);
                        await youtube.Videos.Streams.DownloadAsync(videoStreamInfo, $@"C:\Users\Faluckes\Downloads{title}.{videoStreamInfo.Container}");
                        Log.Information("Vídeo sendo baixado em MP4");
                        using (var memoryStream = new MemoryStream())
                        {
                            videoStream.CopyTo(memoryStream);
                            conteudoVideo = memoryStream.ToArray();
                        }
                        return File(conteudoVideo, "video/mp4", $"{video.Title}.mp4");
                        break;
                }
                Log.Information("Video baixado com sucesso.");
                // Exemplo de URL do YouTube

            }
            catch (Exception ex)
            {
                Log.Error($"Ocorreu um erro: {ex.StackTrace}");
            }
            finally
            {
                View();
            }
            return View("Index");
        }


        [HttpPost]
        public async Task<IActionResult> ConverterMidia(IFormFile file, RequestInfo request)
        {

            try
            {

                if (file != null)
                {
                    var fileName = file.FileName;

                    Log.Information($"Nome do arquivo a ser convertido: {fileName}");

                    byte[] conteudoVideo;

                    // Aqui você pode fazer algo com o nome do arquivo
                    switch (request.Format)
                    {
                        case Format.Mp4:
                            var newFilemp4 = Path.ChangeExtension(fileName, ".mp4");
                            var uploadmp4 = Path.Combine(_env.WebRootPath, "uploads");
                            var NewPathmp4 = Path.Combine(uploadmp4, newFilemp4);

                            Log.Information($"Novo nome: { newFilemp4}");
                            Log.Information($"Upload: {uploadmp4}");
                            Log.Information($"Local onde foi baixado: {NewPathmp4} ");

                            using (var memoryStream = new MemoryStream())
                            {
                                file.CopyTo(memoryStream);
                                conteudoVideo = memoryStream.ToArray();
                            }
                            Log.Information($"Arquivo convertido com Sucesso!!!");

                            return File(conteudoVideo, "video/mp4", $"{newFilemp4}");

                            break;

                        case Format.Mp3:
                            var newFilemp3 = Path.ChangeExtension(fileName, ".mp3");
                            var uploadmp3 = Path.Combine(_env.WebRootPath, "uploads");
                            var NewPathmp3 = Path.Combine(uploadmp3, newFilemp3);

                            Log.Information($"Novo nome: { newFilemp3}");
                            Log.Information($"Upload: {uploadmp3}");
                            Log.Information($"Local onde foi baixado: {NewPathmp3} ");

                            using (var memoryStream = new MemoryStream())
                            {
                                file.CopyTo(memoryStream);
                                conteudoVideo = memoryStream.ToArray();
                            }
                            Log.Information($"Arquivo convertido com Sucesso!!!");

                            return File(conteudoVideo, "video/mp3", $"{newFilemp3}");

                            break;

                        case Format.Wav:
                            var newFilewav = Path.ChangeExtension(fileName, ".wav");
                            var uploadwav = Path.Combine(_env.WebRootPath, "uploads");
                            var NewPathwav = Path.Combine(uploadwav, newFilewav);

                            Log.Information($"Novo nome: { newFilewav}");
                            Log.Information($"Upload: {uploadwav}");
                            Log.Information($"Local onde foi baixado: {NewPathwav} ");

                            using (var memoryStream = new MemoryStream())
                            {
                                file.CopyTo(memoryStream);
                                conteudoVideo = memoryStream.ToArray();
                            }
                            Log.Information($"Arquivo convertido com Sucesso!!!");
                            return File(conteudoVideo, "video/wav", $"{newFilewav}");

                            break;

                        case Format.Mov:
                            var newFilemov = Path.ChangeExtension(fileName, ".mov");
                            var uploadmov = Path.Combine(_env.WebRootPath, "uploads");
                            var NewPathmov = Path.Combine(uploadmov, newFilemov);

                            Log.Information($"Novo nome: { newFilemov}");
                            Log.Information($"Upload: {uploadmov}");
                            Log.Information($"Local onde foi baixado: {NewPathmov} ");

                            using (var memoryStream = new MemoryStream())
                            {
                                file.CopyTo(memoryStream);
                                conteudoVideo = memoryStream.ToArray();
                            }
                            Log.Information($"Arquivo convertido com Sucesso!!!");

                            return File(conteudoVideo, "video/mov", $"{newFilemov}");

                            break;
                    }
                    return Ok();
                }
            }
            catch (Exception ex) { Log.Error(ex.Message); }

            return View("ConverterMidia");
        }


        [HttpPost]
        public async Task<IActionResult> SalvarTT(IFormFile file, RequestInfo request)
        {

            string apiKey = "tAjX2GDkSrIO1YTZJJKGbyyEn";
            string apiSecret = "HXvZG9NdceY1oM0PJpsZCASreI7924t2t4o0hllXM5mBTAMhMn";
            string accessToken = "1546170092634923009-30Ku3amFgRUWpXoZLQ4xC9v2cqb8zQ";
            string accessTokenSecret = "pUqdrqtuVBxEuJ8gUWfVRe1CznE9sOsiRGmrqGJ47m4H0";

            


            try
            {
                Auth.SetUserCredentials(apiKey, apiSecret, accessToken, accessTokenSecret);

                //var urlVideo = request.URL;

                //Uri uri = new Uri(urlVideo);
                //string[] segments = uri.Segments;
                //long tweetId = (long)Convert.ToDouble(segments[segments.Length - 1]);
                long tweetId = 1660745109002940417;
                var tweet = Tweet.GetTweet(tweetId);

                

                //Log.Information(urlVideo);
                Log.Information(tweetId.ToString());

                Console.WriteLine(tweetId);

                if (tweet != null)
                {
                    var media = tweet.Media.First();
                    var videoUrl = media.VideoDetails.Variants.First().URL;

                    using (WebClient client = new WebClient())
                    {
                        byte[] videoBytes = client.DownloadData(videoUrl);

                        string fileName = $"{tweetId}.{media.MediaType.ToString().ToLower()}";
                        string contentType = $"video/{media.MediaType.ToString().ToLower()}";

                        return File(videoBytes, contentType, fileName);

                    }

                }
                else
                {
                    Console.WriteLine("Falha ao obter o tweet!");
                    Log.Error("Falha ao obter o tweet!");
                }


                



            }
            catch { }
            return View("Twitter");
        }

        #endregion


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }





    }



}
