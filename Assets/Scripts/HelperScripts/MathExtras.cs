using UnityEngine;

public class MathExtras {

	public static float CalculateDiagonalOfSquare(float sideLength)
    {
        return sideLength * Mathf.Sqrt(2.0f); //Mathf.Sqrt(Mathf.Pow(sideLength, 2.0f) * Mathf.Pow(sideLength, 2.0f));
    }
}
