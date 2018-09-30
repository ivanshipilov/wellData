using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using well;

namespace Well2
{
    class WellsArea
    {
        Dictionary<string, Well> DictWells = new Dictionary<string, Well>();
        private decimal scale = 20;

        public WellsArea()
        {

        }
        public void AddWells(string wellPath)
        {
            if (!DictWells.ContainsKey(wellPath))
            {
                string fileContent = System.IO.File.ReadAllText(wellPath);
                string fileName = System.IO.Path.GetFileNameWithoutExtension(wellPath);
                Well newWell = new Well(fileName, wellPath);
                DictWells.Add(wellPath, newWell);
            }
            //exepsions
        }
        public void RemoveWells(string wellPath)
        {

        }
        public void CheckWells(string wellPath)
        {

        }
        public Well GetWell(string wellPath)
        {
            //exepsions "файл не был открыт (отсутствует id)"
            if (DictWells.ContainsKey(wellPath))
            {
                return DictWells[wellPath];
            }
            return null;
        }
        public decimal Scale                    
        {
            get { return scale; }
            set { scale = value; }
        }        
    }
}
