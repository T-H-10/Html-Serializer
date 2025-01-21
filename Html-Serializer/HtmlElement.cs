using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html_Serializer
{
    internal class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<string> Classes { get; set; } = new List<string>();
        public string InnerHtml { get; set; } = "";
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();
        //Adds attributes to the element.
        public void AddAttributes(string attribute)
        {
            string[] key_value = attribute.Split('=');
            string key = key_value[0];
            string value = string.Join(" ", key_value.Skip(1));
            if (key.Equals("id"))
            {
                Id = value.Trim('"');
            }
            else if (key.Equals("class"))
            {
                string[] classes = value.Split(' ');
                foreach (var clas in classes)
                {
                    Classes.Add(clas.Trim('"'));
                }
            }
            else
                Attributes.Add(key, value.Trim('"'));
        }

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

        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement current = this;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }

        public List<HtmlElement> FindBySelector(Selector selector)
        {
            HashSet<HtmlElement> result = new HashSet<HtmlElement>();
            FindBySelectorRec(this, selector, result);
            return result.ToList();
        }
        private void FindBySelectorRec(HtmlElement htmlElement, Selector selector, HashSet<HtmlElement> result)
        {
            bool matches = true;
            if (!htmlElement.Name.Equals(selector.TagName))
            {
                matches = false;
            }
            if (selector.Classes.Any() && !selector.Classes.All(c => htmlElement.Classes.Contains(c)))
            {
                matches = false;
            }
            if (matches)
            {
                result.Add(htmlElement);
            }
            foreach (var child in htmlElement.Descendants())
            {
                FindBySelectorRec(child, selector, result);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Name: {Name}");
            if (Id != null) sb.AppendLine($"Id: {Id}");
            if (Classes.Count > 0)
            {
                sb.AppendLine("Classes:");
                foreach (var clas in Classes)
                {
                    sb.AppendLine("\t- " + clas);
                }
            }
            if (Attributes.Count > 0)
            {
                sb.AppendLine("Attributes: ");
                foreach (var attribute in Attributes)
                {
                    sb.AppendLine("\t- " + attribute);
                }
            }
            if (InnerHtml != null) sb.AppendLine("InnerHTML " + InnerHtml);
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
