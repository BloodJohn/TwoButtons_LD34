using HECS.Components;
using System;
using System.Linq;

namespace HECS.Systems
{
    public static class HECSHelper
    {
        private static bool TryToCalculateMask(this ComponentID[] maskTags, out ComponentID maskCalculated)
        {
            ComponentID mask = ComponentID.Default;

            if (maskTags == null || maskTags.Length == 0)
            {
                maskCalculated = mask;
                return false;
            }

            for (var x = 0; x < maskTags.Length; ++x)
            {
                mask |= maskTags[x];
            }

            maskCalculated = mask;
            return true;
        }

        public static ComponentID[] Components()
        {
            var arrayComponents = Enum.GetValues(typeof(ComponentID)).Cast<ComponentID>().ToArray();

            for (var x = 0; x < arrayComponents.Length; x++)
            {
                arrayComponents[x] = ComponentID.Default;
            }

            return arrayComponents;
        }
    }
}