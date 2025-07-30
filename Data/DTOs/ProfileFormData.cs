
using NuGet.Protocol.Plugins;

namespace connect4_backend.Data.DTOs;

public class ProfileFormData
{
    public string firstName { get; set; } = "";
    public string lastName { get; set; } = "";
    public string bio { get; set; } = "";
}