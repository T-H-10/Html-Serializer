﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html_Serializer
{
    /// <summary>
    /// Represents an HTML element with properties such as ID, name, attributes, classes, and children.
    /// Provides methods for querying descendants, ancestors, and finding elements based on a selector.
    /// </summary>
    internal class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<string> Classes { get; set; } = new List<string>();
        public string InnerHtml { get; set; } = "";
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();
        
        private const string IdAttribute = "id";
        private const string ClassAttribute = "class";

        /// <summary>
        /// Adds attributes to the HTML element based on a string in the format "key=value".
        /// Handles special cases for "id" and "class" attributes.
        /// </summary>
        /// <param name="attribute">The attribute string in the format "key=value".</param>
        //public void AddAttributes(string attribute)
        //{
        //    if (!attribute.Contains('=')) return;
        //    string[] key_value = attribute.Split('=');
        //    string key = key_value[0];
        //    string value = string.Join(" ", key_value.Skip(1));
        //    if (key.Equals(IdAttribute))
        //    {
        //        Id = value.Trim('"');
        //    }
        //    else if (key.Equals(ClassAttribute))
        //    {
        //        string[] classes = value.Split(' ');
        //        foreach (var clas in classes)
        //        {
        //            Classes.Add(clas.Trim('"'));
        //        }
        //    }
        //    else
        //        Attributes.Add(key, value.Trim('"'));
        //}

        /// <summary>
        /// Returns all descendants of the current HTML element, including itself.
        /// </summary>
        /// <returns>An enumerable collection of all descendant elements.</returns>
        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);
            while (!(queue.Count == 0))
            {
                HtmlElement current = queue.Dequeue();
                yield return current;
                foreach (var child in current.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }

        /// <summary>
        /// Returns all ancestor elements of the current HTML element, including itself.
        /// </summary>
        /// <returns>An enumerable collection of all ancestor elements.</returns>
        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement current = this;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }

        /// <summary>
        /// Queries the HTML element and its descendants for elements that match a given selector.
        /// </summary>
        /// <param name="selector">The selector object to match elements against.</param>
        /// <returns>A collection of HTML elements that match the selector.</returns>
        public IEnumerable<HtmlElement> Query(Selector selector)
        {
            HashSet<HtmlElement> set = new HashSet<HtmlElement>();
            FindElementBySelector(selector, set, this.Descendants());
            return set;
        }
        
        /// <summary>
        /// Recursively finds elements that match a given selector and adds them to the result set.
        /// </summary>
        /// <param name="selector">The selector object to match elements against.</param>
        /// <param name="list">The result set of matching elements.</param>
        /// <param name="elements">The collection of elements to search within.</param>
        private void FindElementBySelector(Selector selector, HashSet<HtmlElement> list, IEnumerable<HtmlElement> elements)
        {
            if (selector == null || elements==null || !elements.Any())
                return;

            foreach (var item in elements)
            {
                if (CheckSelector(item, selector))
                {
                    if (selector.Child == null)
                        list.Add(item);
                    FindElementBySelector(selector.Child, list, item.Descendants());
                }
            }
        }
        
        /// <summary>
        /// Checks whether a given HTML element matches a specific selector.
        /// </summary>
        /// <param name="element">The HTML element to check.</param>
        /// <param name="selector">The selector to match against.</param>
        /// <returns>True if the element matches the selector; otherwise, false.</returns>
        public bool CheckSelector(HtmlElement element, Selector selector)
        {
            if (selector.Id != null && !selector.Id.Equals(element.Id))
                return false;
            if (selector.TagName!=null && selector.TagName != element.Name)
                return false;
            if(selector.Classes.Count > 0 && !selector.Classes.All(c=>element.Classes.Contains(c)))
                return false;
            return true;
        }
        
        /// <summary>
        /// Returns a string representation of the HTML element, including its properties and structure.
        /// </summary>
        /// <returns>A formatted string representing the HTML element.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Name: {Name}");
            if (Id != null) sb.AppendLine($"Id: {Id}");
            if (Classes.Count > 0)
            {
                sb.AppendLine("Classes:")
                    .AppendLine(string.Join(Environment.NewLine, Classes.Select(c => "\t- " + c)));
                //foreach (var clas in Classes)
                //{
                //    sb.AppendLine("\t- " + clas);
                //}
            }
            if (Attributes.Count > 0)
            {
                sb.AppendLine("Attributes: ")
                    .AppendLine(string.Join(Environment.NewLine, Attributes.Select(attr => $"\t- {attr.Key}: {attr.Value}")));
                //foreach (var attribute in Attributes)
                //{
                //    sb.AppendLine("\t- " + attribute);
                //}
            }
            if (InnerHtml.Length>0) sb.AppendLine("InnerHTML " + InnerHtml);
            if (Parent != null) sb.AppendLine("Parent: " + Parent.Name);
            if (Children.Count > 0)
            {
                sb.AppendLine("Children: ");
                foreach (var child in Children)
                {
                    sb.AppendLine("\t- " + child.Name);
                }
            }
            return sb.ToString();
        }
    }
}
