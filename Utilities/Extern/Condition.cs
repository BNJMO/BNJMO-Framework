using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition {

    public static Condition TRUE = New(() => true);
    public static Condition FALSE = New(() => false);

    public static Condition New(Func<bool> func)
    {
        return new Condition(func);
    }

    private Func<bool> func;
    public bool IsTrue
    { get { return func.Invoke(); } }

    private Condition(Func<bool> func)
    {
        this.func = func;
    }

    public Condition And(Func<bool> func)
    {
        return And(New(func));
    }

    public Condition And(Condition other)
    {
        return New(() => IsTrue && other.IsTrue);
    }

    public Condition Or(Func<bool> func)
    {
        return Or(New(func));
    }

    public Condition Or(Condition other)
    {
        return New(() => IsTrue || other.IsTrue);
    }

    public Condition Not()
    {
        return New(() => !IsTrue);
    }
}
