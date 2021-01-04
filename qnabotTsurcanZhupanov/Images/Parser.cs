using System.Collections.Generic;
using System.Xml;

namespace QnABot.Images
{
    public class Parser
    {
        string path = @"D:\home\site\wwwroot\images_info.xml";
        XmlDocument xDoc = new XmlDocument();
        List<ImageProperties> images = new List<ImageProperties>();
        public List<ImageProperties> Images => images;

        public Parser()
        {
            xDoc.Load(path);
            Parse();
        }

        private void Parse()
        {
            XmlElement xEl = xDoc.DocumentElement;
            foreach (XmlNode xnode in xEl)
            {
                ImageProperties new_image = new ImageProperties();
                //read ittributes
                new_image.Number = xnode.Attributes["number"].Value;
                //read child nodes
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "imagePath")
                    {
                        new_image.Image_path = childnode.InnerText;
                    }
                    if (childnode.Name == "webPath")
                    {
                        new_image.Web_path = childnode.InnerText;
                    }
                    if (childnode.Name == "rightAnswer")
                    {
                        new_image.Right_answer = childnode.InnerText;
                    }
                }
                images.Add(new_image);
            }
        }
    }
}
