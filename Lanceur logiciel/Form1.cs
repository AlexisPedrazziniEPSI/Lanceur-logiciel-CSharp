using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
                string nom = System.IO.Path.GetFileNameWithoutExtension(fichier);
                string chemin = System.IO.Path.GetDirectoryName(fichier);
                Array json = new string[] { nom, chemin };

                // création du fichier JSON au même endroit que le logiciel qui est acutellement en cours d'écriture
                string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\logiciel.json";
                System.IO.File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(json));
            }
        }

        /// <summary>
        /// le but est de lister les applications dans la listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listbox(object sender, EventArgs e)
        {
            // Création d'une listbox
            ListBox listBox1 = new ListBox();
            listBox1.Size = new System.Drawing.Size(400, 200);
            listBox1.Location = new System.Drawing.Point(30, 10);

            this.Controls.Add(listBox1);

            listBox1.MultiColumn = true;
            listBox1.SelectionMode = SelectionMode.MultiExtended;

            // On récupère le chemin du fichier JSON
            string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\logiciel.json";
            Array jsonRead = Newtonsoft.Json.JsonConvert.DeserializeObject<Array>(System.IO.File.ReadAllText(path));

        }

       private void button_Click(object sender, EventArgs e)
        {
            // On récupère le chemin du fichier JSON
            string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\logiciel.json";

            // On récupère le contenu du fichier JSON
            Array jsonRead = Newtonsoft.Json.JsonConvert.DeserializeObject<Array>(System.IO.File.ReadAllText(path));

            // On récupère le nom de l'application
            string nom = ((Button)sender).Text;

            // On récupère le chemin de l'application
            string chemin = jsonRead.GetValue(Array.IndexOf(jsonRead, nom) + 1).ToString();

            // On lance l'application
            System.Diagnostics.Process.Start(chemin + "\\" + nom + ".exe");
        }
    }
}
