using System;
using System.Collections;
using System.Collections.Generic;

public class SecondAgent : behaviac.Agent
{
    private int p1 = 0;
    public void _set_p1(int value)
    {
        p1 = value;
    }
    public int _get_p1()
    {
        return p1;
    }

    public void m1(string value)
    {
        Console.WriteLine();
        Console.WriteLine("{0}", value);
        Console.WriteLine();
    }
}
