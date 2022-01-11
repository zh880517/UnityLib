using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ViewECS
{
    public class EntityContainer<T> where T : Entity
    {
        private static int SectionSize = 20;
        private List<T[]> sections = new List<T[]>();

        public void Add(T entity)
        {

        }
    }

}
