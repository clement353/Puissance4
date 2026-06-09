using System.Windows;
using System.Windows.Controls;

namespace SAE2._2_puissance4
{
	public partial class PageStatistique : Page
	{
		public PageStatistique()
		{
			InitializeComponent();
			this.FontSize = ChoixPersonnalisation.TailleTexte;
		}

		private void BtnRetour_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.GoBack();
		}
	}
}