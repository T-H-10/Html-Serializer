using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Html_Serializer
{
    /// <summary>
    /// Represents a CSS selector, supporting tag name, ID, classes, and hierarchical relationships.
    /// Provides functionality to convert a query string into a selector hierarchy.
    /// </summary>
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
            Selector root = null, current = null;

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

        /// <summary>
        /// Returns a string representation of the selector, including its properties and hierarchy.
        /// </summary>
        /// <returns>A formatted string representing the selector.</returns>
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
            }
            return sb.ToString();
        }
    }
}
