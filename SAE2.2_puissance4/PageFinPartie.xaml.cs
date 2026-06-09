// PageFinPartie.xaml.cs
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SAE2._2_puissance4
{
    public partial class PageFinPartie : Page
    {
        public PageFinPartie(string message)
        {
            InitializeComponent();
			this.FontSize = ChoixPersonnalisation.TailleTexte;
			TxtResultat.Text = message;

            // Couleur du message selon le résultat
            if (message.Contains("gagné"))
                TxtResultat.Foreground = Brushes.Gold;
            else if (message.Contains("nul"))
                TxtResultat.Foreground = Brushes.LightBlue;
            else
                TxtResultat.Foreground = Brushes.Salmon;
        }

        private void BtnRejouer_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageParametres());
        }

        private void BtnChangerParams_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageParametres());
        }

        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageMenu());
        }
    }
}
