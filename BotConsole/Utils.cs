namespace BotConsole
{
    internal static class Utils
    {
        public static T RandomEntry<T>(this List<T> lst) => lst[new Random().Next(lst.Count)];
    }
}
