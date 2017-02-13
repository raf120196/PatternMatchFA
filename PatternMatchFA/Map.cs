using System.Collections;

namespace PatternMatchFA
{
    public class Map : Hashtable
    {
        public override void Add(object key, object mapTo)
        {
            Set set = null;
            if (base.Contains(key) == true)
            {
                set = (Set)base[key];
            }
            else
            {
                set = new Set();
            }
            set.AddElement(mapTo);
            base[key] = set;
        }

        public override object this[object key]
        {
            get
            {
                return base[key];
            }
            set
            {
                Add(key, value);
            }
        }
    }
}
