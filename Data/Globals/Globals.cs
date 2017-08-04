using Data.Resources;
using Data.XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Data.Globals
{
    public static class Global
    {
        private static ResManager resManager = new ResManager();
        private static XmlManager xmlManager = new XmlManager();

        public static ResManager GetResManager()
        {
            return resManager;
        }
        public static XmlManager GetXmlManager()
        {
            return xmlManager;
        }
    }
}
