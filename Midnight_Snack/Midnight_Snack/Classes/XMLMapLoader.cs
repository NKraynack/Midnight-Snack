using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Midnight_Snack
{
    class XMLMapLoader
    {
        private static XMLMapLoader instance = new XMLMapLoader();
        StringBuilder output = new StringBuilder();

        String xmlString =
            @"<bookstore>
        <book genre='autobiography' publicationdate='1981-03-22' ISBN='1-861003-11-0'>
            <title>The Autobiography of Benjamin Franklin</title>
            <author>
                <first-name>Benjamin</first-name>
                <last-name>Franklin</last-name>
            </author>
            <price>8.99</price>
        </book>
    </bookstore>";
        String contentDir = Directory.GetCurrentDirectory() + "\\Content\\testmap.xml";

        private XMLMapLoader()
        {

        }

        public static XMLMapLoader GetInstance()
        {
            return instance;
        }

        public void read()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(contentDir);
            string xmlcontents = doc.InnerXml;

            //Create an XmlReader
            using (XmlReader reader = XmlReader.Create(new StringReader(xmlcontents)))
            {
                reader.ReadToFollowing("map");
                int rows = Convert.ToInt32(reader.GetAttribute("rows"));
                int cols = Convert.ToInt32(reader.GetAttribute("col"));
                int startRow = Convert.ToInt32(reader.GetAttribute("startRow"));
                int startCol = Convert.ToInt32(reader.GetAttribute("startRow"));

                reader.ReadStartElement(); //get turn limit
                int turnLim = Convert.ToInt32(reader.GetAttribute("limit"));

                reader.ReadToNextSibling("cursor");
                int cw = Convert.ToInt32(reader.GetAttribute("width"));
                int ch = Convert.ToInt32(reader.GetAttribute("height"));
                output.AppendLine("The cursor dimensions: ");
                output.AppendLine("\t width: " + cw);
                output.AppendLine("\t height: " + ch);

                reader.ReadToNextSibling("villager");
                int vw = Convert.ToInt32(reader.GetAttribute("width"));
                int vh = Convert.ToInt32(reader.GetAttribute("height"));
                int vrow = Convert.ToInt32(reader.GetAttribute("row"));
                int vcol = Convert.ToInt32(reader.GetAttribute("col"));
                output.AppendLine("The villager dimensions: ");
                output.AppendLine("\t rows: " + vrow);
                output.AppendLine("\t cols: " + vcol);
                output.AppendLine("\t width: " + vw);
                output.AppendLine("\t height: " + vh);

                reader.ReadToNextSibling("enemy");
                int ew = Convert.ToInt32(reader.GetAttribute("width"));
                int eh = Convert.ToInt32(reader.GetAttribute("height"));
                int erow = Convert.ToInt32(reader.GetAttribute("row"));
                int ecol = Convert.ToInt32(reader.GetAttribute("col"));
                output.AppendLine("The enemy dimensions: ");
                output.AppendLine("\t rows: " + erow);
                output.AppendLine("\t cols: " + ecol);
                output.AppendLine("\t width: " + ew);
                output.AppendLine("\t height: " + eh);

                output.AppendLine("The map dimensions: ");
                output.AppendLine("\t rows: " + rows);
                output.AppendLine("\t cols: " + cols);
                output.AppendLine("\t startRow: " + startRow);
                output.AppendLine("\t startCol: " + startCol);

                while (reader.ReadToNextSibling("obstacle"))
                {
                    int orows = Convert.ToInt32(reader.GetAttribute("row"));
                    int ocols = Convert.ToInt32(reader.GetAttribute("col"));
                    output.AppendLine("Obstacle at: " + ocols + " " + orows);
                }

                

                
                Console.WriteLine(output);
            }
        }


    }
}
