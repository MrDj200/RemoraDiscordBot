using Remora.Discord.API.Abstractions.Objects;
using System.Text;
using System.Text.RegularExpressions;

namespace BotConsole
{
    internal static class Utils
    {
        private static HttpClient _httpClient = new();

        public static T RandomEntry<T>(this List<T> lst) => lst[new Random().Next(lst.Count)];

        public static bool Contains(this string str, params string[] values) => values.Any(v => str.Contains(v));

        public static Task<string> ReadPngChunkArray(byte[] bytes)
        {
            byte[] PngSignature = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };

            try
            {
                byte[] fileSignature = bytes.Take(8).ToArray();

                if (Enumerable.SequenceEqual(fileSignature, PngSignature))
                {
                    // Skip file signature
                    int index = 8;

                    while (index < bytes.Length)
                    {
                        // Read length of data and type of chunk
                        byte[] lengthDataField = bytes.Skip(index).Take(4).ToArray();
                        byte[] typeDataField = bytes.Skip(index + 4).Take(4).ToArray();
                        int length = (lengthDataField[0] << 24) | (lengthDataField[1] << 16) | (lengthDataField[2] << 8) | lengthDataField[3];
                        string type = Encoding.UTF8.GetString(typeDataField);

                        if (type.Equals("iTXt"))
                        {
                            // Skip keyword and flag data and encode content
                            int bytesToSkip = Encoding.UTF8.GetByteCount("Description") + 5;
                            byte[] dataField = bytes.Skip(index + 8 + bytesToSkip).Take(length - bytesToSkip).ToArray();
                            return Task.FromResult(Encoding.UTF8.GetString(dataField));
                        }

                        if (type.Equals("IEND"))
                            return Task.FromResult("N/A");

                        index += length + 12;
                    }
                    return Task.FromResult("N/A");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while reading png chunks: {e}");
                Task.FromResult("N/A");
            }
            return Task.FromResult("N/A");
        }

        public static async Task<bool> IsUrlPngAsync(string url)
        {
            HttpRequestMessage request = new(HttpMethod.Head, url);
            try
            {
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                if (response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.MediaType != null)
                {
                    string contentType = response.Content.Headers.ContentType.MediaType;
                    if (Regex.IsMatch(contentType, "^image/png$"))
                    {
                        return true;
                    }
                }
            }
            catch (HttpRequestException)
            {
                // handle exception
            }
            return false;
        }
    }
}
