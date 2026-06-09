using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SAE2._2_puissance4
{
	public partial class PagePersonnalisation : Page
	{
		public PagePersonnalisation()
		{
			InitializeComponent();
			this.FontSize = ChoixPersonnalisation.TailleTexte;
		}

		private void BtnRetour_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.GoBack();
		}

		private void BtnPrevisualiser_Click(object sender, RoutedEventArgs e)
		{
			string forme = (ComboFormes.SelectedItem as ComboBoxItem)?.Content.ToString();
			Brush couleur = ObtenirCouleur();

			if (forme == "Carré")
			{
				ConteneurApercu.Child = new Rectangle
				{
					Width = 70,
					Height = 70,
					Fill = couleur
				};
			}
			else if (forme == "Triangle")
			{
				ConteneurApercu.Child = new Polygon
				{
					Points = new PointCollection {
				new Point(35, 0),
				new Point(70, 70),
				new Point(0, 70)
			},
					Fill = couleur
				};
			}
			else
			{
				ConteneurApercu.Child = new Ellipse
				{
					Width = 70,
					Height = 70,
					Fill = couleur
				};
			}
		}

		private void BtnValider_Click(object sender, RoutedEventArgs e)
		{
			ChoixPersonnalisation.CouleurJ1 =
				(ComboCouleurs.SelectedItem as ComboBoxItem)?.Content.ToString();

			ChoixPersonnalisation.FormePion =
				(ComboFormes.SelectedItem as ComboBoxItem)?.Content.ToString();

			ChoixPersonnalisation.TailleTexte =
				double.Parse((ComboTaille.SelectedItem as ComboBoxItem)?.Content.ToString());

			ChoixPersonnalisation.Contraste =
				SliderContraste.Value;

			MessageBox.Show(
				"Personnalisation enregistrée !\n" +
				"Couleur : " + ChoixPersonnalisation.CouleurJ1 + "\n" +
				"Forme : " + ChoixPersonnalisation.FormePion + "\n" +
				"Taille texte : " + ChoixPersonnalisation.TailleTexte + "\n" +
				"Contraste : " + ChoixPersonnalisation.Contraste + "%",
				"Personnalisation sauvegardée",
				MessageBoxButton.OK,
				MessageBoxImage.Information);
		}

		private SolidColorBrush ObtenirCouleur()
		{
			string choix = (ComboCouleurs.SelectedItem as ComboBoxItem)?.Content.ToString();

			switch (choix)
			{
				case "Rouge": return Brushes.Red;
				case "Jaune": return Brushes.Gold;
				case "Bleu": return Brushes.DodgerBlue;
				case "Vert": return Brushes.LimeGreen;
				case "Orange": return Brushes.Orange;
				case "Rose": return Brushes.HotPink;
				case "Blanc": return Brushes.White;
				default: return Brushes.Red;
			}
		}
	}
}