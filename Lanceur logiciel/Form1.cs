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
            public string NomAffichage { get; set; }
            public string Chemin { get; set; }
            public string NomFichier { get; set; }
            public string Extention { get; set; }
        }

        /// <summary>
        /// le but est d'ouvrir une boîte de dialogue pour ouvrir un fichier executant ou des racourcis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            // Création d'une boîte de dialogue pour ouvrir un fichier
            OpenFileDialog openFileDialog1 = new OpenFileDialog()
            {
                FileName = "Choisir un fichier",
                Filter = "Fichier executable|*.exe|Fichier raccourcie|*.lnk",
                Title = "Choisir un éxecutable ou raccourcie"
            };

            // Si l'utilisateur clique sur OK
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // On récupère le nom du fichier
                string fichier = openFileDialog1.FileName;

                // On crée un fichier JSON qui va répertorier le chemin de l'application + le nom de l'application sans le .exe ni le chemin
                Logiciel nouveauLogiciel = new Logiciel
                {
                    NomAffichage = Path.GetFileNameWithoutExtension(fichier),
                    Chemin = Path.GetDirectoryName(fichier),
                    NomFichier = Path.GetFileNameWithoutExtension(fichier),
                    Extention = Path.GetExtension(fichier)
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
                        Name = "btn_logiciel_" + item.NomAffichage, // Identifiant unique pour ne supprimer que les boutons logiciels
                        Text = item.NomAffichage,
                        Size = new Size(btnWidth, btnHeight),
                        Location = new Point(10 + col * (btnWidth + paddingX), 30 + row * (btnHeight + paddingY))
                    };

                    button.Click += (s, ev) =>
                    {
                        try
                        {
                            // Lancer l'application
                            var startInfo = new System.Diagnostics.ProcessStartInfo()
                            {
                                FileName = Path.Combine(item.Chemin, item.NomFichier + item.Extention),
                                UseShellExecute = true // Permet d'éviter une détection de la schizophrénie de Windows Defender
                            };
                            System.Diagnostics.Process.Start(startInfo);
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
            // crée une fenêtre de dialogue pour demander le nom de l'application à renommer avec une choice box
            string jsonPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "logiciel.json");

            if (!File.Exists(jsonPath)) // Vérifier si le fichier qui liste les applications existe
            {
                MessageBox.Show("Aucune application enregistrée", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string contenuJson = File.ReadAllText(jsonPath);
            List<Logiciel> logiciels = JsonConvert.DeserializeObject<List<Logiciel>>(contenuJson) ?? new List<Logiciel>();

            // Créer une liste de noms d'applications
            List<string> nomsApplications = logiciels.Select(l => l.NomAffichage).ToList();

            // Créer une boîte de dialogue avec une liste déroulante
            using (Form form = new Form())
            {
                Label label = new Label() { Left = 50, Top = 20, Width = 250, Text = "Sélectionnez une application à supprimer" };
                ComboBox comboBox = new ComboBox() { Left = 50, Top = 50, Width = 200 };
                comboBox.DataSource = nomsApplications;
                Button confirmation = new Button() { Text = "Ok", Left = 150, Width = 100, Top = 80, DialogResult = DialogResult.OK };
                form.Controls.Add(label);
                form.Controls.Add(comboBox);
                form.Controls.Add(confirmation);
                form.AcceptButton = confirmation;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    string nom = comboBox.SelectedItem.ToString();

                    // Trouver et supprimer l'élément
                    Logiciel logicielASupprimer = logiciels.FirstOrDefault(l => l.NomAffichage.Equals(nom, StringComparison.OrdinalIgnoreCase));

                    if (logicielASupprimer != null)
                    {
                        logiciels.Remove(logicielASupprimer); // Supprimer l'élément
                        File.WriteAllText(jsonPath, JsonConvert.SerializeObject(logiciels, Formatting.Indented)); // Sauvegarder les modifications
                        // Reset de l'affichage
                        delete_buttons(sender, e, false);
                        Form1_Load(null, null); // Recharge l'affichage
                    }
                    else
                    {
                        MessageBox.Show("Aucune application trouvé", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
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
                delete_buttons(sender, e, true);
            }
            else
            {
                MessageBox.Show("Aucune application enregistrée", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Supprime tous les boutons de l'interface pour réactualiser la liste correctement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="SupData"></param>
        private void delete_buttons(object sender, EventArgs e, bool SupData)
        {
            var allButton = this.Controls.OfType<Button>().Where(btn => btn.Name.StartsWith("btn_logiciel_")).ToList();

            foreach (var button in allButton)
            {
                this.Controls.Remove(button);
            }

            if (SupData) // en cas de suppression de toutes les applications de la liste
            {
                string jsonPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "logiciel.json");
                if (File.Exists(jsonPath))
                {
                    File.Delete(jsonPath);
                    MessageBox.Show("Toutes les applications ont été supprimées", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // crée une fenêtre de dialogue pour demander le nom de l'application à renommer avec une choice box
            string jsonPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "logiciel.json");

            if (!File.Exists(jsonPath)) // Vérifier si le fichier qui liste les applications existe
            {
                MessageBox.Show("Aucune application enregistrée", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string contenuJson = File.ReadAllText(jsonPath);
            List<Logiciel> logiciels = JsonConvert.DeserializeObject<List<Logiciel>>(contenuJson) ?? new List<Logiciel>();

            // Créer une liste de noms d'applications
            List<string> nomsApplications = logiciels.Select(l => l.NomAffichage).ToList();

            // Créer une boîte de dialogue avec une liste déroulante
            using (Form form = new Form())
            {
                Label label = new Label() { Left = 50, Top = 20, Width = 250, Text = "Sélectionnez une application à renommer" };
                ComboBox comboBox = new ComboBox() { Left = 50, Top = 50, Width = 200 };
                comboBox.DataSource = nomsApplications;
                Button confirmation = new Button() { Text = "Ok", Left = 150, Width = 100, Top = 80, DialogResult = DialogResult.OK };
                form.Controls.Add(label);
                form.Controls.Add(comboBox);
                form.Controls.Add(confirmation);
                form.AcceptButton = confirmation;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    string nom = comboBox.SelectedItem.ToString();

                    // Trouver et renommer l'élément
                    Logiciel logicielARenommer = logiciels.FirstOrDefault(l => l.NomAffichage.Equals(nom, StringComparison.OrdinalIgnoreCase));

                    if (logicielARenommer != null)
                    {
                        string nouveauNom = Microsoft.VisualBasic.Interaction.InputBox("Nouveau nom de l'application", "Renommer une application", "");
                        if (string.IsNullOrWhiteSpace(nouveauNom)) // Vérifier si le nom est vide
                        {
                            MessageBox.Show("Veuillez entrer un nom d'application valide", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        logicielARenommer.NomAffichage = nouveauNom; // Renommer l'élément
                        File.WriteAllText(jsonPath, JsonConvert.SerializeObject(logiciels, Formatting.Indented)); // Sauvegarder les modifications

                        // Reset de l'affichage
                        delete_buttons(sender, e, false);
                        Form1_Load(null, null); // Recharge l'affichage
                    }
                    else
                    {
                        MessageBox.Show("Aucune application trouvé", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
    }
}