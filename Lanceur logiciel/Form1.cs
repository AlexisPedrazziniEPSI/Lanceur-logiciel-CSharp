using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Lanceur_logiciel
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Form1_Load(null, null); // charge le formulaire dès son ouverture
        }

        public class Logiciel
        {
            public string Nom { get; set; }
            public string Chemin { get; set; }
        }

        /// <summary>
        /// le but est d'ouvrir une boîte de dialogue pour ouvrir un fichier executant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            // Création d'une boîte de dialogue pour ouvrir un fichier
            OpenFileDialog openFileDialog1 = new OpenFileDialog() 
            {
                FileName = "Choisir un fichier",
                Filter = "Fichiers executant (*.exe)|*.exe",
                Title = "Choisir un éxecutable"
            };

            // Si l'utilisateur clique sur OK
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // On récupère le nom du fichier
                string fichier = openFileDialog1.FileName;

                // On crée un fichier JSON qui va répertorier le chemin de l'application + le nom de l'application sans le .exe ni le chemin
                Logiciel nouveauLogiciel = new Logiciel
                {
                    Nom = Path.GetFileNameWithoutExtension(fichier),
                    Chemin = Path.GetDirectoryName(fichier)
                };

                // chemin vers le jsooon
                string jsonPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "logiciel.json");

                // liste de logiciel
                List<Logiciel> logiciels = new List<Logiciel>();

                // Si le fichier existe déjà, on le ajoute sans supprimer son contenu
                if (File.Exists(jsonPath))
                {
                    string jsonFile = File.ReadAllText(jsonPath);
                    logiciels = JsonConvert.DeserializeObject<List<Logiciel>>(jsonFile) ?? new List<Logiciel>();
                }

                // Ajoute le nouveau logiciel
                logiciels.Add(nouveauLogiciel);

                // Sauvegarde la liste mise à jour
                File.WriteAllText(jsonPath, JsonConvert.SerializeObject(logiciels, Formatting.Indented));

                // Affiche un message et rafraîchit le formulaire
                this.Refresh();

                Form1_Load(null, null); // Recharge l'affichage
            }
        }

        /// <summary>
        /// le but est de lancer l'application enregistrée dans le fichier JSON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            // On récupère le chemin du fichier JSON
            string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\logiciel.json";

            if (!File.Exists(path))
            {
                MessageBox.Show("Aucun logiciel enregistré.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // On lit le fichier JSON
            string json = System.IO.File.ReadAllText(path);

            // On récupère le nom et le chemin de l'application
            List<Logiciel> logiciels = JsonConvert.DeserializeObject<List<Logiciel>>(json) ?? new List<Logiciel>();
            if (logiciels.Count > 0)
            {
                Logiciel premierLogiciel = logiciels[0];
                string nom = premierLogiciel.Nom;
                string chemin = premierLogiciel.Chemin;
            

                // exception si le fichier n'existe pas
                Exception.ReferenceEquals(chemin, null);

                // On lance l'application
                try {  
                    System.Diagnostics.Process.Start(chemin + "\\" + nom + ".exe");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// création de la grille de bouton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            int maxCol = 6; // Nombre max de colonnes
            int paddingX = 10, paddingY = 50; // Espacement
            int btnWidth = 120, btnHeight = 40; // Taille des boutons

            string jsonPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "logiciel.json");

            if (File.Exists(jsonPath))
            {
                var contenueJson = File.ReadAllText(jsonPath);
                List<Logiciel> logiciels = JsonConvert.DeserializeObject<List<Logiciel>>(contenueJson) ?? new List<Logiciel>();

                int row = 0, col = 0;

                foreach (var item in logiciels)
                {
                    Button button = new Button
                    {
                        Name = "btn_logiciel_" + item.Nom, // Identifiant unique pour ne supprimer que les boutons logiciels
                        Text = item.Nom,
                        Size = new Size(btnWidth, btnHeight),
                        Location = new Point(10 + col * (btnWidth + paddingX), 30 + row * (btnHeight + paddingY))
                    };

                    button.Click += (s, ev) =>
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(Path.Combine(item.Chemin, item.Nom + ".exe"));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Erreur : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    };

                    this.Controls.Add(button);

                    col++;
                    if (col >= maxCol)
                    {
                        col = 0;
                        row++;
                    }
                }
            }
        }

        /// <summary>
        /// Le but est de supprimer une application de la liste via une boite de dialogue qui demande le nom de l'application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click_1(object sender, EventArgs e)
        {
            // crée une fenêtre de dialogue pour demander le nom de l'application à supprimer
            string nom = Microsoft.VisualBasic.Interaction.InputBox("Nom de l'application à supprimer", "Supprimer une application", "");

            if (string.IsNullOrWhiteSpace(nom)) // Vérifier si le nom est vide
            {
                MessageBox.Show("Veuillez entrer un nom d'application valide", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string jsonPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "logiciel.json");

            if (!File.Exists(jsonPath)) // Vérifier si le fichier qui liste les applications existe
            {
                MessageBox.Show("Aucune application enregistrée", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string contenuJson = File.ReadAllText(jsonPath);
            List<Logiciel> logiciels = JsonConvert.DeserializeObject<List<Logiciel>>(contenuJson) ?? new List<Logiciel>();

            // Trouver et supprimer l'élément
            Logiciel logicielASupprimer = logiciels.FirstOrDefault(l => l.Nom.Equals(nom, StringComparison.OrdinalIgnoreCase));

            if (logicielASupprimer != null)
            {
                logiciels.Remove(logicielASupprimer); // Supprimer l'élément
                File.WriteAllText(jsonPath, JsonConvert.SerializeObject(logiciels, Formatting.Indented)); // Sauvegarder les modifications

                // Supprimer le bouton correspondant
                var boutonASupprimer = this.Controls.OfType<Button>().FirstOrDefault(btn => btn.Name == "btn_logiciel_" + nom);
                if (boutonASupprimer != null)
                {
                    this.Controls.Remove(boutonASupprimer);
                }
            }
            else
            {
                MessageBox.Show("Aucune application trouvée", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Le but est de supprimer toutes les applications de la liste
        /// </summary>
        private void button3_Click_1(object sender, EventArgs e)
        {
            string jsonPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "logiciel.json");
            if (File.Exists(jsonPath))
            {
                var contenuJson = File.ReadAllText(jsonPath);
                List<Logiciel> logiciels = JsonConvert.DeserializeObject<List<Logiciel>>(contenuJson) ?? new List<Logiciel>();

                if (logiciels.Count > 0) // Si il y a au moins un logiciel dans la liste
                {
                    var allButton = this.Controls.OfType<Button>().Where(btn => btn.Name.StartsWith("btn_logiciel_")).ToList();

                    foreach (var button in allButton)
                    {
                        this.Controls.Remove(button);
                    }

                    // Supprimer le fichier JSON
                    File.Delete(jsonPath);

                    MessageBox.Show("Toutes les applications ont été supprimées", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Aucune application à supprimer", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Aucune application enregistrée", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}