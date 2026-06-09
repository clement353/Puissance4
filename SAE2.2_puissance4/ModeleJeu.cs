// ModeleJeu.cs
// Contient toutes les classes du jeu : Pion, Plateau, Joueur, JoueurHumain, JoueurIA

using System;
using System.Collections.Generic;

namespace SAE2._2_puissance4
{

    // enum d'une case
    public enum Pion
    {
        VIDE    = 0,
        JOUEUR1 = 1,
        JOUEUR2 = 2
    }

    //  enum niveau de difficulte de l'IA
    public enum Difficulte
    {
        FACILE,
        MOYEN,
        DIFFICILE
    }


    public class Plateau
    {
        private Pion[,] grille;
        private int lignes;
        private int colonnes;
        private int aligner;

        public Plateau(int lignes, int colonnes, int aligner)
        {
            this.lignes   = lignes;
            this.colonnes = colonnes;
            this.aligner  = aligner;
            this.grille   = new Pion[lignes, colonnes];
            Reinitialiser();
        }

        public int GetLignes()            { return lignes; }
        public int GetColonnes()          { return colonnes; }
        public int GetAligner()           { return aligner; }
        public Pion GetCase(int l, int c) { return grille[l, c]; }

        public void Reinitialiser()
        {
            for (int l = 0; l < lignes; l++)
                for (int c = 0; c < colonnes; c++)
                    grille[l, c] = Pion.VIDE;
        }

        public bool ColonneValide(int c)
        {
            if (c < 0 || c >= colonnes) return false;
            return grille[0, c] == Pion.VIDE;
        }

        public int LigneDisponible(int c)
        {
            for (int l = lignes - 1; l >= 0; l--)
                if (grille[l, c] == Pion.VIDE) return l;
            return -1;
        }

        public int Poser(int col, Pion p)
        {
            int lig = LigneDisponible(col);
            grille[lig, col] = p;
            return lig;
        }

        public void Retirer(int col)
        {
            for (int l = 0; l < lignes; l++)
            {
                if (grille[l, col] != Pion.VIDE)
                {
                    grille[l, col] = Pion.VIDE;
                    return;
                }
            }
        }

        public bool AGagne(Pion p)
        {
            for (int l = 0; l < lignes; l++)
                for (int c = 0; c <= colonnes - aligner; c++)
                    if (Alignement(l, c, 0, 1, p)) return true;

            for (int l = 0; l <= lignes - aligner; l++)
                for (int c = 0; c < colonnes; c++)
                    if (Alignement(l, c, 1, 0, p)) return true;

            for (int l = 0; l <= lignes - aligner; l++)
                for (int c = 0; c <= colonnes - aligner; c++)
                    if (Alignement(l, c, 1, 1, p)) return true;

            for (int l = aligner - 1; l < lignes; l++)
                for (int c = 0; c <= colonnes - aligner; c++)
                    if (Alignement(l, c, -1, 1, p)) return true;

            return false;
        }

        public bool EstPleine()
        {
            for (int c = 0; c < colonnes; c++)
                if (ColonneValide(c)) return false;
            return true;
        }

        private bool Alignement(int l, int c, int dl, int dc, Pion p)
        {
            for (int k = 0; k < aligner; k++)
                if (grille[l + k * dl, c + k * dc] != p) return false;
            return true;
        }
    }

    public abstract class Joueur
    {
        protected string nom;
        protected Pion pion;

        public Joueur(string nom, Pion pion)
        {
            this.nom  = nom;
            this.pion = pion;
        }

        public string GetNom()  { return nom; }
        public Pion   GetPion() { return pion; }

        public abstract int ChoisirColonne(Plateau plateau);
    }

 
    public class JoueurHumain : Joueur
    {
        public JoueurHumain(string nom, Pion pion) : base(nom, pion) { }

        public override int ChoisirColonne(Plateau plateau)
        {
            // Dans le WPF, le clic souris gere le choix de colonne
            // Cette methode n'est pas appelee directement
            return -1;
        }
    }

    public class JoueurIA : Joueur
    {
        private Difficulte diff;
        private int profondeur;
        private Pion pionAdversaire;
        private Random rng;

        public JoueurIA(string nom, Pion pion, Difficulte diff) : base(nom, pion)
        {
            this.diff            = diff;
            this.rng             = new Random();
            this.pionAdversaire  = (pion == Pion.JOUEUR1) ? Pion.JOUEUR2 : Pion.JOUEUR1;

            if (diff == Difficulte.FACILE)    profondeur = 1;
            else if (diff == Difficulte.MOYEN) profondeur = 3;
            else                               profondeur = 6; 
        }

        public override int ChoisirColonne(Plateau plateau)
        {
            List<int> colsValides = new List<int>();
            for (int c = 0; c < plateau.GetColonnes(); c++)
                if (plateau.ColonneValide(c))
                    colsValides.Add(c);

            if (colsValides.Count == 0) return -1;

            //coup aleatoir selon le niveau choisi de l'ia
            int seuil = 0;
            if (diff == Difficulte.FACILE) seuil = 7;
            if (diff == Difficulte.MOYEN)  seuil = 3;

            if (seuil > 0 && rng.Next(10) < seuil)
                return colsValides[rng.Next(colsValides.Count)];

            // Minimax
            int meilleurCol   = colsValides[0];
            int meilleurScore = int.MinValue;

            foreach (int c in colsValides)
            {
                plateau.Poser(c, pion);
                int score = Minimax(plateau, profondeur - 1, false, int.MinValue, int.MaxValue);
                plateau.Retirer(c);

                if (score > meilleurScore)
                {
                    meilleurScore = score;
                    meilleurCol   = c;
                }
            }
            return meilleurCol;
        }

        private int Minimax(Plateau plateau, int prof, bool estMax, int alpha, int beta)
        {
            if (plateau.AGagne(pion))           return  1000000 + prof;
            if (plateau.AGagne(pionAdversaire)) return -1000000 - prof;
            if (plateau.EstPleine())            return 0;
            if (prof == 0)                      return Heuristique(plateau);

            if (estMax)
            {
                int meilleur = int.MinValue;
                for (int c = 0; c < plateau.GetColonnes(); c++)
                {
                    if (!plateau.ColonneValide(c)) continue;
                    plateau.Poser(c, pion);
                    int score = Minimax(plateau, prof - 1, false, alpha, beta);
                    plateau.Retirer(c);
                    if (score > meilleur) meilleur = score;
                    if (meilleur > alpha) alpha = meilleur;
                    if (alpha >= beta) break;
                }
                return meilleur;
            }
            else
            {
                int meilleur = int.MaxValue;
                for (int c = 0; c < plateau.GetColonnes(); c++)
                {
                    if (!plateau.ColonneValide(c)) continue;
                    plateau.Poser(c, pionAdversaire);
                    int score = Minimax(plateau, prof - 1, true, alpha, beta);
                    plateau.Retirer(c);
                    if (score < meilleur) meilleur = score;
                    if (meilleur < beta)  beta = meilleur;
                    if (alpha >= beta) break;
                }
                return meilleur;
            }
        }

        private int Heuristique(Plateau plateau)
        {
            return ScoreTotal(plateau, pion) - ScoreTotal(plateau, pionAdversaire);
        }

        private int ScoreTotal(Plateau plateau, Pion p)
        {
            int total = 0;
            Pion adv  = (p == Pion.JOUEUR1) ? Pion.JOUEUR2 : Pion.JOUEUR1;
            int A     = plateau.GetAligner();
            int[,] dirs = { {0,1},{1,0},{1,1},{-1,1} };

            for (int d = 0; d < 4; d++)
            {
                int dl = dirs[d, 0], dc = dirs[d, 1];
                for (int l = 0; l < plateau.GetLignes(); l++)
                {
                    for (int c = 0; c < plateau.GetColonnes(); c++)
                    {
                        int el = l + (A - 1) * dl;
                        int ec = c + (A - 1) * dc;
                        if (el < 0 || el >= plateau.GetLignes())   continue;
                        if (ec < 0 || ec >= plateau.GetColonnes()) continue;

                        int cntP = 0, cntAdv = 0;
                        for (int k = 0; k < A; k++)
                        {
                            Pion cellule = plateau.GetCase(l + k * dl, c + k * dc);
                            if (cellule == p)   cntP++;
                            if (cellule == adv) cntAdv++;
                        }
                        if (cntAdv == 0) total += ScoreFenetre(cntP, A);
                    }
                }
            }

            int colCentre = plateau.GetColonnes() / 2;
            for (int l = 0; l < plateau.GetLignes(); l++)
                if (plateau.GetCase(l, colCentre) == p) total += 3;

            return total;
        }

        private int ScoreFenetre(int cnt, int A)
        {
            if (cnt == A)     return 1000;
            if (cnt == A - 1) return 50;
            if (cnt == A - 2) return 10;
            if (cnt == 1)     return 1;
            return 0;
        }
    }
}
