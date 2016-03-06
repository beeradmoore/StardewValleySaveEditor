using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stardew_Valley_Save_Editor
{
    public class SavedGame
    {
        public string Name
        {
            get; set;
        }

        public string Value
        {
            get; set;
        }

        public string FullPath
        {
            get; set;
        }

        public override string ToString()
        {
            return this.Name;
        }

    }
}
