using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities {

    public static Color MakeTransparent(Color color) {
        return new Color(color.r, color.g, color.b, 0f);
    }

    public static Color MakeOpaque(Color color) {
        return new Color(color.r, color.g, color.b, 1f);
    }

    public static Color SetAlpha(Color color, float alpha) {
        return new Color(color.r, color.g, color.b, alpha);
    }

}
