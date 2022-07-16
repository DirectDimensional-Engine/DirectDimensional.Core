using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DirectDimensional.Core.Utilities;

using static StbTrueTypeSharp.StbTrueType;
using DDTexture2D = DirectDimensional.Core.Texture2D;

namespace DirectDimensional.Core {
    public readonly struct CharacterFallbackProfile {
        public readonly Range Range { get; init; }
        public readonly char Character { get; init; }

        public CharacterFallbackProfile(Range range, char character) {
            Range = new Range(range.Start.IsFromEnd ? char.MaxValue - range.Start.Value : range.Start.Value, range.End.IsFromEnd ? char.MaxValue - range.End.Value : range.End.Value);
            Character = character;
        }

        public bool IsInRange(char c) {
            return Range.Start.Value >= c && Range.End.Value < c;
        }
    }

    public sealed unsafe class Font : DDObject {
        private stbtt_packedchar[] _packedChars;
        private readonly float _ascent, _descent, _lineGap;

        private readonly float _spaceWidth = 0;

        public DDTexture2D Bitmap { get; private set; }
        public Range Codepoints { get; private set; }
        public int FontSize { get; private set; }
        public Vector2 BitmapUVStep { get; private set; }

        public int CodepointCount {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => Codepoints.End.Value - Codepoints.Start.Value;
        }
        public int FirstCodepoint {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => Codepoints.Start.Value;
        }
        public int FinalCodepoint {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => Codepoints.End.Value - 1;
        }

        public float Ascent { [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)] get => _ascent; }
        public float Descent { [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)] get => _descent; }
        public float LineGap { [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)] get => _lineGap; }

        public float SpaceWidth { [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)] get => _spaceWidth; }

        /// <summary>
        /// Load ttf font file at given path.
        /// </summary>
        /// <param name="ttfPath">Path of the ttf</param>
        /// <param name="bitmapWidth">Width of bitmap, maximum is <seealso cref="DDTexture2D.TextureMaximumSize"/></param>
        /// <param name="bitmapHeight">Height of bitmap, maximum is <seealso cref="DDTexture2D.TextureMaximumSize"/></param>
        /// <param name="fontSize">Size of the font in pixel</param>
        /// <param name="codepoints">Codepoint range with inclusive start and exclusive end</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid bitmap size or font size not positive</exception>
        public Font(string ttfPath, int bitmapWidth, int bitmapHeight, int fontSize, Range codepoints) {
            if (bitmapWidth <= 0) throw new ArgumentOutOfRangeException(nameof(bitmapWidth));
            if (bitmapHeight <= 0) throw new ArgumentOutOfRangeException(nameof(bitmapHeight));
            if (fontSize <= 0) throw new ArgumentOutOfRangeException(nameof(fontSize));

            Codepoints = codepoints;

            try {
                var ttf = File.ReadAllBytes(ttfPath);

                fixed (byte* pTTF = ttf) {
                    stbtt_fontinfo info = new();
                    if (stbtt_InitFont(info, pTTF, 0) == 0) {
                        throw new Exception("Failed to initialize ttf font at path '" + ttfPath + "'");
                    }

                    _packedChars = new stbtt_packedchar[CodepointCount];
                    FontSize = fontSize;

                    if (bitmapWidth <= 0 || bitmapWidth > DDTexture2D.TextureMaximumSize) {
                        bitmapWidth = Math.Clamp(bitmapWidth, 512, DDTexture2D.TextureMaximumSize);
                        Logger.Warn("Invalid width to create font bitmap. Fallback to " + bitmapWidth);
                    }
                    if (bitmapHeight <= 0 || bitmapHeight > DDTexture2D.TextureMaximumSize) {
                        bitmapHeight = Math.Clamp(bitmapHeight, 512, DDTexture2D.TextureMaximumSize);
                        Logger.Warn("Invalid width to create font bitmap. Fallback to " + bitmapHeight);
                    }

                    byte[] pixels = new byte[bitmapWidth * bitmapHeight * 4];
                    fixed (byte* ppixels = &pixels[0]) {
                        try {
                            stbtt_pack_context packContent = new();
                            stbtt_PackBegin(packContent, ppixels, bitmapWidth, bitmapHeight, bitmapWidth, 1, null);

                            fixed (stbtt_packedchar* pPacked = &_packedChars[0]) {
                                stbtt_PackSetOversampling(packContent, 3, 1);
                                stbtt_PackFontRange(packContent, pTTF, 0, fontSize, FirstCodepoint, CodepointCount, pPacked);
                            }

                            stbtt_PackEnd(packContent);

                            // Blame stb_truetype.h for doing 1 channel pixel
                            for (int i = bitmapWidth * bitmapHeight - 1; i >= 0; i--) {
                                *((Color32*)ppixels + i) = new Color32(255, 255, 255, ppixels[i]);
                            }

                            Bitmap = new(new ReadOnlySpan<byte>(ppixels, bitmapWidth * bitmapHeight * 4), bitmapWidth, TextureFlags.Render);
                        } catch {
                            Bitmap.CheckAndDestroy();
                            throw;
                        }
                    }

                    float fontScale = stbtt_ScaleForPixelHeight(info, fontSize);

                    int ascent, descent, linegap;
                    stbtt_GetFontVMetrics(info, &ascent, &descent, &linegap);

                    _ascent = ascent * fontScale;
                    _descent = descent * fontScale;
                    _lineGap = linegap * fontScale;
                }
            } catch {
                Bitmap.CheckAndDestroy();

                throw;
            }

            BitmapUVStep = new Vector2(1f / Bitmap.Width, 1f / Bitmap.Height);

            if (TryGetPackedChar(' ', out var pc)) {
                _spaceWidth = pc.xadvance;
            }
        }

        public static bool FindFallbackProfile(ReadOnlySpan<CharacterFallbackProfile> profiles, char c, out CharacterFallbackProfile profile) {
            for (int i = 0; i < profiles.Length; i++) {
                if (profiles[i].IsInRange(c)) {
                    profile = profiles[i];
                    return true;
                }
            }

            profile = default;
            return false;
        }

        /// <summary>
        /// Calculate the size of text in unscaled pixel size
        /// </summary>
        /// <param name="str">String to calculate size from</param>
        public Vector2 CalcStringSize(ReadOnlySpan<char> str) {
            if (str.IsEmpty) return default;

            Vector2 output = default;

            foreach (var line in str.EnumerateLines()) {
                float currLineW = 0;

                for (int i = 0; i < line.Length; i++) {
                    var c = str[i];

                    switch (c) {
                        case '\t': currLineW += SpaceWidth * 4; break;
                        case ' ': currLineW += SpaceWidth; break;
                        default:
                            if (c < ' ') continue;

                            if (TryGetPackedChar(c, out var pc)) {
                                currLineW += pc.xadvance;
                            }
                            break;
                    }
                }

                output.X = MathF.Max(currLineW, output.X);
                output.Y += FontSize;
            }

            return output;
        }

        /// <summary>
        /// Calculate the size of text in unscaled pixel size, support fallback codepoint
        /// </summary>
        /// <param name="str">String to calculate size from</param>
        public Vector2 CalcStringSize(ReadOnlySpan<char> str, ReadOnlySpan<CharacterFallbackProfile> fallbacks) {
            if (str.IsEmpty) return default;

            Vector2 output = default;

            foreach (var line in str.EnumerateLines()) {
                float currLineW = 0;

                for (int i = 0; i < line.Length; i++) {
                    var c = str[i];

                    switch (c) {
                        case '\t': currLineW += SpaceWidth * 4; break;
                        case ' ': currLineW += SpaceWidth; break;
                        default:
                            if (c < ' ') {
                                if (FindFallbackProfile(fallbacks, c, out var profile)) {
                                    if (TryGetPackedChar(profile.Character, out var fc)) {
                                        currLineW += fc.xadvance;
                                    }
                                }

                                continue;
                            }

                            if (TryGetPackedChar(c, out var pc)) {
                                currLineW += pc.xadvance;
                            } else if (FindFallbackProfile(fallbacks, c, out var profile)) {
                                if (TryGetPackedChar(profile.Character, out var fc)) {
                                    currLineW += fc.xadvance;
                                }
                            }
                            break;
                    }
                }

                output.X = MathF.Max(currLineW, output.X);
                output.Y += FontSize;
            }

            return output;
        }

        /// <summary>
        /// Calculate the width of text and ignore the height.
        /// </summary>
        /// <param name="str">String to calculate the width from</param>
        public float CalcStringWidth(ReadOnlySpan<char> str) {
            float width = 0;

            foreach (var line in str.EnumerateLines()) {
                float currLineW = 0;
                for (int i = 0; i < line.Length; i++) {
                    var c = str[i];

                    switch (c) {
                        case '\t': currLineW += SpaceWidth * 4; break;
                        case ' ': currLineW += SpaceWidth; break;
                        default:
                            if (c < ' ') continue;

                            if (TryGetPackedChar(c, out var pc)) {
                                currLineW += pc.xadvance;
                            }
                            break;
                    }
                }
                width = MathF.Max(width, currLineW);
            }

            return width;
        }

        /// <summary>
        /// Calculate the string width of a line of text.
        /// </summary>
        /// <param name="str">String to calculate width from.</param>
        public float CalcStringLineWidth(ReadOnlySpan<char> str) {
            float width = 0;

            for (int i = 0; i < str.Length; i++) {
                var c = str[i];

                switch (c) {
                    case '\t': width += SpaceWidth * 4; break;
                    case ' ': width += SpaceWidth; break;
                    default:
                        if (c < ' ') continue;

                        if (TryGetPackedChar(c, out var pc)) {
                            width += pc.xadvance;
                        }
                        break;
                }
            }

            return width;
        }

        /// <summary>
        /// Calculate the width of text and ignore the height, support fallback codepoint
        /// </summary>
        /// <param name="str">String to calculate the width from</param>
        public float CalcStringWidth(ReadOnlySpan<char> str, ReadOnlySpan<CharacterFallbackProfile> fallbacks) {
            float width = 0;

            foreach (var line in str.EnumerateLines()) {
                float currLineW = 0;
                for (int i = 0; i < line.Length; i++) {
                    var c = str[i];

                    switch (c) {
                        case '\t': currLineW += SpaceWidth * 4; break;
                        case ' ': currLineW += SpaceWidth; break;
                        default:
                            if (c < ' ') {
                                if (FindFallbackProfile(fallbacks, c, out var profile)) {
                                    if (TryGetPackedChar(profile.Character, out var fc)) {
                                        currLineW += fc.xadvance;
                                    }
                                }

                                continue;
                            }

                            if (TryGetPackedChar(c, out var pc)) {
                                currLineW += pc.xadvance;
                            } else if (FindFallbackProfile(fallbacks, c, out var profile)) {
                                if (TryGetPackedChar(profile.Character, out var fc)) {
                                    currLineW += fc.xadvance;
                                }
                            }
                            break;
                    }
                }
                width = MathF.Max(width, currLineW);
            }

            return width;
        }

        /// <summary>
        /// Calculate the height of the text based on the line feed character (or <c>\n</c> character)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public float CalcStringHeight(ReadOnlySpan<char> str) {
            if (str.IsEmpty) return 0;
            return (str.Count('\n') + 1) * FontSize;
        }

        /// <summary>
        /// Calculate the height of the text based on the line feed character (or <c>\n</c> character)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        //public float CalcStringHeightW(ReadOnlySpan<char> str, float maxWidth = 100000) {
        //    float sizeY = 0;
        //    float currLineW = 0;

        //    int begin = 0;
        //    while (begin < str.Length) {
        //        var wrap = begin + CalcWordWrapIndex(str[begin..], maxWidth);

        //        var slice = str[begin..wrap];
        //        if (begin != 0) slice = slice.TrimStart();

        //        for (int i = 0; i < slice.Length; i++) {
        //            var c = slice[i];

        //            switch (c) {
        //                case ' ': currLineW += SpaceWidth; break;
        //                case '\t': currLineW += SpaceWidth * 4; break;
        //                case '\n':
        //                    sizeY += FontSize;
        //                    currLineW = 0;
        //                    break;
        //                case '\r':
        //                    currLineW = 0;
        //                    break;

        //                default:
        //                    if (c < ' ') continue;

        //                    if (TryGetPackedChar(c, out var pc)) {
        //                        currLineW += pc.xadvance;
        //                    }
        //                    break;
        //            }
        //        }

        //        sizeY += FontSize;

        //        currLineW = 0;

        //        begin = wrap;
        //    }

        //    return sizeY;
        //}

        /// <summary>
        /// Calculate the height of the text based on the line feed character (or <c>\n</c> character), support fallback codepoint
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        //public float CalcStringHeightW(ReadOnlySpan<char> str, char fallback, float maxWidth = 100000) {
        //    var _fallback = GetPackedChar(fallback);
            
        //    float sizeY = 0;
        //    float currLineW = 0;

        //    int begin = 0;
        //    while (begin < str.Length) {
        //        var wrap = begin + CalcWordWrapIndex(str[begin..], maxWidth);

        //        var slice = str[begin..wrap];
        //        if (begin != 0) slice = slice.TrimStart();

        //        for (int i = 0; i < slice.Length; i++) {
        //            var c = slice[i];

        //            switch (c) {
        //                case ' ': currLineW += SpaceWidth; break;
        //                case '\t': currLineW += SpaceWidth * 4; break;
        //                case '\n':
        //                    sizeY += FontSize;
        //                    currLineW = 0;
        //                    break;
        //                case '\r':
        //                    currLineW = 0;
        //                    break;

        //                default:
        //                    if (c < ' ') continue;

        //                    if (TryGetPackedChar(c, out var pc)) {
        //                        currLineW += pc.xadvance;
        //                    } else if (_fallback.HasValue) {
        //                        currLineW += _fallback.Value.xadvance;
        //                    }
        //                    break;
        //            }
        //        }

        //        sizeY += FontSize;
        //        currLineW = 0;
        //        begin = wrap;
        //    }

        //    return sizeY;
        //}

        /// <summary>
        /// Calculate where to slice text (word wise) to fit in an area
        /// </summary>
        /// <param name="str">String text</param>
        /// <param name="width">The unscaled width</param>
        /// <returns>Exclusive range to cut input string</returns>
        //public int CalcWordWrapIndex(ReadOnlySpan<char> str, float width) {
        //    int current = 0;

        //    float lineWidth = 0;
        //    float spaceWidth = 0;
        //    float wordWidth = 0;

        //    bool insideWord = true;

        //    int prevWordEndPos = -1;
        //    int currWordEndPos = 0;

        //    while (current < str.Length) {
        //        var c = str[current];

        //        if (c == '\n' || c == '\r') {
        //            lineWidth = spaceWidth = wordWidth = 0;
        //            insideWord = true;
        //            current++;

        //            continue;
        //        }

        //        float characterWidth = 0;
        //        if (c == '\t') {
        //            if (TryGetPackedChar('\t', out var pc)) {
        //                characterWidth = pc.xadvance * 4;
        //            }
        //        } else {
        //            if (TryGetPackedChar(c, out var pc)) {
        //                characterWidth = pc.xadvance;
        //            }
        //        }

        //        if (c == ' ' || c == '\t') {
        //            if (insideWord) {
        //                lineWidth += spaceWidth;
        //                spaceWidth = 0;

        //                prevWordEndPos = currWordEndPos;
        //            }

        //            spaceWidth += characterWidth;
        //            insideWord = false;
        //        } else {
        //            wordWidth += characterWidth;

        //            currWordEndPos = current;

        //            lineWidth += characterWidth + spaceWidth;
        //            insideWord = true;
        //            spaceWidth = 0;
        //        }

        //        if (lineWidth > width) {
        //            current = (prevWordEndPos == -1 ? currWordEndPos : prevWordEndPos) + 1; // Make it exclusive
        //            break;
        //        }

        //        current++;
        //    }

        //    return current;
        //}

        /// <summary>
        /// Calculate where to slice text (word wise) to fit in an area, support fallback codepoint.
        /// </summary>
        /// <param name="str">String text</param>
        /// <param name="width">The unscaled width</param>
        /// <returns>Exclusive range to cut input string</returns>
        //public int CalcWordWrapIndex(ReadOnlySpan<char> str, float width, char fallback) {
        //    int current = 0;

        //    float lineWidth = 0;
        //    float spaceWidth = 0;
        //    float wordWidth = 0;

        //    bool insideWord = true;

        //    int prevWordEndPos = -1;
        //    int currWordEndPos = 0;

        //    var _fallback = GetPackedChar(fallback);

        //    while (current < str.Length) {
        //        var c = str[current];

        //        if (c == '\n' || c == '\r') {
        //            lineWidth = spaceWidth = wordWidth = 0;
        //            insideWord = true;
        //            current++;

        //            continue;
        //        }

        //        float characterWidth = 0;
        //        if (c == '\t') {
        //            if (TryGetPackedChar('\t', out var pc)) {
        //                characterWidth = pc.xadvance * 4;
        //            }
        //        } else {
        //            if (TryGetPackedChar(c, out var pc)) {
        //                characterWidth = pc.xadvance;
        //            } else if (_fallback.HasValue) {
        //                characterWidth = _fallback.Value.xadvance;
        //            }
        //        }

        //        if (c == ' ' || c == '\t') {
        //            if (insideWord) {
        //                lineWidth += spaceWidth;
        //                spaceWidth = 0;

        //                prevWordEndPos = currWordEndPos;
        //            }

        //            spaceWidth += characterWidth;
        //            insideWord = false;
        //        } else {
        //            wordWidth += characterWidth;

        //            currWordEndPos = current;

        //            lineWidth += characterWidth + spaceWidth;
        //            insideWord = true;
        //            spaceWidth = 0;
        //        }

        //        if (lineWidth > width) {
        //            current = (prevWordEndPos == -1 ? currWordEndPos : prevWordEndPos) + 1; // Make it exclusive
        //            break;
        //        }

        //        current++;
        //    }

        //    return current;
        //}

        /// <summary>
        /// Calculate string size to fit inside a rectangle with a width. Character truncate.
        /// </summary>
        /// <param name="str">String to calculate</param>
        /// <param name="maxWidth">Width of rectangle to fit text into</param>
        /// <returns></returns>
        //public Vector2 CalcStringSizeC(ReadOnlySpan<char> str, float maxWidth = 100000) {
        //    if (maxWidth <= 0) maxWidth = 100000;

        //    str = str.TrimEnd('\n').Trim('\r');

        //    int lineHeight = FontSize;
        //    int sizeX = 0, sizeY = lineHeight;

        //    int maxWidth2 = (int)(maxWidth / PixelScale);

        //    int currLineW = 0;
        //    for (int i = 0; i < str.Length; i++) {
        //        var c = str[i];

        //        switch (c) {
        //            case ' ': currLineW += SpaceAdvance; break;
        //            case '\t': currLineW += SpaceAdvance * 4; break;
        //            case '\n':
        //                sizeX = Math.Max(sizeX, currLineW);
        //                sizeY += lineHeight;
        //                currLineW = 0;
        //                break;
        //            case '\r':
        //                sizeX = Math.Max(sizeX, currLineW);
        //                currLineW = 0;
        //                break;

        //            default:
        //                if (c < ' ') continue;

        //                int advanceWidth;
        //                stbtt_GetCodepointHMetrics(FontInfo, str[i], &advanceWidth, null);

        //                if (currLineW + advanceWidth > maxWidth2) {
        //                    sizeX = Math.Max(sizeX, currLineW);
        //                    currLineW = 0;
        //                    sizeY += lineHeight;
        //                }

        //                currLineW += advanceWidth;
        //                break;
        //        }
        //    }

        //    return new Vector2(Math.Max(sizeX, currLineW), sizeY) * PixelScale;
        //}

        /// <summary>
        /// Calculate string size to fit inside a rectangle with a width. Word truncate.
        /// </summary>
        /// <param name="str">String to calculate</param>
        /// <param name="maxWidth">Width of rectangle to fit text into</param>
        /// <returns></returns>
        //public Vector2 CalcStringSizeW(ReadOnlySpan<char> str, float maxWidth = 1000000) {
        //    float sizeX = 0, sizeY = 0;
        //    float currLineW = 0;

        //    int begin = 0;
        //    while (begin < str.Length) {
        //        var wrap = begin + CalcWordWrapIndex(str[begin..], maxWidth);

        //        var slice = str[begin..wrap];
        //        if (begin != 0) slice = slice.TrimStart();

        //        for (int i = 0; i < slice.Length; i++) {
        //            var c = slice[i];

        //            switch (c) {
        //                case ' ': currLineW += SpaceWidth; break;
        //                case '\t': currLineW += SpaceWidth * 4; break;
        //                case '\n':
        //                    sizeX = Math.Max(sizeX, currLineW);
        //                    sizeY += FontSize;
        //                    currLineW = 0;
        //                    break;
        //                case '\r':
        //                    sizeX = Math.Max(sizeX, currLineW);
        //                    currLineW = 0;
        //                    break;

        //                default:
        //                    if (c < ' ') continue;

        //                    if (TryGetPackedChar(c, out var pc)) {
        //                        currLineW += pc.xadvance;
        //                    }
        //                    break;
        //            }
        //        }

        //        sizeX = Math.Max(sizeX, currLineW);
        //        sizeY += FontSize;

        //        currLineW = 0;

        //        begin = wrap;
        //    }

        //    return new Vector2(sizeX, sizeY);
        //}

        /// <summary>
        /// Calculate string size to fit inside a rectangle with a width. Word truncate. Support fallback codepoint.
        /// </summary>
        /// <param name="str">String to calculate</param>
        /// <param name="maxWidth">Width of rectangle to fit text into</param>
        /// <returns></returns>
        //public Vector2 CalcStringSizeW(ReadOnlySpan<char> str, char fallback, float maxWidth = 1000000) {
        //    float sizeX = 0, sizeY = 0;
        //    float currLineW = 0;

        //    int begin = 0;

        //    var _fallback = GetPackedChar(fallback);

        //    while (begin < str.Length) {
        //        var wrap = begin + CalcWordWrapIndex(str[begin..], maxWidth);

        //        var slice = str[begin..wrap];
        //        if (begin != 0) slice = slice.TrimStart();

        //        for (int i = 0; i < slice.Length; i++) {
        //            var c = slice[i];

        //            switch (c) {
        //                case ' ': currLineW += SpaceWidth; break;
        //                case '\t': currLineW += SpaceWidth * 4; break;
        //                case '\n':
        //                    sizeX = Math.Max(sizeX, currLineW);
        //                    sizeY += FontSize;
        //                    currLineW = 0;
        //                    break;
        //                case '\r':
        //                    sizeX = Math.Max(sizeX, currLineW);
        //                    currLineW = 0;
        //                    break;

        //                default:
        //                    if (c < ' ') continue;

        //                    if (TryGetPackedChar(c, out var pc)) {
        //                        currLineW += pc.xadvance;
        //                    } else if (_fallback.HasValue) {
        //                        currLineW += _fallback.Value.xadvance;
        //                    }
        //                    break;
        //            }
        //        }

        //        sizeX = Math.Max(sizeX, currLineW);
        //        sizeY += FontSize;

        //        currLineW = 0;

        //        begin = wrap;
        //    }

        //    return new Vector2(sizeX, sizeY);
        //}

        public override bool Alive() {
            return _packedChars != null;
        }

        public override void Destroy() {
            Bitmap.Destroy();
            Bitmap = null!;

            _packedChars = null!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public stbtt_packedchar? GetPackedChar(char codepoint) {
            if (TryGetPackedChar(codepoint, out var output)) {
                return output;
            }

            return null;
        }

        public bool TryGetPackedChar(char codepoint, out stbtt_packedchar output) {
            int d = codepoint - FirstCodepoint;

            if (d >= 0 && d < CodepointCount) {
                output = _packedChars[d];
                return true;
            }

            output = default;
            return false;
        }
    }
}
