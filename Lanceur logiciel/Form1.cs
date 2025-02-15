using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                MessageBox.Show("Fichier enregistré", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            // On lit le fichier JSON
            string json = System.IO.File.ReadAllText(path);

            // On récupère le nom et le chemin de l'application
            string[] tab = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(json);
            string nom = tab[0];
            string chemin = tab[1];

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

        /// <summary>
        /// création de la grille de bouton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Supprimer uniquement les boutons dynamiques (ceux qui ont un Tag "Logiciel")
            foreach (var btn in this.Controls.OfType<Button>())
            {
                if (btn.Tag == "Logiciel")
                {
                    this.Controls.Remove(btn); // Supprimer le bouton
                }
            }

            int maxCol = 5; // Nombre max de colonnes
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
                        Text = item.Nom,
                        Size = new Size(btnWidth, btnHeight),
                        Location = new Point(10 + col * (btnWidth + paddingX), 30 + row * (btnHeight + paddingY)),
                        Tag = "Logiciel" // Marquer ce bouton comme un bouton de logiciel
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
            var nom = Microsoft.VisualBasic.Interaction.InputBox("Nom de l'application à supprimer", "Supprimer une application", "");

            if (nom == null)
            {
                throw new Exception("Veuillez entrez un nom d'application valide");
            }

            string jsonPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "logiciel.json"); // Chemin du fichier JSON
            var contenueJson = File.ReadAllText(jsonPath); // On lit le fichier JSON
            List<Logiciel> logiciels = JsonConvert.DeserializeObject<List<Logiciel>>(contenueJson) ?? new List<Logiciel>(); // On récupère la liste des logiciels ou on crée une nouvelle liste vide en cas d'erreur

            Boolean found = false;

            foreach (var item in logiciels)
            {
                if (item.Nom == nom)
                {
                    found = true;
                    logiciels.Remove(item);
                    File.WriteAllText(jsonPath, JsonConvert.SerializeObject(logiciels, Formatting.Indented));
                    MessageBox.Show("Application supprimée", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return; // on sort de la boucle
                }
            }
            // si rien n'a été trouvé
            if (!found)
            {
                MessageBox.Show("Aucune application trouvée", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // recharge le formulaire
            Form1_Load(null, null);
        }
    }
}
