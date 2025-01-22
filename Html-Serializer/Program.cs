using Html_Serializer;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
/// <summary>
/// Loads the HTML content from a given URL asynchronously.
/// </summary>
/// <param name="url">The URL to load the HTML from.</param>
/// <returns>The HTML content as a string.</returns>
static async Task<string> LoadAsync(string url)
{
    using HttpClient client = new HttpClient();
    try
    {
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error loading HTML: {ex.Message}");
        return string.Empty;
    }
}

/// <summary>
/// Extracts the first word from a given string.
/// </summary>
/// <param name="str">The input string.</param>
/// <returns>The first word or an empty string if input is null or empty.</returns>
static string GetFirstWord(string str)
{
    if (string.IsNullOrWhiteSpace(str))
    {
        return string.Empty;
    }

    string[] words = str.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    return words.Length > 0 ? words[0] : string.Empty;
}

/// <summary>
/// Removes the first word from a given string.
/// </summary>
/// <param name="input">The input string.</param>
/// <returns>The string without the first word, or an empty string if input is null or empty.</returns>
static string RemoveFirstWord(string input)
{
    if (string.IsNullOrWhiteSpace(input))
    {
        return string.Empty;
    }

    string[] words = input.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    return string.Join(" ",words.Skip(1));
}

/// <summary>
/// Creates an HTML element tree from the provided HTML string.
/// </summary>
/// <param name="html">The HTML content.</param>
/// <returns>The root `HtmlElement` representing the HTML structure.</returns>
static HtmlElement CreateElementsTree(string html)
{

    var cleanHtml = Regex.Replace(html,"[\\s]"," ").Trim();
    var htmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(s => !string.IsNullOrEmpty(s));

    HtmlElement root = new HtmlElement() { Name = "root" };
    HtmlElement current = root;

    foreach (var line in htmlLines)
    {
        //if (string.IsNullOrEmpty(line)) continue;
        string tagName = GetFirstWord(line.Trim());
        if (string.IsNullOrEmpty(tagName)) continue;
        if (tagName.Equals("/html",StringComparison.OrdinalIgnoreCase))
        {
            return root;
        }
        if (tagName.StartsWith("/"))
        {
            current = current.Parent ?? root;
        }
        else if (HtmlHelper.Instance.HtmlTags.Contains(tagName))
        {
            HtmlElement child = new HtmlElement() { Name = tagName };
            // Extract attributes.
            var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(RemoveFirstWord(line));
            foreach (Match attribute in attributes)
            {
                child.AddAttributes(attribute.Value);
            }
            current.Children.Add(child);
            child.Parent = current;

            if (!HtmlHelper.Instance.HtmlVoidTags.Contains(tagName))
            {
                current = child;
            }
        }
        else
        {
            current.InnerHtml += line.Trim();
        }
    }
    return root;
}


/// <summary>
/// Finds and prints elements matching a CSS selector in the given HTML element tree.
/// </summary>
/// <param name="selector">The CSS selector.</param>
/// <param name="root">The root HTML element.</param>
static void Check1(Selector selector, HtmlElement root)
{
    var result = root.Query(selector);
    result.ToList().ForEach(element => { Console.WriteLine(element.ToString()); });
}

string html = await LoadAsync("https://chani-k.co.il/sherlok-game/");
HtmlElement root = CreateElementsTree(html);
Selector selector = Selector.ConvertQuery("div a");
//Console.WriteLine(selector.ToString());
Check1(selector, root);

//Print(root);

static void Print(HtmlElement root)
{
    Console.WriteLine(root.ToString());
    foreach (var child in root.Children)
    {
        Print(child);
    }
}

//string query = "div#mydiv.class-name p.class-child";
//Selector rootSelector = Selector.ConvertQuery(query);

//// דוגמת בדיקה של התוצאה
//Console.WriteLine($"Root Tag: {rootSelector.TagName}"); // Output: Root Tag: div
//Console.WriteLine($"Root Id: {rootSelector.Id}");   // Output: Root Id: mydiv
//Console.WriteLine($"Root Classes: {string.Join(", ", rootSelector.Classes)}"); // Output: Root Classes: class-name

//if (rootSelector.Child != null)
//{
//    Console.WriteLine($"Child Tag: {rootSelector.Child.TagName}"); // Output: Child Tag: p
//    Console.WriteLine($"Child Classes: {string.Join(", ", rootSelector.Child.Classes)}"); // Output: Child Classes: class-child
//}



