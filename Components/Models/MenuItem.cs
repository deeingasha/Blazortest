namespace BlazorApp1.Components.Models
{
    public class MenuItem
    {
        public string Text { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string? Href { get; set; }
        public bool IsExpanded { get; set; }
        public List<MenuItem> Children { get; set; } = new();
    }
}