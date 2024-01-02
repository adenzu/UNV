using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

/**
Source:
Fuzzy Guidance Controller for an Autonomous Boat
https://ieeexplore.ieee.org/abstract/document/581294/
*/
public static class FuzzyGuidanceController
{
    // [Y, Heading, X]
    private static Label[,,] _ruleBanks = new Label[,,]
    {
        {
            {Label.PS, Label.PS, Label.PB, Label.NM, Label.NM},
            {Label.PS, Label.PS, Label.PM, Label.PM, Label.PM},
            {Label.NS, Label.NS, Label.PS, Label.PS, Label.PS},
            {Label.NS, Label.NS, Label.ZE, Label.PS, Label.PS},
            {Label.NS, Label.NS, Label.NS, Label.PS, Label.PS},
            {Label.NM, Label.NM, Label.NM, Label.NS, Label.NS},
            {Label.PM, Label.PM, Label.NB, Label.NS, Label.NS},
        },
        {
            {Label.PS, Label.PM, Label.PB, Label.NB, Label.NM},
            {Label.NS, Label.NS, Label.PM, Label.PB, Label.PB},
            {Label.NS, Label.NS, Label.PS, Label.PM, Label.PM},
            {Label.NM, Label.NS, Label.ZE, Label.PM, Label.PM},
            {Label.NM, Label.NM, Label.NS, Label.PS, Label.PS},
            {Label.NB, Label.NB, Label.NM, Label.PS, Label.PS},
            {Label.PM, Label.PB, Label.PB, Label.NM, Label.NS},
        },
        {
            {Label.NS, Label.NS, Label.ZE, Label.NM, Label.NM},
            {Label.NS, Label.NM, Label.PM, Label.NM, Label.NM},
            {Label.NM, Label.NM, Label.PS, Label.NB, Label.NB},
            {Label.NB, Label.NB, Label.ZE, Label.PB, Label.PB},
            {Label.PB, Label.PB, Label.NS, Label.PM, Label.PM},
            {Label.PM, Label.PM, Label.NM, Label.PM, Label.PS},
            {Label.PM, Label.PM, Label.ZE, Label.PS, Label.PS},
        },
        {
            {Label.ZE, Label.NS, Label.PS, Label.NS, Label.NM},
            {Label.NM, Label.PB, Label.NS, Label.NM, Label.NB},
            {Label.NB, Label.PB, Label.NM, Label.NM, Label.NB},
            {Label.NB, Label.PB, Label.PB, Label.NB, Label.PB},
            {Label.PB, Label.PM, Label.PM, Label.NB, Label.PB},
            {Label.PB, Label.PM, Label.PS, Label.NB, Label.PM},
            {Label.PM, Label.PS, Label.NS, Label.PS, Label.ZE},
        },
        {
            {Label.ZE, Label.NM, Label.ZE, Label.NM, Label.NM},
            {Label.NM, Label.NB, Label.NS, Label.NB, Label.NB},
            {Label.NB, Label.NB, Label.NM, Label.NB, Label.NB},
            {Label.NB, Label.PB, Label.PM, Label.NB, Label.NB},
            {Label.PB, Label.PB, Label.PM, Label.PB, Label.PB},
            {Label.PB, Label.PB, Label.PS, Label.PB, Label.PM},
            {Label.PM, Label.PM, Label.ZE, Label.PM, Label.ZE},
        },
    };

    public static float GetCrispRudderAngle(Vector3 position, Vector3 target, Vector3 forward, Vector3 targetHeading)
    {
        // Coordinate system of the waypoint where waypoint heading is the negative y axis 
        Vector2 relativePosition = Util.TransformPoint(position.XZ(), target.XZ(), targetHeading.XZ());
        float heading = Vector2.SignedAngle(forward.XZ(), targetHeading.XZ());

        LabelWithValue[] xLabels = GetXLabels(relativePosition.x);
        LabelWithValue[] yLabels = GetYLabels(relativePosition.y);
        LabelWithValue[] headingLabels = GetHeadingLabels(heading);

        LabelWithValue[] rudderAngleLabels = InferRudderAngleLabels(xLabels, yLabels, headingLabels);

        float crispRudderAngle = CentroidDefuzzification(rudderAngleLabels);

        return crispRudderAngle;
    }

    private static LabelWithValue[] InferRudderAngleLabels(LabelWithValue[] xLabels, LabelWithValue[] yLabels, LabelWithValue[] headingLabels)
    {
        List<LabelWithValue> labels = new List<LabelWithValue>();

        const float RUDDER_ANGLE_SCALE = 1f; // 0.5f -> (-60, 60), 1f -> (-30, 30), 2f -> (-15, 15)

        static float NB(float angle) => TriangularMembershipFunction(angle, -30.01f, -30f, -15f);
        static float NM(float angle) => TriangularMembershipFunction(angle, -25f, -15f, -5f);
        static float NS(float angle) => TriangularMembershipFunction(angle, -12f, -6f, 0f);
        static float ZE(float angle) => TriangularMembershipFunction(angle, -5f, 0f, 5f);
        static float PS(float angle) => TriangularMembershipFunction(angle, 0f, 6f, 12f);
        static float PM(float angle) => TriangularMembershipFunction(angle, 5f, 15f, 25f);
        static float PB(float angle) => TriangularMembershipFunction(angle, 15f, 30f, 30.01f);

        Func<float, float>[] correspondingMembershipFunctions = new Func<float, float>[]
        {
            NB, NM, NS, ZE, PS, PM, PB
        };

        float[,] correspondingMinMaxValues = new float[,]
        {
            {-30f, -15f},
            {-25f, -5f},
            {-12f, 0f},
            {-5f, 5f},
            {0f, 12f},
            {5f, 25f},
            {15f, 30f},
        };

        foreach (LabelWithValue xLabel in xLabels)
        {
            foreach (LabelWithValue yLabel in yLabels)
            {
                foreach (LabelWithValue headingLabel in headingLabels)
                {
                    Label label = _ruleBanks[YLabelToIndex(yLabel.label), HeadingLabelToIndex(headingLabel.label), XLabelToIndex(xLabel.label)];
                    labels.Add(new LabelWithValue
                    {
                        label = label,
                        value = Mathf.Min(xLabel.value, yLabel.value, headingLabel.value),
                        membershipFunction = x => correspondingMembershipFunctions[(int)label](x * RUDDER_ANGLE_SCALE),
                        minX = correspondingMinMaxValues[(int)label, 0],
                        maxX = correspondingMinMaxValues[(int)label, 1],
                    });
                }
            }
        }

        return labels.ToArray();
    }

    private static LabelWithValue[] GetXLabels(float x)
    {
        List<LabelWithValue> labels = new List<LabelWithValue>();

        const float X_SCALE = 1f; // 1f -> (-40, 40), 2f -> (-20, 20)

        x *= X_SCALE; // Scale x to fit membership functions (inverse scales membership functions to fit x)

        static float NB(float x) => TriangularMembershipFunction(x, float.MinValue, float.MinValue, -40f, -15f);
        static float NS(float x) => TriangularMembershipFunction(x, -20f, -10f, 0f);
        static float ZE(float x) => TriangularMembershipFunction(x, -5f, 0f, 5f);
        static float PS(float x) => TriangularMembershipFunction(x, 0, 10f, 20f);
        static float PB(float x) => TriangularMembershipFunction(x, 15f, 40f, float.MaxValue, float.MaxValue);

        labels.Add(new LabelWithValue { label = Label.NB, value = NB(x) });
        labels.Add(new LabelWithValue { label = Label.NS, value = NS(x) });
        labels.Add(new LabelWithValue { label = Label.ZE, value = ZE(x) });
        labels.Add(new LabelWithValue { label = Label.PS, value = PS(x) });
        labels.Add(new LabelWithValue { label = Label.PB, value = PB(x) });

        labels.RemoveAll(l => l.value <= 0.001f);

        return labels.ToArray();
    }

    private static LabelWithValue[] GetYLabels(float y)
    {
        List<LabelWithValue> labels = new List<LabelWithValue>();

        const float Y_SCALE = -1f; // 1f -> (-40, 40), 2f -> (-20, 20)

        y *= Y_SCALE; // Scale y to fit membership functions (inverse scales membership functions to fit y)

        static float NB(float y) => TriangularMembershipFunction(y, float.MinValue, float.MinValue, -40f, -15f);
        static float NS(float y) => TriangularMembershipFunction(y, -20f, -10f, 0f);
        static float ZE(float y) => TriangularMembershipFunction(y, -5f, 0f, 5f);
        static float PS(float y) => TriangularMembershipFunction(y, 0, 10f, 20f);
        static float PB(float y) => TriangularMembershipFunction(y, 15f, 40f, float.MaxValue, float.MaxValue);

        labels.Add(new LabelWithValue { label = Label.NB, value = NB(y) });
        labels.Add(new LabelWithValue { label = Label.NS, value = NS(y) });
        labels.Add(new LabelWithValue { label = Label.ZE, value = ZE(y) });
        labels.Add(new LabelWithValue { label = Label.PS, value = PS(y) });
        labels.Add(new LabelWithValue { label = Label.PB, value = PB(y) });

        labels.RemoveAll(l => l.value <= 0.001f);

        return labels.ToArray();
    }

    private static LabelWithValue[] GetHeadingLabels(float angle)
    {
        List<LabelWithValue> labels = new List<LabelWithValue>();

        const float ANGLE_SCALE = 1f; // 1f -> (-180, 180), 2f -> (-90, 90)

        angle *= ANGLE_SCALE; // Scale angle to fit membership functions (inverse scales membership functions to fit angle)

        const float ANGLE_OFFSET = 0f;
        static float NB(float angle) => TriangularMembershipFunction(angle, ANGLE_OFFSET + -185f, ANGLE_OFFSET + -135f, ANGLE_OFFSET + -85f);
        static float NM(float angle) => TriangularMembershipFunction(angle, ANGLE_OFFSET + -95f, ANGLE_OFFSET + -67.5f, ANGLE_OFFSET + -40f);
        static float NS(float angle) => TriangularMembershipFunction(angle, ANGLE_OFFSET + -45f, ANGLE_OFFSET + -22.5f, ANGLE_OFFSET + 0f);
        static float ZE(float angle) => TriangularMembershipFunction(angle, ANGLE_OFFSET + -10f, ANGLE_OFFSET + 0f, ANGLE_OFFSET + 10f);
        static float PS(float angle) => TriangularMembershipFunction(angle, ANGLE_OFFSET + 0f, ANGLE_OFFSET + 22.5f, ANGLE_OFFSET + 45f);
        static float PM(float angle) => TriangularMembershipFunction(angle, ANGLE_OFFSET + 40f, ANGLE_OFFSET + 67.5f, ANGLE_OFFSET + 95f);
        static float PB(float angle) => TriangularMembershipFunction(angle, ANGLE_OFFSET + 85f, ANGLE_OFFSET + 135f, ANGLE_OFFSET + 185f);

        labels.Add(new LabelWithValue { label = Label.NB, value = NB(angle) });
        labels.Add(new LabelWithValue { label = Label.NS, value = NS(angle) });
        labels.Add(new LabelWithValue { label = Label.NM, value = NM(angle) });
        labels.Add(new LabelWithValue { label = Label.ZE, value = ZE(angle) });
        labels.Add(new LabelWithValue { label = Label.PS, value = PS(angle) });
        labels.Add(new LabelWithValue { label = Label.PM, value = PM(angle) });
        labels.Add(new LabelWithValue { label = Label.PB, value = PB(angle) });

        labels.RemoveAll(l => l.value <= 0.001f);

        return labels.ToArray();
    }

    private static float TriangularMembershipFunction(float x, float a, float b, float c, float d)
    {
        if (x < a || x > d)
        {
            return 0;
        }
        else if (x >= b && x <= c)
        {
            return 1;
        }
        else if (x >= a && x <= b)
        {
            return (x - a) / (b - a);
        }
        else if (x >= c && x <= d)
        {
            return (d - x) / (d - c);
        }
        else
        {
            throw new Exception("Invalid x");
        }
    }

    private static float TriangularMembershipFunction(float x, float a, float b, float c)
    {
        if (x < a || x > c)
        {
            return 0;
        }
        else if (x >= a && x <= b)
        {
            return (x - a) / (b - a);
        }
        else if (x >= b && x <= c)
        {
            return (c - x) / (c - b);
        }
        else
        {
            throw new Exception("Invalid x");
        }
    }

    private static float CentroidDefuzzification(LabelWithValue[] labels, int n = 100)
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        foreach (LabelWithValue label in labels)
        {
            minX = Mathf.Min(minX, label.minX);
            maxX = Mathf.Max(maxX, label.maxX);
        }

        float sum = 0;
        float sumOfMaxValues = 0;
        for (int i = 0; i < n; i++)
        {
            float t = i * 1f / n;
            float x = Mathf.Lerp(minX, maxX, t);
            float y = 0;
            foreach (LabelWithValue label in labels)
            {
                y = Mathf.Max(y, Mathf.Min(label.value, label.membershipFunction(x)));
            }
            sum += x * y;
            sumOfMaxValues += y;
        }
        return sum / sumOfMaxValues;
    }

    private struct LabelWithValue
    {
        public Label label;
        public float value;
        public Func<float, float> membershipFunction;
        public float minX, maxX;
    }

    private enum Label
    {
        NB, // NegativeBig,
        NM, // NegativeMedium,
        NS, // NegativeSmall,
        ZE, // Zero,
        PS, // PositiveSmall,
        PM, // PositiveMedium,
        PB, // PositiveBig,
    }

    private static int XLabelToIndex(Label label)
    {
        return label switch
        {
            Label.NB => 0,
            Label.NS => 1,
            Label.ZE => 2,
            Label.PS => 3,
            Label.PB => 4,
            _ => throw new Exception("Invalid label"),
        };
    }

    private static int YLabelToIndex(Label label)
    {
        return label switch
        {
            Label.NB => 0,
            Label.NS => 1,
            Label.ZE => 2,
            Label.PS => 3,
            Label.PB => 4,
            _ => throw new Exception("Invalid label"),
        };
    }

    private static int HeadingLabelToIndex(Label label)
    {
        return label switch
        {
            Label.NB => 0,
            Label.NS => 1,
            Label.NM => 2,
            Label.ZE => 3,
            Label.PS => 4,
            Label.PM => 5,
            Label.PB => 6,
            _ => throw new Exception("Invalid label"),
        };
    }
}
