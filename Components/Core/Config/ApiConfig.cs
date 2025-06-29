using System;

namespace BlazorApp1.Components.Core.Config;

public class ApiConfig
{
    public string BaseUrl { get; set; } = string.Empty;
    public int Timeout { get; set; } = 30;

}
