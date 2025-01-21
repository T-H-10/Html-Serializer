using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
namespace Html_Serializer
{
    public class HtmlHelper
    {
        private readonly static HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance => _instance;
        public string[] HtmlTags { get; set; }
        public string[] HtmlVoidTags { get; set; }

        //Loading HTML tags from a JSON file.
        private string[] LoadTagsFromJson(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<string[]>(json);
        }
        public HtmlHelper()
        {
            HtmlTags = LoadTagsFromJson("JSON-Files/HtmlTags.json");
            HtmlVoidTags = LoadTagsFromJson("JSON-Files/HtmlVoidTags.json");
        }
        
    }
}
