/*********************************************************
 * Class:           Polygon
 * Author:          Chris Amert
 * Last Modified:   12 April 2010
 * ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace ChairWars.Collisions
{
    /// <summary>
    /// Class for storing a set of points representing a Polygon.  Methods exist for drawing
    /// the polygon, detecting collisions, and forming a convex hull from the points.
    /// </summary>
    public class Polygon
    {
        public List<Vector2> vertexList;
        public float currentRotation = 0.0f;
        protected Vector2 upperLeft;
        protected Vector2 lowerRight;
        protected Vector2 center;
        public float roughRadius;

        /// <summary>
        /// Creates a polygon containing no points.
        /// </summary>
        public Polygon()
        {
            this.vertexList = new List<Vector2>();
            this.upperLeft = Vector2.Zero;
            this.lowerRight = Vector2.Zero;
            this.center = Vector2.Zero;
        }

        /// <summary>
        /// Creates a polygon containing a variable number of points.
        /// </summary>
        /// <param name="p1">The first point contained in the polygon</param>
        /// <param name="list">A list of additional points that are in the polygon.</param>
        public Polygon(Vector2 p1, params Vector2[] list)
        {
            this.vertexList = new List<Vector2>();
            this.vertexList.Add(p1);
            for (int i = 0; i < list.Length; i++)
            {
                this.vertexList.Add(list[i]);
            }

            this.CalculatePoints();
        }

        /// <summary>
        /// Creates a polygon from the given points.
        /// </summary>
        /// <param name="x">The X coordinate of the first point.</param>
        /// <param name="y">The Y coordinate of the first point.</param>
        /// <param name="coords">A list of additional points, in the format X2, Y2, X3, Y3, ...</param>
        public Polygon(float x, float y, params float[] coords)
        {
            this.vertexList = new List<Vector2>();

            if (coords.Length % 2 != 0)
            {
                throw new ArgumentException("Invalid number of coordinates.");
            }

            this.vertexList.Add(new Vector2(x, y));
            for (int i = 0; i < coords.Length; i += 2)
            {
                this.vertexList.Add(new Vector2(coords[i], coords[i + 1]));
            }

            this.CalculatePoints();
        }

        /// <summary>
        /// Creates a new polygon from the given list of Vectors.
        /// </summary>
        /// <param name="inList">The list of points to use to create the polygon.</param>
        public Polygon(List<Vector2> inList)
        {
            this.vertexList = new List<Vector2>();
            foreach (Vector2 item in inList)
            {
                this.vertexList.Add(item);
            }

            this.CalculatePoints();
            
        }


        /// <summary>
        /// Adds a vertex to the object.
        /// </summary>
        /// <param name="v1">The vector to add to the object.</param>
        public void Add(Vector2 v1)
        {
            this.vertexList.Add(v1);
        }

        /// <summary>
        /// Moves all the vertices in the object the given amount.
        /// </summary>
        /// <param name="x">X amount to move</param>
        /// <param name="y">Y amount to move</param>
        public void Move(float x, float y)
        {
            List<Vector2> newList = new List<Vector2>();
            for (int i = 0; i < this.vertexList.Count; i++)
            {
                newList.Add(new Vector2(this.vertexList[i].X + x, this.vertexList[i].Y + y));
            }

            this.center.X += x;
            this.center.Y += y;

            this.vertexList = newList;
        }

        /// <summary>
        /// Moves all the vertices in the object by the given vector.
        /// </summary>
        /// <param name="m">Vector used to move the object.</param>
        public void Move(Vector2 m)
        {
            this.Move(m.X, m.Y);
        }

        /// <summary>
        /// Moves the center of the object to a given point.
        /// </summary>
        /// <param name="m">The point to move to.</param>
        public void MoveTo(Vector2 m)
        {
            this.Move(m - this.center);
        }

        public void MoveXTo(float x)
        {
            float deltaX = x - this.upperLeft.X;

            this.Move(deltaX, 0);
        }

        public void MoveYTo(float y)
        {
            float deltaY = y - this.upperLeft.Y;
            this.Move(0, deltaY);
        }

        public void Rotate(double targetRotation)
        {
            int i;
            float tempX, tempY;
            double rotation = currentRotation - targetRotation;
            for (i = 0; i < this.vertexList.Count; i++)
            {
                this.vertexList[i] -= this.center;
                tempX = (float)(this.vertexList[i].X * Math.Cos(rotation) - this.vertexList[i].Y * Math.Sin(rotation));
                tempY = (float)(this.vertexList[i].X * Math.Sin(rotation) + this.vertexList[i].Y * Math.Cos(rotation));
                this.vertexList[i] = new Vector2(tempX, tempY);
                this.vertexList[i] += this.center;
            }
            currentRotation = (float)targetRotation;

        }

        /// <summary>
        /// Recalculates all the stored points, which are upperLeft, lowerRight, and center.
        /// If the polygon is empty, it sets everything to the 0 vector.
        /// </summary>
        protected void CalculatePoints()
        {
            if (this.vertexList.Count < 1)
            {
                this.lowerRight = Vector2.Zero;
                this.upperLeft = Vector2.Zero;
                this.center = Vector2.Zero;
                return;
            }

            float minX, maxX, minY, maxY, totalX = 0, totalY = 0;
            minX = this.vertexList[0].X;
            maxX = minX;
            minY = this.vertexList[0].Y;
            maxY = minY;

            foreach (Vector2 v in this.vertexList)
            {
                totalX += v.X;
                totalY += v.Y;
                if (v.X < minX)
                    minX = v.X;
                if (v.X > maxX)
                    maxX = v.X;
                if (v.Y < minY)
                    minY = v.Y;
                if (v.Y > maxY)
                    maxY = v.Y;
            }

            this.upperLeft = new Vector2(minX, minY);
            this.lowerRight = new Vector2(maxX, maxY);

            roughRadius = Math.Abs(upperLeft.X - lowerRight.X);
            if(Math.Abs(upperLeft.Y - lowerRight.Y) > roughRadius)
            {
                roughRadius = Math.Abs(upperLeft.Y - lowerRight.Y);
            }
            this.center = new Vector2(totalX / this.vertexList.Count, totalY / this.vertexList.Count);

        }

        /// <summary>
        /// Returns the Upper Left corner of the polygon.  This is not necessarly a point
        /// in the polygon, but rather the Upper Left corner of a box were drawn around the polygon.
        /// </summary>
        /// <returns>The Upper Left corner.</returns>
        public Vector2 GetUpperLeftCorner()
        {
            return new Vector2(this.upperLeft.X, this.upperLeft.Y);
        }

        /// <summary>
        /// Returns the Lower Right corner of the polygon.  This is not necessarly a point
        /// in the polygon, but rather the Lower Right corner of a box were drawn around the polygon.
        /// </summary>
        /// <returns>The Lower Right corner.</returns>
        public Vector2 GetLowerRightCorner()
        {
            return new Vector2(this.lowerRight.X, this.lowerRight.Y);
        }

        /// <summary>
        /// Returns the minimum X value within the Polygon.
        /// </summary>
        /// <returns>The minimum X value.</returns>
        public float GetMinX()
        {
            return this.upperLeft.X;
        }

        /// <summary>
        /// Returns the maximum X value within the Polygon.
        /// </summary>
        /// <returns>The maximum X value.</returns>
        public float GetMaxX()
        {
            return this.lowerRight.X;
        }

        /// <summary>
        /// Returns the minimum Y value within the Polygon.
        /// </summary>
        /// <returns>The minimum Y value.</returns>
        public float GetMinY()
        {
            return this.upperLeft.Y;
        }

        /// <summary>
        /// Returns the maximum Y value within the Polygon.
        /// </summary>
        /// <returns>The maximum Y value.</returns>
        public float GetMaxY()
        {
            return this.lowerRight.Y;
        }

        /// <summary>
        /// Returns the center of the polygon.  Note that this is simply an average of all the points
        /// in the polygon.
        /// </summary>
        /// <returns>The center of the polygon.</returns>
        public Vector2 GetCenter()
        {
            return new Vector2(this.center.X, this.center.Y);
        }

        /// <summary>
        /// Draws a single line.
        /// </summary>
        /// <param name="sprite">SpriteBatch to draw the line.</param>
        /// <param name="pixel">A 1 pixel image used to draw the line.</param>
        /// <param name="start">The starting poing.</param>
        /// <param name="end">The ending poing</param>
        /// <param name="color">The color of the line</param>
        public static void DrawLine(SpriteBatch sprite, Texture2D pixel, Vector2 start, Vector2 end, Color color)
        {
            int distance = (int)Vector2.Distance(start, end);

            Vector2 connection = end - start;
            Vector2 baseVector = new Vector2(1, 0);

            float alpha = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);

            sprite.Draw(pixel, new Rectangle((int)start.X, (int)start.Y, distance, 1),
                null, color, alpha, new Vector2(0, 0), SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draws the object using the given spritebatch, pixel and color.
        /// </summary>
        /// <param name="sprite">The sprite batch to draw the object.</param>
        /// <param name="pixel">A 1 pixel texture to use to draw the lines.</param>
        /// <param name="color">The color of the line.</param>
        public void Draw(SpriteBatch sprite, Texture2D pixel, Color color)
        {
            this.Draw(sprite, pixel, color, true);
        }

        /// <summary>
        /// Same as above, but gives the option to draw the last line or not
        /// </summary>
        /// <param name="sprite">The spriteBach to draw the object.</param>
        /// <param name="pixel">The pixel texture to use to draw the lines.</param>
        /// <param name="color">The color the lines should be.</param>
        /// <param name="drawLast">Tells it to draw the last line or not.</param>
        public void Draw(SpriteBatch sprite, Texture2D pixel, Color color, bool drawLast)
        {
            int length = this.vertexList.Count;

            for (int i = 0; i < length - 1; i++)
            {
                DrawLine(sprite, pixel, this.vertexList[i], this.vertexList[i + 1], color);
            }

            if (drawLast)
                DrawLine(sprite, pixel, this.vertexList[length - 1], this.vertexList[0], color);
        }

        /// <summary>
        /// Draws the object with a given screen offest.  In effect, this subtracts the offset
        /// from all the points when drawing the polygon.
        /// </summary>
        /// <param name="sprite">The SpriteBatch to use to draw the Polygon.</param>
        /// <param name="pixel">The pixel texture to use to draw the line.</param>
        /// <param name="color">The Color the line should be.</param>
        /// <param name="offsetX">The X offset to subtract.</param>
        /// <param name="offsetY">The Y offset to subtract.</param>
        public void DrawOffset(SpriteBatch sprite, Texture2D pixel, Color color, int offsetX, int offsetY)
        {
            int length = this.vertexList.Count;

            for (int i = 0; i < length - 1; i++)
            {
                DrawLine(sprite, pixel, new Vector2(this.vertexList[i].X - offsetX, this.vertexList[i].Y - offsetY), new Vector2(this.vertexList[i + 1].X - offsetX, this.vertexList[i + 1].Y - offsetY), color);
            }

            DrawLine(sprite, pixel, new Vector2(this.vertexList[length - 1].X - offsetX, this.vertexList[length - 1].Y - offsetY), new Vector2(this.vertexList[0].X - offsetX, this.vertexList[0].Y - offsetY), color);
        }

        /// <summary>
        /// Draws the object with a given screen offest.  In effect, this subtracts the offset
        /// from all the points when drawing the polygon.
        /// </summary>
        /// <param name="sprite">The SpriteBatch to use to draw the Polygon.</param>
        /// <param name="pixel">The pixel texture to use to draw the line.</param>
        /// <param name="color">The Color the line should be.</param>
        /// <param name="offsetX">The X offset to subtract.</param>
        /// <param name="offsetY">The Y offset to subtract.</param>
        /// <param name="drawLast">If true, draws the line connecting the first and last points.  Else, it is not drawn.</param>
        public void DrawOffset(SpriteBatch sprite, Texture2D pixel, Color color, int offsetX, int offsetY, bool drawLast)
        {
            int length = this.vertexList.Count;

            for (int i = 0; i < length - 1; i++)
            {
                DrawLine(sprite, pixel, new Vector2(this.vertexList[i].X - offsetX, this.vertexList[i].Y - offsetY), new Vector2(this.vertexList[i + 1].X - offsetX, this.vertexList[i + 1].Y - offsetY), color);
            }

            if (drawLast)
                DrawLine(sprite, pixel, new Vector2(this.vertexList[length - 1].X - offsetX, this.vertexList[length - 1].Y - offsetY), new Vector2(this.vertexList[0].X - offsetX, this.vertexList[0].Y - offsetY), color);
        }

        /// <summary>
        /// Test if this object collides with another.  This uses the Method of Seperating Axis.
        /// </summary>
        /// <param name="other">The object to collide against.</param>
        /// <param name="move">The output vector containing the amount to move the other
        /// polygon to get out of the collision.</param>
        /// <returns>True if they are colliding, false otherwise.</returns>
        public bool CollisionTest(Polygon other, out Vector2 move)
        {
            Vector2 mtd = new Vector2(10000, 10000);        //Use an absurdly large initial value.
            Vector2 par, unit;
            Vector2? temp;

            // test all axis for this object
            for (int i = 0; i < this.vertexList.Count; i++)
            {
                //The line going from vertex (i+1) to (i)
                par = this.vertexList[(i + 1) % this.vertexList.Count] - this.vertexList[i];

                //Get the Unit Vector Normal to the lines.  This is the line we are projecting on to.
                unit = Vector2.Normalize(new Vector2(-par.Y, par.X));

                //Test if the projections are overlapping.
                temp = LineTest(unit, this, other);

                //If not, the objects are not colliding, and we have an early out.
                if (temp == null)
                {
                    move = Vector2.Zero;
                    return false;
                }

                //See if our mtd is smaller, and if it is, replace it.
                if (((Vector2)temp).Length() < mtd.Length())
                    mtd = (Vector2)temp;
            }

            //Special case of the line whice goes from index (0) to index (Size - 1)
            /*
            par = this.vertexList[0] - this.vertexList[this.vertexList.Count - 1];
            unit = Vector2.Normalize(new Vector2(-par.Y, par.X));
            temp = LineTest(unit, this, other);
            if (temp == null)
            {
                move = Vector2.Zero;
                return false;
            }
            
            if (((Vector2)temp).Length() < mtd.Length())
                mtd = (Vector2)temp;
            */
            // test all axis for the other object. We have to do this, since all the lines
            // have to be overlapping to have a collision.
            // For more information, see comments above, as the two sections are the same.
            for (int i = 0; i < other.vertexList.Count; i++)
            {
                par = other.vertexList[(i + 1) % other.vertexList.Count] - other.vertexList[i];
                unit = Vector2.Normalize(new Vector2(-par.Y, par.X));
                temp = LineTest(unit, this, other);
                if (temp == null)
                {
                    move = Vector2.Zero;
                    return false;
                }

                if (((Vector2)temp).Length() < mtd.Length())
                    mtd = (Vector2)temp;
            }

            /*
            par = other.vertexList[0] - other.vertexList[other.vertexList.Count - 1];
            unit = Vector2.Normalize(new Vector2(-par.Y, par.X));
            temp = LineTest(unit, this, other);
            if (temp == null)
            {
                move = Vector2.Zero;
                return false;
            }

            if (((Vector2)temp).Length() < mtd.Length())
                mtd = (Vector2)temp;
            */
            //Since we have made it this far, we know for sure we have a collision.

            if (mtd.X > 0)
                mtd.X += 1;
            if (mtd.X < 0)
                mtd.X -= 1;
            if (mtd.Y > 0)
                mtd.Y += 1;
            if (mtd.Y < 0)
                mtd.Y -= 1;
            move = mtd;
            return true;
        }
        
        /// <summary>
        /// Checks if this object is colliding with another one.  Overloads for 
        /// getting the vector to move out of the collision.
        /// </summary>
        /// <param name="other">The object to check agains.</param>
        /// <returns>True if they are colliding, false otherwise.</returns>
        public bool CollisionTest(Polygon other)
        {
            Vector2 ret;
            return this.CollisionTest(other, out ret);
        }

        /// <summary>
        /// Uses the Jarvis March Algorithm to make the points contained in the polygon 
        /// form a convex hull, going in the clockwise direction.  Any points
        /// not in the hull are discarded.
        /// </summary>
        public void MakeConvex()
        {
            if (this.vertexList.Count < 3)
                return;

            float prevAngle;        //the angle from the previous

            float absoluteTemp, relativeTemp;                   //Temporary variables for storing calculated values
            float absoluteTempSearch, relativeTempSearch;       //Stores the lowest values we have found so far

            List<Vector2> newPoints = new List<Vector2>();      //Stores our points forming the new hull
            List<Vector2> pointsToAdd = new List<Vector2>();    //The list of points we haven't added to the hull yet

            foreach (Vector2 v in this.vertexList)
            {
                pointsToAdd.Add(v);
            }

            Vector2 initialPoint;       //our starting point
            Vector2 prevPoint;       //the previous point
            Vector2 tempPoint;          //temp point for our search

            initialPoint = this.vertexList[0];

            //find left most point
            foreach (Vector2 v in this.vertexList)
            {
                if (v.X < initialPoint.X)
                    initialPoint = v;
            }

            prevAngle = 90;
            prevPoint = initialPoint;
            newPoints.Add(prevPoint);

            //Execute march
            //Note:  The constant converting between degrees and radians
            //was initially added for debugging (I found degrees easier to visualize than radians.)
            //However, if I try removing the conversions, it breaks.  Since this function should only be called
            //in the level editor, in the end the performance hit won't matter at all.
            do
            {
                //Get an initial value
                tempPoint = this.vertexList[0];
                absoluteTemp = ToDegrees(GetPolarAngle(prevPoint, tempPoint));
                relativeTemp = ToDegrees(GetAngleInRange(ToRadians(prevAngle) - ToRadians(absoluteTemp)));
                foreach (Vector2 v in pointsToAdd)
                {
                    //See if we can find a lower value
                    absoluteTempSearch = ToDegrees(GetPolarAngle(prevPoint, v));
                    relativeTempSearch = ToDegrees(GetAngleInRange(ToRadians(prevAngle) - ToRadians(absoluteTempSearch)));
                    if (relativeTempSearch < relativeTemp)
                    {
                        relativeTemp = relativeTempSearch;
                        absoluteTemp = absoluteTempSearch;
                        tempPoint = v;
                    }
                }

                //Add our lowest value to the hull
                prevAngle = absoluteTemp;
                prevPoint = tempPoint;

                newPoints.Add(prevPoint);
                pointsToAdd.Remove(prevPoint);

            } while (prevPoint != initialPoint);

            //Assign our new hull
            this.vertexList = newPoints;

            //recalcuate information
            this.CalculatePoints();

        }

        /// <summary>
        /// Function to get force an angle to be in the 0-2(PI) range.
        /// </summary>
        /// <param name="angle">The angle to force in range.</param>
        /// <returns>An angle going the same direction, but in the 0-2(PI) range.</returns>
        public static float GetAngleInRange(float angle)
        {
            if (angle < 0)
                return angle + ((float)Math.PI * 2);
            else if (angle > ((float)Math.PI * 2))
                return angle - ((float)Math.PI * 2);
            else
                return angle;
        }

        /// <summary>
        /// Returns the Polar angle between the two angles.  The center is used as the center
        /// of the coordinate system.
        /// </summary>
        /// <param name="center">The point to use as the center.</param>
        /// <param name="toFind">The point to find the angle of.</param>
        /// <returns>The polar angle of the toFind point.</returns>
        public static float GetPolarAngle(Vector2 center, Vector2 toFind)
        {
            Vector2 coord = new Vector2();
            coord.X = toFind.X - center.X;
            coord.Y = center.Y - toFind.Y;

            if (coord.X > 0 && coord.Y > 0)
            {
                return (float)Math.Atan(coord.Y / coord.X);
            }
            else if (coord.X > 0 && coord.Y < 0)
            {
                return (float)Math.Atan(coord.Y / coord.X) + (float)(2 * Math.PI);
            }
            else if (coord.X < 0)
            {
                return (float)Math.Atan(coord.Y / coord.X) + (float)Math.PI;
            }
            else if (coord.X == 0 && coord.Y > 0)
            {
                return (float)Math.PI / 2;
            }
            else if (coord.X == 0 && coord.Y < 0)
            {
                return (3 * (float)Math.PI) / 2;
            }
            else if (coord.X == 0 && coord.Y == 0)
            {
                return -100000;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Converts an angle in degrees to radians.
        /// </summary>
        /// <param name="degrees">The angle in degrees.</param>
        /// <returns>The angle in radians.</returns>
        public static float ToRadians(float degrees)
        {
            return degrees * ((float)(Math.PI) / 180);
        }

        /// <summary>
        /// Converts an angle in radians to degrees.
        /// </summary>
        /// <param name="radians">The angle in radians.</param>
        /// <returns>The angle in degrees.</returns>
        public static float ToDegrees(float radians)
        {
            return radians * (180 / (float)Math.PI);
        }

        /// <summary>
        /// The line test used in collision test.  Essentally projects all points from both
        /// Polygons onto the line, then tests if they over lap.
        /// </summary>
        /// <param name="line">The line to project onto.  Assumed to go through
        /// the origin to this point.</param>
        /// <param name="obj1">First object to test</param>
        /// <param name="obj2">Second object to test</param>
        /// <returns>The vector to move obj1 out of the collision, if there is one.  If there isnt one,
        /// this returns null.</returns>
        private static Vector2? LineTest(Vector2 line, Polygon obj1, Polygon obj2)
        {
            double temp;
            double max1, min1;      //Max and min values for obj1
            double max2, min2;      //Max and min values for obj2

            // project each point in obj1 onto the unit vector and find the max and min values
            max1 = min1 = Vector2.Dot(line, obj1.vertexList[0]);
            for (int i = 1; i < obj1.vertexList.Count; i++)
            {
                temp = Vector2.Dot(line, obj1.vertexList[i]);
                if (temp > max1)
                    max1 = temp;
                if (temp < min1)
                    min1 = temp;
            }

            // project each point in obj2 onto the unit vector and find the max and min values
            max2 = min2 = Vector2.Dot(line, obj2.vertexList[0]);
            for (int i = 1; i < obj2.vertexList.Count; i++)
            {
                temp = Vector2.Dot(line, obj2.vertexList[i]);
                if (temp > max2)
                    max2 = temp;
                if (temp < min2)
                    min2 = temp;
            }

            // if they are not overlapping, return null
            if (max1 < min2)
                return null;
            else if (min1 > max2)
                return null;

            // since they are overlapping, find the minimal distance
            if (Math.Abs(min2 - max1) < Math.Abs(max2 - min1))
            {
                line.X *= (float)(min2 - max1);
                line.Y *= (float)(min2 - max1);
                return line;
            }

            line.X *= (float)(max2 - min1);
            line.Y *= (float)(max2 - min1);
            return line;
        }

        /// <summary>
        /// Provides a string representation of the polygon.
        /// </summary>
        /// <returns>The polygon represented as a string.</returns>
        public override string ToString()
        {
            string ret = "";
            foreach (Vector2 item in this.vertexList)
            {
                ret += "(" + item.X + "," + item.Y + "),";
            }

            return ret;
        }
    }
}
