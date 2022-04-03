using BattleshipLiteLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipLiteLibrary
{
    public static class GameLogic
    {
        public static void InitializeGrid(PlayerInfoModel model)
        {
            List<char> letters = new List<char> { 'A', 'B', 'C', 'D', 'E' };
            List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };

            foreach (char c in letters)
            {
                foreach (int number in numbers)
                {
                    AddGridSpot(model, c, number);
                }
            }

        }

        private static void AddGridSpot(PlayerInfoModel model, char letter, int number)
        {
            GridSpotModel spot = new GridSpotModel();
            spot.SpotLetter = letter;
            spot.SpotNumber = number;
            spot.Status = GridSpotStatus.Empty;

            model.ShotGrid.Add(spot);
        }

        public static bool PlayerStillActive(PlayerInfoModel player)
        {
            foreach (GridSpotModel ship in player.ShipLocations)
            {
                if (ship.Status != GridSpotStatus.Sunk)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool PlaceShip(PlayerInfoModel model, string location)
        {
            bool output = false;
            char row; int column;

            try
            {
                (row, column) = SplitShotIntoRowAndColumn(location);
            }
            catch (Exception)
            {
                Console.WriteLine("Error: Input string was not in the correct format");
                return false;
            }

            bool isValidLocation = ValidateGridLocation(model, row, column);
            bool isSpotOpen = ValidateShipLocation(model, row, column);

            if (isValidLocation && isSpotOpen)
            {
                model.ShipLocations.Add(new GridSpotModel { SpotLetter = char.ToUpper(row), SpotNumber = column, Status = GridSpotStatus.Occupied });
                output = true;
            }

            return output;

        }

        private static bool ValidateShipLocation(PlayerInfoModel model, char row, int column)
        {
            foreach (GridSpotModel ship in model.ShipLocations)
            {
                if (ship.SpotLetter == char.ToUpper(row) && ship.SpotNumber == column)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool ValidateGridLocation(PlayerInfoModel player, char row, int column)
        {
            if (char.ToUpper(row) < player.ShotGrid[0].SpotLetter || char.ToUpper(row) > player.ShotGrid[player.ShotGrid.Count - 1].SpotLetter) return false;
            if (column < player.ShotGrid[0].SpotNumber || column > player.ShotGrid[player.ShotGrid.Count - 1].SpotNumber) return false;
            return true;
        }

        public static int GetShotCount(PlayerInfoModel player)
        {
            int shotCount = 0;
            foreach (GridSpotModel shot in player.ShotGrid)
            {
                if (shot.Status != GridSpotStatus.Empty) ++shotCount;
            }

            return shotCount;
        }

        public static (char row, int column) SplitShotIntoRowAndColumn(string shot)
        {
            char row = ' ';
            int column = 0;

            if (shot.Length != 2)
            {
                throw new ArgumentException("Invalid shot", "shot");
            }

            row = shot[0];
            column = int.Parse(shot[1].ToString());

            return (row, column);
        }

        public static bool ValidateShot(PlayerInfoModel activePlayer, char row, int column)
        {
            foreach (GridSpotModel spot in activePlayer.ShotGrid)
            {
                if (row == spot.SpotLetter && column == spot.SpotNumber)
                {
                    if (spot.Status == GridSpotStatus.Empty)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public static bool IdentifyShotResult(PlayerInfoModel opponent, char row, int column)
        {
            return !ValidateShipLocation(opponent, row, column);
        }

        public static void MarkShotResult(PlayerInfoModel activePlayer, char row, int column, bool isAHit)
        {
            foreach (GridSpotModel spot in activePlayer.ShotGrid)
            {
                if (spot.SpotLetter == row && spot.SpotNumber == column)
                {
                    if (isAHit)
                    {
                        spot.Status = GridSpotStatus.Hit;
                    }
                    else spot.Status = GridSpotStatus.Miss;
                }
            }
        }
    }

    
}
