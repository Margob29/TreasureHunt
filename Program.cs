using System;
using System.Linq;
using System.Diagnostics;

namespace Projet2021Tresor
{
    class Program
    {

        //Initialisation des constantes
        public const int MINE = 1;
        public const int TRESOR = 2;


        static void Main(string[] args)
        {
            int lignes, colonnes;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine("------------------------------ Bienvenue sur le jeu de la chasse au trésor ! ------------------------------ \n");
            Console.WriteLine("Souhaitez-vous afficher les règles du jeu ? (Oui/Non)");
            Console.ResetColor();
            string reponse = Console.ReadLine();
            //Affichage des règles
            if (reponse.ToUpper().Equals("OUI"))
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.BackgroundColor = ConsoleColor.White;
                Console.WriteLine("\n*********************************** Règles du jeu ***********************************\n");
                Console.ResetColor();

                Console.WriteLine("Objectif : Trouver tous les trésors et les cases vides sur la carte sans tomber sur une mine !\n ");
                Console.WriteLine("A chaque tour, vous devez choisir une case à réveler pour afficher un décompte.");
                Console.WriteLine("Ce décompte est calculé à partir du nombre de cases adjacentes minées ou ayant un trésor.");
                Console.WriteLine("Une case trésor prend la valeur 2 tandis qu'une case mine prend la valeur 1.");
                Console.WriteLine("Remarque : 'ND' signifie que la case n'est pas découverte.");
                Console.WriteLine("Attention : il peut y avoir 1, 2 ou 3 trésors présents sur la carte.\nBon courage !");

                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.BackgroundColor = ConsoleColor.White;
                Console.WriteLine("\n*************************************************************************************\n");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("C'est parti !");
            }

            //Choix de la difficulté
            int niveau = ChoisirDifficulte();

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine("\nEntrer le nombre de lignes de la grille de jeu souhaitées :");
            Console.ResetColor();
            string nbrLignes = Console.ReadLine();

            //Vérifie que le nombre de lignes entré soit bien un nombre plus grand que 1
            while (!int.TryParse(nbrLignes, out lignes) || lignes <= 1)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Attention ! Veuillez entrer un nombre pour les lignes plus grand que 1 :");
                Console.ResetColor();
                nbrLignes = Console.ReadLine();
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine("Entrer le nombre de colonnes de la grille de jeu souhaitées :");
            Console.ResetColor();
            string nbrColonnes = Console.ReadLine();

            //Vérifie que le nombre de colonnes entré soit bien un nombre plus grand que 1
            while (!int.TryParse(nbrColonnes, out colonnes) || colonnes <= 1)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Attention ! Veuillez entrer un nombre pour les colonnes plus grand que 1 :");
                Console.ResetColor();
                nbrColonnes = Console.ReadLine();
            }

            //Noms de la variable pour incrémenter l'index du tableau des scores, et pour compter le nombre de cases vides.
            int indexScore = 0;
            int countCasesVides = 0;

            //Tableau des scores obtenus : cela correspond au nombre de coups joués en une partie.
            int[] score = new int[100];

            //Fonction permettant de débuter la partie
            LancerPartie(lignes, colonnes, niveau, ref countCasesVides, score, ref indexScore);

        }



        /// <summary>
        /// Fonction permettant à l'utilisateur de choisir le niveau de difficulté de la grille.
        /// </summary>
        /// <returns></returns>
        static int ChoisirDifficulte()
        {
            int difficulte;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine("Veuillez entrer le numéro du niveau souhaité : ");
            Console.ResetColor();

            Console.WriteLine("- n°1: Facile");
            Console.WriteLine("- n°2: Intermédiaire");
            Console.WriteLine("- n°3: Difficile");

            string niveau = Console.ReadLine();

            //On s'assure que le nombre rentré corresponde à un niveau
            while (!int.TryParse(niveau, out difficulte) || difficulte > 3 || difficulte < 1)
            {
                Console.WriteLine("Désolée nous n'avons pas compris votre demande.\nVeuillez entrer le numéro du niveau souhaité : [n°0: Facile, n°1: Intermédiaire,  n°2: Difficile]");
                niveau = Console.ReadLine();
            }

            return difficulte;
        }



        /// <summary>
        /// Fonction permettant d'initialiser la grille de référence, ainsi que de créer la grille de jeu.
        /// </summary>
        /// <param name="grilleRef"></param>
        /// <param name="nbMines"></param>
        /// <param name="nbTresors"></param>
        /// <param name="choixCaseUser"></param>
        /// <returns></returns>
        static string[,] InitialiserGrille(string[,] grilleRef, int nbMines, int nbTresors, out int[] choixCaseUser)
        {
            // Taille de la grille
            int tailleGrille = grilleRef.GetLength(0) * grilleRef.GetLength(1);

            //Initiliasation de la grille de jeu 
            string[,] grilleJeu = CreerGrille(grilleRef.GetLength(0), grilleRef.GetLength(1), "ND");
            AfficherGrille(grilleJeu);

            // Initialisation du J de la case choisie 
            choixCaseUser = ChoisirCase(grilleRef.GetLength(0), grilleRef.GetLength(1));

            // Inscrit dans la case choisie le résultat pour la grille de Jeu
            grilleRef[choixCaseUser[0], choixCaseUser[1]] = "J";

            //Répartition des mines dans la grille
            RepartirObjets(grilleRef, nbMines, "M");

            //Répartition des trésors dans la grille
            RepartirObjets(grilleRef, nbTresors, "T");

            //On enlève le J
            grilleRef[choixCaseUser[0], choixCaseUser[1]] = "  ";

            return grilleJeu;

        }



        /// <summary>
        /// Fonction qui retourne une grille vide de dimensions lignes*colonnes.
        /// </summary>
        /// <param name="lignes"></param>
        /// <param name="colonnes"></param>
        /// <param name="caseJeu"></param>
        /// <returns>Retourne une grille de type string et de taille lignes x colonnes. </returns>
        static string[,] CreerGrille(int lignes, int colonnes, string caseJeu)
        {
            string[,] grille = new string[lignes, colonnes];

            for (int i = 0; i < lignes; i++)
            {
                for (int j = 0; j < colonnes; j++) grille[i, j] = caseJeu;
            }

            return grille;
        }



        /// <summary>
        /// Procédure permettant de répartir aléatoirement les bombes et/ou les trésors
        /// </summary>
        /// <param name="grilleRef"></param>
        /// <param name="nbObjets"></param>
        /// <param name="objet"></param>
        static void RepartirObjets(string[,] grilleRef, int nbObjets, string objet)
        {
            Random rnd = new Random();
            int rndLigne, rndColonne;

            for (int i = 0; i < nbObjets; i++)
            {
                //La méthode .Equals() permet de vérifier que l'utilisateur n'a pas choisi cette case au préalable
                do
                {
                    rndLigne = rnd.Next(0, grilleRef.GetLength(0));
                    rndColonne = rnd.Next(0, grilleRef.GetLength(1));
                } while (!grilleRef[rndLigne, rndColonne].Equals("  "));

                grilleRef[rndLigne, rndColonne] = objet;

            }
        }



        /// <summary>
        /// Fonction permettant de lancer une partie de jeu.
        /// </summary>
        /// <param name="lignes"></param>
        /// <param name="colonnes"></param>
        /// <param name="countCasesVides"></param>
        /// <param name="score"></param>
        /// <param name="indexScore"></param>
        static void LancerPartie(int lignes, int colonnes, int niveau, ref int countCasesVides, int[] score, ref int indexScore)
        {
            string[,] grilleRef = CreerGrille(lignes, colonnes, "  ");

            Random rnd = new Random();
            int nbMines = 0;

            //Initialisation du nombre de mines en fonction de la difficulté
            switch (niveau)
            {
                case 1:
                    if (grilleRef.Length <= 3) nbMines = 1;
                    else nbMines = rnd.Next(grilleRef.GetLength(0) / 2, (grilleRef.Length / 4) + 1);
                    break;
                case 2:
                    if (grilleRef.Length == 2) nbMines = 1;
                    else nbMines = rnd.Next(grilleRef.GetLength(0) / 2, (grilleRef.Length / 3) + 1);
                    break;
                case 3:
                    nbMines = rnd.Next(grilleRef.GetLength(0) / 2, (grilleRef.Length / 2) + 1);
                    break;
            }

            int nbTresors = rnd.Next(1, 4);
            string[,] grilleJeu = InitialiserGrille(grilleRef, nbMines, nbTresors, out int[] coord);

            //Remplir la grille avec les numéros
            RemplirGrilleRef(grilleRef);

            //Dévoilement de la case choisie au début
            DecouvrirCase(grilleRef, grilleJeu, coord[0], coord[1], ref countCasesVides);
            Jouer(grilleRef, grilleJeu, nbTresors, ref countCasesVides, ref score, ref indexScore);

        }




        /// <summary>
        /// Fonction qui retourne un tableau de type int contenant la ligne et la colonne où le joueur souhaite jouer.
        /// </summary>
        /// <returns></returns>
        static int[] ChoisirCase(int ligneTable, int colonneTable)
        {
            int[] choix = new int[2];
            string numeroLigne, numeroColonne;
            int ligne, colonne;

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine("Entrer le numéro de la ligne souhaitée pour jouer :");
            Console.ResetColor();
            numeroLigne = Console.ReadLine();

            //Si l'utilisateur entre X, la partie se termine
            if (int.TryParse(numeroLigne, out ligne) && ligne == 0)
            {
                choix[0] = -1;
                choix[1] = -1;
                return choix;
            }

            //Vérifie que la ligne entrée soit bien un nombre entre 1 et le nombre de lignes
            while (!int.TryParse(numeroLigne, out ligne) || ligne < 1 || ligne > ligneTable)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Attention ! Veuillez entrer un nombre entre 1 et " + ligneTable + " :");
                Console.ResetColor();
                numeroLigne = Console.ReadLine();
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine("Entrer le numéro de la colonne souhaitée pour jouer :");
            Console.ResetColor();

            numeroColonne = Console.ReadLine();
            //Vérifie que la colonne entrée soit bien un nombre entre 1 et le nombre de colonnes
            while (!int.TryParse(numeroColonne, out colonne) || colonne < 1 || colonne > colonneTable)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Attention ! Veuillez entrer un nombre entre 1 et " + colonneTable + " :");
                Console.ResetColor();
                numeroColonne = Console.ReadLine();
            }

            //on retire 1 à la ligne et la colonne pour l'indexation des tableaux qui commence à 0
            choix[0] = ligne - 1;
            choix[1] = colonne - 1;

            return choix;
        }



        /// <summary>
        /// Fonction qui permet de compter le nombre de cases vides dans la grille de référence. 
        /// </summary>
        /// <param name="grilleRef"></param>
        /// <returns></returns>
        static int CountCasesVides(string[,] grilleRef)
        {
            int nbCaseVide = 0;
            for (int ligne = 0; ligne < grilleRef.GetLength(0); ligne++)
            {
                for (int colonne = 0; colonne < grilleRef.GetLength(1); colonne++)
                {
                    if (grilleRef[ligne, colonne] == "  ") nbCaseVide++;
                }
            }
            return nbCaseVide;
        }



        /// <summary>
        /// Fonction principale permettant à l'utilisateur de jouer sur la grille de jeu.
        /// </summary>
        /// <param name="grilleRef"></param>
        /// <param name="grilleJeu"></param>
        /// <param name="nbTresors"></param>
        /// <param name="countCasesVides"></param>
        static void Jouer(string[,] grilleRef, string[,] grilleJeu, int nbTresors, ref int countCasesVides, ref int[] score, ref int indexScore)
        {
            int countTresor = 0, countMine = 0, nbCoups = 0;
            bool stop = false;
            int nbCasesVide = CountCasesVides(grilleRef);
            AfficherGrille(grilleJeu);
            int[] choixCaseUser;

            //Noms des variables pour la ligne et la colonne choisies par l'utilisateur
            int lUser, cUser;

            Console.WriteLine("------------------Début de la partie------------------\n");

            Console.WriteLine("Si vous souhaitez arrêter, vous pouvez taper 0 lorsqu'il vous est demandé de choisir la ligne.\n");

            //Fonctionnalité supplémentaire : Permet de calculer le temps d'une partie de jeu
            Stopwatch tempsPartie = new Stopwatch();
            tempsPartie.Start();

            while ((countTresor < nbTresors || countCasesVides < nbCasesVide) && countMine < 1 && !stop)
            {
                do
                {
                    //On demande une case tant que celle choisie est déjà découverte
                    choixCaseUser = ChoisirCase(grilleJeu.GetLength(0), grilleJeu.GetLength(1));
                    lUser = choixCaseUser[0];
                    cUser = choixCaseUser[1];
                    if ((lUser == -1) && (cUser == -1))
                    {
                        Console.WriteLine("Souhaitez-vous vraiment quitter la partie ? (Oui/Non)");
                        string quitterPartie = Console.ReadLine();

                        if (quitterPartie.ToUpper().Equals("OUI"))
                        {
                            stop = true;
                            break;
                        }
                        else
                        {
                            choixCaseUser = ChoisirCase(grilleJeu.GetLength(0), grilleJeu.GetLength(1));
                            lUser = choixCaseUser[0];
                            cUser = choixCaseUser[1];
                        }
                    }
                    if (grilleJeu[lUser, cUser].Equals("  "))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Attention ! Vous ne pouvez pas jouer sur une case vide.\n");
                        Console.ResetColor();
                    }
                    else if (!grilleJeu[lUser, cUser].Equals("ND") && lUser > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Attention ! Vous ne pouvez pas jouer sur une case découverte.\n");
                        Console.ResetColor();
                    }
                } while (!grilleJeu[lUser, cUser].Equals("ND"));

                if ((lUser != -1) && (cUser != -1))
                {
                    if (grilleRef[lUser, cUser].Equals("M"))
                    {
                        countMine += 1;
                        DecouvrirCase(grilleRef, grilleJeu, lUser, cUser, ref countCasesVides);

                        //Appel de la fonction permettant de découvrir toutes les mines. 
                        DecouvrirMinesTresors(grilleRef, grilleJeu);
                    }
                    else if (grilleRef[lUser, cUser].Equals("T"))
                    {
                        countTresor += 1;
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Bravo ! Vous êtes tombé(e) sur un trésor.");
                        Console.ResetColor();
                        DecouvrirCase(grilleRef, grilleJeu, lUser, cUser, ref countCasesVides);
                    }
                    else
                    {
                        DecouvrirCase(grilleRef, grilleJeu, lUser, cUser, ref countCasesVides);
                    }
                    //On imcéremente le nombre de coups
                    ++nbCoups;

                    AfficherGrille(grilleJeu);
                    Console.WriteLine("---------------------------------------------- \n");
                }
                
            }

            //Si le joueur décide de quitter la partie

            if (stop)
            {
                Console.WriteLine("Vous venez de quitter la partie.");
            }
            else
            {
                //Si le joueur est tombé sur une mine, la partie est finie
                if (countMine == 1)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("La partie est terminée... Vous êtes tombé(e) sur une mine.");
                    Console.ResetColor();
                    tempsPartie.Stop();
                    TimeSpan temps = tempsPartie.Elapsed;
                    Console.WriteLine("Temps de partie : " + temps.Minutes + " minutes et " + temps.Seconds + " secondes.");
                }

                //Si toutes les cases vides sont découvertes et que tous les trésors sont trouvés la partie est gagnée
                else if (countTresor == nbTresors && nbCasesVide == countCasesVides)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Bravo ! Vous avez trouvé tous les trésors de l'île en " + nbCoups + " coups.");
                    Console.ResetColor();
                    tempsPartie.Stop();
                    TimeSpan temps = tempsPartie.Elapsed;
                    Console.WriteLine("Temps de partie : " + temps.Minutes + " minutes et " + temps.Seconds + " secondes.\n");

                    // Permet de tester si le score du nombre de coups est déjà présent dans le tableau des scores. -1 indique qu'il n'y est pas.
                    if (Array.IndexOf(score, nbCoups) == -1)
                    {
                        score[indexScore] = nbCoups;
                        indexScore++;
                    }

                    //Trie du tableau score du meilleur au moins bon.
                    Array.Sort(score);
                    if (nbCoups >= score.Max())
                        Console.WriteLine("Vous obtenez le meilleur score !\n");

                    Console.WriteLine("Voici la liste des premiers meilleurs résultats : \n");
                    int classement = 1;
                    for (int i = score.Length - 1; i >= score.Length - 3; i--)
                    {
                        if (score[i] != 0)
                        {
                            Console.WriteLine("n°" + classement + " : " + score[i]);
                            classement += 1;
                        }
                    }
                    Console.WriteLine();
                }

                //Pouvoir relancer une partie :
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.WriteLine("\nSouhaitez-vous rejouer ? (Oui/Non)");
                Console.ResetColor();
                string rejouer = Console.ReadLine();

                if (rejouer.ToUpper().Equals("OUI"))
                {
                    Console.Clear();
                    int niveau = ChoisirDifficulte();
                    countCasesVides = 0;
                    LancerPartie(grilleJeu.GetLength(0), grilleJeu.GetLength(1), niveau, ref countCasesVides, score, ref indexScore);
                }
                else
                    Console.WriteLine("Merci et à bientôt !");
            }

        }



        /// <summary>
        /// Procédure permettant de découvrir les cases du jeu.
        /// </summary>
        /// <param name="grilleRef"></param>
        /// <param name="grilleJeu"></param>
        /// <param name="ligne"></param>
        /// <param name="colonne"></param>
        /// <param name="countCasesVides"></param>
        static void DecouvrirCase(string[,] grilleRef, string[,] grilleJeu, int ligne, int colonne, ref int countCasesVides)
        {
            if (grilleJeu[ligne, colonne].Equals("ND"))
            {
                if (grilleRef[ligne, colonne].Equals("  "))
                {
                    grilleJeu[ligne, colonne] = grilleRef[ligne, colonne];
                    //On incrémente countCasesVides pour savoir si elles ont toutes été découvertes
                    countCasesVides++;
                    if (colonne < grilleJeu.GetLength(1) - 1)
                        DecouvrirCase(grilleRef, grilleJeu, ligne, colonne + 1, ref countCasesVides);
                    if (colonne > 0)
                        DecouvrirCase(grilleRef, grilleJeu, ligne, colonne - 1, ref countCasesVides);
                    if (ligne < grilleJeu.GetLength(0) - 1)
                        DecouvrirCase(grilleRef, grilleJeu, ligne + 1, colonne, ref countCasesVides);
                    if (ligne > 0)
                        DecouvrirCase(grilleRef, grilleJeu, ligne - 1, colonne, ref countCasesVides);
                }
                else
                    grilleJeu[ligne, colonne] = " " + grilleRef[ligne, colonne];

            }
        }



        /// <summary>
        /// Procédure permettant d'inscrire les numéros sur les cases en fonction des trésors et des mines.
        /// </summary>
        /// <param name="grilleRef"></param>
        static void RemplirGrilleRef(string[,] grilleRef)
        {
            for (int ligne = 0; ligne < grilleRef.GetLength(0); ligne++)
            {
                for (int colonne = 0; colonne < grilleRef.GetLength(1); colonne++)
                {
                    if (grilleRef[ligne, colonne].Equals("M"))
                    {
                        UpdateCountCases(grilleRef, ligne, colonne, MINE);
                    }
                    else if (grilleRef[ligne, colonne].Equals("T"))
                    {
                        UpdateCountCases(grilleRef, ligne, colonne, TRESOR);
                    }
                }
            }
        }



        /// <summary>
        /// Procédure permettant de mettre à jour les cases de la grille de référence.
        /// </summary>
        /// <param name="grilleRef"></param>
        /// <param name="ligne"></param>
        /// <param name="colonne"></param>
        /// <param name="nb">Le paramètre nb correspond soit à la constante MINE ou la constante TRESOR définies au début du programme.</param>
        static void UpdateCountCases(string[,] grilleRef, int ligne, int colonne, int nb)
        {
            int number;

            for (int l = -1; l <= 1; l++) // Boucle for pour les lignes autour de la case courante 
            {
                //Vérification que la ligne soit bien dans la grille
                if (ligne + l >= 0 && ligne + l <= grilleRef.GetLength(0) - 1)
                {
                    for (int c = -1; c <= 1; c++) //  Boucle for pour les colonnes autour de la case courante 
                    {
                        //Vérification que la colonne soit bien dans la grille
                        if (colonne + c >= 0 && colonne + c <= grilleRef.GetLength(1) - 1)
                        {
                            if (!grilleRef[ligne + l, colonne + c].Equals("M") && !grilleRef[ligne + l, colonne + c].Equals("T"))
                            {
                                number = ConvertCase(grilleRef, ligne + l, colonne + c);
                                number += nb;
                                grilleRef[ligne + l, colonne + c] = number.ToString();
                            }

                        }
                    }
                }
            }
        }



        /// <summary>
        /// Fonction permettant de convertir une chaine de caractère en un entier.  
        /// </summary>
        /// <param name="grilleRef"></param>
        /// <param name="ligne"></param>
        /// <param name="colonne"></param>
        /// <returns>L'entier retourné correspond à la conversion</returns>
        static int ConvertCase(string[,] grilleRef, int ligne, int colonne)
        {
            int nombre;
            if (grilleRef[ligne, colonne].Equals("  "))
                nombre = 0;
            else
                nombre = Convert.ToInt32(grilleRef[ligne, colonne]);
            return nombre;
        }



        /// <summary>
        /// Procédure permettant de découvrir toutes les cases de la grille de jeu avec des mines. 
        /// </summary>
        /// <param name="grilleJeu"></param>
        /// <param name="grilleRef"></param>
        static void DecouvrirMinesTresors(string[,] grilleRef, string[,] grilleJeu)
        {
            for (int ligne = 0; ligne < grilleRef.GetLength(0); ligne++)
            {
                for (int colonne = 0; colonne < grilleRef.GetLength(1); colonne++)
                {
                    if (grilleRef[ligne, colonne].Equals("M") || grilleRef[ligne, colonne].Equals("T"))
                        grilleJeu[ligne, colonne] = " " + grilleRef[ligne, colonne];
                }
            }
        }



        /// <summary>
        /// Procédure permettant d'afficher une grille de type string[,]. 
        /// Afin d'avoir plus de lisibilité, l'utilisateur pourra voir l'indice de chaque ligne/colonne.
        /// </summary>
        /// <param name="grille"></param>
        static void AfficherGrille(string[,] grille)
        {
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.DarkGray;

            //Affichage des noms des colonnes
            for (int colonne = 0; colonne <= grille.GetLength(1); colonne++)
            {
                if (colonne == 0) Console.Write("   | ");
                else if (colonne < 10) Console.Write("C" + colonne + " | ");
                else Console.Write("C" + colonne + "| ");
            }
            Console.WriteLine();


            for (int ligne = 0; ligne < grille.GetLength(0); ligne++)
            {
                Console.Write("    ");

                //Séparation des lignes
                if (grille.GetLength(1) % 2 == 0)
                {
                    for (int colonne = 0; colonne <= grille.GetLength(1) - 1; colonne++) Console.Write("-----");
                }
                else
                {
                    for (int colonne = 0; colonne < grille.GetLength(1); colonne++) Console.Write("-----");
                }
                Console.Write(" \n");

                for (int colonne = 0; colonne < grille.GetLength(1); colonne++)
                {
                    //Affichage du nom des lignes
                    if (colonne == 0)
                    {
                        if (ligne < 9) Console.Write("L" + (ligne + 1) + " | ");
                        else Console.Write("L" + (ligne + 1) + "| ");
                    }
                    if (grille[ligne, colonne].Equals(" T"))
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(grille[ligne, colonne]);
                        Console.ResetColor();
                    }
                    else if (grille[ligne, colonne].Equals(" M"))
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(grille[ligne, colonne]);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(grille[ligne, colonne]);
                        Console.ResetColor();
                    }
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.Write(" | ");
                }
                Console.WriteLine();
            }
            Console.ResetColor();

            Console.WriteLine();
        }

    }
}