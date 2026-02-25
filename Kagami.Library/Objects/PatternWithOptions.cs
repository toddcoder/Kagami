namespace Kagami.Library.Objects;

public record PatternWithOptions(string OriginalPattern, bool IgnoreCase, bool Multiline, bool Global, bool TextOnly);