using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using RimTrans.Framework.Converter.Helper;

namespace RimTrans.Framework.Converter
{
    class Program
    {
        static void Main(string[] args) {
            string pathTextAsset = @"..\..\..\Resources\TextAsset";
            string pathDefs = @"..\..\..\..\Defs";

            ShuffledNameHelper.Convert(pathTextAsset, pathDefs);
            SolidNameHelper.Convert(pathTextAsset, pathDefs);
            //Temporay.foo();
        }
    }
}
