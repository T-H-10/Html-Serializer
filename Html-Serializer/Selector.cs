using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Html_Serializer
{
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; } = new List<string>();
        public Selector Parent { get; set; }
        public Selector Child { get; set; }

        /// <summary>
        /// Converts a query string representing a CSS selector into a Selector object with a hierarchy.
        /// This method parses the query to extract tag name, ID, classes, and builds a tree structure.
        /// </summary>
        /// <param name="query">The query string representing the CSS selector.</param>
        /// <returns>The root Selector object representing the entire hierarchy.</returns>

        public static Selector ConvertQuery(string query)
        {
            string[] levels = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Selector root = null;
            Selector current = null;

            foreach (string level in levels)
            {
                Selector newSelector = new Selector();
                string[] parts = Regex.Split(level, "(#|\\.)");
                for (int i = 0; i < parts.Length; i++)
                {
                    //Check if the next part is a class.
                    if (parts[i] == "#" && i + 1 < parts.Length)
                    {
                        newSelector.Id = parts[++i];
                    }
                    //Check if the next part is a class.
                    else if (parts[i] == "." && i + 1 < parts.Length)
                    {
                        newSelector.Classes.Add(parts[++i]);
                    }
                    // Check if it's a valid tag name
                    else if (!string.IsNullOrEmpty(parts[i]) && Regex.IsMatch(parts[i], "^[a-zA-Z]+$"))
                    {
                        newSelector.TagName = parts[i];
                    }
                }
                // If this is the first selector, it becomes the root.
                if (root == null)
                {
                    root = newSelector;
                }
                else
                {
                    // Set the parent-child relationship.
                    current.Child = newSelector;
                    newSelector.Parent = current;
                }
                current = newSelector;
            }
            return root;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Parent != null) sb.AppendLine("Parent: \n" + Parent.ToString());
            sb.AppendLine($"Name: {TagName}");
            if (Id != null) sb.AppendLine($"Id: {Id}");
            if (Classes.Count > 0)
            {
                sb.AppendLine("Classes:");
                foreach (var clas in Classes)
                {
                    sb.AppendLine("\t- " + clas);
                }
            }
            if (Child!=null)
            {
                sb.AppendLine("Child: ");
                sb.AppendLine("\t||");
                sb.AppendLine("\t\\/");
                //if (Child.TagName != null)
                //    sb.Append(" name: " + Child.TagName);
                //if (Child.Id != null) sb.Append(" id: " + Child.Id);
                //if (Child.Classes.Count > 0)
                //{
                //    sb.Append("classes: ");
                //    foreach (var c in Child.Classes)
                //    {
                //        sb.Append($"{c} ");
                //    }
                //}
                //sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
