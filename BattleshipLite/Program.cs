using BattleshipLiteLibrary;
using BattleshipLiteLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipLite
{
    public class Program
    {
        static void Main(string[] args)
        {
            WelcomeMessage();

            PlayerInfoModel activePlayer = CreatePlayer("Player 1");
            PlayerInfoModel opponent = CreatePlayer("Player 2");
            PlayerInfoModel winner = null;

            do
            {
                //Display grid from activePlayer on where they fired
                DisplayShotGrid(activePlayer);

                //Ask activePlayer for a shot
                //Determine if it is a valid shot
                //Determine shot results
                RecordPlayerShot(activePlayer, opponent);

                //Determine if the game is over
                bool doesGameContinue = GameLogic.PlayerStillActive(opponent);

                //If over, set activePlayer as winner
                if (!doesGameContinue)
                {
                    winner = activePlayer;
                }
                //else swap positions (activePlayer to opponent)
                else
                {
                    (activePlayer, opponent) = (opponent, activePlayer);
                }


            } while (winner == null);

            IdentifyWinner(winner);

            Console.ReadLine();
        }

        private static void IdentifyWinner(PlayerInfoModel winner)
        {
            Console.WriteLine($"Congratulations {winner.UsersName}, you win!");
            Console.WriteLine($"{winner.UsersName} took {GameLogic.GetShotCount(winner)} shots.");
        }

        private static void RecordPlayerShot(PlayerInfoModel activePlayer, PlayerInfoModel opponent)
        {
            bool isValidShot = false;
            char row = ' ';
            int column = 0;
            //Ask for shot Eg. "B2"
            //Determine row and column by splitting ^
            //Determine if ^ is a valid shot
            //If invalid shot go back to the beginnninig
            do
            {
                string shot = AskForShot(activePlayer);
                try
                {
                    (row, column) = GameLogic.SplitShotIntoRowAndColumn(shot);
                    isValidShot = GameLogic.ValidateShot(activePlayer, row, column);
                }
                catch (Exception)
                {
                    Console.WriteLine("Error: Input string was not in the correct format");
                    isValidShot = false;
                }

                if (!isValidShot)
                {
                    Console.WriteLine("Invalid shot location, please try again.");
                }

            } while (!isValidShot);

            //Determine and record shot results
            bool isAHit = GameLogic.IdentifyShotResult(opponent, row, column);
            if (isAHit)
            {
                MarkShipAsSunk(opponent, row, column);
                Console.WriteLine("It was a hit!\n");
            }
            else
            {
                Console.WriteLine("You missed!\n");
            }
            GameLogic.MarkShotResult(activePlayer, row, column, isAHit);

        }

        private static void MarkShipAsSunk(PlayerInfoModel opponent, char row, int column)
        {
            foreach (GridSpotModel ship in opponent.ShipLocations)
            {
                if (row == ship.SpotLetter && column == ship.SpotNumber)
                {
                    ship.Status = GridSpotStatus.Sunk;
                    return;
                }
            }
        }

        private static string AskForShot(PlayerInfoModel player)
        {
            Console.Write($"Please enter your shot {player.UsersName}: ");
            string output = Console.ReadLine();

            return output;
        }

        private static void DisplayShotGrid(PlayerInfoModel player)
        {
            char CurrentRow = player.ShotGrid[0].SpotLetter;
            foreach (GridSpotModel gridSpot in player.ShotGrid)
            {
                if (gridSpot.SpotLetter != CurrentRow)
                {
                    Console.WriteLine();
                    CurrentRow = gridSpot.SpotLetter;
                }

                if (gridSpot.Status == GridSpotStatus.Empty)
                {
                    Console.Write($" {gridSpot.SpotLetter}{gridSpot.SpotNumber} "); 
                }
                else if (gridSpot.Status == GridSpotStatus.Hit)
                {
                    Console.Write(" X  ");
                }
                else if (gridSpot.Status == GridSpotStatus.Miss)
                {
                    Console.Write(" 0  ");
                }
                else
                {
                    Console.Write(" ?  ");
                }
            }
            Console.WriteLine("\n");
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine("Welcome to Battleship Lite");
            Console.WriteLine("Created by John Tony");
            Console.WriteLine();
        }

        private static PlayerInfoModel CreatePlayer(string playerTitle)
        {
            PlayerInfoModel output = new PlayerInfoModel();

            Console.WriteLine($"Player information for {playerTitle}");
            //Ask user for their name
            output.UsersName = AskForUsersName();

            //Load shot grid
            GameLogic.InitializeGrid(output);
            
            //Ask user for 5 ship placement
            PlaceShips(output);


            //Clear screen
            Console.Clear();

            return output;
        }

        private static string AskForUsersName()
        {
            Console.Write("What is your name: ");
            string output = Console.ReadLine();

            return output;
        }

        private static void PlaceShips(PlayerInfoModel model)
        {
            do
            {
                Console.Write($"Where do you want to place your next ship number {model.ShipLocations.Count + 1}: ");
                string location = Console.ReadLine();

                bool isValidLocation = GameLogic.PlaceShip(model, location);

                if (!isValidLocation)
                {
                    Console.WriteLine("That was not a valid location. Please try again.");
                }

            } while (model.ShipLocations.Count < 5);
        }
    }
}
