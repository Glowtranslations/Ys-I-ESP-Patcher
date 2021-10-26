using System;
using System.IO;
using Mara.Lib.Common;
using Mara.Lib.Platforms.Generic;

namespace Parcheador_Ys_I_Chronicles_.Controllers
{
    public class ApplyTranslation
    {
        private string path;
        private string url = "https://github.com/Glowtranslations/Ys1Chronicles_ESP/releases/download/parche/ys1esp.zip";
        private string downloadPath = $"{Path.GetTempPath()}{Path.DirectorySeparatorChar}ys.zip";
        private Main process;


        public ApplyTranslation(string path)
        {
            this.path = path;
        }

        public (bool, string) DownloadPatch()
        {
            try
            {
                if (System.IO.File.Exists(downloadPath))
                    System.IO.File.Delete(downloadPath);

                Internet.GetFile(url, downloadPath);
            }
            catch (Exception e)
            {
                return (false, $"Se ha producido un error descargando los archivos.\n{e.Message}\n{e.StackTrace}");
            }
            return (true, string.Empty);
        }

        public (bool, string) ExtractPatch()
        {
            try
            {
                process = new Main(path, path, downloadPath);
            }
            catch (Exception e)
            {
                return (false, $"Se ha producido un error extrayendo los archivos.\n{e.Message}\n{e.StackTrace}");
            }
            return (true, string.Empty);
        }

        public (bool, string) PatchPcGame()
        {
            try
            {
                var result = process.ApplyTranslation();
                if (result.Item1 == 0)
                    return (true, string.Empty);

                return (false, $"Se ha producido un error aplicando la traducción.\nError: {result.Item1}\nMensaje: {result.Item2}");
            }
            catch (Exception e)
            {
                return (false, $"Se ha producido un error aplicando la traducción.\nError: INTERNAL CRASH\nMensaje:\n{e.Message}\n{e.StackTrace}");
            }
        }

        public string[] ShowChangelog()
        {
            var info = process.GetInfo();
            return new[]
            {
                info.PatchVersion,
                info.Changelog
            };

        }

        public bool CheckGame()
        {
            var directories = new[]
            {
                "release"
            };

            var files = new[]
            {
                "config.exe", "ys1plus.exe"
            };

            foreach (var directory in directories)
            {
                if (!Directory.Exists($"{path}{Path.DirectorySeparatorChar}{directory}"))
                    return false;
            }

            foreach (var file in files)
            {
                if (!File.Exists($"{path}{Path.DirectorySeparatorChar}{file}"))
                    return false;
            }

            return true;
        }
    }
}
