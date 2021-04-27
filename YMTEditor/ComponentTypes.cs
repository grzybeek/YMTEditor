using System;

namespace YMTEditor
{
    public class ComponentTypes
    {
        public enum ComponentNumbers
        {
            head = 0,
            berd = 1,
            hair = 2,
            uppr = 3,
            lowr = 4,
            hand = 5,
            feet = 6,
            teef = 7,
            accs = 8,
            task = 9,
            decl = 10,
            jbib = 11
        }

        public string GetComponentNameFromNumber(int number)
        {
            return Enum.GetName(typeof(ComponentNumbers), number);
        }
    }
}
