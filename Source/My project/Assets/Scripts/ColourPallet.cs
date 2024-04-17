using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourPallet
{
    public Color[] Pallet;

    public ColourPallet(Color[] pallet)
    {
        if (pallet != null)
        {
            Pallet = pallet;
        }        
    }

    public ColourPallet()
    {
        Pallet = new Color[] {
            Color.red,
            Color.blue,
            Color.green,
            Color.white,
            Color.gray,
            Color.black,
            Color.magenta,
            Color.yellow,
            Color.cyan,
            new Color(131 / 255f, 107 / 255f, 5 / 255f),
            new Color(059 / 255f, 131 / 255f, 189 / 255f),
            new Color(162 / 255f, 035 / 255f, 029 / 255f),
            new Color(056 / 255f, 044 / 255f, 030 / 255f),
            new Color(150 / 255f, 153 / 255f, 146 / 255f),
            new Color(108 / 255f, 059 / 255f, 042 / 255f),
            new Color(203 / 255f, 208 / 255f, 204 / 255f),
            new Color(169 / 255f, 131 / 255f, 007 / 255f),
            new Color(091 / 255f, 058 / 255f, 041 / 255f),
            new Color(006 / 255f, 057 / 255f, 113 / 255f),
            new Color(229 / 255f, 190 / 255f, 001 / 255f),
            new Color(030 / 255f, 030 / 255f, 030 / 255f),
            new Color(111 / 255f, 079 / 255f, 040 / 255f),
            new Color(031 / 255f, 052 / 255f, 056 / 255f),
            new Color(076 / 255f, 047 / 255f, 039 / 255f),
            new Color(214 / 255f, 174 / 255f, 001 / 255f),
            Color.clear,
        };
    }
}
