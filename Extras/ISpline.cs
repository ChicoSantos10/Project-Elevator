using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extras
{
    public interface ISpline
    {
        Vector3 Evaluate(float t);
        Vector3 EvaluateUnclamped(float t);

#if UNITY_EDITOR
        void OnDrawGizmos(Object target);
#endif
    }

    [Serializable]
    public class QuadraticBezierCurve : ISpline
    {
        [SerializeField] ControlPoint p0, p1, p2;

        public Transform Start => P0;
        public Transform End => P2;

        public ControlPoint P0 => p0;

        public ControlPoint P1 => p1;

        public ControlPoint P2 => p2;

        public QuadraticBezierCurve(Transform start, Transform control, Transform end)
        {
            p0 = new ControlPoint(start);
            p1 = new ControlPoint(control);
            p2 = new ControlPoint(end);
        }

        public Vector3 Evaluate(float t)
        {
            return Evaluate(t, P0.Position, P1.Position, P2.Position);
        }

        public Vector3 EvaluateUnclamped(float t)
        {
            return EvaluateUnclamped(t, P0.Position, P1.Position, P2.Position);
        }

        public static Vector3 Evaluate(float t, Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint)
        {
            // Vector3 a = Vector3.Lerp(startPoint, controlPoint, t);
            // Vector3 b = Vector3.Lerp(controlPoint, endPoint, t);
            // return Vector3.Lerp(a, b, t);

            t = Mathf.Clamp01(t);
            return EvaluateUnclamped(t, startPoint, controlPoint, endPoint);
        }

        public static Vector3 EvaluateUnclamped(float t, Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint)
        {
            Vector3 a = Vector3.LerpUnclamped(startPoint, controlPoint, t);
            Vector3 b = Vector3.LerpUnclamped(controlPoint, endPoint, t);
            return Vector3.LerpUnclamped(a, b, t);
        }

#if UNITY_EDITOR

        public struct DrawOptions
        {
            public bool P0Lock;
            public bool P1Lock;
            public bool P2Lock;
        }

        public void OnDrawGizmos(Object target)
        {
            OnDrawGizmos(target, new DrawOptions());
        }

        public void OnDrawGizmos(Object target, DrawOptions options)
        {
            EditorGUI.BeginChangeCheck();

            DrawHandle(P0, options.P0Lock, out Vector3 pos0, out Quaternion rot0);
            DrawHandle(P1, options.P1Lock, out Vector3 pos1, out Quaternion rot1);
            DrawHandle(P2, options.P2Lock, out Vector3 pos2, out Quaternion rot2);

            // Handles.DrawBezier(p0.Position, p1.Position, p1.Position, p2.Position,
            //     Color.white, 
            //     EditorGUIUtility.whiteTexture,
            //     1);

            const int size = 10;
            List<Vector3> points = new List<Vector3>(size + 1);

            for (int i = 0; i < size + 1; i++)
            {
                points.Add(Evaluate(i / (float) size));
            }

            Handles.DrawAAPolyLine(points.ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                Object[] undoObjs = {p0, p1, p2};
                Undo.RecordObjects(undoObjs, "Moved Control Points");

                p0.Position = pos0;
                p1.Position = pos1;
                p2.Position = pos2;

                p0.Rotation = rot0;
                p1.Rotation = rot1;
                p2.Rotation = rot2;
            }
        }

        void DrawHandle(ControlPoint p, bool locked, out Vector3 pos, out Quaternion rot)
        {
            pos = p.Position;
            rot = p.Rotation;

            if (!locked)
                Handles.TransformHandle(ref pos, ref rot);

            //return p.Position;
        }
#endif
    }
}