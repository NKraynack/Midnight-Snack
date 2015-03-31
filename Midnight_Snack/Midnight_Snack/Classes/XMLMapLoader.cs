using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Text;

namespace Midnight_Snack.Classes
{
    class XMLMapLoader
    {
        private static XMLMapLoader instance = new XMLMapLoader();
        StringBuilder output = new StringBuilder();

        String xmlString =
                @"<?xml version='1.0'?>
        <!-- This is a sample XML document -->
        <Items>
          <Item>test with a child element <more/> stuff</Item>
        </Items>";

        private XMLMapLoader()
        {

        }

        public static XMLMapLoader GetInstance()
        {
            return instance;
        }

        public void read()
        {
            // Create an XmlReader
            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
            {
                reader.ReadToFollowing("book");
                reader.MoveToFirstAttribute();
                string genre = reader.Value;
                output.AppendLine("The genre value: " + genre);

                reader.ReadToFollowing("title");
                output.AppendLine("Content of the title element: " + reader.ReadElementContentAsString());
                Console.WriteLine(output);
            }
        }


    }
}
