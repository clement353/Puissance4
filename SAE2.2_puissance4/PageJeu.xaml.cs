using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SAE2._2_puissance4
{
	public partial class PageJeu : Page
	{
		private Plateau plateau;
		private Joueur joueur1;
		private Joueur joueur2;
		private Joueur joueurActif;
		private bool partieTerminee;

		private int tempsParTour;
		private int tempsRestant;

		private DispatcherTimer timer;

		private const double TAILLE_CASE = 70;
		private static readonly Brush COULEUR_J2 = Brushes.Yellow;
		private static readonly Brush COULEUR_VIDE = new SolidColorBrush(Color.FromRgb(40, 40, 40));

		private Shape[,] ellipses;

		private SolidColorBrush ConvertirCouleur(string couleur)
		{
			switch (couleur)
			{
				case "Rouge": return Brushes.Red;
				case "Bleu": return Brushes.DodgerBlue;
				case "Vert": return Brushes.LimeGreen;
				case "Orange": return Brushes.Orange;
				case "Rose": return Brushes.HotPink;
				case "Blanc": return Brushes.White;
				default: return Brushes.Red;
			}
		}

		public PageJeu(int lignes, int colonnes, int aligner,
					   int temps, string modeJeu, Difficulte diff)
		{
			InitializeComponent();
			this.FontSize = ChoixPersonnalisation.TailleTexte;
			plateau = new Plateau(lignes, colonnes, aligner);
			tempsParTour = temps;
			partieTerminee = false;

			joueur1 = new JoueurHumain("Joueur 1", Pion.JOUEUR1);

			if (modeJeu == "bot")
				joueur2 = new JoueurIA("IA", Pion.JOUEUR2, diff);
			else
				joueur2 = new JoueurHumain("Joueur 2", Pion.JOUEUR2);

			joueurActif = joueur1;

			ConstruireGrille(lignes, colonnes);

			if (tempsParTour > 0)
			{
				tempsRestant = tempsParTour;
				timer = new DispatcherTimer();
				timer.Interval = TimeSpan.FromSeconds(1);
				timer.Tick += Timer_Tick;
				timer.Start();
			}
			else
			{
				TxtTimer.Text = "∞";
			}

			MettreAJourInfoJoueur();

			TxtJoueurActif.FontSize = ChoixPersonnalisation.TailleTexte;
			TxtTimer.FontSize = ChoixPersonnalisation.TailleTexte;
			this.Opacity = ChoixPersonnalisation.Contraste / 100.0;
		}

		private void ConstruireGrille(int lignes, int colonnes)
		{
			ellipses = new Shape[lignes, colonnes];

			for (int l = 0; l < lignes; l++)
				GrilleJeu.RowDefinitions.Add(
					new RowDefinition { Height = new GridLength(TAILLE_CASE) });

			for (int c = 0; c < colonnes; c++)
				GrilleJeu.ColumnDefinitions.Add(
					new ColumnDefinition { Width = new GridLength(TAILLE_CASE) });

			for (int l = 0; l < lignes; l++)
			{
				for (int c = 0; c < colonnes; c++)
				{
					Border bordure = new Border
					{
						Background = new SolidColorBrush(Color.FromRgb(25, 25, 25)),
						BorderBrush = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
						BorderThickness = new Thickness(1),
						Tag = c
					};

					Shape forme;

					if (ChoixPersonnalisation.FormePion == "Carré")
					{
						forme = new Rectangle
						{
							Width = TAILLE_CASE - 12,
							Height = TAILLE_CASE - 12,
							Fill = COULEUR_VIDE,
							Margin = new Thickness(6),
							IsHitTestVisible = false
						};
					}
					else if (ChoixPersonnalisation.FormePion == "Triangle")
					{
						forme = new Polygon
						{
							Points = new PointCollection {
								new Point(30, 0),
								new Point(60, 58),
								new Point(0, 58)
							},
							Fill = COULEUR_VIDE,
							Margin = new Thickness(6),
							IsHitTestVisible = false
						};
					}
					else
					{
						forme = new Ellipse
						{
							Width = TAILLE_CASE - 12,
							Height = TAILLE_CASE - 12,
							Fill = COULEUR_VIDE,
							Margin = new Thickness(6),
							IsHitTestVisible = false
						};
					}

					bordure.Child = forme;

					int colonne = c;
					bordure.MouseLeftButtonDown += (sender, e) =>
					{
						if (!partieTerminee)
							JouerColonne(colonne);
					};

					bordure.MouseEnter += (sender, e) =>
						bordure.Background = new SolidColorBrush(Color.FromRgb(40, 40, 40));
					bordure.MouseLeave += (sender, e) =>
						bordure.Background = new SolidColorBrush(Color.FromRgb(25, 25, 25));

					Grid.SetRow(bordure, l);
					Grid.SetColumn(bordure, c);
					GrilleJeu.Children.Add(bordure);

					ellipses[l, c] = forme;
				}
			}
		}

		private void JouerColonne(int col)
		{
			if (joueurActif is JoueurIA) return;
			if (!plateau.ColonneValide(col))
			{
				MessageBox.Show("Cette colonne est pleine !", "Coup invalide",
								MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			EffectuerCoup(col);
		}

		private void EffectuerCoup(int col)
		{
			int ligne = plateau.Poser(col, joueurActif.GetPion());

			ellipses[ligne, col].Fill =
				(joueurActif.GetPion() == Pion.JOUEUR1)
				? ConvertirCouleur(ChoixPersonnalisation.CouleurJ1)
				: COULEUR_J2;

			string symbole = (joueurActif.GetPion() == Pion.JOUEUR1) ? "X" : "O";
			ListHistorique.Items.Add(
				$"{joueurActif.GetNom()} ({symbole}) → Colonne {col + 1}");
			ListHistorique.ScrollIntoView(
				ListHistorique.Items[ListHistorique.Items.Count - 1]);

			if (plateau.AGagne(joueurActif.GetPion()))
			{
				FinPartie(joueurActif.GetNom() + " a gagné !");
				return;
			}

			if (plateau.EstPleine())
			{
				FinPartie("Match nul !");
				return;
			}

			joueurActif = (joueurActif == joueur1) ? joueur2 : joueur1;
			MettreAJourInfoJoueur();
			ResetTimer();

			if (joueurActif is JoueurIA)
				JouerIA();
		}

		private void JouerIA()
		{
			DispatcherTimer delai = new DispatcherTimer();
			delai.Interval = TimeSpan.FromMilliseconds(500);
			delai.Tick += (s, e) =>
			{
				delai.Stop();
				if (!partieTerminee)
				{
					int col = joueurActif.ChoisirColonne(plateau);
					if (col >= 0)
						EffectuerCoup(col);
				}
			};
			delai.Start();
		}

		private void FinPartie(string message)
		{
			partieTerminee = true;
			timer?.Stop();
			NavigationService.Navigate(new PageFinPartie(message));
		}

		private void MettreAJourInfoJoueur()
		{
			string symbole = (joueurActif.GetPion() == Pion.JOUEUR1) ? "X" : "O";
			TxtJoueurActif.Text = $"Tour : {joueurActif.GetNom()} ({symbole})";
			TxtJoueurActif.Foreground =
				(joueurActif.GetPion() == Pion.JOUEUR1) ? Brushes.Salmon : Brushes.Gold;
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			tempsRestant--;
			TxtTimer.Text = tempsRestant + " s";

			if (tempsRestant <= 5)
				TxtTimer.Foreground = Brushes.Red;
			else
				TxtTimer.Foreground = Brushes.White;

			if (tempsRestant <= 0)
			{
				timer.Stop();
				MessageBox.Show(joueurActif.GetNom() + " a dépassé le temps imparti !",
								"Temps écoulé", MessageBoxButton.OK, MessageBoxImage.Information);
				joueurActif = (joueurActif == joueur1) ? joueur2 : joueur1;
				MettreAJourInfoJoueur();
				ResetTimer();
				if (joueurActif is JoueurIA) JouerIA();
			}
		}

		private void ResetTimer()
		{
			if (tempsParTour > 0)
			{
				tempsRestant = tempsParTour;
				TxtTimer.Text = tempsRestant + " s";
				TxtTimer.Foreground = Brushes.White;
				timer?.Start();
			}
		}

		private void BtnRetour_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult rep = MessageBox.Show(
				"Voulez-vous vraiment quitter la partie en cours ?",
				"Quitter", MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (rep == MessageBoxResult.Yes)
			{
				timer?.Stop();
				NavigationService.Navigate(new PageMenu());
			}
		}

		private void BtnAbandonner_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult rep = MessageBox.Show(
				"Confirmer l'abandon de la partie ?",
				"Abandonner", MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (rep == MessageBoxResult.Yes)
				FinPartie("Partie abandonnée !");
		}

		private void BtnPause_Click(object sender, RoutedEventArgs e)
		{
			if (timer == null) return;

			if (timer.IsEnabled)
			{
				timer.Stop();
				BtnPause.Content = "Reprendre";
			}
			else
			{
				timer.Start();
				BtnPause.Content = "Pause";
			}
		}

		private void BtnSauvegarder_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Partie sauvegardée",
							"Sauvegarde", MessageBoxButton.OK, MessageBoxImage.Information);
		}
	}
}