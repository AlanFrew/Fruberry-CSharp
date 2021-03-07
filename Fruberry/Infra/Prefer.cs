namespace Fruberry {
    /// <summary>
    /// A selectin of various attributes for a data structure to have, ranked by priority
    /// </summary>
    public enum Prefer {
        Nothing = 0,
        Add = 256,
        Remove = 1,
        Find = 2,
        AllowDupes = 4,
        NoDupes = 8,
        NoCompare = 16,
        MinMemory = 32,
        MaxMemory = 64,
        FixedSize = 128
    }
}
