using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Core.Interfaces
{
    namespace Invoice.Core.Interfaces
    {
        public interface INotificationService
        {
            void ShowInfo(string message);
            void ShowError(string message);
        }
    }

}
