using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Unhide_Midwars_MMR
{
    // Represents a class for modifying Heroes of Newerth game files
    class HonZip
    {
        public string PathInstall { get; }

        private Dictionary<string, string> DictReplace;
        private const string RelPathResourcesZero = @"game\resources0.s2z";
        private const string RelPathGameLobby = "ui/fe2/game_lobby.package";

        public HonZip(string pathInstall)
        {
            PathInstall = pathInstall;
            InitializeDictionary();
        }


        private void InitializeDictionary()
        {
            DictReplace = new Dictionary<string, string>
            {
                {
                    @"If(!ShouldHideStats(), Trigger('GameLobby_StatsFade', _psr_tipShow{index}));",
                    @"Trigger('GameLobby_StatsFade', _psr_tipShow{index});"
                },
                {
                    @"If(!ShouldHideStats(), Trigger(\'GameLobby_StatsFade\', _psr_tipShow{index}));",
                    @"Trigger(\'GameLobby_StatsFade\', _psr_tipShow{index});"
                },
                {
                    @"If(!ShouldHideStats(), Trigger('GameLobby_StatsFade', _psr_tipShow{index})),",
                    @"Trigger('GameLobby_StatsFade', _psr_tipShow{index}),"
                },
                {
                    @"Game_Lobby:SetupCampainLevel(self, {index}, param0, param18, param86)",
                    @"Game_Lobby:SetupCampainLevel(self, {index}, param0, param0, param86)"
                },
                {
                    @" or ShouldHideStats()",
                    String.Empty
                },
                {
                    @" and !ShouldHideStats()",
                    String.Empty
                },
            };
        }

        public void Install()
        {
            var resourcesPath = PathInstall + RelPathResourcesZero;
            var oldPath = Path.GetDirectoryName(resourcesPath) + @"\resources0.old.s2z";
            if (!File.Exists(oldPath))
            {
                // Copy old file for backup
                File.Copy(resourcesPath, oldPath);

                string newGameLobby = String.Empty;
                using (var zipToOpen = new FileStream(resourcesPath, FileMode.Open))
                {
                    using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                    {
                        var entryGameLobby = archive.GetEntry(RelPathGameLobby);

                        if (entryGameLobby != null)
                        {
                            using (var readerGameLobby = new StreamReader(entryGameLobby.Open()))
                            {
                                string line;
                                while ((line = readerGameLobby.ReadLine()) != null)
                                {
                                    foreach (var key in DictReplace.Keys)
                                    {
                                        if (line.Contains(key))
                                        {
                                            line = line.Replace(key, DictReplace[key]);
                                        }
                                    }
                                    newGameLobby += line + "\r\n";
                                }
                                newGameLobby = newGameLobby.Substring(0, newGameLobby.Length - 2);
                            }
                            entryGameLobby.Delete();
                        }
                    }
                }

                using (var zipToOpen = new FileStream(resourcesPath, FileMode.Open))
                {
                    using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                    {
                        var entryGameLobby = archive.CreateEntry(RelPathGameLobby);
                        using (var writerGameLobby = new StreamWriter(entryGameLobby.Open()))
                        {
                            writerGameLobby.Write(newGameLobby);
                        }
                    }
                }

                MessageBox.Show("Installation success.", "Midwars MMR", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("The mod is already installed.", "Midwars MMR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void Uninstall()
        {
            var resourcesPath = PathInstall + RelPathResourcesZero;
            var oldDir = Path.GetDirectoryName(resourcesPath) + @"\resources0.old.s2z";
            if (File.Exists(oldDir))
            {
                var tempPath = Path.GetDirectoryName(resourcesPath) + @"\resources0.temp.s2z";
                
                File.Move(resourcesPath, tempPath);
                File.Move(oldDir, resourcesPath);
                File.Delete(tempPath);

                MessageBox.Show("The mod was successfully uninstalled.", "Midwars MMR", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("The mod is not installed.", "Midwars MMR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
