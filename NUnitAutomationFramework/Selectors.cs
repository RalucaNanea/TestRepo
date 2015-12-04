using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitAutomationFramework
{

    public class Selectors : PropertyAttribute
    {
        public string websiteUrl { get; set; }
        public string closeAdvertise { get; set; }
        public string category { get; set; }
        public string firstProduct { get; set; }
        public string[] elementsToSelect { get; set; }
        public string btnAddToCart { get; set; }
        public string goToCart { get; set; }

    }
}
