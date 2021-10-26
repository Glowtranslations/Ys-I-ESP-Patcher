using System.Linq;
using System.Net;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Parcheador_Ys_I_Chronicles_.Controllers
{
    public class HomeController : Controller
    {
        public static ApplyTranslation Core { get; set; }
        public static bool ListenerEnabled { get; set; }

        // GET: HomeController
        public ActionResult Index()
        {
            if (HybridSupport.IsElectronActive && !ListenerEnabled)
            {
                Electron.IpcMain.On("select-rom", async (args) => {
                    var mainWindow = Electron.WindowManager.BrowserWindows.Last();
                    var options = new OpenDialogOptions
                    {
                        Properties = new OpenDialogProperty[] {
                            OpenDialogProperty.openDirectory
                        }
                    };

                    string[] files = await Electron.Dialog.ShowOpenDialogAsync(mainWindow, options);

                    if (files.Length != 0)
                        Core = new ApplyTranslation(files[0]);


                    Electron.IpcMain.Send(mainWindow, "select-rom-reply", files);
                });
                ListenerEnabled = true;

            }
            return View(this);
        }

        public ActionResult CheckGame(string file)
        {
            if (Core == null)
                return Problem("No se ha seleccionado la carpeta de Ys I Chronicles+.");

            return Core.CheckGame() ? Ok() : Problem("La carpeta de instalación de Ys I Chronicles+ no es válida.");

        }

        public ActionResult DownloadPatch(string file)
        {
            if (Core == null)
                return Problem("No se ha seleccionado la carpeta de Ys I Chronicles+.");

            var result = Core.DownloadPatch();

            return result.Item1 ? Ok() : Problem(result.Item2);

        }

        public ActionResult PatchGame(string file)
        {
            if (Core == null)
                return Problem("No se ha seleccionado la carpeta de Ys I Chronicles+.");

            var result = Core.ExtractPatch();

            if (!result.Item1)
                return Problem(result.Item2);

            result = Core.PatchPcGame();

            return result.Item1 ? Ok() : Problem(result.Item2);
        }

        public ActionResult CheckInternet()
        {
            var result = false;
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("https://tradusquare.es/"))
                   result = true;
            }
            catch
            {
                result = false;
            }

            return Json(new { check = result });
        }
    }
}
