using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestProject1
{
    enum HeterogeneousUserType
    {
        /// <summary>
        /// The user attempts to rate ratings accurately.
        /// </summary>
        Dominant,
        /// <summary>
        /// The user attempts to rate ratings using a value
        /// that is intentionally incorrect, e.g., to 
        /// manipulate the system.
        /// </summary>
        Subversive,
    }
}
