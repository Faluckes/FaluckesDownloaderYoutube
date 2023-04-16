//namespace FalcockDownloaderWebSite.Controllers
//{
//    public class PathDialog
//    {
//        private async void WebView_CoreWebView2Ready(object sender, EventArgs e)
//        {
//            var webview = (WebView2)sender;

//            // Carregar o arquivo HTML com o botão de seleção de diretório e o campo de texto
//            await webview.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("document.documentElement.innerHTML = \"<button onclick='selectDirectory()'>Selecionar diretório</button><input id='selectedPath' type='text' readonly>\";");

//            // Adicionar um objeto JavaScript para expor a função selectDirectory para o código C#
//            await webview.CoreWebView2.AddHostObjectToScriptAsync("api", new Api());
//        }

//        public class Api
//        {
//            // Método que será chamado pelo JavaScript para iniciar a seleção de diretório
//            public async Task<string> SelectDirectory()
//            {
//                var dialog = new OpenFolderDialog();
//                if (await dialog.ShowAsync() == CommonFileDialogResult.Ok)
//                {
//                    return dialog.FileName;
//                }
//                return null;
//            }
//        }
//    }
//}
