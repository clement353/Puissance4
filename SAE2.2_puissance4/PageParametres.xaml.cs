// PageParametres.xaml.cs
using System.Windows;
using System.Windows.Controls;

namespace SAE2._2_puissance4
{
    public partial class PageParametres : Page
    {
        public PageParametres()
        {
            InitializeComponent();
			this.FontSize = ChoixPersonnalisation.TailleTexte;
		}

        private void BtnRetour_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnValider_Click(object sender, RoutedEventArgs e)
        {
            // Lecture et validation des parametres
            int lignes, colonnes, aligner, temps;

            bool okLignes   = int.TryParse(TxtLignes.Text,   out lignes)   && lignes   >= 4 && lignes   <= 12;
            bool okColonnes = int.TryParse(TxtColonnes.Text, out colonnes) && colonnes >= 4 && colonnes <= 15;
            bool okAligner  = int.TryParse(TxtAligner.Text,  out aligner)  && aligner  >= 3;
            bool okTemps    = int.TryParse(TxtTemps.Text,    out temps)    && temps    >= 0;

            if (!okLignes || !okColonnes || !okAligner || !okTemps)
            {
                MessageBox.Show("Paramètres invalides !\n" +
                                " - Lignes : 4 à 12\n" +
                                " - Colonnes : 4 à 15\n" +
                                " - Jetons à aligner : minimum 3\n" +
                                " - Temps : 0 ou plus",
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (aligner > lignes && aligner > colonnes)
            {
                MessageBox.Show("Le nombre de jetons à aligner est plus grand que la grille !",
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Determination du mode de jeu
            string modeJeu;
            Difficulte difficulte = Difficulte.FACILE;

            if (RadioBotBas.IsChecked == true)
            {
                modeJeu    = "bot";
                difficulte = Difficulte.FACILE;
            }
            else if (RadioBotHaut.IsChecked == true)
            {
                modeJeu    = "bot";
                difficulte = Difficulte.DIFFICILE;
            }
            else
            {
                modeJeu = "deuxjoueurs";
            }

            // Navigation vers la page de jeu
            NavigationService.Navigate(
                new PageJeu(lignes, colonnes, aligner, temps, modeJeu, difficulte));
        }
    }
}
