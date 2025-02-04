namespace Amethyst.Console;

internal sealed class AuthenticationService
{
    private bool hasAuthenticated;

    public bool TryAuthenticate(string message)
    {
        if (hasAuthenticated)
        {
            return true;
        }

        hasAuthenticated = true;

        return message is "12345";
    }
}