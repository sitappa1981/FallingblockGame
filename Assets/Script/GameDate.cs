using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDate {

    private static GameDate instance = new GameDate();
    private GameDate() { }
    public int[] point = new int[6];
    public int getpoint = 0;

    public static int Point0 {
        get { return instance.point[0]; }
        set { instance.point[0] = value; }
    }
    public static int Point1 {
        get { return instance.point[1]; }
        set { instance.point[1] = value; }
    }
    public static int Point2 {
        get { return instance.point[2]; }
        set { instance.point[2] = value; }
    }
    public static int Point3 {
        get { return instance.point[3]; }
        set { instance.point[3] = value; }
    }
    public static int Point4 {
        get { return instance.point[4]; }
        set { instance.point[4] = value; }
    }
    public static int Point5 {
        get { return instance.point[5]; }
        set { instance.point[5] = value; }
    }
    public static int getPoint {
        get { return instance.getpoint; }
        set { instance.getpoint = value; }
    }
}
