using System;

namespace TenantSearch.Comparers
{
    /// <summary>
    /// Uses correlation to compare featrues of two tenants
    /// </summary>
    public class TenantComparer
    {
        public static double CompareVectors(double[] tenantFeaturesOne, double[] tenantFeaturesTwo)
        {
            double average1 = 0.0;
            double average2 = 0.0;
            int count = 0;

            for (int i = 0; i < tenantFeaturesOne.Length; i++)
            {
                if (tenantFeaturesOne[i] != 0 && tenantFeaturesTwo[i] != 0)
                {
                    average1 += tenantFeaturesOne[i];
                    average2 += tenantFeaturesTwo[i];
                    count++;
                }
            }

            average1 /= count;
            average2 /= count;

            double sum = 0.0;
            double squares1 = 0.0;
            double squares2 = 0.0;

            for (int i = 0; i < tenantFeaturesOne.Length; i++)
            {
                if (tenantFeaturesOne[i] != 0 && tenantFeaturesTwo[i] != 0)
                {
                    sum += (tenantFeaturesOne[i] - average1) * (tenantFeaturesTwo[i] - average2);
                    squares1 += Math.Pow(tenantFeaturesOne[i] - average1, 2);
                    squares2 += Math.Pow(tenantFeaturesTwo[i] - average2, 2);
                }
            }

            return sum / Math.Sqrt(squares1 * squares2);
        }
    }
}
