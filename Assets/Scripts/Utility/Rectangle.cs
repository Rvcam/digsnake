using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MyUtil
{
    public class Rectangle
    {
        private Vector3 _topLeft { get; }
        private float length { get; }
        private float height { get; }

        public Vector3 topLeft { get => _topLeft; }
        public Vector3 topRight { get => _topLeft + new Vector3(length, 0, 0); }
        public Vector3 bottomRight { get => _topLeft + new Vector3(length, -height, 0); }
        public Vector3 bottomLeft { get => _topLeft + new Vector3(0, -height, 0); }

        public Rectangle(Vector3 center, float length, float height)
        {
            _topLeft = center - new Vector3(length / 2, -height / 2);
            this.length = length;
            this.height = height;
        }

    }
}