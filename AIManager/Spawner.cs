using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIManager
{
    public class Spawner<T>
    {
        public T SpawnMonster(params object[] args)
        {
            return (T)Activator.CreateInstance(typeof(T),args);
        }
    }
}
