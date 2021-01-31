using System;
using System.Collections.Generic;
using System.IO;

namespace Regul.S3PI.Interfaces
{
    /// <summary>
    /// Defines a vertex - a point in 3d space defined by three coordinates.
    /// </summary>
    public class Vertex : AHandlerElement, IEquatable<Vertex>
    {
        #region Attributes
        float x = 0f;
        float y = 0f;
        float z = 0f;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a vertex at { 0, 0, 0 }.
        /// </summary>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        public Vertex(EventHandler handler) : base(handler) { }
        /// <summary>
        /// Create a vertex from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        /// <param name="s"><see cref="Stream"/> containing coordinates.</param>
        public Vertex(EventHandler handler, Stream s) : base(handler) { Parse(s); }
        /// <summary>
        /// Create a vertex from a given value.
        /// </summary>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        /// <param name="basis"><see cref="Vertex"/> to copy.</param>
        public Vertex(EventHandler handler, Vertex basis)
            : this(handler, basis.x, basis.y, basis.z) { }
        /// <summary>
        /// Create a vertex at { x, y, z }.
        /// </summary>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        public Vertex(EventHandler handler, float x, float y, float z)
            : base(handler)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        #endregion

        #region Data I/O
        void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            x = r.ReadSingle();
            y = r.ReadSingle();
            z = r.ReadSingle();
        }

        /// <summary>
        /// Write the vertex to the given <see cref="Stream"/>.
        /// </summary>
        /// <param name="s"><see cref="Stream"/> to contain coordinates.</param>
        public void UnParse(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            w.Write(x);
            w.Write(y);
            w.Write(z);
        }
        #endregion

        #region AHandlerElement Members
        /// <summary>
        /// The list of available field names on this API object.
        /// </summary>
        public override List<string> ContentFields { get { return GetContentFields(GetType()); } }

        // /// <summary>
        // /// Get a copy of the <see cref="Vertex"/> but with a new change <see cref="EventHandler"/>.
        // /// </summary>
        // /// <param name="handler">The replacement <see cref="EventHandler"/> delegate.</param>
        // /// <returns>Return a copy of the <see cref="Vertex"/> but with a new change <see cref="EventHandler"/>.</returns>
        // public override AHandlerElement Clone(EventHandler handler) { return new Vertex(requestedApiVersion, handler, this); }
        #endregion

        #region IEquatable<BoxPoint> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(Vertex other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="Vertex"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="Vertex"/>.</param>
        /// <returns>true if the specified <see cref="object"/> is equal to the current <see cref="Vertex"/>; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj as Vertex != null ? Equals(obj as Vertex) : false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        #endregion

        #region Content Fields
        /// <summary>
        /// X coordinate
        /// </summary>
        [ElementPriority(1)]
        public float X { get { return x; } set { if (x != value) { x = value; OnElementChanged(); } } }
        /// <summary>
        /// Y coordinate
        /// </summary>
        [ElementPriority(2)]
        public float Y { get { return y; } set { if (y != value) { y = value; OnElementChanged(); } } }
        /// <summary>
        /// Z coordinate
        /// </summary>
        [ElementPriority(3)]
        public float Z { get { return z; } set { if (z != value) { z = value; OnElementChanged(); } } }

        /// <summary>
        /// A displayable representation of the object
        /// </summary>
        public string Value { get { return "{ " + string.Format("{0:F4}; {1:F4}; {2:F4}", x, y, z) + " }"; } }
        #endregion
    }

    /// <summary>
    /// Defines a bounding box - a imaginary box large enough to completely contain an object
    /// - by its minimum and maximum vertices.
    /// </summary>
    public class BoundingBox : AHandlerElement, IEquatable<BoundingBox>
    {
        #region Attributes
        Vertex min;
        Vertex max;
        #endregion

        #region Constructors
        /// <summary>
        /// Create an zero-sized bounding box.
        /// </summary>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        public BoundingBox(EventHandler handler) : base(handler)
        {
            min = new Vertex(handler);
            max = new Vertex(handler);
        }
        /// <summary>
        /// Create a bounding box from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        /// <param name="s"><see cref="Stream"/> containing vertices.</param>
        public BoundingBox(EventHandler handler, Stream s) : base(handler) { Parse(s); }
        /// <summary>
        /// Create a bounding box from a given value.
        /// </summary>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        /// <param name="basis"><see cref="BoundingBox"/> to copy.</param>
        public BoundingBox(EventHandler handler, BoundingBox basis)
            : this(handler, basis.min, basis.max) { }
        /// <summary>
        /// Create a bounding box with the specified minimum and maximum vertices.
        /// </summary>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        /// <param name="min">Minimum vertex.</param>
        /// <param name="max">Maximum vertex.</param>
        public BoundingBox(EventHandler handler, Vertex min, Vertex max)
            : base(handler)
        {
            this.min = new Vertex(handler, min);
            this.max = new Vertex(handler, max);
        }
        #endregion

        #region Data I/O
        void Parse(Stream s)
        {
            min = new Vertex(handler, s);
            max = new Vertex(handler, s);
        }

        /// <summary>
        /// Write the bounding box to the given <see cref="Stream"/>.
        /// </summary>
        /// <param name="s"><see cref="Stream"/> to contain vertices.</param>
        public void UnParse(Stream s)
        {
            if (min == null) min = new Vertex(handler);
            min.UnParse(s);

            if (max == null) max = new Vertex(handler);
            max.UnParse(s);
        }
        #endregion

        #region AHandlerElement Members

        /// <summary>
        /// The list of available field names on this API object.
        /// </summary>
        public override List<string> ContentFields { get { return GetContentFields(GetType()); } }

        // /// <summary>
        // /// Get a copy of the <see cref="BoundingBox"/> but with a new change <see cref="EventHandler"/>.
        // /// </summary>
        // /// <param name="handler">The replacement <see cref="EventHandler"/> delegate.</param>
        // /// <returns>Return a copy of the <see cref="BoundingBox"/> but with a new change <see cref="EventHandler"/>.</returns>
        // public override AHandlerElement Clone(EventHandler handler) { return new BoundingBox(requestedApiVersion, handler, this); }
        #endregion

        #region IEquatable<BoundingBox> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(BoundingBox other)
        {
            return min.Equals(other.min) && max.Equals(other.max);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="BoundingBox"/>.</param>
        /// <returns>true if the specified <see cref="object"/> is equal to the current <see cref="BoundingBox"/>; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj as BoundingBox != null ? Equals(obj as BoundingBox) : false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return min.GetHashCode() ^ max.GetHashCode();
        }

        #endregion

        #region Content Fields
        /// <summary>
        /// Minimum vertex
        /// </summary>
        [ElementPriority(1)]
        public Vertex Min { get { return min; } set { if (min != value) { min = new Vertex(handler, value); OnElementChanged(); } } }
        /// <summary>
        /// Maximum vertex
        /// </summary>
        [ElementPriority(2)]
        public Vertex Max { get { return max; } set { if (max != value) { max = new Vertex(handler, value); OnElementChanged(); } } }

        /// <summary>
        /// A displayable representation of the object
        /// </summary>
        public string Value { get { return string.Format("[ Min: {0} | Max: {1} ]", min.Value, max.Value); } }
        #endregion
    }
}