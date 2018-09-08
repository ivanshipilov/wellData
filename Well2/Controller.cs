using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using well;

namespace Well2
{
    class Controller
    {
        WellsArea area = null;
        public Controller(WellsArea area)
        {
            this.area = area;
        }
        public void AddWells(string wellPath)
        {
            area.AddWells(wellPath);
        }
        public void RemoveWells(string wellPath)
        {
            area.RemoveWells(wellPath);
        }
        public void CheckWells(string wellPath)
        {
            area.CheckWells(wellPath);
        }
        public Well GetWell(string wellPath)
        {
            return area.GetWell(wellPath);
        }
    }
}
