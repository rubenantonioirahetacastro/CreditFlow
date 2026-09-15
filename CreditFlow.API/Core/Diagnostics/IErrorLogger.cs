using System.Runtime.CompilerServices;

namespace CreditFlow.API.Core.Diagnostics;

public interface IErrorLogger
{
    Task LogAsync(
        Exception exception,
        [CallerMemberName] string method = "",
        [CallerFilePath] string file = "");
}
