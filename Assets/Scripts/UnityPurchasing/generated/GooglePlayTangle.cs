// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("lYk4ftjqnB6HqWoSlM6sfX2ZD3Hu7k0wxcYyKOptOda0LkxuHXnZ2yD/ZbAnX9NXXgpxM8LKukRA0nq3ltLpwkETn9eIVZcwB/YMcKnljz7sriFB69QtNj16k5JOLMpBAvtbwow/IHYnK/c8KFOHUjPKLrp057dkS/l6WUt2fXJR/TP9jHZ6enp+e3g8JePnX8W1gGeT71ClDi5ptneFX31es9eu/FfVmU0SXzj8A/CP7Me1+Xp0e0v5enF5+Xp6e6XL0A0j/Dcj6wx23ZOk2g5FFzI2IoReNKBSbjlRBWKW6ntmbEvQ5TnUuB0iyt1FSIfoLmpIpxolMkq2uacxMu+pyyRjYK7WILEqYoAav/CcuC1KWRbs1eBorN9x/xJDenl4ent6");
        private static int[] order = new int[] { 2,8,5,10,4,9,9,12,10,10,13,13,13,13,14 };
        private static int key = 123;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
