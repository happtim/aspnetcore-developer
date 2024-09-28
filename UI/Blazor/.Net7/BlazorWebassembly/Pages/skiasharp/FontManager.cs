using SkiaSharp;
using System.Collections.Concurrent;

namespace BlazorWebassembly.Pages.skiasharp
{
    public class FontManager
    {
        private readonly HttpClient _httpClient;
        private readonly ConcurrentDictionary<string, SKTypeface> _fontCache = new ConcurrentDictionary<string, SKTypeface>();

        public FontManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SKTypeface> GetFontAsync(string fontName, string fontUrl)
        {
            if (_fontCache.TryGetValue(fontName, out var cachedFont))
            {
                return cachedFont;
            }

            byte[] fontData = await _httpClient.GetByteArrayAsync(fontUrl);
            var typeface = SKTypeface.FromData(SKData.CreateCopy(fontData));

            _fontCache[fontName] = typeface;
            return typeface;
        }

        public void Dispose()
        {
            foreach (var font in _fontCache.Values)
            {
                font.Dispose();
            }
        }
    }
}
