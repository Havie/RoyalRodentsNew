using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DelegateTutorial 
{
    public delegate  void DelegateExample(int a);
   static DelegateExample _Delegate1 = Foo;
   // 
   //     private void Start()
   //     {
   //         DelegateExample myDelegate = Foo;
   // 
   //         myDelegate.Invoke(1);
   //         myDelegate(2); // shorthand
   // 
   //         Foobar(myDelegate);
   //         Foobar(Foo);
   //         Foobar(Bar);
   //     }

   public static void  Foobar(DelegateExample customDelegate)
    {
        Debug.Log("FooBar: ");
        customDelegate(50);
    }


    static void  Foo(int a)
    {
        Debug.Log("FOO: " + a);
    }

    static void  Bar(int a)
    {

        Debug.Log("BAR: " + a);
    }

    public static void AddDelegate(DelegateExample customDelegate)
    {
        _Delegate1 += customDelegate;

    }

    public static void playDelegate()
    {
        _Delegate1(0);
    }
}
