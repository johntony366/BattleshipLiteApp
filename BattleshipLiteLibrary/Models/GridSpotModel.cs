using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipLiteLibrary.Models
{
    public class GridSpotModel
    {
        public char SpotLetter { get; set; }
        public int SpotNumber { get; set; }
        public GridSpotStatus Status { get; set; } = GridSpotStatus.Empty; //0 = empty, 1 = ship, 2 = miss, 3 = hit, 4 = sunk
    }
}
