using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace evenotify_v2.models
{
    public class Recaptcha
    {
        public bool success { get; set; }
        public string[] errorcodes { get; set; }

    }
}
