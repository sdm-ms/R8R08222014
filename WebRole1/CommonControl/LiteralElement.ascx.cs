using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

public partial class CommonControl_LiteralElement : System.Web.UI.UserControl
{
    public bool OpeningTag { get; set; }
    public bool ClosingTag { get; set; }
    public string ElementType { get; set; }
    public string MarkupAttributeType1 { get; set; }
    public string MarkupAttributeContent1 { get; set; }
    public string MarkupAttributeType2 { get; set; }
    public string MarkupAttributeContent2 { get; set; }
    public string MarkupAttributeType3 { get; set; }
    public string MarkupAttributeContent3 { get; set; }
    public string MarkupAttributeType4 { get; set; }
    public string MarkupAttributeContent4 { get; set; }
    public List<string> AttributeTypes = new List<string>();
    public List<string> AttributeContents = new List<string>();
    internal bool rendered = false;

    public void AddAttribute(string type, string content)
    {
        if (type != null && type != "")
        {
            var existingAttributeType = AttributeTypes.Where(s => s == type);
            if (existingAttributeType.Any())
            {
                int existingAttributeTypeIndex = existingAttributeType.Select((x, i) => i).First();
                AttributeContents[existingAttributeTypeIndex] += " " + content;
            }
            else
            {
                AttributeTypes.Add(type);
                if (content == null)
                    content = "";
                AttributeContents.Add(content);
            }
        }
    }

    public void RemoveAttribute(string type)
    {
        if (type != null && type != "")
        {
            var existingAttributeType = AttributeTypes.Where(s => s == type);
            if (existingAttributeType.Any())
            {
                int existingAttributeTypeIndex = existingAttributeType.Select((x, i) => i).First();
                AttributeContents.RemoveAt(existingAttributeTypeIndex);
                AttributeTypes.RemoveAt(existingAttributeTypeIndex);
            }
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (ElementType == null || ElementType == "")
            throw new Exception("Internal error: ElementType must be specified in markup of LiteralElement.");
        AddAttribute(MarkupAttributeType1, MarkupAttributeContent1);
        AddAttribute(MarkupAttributeType2, MarkupAttributeContent2);
        AddAttribute(MarkupAttributeType3, MarkupAttributeContent3);
        AddAttribute(MarkupAttributeType3, MarkupAttributeContent4);
    }

    public void RenderNow()
    {
        rendered = true;
        StringBuilder theBuilder = new StringBuilder();
        theBuilder.Append("<");
        if (!OpeningTag && ClosingTag)
        { // e.g., </table>
            theBuilder.Append("/");
            theBuilder.Append(ElementType);
        }
        else
        {
            theBuilder.Append(ElementType);
            int count = AttributeTypes.Count;
            for (int i = 0; i < count; i++)
            {
                theBuilder.Append(" ");
                theBuilder.Append(AttributeTypes[i]);
                theBuilder.Append("=\"");
                theBuilder.Append(AttributeContents[i]);
                theBuilder.Append("\"");
            }
            if (ClosingTag)
                theBuilder.Append("/");
        }
        theBuilder.Append(">");
        MyLiteral.Text = theBuilder.ToString();
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!rendered)
            RenderNow();
    }
}
