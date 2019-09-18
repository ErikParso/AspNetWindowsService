using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspWinService.Model
{
    public class CurrentUserInfo
    {
        public string UserName { get; set; }
        public string AppLocalPath { get; set; }

        public string CommonDesktop { get; set; }
        public string CommonPrograms { get; set; }
        
        public string UserDesktop { get; set; }
        public string UserPrograms { get; set; }
    }
}
