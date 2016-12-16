using System.Collections;
using UnityEngine;

public class ReMapping
{
    // 原始左边界
    private float srcLeftMargin;

    // 原始右边界
    private float srcRightMargin;


    // 目的左边界
    private float dstLeftMargin;

    // 目的右边界
    private float dstRightMargin;

    private readonly float slope;
    private readonly float intercept;

    public ReMapping(float srcLeftMargin, float srcRightMargin, float dstLeftMargin, float dstRightMargin)
    {
        this.srcLeftMargin = srcLeftMargin;
        this.srcRightMargin = srcRightMargin;
        this.dstLeftMargin = dstLeftMargin;
        this.dstRightMargin = dstRightMargin;

        float srcDelta = srcRightMargin - srcLeftMargin;
        float dstDelta = dstRightMargin - dstLeftMargin;

        // y = slope * x + intercept
        this.slope = dstDelta / srcDelta;
        this.intercept = dstLeftMargin - slope * srcLeftMargin;
    }

    public float GetValue(float value)
    {
        float result = slope*value + intercept;
        result = Mathf.Clamp(result, dstLeftMargin, dstRightMargin);
        return result;
    }
}
