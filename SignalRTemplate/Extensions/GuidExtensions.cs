using System.Security.Cryptography;

namespace SignalRTemplate.Extensions;

public static class GuidExtensions
{
    public static string ToAlphaNumeric(this Guid self)
    {
        return self.ToString().Replace("-", string.Empty);
    }
}
