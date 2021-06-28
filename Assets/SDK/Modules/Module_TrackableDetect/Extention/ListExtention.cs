using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

public static class ListExtention
{
    public static void SafeAdd<T>(this List<T> list, object item) where T : class
    {
        if (item is T)
        {
            list.Add(item as T);
        }
    }
}
