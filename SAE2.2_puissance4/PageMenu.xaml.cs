// PageMenu.xaml.cs
using System.Windows;
using System.Windows.Controls;

namespace SAE2._2_puissance4
{
    public partial class PageMenu : Page
    {
        public PageMenu()
        {
            InitializeComponent();
			this.FontSize = ChoixPersonnalisation.TailleTexte;
		}

        // Nouvelle partie on va sur la page des paramètres
        private void BtnNouvellePartie_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageParametres());
        }

        // Personnalisation PAS ENCORE FAIT
        private void BtnPersonnalisation_Click(object sender, RoutedEventArgs e)
        {
			NavigationService.Navigate(new PagePersonnalisation());
		}

        // Statistiques PAS ENCORE FAIT
        private void BtnStatistiques_Click(object sender, RoutedEventArgs e)
        {
			NavigationService.Navigate(new PageStatistique());
		}

        private void BtnChargerPartie_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageChargerPartie());
        }

        // Quitter l'application
        private void BtnQuitter_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
