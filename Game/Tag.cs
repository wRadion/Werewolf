using System;

namespace Werewolf.Game
{
    [Flags]
    public enum Tag
    {
        NONE = 0,
        LOVER = 1,
        MAYOR = 2,
        WEREWOLF_TARGET = 4,
        WITCH_TARGET = 8
    }
}
