using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SAE2._2_puissance4{
	public partial class PageChargerPartie : Page
	{
		public PageChargerPartie()
		{
			InitializeComponent();
			this.FontSize = ChoixPersonnalisation.TailleTexte;
		}

		private void BtnRetour_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.GoBack();
		}

		private void BtnCharger_Click(object sender, RoutedEventArgs e)
		{
			if (ListParties.SelectedItem == null)
			{
				MessageBox.Show("Sélectionnez une partie d'abord !",
								"Aucune sélection",
								MessageBoxButton.OK,
								MessageBoxImage.Warning);
				return;
			}

			MessageBox.Show("Chargement de la partie... (fonctionnalité à venir avec la BD)",
							"Chargement",
							MessageBoxButton.OK,
							MessageBoxImage.Information);
		}
	}
}