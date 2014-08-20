using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivateSensorCore
{
    public enum ActivationRequestResults
    {
        AllEnabled,
        AskMeLater,
        NoAndDontAskAgain
    };

    public class ActivateSensorCoreResult
    {
        public ActivationRequestResults ActivationRequestResult;
        public bool Ongoing = false;
    }
}
