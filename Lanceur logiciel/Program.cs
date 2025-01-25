using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lanceur_logiciel
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles(); // active les styles visuels pour les contrôles
            Application.SetCompatibleTextRenderingDefault(false); // active la compatibilité avec les contrôles
            Application.Run(new Form1()); // lance l'application avec le formulaire Form1
        }

        /// <summary>
        /// Création d'un formulaire pour ajouter l'application dans un JSON
        /// </summary>
        static void CreateForm()
        {
            Application.EnableVisualStyles(); // active les styles visuels pour les contrôles
            Application.SetCompatibleTextRenderingDefault(false); // active la compatibilité avec les contrôles
            Application.Run(new Form1()); // lance l'application avec le formulaire Form1
        }
    }
}
