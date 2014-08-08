using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AutoTest.VS.Resources
{
    static class ResourceReader
    {
        public static Image GetImage(string name)
        {
            System.Reflection.Assembly thisExe;
            thisExe = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream file =
                thisExe.GetManifestResourceStream("AutoTest.VS.Resources." + name);
            return Image.FromStream(file);
        }

        public static Icon GetIcon(string name)
        {
            System.Reflection.Assembly thisExe;
            thisExe = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream file =
                thisExe.GetManifestResourceStream("AutoTest.VS.Resources." + name);
            return new Icon(file);
        }
    }
}
