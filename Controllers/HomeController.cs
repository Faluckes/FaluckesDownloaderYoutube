using FalcockDownloaderWebSite.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Diagnostics;
using System.Media;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using System.IO;
using System.Text.RegularExpressions;

namespace FalcockDownloaderWebSite.Controllers
{
    public class HomeController : Controller
    {




        public IActionResult Index()
        {



            Log.Information("Abrindo Index...");


            //BORA TRATAR ESSA EXCEÇÕES PARCEIRO!!!!!!
            //BORA TRATAR ESSA EXCEÇÕES PARCEIRO!!!!!!


            RequestInfo request = new RequestInfo();
            request.URL = "";

            return View(request);
        }


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
                        await youtube.Videos.Streams.DownloadAsync(streamInfo, $@"{title}.{streamInfo}.mp3");
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
                        await youtube.Videos.Streams.DownloadAsync(videoStreamInfo, $@"{title}.{videoStreamInfo.Container}");
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



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}