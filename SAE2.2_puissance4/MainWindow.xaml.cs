// MainWindow.xaml.cs
using System.Windows;

namespace SAE2._2_puissance4
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			this.FontSize = ChoixPersonnalisation.TailleTexte;
			// On demarre sur la page du menu principal
			FramePrincipal.Navigate(new PageMenu());
		}
	}
}
