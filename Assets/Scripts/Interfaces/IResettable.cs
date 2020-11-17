using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    interface IResettable
    {
        void ResetSelf();
        void DisableSelf();        
    }
}
