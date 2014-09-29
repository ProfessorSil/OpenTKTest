using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKTest
{
    class Calc
    {
        /// <summary>
        /// Rotates the Point Clockwise around the Origin point 
        /// Only clockwise if y+ is down and x+ is right
        /// </summary>
        public static Vector2 RotateAround(Vector2 point, Vector2 origin, float radians)
        {
            Vector2 p = point - origin;

            double rotation = DirectionTo(origin, point) + radians;
            double test = MathHelper.RadiansToDegrees(rotation);

            Vector2 dX = new Vector2((float)Math.Cos(radians), (float)Math.Sin(radians));
            Vector2 dY = new Vector2((float)Math.Cos(radians + MathHelper.PiOver2), (float)Math.Sin(radians + MathHelper.PiOver2));

            p = (p.X * dX) + (p.Y * dY);

            p += origin;

            return p;
        }

        /// <summary>
        /// Returns the heading angle from 'from' to 'to' in radians
        /// </summary>
        public static double DirectionTo(Vector2 from, Vector2 to)
        {
            double dir = Math.Atan2(to.Y - from.Y, to.X - from.X);
            return (dir < 0) ? (dir + MathHelper.Pi * 2) : (dir);
        }

        /// <summary>
        /// Returns the distance between the two points
        /// </summary>
        public static float Distance(Vector2 p1, Vector2 p2)
        {
            return (float)Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
        /// <summary>
        /// Returns the distance squared between the two points
        /// Useful when efficiency is important
        /// </summary>
        public static float DistanceSquared(Vector2 p1, Vector2 p2)
        {
            return (float)(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
        /// <summary>
        /// Returns a point on the circle in the direction 'rotation' with radius
        /// </summary>
        /// <param name="rotation">In radians</param>
        /// <param name="radius">radius of the circle</param>
        /// <returns></returns>
        public static Vector2 CirclePoint(float rotation, float radius)
        {
            return new Vector2(radius * (float)Math.Cos(rotation), radius * (float)Math.Sin(rotation));
        }
        /// <summary>
        /// Finds the rotational gap between two angles
        /// Solves the 0 = 360 degree problem
        /// e.g. 1 degree and 359 degrees will return 2 degrees
        /// </summary>
        /// <param name="angle1"></param>
        /// <param name="angle2"></param>
        /// <param name="inRadians">Whether or not to work in radians</param>
        /// <returns>Difference from angle1 to angle2 (Not absolute value)</returns>
        public static float AngleDifference(float angle1, float angle2, bool inRadians = true)
        {
            float full = (inRadians ? MathHelper.Pi * 2 : 360);
            float a1 = angle1;
            float a2 = angle2;
            if (a1 > full)
                a1 -= full;
            if (a1 < 0)
                a1 += full;
            if (a2 > full)
                a2 -= full;
            if (a2 < 0)
                a2 += full;

            if (Math.Abs(a1 - a2) > full / 2f)
            {
                a2 -= full;
            }

            return a2 - a1;
        }
        /// <summary>
        /// Simple helper to return whether value falls inside bounds
        /// </summary>
        public static bool WithinBounds(float p1, float p2, float value)
        {
            return (Math.Min(p1, p2) < value && value < Math.Max(p1, p2));
        }
        /// <summary>
        /// Helper function to determine if two "bounded regions" on an axis intersect each other
        /// </summary>
        /// <param name="inclusive">Whether or not same ending points inflict and intersection</param>
        public static bool BoundsIntersect(float p1, float p2, float p3, float p4, bool inclusive = true)
        {
            float min1 = Math.Min(p1, p2);
            float max1 = Math.Max(p1, p2);
            float min2 = Math.Min(p3, p4);
            float max2 = Math.Max(p3, p4);

            if (inclusive)
            {
                return (min1 <= max2 && max1 >= min2);
            }
            else
            {
                return (min1 < max2 && max1 > min2);
            }
        }

        #region Longer Functions
        /// <summary>
        /// Does a continuous collision check between two Rectangles (one of them moving, other static)
        /// </summary>
        /// <param name="staticRec">The static rectangle</param>
        /// <param name="moveRec">The moving rectangle (with 'velocity')</param>
        /// <param name="velocity">Velocity of the moving rectangle</param>
        /// <param name="solution">The rectangle representing where moveRec should be at the end of the step</param>
        /// <param name="isPlatform">True = only collide with top of statRec</param>
        /// <returns>whether they collided</returns>
        public static bool SweepTest(RectangleF staticRec, RectangleF moveRec, Vector2 velocity, out RectangleF solution, bool isPlatform = false)
        {
            RectangleF startRec = moveRec;
            RectangleF endRec = new RectangleF(moveRec.X + velocity.X, moveRec.Y + velocity.Y, moveRec.Width, moveRec.Height);
            RectangleF unionRec = RectangleF.Union(startRec, endRec);

            if (!staticRec.IntersectsWith(unionRec))
            {
                // Static rectangle was nowhere inside the area we wanted to move through
                solution = endRec;
                return false;
            }

            #region JointX
            //displacement along the x-axis where the two rectangle will begin to be joined
            float jointX = 0;
            if (velocity.X > 0)
                jointX = staticRec.Left - startRec.Right;
            else
                //We make it negative because the displacement is in the -x direction
                jointX = -(startRec.Left - staticRec.Right);
            #endregion

            #region JointY
            //displacement along the y-axis where the two rectangle will begin to be joined
            float jointY = 0;
            if (velocity.Y > 0)
                jointY = staticRec.Top - startRec.Bottom;
            else
                //We make it negative because the displacement is in the -y direction
                jointY = -(startRec.Top - staticRec.Bottom);
            #endregion

            #region MaxJointTime
            //Find the 'times' at which the this joints occur
            //By time i mean 0-1 where 0 is begining of step and 1 is end of step with
            //full velocity added
            double jointTimeX = jointX / velocity.X;
            double jointTimeY = jointY / velocity.Y;

            if ((jointTimeX < 0 && jointTimeY < 0) || (jointTimeX > 1 && jointTimeY > 1))
            {
                //continuing this velocity will run into it eventually but not in this step
                //or the times are in the past (we are moving away from staticRec)
                solution = endRec;
                return false;
            }

            //This will hold the joint time that happens last/second
            //It will tell us the absolute latest they could be joined on both axises
            double maxJointTime;
            if (double.IsInfinity(jointTimeX) || double.IsInfinity(jointTimeY))
            {
                if (double.IsInfinity(jointTimeX) && !double.IsInfinity(jointTimeY))
                    maxJointTime = jointTimeY;
                else if (double.IsInfinity(jointTimeY) && !double.IsInfinity(jointTimeX))
                    maxJointTime = jointTimeX;
                else
                {
                    //Both joint times were infinite
                    //Usually happens if velocity was 0,0
                    solution = endRec;
                    return false;
                }
            }
            else
            {
                //Neither were infinite so take the greater one
                maxJointTime = Math.Max(jointTimeX, jointTimeY);
            }
            #endregion

            #region DisjointX
            //Now do the same process but for when they will 'disjoint'
            float disjointX = 0;
            if (velocity.X > 0)
                disjointX = staticRec.Right - startRec.Left;
            else
                disjointX = -(startRec.Right - staticRec.Left);
            #endregion

            #region DisjointY
            float disjointY = 0;
            if (velocity.Y > 0)
                disjointY = staticRec.Bottom - startRec.Top;
            else
                disjointY = -(startRec.Bottom - staticRec.Top);
            #endregion

            #region MinDisjointTime
            double disjointTimeX = disjointX / velocity.X;
            double disjointTimeY = disjointY / velocity.Y;

            //This time we want to know the earliest they will disjoint
            double minDisjointTime;
            if (double.IsInfinity(disjointTimeX) || double.IsInfinity(disjointTimeY))
            {
                if (double.IsInfinity(disjointTimeX) && !double.IsInfinity(disjointTimeY))
                    minDisjointTime = disjointTimeY;
                else if (double.IsInfinity(disjointTimeY) && !double.IsInfinity(disjointTimeX))
                    minDisjointTime = disjointTimeX;
                else
                {
                    //Both disjoint times were infinite
                    //Usually happens if velocity was 0,0
                    //We should never really hit this since it should 
                    //have been handled when going through jointTimes
                    solution = endRec;
                    return false;
                }
            }
            else
            {
                //Neither were infinite so take the lesser one
                minDisjointTime = Math.Min(disjointTimeX, disjointTimeY);
            }
            #endregion


            if (minDisjointTime < maxJointTime)
            {
                //One of the axises disjointed before both were joined
                //No overlap of joint periods from both axises
                solution = endRec;
                return false;
            }

            //We have a solution if we got here
            Vector2 solEndPosition = new Vector2(startRec.X, startRec.Y) + (velocity * (float)maxJointTime);

            if (isPlatform)
            {
                //We need to make sure it was a top collision
                #region Platform Handling
                if (jointTimeX > jointTimeY && !double.IsInfinity(jointTimeX))
                {
                    //We collided with either left or right face
                    solution = endRec;
                    return false;
                }

                if (jointTimeY > jointTimeX && !double.IsInfinity(jointTimeY) && velocity.Y < 0)
                {
                    //We collided with the bottom
                    solution = endRec;
                    return false;
                }

                if (double.IsInfinity(jointTimeY))
                {
                    solution = endRec;
                    return false;
                }
                if (double.IsInfinity(jointTimeX) && velocity.Y <= 0)
                {
                    solution = endRec;
                    return false;
                }
                #endregion
            }

            #region Rounding Solution
            //I added this so the edge values of the static and moving recs
            //will align if there was a collision
            //It helps when I want to know what blocks I am immediately next
            //to after the collision
            if (jointTimeX > jointTimeY && !double.IsInfinity(jointTimeX))
            {
                //Collision was in the x direction
                if (velocity.X > 0)
                    solEndPosition.X = staticRec.Left - startRec.Width;
                else if (velocity.X < 0)
                    solEndPosition.X = staticRec.Right;
            }
            if (jointTimeY > jointTimeX && !double.IsInfinity(jointTimeY))
            {
                //Collision was in the y direction
                if (velocity.Y > 0)
                    solEndPosition.Y = staticRec.Top - startRec.Height;
                else if (velocity.Y < 0)
                    solEndPosition.Y = staticRec.Bottom;
            }
            #endregion

            solution = new RectangleF(solEndPosition.X, solEndPosition.Y, startRec.Width, startRec.Height);
            return true;
        }

        /// <summary>
        /// Tests for intersection between two lines
        /// </summary>
        /// <param name="l1P1">First point of line 1</param>
        /// <param name="l1P2">Second point of line 1</param>
        /// <param name="l2P1">First point of line 2</param>
        /// <param name="l2P2">Second point of line 2</param>
        /// <param name="intersection">Point where the two lines intersect. Vector2.Zero if no intersection</param>
        /// <returns>wether it found an intersection</returns>
        public static bool LineVLine(Vector2 l1P1, Vector2 l1P2, Vector2 l2P1, Vector2 l2P2, out Vector2 intersection)
        {
            if (l1P1.X != l1P2.X)
            {
                //This is the formula this is based off of
                //E = B-A = ( Bx-Ax, By-Ay )
                //F = D-C = ( Dx-Cx, Dy-Cy ) 
                //P = ( -Ey, Ex )
                //h = ( (A-C) * P ) / ( F * P )

                Vector2 A = l1P1;
                Vector2 B = l1P2;
                Vector2 C = l2P1;
                Vector2 D = l2P2;
                Vector2 E = B - A;
                Vector2 F = D - C;
                Vector2 P = new Vector2(-E.Y, E.X);
                float H = Vector2.Dot((A - C), P) / Vector2.Dot(F, P);
                if (0 <= H && H <= 1)
                {
                    Vector2 ret = (l2P1 + (l2P2 - l2P1) * H);
                    float H2 = (ret.X - l1P1.X) / (l1P2.X - l1P1.X);
                    if (0 <= H2 && H2 <= 1)
                    {
                        intersection = ret;
                        return true;
                    }
                }
            }
            else
            {
                //This is a handler for when the slope of l1 is undefined
                //I can't remember what this if was for but removing it fixed some problems..
                if (l2P1.X == l2P2.X)
                {
                    //Parrallell lines 
                    if (l2P1.X == l1P1.X && BoundsIntersect(l2P1.Y, l2P2.Y, l1P1.Y, l1P2.Y, true))
                    {
                        if (WithinBounds(l1P1.Y, l1P2.Y, l2P1.Y))
                        {
                            intersection = l2P1;
                        }
                        else if (WithinBounds(l1P1.Y, l1P2.Y, l2P2.Y))
                        {
                            intersection = l2P2;
                        }
                        else
                        {
                            intersection = (l1P1 + l1P2) / 2f;
                        }
                        return true;
                    }
                }
                if (WithinBounds(l2P1.X, l2P2.X, l1P1.X))
                {
                    float height = ((float)(l2P2.Y - l2P1.Y) / (float)(l2P2.X - l2P1.X)) * (l1P1.X - l2P1.X);
                    height += l2P1.Y;
                    if (l1P1.Y <= height && l1P2.Y >= height)
                    {
                        intersection = new Vector2(l1P1.X, height);
                        return true;
                    }
                    else if (l1P1.Y >= height && l1P2.Y <= height)
                    {
                        intersection = new Vector2(l1P1.X, height);
                        return true;
                    }
                }
            }
            intersection = Vector2.Zero;
            return false;
        }

        /// <summary>
        /// Tests for intersection between a line and a rectangle
        /// </summary>
        /// <param name="rec">First point of line 1</param>
        /// <param name="l2P1">First point of line 2</param>
        /// <param name="l2P2">Second point of line 2</param>
        /// <param name="intersection">Point where the two intersect. Vector2.Zero if no intersection</param>
        /// <returns>wether it found an intersection</returns>
        public static bool LineVRectangle(RectangleF rec, Vector2 l1P1, Vector2 l1P2, out Vector2 intersection1, out Vector2? intersection2)
        {
            Vector2 rP1 = new Vector2(rec.X, rec.Y);
            Vector2 rP2 = new Vector2(rec.Right, rec.Y);
            Vector2 rP3 = new Vector2(rec.Right, rec.Bottom);
            Vector2 rP4 = new Vector2(rec.X, rec.Bottom);
            Vector2 inter = Vector2.Zero;
            intersection1 = Vector2.Zero;
            intersection2 = null;
            bool found1 = false;
            if (LineVLine(rP1, rP2, l1P1, l1P2, out inter))
            {
                if (!found1)
                {
                    intersection1 = inter;
                    found1 = true;
                }
                else
                {
                    intersection2 = inter;
                }
            }
            if (LineVLine(rP2, rP3, l1P1, l1P2, out inter))
            {
                if (!found1)
                {
                    intersection1 = inter;
                    found1 = true;
                }
                else
                {
                    intersection2 = inter;
                }
            }
            if (LineVLine(rP3, rP4, l1P1, l1P2, out inter))
            {
                if (!found1)
                {
                    intersection1 = inter;
                    found1 = true;
                }
                else
                {
                    intersection2 = inter;
                }
            }
            if (LineVLine(rP4, rP1, l1P1, l1P2, out inter))
            {
                if (!found1)
                {
                    intersection1 = inter;
                    found1 = true;
                }
                else
                {
                    intersection2 = inter;
                }
            }

            if (found1 && intersection2.HasValue)
            {
                if (Distance(intersection1, l1P1) > Distance(intersection2.Value, l1P1))
                {
                    Vector2 o = intersection1;
                    intersection1 = intersection2.Value;
                    intersection2 = o;
                }
            }

            if (found1)
                return true;
            else
                return false;
        }
        #endregion
    }
}
